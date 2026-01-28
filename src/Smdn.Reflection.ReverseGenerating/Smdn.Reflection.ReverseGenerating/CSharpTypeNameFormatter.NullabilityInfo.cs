// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cSpell:ignore retval
#if SYSTEM_REFLECTION_NULLABILITYINFO
using System;
using System.Reflection;
using System.Text;

namespace Smdn.Reflection.ReverseGenerating;

#pragma warning disable IDE0040
static partial class CSharpTypeNameFormatter {
#pragma warning restore IDE0040
#if DEBUG
  private static void ThrowIfTypeIsCompoundType(Type type, string name)
  {
    if (type.IsArray || type.IsPointer || type.IsByRef)
      throw new InvalidOperationException($"'{name}' is expected to be non-compound elemental type, but was compound: {type}");
  }
#endif

  private static StringBuilder AppendNullableTypeAnnotation(
    this StringBuilder builder,
    NullabilityInfo target
  )
  {
    if (target.ReadState == NullabilityState.Nullable)
      return builder.Append('?');
    if (target.WriteState == NullabilityState.Nullable)
      return builder.Append('?');

    return builder;
  }

  private static StringBuilder Format(
    NullabilityInfo target,
    StringBuilder builder,
    in CSharpTypeNameFormatOptions options
  )
  {
    var type = target.Type;

    if (type.IsByRef) {
      _ = CSharpFormatter.AppendByRefModifier(options.AttributeProvider, builder);

      type = type.GetElementType()!;
    }

    if (type.IsArray) {
      return FormatArrayType(
        typeOfArray: type,
        typeOfElement: target.ElementType!,
        format: Format,
        builder: builder,
        options: options
      ).AppendNullableTypeAnnotation(target);
    }

    if (type.IsPointer) // pointer types
      return Format(type, builder, options);

#if DEBUG
    // where 'type' should be an non-compound elemental type (i.e., the element type of a type of an array, pointer, or by-ref)
    ThrowIfTypeIsCompoundType(type, nameof(type));
#endif

    var nullableUnderlyingType = Nullable.GetUnderlyingType(type);

    if (
      CSharpFormatter.IsLanguagePrimitiveValueTupleType(type) ||
      (
        nullableUnderlyingType is not null &&
        CSharpFormatter.IsLanguagePrimitiveValueTupleType(nullableUnderlyingType)
      )
    ) {
      // special case for value tuples (ValueTuple<>)
      // or nullable value tuples (Nullable<ValueTuple<>>)
      return FormatValueTupleType(
        target.GenericTypeArguments,
        Format,
        builder,
        options
      ).AppendNullableTypeAnnotation(target);
    }

    if (nullableUnderlyingType is not null) {
      if (nullableUnderlyingType.IsGenericType)
        // case for nullable generic value types (Nullable<GenericValueType<>>)
        return FormatNullableGenericValueType(target, builder, options);

      type = nullableUnderlyingType;
    }
    else if (IsGenericConstructedOrDefinition(type)) {
      // other generic types
      return FormatClosedGenericTypeOrGenericTypeDefinition(
        type: target.Type.IsByRef
          ? target.Type.GetElementType()!
          : target.Type,
        genericTypeArguments: target.GenericTypeArguments,
        format: Format,
        builder: builder,
        options: options
      ).AppendNullableTypeAnnotation(target);
    }

    if (
      options.TranslateLanguagePrimitiveType &&
      CSharpFormatter.IsLanguagePrimitiveType(type, out var n)
    ) {
      // language primitive types
      return builder
        .Append(n)
        .AppendNullableTypeAnnotation(target);
    }

    if (type.IsGenericParameter && type.HasGenericParameterNoConstraints())
      // generic parameter which has no constraints must not have nullability annotation
      return builder.AppendTypeName(type, options);

    if (!type.IsGenericParameter) {
      if (type.IsNested && options.WithDeclaringTypeName)
        _ = Format(type.GetDeclaringTypeOrThrow(), builder, options).Append('.');
      else if (options.WithNamespace)
        builder.Append(type.Namespace).Append('.');
    }

    return builder
      .AppendTypeName(type, options)
      .AppendNullableTypeAnnotation(target);
  }

  private static StringBuilder FormatNullableGenericValueType(
    NullabilityInfo target,
    StringBuilder builder,
    in CSharpTypeNameFormatOptions options
  )
  {
    if (options.WithNamespace && !target.Type.IsNested)
      builder.Append(target.Type.Namespace).Append('.');

    return builder
      .AppendTypeName(
        target.Type.GenericTypeArguments[0], // the type of GenericValueType of Nullable<GenericValueType<>>
        options
      )
      .AppendGenericTypeArgumentList(
        target.GenericTypeArguments,
        Format,
        options
      )
      .AppendNullableTypeAnnotation(target);
  }
}
#endif // SYSTEM_REFLECTION_NULLABILITYINFO
