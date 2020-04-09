using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Smdn.Reflection.ReverseGenerating {
  public static class Generator {
    public static string GenerateTypeDeclaration(
      Type t,
      ISet<string> referencingNamespaces,
      GeneratorOptions options
    ) =>
      GenerateTypeDeclaration(
        t,
        false,
        referencingNamespaces,
        options ?? throw new ArgumentNullException(nameof(options))
      ).First();

    public static IEnumerable<string> GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(
      Type t,
      ISet<string> referencingNamespaces,
      GeneratorOptions options
    ) =>
      GenerateTypeDeclaration(
        t,
        true,
        referencingNamespaces,
        options ?? throw new ArgumentNullException(nameof(options))
      );

    private static IEnumerable<string> GenerateTypeDeclaration(
      Type t,
      bool generateExplicitBaseTypeAndInterfaces,
      ISet<string> referencingNamespaces,
      GeneratorOptions options
    )
    {
      var accessibilities = CSharpFormatter.FormatAccessibility(t.GetAccessibility());
      var typeName = t.FormatTypeName(typeWithNamespace: false, withDeclaringTypeName: false);

      var genericArgumentConstraints = t
        .GetGenericArguments()
        .Select(arg => GenerateGenericArgumentConstraintDeclaration(arg, referencingNamespaces, options))
        .Where(d => d != null)
        .ToList();

      string GetSingleLineGenericArgumentConstraintsDeclaration()
        => genericArgumentConstraints.Count == 0 ? string.Empty : " " + string.Join(" ", genericArgumentConstraints);

      if (t.IsEnum) {
        yield return $"{accessibilities} enum {typeName} : {t.GetEnumUnderlyingType().FormatTypeName()}";
        yield break;
      }

      if (t.IsDelegate()) {
        var signatureInfo = t.GetDelegateSignatureMethod();

        referencingNamespaces?.AddRange(signatureInfo.GetSignatureTypes().Where(mpt => !mpt.ContainsGenericParameters).SelectMany(CSharpFormatter.ToNamespaceList));

        var genericArgumentConstraintDeclaration = genericArgumentConstraints.Count == 0 ? string.Empty : " " + string.Join(" ", genericArgumentConstraints);

        yield return $"{accessibilities} delegate {signatureInfo.ReturnType.FormatTypeName(attributeProvider: signatureInfo.ReturnTypeCustomAttributes, typeWithNamespace: options.TypeDeclarationWithNamespace)} {typeName}({CSharpFormatter.FormatParameterList(signatureInfo, typeWithNamespace: options.TypeDeclarationWithNamespace, useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral)}){genericArgumentConstraintDeclaration};";
        yield break;
      }

      string typeDeclaration = null;

      if (t.IsInterface) {
        typeDeclaration = $"{accessibilities} interface {typeName}";
      }
      else if (t.IsValueType) {
        var isReadOnly = t.IsReadOnlyValueType() ? " readonly" : string.Empty;
        var isByRefLike = t.IsByRefLikeValueType() ? " ref" : string.Empty;

        typeDeclaration = $"{accessibilities}{isReadOnly}{isByRefLike} struct {typeName}";
      }
      else {
        string modifier = null;

        if (t.IsAbstract && t.IsSealed)
          modifier = " static";
        else if (t.IsAbstract)
          modifier = " abstract";
        else if (t.IsSealed)
          modifier = " sealed";

        typeDeclaration = $"{accessibilities}{modifier} class {typeName}";
      }

      if (!generateExplicitBaseTypeAndInterfaces) {
        yield return typeDeclaration + GetSingleLineGenericArgumentConstraintsDeclaration();
        yield break;
      }

      var baseTypeList = GenerateExplicitBaseTypeAndInterfaces(t, referencingNamespaces, options).ToList();

      if (baseTypeList.Count <= 1) {
        var baseTypeDeclaration = baseTypeList.Count == 0 ? string.Empty : " : " + baseTypeList[0];
        var genericArgumentConstraintDeclaration = GetSingleLineGenericArgumentConstraintsDeclaration();

        yield return typeDeclaration + baseTypeDeclaration + genericArgumentConstraintDeclaration;
      }
      else {
        yield return typeDeclaration + " :";

        for (var index = 0; index < baseTypeList.Count; index++) {
          if (index == baseTypeList.Count - 1)
            yield return options.Indent + baseTypeList[index];
          else
            yield return options.Indent + baseTypeList[index] + ",";
        }

        foreach (var constraint in genericArgumentConstraints) {
          yield return options.Indent + constraint;
        }
      }
    }

    public static string GenerateGenericArgumentConstraintDeclaration(
      Type genericArgument,
      ISet<string> referencingNamespaces,
      GeneratorOptions options
    )
    {
      static IEnumerable<string> GetGenericArgumentConstraintsOf(Type argument, ISet<string> _referencingNamespaces, bool typeWithNamespace)
      {
        var constraintAttrs = argument.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
        var constraintTypes = argument.GetGenericParameterConstraints();

        _referencingNamespaces?.AddRange(constraintTypes.Where(ct => ct != typeof(ValueType)).SelectMany(CSharpFormatter.ToNamespaceList));

        if (
          constraintAttrs.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint) &&
          constraintAttrs.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint) &&
          constraintTypes.Any(ct => ct == typeof(ValueType))
        ) {
          constraintAttrs &= ~GenericParameterAttributes.NotNullableValueTypeConstraint;
          constraintAttrs &= ~GenericParameterAttributes.DefaultConstructorConstraint;
          constraintTypes = constraintTypes.Where(ct => ct != typeof(ValueType)).ToArray();

          yield return "struct";
        }

        if (constraintAttrs.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
          yield return "class";
        if (constraintAttrs.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
          yield return "struct"; // XXX

        foreach (var ctn in constraintTypes.Select(i => i.FormatTypeName(typeWithNamespace: typeWithNamespace)).OrderBy(name => name, StringComparer.Ordinal))
          yield return ctn;

        if (constraintAttrs.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
          yield return "new()";
      }

      var constraints = string.Join(
        ", ",
        GetGenericArgumentConstraintsOf(
          genericArgument,
          referencingNamespaces,
          genericArgument.DeclaringMethod == null ? options.TypeDeclarationWithNamespace : options.MemberDeclarationWithNamespace
        )
      );

      if (0 < constraints.Length)
        return $"where {genericArgument.FormatTypeName(typeWithNamespace: false)} : {constraints}";
      else
        return null;
    }

    public static IEnumerable<string> GenerateExplicitBaseTypeAndInterfaces(Type t, ISet<string> referencingNamespaces, GeneratorOptions options)
    {
      if (options == null)
        throw new ArgumentNullException(nameof(options));

      return t
        .GetExplicitBaseTypeAndInterfaces()
        .Where(type => !(options.IgnorePrivateOrAssembly && (type.IsNotPublic || type.IsNestedAssembly || type.IsNestedFamily || type.IsNestedFamANDAssem || type.IsNestedPrivate)))
        .Select(type => {
          referencingNamespaces?.AddRange(CSharpFormatter.ToNamespaceList(type));
          return new { IsInterface = type.IsInterface, Name = type.FormatTypeName(typeWithNamespace: options.TypeDeclarationWithNamespace) };
        })
        .OrderBy(type => type.IsInterface)
        .ThenBy(type => type.Name, StringComparer.Ordinal)
        .Select(type => type.Name);
    }

    internal struct DefaultLayoutStruct {
      // XXX
      public static readonly StructLayoutAttribute Attribute = typeof(DefaultLayoutStruct).StructLayoutAttribute;
    }

    public static IEnumerable<string> GenerateAttributeList(ICustomAttributeProvider attributeProvider, ISet<string> referencingNamespaces, GeneratorOptions options)
    {
      return GetAttributes()
        .OrderBy(attr => attr.GetType().FullName)
        .Select(attr => new { Name = ConvertAttributeName(attr), Params = ConvertAttributeParameters(attr) })
        .Select(a => "[" + a.Name + (string.IsNullOrEmpty(a.Params) ? string.Empty : "(" + a.Params + ")") + "]");

      IEnumerable<Attribute> GetAttributes()
      {
        foreach (var attr in attributeProvider.GetCustomAttributes(typeof(Attribute), false)) {
          if (attr is System.CLSCompliantAttribute)
            continue; // ignore
          if (attr is System.Reflection.DefaultMemberAttribute)
            continue; // ignore

          var nsAttr = attr.GetType().Namespace;

          if (string.Equals("System.Runtime.CompilerServices", nsAttr, StringComparison.Ordinal))
            continue; // ignore

          if (string.Equals("System", nsAttr.Split('.')[0], StringComparison.Ordinal))
            yield return attr as Attribute;
        }

        if (attributeProvider is Type t && t.IsValueType && !t.IsEnum) {
          if (
            t.StructLayoutAttribute.Value != DefaultLayoutStruct.Attribute.Value ||
            t.StructLayoutAttribute.Pack != DefaultLayoutStruct.Attribute.Pack ||
            t.StructLayoutAttribute.CharSet != DefaultLayoutStruct.Attribute.CharSet
          ) {
            yield return t.StructLayoutAttribute;
          }
        }
      }

      string ConvertAttributeName(Attribute attr)
      {
        var typeOfAttr = attr.GetType();

        referencingNamespaces?.Add(typeOfAttr.Namespace);

        var nameOfAttr = typeOfAttr.FormatTypeName(typeWithNamespace: options.TypeDeclarationWithNamespace);

        if (nameOfAttr.EndsWith("Attribute", StringComparison.Ordinal))
          nameOfAttr = nameOfAttr.Substring(0, nameOfAttr.Length - 9);

        return nameOfAttr;
      }

      string ConvertAttributeParameters(Attribute attr)
      {
        switch (attr) {
          case System.AttributeUsageAttribute aua:
            var allowMultiple = aua.AllowMultiple ? ", AllowMultiple = " + ConvertValue(aua.AllowMultiple) : null;
            var inherited =  aua.Inherited ? ", Inherited = " + ConvertValue(aua.Inherited) : null;

            return ConvertValue(aua.ValidOn) + allowMultiple + inherited;

          case System.FlagsAttribute fa:
            return null;

          case System.ObsoleteAttribute oa:
            var isError = oa.IsError ? ", " + ConvertValue(oa.IsError) : null;

            if (oa.Message != null || isError != null)
              return ConvertValue(oa.Message) + isError;
            else
              return null;

          case System.Diagnostics.DebuggerHiddenAttribute dha:
            return null;

          case System.SerializableAttribute sa:
            return null;

          case System.Diagnostics.ConditionalAttribute ca:
            if (string.IsNullOrEmpty(ca.ConditionString))
              return null;
            else
              return ConvertValue(ca.ConditionString);

          case System.Runtime.InteropServices.FieldOffsetAttribute foa:
            return ConvertValue(foa.Value);

          case System.Runtime.InteropServices.StructLayoutAttribute sla:
            var pack = sla.Pack == DefaultLayoutStruct.Attribute.Pack ? null : ", Pack = " + ConvertValue(sla.Pack);
            var size = sla.Size == 0 ? null : ", Size = " + ConvertValue(sla.Size);
            var charset = sla.CharSet == DefaultLayoutStruct.Attribute.CharSet ? null : ", CharSet = " + ConvertValue(sla.CharSet);

            return ConvertValue(sla.Value) + pack + size + charset;
        }

        throw new NotSupportedException($"unsupported attribute type: {attr.GetType().FullName}");
      }

      string ConvertValue(object @value)
      {
        return CSharpFormatter.FormatValueDeclaration(
          @value,
          @value?.GetType(),
          typeWithNamespace: options.MemberDeclarationWithNamespace,
          findConstantField: true,
          useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral
        );
      }
    }
  }
}
