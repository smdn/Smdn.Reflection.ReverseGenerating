// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating;

public abstract class GeneratorTestCaseAttribute : Attribute {
  private readonly string expectedValue;
  public Type? ExpectedValueGeneratorType { get; set; } = null;
  public string? ExpectedValueGeneratorMemberName { get; set; } = null;
  public bool TranslateLanguagePrimitiveTypeDeclaration { get; set; } = true;
  public bool MemberWithNamespace { get; set; } = true;
  public bool MemberWithDeclaringTypeName { get; set; } = false;
  public bool MemberWithAccessibility { get; set; } = true;
  public bool MemberOmitEndOfStatement { get; set; } = false;
  public bool TypeWithNamespace { get; set; } = true;
  public bool TypeWithDeclaringTypeName { get; set; } = false;
  public bool TypeWithAccessibility { get; set; } = true;
  public bool TypeOmitEndOfStatement { get; set; } = false;
  public bool AttributeWithNamespace { get; set; } = true;
  public bool AttributeWithNamedArguments { get; set; } = false;
  public Type? TypeOfAttributeTypeFilterFunc { get; set; } = null;
  public string? NameOfAttributeTypeFilterFunc { get; set; } = null;
  public bool UseDefaultLiteral { get; set; } = false;
  public bool IgnorePrivateOrAssembly { get; set; } = false;
  public MethodBodyOption MethodBody { get; set; } = MethodBodyOption.EmptyImplementation;
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
public class SkipTestCaseAttribute : Attribute {
  public string Reason { get; init; }

  public SkipTestCaseAttribute(string reason)
  {
    this.Reason = reason;
  }

  public void Throw() => throw new IgnoreException(Reason);
}

[TestFixture]
public partial class GeneratorTests {
  private static IEnumerable<Type> FindTypes(Func<Type, bool> predicate)
    => Assembly.GetExecutingAssembly().GetTypes().Where(predicate);

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

    var memberDeclarationOptions = options.MemberDeclaration;

    memberDeclarationOptions.WithNamespace = testCaseAttribute.MemberWithNamespace;
    memberDeclarationOptions.WithDeclaringTypeName = testCaseAttribute.MemberWithDeclaringTypeName;
    memberDeclarationOptions.WithAccessibility = testCaseAttribute.MemberWithAccessibility;
    memberDeclarationOptions.MethodBody = testCaseAttribute.MethodBody;
    memberDeclarationOptions.OmitEndOfStatement = testCaseAttribute.MemberOmitEndOfStatement;

    var attributeDeclarationOptions = options.AttributeDeclaration;

    attributeDeclarationOptions.WithNamespace = testCaseAttribute.AttributeWithNamespace;
    attributeDeclarationOptions.WithNamedArguments = testCaseAttribute.AttributeWithNamedArguments;

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

    valueDeclarationOptions.UseDefaultLiteral = testCaseAttribute.UseDefaultLiteral;

    return options;
  }
}
