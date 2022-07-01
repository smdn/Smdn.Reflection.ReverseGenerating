// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class TypeInheritedTypeExtensions {
  public static bool IsHidingInheritedType(this Type t)
  {
    if (t is null)
      throw new ArgumentNullException(nameof(t));
    if (!t.IsNested)
      return false;

    return EnumerateTypeHierarchy(t.DeclaringType!)
      .SelectMany(static th => th.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
      .Any(ti => ti.Name == t.Name);

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
}
