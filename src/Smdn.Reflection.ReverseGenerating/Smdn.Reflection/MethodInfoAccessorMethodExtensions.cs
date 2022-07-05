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
      : m.DeclaringType?.GetProperties()?.FirstOrDefault(p =>
          m == p.GetMethod || m == p.SetMethod
        ) != null;

  public static bool IsEventAccessorMethod(this MethodInfo m)
    => m is null
      ? throw new ArgumentNullException(nameof(m))
      : m.DeclaringType?.GetEvents()?.FirstOrDefault(ev =>
          m == ev.AddMethod || m == ev.RemoveMethod
        ) != null;
}
