// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.GenericTypes {
  [TypeDeclarationTestCase("public class C1<T>")] public class C1<T> { }
  [TypeDeclarationTestCase("public class C2<T1, T2>")] public class C2<T1, T2> { }
  [TypeDeclarationTestCase("public class C3<T1, T2, T3>")] public class C3<T1, T2, T3> { }

  [TypeDeclarationTestCase("public struct S1<T>")] public struct S1<T> { }
  [TypeDeclarationTestCase("public struct S2<T1, T2>")] public struct S2<T1, T2> { }
  [TypeDeclarationTestCase("public struct S3<T1, T2, T3>")] public struct S3<T1, T2, T3> { }

  [TypeDeclarationTestCase("public interface I1<T>")] public interface I1<T> { }
  [TypeDeclarationTestCase("public interface I2<T1, T2>")] public interface I2<T1, T2> { }
  [TypeDeclarationTestCase("public interface I3<T1, T2, T3>")] public interface I3<T1, T2, T3> { }

  [TypeDeclarationTestCase("public delegate void D1<T>(T x);")] public delegate void D1<T>(T x);
  [TypeDeclarationTestCase("public delegate void D2<T1, T2>(T1 x, T2 y);")] public delegate void D2<T1, T2>(T1 x, T2 y);
  [TypeDeclarationTestCase("public delegate void D3<T1, T2, T3>(T1 x, T2 y, T3 z);")] public delegate void D3<T1, T2, T3>(T1 x, T2 y, T3 z);

  namespace GenericAttributeTypes {
#if NET7_0_OR_GREATER
#nullable enable annotations
    [TypeDeclarationTestCase("public class GenericAttribute<T>")]
    public class GenericAttribute<T> : Attribute { }

    [TypeDeclarationTestCase("public class GenericAttribute<T1, T2>")]
    public class GenericAttribute<T1, T2> : Attribute { }

    namespace Constraints {
      [TypeDeclarationTestCase("public class GenericAttribute0<TStruct> where TStruct : struct")]
      public class GenericAttribute0<TStruct> : Attribute where TStruct : struct { }

      [TypeDeclarationTestCase("public class GenericAttribute1<TNew> where TNew : new()")]
      public class GenericAttribute1<TNew> : Attribute where TNew : new() { }
    }
#nullable restore
#endif // NET7_0_OR_GREATER
  }

  namespace Constraints {
    [TypeDeclarationTestCase("public class C0<T>")] public class C0<T> { }
    [TypeDeclarationTestCase("public class C1<T> where T : new()")] public class C1<T> where T : new() { }
    [TypeDeclarationTestCase("public class C2_0<T> where T : struct")] public class C2_0<T> where T : struct { }
    [TypeDeclarationTestCase("public class C2_1<T> where T : struct, System.IDisposable")] public class C2_1<T> where T : struct, IDisposable { }
    [TypeDeclarationTestCase("public class C3_0<T> where T : class")] public class C3_0<T> where T : class { }
    [TypeDeclarationTestCase("public class C3_1<T> where T : class, System.IDisposable")] public class C3_1<T> where T : class, IDisposable { }
    [TypeDeclarationTestCase("public class C4<T> where T : System.IDisposable")] public class C4<T> where T : IDisposable { }
    [TypeDeclarationTestCase("public class C5<T> where T : System.ICloneable, System.IDisposable")] public class C5<T> where T : IDisposable, ICloneable { }
    [TypeDeclarationTestCase("public class C6<T> where T : System.ICloneable, new()")] public class C6<T> where T : ICloneable, new() { }
    [TypeDeclarationTestCase("public class C7<T> where T : class, new()")] public class C7<T> where T : class, new() { }
    [TypeDeclarationTestCase("public class C8<T> where T : class, System.ICloneable, new()")] public class C8<T> where T : class, ICloneable, new() { }
    [TypeDeclarationTestCase("public class C9<T> where T : class, System.ICloneable, System.IDisposable, new()")] public class C9<T> where T : class, ICloneable, IDisposable, new() { }

    [TypeDeclarationTestCase("public class C10<T> where T : CBase", TypeWithNamespace = false)]
    public class C10<T> where T : CBase { }
    public class CBase { }

    [TypeDeclarationTestCase("public class C11<T> where T : System.Enum")] public class C11<T> where T : System.Enum { }
    [TypeDeclarationTestCase("public class C12<T> where T : System.Delegate")] public class C12<T> where T : System.Delegate { }
    [TypeDeclarationTestCase("public class C13<T> where T : System.MulticastDelegate")] public class C13<T> where T : System.MulticastDelegate { }

    [TypeDeclarationTestCase("public class C14_0<T> where T : unmanaged")] public class C14_0<T> where T : unmanaged { }
    [TypeDeclarationTestCase("public class C14_1<T> where T : unmanaged, System.IDisposable")] public class C14_1<T> where T : unmanaged, IDisposable { }

    [TypeDeclarationTestCase("public class C15_0<T> where T : notnull")] public class C15_0<T> where T : notnull { }
    [TypeDeclarationTestCase("public class C15_1<T> where T : notnull, System.IDisposable")] public class C15_1<T> where T : notnull, IDisposable { }

    [TypeDeclarationTestCase("public struct S1<T> where T : new()")] public struct S1<T> where T : new() { }

    [TypeDeclarationTestCase("public interface I1<T> where T : new()")] public interface I1<T> where T : new() { }

    [TypeDeclarationTestCase("public delegate void D1<T>() where T : new();")] public delegate void D1<T>() where T : new();

    namespace MultipleTypeArguments {
      [TypeDeclarationTestCase("public class C1<T1, T2> where T1 : new() where T2 : new()")]
      public class C1<T1, T2>
        where T1 : new()
        where T2 : new() { }

      [TypeDeclarationTestCase("public class C2<T1, T2> where T1 : new() where T2 : new()")]
      public class C2<T1, T2>
        where T2 : new()
        where T1 : new() { }

      [TypeDeclarationTestCase("public class C3<T1, T2> where T1 : System.ICloneable where T2 : System.IDisposable")]
      public class C3<T1, T2>
        where T1 : ICloneable
        where T2 : IDisposable { }

      [TypeDeclarationTestCase("public class C4<T1, T2> where T1 : class, System.ICloneable where T2 : System.IDisposable, new()")]
      public class C4<T1, T2>
        where T1 : class, ICloneable
        where T2 : IDisposable, new() { }
    }

    namespace TSelf {
#nullable enable annotations
      [TypeDeclarationTestCase("public interface I<TSelf> where TSelf : I<TSelf>", TypeWithNamespace = false)]
      public interface I<TSelf> where TSelf : I<TSelf> { }

      [TypeDeclarationTestCase("public class C<T> where T : I<T>", TypeWithNamespace = false)]
      public class C<T> where T : I<T> { }

      [TypeDeclarationTestCase("public class C<U, V> where U : I<U> where V : struct", TypeWithNamespace = false)]
      public class C<U, V> where U : I<U> where V : struct { }

#if NET7_0_OR_GREATER
      [TypeDeclarationTestCase("public class CParsable<TParsable> where TParsable : IParsable<TParsable>", TypeWithNamespace = false)]
      public class CParsable<TParsable> where TParsable : IParsable<TParsable> { }
#endif // NET7_0_OR_GREATER
#nullable restore
    }
  }

  namespace VariantTypeParameters {
    [TypeDeclarationTestCase("public interface I1<in T>")] public interface I1<in T> { }
    [TypeDeclarationTestCase("public interface I2<out T>")] public interface I2<out T> { }

    [TypeDeclarationTestCase("public delegate void D1<in T>(T x);")] public delegate void D1<in T>(T x);
    [TypeDeclarationTestCase("public delegate T D2<out T>();")] public delegate T D2<out T>();
  }

  namespace NestedTypes {
    [TypeDeclarationTestCase("public class CN1", TypeWithDeclaringTypeName = false)]
    [TypeDeclarationTestCase("public class CN1", TypeWithDeclaringTypeName = true)]
    public class CN1 {
      [TypeDeclarationTestCase("public class CN2", TypeWithDeclaringTypeName = false)]
      [TypeDeclarationTestCase("public class CN1.CN2", TypeWithDeclaringTypeName = true)]
      public class CN2 {
        [TypeDeclarationTestCase("public class CN3", TypeWithDeclaringTypeName = false)]
        [TypeDeclarationTestCase("public class CN1.CN2.CN3", TypeWithDeclaringTypeName = true)]
        public class CN3 {
        }
      }
    }

    public class C<T> {
      [TypeDeclarationTestCase("public class COpenNested", TypeWithDeclaringTypeName = false)]
      [TypeDeclarationTestCase("public class C<T>.COpenNested", TypeWithDeclaringTypeName = true)]
      public class COpenNested { }

      [TypeDeclarationTestCase("public class COpenNested<U>", TypeWithDeclaringTypeName = false)]
      [TypeDeclarationTestCase("public class C<T>.COpenNested<U>", TypeWithDeclaringTypeName = true)]
      public class COpenNested<U> {}
    }

    public class C : C<int> {
      [TypeDeclarationTestCase("public class CCloseNested", TypeWithDeclaringTypeName = false)]
      [TypeDeclarationTestCase("public class C.CCloseNested", TypeWithDeclaringTypeName = true)]
      public class CCloseNested { }

      [TypeDeclarationTestCase("public class CCloseNested<U>", TypeWithDeclaringTypeName = false)]
      [TypeDeclarationTestCase("public class C.CCloseNested<U>", TypeWithDeclaringTypeName = true)]
      public class CCloseNested<U> { }
    }
  }
}
