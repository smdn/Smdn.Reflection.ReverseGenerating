// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#nullable enable

using System;
using System.Reflection;

using Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

namespace Smdn.Reflection.ReverseGenerating;

partial class GeneratorTests {
  private static GeneratorOptions GetGeneratorOptions(GeneratorTestCaseAttribute testCaseAttribute)
  {
    var options = new GeneratorOptions() {
      IgnorePrivateOrAssembly = testCaseAttribute.IgnorePrivateOrAssembly,

      TranslateLanguagePrimitiveTypeDeclaration = testCaseAttribute.TranslateLanguagePrimitiveTypeDeclaration,
    };

    var typeDeclarationOptions = options.TypeDeclaration;

    typeDeclarationOptions.WithNamespace = testCaseAttribute.TypeWithNamespace;
    typeDeclarationOptions.WithDeclaringTypeName = testCaseAttribute.TypeWithDeclaringTypeName;
    typeDeclarationOptions.WithAccessibility = testCaseAttribute.TypeWithAccessibility;
    typeDeclarationOptions.OmitEndOfStatement = testCaseAttribute.TypeOmitEndOfStatement;
    typeDeclarationOptions.OmitEnumUnderlyingTypeIfPossible = testCaseAttribute.TypeOmitEnumUnderlyingTypeIfPossible;
    typeDeclarationOptions.EnableRecordTypes = testCaseAttribute.TypeEnableRecordTypes;
    typeDeclarationOptions.OmitRecordImplicitInterface = testCaseAttribute.TypeOmitRecordImplicitInterface;
    typeDeclarationOptions.DetectUnsafe = testCaseAttribute.TypeDetectUnsafe;
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    typeDeclarationOptions.NullabilityInfoContext = testCaseAttribute.TypeEnableNullabilityAnnotations
      ? typeDeclarationOptions.NullabilityInfoContext ?? new()
      : null;
#endif

    var memberDeclarationOptions = options.MemberDeclaration;

    memberDeclarationOptions.WithNamespace = testCaseAttribute.MemberWithNamespace;
    memberDeclarationOptions.WithDeclaringTypeName = testCaseAttribute.MemberWithDeclaringTypeName;
    memberDeclarationOptions.WithEnumTypeName = testCaseAttribute.MemberWithEnumTypeName;
    memberDeclarationOptions.WithAccessibility = testCaseAttribute.MemberWithAccessibility;
    memberDeclarationOptions.MethodBody = testCaseAttribute.MethodBody;
    memberDeclarationOptions.AccessorBody = testCaseAttribute.AccessorBody;
    memberDeclarationOptions.OmitEndOfStatement = testCaseAttribute.MemberOmitEndOfStatement;
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    memberDeclarationOptions.NullabilityInfoContext = testCaseAttribute.MemberEnableNullabilityAnnotations
      ? memberDeclarationOptions.NullabilityInfoContext ?? new()
      : null;
#endif

    var attributeDeclarationOptions = options.AttributeDeclaration;

    attributeDeclarationOptions.WithNamespace = testCaseAttribute.AttributeWithNamespace;
    attributeDeclarationOptions.WithDeclaringTypeName = testCaseAttribute.AttributeWithDeclaringTypeName;
    attributeDeclarationOptions.WithNamedArguments = testCaseAttribute.AttributeWithNamedArguments;
    attributeDeclarationOptions.OmitAttributeSuffix = testCaseAttribute.AttributeOmitAttributeSuffix;
    attributeDeclarationOptions.OmitInaccessibleMembersInNullStateAttribute = testCaseAttribute.AttributeOmitInaccessibleMembersInNullStateAttribute;
    attributeDeclarationOptions.AccessorFormat = testCaseAttribute.AttributeAccessorFormat;
    attributeDeclarationOptions.AccessorParameterFormat = testCaseAttribute.AttributeAccessorParameterFormat;
    attributeDeclarationOptions.BackingFieldFormat = testCaseAttribute.AttributeBackingFieldFormat;
    attributeDeclarationOptions.GenericParameterFormat = testCaseAttribute.AttributeGenericParameterFormat;
    attributeDeclarationOptions.MethodParameterFormat = testCaseAttribute.AttributeMethodParameterFormat;
    attributeDeclarationOptions.DelegateParameterFormat = testCaseAttribute.AttributeDelegateParameterFormat;

    if (testCaseAttribute.TypeOfAttributeTypeFilterFunc is not null && !string.IsNullOrEmpty(testCaseAttribute.NameOfAttributeTypeFilterFunc)) {
      var methodSignatureOfAttributeTypeFilter = typeof(AttributeTypeFilter).GetDelegateSignatureMethod() ?? throw new InvalidOperationException("cannot get delegate signature method");
      var filterFunc = testCaseAttribute.TypeOfAttributeTypeFilterFunc.GetMethod(
        name: testCaseAttribute.NameOfAttributeTypeFilterFunc,
        bindingAttr: BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
        binder: null,
        callConvention: methodSignatureOfAttributeTypeFilter.CallingConvention,
        types: Array.ConvertAll(methodSignatureOfAttributeTypeFilter.GetParameters(), static p => p.ParameterType),
        modifiers: null
      );

      if (filterFunc is not null)
        attributeDeclarationOptions.TypeFilter = (AttributeTypeFilter)Delegate.CreateDelegate(typeof(AttributeTypeFilter), filterFunc);
    }

    var valueDeclarationOptions = options.ValueDeclaration;

    valueDeclarationOptions.UseDefaultLiteral = testCaseAttribute.ValueUseDefaultLiteral;
    valueDeclarationOptions.WithNamespace = testCaseAttribute.ValueWithNamespace;
    valueDeclarationOptions.WithDeclaringTypeName = testCaseAttribute.ValueWithDeclaringTypeName;

    var parameterDeclarationOptions = options.ParameterDeclaration;

    parameterDeclarationOptions.WithNamespace = testCaseAttribute.ParameterWithNamespace;
    parameterDeclarationOptions.WithDeclaringTypeName = testCaseAttribute.ParameterWithDeclaringTypeName;

    return options;
  }
}
