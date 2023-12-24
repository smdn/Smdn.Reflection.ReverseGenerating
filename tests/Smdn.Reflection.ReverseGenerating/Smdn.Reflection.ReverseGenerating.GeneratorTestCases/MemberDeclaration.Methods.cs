// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods {
  namespace GeneratorOptions {
    public abstract class Abstract {
      [MemberDeclarationTestCase("public abstract void M()", MethodBody = MethodBodyOption.None)]
      [MemberDeclarationTestCase("public abstract void M();", MethodBody = MethodBodyOption.EmptyImplementation)]
      [MemberDeclarationTestCase("public abstract void M()", MethodBody = MethodBodyOption.EmptyImplementation, MemberOmitEndOfStatement = true)]
      [MemberDeclarationTestCase("public abstract void M();", MethodBody = MethodBodyOption.ThrowNotImplementedException)]
      [MemberDeclarationTestCase("public abstract void M()", MethodBody = MethodBodyOption.ThrowNotImplementedException, MemberOmitEndOfStatement = true)]
      [MemberDeclarationTestCase("public abstract void M();", MethodBody = MethodBodyOption.ThrowNull)]
      [MemberDeclarationTestCase("public abstract void M()", MethodBody = MethodBodyOption.ThrowNull, MemberOmitEndOfStatement = true)]
      [MemberDeclarationTestCase("public abstract void Abstract.M()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
      [MemberDeclarationTestCase("public abstract void Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.GeneratorOptions.Abstract.M()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
      public abstract void M();
    }

    public abstract class NonAbstract {
      [MemberDeclarationTestCase("public void M()", MethodBody = MethodBodyOption.None)]
      [MemberDeclarationTestCase("public void M() {}", MethodBody = MethodBodyOption.EmptyImplementation)]
      [MemberDeclarationTestCase("public void M() {}", MethodBody = MethodBodyOption.EmptyImplementation, MemberOmitEndOfStatement = true)]
      [MemberDeclarationTestCase("public void M() => throw new NotImplementedException();", MethodBody = MethodBodyOption.ThrowNotImplementedException)]
      [MemberDeclarationTestCase("public void M() => throw new NotImplementedException()", MethodBody = MethodBodyOption.ThrowNotImplementedException, MemberOmitEndOfStatement = true)]
      [MemberDeclarationTestCase("public void M() => throw null;", MethodBody = MethodBodyOption.ThrowNull)]
      [MemberDeclarationTestCase("public void M() => throw null", MethodBody = MethodBodyOption.ThrowNull, MemberOmitEndOfStatement = true)]
      [MemberDeclarationTestCase("public void NonAbstract.M()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
      [MemberDeclarationTestCase("public void Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.GeneratorOptions.NonAbstract.M()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
      public void M() => throw new NotImplementedException();
    }
  }

  namespace Modifiers {
    public class Static {
      [MemberDeclarationTestCase("public static void M1() {}")] public static void M1() { }
    }

    public abstract class Abstract {
      [MemberDeclarationTestCase("public abstract void M();")] public abstract void M();
    }

    public class Virtual {
      [MemberDeclarationTestCase("public virtual void M() {}")] public virtual void M() { }

      [MemberDeclarationTestCase("protected virtual void MProtectedVirtual() {}")] protected virtual void MProtectedVirtual() { }
    }

    public class Override : Virtual {
      [MemberDeclarationTestCase("public override void M() {}")] public override void M() { }

      [MemberDeclarationTestCase("protected override void MProtectedVirtual() {}")] protected override void MProtectedVirtual() { }
    }

    public class SealedOverride1 : Virtual {
      [MemberDeclarationTestCase("public sealed override void M() {}")] public sealed override void M() { }

      [MemberDeclarationTestCase("protected sealed override void MProtectedVirtual() {}")] protected sealed override void MProtectedVirtual() { }

    }

    public class SealedOverride2 : Virtual {
      [MemberDeclarationTestCase("public sealed override void M() {}")] public override sealed void M() { }
    }

    public abstract class New : Virtual {
      [MemberDeclarationTestCase("new public void M() {}")] public new void M() { }

      [MemberDeclarationTestCase("new protected void MProtectedVirtual() {}")] protected new void MProtectedVirtual() { }
    }

    public abstract class NewVirtual : Virtual {
      [MemberDeclarationTestCase("new public virtual void M() {}")] public new virtual void M() { }

      [MemberDeclarationTestCase("new protected virtual void MProtectedVirtual() {}")] protected new virtual void MProtectedVirtual() { }
    }

    public class Unsafe {
      [MemberDeclarationTestCase("public unsafe void M(int* x) {}")] public unsafe void M(int* x) { }
    }

    public class Async {
      [MemberDeclarationTestCase("public async void M0() {}")]
      public async void M0() { await Task.Delay(0); }

      [MemberDeclarationTestCase("public async System.Threading.Tasks.Task MTask1() {}")]
      public async Task MTask1() { await Task.Delay(0); }

      [MemberDeclarationTestCase("public async System.Threading.Tasks.Task<int> MTask2() {}")]
      public async Task<int> MTask2() { await Task.Delay(0); return 0; }

      [MemberDeclarationTestCase("public System.Threading.Tasks.Task MTask3() {}")]
      public Task MTask3() => Task.Delay(0);

#if SYSTEM_THREADING_TASKS_VALUETASK
      [MemberDeclarationTestCase("public async System.Threading.Tasks.ValueTask MValueTask1() {}")]
      public async ValueTask MValueTask1() { await Task.Delay(0); }

      [MemberDeclarationTestCase("public async System.Threading.Tasks.ValueTask<int> MValueTask2() {}")]
      public async ValueTask<int> MValueTask2() { await Task.Delay(0); return 0; }

      [MemberDeclarationTestCase("public System.Threading.Tasks.ValueTask MValueTask3() {}")]
      public ValueTask MValueTask3() => new ValueTask();
#endif
    }

    public struct ReadOnly {
      [MemberDeclarationTestCase("public readonly int MReadOnly() {}")]
      public readonly int MReadOnly() => throw new NotImplementedException();

      [MemberDeclarationTestCase("public readonly unsafe int MReadOnlyUnsafe(int* x) {}")]
      public readonly unsafe int MReadOnlyUnsafe(int* x) => throw new NotImplementedException();

      [MemberDeclarationTestCase("public readonly async System.Threading.Tasks.Task<int> MAsyncReadOnly() {}")]
      public async readonly Task<int> MAsyncReadOnly() { await Task.Delay(0); return 0; }
    }
  }

  public class Accessibilities {
    [MemberDeclarationTestCase("public void M1() {}")] public void M1() { }
    [MemberDeclarationTestCase("internal void M2() {}")] internal void M2() { }
    [MemberDeclarationTestCase("protected void M3() {}")] protected void M3() { }
    [MemberDeclarationTestCase("internal protected void M4() {}")] protected internal void M4() { }
    [MemberDeclarationTestCase("internal protected void M5() {}")] internal protected void M5() { }
    [MemberDeclarationTestCase("private protected void M6() {}")] private protected void M6() { }
    [MemberDeclarationTestCase("private protected void M7() {}")] protected private void M7() { }
    [MemberDeclarationTestCase("private void M8() {}")] private void M8() { }

    [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] internal void M9() { }
    [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private protected void M10() { }
    [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private void M11() { }
  }

  public class ParameterNames {
    [MemberDeclarationTestCase("public void M1(int @value) {}")] public void M1(int @value) { }
    [MemberDeclarationTestCase("public void M2(int @readonly) {}")] public void M2(int @readonly) { }
    [MemberDeclarationTestCase("public void M3(int @where) {}")] public void M3(int @where) { }
    [MemberDeclarationTestCase("public void M4(int @var) {}")] public void M4(int @var) { }
  }

  public static class ExtensionMethods {
    [MemberDeclarationTestCase("public static void M(this int x) {}")] public static void M(this int x) { }
    [MemberDeclarationTestCase("public static void M(this int x, int y) {}")] public static void M(this int x, int y) { }
  }

  public static class ReferenceReturnValues {
    [MemberDeclarationTestCase($"public static ref int {nameof(MRefInt)}() {{}}")] public static ref int MRefInt() => throw new NotImplementedException();
    [MemberDeclarationTestCase($"public static ref int? {nameof(MRefNullableInt)}() {{}}")] public static ref int? MRefNullableInt() => throw new NotImplementedException();
    [MemberDeclarationTestCase($"public static ref string {nameof(MRefString)}() {{}}")] public static ref string MRefString() => throw new NotImplementedException();
    [MemberDeclarationTestCase($"public static ref System.Collections.Generic.KeyValuePair<int, int> {nameof(MRefKeyValuePairOfIntInt)}() {{}}")] public static ref KeyValuePair<int, int> MRefKeyValuePairOfIntInt() => throw new NotImplementedException();
  }

  public static class ValueTupleReturnValues {
    [MemberDeclarationTestCase("public static (int, int) M1() {}")] public static (int, int) M1() => throw new NotImplementedException();
    [MemberDeclarationTestCase("public static (int x, int y) M2() {}")] public static (int x, int y) M2() => throw new NotImplementedException();
    [MemberDeclarationTestCase("public static (int, int, int) M3() {}")] public static (int, int, int) M3() => throw new NotImplementedException();
    [MemberDeclarationTestCase("public static (string x, string y) M4() {}")] public static (string x, string y) M4() => throw new NotImplementedException();
    [MemberDeclarationTestCase("public static System.ValueTuple<string> M5() {}")] public static ValueTuple<string> M5() => throw new NotImplementedException();
    [MemberDeclarationTestCase("public static (string, string) M6() {}")] public static ValueTuple<string, string> M6() => throw new NotImplementedException();
  }

  namespace Constructors {
    public class C {
      [MemberDeclarationTestCase("public C() {}")]
      [MemberDeclarationTestCase("public C.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
      [MemberDeclarationTestCase("public Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.Constructors.C.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
      public C() => throw new NotImplementedException();

      [MemberDeclarationTestCase("static C() {}")]
      [MemberDeclarationTestCase("static C.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
      [MemberDeclarationTestCase("static Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.Constructors.C.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
      static C() { }
    }

    public class C<T> {
      [MemberDeclarationTestCase("public C() {}")]
      [MemberDeclarationTestCase("public C<T>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
      [MemberDeclarationTestCase("public Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.Constructors.C<T>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
      public C() => throw new NotImplementedException();

      [MemberDeclarationTestCase("static C() {}")]
      [MemberDeclarationTestCase("static C<T>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
      [MemberDeclarationTestCase("static Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.Constructors.C<T>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
      static C() { }
    }

    public class C<T1, T2> {
      [MemberDeclarationTestCase("public C() {}")]
      [MemberDeclarationTestCase("public C<T1, T2>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
      [MemberDeclarationTestCase("public Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.Constructors.C<T1, T2>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
      public C() => throw new NotImplementedException();

      [MemberDeclarationTestCase("static C() {}")]
      [MemberDeclarationTestCase("static C<T1, T2>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
      [MemberDeclarationTestCase("static Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.Constructors.C<T1, T2>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
      static C() { }
    }

    public class Accessibilities {
      [MemberDeclarationTestCase("public Accessibilities(int p) {}")] public Accessibilities(int p) { throw new NotImplementedException(); }
      [MemberDeclarationTestCase("internal Accessibilities(short p) {}")] internal Accessibilities(short p) { throw new NotImplementedException(); }
      [MemberDeclarationTestCase("protected Accessibilities(byte p) {}")] protected Accessibilities(byte p) { throw new NotImplementedException(); }
      [MemberDeclarationTestCase("internal protected Accessibilities(uint p) {}")] protected internal Accessibilities(uint p) { throw new NotImplementedException(); }
      [MemberDeclarationTestCase("internal protected Accessibilities(ulong p) {}")] internal protected Accessibilities(ulong p) { throw new NotImplementedException(); }
      [MemberDeclarationTestCase("private protected Accessibilities(ushort p) {}")] private protected Accessibilities(ushort p) { throw new NotImplementedException(); }
      [MemberDeclarationTestCase("private protected Accessibilities(sbyte p) {}")] protected private Accessibilities(sbyte p) { throw new NotImplementedException(); }
      [MemberDeclarationTestCase("private Accessibilities(bool p) {}")] private Accessibilities(bool p) { throw new NotImplementedException(); }

      [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] internal Accessibilities(string p) => throw new NotImplementedException();
      [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private protected Accessibilities(object p) => throw new NotImplementedException();
      [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private Accessibilities(long p) => throw new NotImplementedException();
    }
  }

  namespace Destructors {
    public class C {
      [MemberDeclarationTestCase("~C() {}")]
      [MemberDeclarationTestCase("C.~C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
      [MemberDeclarationTestCase("Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.Destructors.C.~C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
      ~C() => throw new NotImplementedException();
    }

    public class C<T> {
      [MemberDeclarationTestCase("~C() {}")]
      [MemberDeclarationTestCase("C<T>.~C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
      [MemberDeclarationTestCase("Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.Destructors.C<T>.~C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
      ~C() => throw new NotImplementedException();
    }

    public class C<T1, T2> {
      [MemberDeclarationTestCase("~C() {}")]
      [MemberDeclarationTestCase("C<T1, T2>.~C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
      [MemberDeclarationTestCase("Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.Destructors.C<T1, T2>.~C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
      ~C() => throw new NotImplementedException();
    }
  }

  namespace Operators {
    namespace UnaryOperators {
      public class C {
        [MemberDeclarationTestCase("public static C operator + (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator + (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator +(C c) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator - (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator - (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator -(C c) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator checked - (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator checked - (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator checked -(C c) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator ! (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator ! (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator !(C c) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator ~ (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator ~ (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator ~(C c) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static bool operator true (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static bool operator true (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static bool operator true(C c) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static bool operator false (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static bool operator false (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static bool operator false(C c) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator ++ (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator ++ (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator ++(C c) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator checked ++ (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator checked ++ (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator checked ++(C c) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator -- (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator -- (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator --(C c) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator checked -- (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator checked -- (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator checked --(C c) => throw new NotImplementedException();
      }
    }

    namespace BinaryOperators {
      public class C {
        [MemberDeclarationTestCase("public static C operator + (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator + (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator +(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator checked + (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator checked + (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator checked +(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator - (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator - (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator -(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator checked - (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator checked - (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator checked -(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator * (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator * (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator *(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator checked * (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator checked * (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator checked *(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator / (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator / (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator /(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator checked / (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator checked / (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator checked /(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator % (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator % (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator %(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator & (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator & (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator &(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator | (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator | (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator |(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator ^ (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator ^ (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator ^(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator >> (C x, int y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator >> (C x, int y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator >>(C x, int y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static C operator << (C x, int y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static C operator << (C x, int y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static C operator <<(C x, int y) => throw new NotImplementedException();
      }
    }

    namespace Comparison {
      public class C : IEquatable<C>, IComparable<C> {
        [MemberDeclarationTestCase("public static bool operator == (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static bool operator == (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static bool operator ==(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static bool operator != (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static bool operator != (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static bool operator !=(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static bool operator < (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static bool operator < (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static bool operator <(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static bool operator > (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static bool operator > (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static bool operator >(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static bool operator <= (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static bool operator <= (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static bool operator <=(C x, C y) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static bool operator >= (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static bool operator >= (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static bool operator >=(C x, C y) => throw new NotImplementedException();

        public bool Equals(C other) => throw new NotImplementedException();
        public int CompareTo(C other) => throw new NotImplementedException();
        public override bool Equals(object obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();
      }
    }

    namespace TypeCasts {
      public class V { }
      public class W { }

      public class C {
        [MemberDeclarationTestCase("public static explicit operator C(V v) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static explicit operator C(V v) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static explicit operator C(V v) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static explicit operator checked C(V v) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static explicit operator checked C(V v) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static explicit operator checked C(V v) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static explicit operator V(C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static explicit operator V(C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static explicit operator V(C c) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static explicit operator checked V(C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static explicit operator checked V(C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static explicit operator checked V(C c) => throw new NotImplementedException();


        [MemberDeclarationTestCase("public static implicit operator C(W w) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static implicit operator C(W w) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static implicit operator C(W w) => throw new NotImplementedException();

        [MemberDeclarationTestCase("public static implicit operator W(C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false, ParameterWithNamespace = false, ParameterWithDeclaringTypeName = false)]
        [MemberDeclarationTestCase("public static implicit operator W(C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true,  ParameterWithNamespace = false, ParameterWithDeclaringTypeName = true)]
        public static implicit operator W(C c) => throw new NotImplementedException();
      }
    }
  }

  public class ParametersWithAttribute {
    [MemberDeclarationTestCase(
      "public void M([CallerFilePath] [Optional] string sourceFilePath = null, [CallerLineNumber] [Optional] int sourceLineNumber = 0)",
      AttributeWithNamespace = false,
      TypeOfAttributeTypeFilterFunc = typeof(ParametersWithAttribute),
      NameOfAttributeTypeFilterFunc = nameof(_Filter),
      MethodBody = MethodBodyOption.None
    )]
    [MemberDeclarationTestCase(
      "public void M([System.Runtime.CompilerServices.CallerFilePath] [System.Runtime.InteropServices.Optional] string sourceFilePath = null, [System.Runtime.CompilerServices.CallerLineNumber] [System.Runtime.InteropServices.Optional] int sourceLineNumber = 0)",
      AttributeWithNamespace = true,
      TypeOfAttributeTypeFilterFunc = typeof(ParametersWithAttribute),
      NameOfAttributeTypeFilterFunc = nameof(_Filter),
      MethodBody = MethodBodyOption.None
    )]
    public void M(
      [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = default,
      [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = default
    ) => throw new NotImplementedException();

    private static bool _Filter(Type attr, ICustomAttributeProvider attrProvider) => true;
  }

  public class ReturnParametersWithAttribute {
    [MemberDeclarationTestCase(
      "[return: XmlIgnore] public string M()",
      AttributeWithNamespace = false,
      TypeOfAttributeTypeFilterFunc = typeof(ReturnParametersWithAttribute),
      NameOfAttributeTypeFilterFunc = nameof(_Filter),
      MethodBody = MethodBodyOption.None
    )]
    [MemberDeclarationTestCase(
      "[return: System.Xml.Serialization.XmlIgnore] public string M()",
      AttributeWithNamespace = true,
      TypeOfAttributeTypeFilterFunc = typeof(ReturnParametersWithAttribute),
      NameOfAttributeTypeFilterFunc = nameof(_Filter),
      MethodBody = MethodBodyOption.None
    )]
    [return: System.Xml.Serialization.XmlIgnore]
    public string M() => throw new NotImplementedException();

    private static bool _Filter(Type attr, ICustomAttributeProvider attrProvider) => true;
  }
}
