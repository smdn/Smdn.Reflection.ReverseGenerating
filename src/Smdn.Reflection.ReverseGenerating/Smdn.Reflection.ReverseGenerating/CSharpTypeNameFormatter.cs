// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cSpell:ignore retval
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Smdn.Reflection.Attributes;

namespace Smdn.Reflection.ReverseGenerating;

internal static partial class CSharpTypeNameFormatter {
  public static string Format(
    Type type,
    in CSharpTypeNameFormatOptions options
  )
  {
    var builder = new StringBuilder(capacity: 64); // TODO: best initial capacity

    return Format(type, builder, options).ToString();
  }

#if SYSTEM_REFLECTION_NULLABILITYINFO
  public static string Format(
    NullabilityInfo target,
    in CSharpTypeNameFormatOptions options
  )
  {
    var builder = new StringBuilder(capacity: 64); // TODO: best initial capacity

    return Format(target, builder, options).ToString();
  }
#endif

  private static bool IsGenericConstructedOrDefinition(Type type)
    =>
      type.IsGenericTypeDefinition ||
      type.IsConstructedGenericType ||
      (type.IsGenericType && type.ContainsGenericParameters);

  private static
  IReadOnlyList<CustomAttributeTypedArgument>?
  GetTupleItemNames(ICustomAttributeProvider? attributeProvider)
    => attributeProvider
      ?.GetCustomAttributeDataList()
      ?.FirstOrDefault(static d =>
        string.Equals(
          typeof(TupleElementNamesAttribute).FullName,
          d.AttributeType.FullName,
          StringComparison.Ordinal
        )
      )
      ?.ConstructorArguments
      ?.FirstOrDefault()
      .Value
      as IReadOnlyList<CustomAttributeTypedArgument>;

  private delegate StringBuilder FormatFunc<TTypeOrNullabilityInfo>(
    TTypeOrNullabilityInfo target,
    StringBuilder builder,
    in CSharpTypeNameFormatOptions options
  ) where TTypeOrNullabilityInfo : class;

  private static StringBuilder FormatArrayType<TTypeOrNullabilityInfo>(
    Type typeOfArray,
    TTypeOrNullabilityInfo typeOfElement,
    FormatFunc<TTypeOrNullabilityInfo> format,
    StringBuilder builder,
    in CSharpTypeNameFormatOptions options
  )
    where TTypeOrNullabilityInfo : class
    => format(typeOfElement, builder, options)
      .Append('[')
      .Append(',', typeOfArray.GetArrayRank() - 1)
      .Append(']');

  private static StringBuilder FormatValueTupleType<TTypeOrNullabilityInfo>(
    ReadOnlySpan<TTypeOrNullabilityInfo> genericTypeArguments,
    FormatFunc<TTypeOrNullabilityInfo> format,
    StringBuilder builder,
    in CSharpTypeNameFormatOptions options
  )
    where TTypeOrNullabilityInfo : class
    => builder
      .Append('(')
      .AppendGenericTypeArguments(
        genericTypeArguments: genericTypeArguments,
        tupleItemNames: GetTupleItemNames(options.AttributeProvider),
        format: format,
        options: options
      )
      .Append(')');

  private static
  StringBuilder
  FormatClosedGenericTypeOrGenericTypeDefinition<TTypeOrNullabilityInfo>(
    Type type,
    ReadOnlySpan<TTypeOrNullabilityInfo> genericTypeArguments,
    FormatFunc<TTypeOrNullabilityInfo> format,
    StringBuilder builder,
    in CSharpTypeNameFormatOptions options
  )
    where TTypeOrNullabilityInfo : class
  {
    if (options.WithNamespace && !type.IsNested)
      builder.Append(type.Namespace).Append('.');

    if (type.IsNested) {
      var declaringType = type.GetDeclaringTypeOrThrow();
      var genericArgsCountOfDeclaringType = declaringType.GetGenericArguments().Length;

      if (options.WithDeclaringTypeName) {
        if (declaringType.IsGenericTypeDefinition) {
          declaringType = declaringType.MakeGenericType(
            [.. type.GetGenericArguments().Take(genericArgsCountOfDeclaringType)]
#if false
            // ???: AsSpan() throws ArrayTypeMismatchException in reflection-only context
            [.. type.GetGenericArguments().AsSpan(0, genericArgsCountOfDeclaringType)]
#endif
          );
        }

        _ = Format(declaringType, builder, options).Append('.');
      }

      genericTypeArguments = genericTypeArguments.Slice(genericArgsCountOfDeclaringType);
    }

    builder.AppendTypeName(type, options);

    if (genericTypeArguments.IsEmpty)
      return builder;

    return builder.AppendGenericTypeArgumentList(
      genericTypeArguments,
      format,
      options
    );
  }

  private static
  StringBuilder
  AppendGenericTypeArgumentList<TTypeOrNullabilityInfo>(
    this StringBuilder builder,
    ReadOnlySpan<TTypeOrNullabilityInfo> genericTypeArguments,
    FormatFunc<TTypeOrNullabilityInfo> format,
    in CSharpTypeNameFormatOptions options
  )
    where TTypeOrNullabilityInfo : class
  {
    builder.Append('<');

    if (options.AsUnboundTypeName) {
      return builder
        .Append(',', repeatCount: genericTypeArguments.Length - 1)
        .Append('>');
    }

    return builder
      .AppendGenericTypeArguments(
        genericTypeArguments: genericTypeArguments,
        tupleItemNames: null,
        format: format,
        options: options
      )
      .Append('>');
  }

  private static
  StringBuilder
  AppendGenericTypeArguments<TTypeOrNullabilityInfo>(
    this StringBuilder builder,
    ReadOnlySpan<TTypeOrNullabilityInfo> genericTypeArguments,
    IReadOnlyList<CustomAttributeTypedArgument>? tupleItemNames,
    FormatFunc<TTypeOrNullabilityInfo> format,
    in CSharpTypeNameFormatOptions options
  )
    where TTypeOrNullabilityInfo : class
  {
    // TODO: consider using StringBuilder.AppendJoin() (net9.0 <=)
    for (var i = 0; i < genericTypeArguments.Length; i++) {
      if (0 < i)
        builder.Append(", ");

      _ = format(genericTypeArguments[i], builder, options);

      if (tupleItemNames?[i].Value is { } tupleItemName)
        builder.Append(' ').Append(tupleItemName);
    }

    return builder;
  }

  private static StringBuilder AppendTypeName(
    this StringBuilder builder,
    Type type,
    in CSharpTypeNameFormatOptions options
  )
  {
    var typeName = type.IsGenericType
      ? type.GetGenericTypeName()
      : type.Name;

    if (
      options.OmitAttributeSuffix &&
      typeName.EndsWith("Attribute", StringComparison.Ordinal) &&
      (typeof(Attribute).IsAssignableFrom(type) || IsAttributeType(type)) &&
      type != typeof(Attribute)
    ) {
      const int LengthOfAttributeSuffix = 9; // "Attribute".Length

      return builder.Append(
        typeName,
        0,
        typeName.Length - LengthOfAttributeSuffix
      );
    }

    return builder.Append(typeName);

    // alternative method to Type.IsAssignableTo(typeof(Attribute)) for the
    // reflection-only context types
    static bool IsAttributeType(Type maybeReflectionOnlyType)
    {
      var t = maybeReflectionOnlyType.BaseType;

      for (; ; ) {
        if (t is null)
          return false;
        if ("System.Attribute".Equals(t.FullName, StringComparison.Ordinal))
          return true;

        t = t.BaseType;
      }
    }
  }
}
