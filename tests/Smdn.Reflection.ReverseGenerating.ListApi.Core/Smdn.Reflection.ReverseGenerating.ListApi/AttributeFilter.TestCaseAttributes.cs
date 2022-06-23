// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;

using Smdn.Reflection.ReverseGenerating;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public abstract class GeneratorTestCaseAttribute : Attribute {
  public string ExpectedResult { get; }
  public string SourceLocation { get; }

  protected GeneratorTestCaseAttribute(
    string expectedResult,
    string? sourceFilePath,
    int lineNumber
  )
  {
    this.ExpectedResult = expectedResult;
    this.SourceLocation = $"{(sourceFilePath is null ? string.Empty : Path.GetFileName(sourceFilePath))}:{lineNumber}";
  }

  public virtual GeneratorOptions CreateGeneratorOptions()
  {
    var options = new GeneratorOptions() {
      IgnorePrivateOrAssembly = false,
      TranslateLanguagePrimitiveTypeDeclaration = true,
    };

    var typeDeclarationOptions = options.TypeDeclaration;

    typeDeclarationOptions.WithNamespace = false;
    typeDeclarationOptions.WithDeclaringTypeName = false;
    typeDeclarationOptions.WithAccessibility = true;
    typeDeclarationOptions.OmitEndOfStatement = true;

    var memberDeclarationOptions = options.MemberDeclaration;

    memberDeclarationOptions.WithNamespace = false;
    memberDeclarationOptions.WithDeclaringTypeName = false;
    memberDeclarationOptions.WithAccessibility = true;
    memberDeclarationOptions.MethodBody = MethodBodyOption.None;
    memberDeclarationOptions.OmitEndOfStatement = true;

    var attributeDeclarationOptions = options.AttributeDeclaration;

    attributeDeclarationOptions.WithNamespace = false;
    attributeDeclarationOptions.WithNamedArguments = false;

    var valueDeclarationOptions = options.ValueDeclaration;

    valueDeclarationOptions.UseDefaultLiteral = true;

    return options;
  }
}

public abstract class AttributeFilterTestCaseAttribute : GeneratorTestCaseAttribute {
  public Type? FilterType { get; set; }
  public string? FilterMemberName { get; set; }

  protected AttributeFilterTestCaseAttribute(
    string expectedResult,
    string? sourceFilePath,
    int lineNumber
  )
    : base(
      expectedResult,
      sourceFilePath,
      lineNumber
    )
  {
  }

  public override GeneratorOptions CreateGeneratorOptions()
  {
    var options = base.CreateGeneratorOptions();

    AttributeTypeFilter? filter = null;

    if (FilterType is null || FilterMemberName is null) {
      filter = null;
    }
    else {
      var filterMember = FilterType.GetMember(
        FilterMemberName,
        MemberTypes.Field | MemberTypes.Method | MemberTypes.Property,
        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
      ).FirstOrDefault();

      filter = filterMember switch {
        FieldInfo f => (AttributeTypeFilter)(f.GetValue(obj: null) ?? throw new InvalidOperationException("expected value must not be null")),
        MethodInfo m => (AttributeTypeFilter)(m.Invoke(obj: null, parameters: null) ?? throw new InvalidOperationException("expected value must not be null")),
        PropertyInfo p => (AttributeTypeFilter)(p.GetGetMethod(nonPublic: true)?.Invoke(obj: null, parameters: null) ?? throw new InvalidOperationException("expected value must not be null")),
        null => throw new InvalidOperationException($"member not found: {FilterType.FullName}.{FilterMemberName}"),
        _ => throw new InvalidOperationException($"invalid member type: {FilterType.FullName}.{FilterMemberName}"),
      };
    }

    options.AttributeDeclaration.TypeFilter = (Type attrType, ICustomAttributeProvider attrProvider) => {
      // except test case attributes
      if (string.Equals(attrType.FullName, typeof(MemberAttributeFilterTestCaseAttribute).FullName, StringComparison.Ordinal))
        return false;
      if (string.Equals(attrType.FullName, typeof(TypeAttributeFilterTestCaseAttribute).FullName, StringComparison.Ordinal))
        return false;

      if (filter is not null)
        return filter(attrType, attrProvider);

      return true;
    };

    return options;
  }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class TypeAttributeFilterTestCaseAttribute : AttributeFilterTestCaseAttribute {
  public TypeAttributeFilterTestCaseAttribute(
    string expectedResult,
    [CallerFilePath] string? sourceFilePath = null,
    [CallerLineNumber] int lineNumber = 0
  )
    : base(
      expectedResult,
      sourceFilePath,
      lineNumber
    )
  {
  }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class MemberAttributeFilterTestCaseAttribute : AttributeFilterTestCaseAttribute {
  public MemberAttributeFilterTestCaseAttribute(
    string expectedResult,
    [CallerFilePath] string? sourceFilePath = null,
    [CallerLineNumber] int lineNumber = 0
  )
    : base(
      expectedResult,
      sourceFilePath,
      lineNumber
    )
  {
  }
}
