// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

internal static class ROCType {
  public static bool FullNameEquals(Type type, Type? t)
    => FullNameEquals(type, t, ignoreGenericTypeParameters: false);

  public static bool Equals(Type type, Type? t, bool ignoreGenericTypeParameters = false)
    =>
      t is not null &&
      FullNameEquals(type, t, ignoreGenericTypeParameters) &&
      string.Equals(type.Assembly.GetName().Name, t.Assembly.GetName().Name, StringComparison.Ordinal);

  private static bool FullNameEquals(Type type, Type? t, bool ignoreGenericTypeParameters)
  {
    if (t is null)
      return false;
    if (string.Equals(type.FullName, t.FullName, StringComparison.Ordinal))
      return true; // FullName's are equal
    if (!ignoreGenericTypeParameters)
      return false;

#pragma warning disable SA1114
    // exclude generic type parameter specifications to treat
    // GenericType`1[TValue] and GenericType`1 as equivalent
    return MemoryExtensions.Equals(
      // if Type.FullName is null, use the string returned by
      // Type.ToString() for comparison instead
      RemoveGenericTypeParameter(type.FullName ?? type.ToString()),
      RemoveGenericTypeParameter(t.FullName ?? t.ToString()),
      StringComparison.Ordinal
    );
#pragma warning restore SA1114
  }

  private static ReadOnlySpan<char> RemoveGenericTypeParameter(ReadOnlySpan<char> fullName)
  {
    var indexOfGenericParameterCount = fullName.IndexOf('`');

    if (indexOfGenericParameterCount < 0)
      return fullName;

    var offsetOfGenericParameters = fullName
      .Slice(indexOfGenericParameterCount)
      .IndexOf('[');

    if (offsetOfGenericParameters < 0)
      return fullName;

    return fullName.Slice(0, indexOfGenericParameterCount + offsetOfGenericParameters);
  }
}
