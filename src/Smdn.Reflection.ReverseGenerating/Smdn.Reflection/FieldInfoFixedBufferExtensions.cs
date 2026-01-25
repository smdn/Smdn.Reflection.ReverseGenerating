// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Smdn.Reflection;

internal static class FieldInfoFixedBufferExtensions {
  public static bool IsFixedBuffer(this FieldInfo f)
    => TryGetFixedBufferAttributeArgsCore(f, getConstructorArgs: false, out _, out _);

  public static bool TryGetFixedBufferAttributeArgs(
    this FieldInfo f,
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out Type? elementType,
    out int length
  )
    => TryGetFixedBufferAttributeArgsCore(f, getConstructorArgs: true, out elementType, out length);

  private static bool TryGetFixedBufferAttributeArgsCore(
    FieldInfo field,
    bool getConstructorArgs,
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out Type? elementType,
    out int length
  )
  {
    elementType = default;
    length = default;

    if (field.IsStatic)
      return false;
    if (field.IsInitOnly)
      return false;
    if (field.IsLiteral)
      return false;

    var attrFixedBuffer = field.GetCustomAttributesData().FirstOrDefault(
      static d => string.Equals(typeof(FixedBufferAttribute).FullName, d.AttributeType.FullName, StringComparison.Ordinal)
    );

    var hasFixedBufferAttribute = attrFixedBuffer is not null;

    if (hasFixedBufferAttribute && getConstructorArgs) {
#if SYSTEM_INDEX
      if (attrFixedBuffer!.ConstructorArguments is [var argElementType, var argLength]) {
#else
      if (attrFixedBuffer!.ConstructorArguments.Count == 2) {
        var argElementType = attrFixedBuffer.ConstructorArguments[0];
        var argLength = attrFixedBuffer.ConstructorArguments[1];
#endif
        if (argElementType.ArgumentType == typeof(Type) && argElementType.Value is Type argElementTypeValue)
          elementType = argElementTypeValue;
        else
          hasFixedBufferAttribute = false; // has an unexpected arg

        if (argLength.ArgumentType == typeof(int) && argLength.Value is int argLengthValue)
          length = argLengthValue;
        else
          hasFixedBufferAttribute = false; // has an unexpected arg
      }
      else {
        hasFixedBufferAttribute = false; // has unexpected args
      }
    }

    return hasFixedBufferAttribute;
  }
}
