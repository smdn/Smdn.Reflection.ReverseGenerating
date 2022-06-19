// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
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
  private readonly /*ref*/ struct FormatTypeNameOptions {
    public readonly ICustomAttributeProvider AttributeProvider;
    public readonly bool TypeWithNamespace;
    public readonly bool WithDeclaringTypeName;
    public readonly bool TranslateLanguagePrimitiveType;

    public FormatTypeNameOptions(
      ICustomAttributeProvider attributeProvider,
      bool typeWithNamespace,
      bool withDeclaringTypeName,
      bool translateLanguagePrimitiveType
    )
    {
      this.AttributeProvider = attributeProvider;
      this.TypeWithNamespace = typeWithNamespace;
      this.WithDeclaringTypeName = withDeclaringTypeName;
      this.TranslateLanguagePrimitiveType = translateLanguagePrimitiveType;
    }
  }

  private static string FormatTypeNameCore(
    Type t,
    bool showVariance,
    FormatTypeNameOptions options
  )
  {
    if (t.IsArray) {
      return string.Concat(
        FormatTypeNameCore(t.GetElementTypeOrThrow(), showVariance: false, options),
        "[",
        new string(',', t.GetArrayRank() - 1),
        "]"
      );
    }

    if (t.IsByRef)
      return FormatTypeNameCore(t.GetElementTypeOrThrow(), showVariance: false, options) + "&";

    if (t.IsPointer)
      return FormatTypeNameCore(t.GetElementTypeOrThrow(), showVariance: false, options) + "*";

    var nullableUnderlyingType = Nullable.GetUnderlyingType(t);

    if (nullableUnderlyingType != null)
      return FormatTypeNameCore(nullableUnderlyingType, showVariance: false, options) + "?";

    if (t.IsGenericParameter) {
      if (showVariance && t.ContainsGenericParameters) {
        var variance = t.GenericParameterAttributes & GenericParameterAttributes.VarianceMask;

        switch (variance) {
          case GenericParameterAttributes.Contravariant:
            return "in " + t.Name;
          case GenericParameterAttributes.Covariant:
            return "out " + t.Name;
        }
      }

      return t.Name;
    }

    if (t.IsGenericTypeDefinition || t.IsConstructedGenericType || (t.IsGenericType && t.ContainsGenericParameters)) {
      var sb = new StringBuilder();
      var isValueTuple =
        t.IsConstructedGenericType &&
        "System".Equals(t.Namespace, StringComparison.Ordinal) &&
        t.GetGenericTypeName().Equals("ValueTuple", StringComparison.Ordinal);

      if (isValueTuple) {
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

        sb.Append('(')
          .Append(
            string.Join(
              ", ",
              t
                .GetGenericArguments()
                .Select((arg, index) => string.Concat(
                  FormatTypeNameCore(arg, showVariance: true, options),
                  tupleItemNames is null ? null : " ", // append delimiter between type and name
                  tupleItemNames?[index].Value
                ))
            )
          )
          .Append(')');
      }
      else {
        if (options.TypeWithNamespace && !t.IsNested)
          sb.Append(t.Namespace).Append('.');

        IEnumerable<Type> genericArgs = t.GetGenericArguments();

        if (t.IsNested) {
          var declaringType = t.GetDeclaringTypeOrThrow();
          var genericArgsOfDeclaringType = declaringType.GetGenericArguments();

          if (options.WithDeclaringTypeName) {
            if (declaringType.IsGenericTypeDefinition)
              declaringType = declaringType.MakeGenericType(genericArgs.Take(genericArgsOfDeclaringType.Length).ToArray());

            sb.Append(FormatTypeNameCore(declaringType, showVariance: true, options)).Append('.');
          }

          genericArgs = genericArgs.Skip(genericArgsOfDeclaringType.Length);
        }

        sb.Append(t.GetGenericTypeName());

        var formattedGenericArgs = string.Join(", ", genericArgs.Select(arg => FormatTypeNameCore(arg, showVariance: true, options)));

        if (0 < formattedGenericArgs.Length)
          sb.Append('<').Append(formattedGenericArgs).Append('>');
      }

      return sb.ToString();
    }

    if (options.TranslateLanguagePrimitiveType && IsLanguagePrimitiveType(t, out var n))
      return n;

    if (options.WithDeclaringTypeName && t.IsNested)
      return FormatTypeNameCore(t.GetDeclaringTypeOrThrow(), showVariance, options) + "." + t.Name;
    if (options.TypeWithNamespace)
      return t.Namespace + "." + t.Name;

    return t.Name;
  }
}
