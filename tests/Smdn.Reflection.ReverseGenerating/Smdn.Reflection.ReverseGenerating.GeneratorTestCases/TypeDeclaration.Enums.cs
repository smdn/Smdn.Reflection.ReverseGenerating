// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.Enums {
  [TypeDeclarationTestCase("internal enum E0 : int")] internal enum E0 { };
  [TypeDeclarationTestCase("public enum E1 : int")] public enum E1 { };

  class Accessibilities {
    [TypeDeclarationTestCase("public enum E1 : int")] public enum E1 { }
    [TypeDeclarationTestCase("internal enum E2 : int")] internal enum E2 { }
    [TypeDeclarationTestCase("protected enum E3 : int")] protected enum E3 { }
    [TypeDeclarationTestCase("internal protected enum E4 : int")] protected internal enum E4 { }
    [TypeDeclarationTestCase("internal protected enum E5 : int")] internal protected enum E5 { }
    [TypeDeclarationTestCase("private protected enum E6 : int")] private protected enum E6 { }
    [TypeDeclarationTestCase("private protected enum E7 : int")] protected private enum E7 { }
    [TypeDeclarationTestCase("private enum E8 : int")] private enum E8 { }
  }

  class ModifierNew : Accessibilities {
    [TypeDeclarationTestCase("new public enum E3 : int")] new public enum E3 { }
  }
}
