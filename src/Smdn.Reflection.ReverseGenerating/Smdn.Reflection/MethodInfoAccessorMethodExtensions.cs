// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class MethodInfoAccessorMethodExtensions {
  public static bool IsPropertyAccessorMethod(this MethodInfo m)
    => TryGetPropertyFromAccessorMethod(m, out _);

  public static bool TryGetPropertyFromAccessorMethod(
    this MethodInfo accessor,
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out PropertyInfo? property
  )
  {
    if (accessor is null)
      throw new ArgumentNullException(nameof(accessor));

    const BindingFlags bindingFlags =
      BindingFlags.Instance |
      BindingFlags.Static |
      BindingFlags.Public |
      BindingFlags.NonPublic;

    property = accessor.DeclaringType?.GetProperties(bindingFlags)?.FirstOrDefault(p =>
      accessor == p.GetMethod || accessor == p.SetMethod
    );

    return property is not null;
  }

  public static bool IsEventAccessorMethod(this MethodInfo m)
    => m is null
      ? throw new ArgumentNullException(nameof(m))
      : m.DeclaringType?.GetEvents()?.FirstOrDefault(ev =>
          m == ev.AddMethod || m == ev.RemoveMethod
        ) != null;
}
