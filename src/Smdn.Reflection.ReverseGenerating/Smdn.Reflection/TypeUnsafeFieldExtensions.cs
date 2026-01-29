// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class TypeUnsafeFieldExtensions {
  public static bool HasUnsafeFields(this Type t)
    => t
      .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
      .Any(static f => f.FieldType.IsPointer || f.IsFixedBuffer());
}
