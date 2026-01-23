// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Smdn.Reflection;

internal static class FieldInfoFixedBufferExtensions {
  public static bool IsFixedBuffer(this FieldInfo f)
  {
    if (f.IsStatic)
      return false;
    if (f.IsInitOnly)
      return false;
    if (f.IsLiteral)
      return false;

    return f.GetCustomAttributesData().Any(
      static d => string.Equals(typeof(FixedBufferAttribute).FullName, d.AttributeType.FullName, StringComparison.Ordinal)
    );
  }
}
