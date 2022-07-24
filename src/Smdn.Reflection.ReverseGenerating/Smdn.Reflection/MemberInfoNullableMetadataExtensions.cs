// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class MemberInfoNullableMetadataExtensions {
  private static bool IsNullableAttribute(CustomAttributeData attr)
    => "System.Runtime.CompilerServices.NullableAttribute".Equals(attr.AttributeType.FullName, StringComparison.Ordinal);

  private static bool IsNullableContextAttribute(CustomAttributeData attr)
    => "System.Runtime.CompilerServices.NullableContextAttribute".Equals(attr.AttributeType.FullName, StringComparison.Ordinal);

  internal static NullableMetadataValue? GetNullableAttributeMetadataValue(this MemberInfo memberOrType)
  {
    var data = memberOrType.CustomAttributes.FirstOrDefault(IsNullableAttribute);

    if (data is null)
      return null;

    return (NullableMetadataValue?)(byte?)data.ConstructorArguments[0].Value;
  }

  internal static NullableMetadataValue? GetNullableContextAttributeMetadataValue(this MemberInfo memberOrType)
  {
    if (
      memberOrType is Type genericParameter &&
#if SYSTEM_TYPE_ISGENERICMETHODPARAMETER
      genericParameter.IsGenericMethodParameter &&
#else
      genericParameter.DeclaringMethod is not null &&
      genericParameter.IsGenericParameter &&
#endif
      GetMetadataValue(genericParameter.DeclaringMethod!, out var valueOfGenericParameter)
    ) {
      return valueOfGenericParameter;
    }

    MemberInfo? m = memberOrType;

    for (; ; ) {
      if (GetMetadataValue(m, out var value))
        return value;
      if ((m = m!.DeclaringType) is null)
        return null;

      // retry against to the outer type
      continue;
    }

    static bool GetMetadataValue(MemberInfo memberOrType, out NullableMetadataValue? value)
    {
      var data = memberOrType.CustomAttributes.FirstOrDefault(IsNullableContextAttribute);

      if (data is null) {
        value = null;
        return false;
      }

      value = (NullableMetadataValue?)(byte?)data.ConstructorArguments[0].Value;

      return value.HasValue;
    }
  }
}
