// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#nullable enable

using System;
using System.IO;
using System.Runtime.CompilerServices;

using NUnit.Framework;

using Smdn.IO;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

#pragma warning disable IDE0040
partial class ApiListWriterTests {
#pragma warning restore IDE0040
  private void WriteExportedTypes_RecordTypes_OmitCompilerGeneratedRecordEqualityMethods(
    string sourceCode,
    string expectedMemberDeclaration,
    bool enableRecordTypes,
    bool omitCompilerGeneratedRecordEqualityMethods,
    bool shouldWritten
  )
  {
    var options = new ApiListWriterOptions();

    options.TypeDeclaration.EnableRecordTypes = enableRecordTypes;
    options.Writer.OmitCompilerGeneratedRecordEqualityMethods = omitCompilerGeneratedRecordEqualityMethods;
    options.Writer.WriteHeader = false;
    options.Writer.WriteFooter = false;
    options.AttributeDeclaration.TypeFilter = static (attrType, _) => {
      return string.Equals(attrType.Name, nameof(CompilerGeneratedAttribute), StringComparison.Ordinal);
    };
    options.Indent = string.Empty;

    var output = new StringReader(
      WriteApiListFromSourceCode(
        sourceCode,
        options,
        referenceAssemblyFileNames: [
          typeof(CompilerGeneratedAttribute).Assembly.GetName().Name + ".dll",
          typeof(IEquatable<>).Assembly.GetName().Name + ".dll",
        ]
      )
    ).ReadAllLines();

    var joinedOutput = string.Join("\n", output);

    //Console.WriteLine(joinedOutput);

    if (shouldWritten)
      Assert.That(joinedOutput, Does.Contain(expectedMemberDeclaration));
    else
      Assert.That(joinedOutput, Does.Not.Contain(expectedMemberDeclaration));
  }

  [TestCase("public int X { [CompilerGenerated] get; [CompilerGenerated] init; }", true, true)]
  [TestCase("public int X { [CompilerGenerated] get; [CompilerGenerated] init; }", false, true)]
  [TestCase("public R(int X) {}", true, true)]
  [TestCase("public R(int X) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic override bool Equals(object? obj) {}", true, false)]
  [TestCase("[CompilerGenerated]\npublic override bool Equals(object? obj) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic virtual bool Equals(R? other) {}", true, false)]
  [TestCase("[CompilerGenerated]\npublic virtual bool Equals(R? other) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic override int GetHashCode() {}", true, false)]
  [TestCase("[CompilerGenerated]\npublic override int GetHashCode() {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic static bool operator == (R? left, R? right) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic static bool operator == (R? left, R? right) {}", true, false)]
  [TestCase("[CompilerGenerated]\npublic static bool operator != (R? left, R? right) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic static bool operator != (R? left, R? right) {}", true, false)]
  public void WriteExportedTypes_RecordTypes_OmitCompilerGeneratedRecordEqualityMethods(
    string expectedMemberDeclaration,
    bool omitCompilerGeneratedRecordEqualityMethods,
    bool shouldWritten
  )
    => WriteExportedTypes_RecordTypes_OmitCompilerGeneratedRecordEqualityMethods(
      sourceCode: @"public record R(int X) {}",
      expectedMemberDeclaration: expectedMemberDeclaration,
      enableRecordTypes: true,
      omitCompilerGeneratedRecordEqualityMethods: omitCompilerGeneratedRecordEqualityMethods,
      shouldWritten: shouldWritten
    );

  [TestCase("public int X { [CompilerGenerated] get; [CompilerGenerated] init; }", true, true)]
  [TestCase("public int X { [CompilerGenerated] get; [CompilerGenerated] init; }", false, true)]
  [TestCase("public R(int X) {}", true, true)]
  [TestCase("public R(int X) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic override bool Equals(object? obj) {}", true, true)]
  [TestCase("[CompilerGenerated]\npublic override bool Equals(object? obj) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic virtual bool Equals(R? other) {}", true, true)]
  [TestCase("[CompilerGenerated]\npublic virtual bool Equals(R? other) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic override int GetHashCode() {}", true, true)]
  [TestCase("[CompilerGenerated]\npublic override int GetHashCode() {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic static bool operator == (R? left, R? right) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic static bool operator == (R? left, R? right) {}", true, true)]
  [TestCase("[CompilerGenerated]\npublic static bool operator != (R? left, R? right) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic static bool operator != (R? left, R? right) {}", true, true)]
  public void WriteExportedTypes_RecordTypes_OmitCompilerGeneratedRecordEqualityMethods_DisableRecordTypes(
    string expectedMemberDeclaration,
    bool omitCompilerGeneratedRecordEqualityMethods,
    bool shouldWritten
  )
    => WriteExportedTypes_RecordTypes_OmitCompilerGeneratedRecordEqualityMethods(
      sourceCode: @"public record R(int X) {}",
      expectedMemberDeclaration: expectedMemberDeclaration,
      enableRecordTypes: false,
      omitCompilerGeneratedRecordEqualityMethods: omitCompilerGeneratedRecordEqualityMethods,
      shouldWritten: shouldWritten
    );

  [TestCase("public virtual bool Equals(R? other) {}", true, true)]
  [TestCase("public virtual bool Equals(R? other) {}", false, true)]
  [TestCase("public override int GetHashCode() {}", true, true)]
  [TestCase("public override int GetHashCode() {}", false, true)]
  public void WriteExportedTypes_RecordTypes_OmitCompilerGeneratedRecordEqualityMethods_ExplicitlyImplemented(
    string expectedMemberDeclaration,
    bool omitCompilerGeneratedRecordEqualityMethods,
    bool shouldWritten
  )
    => WriteExportedTypes_RecordTypes_OmitCompilerGeneratedRecordEqualityMethods(
      sourceCode: @"#nullable enable
using System;

public record R(int X) {
  public virtual bool Equals(R? other) => throw new NotImplementedException();
  public override int GetHashCode() => throw new NotImplementedException();
}",
      expectedMemberDeclaration: expectedMemberDeclaration,
      enableRecordTypes: true,
      omitCompilerGeneratedRecordEqualityMethods: omitCompilerGeneratedRecordEqualityMethods,
      shouldWritten: shouldWritten
    );

  [TestCase("public int Y { [CompilerGenerated] get; [CompilerGenerated] init; }", true, true)]
  [TestCase("public int Y { [CompilerGenerated] get; [CompilerGenerated] init; }", false, true)]
  [TestCase("public RX(int X, int Y) {}", true, true)]
  [TestCase("public RX(int X, int Y) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic sealed override bool Equals(R? other) {}", true, false)]
  [TestCase("[CompilerGenerated]\npublic sealed override bool Equals(R? other) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic virtual bool Equals(RX? other) {}", true, false)]
  [TestCase("[CompilerGenerated]\npublic virtual bool Equals(RX? other) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic static bool operator == (RX? left, RX? right) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic static bool operator == (RX? left, RX? right) {}", true, false)]
  [TestCase("[CompilerGenerated]\npublic static bool operator != (RX? left, RX? right) {}", false, true)]
  [TestCase("[CompilerGenerated]\npublic static bool operator != (RX? left, RX? right) {}", true, false)]
  public void WriteExportedTypes_RecordTypes_OmitCompilerGeneratedRecordEqualityMethods_Derived(
    string expectedMemberDeclaration,
    bool omitCompilerGeneratedRecordEqualityMethods,
    bool shouldWritten
  )
    => WriteExportedTypes_RecordTypes_OmitCompilerGeneratedRecordEqualityMethods(
      sourceCode: @"
public record R(int X) {}
public record RX(int X, int Y) : R(X) {}",
      expectedMemberDeclaration: expectedMemberDeclaration,
      enableRecordTypes: true,
      omitCompilerGeneratedRecordEqualityMethods: omitCompilerGeneratedRecordEqualityMethods,
      shouldWritten: shouldWritten
    );
}
