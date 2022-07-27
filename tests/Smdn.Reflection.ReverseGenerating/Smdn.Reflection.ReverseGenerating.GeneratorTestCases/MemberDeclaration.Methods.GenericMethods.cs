// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.GenericMethods;

public class C<T> {
  [MemberDeclarationTestCase("public T M(T x) {}")] public T M(T x) => throw new NotImplementedException();
}

public class C<T1, T2> {
  [MemberDeclarationTestCase("public T1 M(T2 x) {}")] public T1 M(T2 x) => throw new NotImplementedException();
}

public class C {
  [MemberDeclarationTestCase("public T M<T>(T x) {}")]
  [MemberDeclarationTestCase("public T C.M<T>(T x)", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("public T Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.GenericMethods.C.M<T>(T x)", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
  public T M<T>(T x) => throw new NotImplementedException();

  [MemberDeclarationTestCase("public T1 M<T1, T2>(T2 x) {}")]
  [MemberDeclarationTestCase("public T1 C.M<T1, T2>(T2 x)", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("public T1 Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.GenericMethods.C.M<T1, T2>(T2 x)", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
  public T1 M<T1, T2>(T2 x) => throw new NotImplementedException();
}

public abstract class COpen<T1, T2> {
  [MemberDeclarationTestCase("public abstract void M(T1 t1, T2 t2);")]
  public abstract void M(T1 t1, T2 t2);
}

public class CClose1<T2> : COpen<int, T2> {
  [MemberDeclarationTestCase("public override void M(int t1, T2 t2) {}")]
  [MemberDeclarationTestCase("public override void CClose1<T2>.M(int t1, T2 t2)", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("public override void Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.GenericMethods.CClose1<T2>.M(int t1, T2 t2)", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
  public override void M(int t1, T2 t2) { }
}

public class CClose2 : CClose1<int> {
  [MemberDeclarationTestCase("public override void M(int t1, int t2) {}")]
  [MemberDeclarationTestCase("public override void CClose2.M(int t1, int t2)", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("public override void Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.GenericMethods.CClose2.M(int t1, int t2)", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
  public override void M(int t1, int t2) { }
}

public class Constraints {
#nullable enable annotations
  [MemberDeclarationTestCase("public T M0_NullableEnableContext<T>(T x) {}")]
  public T M0_NullableEnableContext<T>(T x) => throw new NotImplementedException();
#nullable disable annotations
  [MemberDeclarationTestCase("public T M0_NullableDisableContext<T>(T x) {}")]
  public T M0_NullableDisableContext<T>(T x) => throw new NotImplementedException();
#nullable restore

  [MemberDeclarationTestCase("public T M1<T>(T x) where T : new() {}")] public T M1<T>(T x) where T : new() => throw new NotImplementedException();
  [MemberDeclarationTestCase("public T M2_0<T>(T x) where T : struct {}")] public T M2_0<T>(T x) where T : struct => throw new NotImplementedException();
  [MemberDeclarationTestCase("public T M2_1<T>(T x) where T : struct, System.IDisposable {}")] public T M2_1<T>(T x) where T : struct, IDisposable => throw new NotImplementedException();
  [MemberDeclarationTestCase("public T M3_0<T>(T x) where T : class {}")] public T M3_0<T>(T x) where T : class => throw new NotImplementedException();
  [MemberDeclarationTestCase("public T M3_1<T>(T x) where T : class, System.IDisposable {}")] public T M3_1<T>(T x) where T : class, IDisposable => throw new NotImplementedException();
  [MemberDeclarationTestCase("public T M4<T>(T x) where T : System.IDisposable {}")] public T M4<T>(T x) where T : IDisposable => throw new NotImplementedException();
  [MemberDeclarationTestCase("public T M5<T>(T x) where T : System.ICloneable, System.IDisposable {}")] public T M5<T>(T x) where T : IDisposable, ICloneable => throw new NotImplementedException();
  [MemberDeclarationTestCase("public T M6<T>(T x) where T : System.ICloneable, new() {}")] public T M6<T>(T x) where T : ICloneable, new() => throw new NotImplementedException();
  [MemberDeclarationTestCase("public T M7<T>(T x) where T : class, new() {}")] public T M7<T>(T x) where T : class, new() => throw new NotImplementedException();
  [MemberDeclarationTestCase("public T M8<T>(T x) where T : class, System.ICloneable, new() {}")] public T M8<T>(T x) where T : class, ICloneable, new() => throw new NotImplementedException();
  [MemberDeclarationTestCase("public T M9<T>(T x) where T : class, System.ICloneable, System.IDisposable, new() {}")] public T M9<T>(T x) where T : class, ICloneable, IDisposable, new() => throw new NotImplementedException();

  [MemberDeclarationTestCase("public T M10<T>(T x) where T : Constraints.CBase {}", MemberWithNamespace = false)]
  public T M10<T>(T x) where T : CBase => throw new NotImplementedException();
  public class CBase { }

  [MemberDeclarationTestCase("public T M11<T>(T x) where T : System.Enum {}")] public T M11<T>(T x) where T : System.Enum => throw new NotImplementedException();
  [MemberDeclarationTestCase("public T M12<T>(T x) where T : System.Delegate {}")] public T M12<T>(T x) where T : System.Delegate => throw new NotImplementedException();
  [MemberDeclarationTestCase("public T M13<T>(T x) where T : System.MulticastDelegate {}")] public T M13<T>(T x) where T : System.MulticastDelegate => throw new NotImplementedException();

  [MemberDeclarationTestCase("public T M15_0<T>(T x) where T : unmanaged {}")] public T M15_0<T>(T x) where T : unmanaged => throw new NotImplementedException();
  [MemberDeclarationTestCase("public T M15_1<T>(T x) where T : unmanaged, System.IDisposable {}")] public T M15_1<T>(T x) where T : unmanaged, IDisposable => throw new NotImplementedException();
}

public class ConstraintsNotNull {
#nullable enable annotations
  [MemberDeclarationTestCase($"public void {nameof(NotNull_NullableEnableContext)}<TNotNull, T>(TNotNull pNotNull, T p) where TNotNull : notnull {{}}")]
  public void NotNull_NullableEnableContext<TNotNull, T>(TNotNull pNotNull, T p) where TNotNull : notnull => throw new NotImplementedException();
#nullable disable annotations
  [MemberDeclarationTestCase($"public void {nameof(NotNull_NullableDisableContext)}<TNotNull, T>(TNotNull pNotNull, T p) where TNotNull : notnull {{}}")]
  public void NotNull_NullableDisableContext<TNotNull, T>(TNotNull pNotNull, T p) where TNotNull : notnull => throw new NotImplementedException();
#nullable restore

#nullable enable annotations
  [MemberDeclarationTestCase($"public TNotNull {nameof(NotNull_ReturnParameter_NullableEnableContext)}<TNotNull, T>(TNotNull pNotNull, T p) where TNotNull : notnull {{}}")]
  public TNotNull NotNull_ReturnParameter_NullableEnableContext<TNotNull, T>(TNotNull pNotNull, T p) where TNotNull : notnull => throw new NotImplementedException();
#nullable disable annotations
  [MemberDeclarationTestCase($"public TNotNull {nameof(NotNull_ReturnParameter_NullableDisableContext)}<TNotNull, T>(TNotNull pNotNull, T p) where TNotNull : notnull {{}}")]
  public TNotNull NotNull_ReturnParameter_NullableDisableContext<TNotNull, T>(TNotNull pNotNull, T p) where TNotNull : notnull => throw new NotImplementedException();
#nullable restore

#nullable enable annotations
  [MemberDeclarationTestCase($"public T {nameof(NotNull_OnlyParameter_NullableEnableContext)}<TNotNull, T>(TNotNull pNotNull, T p) where TNotNull : notnull {{}}")]
  public T NotNull_OnlyParameter_NullableEnableContext<TNotNull, T>(TNotNull pNotNull, T p) where TNotNull : notnull => throw new NotImplementedException();
#nullable disable annotations
  [MemberDeclarationTestCase($"public T {nameof(NotNull_OnlyParameter_NullableDisableContext)}<TNotNull, T>(TNotNull pNotNull, T p) where TNotNull : notnull {{}}")]
  public T NotNull_OnlyParameter_NullableDisableContext<TNotNull, T>(TNotNull pNotNull, T p) where TNotNull : notnull => throw new NotImplementedException();
#nullable restore

#nullable enable annotations
  [MemberDeclarationTestCase($"public TNotNullDisposable {nameof(NotNullAndTypeConstraint_NullableEnableContext)}<TNotNullDisposable>(TNotNullDisposable p) where TNotNullDisposable : notnull, System.IDisposable {{}}")]
  public TNotNullDisposable NotNullAndTypeConstraint_NullableEnableContext<TNotNullDisposable>(TNotNullDisposable p) where TNotNullDisposable : notnull, IDisposable => throw new NotImplementedException();
#nullable disable annotations
  [MemberDeclarationTestCase($"public TNotNullDisposable {nameof(NotNullAndTypeConstraint_NullableDisableContext)}<TNotNullDisposable>(TNotNullDisposable p) where TNotNullDisposable : notnull, System.IDisposable {{}}")]
  public TNotNullDisposable NotNullAndTypeConstraint_NullableDisableContext<TNotNullDisposable>(TNotNullDisposable p) where TNotNullDisposable : notnull, IDisposable => throw new NotImplementedException();
#nullable restore
}

public class ConstraintsNotNullWithNullableMetadata {
#nullable enable annotations
  class ClassWithOptimizedNullableMetadata {
    // these members are required to reproduce optimized nullable metadata
    public void RequiredToBeOptimized0(string p) { }
    public void RequiredToBeOptimized1(string? p) { }

    // this class will have no NullableContextAttribute after optimization
    class NotNullConstraints {
      [MemberDeclarationTestCase($"public void {nameof(NotNullConstraint)}<TNotNull>(TNotNull p) where TNotNull : notnull {{}}")]
      public void NotNullConstraint<TNotNull>(TNotNull p) where TNotNull : notnull { }

      // this member is required to reproduce optimized nullable metadata
#if SYSTEM_REFLECTION_NULLABILITYINFO
      [MemberDeclarationTestCase($"public void {nameof(NotNullConstraint_Nullable)}<TNotNull>(TNotNull? p) where TNotNull : notnull {{}}")]
#else
      [MemberDeclarationTestCase($"public void {nameof(NotNullConstraint_Nullable)}<TNotNull>(TNotNull p) where TNotNull : notnull {{}}")]
#endif
      public void NotNullConstraint_Nullable<TNotNull>(TNotNull? p) where TNotNull : notnull { }
    }
  }
#nullable restore annotations
}

class ConstraintsOfMultipleGenericParameters {
  [MemberDeclarationTestCase("public T1 M1<T1, T2>(T2 x) where T1 : new() where T2 : new() {}")]
  public T1 M1<T1, T2>(T2 x)
    where T1 : new()
    where T2 : new()
    => throw new NotImplementedException();

  [MemberDeclarationTestCase("public T1 M2<T1, T2>(T2 x) where T1 : new() where T2 : new() {}")]
  public T1 M2<T1, T2>(T2 x)
    where T2 : new()
    where T1 : new()
    => throw new NotImplementedException();

  [MemberDeclarationTestCase("public T1 M3<T1, T2>(T2 x) where T1 : System.ICloneable where T2 : System.IDisposable {}")]
  public T1 M3<T1, T2>(T2 x)
    where T1 : ICloneable
    where T2 : IDisposable
    => throw new NotImplementedException();

  [MemberDeclarationTestCase("public T1 M4<T1, T2>(T2 x) where T1 : class, System.ICloneable where T2 : System.IDisposable, new() {}")]
  public T1 M4<T1, T2>(T2 x)
    where T1 : class, ICloneable
    where T2 : IDisposable, new()
    => throw new NotImplementedException();
}
