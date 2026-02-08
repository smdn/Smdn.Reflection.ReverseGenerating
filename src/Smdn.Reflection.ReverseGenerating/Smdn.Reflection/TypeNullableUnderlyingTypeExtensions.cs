// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif

namespace Smdn.Reflection;

internal static class TypeNullableUnderlyingTypeExtensions {
  public static bool IsConstructedNullableType(this Type t)
  {
    if (t is null)
      throw new ArgumentNullException(nameof(t));

    return TryGetNullableUnderlyingType(t, out _);
  }

  public static bool TryGetNullableUnderlyingType(
    this Type t,
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out Type? nullableUnderlyingType
  )
  {
    if (t is null)
      throw new ArgumentNullException(nameof(t));

    nullableUnderlyingType = default;

    if (!t.IsValueType)
      return false; // not a struct
    if (!t.IsGenericType)
      return false; // not a generic type
    if (t.IsGenericTypeDefinition)
      return false; // not a constructed generic type

    if (!"System".Equals(t.Namespace, StringComparison.Ordinal))
      return false; // not System.Nullable`1
    if (!"Nullable`1".Equals(t.Name, StringComparison.Ordinal))
      return false; // not System.Nullable`1

    var genericArgs = t.GetGenericArguments();

    if (genericArgs.Length != 1)
      return false; // not System.Nullable`1

    nullableUnderlyingType = genericArgs[0]; // T of System.Nullable<T>

    return true;
  }
}
