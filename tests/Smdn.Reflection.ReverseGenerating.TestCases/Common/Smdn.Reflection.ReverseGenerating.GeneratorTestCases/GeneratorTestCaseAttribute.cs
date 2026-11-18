// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

public abstract class GeneratorTestCaseAttribute : Attribute, ITestCaseAttribute {
  private readonly string? expectedValue;
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
  public bool TypeEnableRecordTypes { get; set; } = false;
  public bool TypeOmitRecordImplicitInterface { get; set; } = false;
  public bool TypeDetectUnsafe { get; set; } = false;
  public bool TypeEnableNullabilityAnnotations { get; set; } = true;
  public bool AttributeWithNamespace { get; set; } = true;
  public bool AttributeWithNamedArguments { get; set; } = false;
  public bool AttributeWithDeclaringTypeName { get; set; } = true;
  public bool AttributeOmitAttributeSuffix { get; set; } = true;
  public bool AttributeOmitInaccessibleMembersInNullStateAttribute { get; set; } = false;
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

  public string? Expected {
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
    string? expected,
    string? sourceFilePath,
    int lineNumber
  )
  {
    this.expectedValue = expected;
    this.SourceLocation = $"{Path.GetFileName(sourceFilePath)}:{lineNumber}";
  }
}
