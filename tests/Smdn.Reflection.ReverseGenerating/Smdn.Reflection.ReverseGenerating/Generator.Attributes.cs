// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CS0067, CS8597

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
  public class AttributeTestCaseAttribute : GeneratorTestCaseAttribute {
    public AttributeTestCaseAttribute(
      string expected,
      [CallerFilePath] string sourceFilePath = null,
      [CallerLineNumber] int lineNumber = 0
    )
      : base(
        expected,
        sourceFilePath,
        lineNumber
      )
    {
    }
  }

  namespace TestCases {
    namespace Attributes {
      namespace Options {
        namespace WithNamespace {
          [AttributeTestCase("[System.Obsolete]", AttributeWithNamespace = true)]
          [Obsolete] class C0 { }

          [AttributeTestCase("[Obsolete]", AttributeWithNamespace = false)]
          [Obsolete] class C1 { }
        }

        namespace WithNamedArguments {
          [AttributeTestCase("[Obsolete]", AttributeWithNamedArguments = true, AttributeWithNamespace = false)]
          [Obsolete] class C0 { }

          [AttributeTestCase("[Obsolete(message: \"message\")]", AttributeWithNamedArguments = true, AttributeWithNamespace = false)]
          [Obsolete(message: "message")] class C1 { }

          [AttributeTestCase("[Obsolete(\"message\")]", AttributeWithNamedArguments = false, AttributeWithNamespace = false)]
          [Obsolete(message: "message")] class C2 { }
        }

        namespace WithDeclaringTypeName {
          public class C {
            [AttributeUsage(System.AttributeTargets.Class)]
            public class ClassAttribute : Attribute { }

            [AttributeTestCase("[C.Class]", AttributeWithDeclaringTypeName = true, AttributeWithNamespace = false)]
            [Class] public class C0 { }

            [AttributeTestCase("[Class]", AttributeWithDeclaringTypeName = false, AttributeWithNamespace = false)]
            [Class] public class C1 { }

            [AttributeUsage(System.AttributeTargets.Method)]
            public class MethodAttribute : Attribute { }

            [AttributeTestCase("[C.Method]", AttributeWithDeclaringTypeName = true, AttributeWithNamespace = false)]
            [Method] public void M0() => throw null;

            [AttributeTestCase("[Method]", AttributeWithDeclaringTypeName = false, AttributeWithNamespace = false)]
            [Method] public void M1() => throw null;
          }
        }
      }

      namespace AttributeTargets {
        [AttributeTestCase("[System.Obsolete]")][Obsolete] class C { }
        [AttributeTestCase("[System.Obsolete]")][Obsolete] struct S { }
        [AttributeTestCase("[System.Obsolete]")][Obsolete] delegate void D();
        [AttributeTestCase("[System.Obsolete]")][Obsolete] interface I { }
        [AttributeTestCase("[System.Obsolete]")][Obsolete] enum E { }

        [AttributeTestCase("")] class None { }

#pragma warning disable CS0414
        class AttributeTargetsMember {
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public AttributeTargetsMember() { }
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public void M() { }
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public int P { get; set; }
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public event EventHandler E = null;
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public int F = default;

          [AttributeTestCase("")] public void None() { }
        }

        class AttributeTargetsGenericTypeParameter {
          [AttributeUsage(System.AttributeTargets.GenericParameter)] public class GP0Attribute : Attribute { }
          [AttributeUsage(System.AttributeTargets.GenericParameter)] public class GP1Attribute : Attribute { }

          [TypeDeclarationTestCase(
            "public class C<[GP0] T0, [GP1] T1, T2, [GP0] [GP1] T3>",
            TypeWithNamespace = false,
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false,
            AttributeGenericParameterFormat = AttributeSectionFormat.Discrete
          )]
          [TypeDeclarationTestCase(
            "public class C<[GP0] T0, [GP1] T1, T2, [GP0, GP1] T3>",
            TypeWithNamespace = false,
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false,
            AttributeGenericParameterFormat = AttributeSectionFormat.List
          )]
          public class C<
            [AttributeTestCase("[GP0]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
            [GP0]
            T0,

            [AttributeTestCase("[GP1]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
            [GP1]
            T1,

            [AttributeTestCase("", AttributeWithNamespace = false)]
            T2,

            [AttributeTestCase("[GP0], [GP1]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false, AttributeGenericParameterFormat = AttributeSectionFormat.Discrete)]
            [AttributeTestCase("[GP0, GP1]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false, AttributeGenericParameterFormat = AttributeSectionFormat.List)]
            [GP0]
            [GP1]
            T3
          > { }

#nullable enable annotations
          [TypeDeclarationTestCase(
            "public class GenericTypeParameter_NullableEnableContext<[System.Runtime.CompilerServices.Nullable(2)] T>",
            TypeWithNamespace = false
          )]
          public class GenericTypeParameter_NullableEnableContext<
            [AttributeTestCase("[Nullable(2)]", AttributeWithNamespace = false)]
            T
          > { }
#nullable restore
#nullable disable annotations
          [TypeDeclarationTestCase(
            "public class GenericTypeParameter_NullableDisableContext<T>",
            TypeWithNamespace = false
          )]
          public class GenericTypeParameter_NullableDisableContext<
            [AttributeTestCase("", AttributeWithNamespace = false)]
            T
          > { }
#nullable restore

          [TypeDeclarationTestCase(
            "public class ConstraintStruct<[GP0] T> where T : struct",
            TypeWithNamespace = false,
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false
          )]
          public class ConstraintStruct<
            [AttributeTestCase("[GP0]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
            [GP0]
            T
          > where T : struct { }

          [TypeDeclarationTestCase(
            "public class ConstraintUnmanaged<[GP0] [IsUnmanaged] T> where T : unmanaged",
            TypeWithNamespace = false,
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false
          )]
          public class ConstraintUnmanaged<
            [AttributeTestCase("[GP0], [IsUnmanaged]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
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
            MethodBody = MethodBodyOption.None
          )]
          [MemberDeclarationTestCase(
            "public void M<[GP0] T0, [GP1] T1, T2, [GP0, GP1] T3>(T0 p0, T1 p1, T2 p2, T3 p3)",
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false,
            AttributeGenericParameterFormat = AttributeSectionFormat.List,
            TypeWithNamespace = false,
            MethodBody = MethodBodyOption.None
          )]
          public void M<
            [AttributeTestCase("[GP0]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
            [GP0]
            T0,

            [AttributeTestCase("[GP1]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
            [GP1]
            T1,

            [AttributeTestCase("", AttributeWithNamespace = false)]
            T2,

            [AttributeTestCase("[GP0], [GP1]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false, AttributeGenericParameterFormat = AttributeSectionFormat.Discrete)]
            [AttributeTestCase("[GP0, GP1]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false, AttributeGenericParameterFormat = AttributeSectionFormat.List)]
            [GP0]
            [GP1]
            T3
          >(T0 p0, T1 p1, T2 p2, T3 p3) => throw new NotImplementedException();

#nullable enable annotations
          [MemberDeclarationTestCase(
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
            "public void GenericMethodParameter_NullableEnableContext<[Nullable(2)] T>(T? p)",
#else
            "public void GenericMethodParameter_NullableEnableContext<[Nullable(2)] T>(T p)",
#endif
            AttributeWithNamespace = false,
            TypeWithNamespace = false,
            MethodBody = MethodBodyOption.None
          )]
          public void GenericMethodParameter_NullableEnableContext<
            [AttributeTestCase("[Nullable(2)]", AttributeWithNamespace = false)]
            T
          >(T p) { }
#nullable restore
#nullable disable annotations
          [MemberDeclarationTestCase(
            "public void GenericMethodParameter_NullableDisableContext<T>(T p)",
            AttributeWithNamespace = false,
            TypeWithNamespace = false,
            MethodBody = MethodBodyOption.None
          )]
          public void GenericMethodParameter_NullableDisableContext<
            [AttributeTestCase("", AttributeWithNamespace = false)]
            T
          >(T p) { }
#nullable restore

          [MemberDeclarationTestCase(
            "public void ConstraintStruct<[GP0] T>(T p) where T : struct",
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false,
            TypeWithNamespace = false,
            MethodBody = MethodBodyOption.None
          )]
          public void ConstraintStruct<
            [AttributeTestCase("[GP0]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
            [GP0]
            T
          >(T p) where T : struct { }

          [MemberDeclarationTestCase(
            "public void ConstraintUnmanaged<[GP0] [IsUnmanaged] T>(T p) where T : unmanaged",
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false,
            TypeWithNamespace = false,
            MethodBody = MethodBodyOption.None
          )]
          public void ConstraintUnmanaged<
            [AttributeTestCase("[GP0], [IsUnmanaged]", AttributeWithNamespace = false, AttributeWithDeclaringTypeName = false)]
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
            AttributeAccessorFormat = AttributeSectionFormat.Discrete
          )]
          [MemberDeclarationTestCase(
            "public int P0 { [GetMethod, Obsolete] get; [SetMethod, DebuggerHidden] set; }",
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false,
            AttributeAccessorFormat = AttributeSectionFormat.List
          )]
          [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
          [AttributeTestCase("[System.Obsolete]")]
          public int P0 {
            [AttributeTestCase(
              "[GetMethod], [Obsolete]",
              AttributeWithNamespace = false,
              AttributeWithDeclaringTypeName = false,
              AttributeAccessorFormat = AttributeSectionFormat.Discrete
            )]
            [AttributeTestCase(
              "[GetMethod, Obsolete]",
              AttributeWithNamespace = false,
              AttributeWithDeclaringTypeName = false,
              AttributeAccessorFormat = AttributeSectionFormat.List
            )]
            [Obsolete]
            [GetMethod]
            get => throw null;

            [AttributeTestCase(
              "[SetMethod], [DebuggerHidden]",
              AttributeWithNamespace = false,
              AttributeWithDeclaringTypeName = false,
              AttributeAccessorFormat = AttributeSectionFormat.Discrete
            )]
            [AttributeTestCase(
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
            "[field: DebuggerBrowsable(DebuggerBrowsableState.Never)] [field: Obsolete] [field: CompilerGenerated] public int P0 { [CompilerGenerated] get; [CompilerGenerated] set; }",
            AttributeWithNamespace = false,
            AttributeBackingFieldFormat = AttributeSectionFormat.Discrete,
            TypeWithNamespace = false
          )]
          [MemberDeclarationTestCase(
            "[field: DebuggerBrowsable(DebuggerBrowsableState.Never), Obsolete, CompilerGenerated] public int P0 { [CompilerGenerated] get; [CompilerGenerated] set; }",
            AttributeWithNamespace = false,
            AttributeBackingFieldFormat = AttributeSectionFormat.List,
            TypeWithNamespace = false
          )]
          [AttributeTestCase("[System.Obsolete]")]
          [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
          [field: AttributeTestCase(
            "[field: System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)], [field: System.Obsolete], [field: System.Runtime.CompilerServices.CompilerGenerated]",
            AttributeBackingFieldFormat = AttributeSectionFormat.Discrete
          )]
          [field: AttributeTestCase(
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
            AttributeAccessorFormat = AttributeSectionFormat.Discrete
          )]
          [MemberDeclarationTestCase(
            "public event System.EventHandler E0 { [AddMethod, CustomAccessor] add; [RemoveMethod, DebuggerHidden] remove; }",
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false,
            AttributeAccessorFormat = AttributeSectionFormat.List
          )]
          [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
          [AttributeTestCase("[System.Obsolete]")]
          public event EventHandler E0 {
            [AttributeTestCase(
              "[AddMethod], [CustomAccessor]",
              AttributeWithNamespace = false,
              AttributeWithDeclaringTypeName = false,
              AttributeAccessorFormat = AttributeSectionFormat.Discrete
            )]
            [AttributeTestCase(
              "[AddMethod, CustomAccessor]",
              AttributeWithNamespace = false,
              AttributeWithDeclaringTypeName = false,
              AttributeAccessorFormat = AttributeSectionFormat.List
            )]
            [CustomAccessor]
            [AddMethod]
            add => throw null;

            [AttributeTestCase(
              "[RemoveMethod], [DebuggerHidden]",
              AttributeWithNamespace = false,
              AttributeWithDeclaringTypeName = false,
              AttributeAccessorFormat = AttributeSectionFormat.Discrete
            )]
            [AttributeTestCase(
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
            AttributeAccessorFormat = AttributeSectionFormat.Discrete
          )]
          [MemberDeclarationTestCase(
            "[field: DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)] [field: CompilerGenerated] public event System.EventHandler CompilerGeneratedAccessorWithAccessorAttributes { [CompilerGeneratedAccessor, CompilerGenerated] add; [CompilerGeneratedAccessor, CompilerGenerated] remove; }",
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false,
            AttributeAccessorFormat = AttributeSectionFormat.List,
            AttributeBackingFieldFormat = AttributeSectionFormat.Discrete
          )]
          [MemberDeclarationTestCase(
            "[field: DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never), CompilerGenerated] public event System.EventHandler CompilerGeneratedAccessorWithAccessorAttributes { [CompilerGeneratedAccessor, CompilerGenerated] add; [CompilerGeneratedAccessor, CompilerGenerated] remove; }",
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false,
            AttributeAccessorFormat = AttributeSectionFormat.List,
            AttributeBackingFieldFormat = AttributeSectionFormat.List
          )]
          [method: CompilerGeneratedAccessor]
          public event EventHandler CompilerGeneratedAccessorWithAccessorAttributes;
        }

        interface IAttributeTargetsEventAccessorMethod {
          [MemberDeclarationTestCase(
            "event System.EventHandler E;",
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false
          )]
          event EventHandler E;

          [MemberDeclarationTestCase(
            "event System.EventHandler EAccessorAttributes { [CustomAccessor] [CompilerGenerated] add; [CustomAccessor] [CompilerGenerated] remove; }",
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false,
            AttributeAccessorFormat = AttributeSectionFormat.Discrete
          )]
          [MemberDeclarationTestCase(
            "event System.EventHandler EAccessorAttributes { [CustomAccessor, CompilerGenerated] add; [CustomAccessor, CompilerGenerated] remove; }",
            AttributeWithNamespace = false,
            AttributeWithDeclaringTypeName = false,
            AttributeAccessorFormat = AttributeSectionFormat.List
          )]
          [method: CustomAccessor]
          event EventHandler EAccessorAttributes;
        }

        class AttributeTargetsEventBackingField {
          [MemberDeclarationTestCase(
            "[field: DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)] [field: Obsolete] [field: CompilerGenerated] public event System.EventHandler E0;",
            AttributeWithNamespace = false,
            AttributeBackingFieldFormat = AttributeSectionFormat.Discrete
          )]
          [MemberDeclarationTestCase(
            "[field: DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never), Obsolete, CompilerGenerated] public event System.EventHandler E0;",
            AttributeWithNamespace = false,
            AttributeBackingFieldFormat = AttributeSectionFormat.List
          )]
          [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
          [AttributeTestCase("[System.Obsolete]")]
          [field: AttributeTestCase(
            "[field: System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)], [field: System.Obsolete], [field: System.Runtime.CompilerServices.CompilerGenerated]",
            AttributeBackingFieldFormat = AttributeSectionFormat.Discrete
          )]
          [field: AttributeTestCase(
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

          [return: AttributeTestCase(
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
            MethodBody = MethodBodyOption.None
          )]
          [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
          [AttributeTestCase("[System.Obsolete]")]
          [return: AttributeTestCase(
            "[return: ReturnParameter(0, NamedArg = 1)], [return: XmlIgnore]",
            AttributeWithNamespace = false
          )]
          [return: System.Xml.Serialization.XmlIgnore]
          [return: ReturnParameter(0, NamedArg = 1)]
          public bool M1() => throw new NotImplementedException();

          [TypeDeclarationTestCase(
            "[return: ReturnParameter(0, NamedArg = 1)] [return: XmlIgnore] public delegate bool D0();",
            AttributeWithNamespace = false
          )]
          [Obsolete] // must not be shown in the result of TypeDeclarationTestCase
          [AttributeTestCase("[System.Obsolete]")]
          [return: AttributeTestCase(
            "[return: ReturnParameter(0, NamedArg = 1)], [return: XmlIgnore]",
            AttributeWithNamespace = false
          )]
          [return: System.Xml.Serialization.XmlIgnore]
          [return: ReturnParameter(0, NamedArg = 1)]
          public delegate bool D0();

          [MemberDeclarationTestCase(
            "public int P0 { [return: ReturnParameter(0, NamedArg = 1)] [return: XmlIgnore] get; }",
            AttributeWithNamespace = false,
            AttributeAccessorParameterFormat = AttributeSectionFormat.Discrete
          )]
          [MemberDeclarationTestCase(
            "public int P0 { [return: ReturnParameter(0, NamedArg = 1), XmlIgnore] get; }",
            AttributeWithNamespace = false,
            AttributeAccessorParameterFormat = AttributeSectionFormat.List
          )]
          [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
          [AttributeTestCase("[System.Obsolete]")]
          public int P0 {
            [return: AttributeTestCase(
              "[return: ReturnParameter(0, NamedArg = 1)], [return: XmlIgnore]",
              AttributeWithNamespace = false,
              AttributeAccessorParameterFormat = AttributeSectionFormat.Discrete
            )]
            [return: AttributeTestCase(
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
            MethodBody = MethodBodyOption.None
          )]
          [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
          public bool M(
            [AttributeTestCase(
              "[System.Runtime.CompilerServices.CallerFilePath], [System.Runtime.InteropServices.Optional]",
              AttributeWithNamespace = true
            )]
            [AttributeTestCase(
              "[CallerFilePath], [Optional]",
              AttributeWithNamespace = false
            )]
            [CallerFilePath]
            string sourceFilePath = default,

            [AttributeTestCase(
              "[System.Runtime.CompilerServices.CallerLineNumber], [System.Runtime.InteropServices.Optional]",
              AttributeWithNamespace = true
            )]
            [AttributeTestCase(
              "[CallerLineNumber], [Optional]",
              AttributeWithNamespace = false
            )]
            [CallerLineNumber]
            int sourceLineNumber = default
          ) => throw new NotImplementedException();

          [TypeDeclarationTestCase(
            "public delegate bool D([Parameter(0, NamedArg = 1)] [Optional] int arg = 0);",
            AttributeWithNamespace = false
          )]
          [Obsolete] // must not be shown in the result of TypeDeclarationTestCase
          [AttributeTestCase("[System.Obsolete]")]
          public delegate bool D(
            [AttributeTestCase(
              "[Parameter(0, NamedArg = 1)], [Optional]",
              AttributeWithNamespace = false
            )]
            [Parameter(0, NamedArg = 1)]
            int arg = 0
          );

          [MemberDeclarationTestCase(
            "public int P0 { [param: Parameter(0, NamedArg = 1)] [param: XmlIgnore] set; }",
            AttributeWithNamespace = false,
            AttributeAccessorParameterFormat = AttributeSectionFormat.Discrete
          )]
          [MemberDeclarationTestCase(
            "public int P0 { [param: Parameter(0, NamedArg = 1), XmlIgnore] set; }",
            AttributeWithNamespace = false,
            AttributeAccessorParameterFormat = AttributeSectionFormat.List
          )]
          [Obsolete] // must not be shown in the result of MemberDeclarationTestCase
          [AttributeTestCase("[System.Obsolete]")]
          public int P0 {
            [param: AttributeTestCase(
              "[param: Parameter(0, NamedArg = 1)], [param: XmlIgnore]",
              AttributeWithNamespace = false,
              AttributeAccessorParameterFormat = AttributeSectionFormat.Discrete
            )]
            [param: AttributeTestCase(
              "[param: Parameter(0, NamedArg = 1), XmlIgnore]",
              AttributeWithNamespace = false,
              AttributeAccessorParameterFormat = AttributeSectionFormat.List
            )]
            [param: System.Xml.Serialization.XmlIgnore]
            [param: Parameter(0, NamedArg = 1)]
            set => throw null;
          }
        }
#pragma warning restore CS0414
      }

      namespace AttributeTypes {
        [AttributeTestCase("[System.Flags]")]
        [Flags]
        enum Flags1 : int { }

        [AttributeTestCase("[System.Flags]", AttributeWithNamespace = true)]
        [AttributeTestCase("[Flags]", AttributeWithNamespace = false)]
        [Flags]
        enum Flags2 : int { }

        [AttributeTestCase("[System.Flags], [System.Obsolete]")]
        [Flags]
        [Obsolete]
        enum Flags3 : int { }

        [AttributeTestCase("[System.Flags], [System.Obsolete]")]
        [Obsolete]
        [Flags]
        enum Flags4 : int { }

        [AttributeTestCase("[System.Obsolete]")]
        [Obsolete]
        class Obsolete1 { }

        [AttributeTestCase("[System.Obsolete(\"obsolete\")]")]
        [Obsolete("obsolete")]
        class Obsolete2 { }

        [AttributeTestCase("[System.Obsolete(\"deprecated\", true)]", AttributeWithNamedArguments = false)]
        [AttributeTestCase("[System.Obsolete(message: \"deprecated\", error: true)]", AttributeWithNamedArguments = true)]
        [Obsolete("deprecated", true)]
        class Obsolete3 { }

        [AttributeTestCase("[System.Obsolete(\"deprecated\", false)]", AttributeWithNamedArguments = false)]
        [AttributeTestCase("[System.Obsolete(message: \"deprecated\", error: false)]", AttributeWithNamedArguments = true)]
        [Obsolete("deprecated", false)]
        class Obsolete4 { }

        [AttributeTestCase("[System.Serializable]")]
        [Serializable]
        class Serializable1 { }

        class Conditionals {
          [AttributeTestCase("[System.Diagnostics.Conditional(\"DEBUG\")]")]
          [AttributeTestCase("[Conditional(\"DEBUG\")]", AttributeWithNamespace = false)]
          [System.Diagnostics.Conditional("DEBUG")]
          public void M() { }
        }

        [AttributeTestCase("[System.Runtime.CompilerServices.Extension]")]
        static class Extension {
          [AttributeTestCase("[System.Runtime.CompilerServices.Extension]")]
          public static void M1(this int x) { }
        }

        [AttributeTestCase("[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 1)]", AttributeWithNamespace = true, TypeWithNamespace = true)]
        [AttributeTestCase("[System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack = 1)]", AttributeWithNamespace = true, TypeWithNamespace = false)]
        [AttributeTestCase("[StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 1)]", AttributeWithNamespace = false, TypeWithNamespace = true)]
        [AttributeTestCase("[StructLayout(LayoutKind.Explicit, Pack = 1)]", AttributeWithNamespace = false, TypeWithNamespace = false)]
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 1)]
        struct StructLayout1 {
          [AttributeTestCase("[System.Runtime.InteropServices.FieldOffset(0)]")]
          [AttributeTestCase("[FieldOffset(0)]", AttributeWithNamespace = false)]
          [System.Runtime.InteropServices.FieldOffset(0)]
          public byte F0;
        }

        [AttributeTestCase("[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 1)]", AttributeWithNamespace = true, TypeWithNamespace = true)]
        [AttributeTestCase("[System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Size = 1)]", AttributeWithNamespace = true, TypeWithNamespace = false)]
        [AttributeTestCase("[StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 1)]", AttributeWithNamespace = false, TypeWithNamespace = true)]
        [AttributeTestCase("[StructLayout(LayoutKind.Explicit, Size = 1)]", AttributeWithNamespace = false, TypeWithNamespace = false)]
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 1)]
        struct StructLayout2 {
          [AttributeTestCase("[System.Runtime.InteropServices.FieldOffset(0)]")]
          [AttributeTestCase("[FieldOffset(0)]", AttributeWithNamespace = false)]
          [System.Runtime.InteropServices.FieldOffset(0)] public byte F0;
        }

        [AttributeTestCase("")]
        struct NoStructLayout { }
      }

      namespace AttributeArguments {
        public class ObjectValueAttribute : Attribute {
          public object Value { get; }
          public ObjectValueAttribute(object value)
          {
            this.Value = value;
          }
        }

        class C {
          [AttributeTestCase("[ObjectValue(null)]", AttributeWithNamespace = false)]
          [ObjectValue(null)]
          public int NullValue = 0;

          [AttributeTestCase("[ObjectValue(\"str\")]", AttributeWithNamespace = false)]
          [ObjectValue("str")]
          public int StringValue = 0;

          [AttributeTestCase("[ObjectValue(0)]", AttributeWithNamespace = false)]
          [ObjectValue((byte)0)]
          public int ByteValue = 0;

          [AttributeTestCase("[ObjectValue(0)]", AttributeWithNamespace = false)]
          [ObjectValue((int)0)]
          public int IntValue = 0;

          [AttributeTestCase("[ObjectValue(0)]", AttributeWithNamespace = false)]
          [ObjectValue((double)0.0)]
          public int DoubleValue = 0;

          [AttributeTestCase("[ObjectValue(DayOfWeek.Sunday)]", AttributeWithNamespace = false, TypeWithNamespace = false)]
          [AttributeTestCase("[ObjectValue(System.DayOfWeek.Sunday)]", AttributeWithNamespace = false, TypeWithNamespace = true)]
          [ObjectValue(DayOfWeek.Sunday)]
          public int EnumValue = 0;

          [AttributeTestCase("[ObjectValue((DayOfWeek)999)]", AttributeWithNamespace = false, TypeWithNamespace = false)]
          [AttributeTestCase("[ObjectValue((System.DayOfWeek)999)]", AttributeWithNamespace = false, TypeWithNamespace = true)]
          [ObjectValue((DayOfWeek)999)]
          public int EnumValueUndefined = 0;
        }
      }
    }
  }

  partial class GeneratorTests {
    private static bool ExceptTestCaseAttributeFilter(Type type, ICustomAttributeProvider _)
      => !typeof(ITestCaseAttribute).IsAssignableFrom(type);

    private static System.Collections.IEnumerable YieldAttributeListTestCase()
      => FindTypes(t => t.FullName!.Contains(".TestCases.Attributes."))
        .SelectMany(t => t
          .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
          .Prepend((MemberInfo)t) // prepend type itself as a test target
          .Concat(t.GetGenericArguments())
          .Concat(
            t
              .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
              .SelectMany(static method => method.GetGenericArguments())
          )
        )
        .SelectMany(
          m => m.GetCustomAttributes<AttributeTestCaseAttribute>().Select(
            a => new object[] { a, m }
          )
        );

    [TestCaseSource(nameof(YieldAttributeListTestCase))]
    public void TestGenerateAttributeList(
      AttributeTestCaseAttribute attrTestCase,
      MemberInfo typeOrMember
    )
    {
      var options = GetGeneratorOptions(attrTestCase);

      options.AttributeDeclaration.TypeFilter = ExceptTestCaseAttributeFilter;

      var typeOrMemberName = typeOrMember is Type t
        ? t.FullName
        : $"{typeOrMember.DeclaringType?.FullName}.{typeOrMember.Name}";

      Assert.AreEqual(
        attrTestCase.Expected,
        string.Join(", ", Generator.GenerateAttributeList(typeOrMember, null, options)),
        message: $"{attrTestCase.SourceLocation} ({typeOrMemberName})"
      );
    }

    private static System.Collections.IEnumerable YieldAttributeListOfParameterInfoTestCase()
      => FindTypes(static t => t.FullName!.Contains(".TestCases.Attributes."))
        .SelectMany(static t =>
          t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
        )
        .Where(static method => method.DeclaringType is not null && !method.DeclaringType.IsDelegate())
        .SelectMany(static method =>
          method.GetParameters().Prepend(method.ReturnParameter)
        )
        .SelectMany(
          static p => p.GetCustomAttributes<AttributeTestCaseAttribute>().Select(
            a => new object[] { a, p }
          )
        );

    [TestCaseSource(nameof(YieldAttributeListOfParameterInfoTestCase))]
    public void TestGenerateAttributeListOfParameterInfo(
      AttributeTestCaseAttribute attrTestCase,
      ParameterInfo param
    )
    {
      var options = GetGeneratorOptions(attrTestCase);

      options.AttributeDeclaration.TypeFilter = ExceptTestCaseAttributeFilter;

      Assert.AreEqual(
        attrTestCase.Expected,
        string.Join(", ", Generator.GenerateAttributeList(param, null, options)),
        message: $"{attrTestCase.SourceLocation} ({param.Member.DeclaringType!.FullName}.{param.Member.Name} {(param.Name ?? "return value")})"
      );
    }

    private static System.Collections.IEnumerable YieldTypeWithAttributeDeclarationTestCase()
      => FindTypes(static t => t.FullName!.Contains(".TestCases.Attributes."))
        .SelectMany(
          static t => t.GetCustomAttributes<TypeDeclarationTestCaseAttribute>().Select(
            a => new object[] { a, t }
          )
        );

    [TestCaseSource(nameof(YieldTypeWithAttributeDeclarationTestCase))]
    public void TestGenerateTypeWithAttributeDeclaration(
      TypeDeclarationTestCaseAttribute attrTestCase,
      Type type
    )
    {
      type.GetCustomAttribute<SkipTestCaseAttribute>()?.Throw();

      var options = GetGeneratorOptions(attrTestCase);

      options.AttributeDeclaration.TypeFilter = ExceptTestCaseAttributeFilter;

      Assert.AreEqual(
        attrTestCase.Expected,
        Generator.GenerateTypeDeclaration(type, null, options),
        message: $"{attrTestCase.SourceLocation} ({type.FullName})"
      );
    }

    private static System.Collections.IEnumerable YieldMemberWithAttributeDeclarationTestCase()
      => FindTypes(static t => t.FullName!.Contains(".TestCases.Attributes."))
        .SelectMany(static t => t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
        .Where(static m => m is not Type) // except nested type
        .SelectMany(
          static m => m.GetCustomAttributes<MemberDeclarationTestCaseAttribute>().Select(
            a => new object[] { a, m }
          )
        );

    [TestCaseSource(nameof(YieldMemberWithAttributeDeclarationTestCase))]
    public void TestGenerateMemberWithAttributeDeclaration(
      MemberDeclarationTestCaseAttribute attrTestCase,
      MemberInfo member
    )
    {
      member.GetCustomAttribute<SkipTestCaseAttribute>()?.Throw();

      var options = GetGeneratorOptions(attrTestCase);

      options.AttributeDeclaration.TypeFilter = ExceptTestCaseAttributeFilter;

      Assert.AreEqual(
        attrTestCase.Expected,
        Generator.GenerateMemberDeclaration(member, null, options),
        message: $"{attrTestCase.SourceLocation} ({member.DeclaringType!.FullName}.{member.Name})"
      );
    }
  }
}
