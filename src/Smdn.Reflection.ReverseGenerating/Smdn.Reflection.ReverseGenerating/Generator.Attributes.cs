// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
#if SYSTEM_DIAGNOSTICS_UNREACHABLEEXCEPTION
using System.Diagnostics;
#endif
using System.Linq;
using System.Reflection;

using Smdn.Reflection.Attributes;

namespace Smdn.Reflection.ReverseGenerating;

#pragma warning disable IDE0040
partial class Generator {
#pragma warning restore IDE0040
  private enum AttributeTarget {
    Default,
    PropertyAccessorMethod,
    PropertyGetMethodReturnParameter,
    PropertySetMethodParameter,
    PropertyBackingField,
    EventAccessorMethod,
    EventAccessorMethodParameter,
    EventBackingField,
    GenericParameter,
    MethodParameter,
    MethodReturnParameter,
    DelegateParameter,
    DelegateReturnParameter,
  }

#pragma warning disable CA1502, CA1506 // TODO: reduce code complexity
  public static IEnumerable<string> GenerateAttributeList(
    ICustomAttributeProvider attributeProvider,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    if (attributeProvider is null)
      throw new ArgumentNullException(nameof(attributeProvider));
    if (options is null)
      throw new ArgumentNullException(nameof(options));

    const string AttributeSectionPrefixDefault = "[";
    const string AttributeSectionPrefixField = "[field: ";
    const string AttributeSectionPrefixParameter = "[param: ";
    const string AttributeSectionPrefixReturnParameter = "[return: ";
    const string AttributeSectionSuffix = "]";

    var attributeTarget = AttributeTarget.Default;
    var attributeSectionPrefix = AttributeSectionPrefixDefault;

    switch (attributeProvider) {
      case Type t:
        attributeSectionPrefix = AttributeSectionPrefixDefault;
        attributeTarget = t.IsGenericParameter
          ? AttributeTarget.GenericParameter
          : AttributeTarget.Default;
        break;

      case ParameterInfo para:
        if (para.GetDeclaringProperty() is PropertyInfo p) {
          if (para.Member == p.GetMethod) {
            attributeSectionPrefix = AttributeSectionPrefixReturnParameter;
            attributeTarget = AttributeTarget.PropertyGetMethodReturnParameter;
          }
          else if (para.Member == p.SetMethod) {
            attributeSectionPrefix = AttributeSectionPrefixParameter;
            attributeTarget = AttributeTarget.PropertySetMethodParameter;
          }
        }
        else if (para.GetDeclaringEvent() is EventInfo ev) {
          attributeSectionPrefix = AttributeSectionPrefixParameter;
          attributeTarget = AttributeTarget.EventAccessorMethodParameter;
        }
        else if (para.IsReturnParameter()) {
          attributeSectionPrefix = AttributeSectionPrefixReturnParameter;
          attributeTarget = para.Member.GetDeclaringTypeOrThrow().IsDelegate()
            ? AttributeTarget.DelegateReturnParameter
            : AttributeTarget.MethodReturnParameter;
        }
        else {
          attributeSectionPrefix = AttributeSectionPrefixDefault;
          attributeTarget = para.Member.GetDeclaringTypeOrThrow().IsDelegate()
            ? AttributeTarget.DelegateParameter
            : AttributeTarget.MethodParameter;
        }

        break;

      case FieldInfo f:
        if (f.IsPropertyBackingField()) {
          attributeSectionPrefix = AttributeSectionPrefixField;
          attributeTarget = AttributeTarget.PropertyBackingField;
        }
        else if (f.IsEventBackingField()) {
          attributeSectionPrefix = AttributeSectionPrefixField;
          attributeTarget = AttributeTarget.EventBackingField;
        }

        break;

      case MethodInfo m:
        if (m.IsPropertyAccessorMethod()) {
          attributeSectionPrefix = AttributeSectionPrefixDefault;
          attributeTarget = AttributeTarget.PropertyAccessorMethod;
        }

        try {
          if (m.IsEventAccessorMethod()) {
            attributeSectionPrefix = AttributeSectionPrefixDefault;
            attributeTarget = AttributeTarget.EventAccessorMethod;
          }
        }
        catch (TypeLoadException) {
          // FIXME: https://github.com/smdn/Smdn.Reflection.ReverseGenerating/issues/31
        }

        break;
    }

    var attributeSectionFormat = attributeTarget switch {
      AttributeTarget.PropertyAccessorMethod or
      AttributeTarget.EventAccessorMethod => options.AttributeDeclaration.AccessorFormat,

      AttributeTarget.PropertyGetMethodReturnParameter or
      AttributeTarget.PropertySetMethodParameter or
      AttributeTarget.EventAccessorMethodParameter => options.AttributeDeclaration.AccessorParameterFormat,

      AttributeTarget.PropertyBackingField or
      AttributeTarget.EventBackingField => options.AttributeDeclaration.BackingFieldFormat,

      AttributeTarget.GenericParameter => options.AttributeDeclaration.GenericParameterFormat,

      AttributeTarget.MethodParameter or
      AttributeTarget.MethodReturnParameter => options.AttributeDeclaration.MethodParameterFormat,

      AttributeTarget.DelegateParameter or
      AttributeTarget.DelegateReturnParameter => options.AttributeDeclaration.DelegateParameterFormat,

      _ => AttributeSectionFormat.Discrete,
    };

    attributeSectionFormat = attributeSectionFormat switch {
      // valid
      AttributeSectionFormat.Discrete or
      AttributeSectionFormat.List => attributeSectionFormat,

      // invalid
      _ => throw new InvalidOperationException($"invalid AttributeSectionFormat value ({attributeSectionFormat})"),
    };

    var attributes = GetAttributes(attributeProvider, options.AttributeDeclaration.TypeFilter)
      .OrderBy(static attr => attr.GetAttributeType().FullName)
      .Select(attr =>
        (
          name: ConvertAttributeName(attr),
          args: string.Join(", ", ConvertAttributeArguments(attr))
        )
      );

    if (!attributes.Any())
      return Enumerable.Empty<string>();

    return attributeSectionFormat switch {
      AttributeSectionFormat.Discrete => attributes.Select(
        a => attributeSectionPrefix + a.name + (string.IsNullOrEmpty(a.args) ? string.Empty : "(" + a.args + ")") + AttributeSectionSuffix
      ),

      AttributeSectionFormat.List => Enumerable.Repeat(
        string.Concat(
          attributeSectionPrefix,
          string.Join(
            ", ",
            attributes.Select(
              static a => string.IsNullOrEmpty(a.args) ? a.name : a.name + "(" + a.args + ")"
            )
          ),
          AttributeSectionSuffix
        ),
        count: 1
      ),

      _ =>
#if SYSTEM_DIAGNOSTICS_UNREACHABLEEXCEPTION
        throw new UnreachableException(),
#else
        throw new NotImplementedException("unreachable"),
#endif
    };

    static IEnumerable<CustomAttributeData> GetAttributes(
      ICustomAttributeProvider attributeProvider,
      AttributeTypeFilter? filter
    )
    {
      foreach (var attr in attributeProvider.GetCustomAttributeDataList()) {
        if (filter is not null && !filter(attr.AttributeType, attributeProvider))
          continue;

        yield return attr;
      }

      if (
        attributeProvider is Type t &&
        t.IsValueType &&
        !t.IsGenericParameter &&
        !t.IsEnum &&
        !t.IsStructLayoutDefault()
      ) {
        // yield pseudo CustomAttributeData for StructLayoutAttribute
        yield return new StructLayoutCustomAttributeData(
          t.StructLayoutAttribute ?? throw new InvalidOperationException($"can not get {nameof(Type.StructLayoutAttribute)} of {t}")
        );
      }
    }

    string ConvertAttributeName(CustomAttributeData attr)
    {
      if (referencingNamespaces is not null) {
        var attributeTypeNamespace = attr.GetAttributeType().Namespace;

        if (attributeTypeNamespace is not null)
          referencingNamespaces.Add(attributeTypeNamespace);
      }

      var typeOfAttribute = attr.GetAttributeType();

      return CSharpFormatter.FormatTypeNameCore(
        typeOfAttribute,
        options: new(
          AttributeProvider: attributeProvider ?? typeOfAttribute,
          TypeWithNamespace: options.AttributeDeclaration.WithNamespace,
          WithDeclaringTypeName: options.AttributeDeclaration.WithDeclaringTypeName,
          TranslateLanguagePrimitiveType: options.TranslateLanguagePrimitiveTypeDeclaration, // TODO: options.AttributeDeclaration.TranslateLanguagePrimitiveType
          OmitAttributeSuffix: options.AttributeDeclaration.OmitAttributeSuffix
        )
      );
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
        val: arg.GetTypedValue(),
        typeOfValue: arg.ArgumentType,
        options: CSharpFormatter.ValueFormatOptions.FromGeneratorOptions(
          options: options,
          tryFindConstantField: false
        )
      );
  }
#pragma warning restore CA1502, CA1506
}
