// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.Records {
  namespace RecordClasses {
    [SkipTestCase("`record` types are not supported currently")]
    [TypeDeclarationTestCase("public record RExpected(int X, string Y);")]
    public record RExpected(int X, string Y);

    [TypeDeclarationTestCase("public class R0")]
    public record R0(int X, string Y);

    [TypeDeclarationTestCase("public class R1")]
    public record class R1(int X, string Y);
  }

  namespace RecordStructs {
    [SkipTestCase("`record` types are not supported currently")]
    [TypeDeclarationTestCase("public record struct RSExpected(int X, string Y);")]
    public record struct RSExpected(int X, string Y);

    [TypeDeclarationTestCase("public struct RS0")]
    public record struct RS0(int X, string Y);
  }

  namespace ReadOnlyRecordStructs {
    [SkipTestCase("`record` types are not supported currently")]
    [TypeDeclarationTestCase("public readonly record struct RRSExpected(int X, string Y);")]
    public readonly record struct RRSExpected(int X, string Y);

    [TypeDeclarationTestCase("public readonly struct RRS0")]
    public readonly record struct RRS0(int X, string Y);
  }
}
