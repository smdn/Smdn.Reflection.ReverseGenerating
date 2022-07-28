// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.Enums {
  [TypeDeclarationTestCase("internal enum E0 : int")] internal enum E0 { };
  [TypeDeclarationTestCase("public enum E1 : int")] public enum E1 { };

  class UnderlyingTypes {
    [TypeDeclarationTestCase($"public enum {nameof(EInt)}", TypeOmitEnumUnderlyingTypeIfPossible = true, TranslateLanguagePrimitiveTypeDeclaration = true)]
    [TypeDeclarationTestCase($"public enum {nameof(EInt)}", TypeOmitEnumUnderlyingTypeIfPossible = true, TranslateLanguagePrimitiveTypeDeclaration = false)]
    [TypeDeclarationTestCase($"public enum {nameof(EInt)} : int", TranslateLanguagePrimitiveTypeDeclaration = true)]
    [TypeDeclarationTestCase($"public enum {nameof(EInt)} : Int32", TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]
    [TypeDeclarationTestCase($"public enum {nameof(EInt)} : System.Int32", TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = true)]
    public enum EInt : int { }

    [TypeDeclarationTestCase($"public enum {nameof(EInt32)} : int", TranslateLanguagePrimitiveTypeDeclaration = true)]
    [TypeDeclarationTestCase($"public enum {nameof(EInt32)} : Int32", TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]
    [TypeDeclarationTestCase($"public enum {nameof(EInt32)} : System.Int32", TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = true)]
    public enum EInt32 : System.Int32 { }

    [TypeDeclarationTestCase($"public enum {nameof(EByte)} : byte", TypeOmitEnumUnderlyingTypeIfPossible = true)]
    [TypeDeclarationTestCase($"public enum {nameof(EByte)} : byte", TypeOmitEnumUnderlyingTypeIfPossible = false)]
    [TypeDeclarationTestCase($"public enum {nameof(EByte)} : Byte", TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]
    public enum EByte : byte { }

    [TypeDeclarationTestCase($"public enum {nameof(ESByte)} : sbyte", TypeOmitEnumUnderlyingTypeIfPossible = true)]
    [TypeDeclarationTestCase($"public enum {nameof(ESByte)} : sbyte", TypeOmitEnumUnderlyingTypeIfPossible = false)]
    [TypeDeclarationTestCase($"public enum {nameof(ESByte)} : SByte", TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]
    public enum ESByte : sbyte { }

    [TypeDeclarationTestCase($"public enum {nameof(EShort)} : short", TypeOmitEnumUnderlyingTypeIfPossible = true)]
    [TypeDeclarationTestCase($"public enum {nameof(EShort)} : short", TypeOmitEnumUnderlyingTypeIfPossible = false)]
    [TypeDeclarationTestCase($"public enum {nameof(EShort)} : Int16", TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]
    public enum EShort : short { }

    [TypeDeclarationTestCase($"public enum {nameof(EUShort)} : ushort", TypeOmitEnumUnderlyingTypeIfPossible = true)]
    [TypeDeclarationTestCase($"public enum {nameof(EUShort)} : ushort", TypeOmitEnumUnderlyingTypeIfPossible = false)]
    [TypeDeclarationTestCase($"public enum {nameof(EUShort)} : UInt16", TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]
    public enum EUShort : ushort { }

    [TypeDeclarationTestCase($"public enum {nameof(EUInt)} : uint", TypeOmitEnumUnderlyingTypeIfPossible = true)]
    [TypeDeclarationTestCase($"public enum {nameof(EUInt)} : uint", TypeOmitEnumUnderlyingTypeIfPossible = false)]
    [TypeDeclarationTestCase($"public enum {nameof(EUInt)} : UInt32", TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]
    public enum EUInt : uint { }

    [TypeDeclarationTestCase($"public enum {nameof(ELong)} : long", TypeOmitEnumUnderlyingTypeIfPossible = true)]
    [TypeDeclarationTestCase($"public enum {nameof(ELong)} : long", TypeOmitEnumUnderlyingTypeIfPossible = false)]
    [TypeDeclarationTestCase($"public enum {nameof(ELong)} : Int64", TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]
    public enum ELong : long { }

    [TypeDeclarationTestCase($"public enum {nameof(EULong)} : ulong", TypeOmitEnumUnderlyingTypeIfPossible = true)]
    [TypeDeclarationTestCase($"public enum {nameof(EULong)} : ulong", TypeOmitEnumUnderlyingTypeIfPossible = false)]
    [TypeDeclarationTestCase($"public enum {nameof(EULong)} : UInt64", TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]
    public enum EULong : ulong { }
  }

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
