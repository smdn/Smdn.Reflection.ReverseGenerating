// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class MethodInfoAccessorMethodExtensions {
  public static bool IsPropertyAccessorMethod(this MethodInfo m)
    => m is null
      ? throw new ArgumentNullException(nameof(m))
      : m.DeclaringType?.GetProperties()?.FirstOrDefault(accessor =>
          m == accessor.GetMethod || m == accessor.SetMethod
        ) != null;

  public static bool IsEventAccessorMethod(this MethodInfo m)
    => m is null
      ? throw new ArgumentNullException(nameof(m))
      : m.DeclaringType?.GetEvents()?.FirstOrDefault(accessor =>
          m == accessor.AddMethod || m == accessor.RemoveMethod
        ) != null;
}
