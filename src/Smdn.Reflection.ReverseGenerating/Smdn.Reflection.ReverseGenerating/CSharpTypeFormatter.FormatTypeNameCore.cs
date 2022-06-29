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
  private static bool IsValueTupleType(Type t)
    =>
      t.IsConstructedGenericType &&
      1 < t.GetGenericArguments().Length && // except single element tuples
      "System".Equals(t.Namespace, StringComparison.Ordinal) &&
      t.GetGenericTypeName().Equals("ValueTuple", StringComparison.Ordinal);

  internal static string FormatTypeNameCore(
    Type t,
    FormatTypeNameOptions options
  )
  {
    return FormatCore(t, showVariance: false, options);

    static string FormatCore(Type t, bool showVariance, FormatTypeNameOptions options)
    {
      if (t.IsArray) {
        return string.Concat(
          FormatCore(t.GetElementTypeOrThrow(), showVariance: false, options),
          "[",
          new string(',', t.GetArrayRank() - 1),
          "]"
        );
      }

      if (t.IsByRef) {
        var typeName = FormatCore(t.GetElementTypeOrThrow(), showVariance: false, options);

        if (options.AttributeProvider is ParameterInfo p) {
          // if (p.IsRetval)
          //  return "ref " + typeName;
          if (p.IsIn)
            return "in " + typeName;
          else if (p.IsOut)
            return "out " + typeName;
          else
            return "ref " + typeName;
        }
        else {
          return typeName + "&";
        }
      }

      if (t.IsPointer)
        return FormatCore(t.GetElementTypeOrThrow(), showVariance: false, options) + "*";

      var nullableUnderlyingType = Nullable.GetUnderlyingType(t);

      if (nullableUnderlyingType != null)
        return FormatCore(nullableUnderlyingType, showVariance: false, options) + "?";

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

        if (IsValueTupleType(t)) {
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
                    FormatCore(arg, showVariance: true, options),
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

              sb.Append(FormatCore(declaringType, showVariance: true, options)).Append('.');
            }

            genericArgs = genericArgs.Skip(genericArgsOfDeclaringType.Length);
          }

          sb.Append(t.GetGenericTypeName());

          var formattedGenericArgs = string.Join(
            ", ",
            genericArgs.Select(arg => {
              var name = FormatCore(arg, showVariance: true, options);

              if (t.IsGenericTypeDefinition && options.GenericParameterNameModifier is not null)
                name = options.GenericParameterNameModifier(arg, name);

              return name;
            })
          );

          if (0 < formattedGenericArgs.Length)
            sb.Append('<').Append(formattedGenericArgs).Append('>');
        }

        return sb.ToString();
      }

      if (options.TranslateLanguagePrimitiveType && IsLanguagePrimitiveType(t, out var n))
        return n;

      if (options.WithDeclaringTypeName && t.IsNested)
        return FormatCore(t.GetDeclaringTypeOrThrow(), showVariance, options) + "." + t.Name;
      if (options.TypeWithNamespace)
        return t.Namespace + "." + t.Name;

      return t.Name;
    }
  }
}
