// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class ParameterInfoPropertyAccessorExtensions {
  private static bool IsPropertySetMethod(MethodInfo m)
    => m.IsSpecialName &&
      m.DeclaringType is not null &&
      m.DeclaringType.GetProperties(
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
      ).Any(p => p.SetMethod == m);

  public static bool IsPropertySetMethodParameter(this ParameterInfo p)
    => p is null
      ? throw new ArgumentNullException(nameof(p))
      : p.Member is MethodInfo m &&
        IsPropertySetMethod(m);
}
