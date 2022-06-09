// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;

namespace Smdn.Reflection;

internal static class TypeElementTypeExtensions {
  public static Type GetElementTypeOrThrow(this Type t)
  {
    if (t is null)
      throw new ArgumentNullException(nameof(t));

    return t.GetElementType()
      ?? throw new InvalidOperationException($"can not get element type of {t}");
  }
}
