// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Runtime.CompilerServices;

using NUnit.Framework;

using Smdn.IO;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

#pragma warning disable IDE0040
partial class ApiListWriterTests {
#pragma warning restore IDE0040
  private void WriteExportedTypes_FixedBufferFields_ExcludeFixedBufferFieldTypes_Core(
    string sourceCode,
    string[] expectedDeclarationLines,
    bool excludeFixedBufferFieldTypes,
    bool shouldWritten
  )
  {
    var options = new ApiListWriterOptions();

    options.Writer.ExcludeFixedBufferFieldTypes = excludeFixedBufferFieldTypes;
    options.Writer.WriteHeader = false;
    options.Writer.WriteFooter = false;
    options.AttributeDeclaration.TypeFilter = static (attrType, _) => {
      return
        string.Equals(attrType.Name, nameof(CompilerGeneratedAttribute), StringComparison.Ordinal) ||
        string.Equals(attrType.Name, nameof(FixedBufferAttribute), StringComparison.Ordinal) ||
        string.Equals(attrType.Name, nameof(UnsafeValueTypeAttribute), StringComparison.Ordinal);
    };
    options.Indent = string.Empty;

    var output = new StringReader(
      WriteApiListFromSourceCode(
        sourceCode,
        options,
        referenceAssemblyFileNames: [
          typeof(FixedBufferAttribute).Assembly.GetName().Name + ".dll",
        ]
      )
    ).ReadAllLines();

    var joinedOutput = string.Join("\n", output);

    //Console.WriteLine(joinedOutput);

    foreach (var expectedDeclarationLine in expectedDeclarationLines) {
      if (shouldWritten)
        Assert.That(joinedOutput, Does.Contain(expectedDeclarationLine));
      else
        Assert.That(joinedOutput, Does.Not.Contain(expectedDeclarationLine));
    }
  }

  private static System.Collections.IEnumerable YieldTestCases_WriteExportedTypes_FixedBufferFields_ExcludeFixedBufferFieldTypes()
  {
    foreach (var excludeFixedBufferFieldTypes in new[] { true, false }) {
      yield return new object[] {
        @"public unsafe struct S { public fixed int F[4]; }",
        new[] {
          "[CompilerGenerated]",
          "[UnsafeValueType]",
          "public struct <F>",
          "public int FixedElementField;"
        },
        excludeFixedBufferFieldTypes
      };
      yield return new object[] {
        @"public unsafe struct S { public fixed byte FixedBufferField[1]; }",
        new[] {
          "[CompilerGenerated]",
          "[UnsafeValueType]",
          "public struct <FixedBufferField>",
          "public byte FixedElementField;"
        },
        excludeFixedBufferFieldTypes
      };
    }
  }

  [TestCaseSource(nameof(YieldTestCases_WriteExportedTypes_FixedBufferFields_ExcludeFixedBufferFieldTypes))]
  public void WriteExportedTypes_FixedBufferFields_ExcludeFixedBufferFieldTypes(
    string sourceCode,
    string[] expectedDeclarationLines,
    bool excludeFixedBufferFieldTypes
  )
    => WriteExportedTypes_FixedBufferFields_ExcludeFixedBufferFieldTypes_Core(
      sourceCode: sourceCode,
      expectedDeclarationLines: expectedDeclarationLines,
      excludeFixedBufferFieldTypes: excludeFixedBufferFieldTypes,
      shouldWritten: !excludeFixedBufferFieldTypes
    );
}
