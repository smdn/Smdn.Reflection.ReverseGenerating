// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if SYSTEM_REFLECTION_NULLABILITYINFO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Smdn.Reflection.Attributes;

namespace Smdn.Reflection.ReverseGenerating;

#pragma warning disable IDE0040
static partial class CSharpFormatter {
#pragma warning restore IDE0040
  private static readonly string NullableAnnotationSyntaxString = "?";

  private static string GetNullabilityAnnotation(NullabilityInfo target)
    => target.ReadState == NullabilityState.Nullable || target.WriteState == NullabilityState.Nullable
      ? NullableAnnotationSyntaxString
      : string.Empty;

  private static StringBuilder FormatTypeNameWithNullabilityAnnotation(
    NullabilityInfo target,
    StringBuilder builder,
    FormatTypeNameOptions options
  )
  {
    static bool IsValueTupleType(Type t)
      =>
        t.IsConstructedGenericType &&
        "System".Equals(t.Namespace, StringComparison.Ordinal) &&
        t.GetGenericTypeName().Equals("ValueTuple", StringComparison.Ordinal);

    if (target.ElementType is not null) {
      // arrays
      return FormatTypeNameWithNullabilityAnnotation(target.ElementType, builder, options)
        .Append('[')
        .Append(',', target.Type.GetArrayRank() - 1)
        .Append(']')
        .Append(GetNullabilityAnnotation(target));
    }

    var targetType = target.Type;

    if (targetType.IsByRef) {
      return builder
        .Append(FormatTypeNameCore(targetType.GetElementTypeOrThrow(), showVariance: false, options))
        .Append('&');
    }

    if (targetType.IsPointer) {
      return builder
        .Append(FormatTypeNameCore(targetType.GetElementTypeOrThrow(), showVariance: false, options))
        .Append('*');
    }

    if (IsValueTupleType(targetType) && 0 < target.GenericTypeArguments.Length)
      return FormatValueTuple(target, builder, options);

    var isGenericTypeClosedOrDefinition =
      targetType.IsGenericTypeDefinition ||
      targetType.IsConstructedGenericType ||
      (targetType.IsGenericType && targetType.ContainsGenericParameters);

    if (Nullable.GetUnderlyingType(targetType) is Type nullableUnderlyingType) {
      // nullable value types (Nullable<>)
      if (IsValueTupleType(nullableUnderlyingType) && 0 < target.GenericTypeArguments.Length)
        // special case for nullable value tuples (Nullable<ValueTuple<>>)
        return FormatValueTuple(target, builder, options);

      targetType = nullableUnderlyingType;
    }
    else if (isGenericTypeClosedOrDefinition) {
      // other generic types
      return FormatGenericTypeClosedOrDefinition(target, builder, options);
    }

    if (options.TranslateLanguagePrimitiveType && IsLanguagePrimitiveType(targetType, out var n))
      // language primitive types
      return builder.Append(n).Append(GetNullabilityAnnotation(target));

    if (options.TypeWithNamespace)
      builder.Append(targetType.Namespace).Append('.');

    if (options.WithDeclaringTypeName && targetType.IsNested)
      builder.Append(FormatTypeNameCore(targetType.GetDeclaringTypeOrThrow(), showVariance: false, options)).Append('.');

    return builder.Append(targetType.Name).Append(GetNullabilityAnnotation(target));
  }

  private static StringBuilder FormatGenericTypeClosedOrDefinition(
    NullabilityInfo target,
    StringBuilder builder,
    FormatTypeNameOptions options
  )
  {
    if (options.TypeWithNamespace && !target.Type.IsNested)
      builder.Append(target.Type.Namespace).Append('.');

    IEnumerable<NullabilityInfo> genericTypeArguments = target.GenericTypeArguments;

    if (target.Type.IsNested) {
      var declaringType = target.Type.GetDeclaringTypeOrThrow();
      var genericArgsOfDeclaringType = declaringType.GetGenericArguments();

      if (options.WithDeclaringTypeName) {
        if (declaringType.IsGenericTypeDefinition) {
          declaringType = declaringType.MakeGenericType(
            target.Type.GetGenericArguments().Take(genericArgsOfDeclaringType.Length).ToArray()
          );
        }

        builder.Append(FormatTypeNameCore(declaringType, showVariance: true, options)).Append('.');
      }

      genericTypeArguments = genericTypeArguments.Skip(genericArgsOfDeclaringType.Length);
    }

    builder.Append(target.Type.GetGenericTypeName());

    if (genericTypeArguments.Any()) {
      builder.Append('<');

      // omit declaring type name of generic type arguments
      var optionsForGenericTypeArguments = new FormatTypeNameOptions(
        attributeProvider: options.AttributeProvider,
        typeWithNamespace: options.TypeWithNamespace,
        withDeclaringTypeName: false,
        translateLanguagePrimitiveType: options.TranslateLanguagePrimitiveType
      );

      foreach (var (arg, i) in genericTypeArguments.Select(static (arg, i) => (arg, i))) {
        if (0 < i)
          builder.Append(", ");

        FormatTypeNameWithNullabilityAnnotation(arg, builder, optionsForGenericTypeArguments);
      }

      builder.Append('>');
    }

    return builder.Append(GetNullabilityAnnotation(target));
  }

  private static StringBuilder FormatValueTuple(
    NullabilityInfo target,
    StringBuilder builder,
    FormatTypeNameOptions options
  )
  {
    var tupleItemNames = options
      .AttributeProvider
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

    builder.Append('(');

    for (var i = 0; i < target.GenericTypeArguments.Length; i++) {
      if (0 < i)
        builder.Append(", ");

      FormatTypeNameWithNullabilityAnnotation(target.GenericTypeArguments[i], builder, options);

      if (tupleItemNames is not null)
        builder.Append(' ').Append(tupleItemNames[i].Value);
    }

    return builder.Append(')').Append(GetNullabilityAnnotation(target));
  }
}
#endif // SYSTEM_REFLECTION_NULLABILITYINFO