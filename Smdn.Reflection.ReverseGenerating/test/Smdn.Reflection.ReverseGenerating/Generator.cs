// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  internal abstract class GeneratorTestCaseAttribute : Attribute {
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
      return new GeneratorOptions() {
        IgnorePrivateOrAssembly = testCaseAttribute.IgnorePrivateOrAssembly,

        TranslateLanguagePrimitiveTypeDeclaration = testCaseAttribute.TranslateLanguagePrimitiveTypeDeclaration,

        TypeDeclarationWithNamespace = testCaseAttribute.TypeWithNamespace,
        TypeDeclarationWithDeclaringTypeName = testCaseAttribute.TypeWithDeclaringTypeName,
        TypeDeclarationWithAccessibility = testCaseAttribute.TypeWithAccessibility,
        TypeDeclarationOmitEndOfStatement = testCaseAttribute.TypeOmitEndOfStatement,

        MemberDeclarationWithNamespace = testCaseAttribute.MemberWithNamespace,
        MemberDeclarationWithDeclaringTypeName = testCaseAttribute.MemberWithDeclaringTypeName,
        MemberDeclarationWithAccessibility = testCaseAttribute.MemberWithAccessibility,
        MemberDeclarationUseDefaultLiteral = testCaseAttribute.UseDefaultLiteral,
        MemberDeclarationMethodBody = testCaseAttribute.MethodBody,
        MemberDeclarationOmitEndOfStatement = testCaseAttribute.MemberOmitEndOfStatement,
      };
    }
  }
}
