// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cSpell:ignore retval
using System;
using System.Reflection;
using System.Text;

namespace Smdn.Reflection.ReverseGenerating;

#pragma warning disable IDE0040
static partial class CSharpTypeNameFormatter {
#pragma warning restore IDE0040
  private static StringBuilder Format(
    Type type,
    StringBuilder builder,
    in CSharpTypeNameFormatOptions options
  )
  {
    if (type.IsByRef) {
      if (ReferenceEquals(options.AttributeProvider, type))
        return Format(type.GetElementTypeOrThrow(), builder, options).Append('&');

      _ = CSharpFormatter.AppendByRefModifier(options.AttributeProvider, builder);

      type = type.GetElementTypeOrThrow();
    }

    if (type.IsArray) {
      return FormatArrayType(
        typeOfArray: type,
        typeOfElement: type.GetElementTypeOrThrow(),
        format: Format,
        builder: builder,
        options: options
      );
    }

    if (type.IsPointer)
      return Format(type.GetElementTypeOrThrow(), builder, options).Append('*');

    if (type.TryGetNullableUnderlyingType(out var nullableUnderlyingType)) {
#pragma warning disable SA1114
      return Format(
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
        nullableUnderlyingType,
#else
        nullableUnderlyingType!,
#endif
        builder,
        options
      ).Append('?');
#pragma warning restore SA1114
    }

    if (type.IsGenericParameter) {
      if (
        options.WithGenericParameterVariance &&
        type.ContainsGenericParameters
#if NETFRAMEWORK
        &&
        // disables displaying variances for the type parameters used as
        // types for field, event, property, or parameter
        options.AttributeProvider is Type
#endif
      ) {
        builder.Append(
          (type.GenericParameterAttributes & GenericParameterAttributes.VarianceMask) switch {
            GenericParameterAttributes.Contravariant => "in ",
            GenericParameterAttributes.Covariant => "out ",
            _ => null,
          }
        );
      }

      return builder.Append(type.Name);
    }

    if (IsGenericConstructedOrDefinition(type)) {
      if (CSharpFormatter.IsLanguagePrimitiveValueTupleType(type)) {
        // special case for value tuples (ValueTuple<>)
        return FormatValueTupleType(
          type.GetGenericArguments(),
          Format,
          builder,
          options
        );
      }

      return FormatClosedGenericTypeOrGenericTypeDefinition(
        type: type,
        genericTypeArguments: type.GetGenericArguments(),
        format: (arg, sb, in opts) => {
          if (type.IsGenericTypeDefinition)
            opts.PrependGenericParameterAttributes?.Invoke(arg, sb);

          return Format(arg, sb, opts);
        },
        builder: builder,
        options: options
      );
    }

    if (
      options.TranslateLanguagePrimitiveType &&
      CSharpFormatter.IsLanguagePrimitiveType(type, out var n)
    ) {
      return builder.Append(n);
    }

    if (options.WithDeclaringTypeName && type.IsNested)
      _ = Format(type.GetDeclaringTypeOrThrow(), builder, options).Append('.');
    else if (options.WithNamespace)
      builder.Append(type.Namespace).Append('.');

    return builder.AppendTypeName(type, options);
  }
}
