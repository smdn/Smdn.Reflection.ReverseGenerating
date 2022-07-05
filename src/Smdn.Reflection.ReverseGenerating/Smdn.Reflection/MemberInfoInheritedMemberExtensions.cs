// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class MemberInfoInheritedMemberExtensions {
  public static bool IsHidingInheritedMember(this MemberInfo member, bool nonPublic)
  {
    if (member is null)
      throw new ArgumentNullException(nameof(member));

    if (member is ConstructorInfo)
      return false; // constructors can not be 'new'

    var bindingFlagsVisibility = BindingFlags.Public;

    if (nonPublic)
      bindingFlagsVisibility |= BindingFlags.NonPublic;

    if (member is Type t) {
      if (!t.IsNested)
        return false; // non-nested type never hides any types

      // is hiding any nested type in type hierarchy?
      return EnumerateTypeHierarchy(t.DeclaringType!)
        .SelectMany(th => th.GetNestedTypes(bindingFlagsVisibility))
        .Any(ti => string.Equals(ti.Name, t.Name, StringComparison.Ordinal));

      static IEnumerable<Type> EnumerateTypeHierarchy(Type t)
      {
        Type? _t = t;

        for (; ; ) {
          if ((_t = _t?.BaseType) is not null)
            yield return _t;
          else
            break;
        }
      }
    }

    if (member.DeclaringType is null)
      return false; // XXX: ???

    var declaringType = member.DeclaringType;
    var baseTypeOrInterfaces = declaringType.IsInterface
      ? declaringType.GetInterfaces()
      : declaringType.BaseType is null
        ? Enumerable.Empty<Type>()
        : Enumerable.Repeat(declaringType.BaseType!, 1);
    var overriddenOrHiddenMembers = baseTypeOrInterfaces
      .SelectMany(t =>
        t.GetMember(
          member.Name,
          member.MemberType,
          bindingFlagsVisibility | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy
        )
      );

    return member switch {
      FieldInfo f => overriddenOrHiddenMembers
        .OfType<FieldInfo>()
        .Any(), // fields cannot be overridden, always 'new'

      MethodInfo m => overriddenOrHiddenMembers
        .OfType<MethodInfo>()
        .Any(mh => HasSameSignature(m, mh) && (declaringType.IsInterface || !m.IsOverridden())),

      EventInfo ev =>
        overriddenOrHiddenMembers.Any() &&
        ev.GetMethods(nonPublic: nonPublic).All(accessor => IsHidingInheritedMember(accessor, nonPublic)),

      PropertyInfo p =>
        overriddenOrHiddenMembers.Any() &&
        p.GetAccessors(nonPublic: nonPublic).All(accessor => IsHidingInheritedMember(accessor, nonPublic)),

      _ => false,
    };

    static bool HasSameSignature(MethodInfo x, MethodInfo y)
    {
      if (x.ReturnParameter.ParameterType != y.ReturnParameter.ParameterType)
        return false;

      var paramsX = x.GetParameters();
      var paramsY = y.GetParameters();

      if (paramsX.Length != paramsY.Length)
        return false;

      for (var i = 0; i < paramsX.Length; i++) {
        if (paramsX[i].ParameterType != paramsY[i].ParameterType)
          return false;
      }

      return true;
    }
  }
}
