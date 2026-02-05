// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Smdn.Reflection.ReverseGenerating;

internal static class CSharpMemberModifierFormatter {
  internal static void Append(
    StringBuilder sb,
    PropertyInfo p,
    bool asExplicitInterfaceMember,
    GeneratorOptions options,
    out string setMethodAccessibility,
    out string getMethodAccessibility
  )
  {
    setMethodAccessibility = string.Empty;
    getMethodAccessibility = string.Empty;

    Accessibility? propertyGetMethodAccessibility = null;
    Accessibility? propertySetMethodAccessibility = null;

    try {
      if (p.IsHidingInheritedMember(nonPublic: true))
        sb.Append("new ");
    }
    catch (TypeLoadException) {
      // FIXME: https://github.com/smdn/Smdn.Reflection.ReverseGenerating/issues/31
    }

    var mostOpenAccessibility = p.GetAccessors(true).Select(Smdn.Reflection.MemberInfoExtensions.GetAccessibility).Max();

    if (!asExplicitInterfaceMember && options.MemberDeclaration.WithAccessibility)
      AppendAccessibility(sb, p, mostOpenAccessibility);

    if (p.GetMethod != null) {
      var accessorAccessibility = p.GetMethod.GetAccessibility();

      if (accessorAccessibility < mostOpenAccessibility)
        propertyGetMethodAccessibility = accessorAccessibility;
    }

    if (p.SetMethod != null) {
      var accessorAccessibility = p.SetMethod.GetAccessibility();

      if (accessorAccessibility < mostOpenAccessibility)
        propertySetMethodAccessibility = accessorAccessibility;
    }

    if (p.GetAccessors(true).FirstOrDefault() is { } accessor)
      AppendMethodModifiers(sb, accessor);

    if (p.IsRequired())
      sb.Append("required ");
    else if (p.IsAccessorReadOnly())
      sb.Append("readonly ");

    if (propertyGetMethodAccessibility.HasValue)
      getMethodAccessibility = CSharpFormatter.FormatAccessibility(propertyGetMethodAccessibility.Value);
    if (propertySetMethodAccessibility.HasValue)
      setMethodAccessibility = CSharpFormatter.FormatAccessibility(propertySetMethodAccessibility.Value);
  }

  internal static void Append(
    StringBuilder sb,
    MemberInfo member,
    bool asExplicitInterfaceMember,
    GeneratorOptions options
  )
  {
    try {
      if (member.IsHidingInheritedMember(nonPublic: true))
        sb.Append("new ");
    }
    catch (TypeLoadException) {
      // FIXME: https://github.com/smdn/Smdn.Reflection.ReverseGenerating/issues/31
    }

  SWITCH_MEMBER_TYPE:
    switch (member) {
      case EventInfo ev:
        member = ev.GetMethods(true).First();
        goto SWITCH_MEMBER_TYPE;

      case FieldInfo f:
        if (!asExplicitInterfaceMember && options.MemberDeclaration.WithAccessibility)
          AppendAccessibility(sb, f, f.GetAccessibility());

        AppendFieldModifier(sb, f);

        break;

#if DEBUG
      case PropertyInfo:
        throw new InvalidOperationException();
#endif

      case MethodBase m:
        if (!asExplicitInterfaceMember && m != m.DeclaringType?.TypeInitializer) {
          if (m is MethodInfo method && method.IsDelegateSignatureMethod()) {
            if (options.TypeDeclaration.WithAccessibility)
              AppendAccessibility(sb, null, m.GetDeclaringTypeOrThrow().GetAccessibility());
          }
          else {
            if (options.MemberDeclaration.WithAccessibility)
              AppendAccessibility(sb, m, m.GetAccessibility());
          }
        }

        AppendMethodModifiers(sb, m);

        break;
    }
  }

  private static void AppendAccessibility(StringBuilder sb, MemberInfo? member, Accessibility accessibility)
  {
    var isInterfaceMember = member is not null && member.GetDeclaringTypeOrThrow().IsInterface;

    if (accessibility == Accessibility.Public && isInterfaceMember)
      return;

    sb.Append(CSharpFormatter.FormatAccessibility(accessibility)).Append(' ');
  }

  // TODO: extern
  private static void AppendMethodModifiers(
    StringBuilder sb,
    MethodBase m
  )
  {
    var method = m as MethodInfo;

    if (method is null || !method.IsDelegateSignatureMethod()) {
      var isInterfaceMethod = m.GetDeclaringTypeOrThrow().IsInterface;

      if (m.IsStatic)
        sb.Append("static ");

      if (m.IsAbstract) {
        if (isInterfaceMethod) {
          if (m.IsStatic)
            sb.Append("abstract ");
        }
        else {
          sb.Append("abstract ");
        }
      }
      else if (!isInterfaceMethod && method is not null && method.IsOverride()) {
        if (m.IsFinal)
          sb.Append("sealed ");

        sb.Append("override ");
      }
      else if (m.IsVirtual && !m.IsFinal) {
        sb.Append("virtual ");
      }
      else if (
        m is MethodInfo methodMayBeAccessor &&
        methodMayBeAccessor.TryGetPropertyFromAccessorMethod(out var p) &&
#if !NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
#pragma warning disable CS8602
#endif
        p.GetAccessors(nonPublic: true).Any(static a => a.IsVirtual && !a.IsFinal)
#pragma warning restore CS8602
      ) {
        sb.Append("virtual ");
      }
    }

    if (method is not null) {
      if (!method.IsPropertyAccessorMethod() && method.IsReadOnly())
        sb.Append("readonly ");
      if (method.IsAsyncStateMachine())
        sb.Append("async ");

      try {
        if (method.GetParameters().Any(IsParameterUnsafe))
          sb.Append("unsafe ");
      }
      catch (TypeLoadException) {
        // FIXME: https://github.com/smdn/Smdn.Reflection.ReverseGenerating/issues/31
      }
    }
  }

  private static bool IsParameterUnsafe(ParameterInfo p)
  {
    if (p.ParameterType.IsPointer)
      return true;
    if (p.ParameterType.IsByRef && p.ParameterType.HasElementType && p.ParameterType.GetElementType()!.IsPointer)
      return true;

    return false;
  }

  private static void AppendFieldModifier(
    StringBuilder sb,
    FieldInfo f
  )
  {
    if (f.IsStatic && !f.IsLiteral)
      sb.Append("static ");

    if (f.IsInitOnly || f.IsLiteral) {
      if (f.IsInitOnly)
        sb.Append("readonly ");
      else // f.IsLiteral
        sb.Append("const ");
    }
    else if (f.GetRequiredCustomModifiers().Any(static t => t == typeof(IsVolatile))) {
      sb.Append("volatile ");
    }

    if (f.IsRequired())
      sb.Append("required ");
  }
}
