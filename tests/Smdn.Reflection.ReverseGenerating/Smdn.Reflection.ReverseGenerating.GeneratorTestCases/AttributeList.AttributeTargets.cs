// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CS0067, CS0414, CS8597

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.AttributeList.AttributeTargets;

[AttributeListTestCase("[System.Obsolete]")][Obsolete] class C { }
[AttributeListTestCase("[System.Obsolete]")][Obsolete] struct S { }
[AttributeListTestCase("[System.Obsolete]")][Obsolete] delegate void D();
[AttributeListTestCase("[System.Obsolete]")][Obsolete] interface I { }
[AttributeListTestCase("[System.Obsolete]")][Obsolete] enum E { }

[AttributeListTestCase("")] class None { }

class AttributeTargetsMember {
  [AttributeListTestCase("[System.Obsolete]")][Obsolete] public AttributeTargetsMember() { }
  [AttributeListTestCase("[System.Obsolete]")][Obsolete] public void M() { }
  [AttributeListTestCase("[System.Obsolete]")][Obsolete] public int P { get; set; }
  [AttributeListTestCase("[System.Obsolete]")][Obsolete] public event EventHandler E = null;
  [AttributeListTestCase("[System.Obsolete]")][Obsolete] public int F = default;

  [AttributeListTestCase("")] public void None() { }
}

class AttributeTargetsGenericTypeParameter {
  [AttributeUsage(System.AttributeTargets.GenericParameter)] public class GP0Attribute : Attribute { }
  [AttributeUsage(System.AttributeTargets.GenericParameter)] public class GP1Attribute : Attribute { }

  [TypeDeclarationTestCase(
    "public class C<[GP0] T0, [GP1] T1, T2, [GP0] [GP1] T3>",
    TypeWithNamespace = false,
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeGenericParameterFormat = AttributeSectionFormat.Discrete,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [TypeDeclarationTestCase(
    "public class C<[GP0] T0, [GP1] T1, T2, [GP0, GP1] T3>",
    TypeWithNamespace = false,
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeGenericParameterFormat = AttributeSectionFormat.List,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  public class C<
    [AttributeListTestCase("[GP0]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
    [GP0]
    T0,

    [AttributeListTestCase("[GP1]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
    [GP1]
    T1,

    [AttributeListTestCase("", AttributeWithNamespace = false)]
    T2,

    [AttributeListTestCase("[GP0], [GP1]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false, AttributeGenericParameterFormat = AttributeSectionFormat.Discrete)]
    [AttributeListTestCase("[GP0, GP1]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false, AttributeGenericParameterFormat = AttributeSectionFormat.List)]
    [GP0]
    [GP1]
    T3
  > { }

#nullable enable annotations
  [TypeDeclarationTestCase(
    "public class GenericTypeParameter_NullableEnableContext<[System.Runtime.CompilerServices.Nullable(2)] T>",
    TypeWithNamespace = false,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  public class GenericTypeParameter_NullableEnableContext<
    [AttributeListTestCase("[Nullable(2)]", AttributeWithNamespace = false)]
    T
  > { }
#nullable restore
#nullable disable annotations
  [TypeDeclarationTestCase(
    "public class GenericTypeParameter_NullableDisableContext<T>",
    TypeWithNamespace = false,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  public class GenericTypeParameter_NullableDisableContext<
    [AttributeListTestCase("", AttributeWithNamespace = false)]
    T
  > { }
#nullable restore

  [TypeDeclarationTestCase(
    "public class ConstraintStruct<[GP0] T> where T : struct",
    TypeWithNamespace = false,
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  public class ConstraintStruct<
    [AttributeListTestCase("[GP0]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
    [GP0]
    T
  > where T : struct { }

  [TypeDeclarationTestCase(
    "public class ConstraintUnmanaged<[GP0] [IsUnmanaged] T> where T : unmanaged",
    TypeWithNamespace = false,
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  public class ConstraintUnmanaged<
    [AttributeListTestCase("[GP0], [IsUnmanaged]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
    [GP0]
    T
  > where T : unmanaged { }
}

class AttributeTargetsGenericMethodParameter {
  [AttributeUsage(System.AttributeTargets.GenericParameter)] public class GP0Attribute : Attribute { }
  [AttributeUsage(System.AttributeTargets.GenericParameter)] public class GP1Attribute : Attribute { }

  [MemberDeclarationTestCase(
    "public void M<[GP0] T0, [GP1] T1, T2, [GP0] [GP1] T3>(T0 p0, T1 p1, T2 p2, T3 p3)",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeGenericParameterFormat = AttributeSectionFormat.Discrete,
    TypeWithNamespace = false,
    MethodBody = MethodBodyOption.None,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "public void M<[GP0] T0, [GP1] T1, T2, [GP0, GP1] T3>(T0 p0, T1 p1, T2 p2, T3 p3)",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeGenericParameterFormat = AttributeSectionFormat.List,
    TypeWithNamespace = false,
    MethodBody = MethodBodyOption.None,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  public void M<
    [AttributeListTestCase("[GP0]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
    [GP0]
    T0,

    [AttributeListTestCase("[GP1]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
    [GP1]
    T1,

    [AttributeListTestCase("", AttributeWithNamespace = false)]
    T2,

    [AttributeListTestCase("[GP0], [GP1]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false, AttributeGenericParameterFormat = AttributeSectionFormat.Discrete)]
    [AttributeListTestCase("[GP0, GP1]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false, AttributeGenericParameterFormat = AttributeSectionFormat.List)]
    [GP0]
    [GP1]
    T3
  >(T0 p0, T1 p1, T2 p2, T3 p3) => throw new NotImplementedException();

#nullable enable annotations
  [MemberDeclarationTestCase(
    "public void GenericMethodParameter_NullableEnableContext<[Nullable(2)] T>(T p)",
    AttributeWithNamespace = false,
    TypeWithNamespace = false,
    MethodBody = MethodBodyOption.None,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  public void GenericMethodParameter_NullableEnableContext<
    [AttributeListTestCase("[Nullable(2)]", AttributeWithNamespace = false)]
    T
  >(T p) { }
#nullable restore
#nullable disable annotations
  [MemberDeclarationTestCase(
    "public void GenericMethodParameter_NullableDisableContext<T>(T p)",
    AttributeWithNamespace = false,
    TypeWithNamespace = false,
    MethodBody = MethodBodyOption.None,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  public void GenericMethodParameter_NullableDisableContext<
    [AttributeListTestCase("", AttributeWithNamespace = false)]
    T
  >(T p) { }
#nullable restore

  [MemberDeclarationTestCase(
    "public void ConstraintStruct<[GP0] T>(T p) where T : struct",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    TypeWithNamespace = false,
    MethodBody = MethodBodyOption.None,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  public void ConstraintStruct<
    [AttributeListTestCase("[GP0]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
    [GP0]
    T
  >(T p) where T : struct { }

  [MemberDeclarationTestCase(
    "public void ConstraintUnmanaged<[GP0] [IsUnmanaged] T>(T p) where T : unmanaged",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    TypeWithNamespace = false,
    MethodBody = MethodBodyOption.None,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  public void ConstraintUnmanaged<
    [AttributeListTestCase("[GP0], [IsUnmanaged]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
    [GP0]
    T
  >(T p) where T : unmanaged { }
}

class AttributeTargetsPropertyAccessorMethod {
  [AttributeUsage(System.AttributeTargets.Method)]
  public class GetMethodAttribute : Attribute { }

  [AttributeUsage(System.AttributeTargets.Method)]
  public class SetMethodAttribute : Attribute { }

  [MemberDeclarationTestCase(
    "public int P0 { [GetMethod] [Obsolete] get; [SetMethod] [DebuggerHidden] set; }",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeAccessorFormat = AttributeSectionFormat.Discrete,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "public int P0 { [GetMethod, Obsolete] get; [SetMethod, DebuggerHidden] set; }",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeAccessorFormat = AttributeSectionFormat.List,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
  [AttributeListTestCase("[System.Obsolete]")]
  public int P0 {
    [AttributeListTestCase(
      "[GetMethod], [Obsolete]",
      AttributeWithNamespace = false,
      AttributeWithDeclaringTypeName = false,
      AttributeAccessorFormat = AttributeSectionFormat.Discrete
    )]
    [AttributeListTestCase(
      "[GetMethod, Obsolete]",
      AttributeWithNamespace = false,
      AttributeWithDeclaringTypeName = false,
      AttributeAccessorFormat = AttributeSectionFormat.List
    )]
    [Obsolete]
    [GetMethod]
    get => throw null;

    [AttributeListTestCase(
      "[SetMethod], [DebuggerHidden]",
      AttributeWithNamespace = false,
      AttributeWithDeclaringTypeName = false,
      AttributeAccessorFormat = AttributeSectionFormat.Discrete
    )]
    [AttributeListTestCase(
      "[SetMethod, DebuggerHidden]",
      AttributeWithNamespace = false,
      AttributeWithDeclaringTypeName = false,
      AttributeAccessorFormat = AttributeSectionFormat.List
    )]
    [DebuggerHidden]
    [SetMethod]
    set => throw null;
  }
}

class AttributeTargetsPropertyBackingField {
  [MemberDeclarationTestCase(
    "[field: DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)] [field: Obsolete] [field: CompilerGenerated] public int P0 { [CompilerGenerated] get; [CompilerGenerated] set; }",
    AttributeWithNamespace = false,
    AttributeBackingFieldFormat = AttributeSectionFormat.Discrete,
    TypeWithNamespace = false,
    ValueWithNamespace = true,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "[field: DebuggerBrowsable(DebuggerBrowsableState.Never)] [field: Obsolete] [field: CompilerGenerated] public int P0 { [CompilerGenerated] get; [CompilerGenerated] set; }",
    AttributeWithNamespace = false,
    AttributeBackingFieldFormat = AttributeSectionFormat.Discrete,
    TypeWithNamespace = false,
    ValueWithNamespace = false,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "[field: DebuggerBrowsable(DebuggerBrowsableState.Never), Obsolete, CompilerGenerated] public int P0 { [CompilerGenerated] get; [CompilerGenerated] set; }",
    AttributeWithNamespace = false,
    AttributeBackingFieldFormat = AttributeSectionFormat.List,
    TypeWithNamespace = false,
    ValueWithNamespace = false,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [AttributeListTestCase("[System.Obsolete]")]
  [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
  [field: AttributeListTestCase(
    "[field: System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)], [field: System.Obsolete], [field: System.Runtime.CompilerServices.CompilerGenerated]",
    AttributeBackingFieldFormat = AttributeSectionFormat.Discrete
  )]
  [field: AttributeListTestCase(
    "[field: System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never), System.Obsolete, System.Runtime.CompilerServices.CompilerGenerated]",
    AttributeBackingFieldFormat = AttributeSectionFormat.List
  )]
  [field: Obsolete]
  public int P0 { get; set; }
}

[AttributeUsage(System.AttributeTargets.Method)]
public class AddMethodAttribute : Attribute { }

[AttributeUsage(System.AttributeTargets.Method)]
public class RemoveMethodAttribute : Attribute { }

[AttributeUsage(System.AttributeTargets.Method)]
public class CustomAccessorAttribute : Attribute { }

[AttributeUsage(System.AttributeTargets.Method)]
public class CompilerGeneratedAccessorAttribute : Attribute { }

class AttributeTargetsEventAccessorMethod {
  [MemberDeclarationTestCase(
    "public event System.EventHandler E0 { [AddMethod] [CustomAccessor] add; [RemoveMethod] [DebuggerHidden] remove; }",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeAccessorFormat = AttributeSectionFormat.Discrete,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "public event System.EventHandler E0 { [AddMethod, CustomAccessor] add; [RemoveMethod, DebuggerHidden] remove; }",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeAccessorFormat = AttributeSectionFormat.List,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
  [AttributeListTestCase("[System.Obsolete]")]
  public event EventHandler E0 {
    [AttributeListTestCase(
      "[AddMethod], [CustomAccessor]",
      AttributeWithNamespace = false,
      AttributeWithDeclaringTypeName = false,
      AttributeAccessorFormat = AttributeSectionFormat.Discrete
    )]
    [AttributeListTestCase(
      "[AddMethod, CustomAccessor]",
      AttributeWithNamespace = false,
      AttributeWithDeclaringTypeName = false,
      AttributeAccessorFormat = AttributeSectionFormat.List
    )]
    [CustomAccessor]
    [AddMethod]
    add => throw null;

    [AttributeListTestCase(
      "[RemoveMethod], [DebuggerHidden]",
      AttributeWithNamespace = false,
      AttributeWithDeclaringTypeName = false,
      AttributeAccessorFormat = AttributeSectionFormat.Discrete
    )]
    [AttributeListTestCase(
      "[RemoveMethod, DebuggerHidden]",
      AttributeWithNamespace = false,
      AttributeWithDeclaringTypeName = false,
      AttributeAccessorFormat = AttributeSectionFormat.List
    )]
    [DebuggerHidden]
    [RemoveMethod]
    remove => throw null;
  }

  [MemberDeclarationTestCase(
    "[field: DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)] [field: CompilerGenerated] public event System.EventHandler CompilerGeneratedAccessorWithAccessorAttributes { [CompilerGeneratedAccessor] [CompilerGenerated] add; [CompilerGeneratedAccessor] [CompilerGenerated] remove; }",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeAccessorFormat = AttributeSectionFormat.Discrete,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "[field: DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)] [field: CompilerGenerated] public event System.EventHandler CompilerGeneratedAccessorWithAccessorAttributes { [CompilerGeneratedAccessor, CompilerGenerated] add; [CompilerGeneratedAccessor, CompilerGenerated] remove; }",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeAccessorFormat = AttributeSectionFormat.List,
    AttributeBackingFieldFormat = AttributeSectionFormat.Discrete,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "[field: DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never), CompilerGenerated] public event System.EventHandler CompilerGeneratedAccessorWithAccessorAttributes { [CompilerGeneratedAccessor, CompilerGenerated] add; [CompilerGeneratedAccessor, CompilerGenerated] remove; }",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeAccessorFormat = AttributeSectionFormat.List,
    AttributeBackingFieldFormat = AttributeSectionFormat.List,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [method: CompilerGeneratedAccessor]
  public event EventHandler CompilerGeneratedAccessorWithAccessorAttributes;
}

interface IAttributeTargetsEventAccessorMethod {
  [MemberDeclarationTestCase(
    "event System.EventHandler E;",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  event EventHandler E;

  [MemberDeclarationTestCase(
    "event System.EventHandler EAccessorAttributes { [CustomAccessor] [CompilerGenerated] add; [CustomAccessor] [CompilerGenerated] remove; }",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeAccessorFormat = AttributeSectionFormat.Discrete,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "event System.EventHandler EAccessorAttributes { [CustomAccessor, CompilerGenerated] add; [CustomAccessor, CompilerGenerated] remove; }",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeAccessorFormat = AttributeSectionFormat.List,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [method: CustomAccessor]
  event EventHandler EAccessorAttributes;
}

class AttributeTargetsEventBackingField {
  [MemberDeclarationTestCase(
    "[field: DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)] [field: Obsolete] [field: CompilerGenerated] public event System.EventHandler E0;",
    AttributeWithNamespace = false,
    AttributeBackingFieldFormat = AttributeSectionFormat.Discrete,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "[field: DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never), Obsolete, CompilerGenerated] public event System.EventHandler E0;",
    AttributeWithNamespace = false,
    AttributeBackingFieldFormat = AttributeSectionFormat.List,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
  [AttributeListTestCase("[System.Obsolete]")]
  [field: AttributeListTestCase(
    "[field: System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)], [field: System.Obsolete], [field: System.Runtime.CompilerServices.CompilerGenerated]",
    AttributeBackingFieldFormat = AttributeSectionFormat.Discrete
  )]
  [field: AttributeListTestCase(
    "[field: System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never), System.Obsolete, System.Runtime.CompilerServices.CompilerGenerated]",
    AttributeBackingFieldFormat = AttributeSectionFormat.List
  )]
  [field: Obsolete]
  public event EventHandler E0;
}

[AttributeUsage(System.AttributeTargets.ReturnValue)]
public class ReturnParameterAttribute : Attribute {
  public int CtorArg;
  public int NamedArg;

  public ReturnParameterAttribute(int ctorArg)
  {
    CtorArg = ctorArg;
  }
}

class AttributeTargetsReturnParameter {
#if NETFRAMEWORK
  static string M0_ExpectedResult =>
    RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.Ordinal)
      ? "[return: System.Runtime.InteropServices.MarshalAs(" +
        "System.Runtime.InteropServices.UnmanagedType.Bool, "+
        "ArraySubType = default(System.Runtime.InteropServices.UnmanagedType), " +
        "SizeParamIndex = 0, " +
        "SizeConst = 0, " +
        "IidParameterIndex = 0, " +
        "SafeArraySubType = System.Runtime.InteropServices.VarEnum.VT_EMPTY" +
        ")]"
      : "[return: System.Runtime.InteropServices.MarshalAs(" +
        "System.Runtime.InteropServices.UnmanagedType.Bool)" +
        "]";

  [return: AttributeListTestCase(
    expected: null,
    ExpectedValueGeneratorType = typeof(AttributeTargetsReturnParameter),
    ExpectedValueGeneratorMemberName = nameof(M0_ExpectedResult)
  )]
  [return: MarshalAs(UnmanagedType.Bool)]
  public bool M0() => throw new NotImplementedException();
#endif

  [MemberDeclarationTestCase(
    "[return: ReturnParameter(0, NamedArg = 1)] [return: XmlIgnore] public bool M1()",
    AttributeWithNamespace = false,
    AttributeMethodParameterFormat = AttributeSectionFormat.Discrete,
    MethodBody = MethodBodyOption.None,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "[return: ReturnParameter(0, NamedArg = 1), XmlIgnore] public bool M1()",
    AttributeWithNamespace = false,
    AttributeMethodParameterFormat = AttributeSectionFormat.List,
    MethodBody = MethodBodyOption.None,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
  [AttributeListTestCase("[System.Obsolete]")]
  [return: AttributeListTestCase(
    "[return: ReturnParameter(0, NamedArg = 1)], [return: XmlIgnore]",
    AttributeMethodParameterFormat = AttributeSectionFormat.Discrete,
    AttributeWithNamespace = false
  )]
  [return: AttributeListTestCase(
    "[return: ReturnParameter(0, NamedArg = 1), XmlIgnore]",
    AttributeMethodParameterFormat = AttributeSectionFormat.List,
    AttributeWithNamespace = false
  )]
  [return: System.Xml.Serialization.XmlIgnore]
  [return: ReturnParameter(0, NamedArg = 1)]
  public bool M1() => throw new NotImplementedException();

  [TypeDeclarationTestCase(
    "[return: ReturnParameter(0, NamedArg = 1)] [return: XmlIgnore] public delegate bool D0();",
    AttributeDelegateParameterFormat = AttributeSectionFormat.Discrete,
    AttributeWithNamespace = false,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [TypeDeclarationTestCase(
    "[return: ReturnParameter(0, NamedArg = 1), XmlIgnore] public delegate bool D0();",
    AttributeDelegateParameterFormat = AttributeSectionFormat.List,
    AttributeWithNamespace = false,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [Obsolete] // must not be shown in the result of TypeDeclarationTestCase
  [AttributeListTestCase("[System.Obsolete]")]
  [return: AttributeListTestCase(
    "[return: ReturnParameter(0, NamedArg = 1)], [return: XmlIgnore]",
    AttributeDelegateParameterFormat = AttributeSectionFormat.Discrete,
    AttributeWithNamespace = false
  )]
  [return: AttributeListTestCase(
    "[return: ReturnParameter(0, NamedArg = 1), XmlIgnore]",
    AttributeDelegateParameterFormat = AttributeSectionFormat.List,
    AttributeWithNamespace = false
  )]
  [return: System.Xml.Serialization.XmlIgnore]
  [return: ReturnParameter(0, NamedArg = 1)]
  public delegate bool D0();

  [MemberDeclarationTestCase(
    "public int P0 { [return: ReturnParameter(0, NamedArg = 1)] [return: XmlIgnore] get; }",
    AttributeWithNamespace = false,
    AttributeAccessorParameterFormat = AttributeSectionFormat.Discrete,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "public int P0 { [return: ReturnParameter(0, NamedArg = 1), XmlIgnore] get; }",
    AttributeWithNamespace = false,
    AttributeAccessorParameterFormat = AttributeSectionFormat.List,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
  [AttributeListTestCase("[System.Obsolete]")]
  public int P0 {
    [return: AttributeListTestCase(
      "[return: ReturnParameter(0, NamedArg = 1)], [return: XmlIgnore]",
      AttributeWithNamespace = false,
      AttributeAccessorParameterFormat = AttributeSectionFormat.Discrete
    )]
    [return: AttributeListTestCase(
      "[return: ReturnParameter(0, NamedArg = 1), XmlIgnore]",
      AttributeWithNamespace = false,
      AttributeAccessorParameterFormat = AttributeSectionFormat.List
    )]
    [return: System.Xml.Serialization.XmlIgnore]
    [return: ReturnParameter(0, NamedArg = 1)]
    get => 0;
  }
}

[AttributeUsage(System.AttributeTargets.Parameter)]
public class ParameterAttribute : Attribute {
  public int CtorArg;
  public int NamedArg;

  public ParameterAttribute(int ctorArg)
  {
    CtorArg = ctorArg;
  }
}

class AttributeTargetsParameter {
  [MemberDeclarationTestCase(
    "public bool M([CallerFilePath] [Optional] string sourceFilePath = null, [CallerLineNumber] [Optional] int sourceLineNumber = 0)",
    AttributeWithNamespace = false,
    AttributeMethodParameterFormat = AttributeSectionFormat.Discrete,
    MethodBody = MethodBodyOption.None,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "public bool M([CallerFilePath, Optional] string sourceFilePath = null, [CallerLineNumber, Optional] int sourceLineNumber = 0)",
    AttributeWithNamespace = false,
    AttributeMethodParameterFormat = AttributeSectionFormat.List,
    MethodBody = MethodBodyOption.None,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
  public bool M(
    [AttributeListTestCase(
      "[System.Runtime.CompilerServices.CallerFilePath], [System.Runtime.InteropServices.Optional]",
      AttributeWithNamespace = true
    )]
    [AttributeListTestCase(
      "[CallerFilePath], [Optional]",
      AttributeWithNamespace = false,
      AttributeMethodParameterFormat = AttributeSectionFormat.Discrete
    )]
    [AttributeListTestCase(
      "[CallerFilePath, Optional]",
      AttributeWithNamespace = false,
      AttributeMethodParameterFormat = AttributeSectionFormat.List
    )]
    [CallerFilePath]
    string sourceFilePath = default,

    [AttributeListTestCase(
      "[System.Runtime.CompilerServices.CallerLineNumber], [System.Runtime.InteropServices.Optional]",
      AttributeWithNamespace = true
    )]
    [AttributeListTestCase(
      "[CallerLineNumber], [Optional]",
      AttributeWithNamespace = false
    )]
    [CallerLineNumber]
    int sourceLineNumber = default
  ) => throw new NotImplementedException();

  [TypeDeclarationTestCase(
    "public delegate bool D([Parameter(0, NamedArg = 1)] [Optional] int arg = 0);",
    AttributeWithNamespace = false,
    AttributeDelegateParameterFormat = AttributeSectionFormat.Discrete,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [TypeDeclarationTestCase(
    "public delegate bool D([Parameter(0, NamedArg = 1), Optional] int arg = 0);",
    AttributeWithNamespace = false,
    AttributeDelegateParameterFormat = AttributeSectionFormat.List,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [Obsolete] // must not be shown in the result of TypeDeclarationTestCase
  [AttributeListTestCase("[System.Obsolete]")]
  public delegate bool D(
    [AttributeListTestCase(
      "[Parameter(0, NamedArg = 1)], [Optional]",
      AttributeWithNamespace = false,
      AttributeMethodParameterFormat = AttributeSectionFormat.Discrete
    )]
    [AttributeListTestCase(
      "[Parameter(0, NamedArg = 1), Optional]",
      AttributeWithNamespace = false,
      AttributeMethodParameterFormat = AttributeSectionFormat.List
    )]
    [Parameter(0, NamedArg = 1)]
    int arg = 0
  );

  [MemberDeclarationTestCase(
    "public int P0 { [param: Parameter(0, NamedArg = 1)] [param: XmlIgnore] set; }",
    AttributeWithNamespace = false,
    AttributeAccessorParameterFormat = AttributeSectionFormat.Discrete,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "public int P0 { [param: Parameter(0, NamedArg = 1), XmlIgnore] set; }",
    AttributeWithNamespace = false,
    AttributeAccessorParameterFormat = AttributeSectionFormat.List,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
  [AttributeListTestCase("[System.Obsolete]")]
  public int P0 {
    [param: AttributeListTestCase(
      "[param: Parameter(0, NamedArg = 1)], [param: XmlIgnore]",
      AttributeWithNamespace = false,
      AttributeAccessorParameterFormat = AttributeSectionFormat.Discrete
    )]
    [param: AttributeListTestCase(
      "[param: Parameter(0, NamedArg = 1), XmlIgnore]",
      AttributeWithNamespace = false,
      AttributeAccessorParameterFormat = AttributeSectionFormat.List
    )]
    [param: System.Xml.Serialization.XmlIgnore]
    [param: Parameter(0, NamedArg = 1)]
    set => throw null;
  }

  [MemberDeclarationTestCase(
    "public event System.EventHandler E0 { [param: Parameter(0, NamedArg = 1)] [param: XmlIgnore] add; remove; }",
    AttributeWithNamespace = false,
    AttributeAccessorParameterFormat = AttributeSectionFormat.Discrete,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [MemberDeclarationTestCase(
    "public event System.EventHandler E0 { [param: Parameter(0, NamedArg = 1), XmlIgnore] add; remove; }",
    AttributeWithNamespace = false,
    AttributeAccessorParameterFormat = AttributeSectionFormat.List,
    TypeOfAttributeTypeFilterFunc = typeof(GeneratorTests),
    NameOfAttributeTypeFilterFunc = nameof(GeneratorTests.ExceptTestCaseAttributeFilter)
  )]
  [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
  [AttributeListTestCase("[System.Obsolete]")]
  public event EventHandler E0 {
    [param: AttributeListTestCase(
      "[param: Parameter(0, NamedArg = 1)], [param: XmlIgnore]",
      AttributeWithNamespace = false,
      AttributeAccessorParameterFormat = AttributeSectionFormat.Discrete
    )]
    [param: AttributeListTestCase(
      "[param: Parameter(0, NamedArg = 1), XmlIgnore]",
      AttributeWithNamespace = false,
      AttributeAccessorParameterFormat = AttributeSectionFormat.List
    )]
    [param: System.Xml.Serialization.XmlIgnore]
    [param: Parameter(0, NamedArg = 1)]
    add => throw null;

    remove => throw null;
  }
}
