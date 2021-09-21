// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  public abstract class GeneratorTestCaseAttribute : Attribute {
    public string Expected { get; private set; }
    public bool TranslateLanguagePrimitiveTypeDeclaration { get; set; } = true;
    public bool MemberWithNamespace { get; set; } = true;
    public bool MemberWithDeclaringTypeName { get; set; } = false;
    public bool MemberWithAccessibility { get; set; } = true;
    public bool MemberOmitEndOfStatement { get; set; } = false;
    public bool TypeWithNamespace { get; set; } = true;
    public bool TypeWithDeclaringTypeName { get; set; } = false;
    public bool TypeWithAccessibility { get; set; } = true;
    public bool TypeOmitEndOfStatement { get; set; } = false;
    public bool UseDefaultLiteral { get; set; } = false;
    public bool IgnorePrivateOrAssembly { get; set; } = false;
    public MethodBodyOption MethodBody { get; set; } = MethodBodyOption.EmptyImplementation;
    public string SourceLocation { get; }

    public GeneratorTestCaseAttribute(
      string expected,
      string sourceFilePath,
      int lineNumber
    )
    {
      this.Expected = expected;
      this.SourceLocation = $"{Path.GetFileName(sourceFilePath)}:{lineNumber}";
    }
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

      var valueDeclarationOptions = options.ValueDeclaration;

      valueDeclarationOptions.UseDefaultLiteral = testCaseAttribute.UseDefaultLiteral;

      return options;
    }
  }
}
