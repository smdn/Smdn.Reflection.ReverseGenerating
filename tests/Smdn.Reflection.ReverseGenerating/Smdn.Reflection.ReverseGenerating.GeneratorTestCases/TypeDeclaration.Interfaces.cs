// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.Interfaces {
  [TypeDeclarationTestCase("internal interface I0")] internal interface I0 { };
  [TypeDeclarationTestCase("public interface I1")] public interface I1 { };

  class Accessibilities {
    [TypeDeclarationTestCase("public interface I1")] public interface I1 { }
    [TypeDeclarationTestCase("internal interface I2")] internal interface I2 { }
    [TypeDeclarationTestCase("protected interface I3")] protected interface I3 { }
    [TypeDeclarationTestCase("internal protected interface I4")] protected internal interface I4 { }
    [TypeDeclarationTestCase("internal protected interface I5")] internal protected interface I5 { }
    [TypeDeclarationTestCase("private protected interface I6")] private protected interface I6 { }
    [TypeDeclarationTestCase("private protected interface I7")] protected private interface I7 { }
    [TypeDeclarationTestCase("private interface I8")] private interface I8 { }
  }
}
