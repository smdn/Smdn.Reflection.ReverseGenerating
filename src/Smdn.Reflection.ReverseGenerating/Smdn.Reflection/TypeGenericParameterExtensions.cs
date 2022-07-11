// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
#define SYSTEM_TYPE_ISGENERICMETHODPARAMETER
#endif

using System;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class TypeGenericParameterExtensions {
  private static void ValidateGenericParameterArgument(Type param, string paramName)
  {
    if (param is null)
      throw new ArgumentNullException(paramName);
    if (!param.IsGenericParameter)
      throw new InvalidOperationException($"{paramName} must be a generic parameter or generic argument");
  }

  // ref: https://github.com/dotnet/roslyn/blob/main/docs/features/nullable-metadata.md#type-parameters
  public static bool HasGenericParameterNotNullConstraint(this Type genericParameter)
  {
    ValidateGenericParameterArgument(genericParameter, nameof(genericParameter));

    var attrNullable = genericParameter.CustomAttributes.FirstOrDefault(IsNullableAttribute);
    var attrNullableContext = FindNullableContextAttribute(genericParameter);

    const byte notAnnotated = 1;

    if (attrNullableContext is not null && notAnnotated.Equals(attrNullableContext.ConstructorArguments[0].Value))
      // `#nullable enable` context
      return attrNullable is null;
    else
      // `#nullable disable` context
      return attrNullable is not null && notAnnotated.Equals(attrNullable.ConstructorArguments[0].Value);
  }

  private static bool IsNullableAttribute(CustomAttributeData attr)
    => "System.Runtime.CompilerServices.NullableAttribute".Equals(attr.AttributeType.FullName, StringComparison.Ordinal);

  private static bool IsNullableContextAttribute(CustomAttributeData attr)
    => "System.Runtime.CompilerServices.NullableContextAttribute".Equals(attr.AttributeType.FullName, StringComparison.Ordinal);

  private static bool TryGetNullableContextAttribute(
    MemberInfo member,
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [NotNullWhen(true)]
#endif
    out CustomAttributeData? attrNullableContext
  )
  {
    attrNullableContext = member.CustomAttributes.FirstOrDefault(IsNullableContextAttribute);

    return attrNullableContext is not null;
  }

  private static CustomAttributeData? FindNullableContextAttribute(Type genericParameter)
  {
    if (
#if SYSTEM_TYPE_ISGENERICMETHODPARAMETER
      genericParameter.IsGenericMethodParameter &&
#else
      genericParameter.DeclaringMethod is not null &&
      genericParameter.IsGenericParameter &&
#endif
      TryGetNullableContextAttribute(genericParameter.DeclaringMethod!, out var methodAttr)
    ) {
      return methodAttr;
    }

    Type? t = genericParameter;

    for (; ; ) {
      if ((t = t!.DeclaringType) is null)
        return null;
      if (TryGetNullableContextAttribute(t, out var typeAttr))
        return typeAttr;

      // retry against to the outer type
      continue;
    }
  }

  public static bool HasGenericParameterNoConstraints(this Type genericParameter)
  {
    ValidateGenericParameterArgument(genericParameter, nameof(genericParameter));

    var genericParameterConstraintAttrs = genericParameter.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;

    if (genericParameterConstraintAttrs != GenericParameterAttributes.None)
      return false;

    return !HasGenericParameterNotNullConstraint(genericParameter);
  }
}
