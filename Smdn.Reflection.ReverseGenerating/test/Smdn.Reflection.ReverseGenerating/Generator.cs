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
    public bool TypeWithNamespace { get; set; } = true;
    public bool TypeWithAccessibility { get; set; } = true;
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
        TypeDeclarationWithAccessibility = testCaseAttribute.TypeWithAccessibility,

        MemberDeclarationWithNamespace = testCaseAttribute.MemberWithNamespace,
        MemberDeclarationUseDefaultLiteral = testCaseAttribute.UseDefaultLiteral,
        MemberDeclarationMethodBody = testCaseAttribute.MethodBody,
      };
    }
  }
}
