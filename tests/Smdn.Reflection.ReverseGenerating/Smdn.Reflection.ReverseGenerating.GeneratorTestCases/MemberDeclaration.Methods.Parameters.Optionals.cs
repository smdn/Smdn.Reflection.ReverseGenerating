// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Threading;

using NSTestCases = Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.Parameters.Optionals;

public class Primitives1 {
  [MemberDeclarationTestCase("public void M1(int x = 0) {}")] public void M1(int x = 0) { }
  [MemberDeclarationTestCase("public void M2(int x = 0) {}")] public void M2(int x = default(int)) { }
  [MemberDeclarationTestCase("public void M3(int x = 1) {}")] public void M3(int x = 1) { }
  [MemberDeclarationTestCase("public void M4(int x = int.MaxValue) {}")] public void M4(int x = int.MaxValue) { }
  [MemberDeclarationTestCase("public void M5(int x = int.MinValue) {}")] public void M5(int x = int.MinValue) { }
}

public class Primitives2 {
  [MemberDeclarationTestCase("public void M1(int x, int y = 0) {}")] public void M1(int x, int y = 0) { }
  [MemberDeclarationTestCase("public void M2(int x, int y = 0) {}")] public void M2(int x, int y = default(int)) { }
  [MemberDeclarationTestCase("public void M3(int x, int y = 1) {}")] public void M3(int x, int y = 1) { }
  [MemberDeclarationTestCase("public void M4(int x, int y = int.MaxValue) {}")] public void M4(int x, int y = int.MaxValue) { }
  [MemberDeclarationTestCase("public void M5(int x, int y = int.MinValue) {}")] public void M5(int x, int y = int.MinValue) { }
}

public class Booleans {
  [MemberDeclarationTestCase("public void M1(bool x = true) {}")] public void M1(bool x = true) { }
  [MemberDeclarationTestCase("public void M2(bool x = false) {}")] public void M2(bool x = false) { }
  [MemberDeclarationTestCase("public void M3(bool x = false) {}")] public void M3(bool x = default(bool)) { }
}

public class Strings {
  [MemberDeclarationTestCase("public void M1(string x = null) {}")] public void M1(string x = null) { }
  [MemberDeclarationTestCase("public void M2(string x = \"\") {}")] public void M2(string x = "") { }
  [MemberDeclarationTestCase("public void M3(string x = \"str\") {}")] public void M3(string x = "str") { }
  [MemberDeclarationTestCase("public void M4(string x = \"\\u0000\") {}")] public void M4(string x = "\0") { }
}

public class Enums {
  [MemberDeclarationTestCase("public void M11(System.DateTimeKind x = System.DateTimeKind.Local) {}")] public void M11(DateTimeKind x = DateTimeKind.Local) { }
  [MemberDeclarationTestCase("public void M12(System.DateTimeKind x = System.DateTimeKind.Unspecified) {}")] public void M12(DateTimeKind x = DateTimeKind.Unspecified) { }
  [MemberDeclarationTestCase("public void M13(System.DateTimeKind x = System.DateTimeKind.Unspecified) {}")] public void M13(DateTimeKind x = default(DateTimeKind)) { }
  [MemberDeclarationTestCase("public void M14(System.DateTimeKind x = (System.DateTimeKind)42) {}")] public void M14(DateTimeKind x = (DateTimeKind)42) { }

  [MemberDeclarationTestCase("public void M21(System.AttributeTargets x = System.AttributeTargets.All) {}")] public void M21(AttributeTargets x = AttributeTargets.All) { }
  [MemberDeclarationTestCase("public void M22(System.AttributeTargets x = System.AttributeTargets.Assembly) {}")] public void M22(AttributeTargets x = AttributeTargets.Assembly) { }

  [MemberDeclarationTestCase("public void M23(System.AttributeTargets x = default(System.AttributeTargets)) {}", ValueUseDefaultLiteral = false)]
  [MemberDeclarationTestCase("public void M23(System.AttributeTargets x = default) {}", ValueUseDefaultLiteral = true)]
  public void M23(AttributeTargets x = default(AttributeTargets)) { }

  [MemberDeclarationTestCase("public void M24(System.AttributeTargets x = (System.AttributeTargets)42) {}")] public void M24(AttributeTargets x = (System.AttributeTargets)42) { }
  [MemberDeclarationTestCase("public void M25(System.AttributeTargets x = (System.AttributeTargets)3) {}")] public void M25(AttributeTargets x = AttributeTargets.Assembly | AttributeTargets.Module) { }

  [MemberDeclarationTestCase(
    "public void M30(System.Environment.SpecialFolder x = System.Environment.SpecialFolder.UserProfile) {}",
    ParameterWithNamespace = true,
    ParameterWithDeclaringTypeName = true,
    ValueWithNamespace = true,
    ValueWithDeclaringTypeName = true
  )]
  [MemberDeclarationTestCase(
    "public void M30(Environment.SpecialFolder x = System.Environment.SpecialFolder.UserProfile) {}",
    ParameterWithNamespace = false,
    ParameterWithDeclaringTypeName = true,
    ValueWithNamespace = true,
    ValueWithDeclaringTypeName = true
  )]
  [MemberDeclarationTestCase(
    "public void M30(SpecialFolder x = System.Environment.SpecialFolder.UserProfile) {}",
    ParameterWithNamespace = false,
    ParameterWithDeclaringTypeName = false,
    ValueWithNamespace = true,
    ValueWithDeclaringTypeName = true
  )]
  [MemberDeclarationTestCase(
    "public void M30(SpecialFolder x = Environment.SpecialFolder.UserProfile) {}",
    ParameterWithNamespace = false,
    ParameterWithDeclaringTypeName = false,
    ValueWithNamespace = false,
    ValueWithDeclaringTypeName = true
  )]
  [MemberDeclarationTestCase(
    "public void M30(SpecialFolder x = SpecialFolder.UserProfile) {}",
    ParameterWithNamespace = false,
    ParameterWithDeclaringTypeName = false,
    ValueWithNamespace = false,
    ValueWithDeclaringTypeName = false
  )]
  public void M30(Environment.SpecialFolder x = Environment.SpecialFolder.UserProfile) { }
}

public class EnumsDefaultEnumValue {
  [MemberDeclarationTestCase(
    $"public void M({NSTestCases.NS.Namespace}.{nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember)}.{nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember.E)} x = default({NSTestCases.NS.Namespace}.{nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember)}.{nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember.E)}))",
    ParameterWithNamespace = true,
    ParameterWithDeclaringTypeName = true,
    ValueWithNamespace = true,
    ValueWithDeclaringTypeName = true,
    ValueUseDefaultLiteral = false,
    MethodBody = MethodBodyOption.None
  )]
  [MemberDeclarationTestCase(
    $"public void M({nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember)}.{nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember.E)} x = default({NSTestCases.NS.Namespace}.{nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember)}.{nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember.E)}))",
    ParameterWithNamespace = false,
    ParameterWithDeclaringTypeName = true,
    ValueWithNamespace = true,
    ValueWithDeclaringTypeName = true,
    ValueUseDefaultLiteral = false,
    MethodBody = MethodBodyOption.None
  )]
  [MemberDeclarationTestCase(
    $"public void M({nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember)}.{nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember.E)} x = default({nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember)}.{nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember.E)}))",
    ParameterWithNamespace = false,
    ParameterWithDeclaringTypeName = true,
    ValueWithNamespace = false,
    ValueWithDeclaringTypeName = true,
    ValueUseDefaultLiteral = false,
    MethodBody = MethodBodyOption.None
  )]
  [MemberDeclarationTestCase(
    $"public void M({nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember)}.{nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember.E)} x = default({nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember.E)}))",
    ParameterWithNamespace = false,
    ParameterWithDeclaringTypeName = true,
    ValueWithNamespace = false,
    ValueWithDeclaringTypeName = false,
    ValueUseDefaultLiteral = false,
    MethodBody = MethodBodyOption.None
  )]
  [MemberDeclarationTestCase(
    $"public void M({nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember)}.{nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember.E)} x = default({NSTestCases.NS.Namespace}.{nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember.E)}))",
    ParameterWithNamespace = false,
    ParameterWithDeclaringTypeName = true,
    ValueWithNamespace = true,
    ValueWithDeclaringTypeName = false,
    ValueUseDefaultLiteral = false,
    MethodBody = MethodBodyOption.None
  )]
  [MemberDeclarationTestCase(
    $"public void M({nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember)}.{nameof(NSTestCases.CNestedEnumWithoutDefaultValueMember.E)} x = default)",
    ParameterWithNamespace = false,
    ParameterWithDeclaringTypeName = true,
    ValueWithNamespace = true,
    ValueWithDeclaringTypeName = true,
    ValueUseDefaultLiteral = true,
    MethodBody = MethodBodyOption.None
  )]
  public void M(NSTestCases.CNestedEnumWithoutDefaultValueMember.E x = default) { }
}

public class Nullables {
  [MemberDeclarationTestCase("public void M1(int? x = 0) {}")] public void M1(int? x = 0) { }
  [MemberDeclarationTestCase("public void M2(int? x = null) {}")] public void M2(int? x = null) { }
  [MemberDeclarationTestCase("public void M3(int? x = null) {}")] public void M3(int? x = default(int?)) { }
  [MemberDeclarationTestCase("public void M4(int? x = int.MaxValue) {}")] public void M4(int? x = int.MaxValue) { }
  [MemberDeclarationTestCase("public void M5(int? x = int.MinValue) {}")] public void M5(int? x = int.MinValue) { }

  [MemberDeclarationTestCase("public void M6(System.Guid? x = null) {}")] public void M6(Guid? x = null) { }
}

public class ValueTypes {
  [MemberDeclarationTestCase("public void M1(System.DateTimeOffset x = default(System.DateTimeOffset)) {}", ParameterWithNamespace = true, ValueWithNamespace = true, ValueUseDefaultLiteral = false)]
  [MemberDeclarationTestCase("public void M1(System.DateTimeOffset x = default(DateTimeOffset)) {}", ParameterWithNamespace = true, ValueWithNamespace = false, ValueUseDefaultLiteral = false)]
  [MemberDeclarationTestCase("public void M1(DateTimeOffset x = default(DateTimeOffset)) {}", ParameterWithNamespace = false, ValueWithNamespace = false, ValueUseDefaultLiteral = false)]
  [MemberDeclarationTestCase("public void M1(System.DateTimeOffset x = default) {}", ValueUseDefaultLiteral = true)]
  public void M1(DateTimeOffset x = default(DateTimeOffset)) { }

  [MemberDeclarationTestCase("public void M2(System.Guid x = default(System.Guid)) {}", ValueUseDefaultLiteral = false)]
  [MemberDeclarationTestCase("public void M2(System.Guid x = default) {}", ValueUseDefaultLiteral = true)]
  public void M2(Guid x = default(Guid)) { }

  [MemberDeclarationTestCase("public void M3(System.Threading.CancellationToken x = default(System.Threading.CancellationToken)) {}", ValueUseDefaultLiteral = false)]
  [MemberDeclarationTestCase("public void M3(System.Threading.CancellationToken x = default) {}", ValueUseDefaultLiteral = true)]
  public void M3(CancellationToken x = default(CancellationToken)) { }
}
