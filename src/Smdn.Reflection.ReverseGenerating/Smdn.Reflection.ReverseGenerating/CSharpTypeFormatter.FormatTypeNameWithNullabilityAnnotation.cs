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
  private static StringBuilder FormatTypeNameWithNullabilityAnnotation(
    NullabilityInfo target,
    StringBuilder builder,
    FormatTypeNameOptions options
  )
  {
    const string NullableAnnotationSyntaxString = "?";

    static string GetNullabilityAnnotation(NullabilityInfo target)
      => target.ReadState == NullabilityState.Nullable || target.WriteState == NullabilityState.Nullable
        ? NullableAnnotationSyntaxString
        : string.Empty;

    Type? byRefParameterType = null;

    if (target.Type.IsByRef && options.AttributeProvider is ParameterInfo p) {
      // retval/parameter modifiers
      if (p.IsIn)
        builder.Append("in ");
      else if (p.IsOut)
        builder.Append("out ");
      else /*if (p.IsRetval)*/
        builder.Append("ref ");

      byRefParameterType = target.Type.GetElementTypeOrThrow();
    }

    if (target.ElementType is not null) {
      // arrays
      return FormatTypeNameWithNullabilityAnnotation(target.ElementType, builder, options)
        .Append('[')
        .Append(',', target.Type.GetArrayRank() - 1)
        .Append(']')
        .Append(GetNullabilityAnnotation(target));
    }

    if (target.Type.IsPointer || (target.Type.IsByRef && options.AttributeProvider is not ParameterInfo))
      return builder.Append(FormatTypeNameCore(target.Type, options));

    var targetType = byRefParameterType ?? target.Type;

    if (IsValueTupleType(targetType)) {
      if (byRefParameterType is not null)
        // TODO: cannot get NullabilityInfo of generic type arguments from by-ref parameter type
        return builder.Append(FormatTypeNameCore(targetType, options));

      // special case for value tuples (ValueTuple<>)
      return FormatValueTupleType(target, builder, options)
        .Append(GetNullabilityAnnotation(target));
    }

    var isGenericTypeClosedOrDefinition =
      targetType.IsGenericTypeDefinition ||
      targetType.IsConstructedGenericType ||
      (targetType.IsGenericType && targetType.ContainsGenericParameters);
    string? nullabilityAnnotationForByRefParameter = null;

    if (Nullable.GetUnderlyingType(targetType) is Type nullableUnderlyingType) {
      if (byRefParameterType is not null)
        // note: if the by-ref parameter is Nullable<>, NullabilityState will not be Nullable
        nullabilityAnnotationForByRefParameter = NullableAnnotationSyntaxString;

      // nullable value types (Nullable<>)
      if (IsValueTupleType(nullableUnderlyingType)) {
        // special case for nullable value tuples (Nullable<ValueTuple<>>)
        return FormatValueTupleType(target, builder, options)
          .Append(GetNullabilityAnnotation(target))
          .Append(nullabilityAnnotationForByRefParameter);
      }
      else if (nullableUnderlyingType.IsGenericType) {
        // case for nullable generic value types (Nullable<GenericValueType<>>)
        return FormatNullableGenericValueType(target, builder, options)
          .Append(GetNullabilityAnnotation(target))
          .Append(nullabilityAnnotationForByRefParameter);
      }

      targetType = nullableUnderlyingType;
    }
    else if (isGenericTypeClosedOrDefinition) {
      // other generic types
      if (targetType == byRefParameterType) {
        // TODO: cannot get NullabilityInfo of generic type arguments from by-ref parameter type
        return builder.Append(FormatTypeNameCore(targetType, options));
      }
      else {
        return FormatClosedGenericTypeOrGenericTypeDefinition(target, builder, options)
          .Append(GetNullabilityAnnotation(target))
          .Append(nullabilityAnnotationForByRefParameter);
      }
    }

    if (options.TranslateLanguagePrimitiveType && IsLanguagePrimitiveType(targetType, out var n)) {
      // language primitive types
      return builder
        .Append(n)
        .Append(GetNullabilityAnnotation(target))
        .Append(nullabilityAnnotationForByRefParameter);
    }

    if (targetType.IsGenericParameter && targetType.HasGenericParameterNoConstraints())
      // generic parameter which has no constraints must not have nullability annotation
      return builder.Append(GetTypeName(targetType, options));

    if (!targetType.IsGenericParameter) {
      if (targetType.IsNested && options.WithDeclaringTypeName)
        builder.Append(FormatTypeNameCore(targetType.GetDeclaringTypeOrThrow(), options)).Append('.');
      else if (options.TypeWithNamespace)
        builder.Append(targetType.Namespace).Append('.');
    }

    return builder
      .Append(GetTypeName(targetType, options))
      .Append(GetNullabilityAnnotation(target))
      .Append(nullabilityAnnotationForByRefParameter);
  }

  private static StringBuilder FormatClosedGenericTypeOrGenericTypeDefinition(
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

        builder.Append(FormatTypeNameCore(declaringType, options)).Append('.');
      }

      genericTypeArguments = genericTypeArguments.Skip(genericArgsOfDeclaringType.Length);
    }

    builder.Append(GetTypeName(target.Type, options));

    if (genericTypeArguments.Any()) {
      builder.Append('<');

      foreach (var (arg, i) in genericTypeArguments.Select(static (arg, i) => (arg, i))) {
        if (0 < i)
          builder.Append(", ");

        FormatTypeNameWithNullabilityAnnotation(arg, builder, options);
      }

      builder.Append('>');
    }

    return builder;
  }

  private static StringBuilder FormatValueTupleType(
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

    return builder.Append(')');
  }

  private static StringBuilder FormatNullableGenericValueType(
    NullabilityInfo target,
    StringBuilder builder,
    FormatTypeNameOptions options
  )
  {
    if (options.TypeWithNamespace && !target.Type.IsNested)
      builder.Append(target.Type.Namespace).Append('.');

    builder
      .Append(
        GetTypeName(
          target.Type.GenericTypeArguments[0], // the type of GenericValueType of Nullable<GenericValueType<>>
          options
        )
      )
      .Append('<');

    for (var i = 0; i < target.GenericTypeArguments.Length; i++) {
      if (0 < i)
        builder.Append(", ");

      FormatTypeNameWithNullabilityAnnotation(target.GenericTypeArguments[i], builder, options);
    }

    return builder.Append('>');
  }
}
#endif // SYSTEM_REFLECTION_NULLABILITYINFO
