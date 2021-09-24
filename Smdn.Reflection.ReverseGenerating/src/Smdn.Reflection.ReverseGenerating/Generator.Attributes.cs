// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Smdn.Reflection.Attributes;

namespace Smdn.Reflection.ReverseGenerating {
  partial class Generator {
    public static IEnumerable<string> GenerateAttributeList(
      ICustomAttributeProvider attributeProvider,
      ISet<string> referencingNamespaces,
      GeneratorOptions options,
      Func<Type, bool> attributeTypePredicate = null
    )
    {
      if (attributeProvider is null)
        throw new ArgumentNullException(nameof(attributeProvider));

      var prefix = (attributeProvider is ParameterInfo p && p.IsReturnParameter()) ? "[return: " : "[";

      return GetAttributes(attributeProvider, attributeTypePredicate)
        .OrderBy(attr => attr.GetAttributeType().FullName)
        .Select(attr =>
          (
            name: ConvertAttributeName(attr),
            args: string.Join(", ", ConvertAttributeArguments(attr))
          )
        )
        .Select(a => prefix + a.name + (string.IsNullOrEmpty(a.args) ? string.Empty : "(" + a.args + ")") + "]");

      static IEnumerable<CustomAttributeData> GetAttributes(ICustomAttributeProvider attributeProvider, Func<Type, bool> predicate)
      {
        foreach (var attr in attributeProvider.GetCustomAttributeDataList()) {
          if (predicate is not null && !predicate(attr.AttributeType))
            continue;

          if (attr.AttributeType == typeof(System.CLSCompliantAttribute))
            continue; // ignore
          if (attr.AttributeType == typeof(System.Reflection.DefaultMemberAttribute))
            continue; // ignore
          if (attributeProvider is MethodBase) {
            if (string.Equals("System.Runtime.CompilerServices.ExtensionAttribute", attr.AttributeType.FullName, StringComparison.Ordinal))
              continue; // ignore
          }

          if (string.Equals("System", attr.AttributeType.Namespace.Split('.')[0], StringComparison.Ordinal))
            yield return attr;
        }

        if (
          attributeProvider is Type t &&
          t.IsValueType &&
          !t.IsEnum &&
          !t.IsStructLayoutDefault()
        ) {
          // yield pseudo CustomAttributeData for StructLayoutAttribute
          yield return new StructLayoutCustomAttributeData(t.StructLayoutAttribute);
        }
      }

      string ConvertAttributeName(CustomAttributeData attr)
      {
        referencingNamespaces?.Add(attr.GetAttributeType().Namespace);

        var nameOfAttr = attr.GetAttributeType().FormatTypeName(typeWithNamespace: options.AttributeDeclaration.WithNamespace);

        if (nameOfAttr.EndsWith("Attribute", StringComparison.Ordinal))
          nameOfAttr = nameOfAttr.Substring(0, nameOfAttr.Length - 9);

        return nameOfAttr;
      }

      IEnumerable<string> ConvertAttributeArguments(CustomAttributeData attr)
      {
        foreach (var param in attr.Constructor.GetParameters()) {
          var arg = attr.ConstructorArguments[param.Position];

          if (options.AttributeDeclaration.WithNamedArguments) {
            yield return string.Concat(
              param.Name,
              ": ",
              ConvertAttributeTypedArgument(arg)
            );
          }
          else {
            yield return ConvertAttributeTypedArgument(arg);
          }
        }

        foreach (var namedArg in attr.NamedArguments) {
          yield return string.Concat(
            namedArg.MemberName,
            " = ",
            ConvertAttributeTypedArgument(namedArg.TypedValue)
          );
        }
      }

      string ConvertAttributeTypedArgument(CustomAttributeTypedArgument arg)
        => CSharpFormatter.FormatValueDeclaration(
          arg.GetTypedValue(),
          arg.ArgumentType,
          typeWithNamespace: options.TypeDeclaration.WithNamespace,
          findConstantField: true,
          useDefaultLiteral: options.ValueDeclaration.UseDefaultLiteral
        );
    }
  }
}