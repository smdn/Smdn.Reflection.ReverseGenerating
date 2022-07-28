// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Reflection;

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

public interface ITestCaseAttribute { }

public abstract class GeneratorTestCaseAttribute : Attribute, ITestCaseAttribute {
  private readonly string expectedValue;
  public Type? ExpectedValueGeneratorType { get; set; } = null;
  public string? ExpectedValueGeneratorMemberName { get; set; } = null;
  public bool TranslateLanguagePrimitiveTypeDeclaration { get; set; } = true;
  public bool MemberWithNamespace { get; set; } = true;
  public bool MemberWithDeclaringTypeName { get; set; } = false;
  public bool MemberWithEnumTypeName { get; set; } = false;
  public bool MemberWithAccessibility { get; set; } = true;
  public bool MemberOmitEndOfStatement { get; set; } = false;
  public bool MemberEnableNullabilityAnnotations { get; set; } = true;
  public bool TypeWithNamespace { get; set; } = true;
  public bool TypeWithDeclaringTypeName { get; set; } = false;
  public bool TypeWithAccessibility { get; set; } = true;
  public bool TypeOmitEndOfStatement { get; set; } = false;
  public bool TypeOmitEnumUnderlyingTypeIfPossible { get; set; } = false;
  public bool TypeEnableNullabilityAnnotations { get; set; } = true;
  public bool AttributeWithNamespace { get; set; } = true;
  public bool AttributeWithNamedArguments { get; set; } = false;
  public bool AttributeWithDeclaringTypeName { get; set; } = true;
  public bool AttributeOmitAttributeSuffix { get; set; } = true;
  public AttributeSectionFormat AttributeAccessorFormat { get; set; } = AttributeSectionFormat.Discrete;
  public AttributeSectionFormat AttributeAccessorParameterFormat { get; set; } = AttributeSectionFormat.Discrete;
  public AttributeSectionFormat AttributeBackingFieldFormat { get; set; } = AttributeSectionFormat.Discrete;
  public AttributeSectionFormat AttributeGenericParameterFormat { get; set; } = AttributeSectionFormat.Discrete;
  public AttributeSectionFormat AttributeMethodParameterFormat { get; set; } = AttributeSectionFormat.Discrete;
  public AttributeSectionFormat AttributeDelegateParameterFormat { get; set; } = AttributeSectionFormat.Discrete;
  public Type? TypeOfAttributeTypeFilterFunc { get; set; } = null;
  public string? NameOfAttributeTypeFilterFunc { get; set; } = null;
  public bool ValueUseDefaultLiteral { get; set; } = false;
  public bool ValueWithNamespace { get; set; } = true;
  public bool ValueWithDeclaringTypeName { get; set; } = true;
  public bool ParameterWithNamespace { get; set; } = true;
  public bool ParameterWithDeclaringTypeName { get; set; } = true;
  public bool IgnorePrivateOrAssembly { get; set; } = false;
  public MethodBodyOption MethodBody { get; set; } = MethodBodyOption.EmptyImplementation;
  public MethodBodyOption AccessorBody { get; set; } = MethodBodyOption.EmptyImplementation;
  public string SourceLocation { get; }

  public string Expected {
    get {
      if (ExpectedValueGeneratorType is null || ExpectedValueGeneratorMemberName is null)
        return expectedValue;

      var expectedValueGeneratorMember = ExpectedValueGeneratorType.GetMember(
        ExpectedValueGeneratorMemberName,
        MemberTypes.Field | MemberTypes.Method | MemberTypes.Property,
        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
      ).FirstOrDefault();

      return expectedValueGeneratorMember switch {
        FieldInfo f => (string)(f.GetValue(obj: null) ?? throw new InvalidOperationException("expected value must not be null")),
        MethodInfo m => (string)(m.Invoke(obj: null, parameters: null) ?? throw new InvalidOperationException("expected value must not be null")),
        PropertyInfo p => (string)(p.GetGetMethod(nonPublic: true)?.Invoke(obj: null, parameters: null) ?? throw new InvalidOperationException("expected value must not be null")),
        null => throw new InvalidOperationException($"member not found: {ExpectedValueGeneratorType.FullName}.{ExpectedValueGeneratorMemberName}"),
        _ => throw new InvalidOperationException($"invalid member type: {ExpectedValueGeneratorType.FullName}.{ExpectedValueGeneratorMemberName}"),
      };
    }
  }

  public GeneratorTestCaseAttribute(
    string expected,
    string sourceFilePath,
    int lineNumber
  )
  {
    this.expectedValue = expected;
    this.SourceLocation = $"{Path.GetFileName(sourceFilePath)}:{lineNumber}";
  }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
public class SkipTestCaseAttribute : Attribute, ITestCaseAttribute {
  public string Reason { get; init; }

  public SkipTestCaseAttribute(string reason)
  {
    this.Reason = reason;
  }

  public void Throw() => throw new NUnit.Framework.IgnoreException(Reason);
}
