// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
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

  public static bool HasGenericParameterUnmanagedConstraint(this Type genericParameter)
  {
    ValidateGenericParameterArgument(genericParameter, nameof(genericParameter));

    return genericParameter.CustomAttributes.Any(
      static attr => string.Equals("System.Runtime.CompilerServices.IsUnmanagedAttribute", attr.AttributeType.FullName, StringComparison.Ordinal)
    );
  }

  // ref: https://github.com/dotnet/roslyn/blob/main/docs/features/nullable-metadata.md#type-parameters
  public static bool HasGenericParameterNotNullConstraint(this Type genericParameter)
  {
    ValidateGenericParameterArgument(genericParameter, nameof(genericParameter));

    if (genericParameter.GetNullableContextAttributeMetadataValue() == NullableMetadataValue.NotAnnotated)
      // `#nullable enable` context
      return genericParameter.GetNullableAttributeMetadataValue() == null;
    else
      // `#nullable disable` context
      return genericParameter.GetNullableAttributeMetadataValue() == NullableMetadataValue.NotAnnotated;
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
