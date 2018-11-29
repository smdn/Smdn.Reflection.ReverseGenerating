using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

#pragma warning disable 0169, 0649, 0067
namespace TestCases {
  class TypeNameConversion {
    private static Tuple<Type, string, bool> TestCase(Type type, string expectedResult, bool fullName = true)
    {
      return Tuple.Create(type, expectedResult, fullName);
    }

    public static IEnumerable<Tuple<Type, string, bool>> GenerateTestCases()
    {
      yield return TestCase(typeof(int), "int");
      yield return TestCase(typeof(int[]), "int[]");
      yield return TestCase(typeof(int[,]), "int[,]");
      yield return TestCase(typeof(int[][]), "int[][]");
      yield return TestCase(typeof(int?), "int?");
      yield return TestCase(typeof(int).MakePointerType(), "int*");
      yield return TestCase(typeof(int).MakeByRefType(), "int&");

      yield return TestCase(typeof(System.Guid), "Guid", fullName: false);
      yield return TestCase(typeof(System.Guid?), "Guid?", fullName: false);
      yield return TestCase(typeof(System.Guid), "System.Guid", fullName: true);
      yield return TestCase(typeof(System.Guid?), "System.Guid?", fullName: true);

      yield return TestCase(typeof(Tuple<>), "Tuple<T1>", fullName: false);
      yield return TestCase(typeof(Tuple<int>), "Tuple<int>", fullName: false);
      yield return TestCase(typeof(Tuple<int[]>), "Tuple<int[]>", fullName: false);
      yield return TestCase(typeof(Tuple<int?>), "Tuple<int?>", fullName: false);

      yield return TestCase(typeof(Tuple<,>), "Tuple<T1, T2>", fullName: false);
      yield return TestCase(typeof(Tuple<int, int>), "Tuple<int, int>", fullName: false);

      yield return TestCase(typeof((int, int)), "(int, int)");
      yield return TestCase(typeof((int x, int y)), "(int, int)");
      yield return TestCase(typeof((int, (int, int))), "(int, (int, int))");

      yield return TestCase(typeof(KeyValuePair<,>), "KeyValuePair<TKey, TValue>", fullName: false);
      yield return TestCase(typeof(KeyValuePair<string, int>), "KeyValuePair<string, int>", fullName: false);

      yield return TestCase(typeof(Tuple<Tuple<int>>), "Tuple<Tuple<int>>", fullName: false);
      yield return TestCase(typeof(Tuple<Tuple<int>, Tuple<int>>), "Tuple<Tuple<int>, Tuple<int>>", fullName: false);

      yield return TestCase(typeof(Converter<,>), "Converter<in TInput, out TOutput>", fullName: false);
      yield return TestCase(typeof(Converter<int, string>), "Converter<int, string>", fullName: false);
      yield return TestCase(typeof(Tuple<Converter<int, string>>), "Tuple<Converter<int, string>>", fullName: false);
    }
  }

  class TestAttribute : Attribute {
    public string Expected { get; private set; }

    public bool WithNamespace { get; set; } = true;

    public TestAttribute()
      : this(null)
    {
    }

    public TestAttribute(string expected)
    {
      this.Expected = expected;
    }
  }

  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
  class BaseTypeTestAttribute : Attribute {
    public string Expected { get; private set; }

    public BaseTypeTestAttribute(string expected)
    {
      this.Expected = expected;
    }
  }

  [AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Delegate |
    AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Property
  )]
  class NamespacesTestAttribute : Attribute {
    public string Expected { get; private set; }

    public NamespacesTestAttribute(string expected)
    {
      this.Expected = expected;
    }
  }

  namespace Namespaces {
    namespace Types {
      [Test][NamespacesTest("")] delegate void D0();
      [Test][NamespacesTest("")] delegate void D1(int x);
      [Test][NamespacesTest("")] delegate void D2(int? x);
      [Test][NamespacesTest("")] delegate void D3(int[] x);
      [Test][NamespacesTest("")] unsafe delegate void D4(int* x);
      [Test][NamespacesTest("System")] delegate void D5(Guid x);
      [Test][NamespacesTest("System")] delegate void D6(Guid? x);
      [Test][NamespacesTest("System")] delegate void D7(Guid[] x);
      [Test][NamespacesTest("System.Collections.Generic")] delegate void D8(List<int> x);
      [Test][NamespacesTest("System, System.Collections.Generic")] delegate void D9(List<Guid> x);

      [Test][NamespacesTest("System.Collections.Generic")] class ListEx1 : List<int> {}
      [Test][NamespacesTest("System, System.Collections.Generic")] class ListEx2 : List<Guid> {}
      [Test][NamespacesTest("System.Collections.Generic")] class ListEx3<T> : List<T> {}
    }

    namespace Members {
      class C {
        [Test][NamespacesTest("")] public int F1;
        [Test][NamespacesTest("")] public int? F2;
        [Test][NamespacesTest("System")] public Guid F3;
        [Test][NamespacesTest("System.Collections.Generic")] public List<int> F4;
        [Test][NamespacesTest("System, System.Collections.Generic")] public List<Guid> F5;
        [Test][NamespacesTest("")] public int[] F6;
        [Test][NamespacesTest("")] public Nullable<int> F7;
        [Test][NamespacesTest("System")] public Action<int> F8;
        [Test][NamespacesTest("System, System.Collections.Generic")] public List<Action<int>> F9;
        [Test][NamespacesTest("System, System.Collections.Generic")] public Action<List<int>> F10;
        [Test][NamespacesTest("System, System.Collections.Generic, System.Collections.ObjectModel")] public System.Collections.ObjectModel.Collection<List<Action<int>>> F11;
        [Test][NamespacesTest("System, System.Collections.Generic, System.Collections.ObjectModel")] public Action<System.Collections.ObjectModel.Collection<List<int>>> F12;

        [Test][NamespacesTest("System")] public event EventHandler E1;
        [Test][NamespacesTest("System, System.Collections.Generic")] public event EventHandler<IList<int>> E2;

        [Test][NamespacesTest("")] public int P1 { get; set; }
        [Test][NamespacesTest("")] public int? P2 { get; set; }
        [Test][NamespacesTest("System")] public Guid P3 { get; set; }
        [Test][NamespacesTest("System.Collections.Generic")] public IList<int> P4 { get; set; }
        [Test][NamespacesTest("System, System.Collections.Generic")] public IList<Guid> P5 { get; set; }

        [Test][NamespacesTest("")] public void M0() {}
        [Test][NamespacesTest("")] public void M1(int x) {}
        [Test][NamespacesTest("")] public void M2(int? x) {}
        [Test][NamespacesTest("System")] public void M3(Guid x) {}
        [Test][NamespacesTest("System.Collections.Generic")] public void M4(List<int> x) {}
        [Test][NamespacesTest("System, System.Collections.Generic")] public void M5(List<Guid> x) {}
        [Test][NamespacesTest("")] public unsafe void M6(int* x) {}

        [Test][NamespacesTest("")] public int M1() => throw new NotImplementedException();
        [Test][NamespacesTest("")] public int? M2() => throw new NotImplementedException();
        [Test][NamespacesTest("System")] public Guid M3() => throw new NotImplementedException();
        [Test][NamespacesTest("System.Collections.Generic")] public List<int> M4() => throw new NotImplementedException();
        [Test][NamespacesTest("System, System.Collections.Generic")] public List<Guid> M5() => throw new NotImplementedException();
      }
    }
  }

  [AttributeUsage(AttributeTargets.All)]
  class AttributeTestAttribute : Attribute {
    public string Expected { get; private set; }

    public AttributeTestAttribute(string expected)
    {
      this.Expected = expected;
    }
  }

  namespace Attributes {
    [Test][AttributeTest("[System.Flags]")]
    [Flags]
    enum Flags1 : int {}

    [Test(WithNamespace = false)][AttributeTest("[Flags]")]
    [Flags]
    enum Flags2 : int {}

    [Test][AttributeTest("[System.Flags], [System.Obsolete]")]
    [Flags]
    [Obsolete]
    enum Flags3 : int {}

    [Test][AttributeTest("[System.Flags], [System.Obsolete]")]
    [Obsolete]
    [Flags]
    enum Flags4 : int {}

    [Test][AttributeTest("[System.Obsolete]")]
    [Obsolete]
    class Obsolete1 {}

    [Test][AttributeTest("[System.Obsolete(\"obsolete\")]")]
    [Obsolete("obsolete")]
    class Obsolete2 {}

    [Test][AttributeTest("[System.Obsolete(\"deprecated\", true)]")]
    [Obsolete("deprecated", true)]
    class Obsolete3 {}

    [Test][AttributeTest("[System.Obsolete(\"deprecated\")]")]
    [Obsolete("deprecated", false)]
    class Obsolete4 {}

    [Test][AttributeTest("[System.Serializable]")]
    [Serializable]
    class Serializable1 {}

    class Conditionals {
      [Test][AttributeTest("[System.Diagnostics.Conditional(\"DEBUG\")]")]
      [System.Diagnostics.Conditional("DEBUG")]
      public void M1() {}

      [Test(WithNamespace = false)][AttributeTest("[Conditional(\"DEBUG\")]")]
      [System.Diagnostics.Conditional("DEBUG")]
      public void M2() {}
    }

    static class Extension {
      [Test][AttributeTest("")] // does not emit System.Runtime.CompilerServices.ExtensionAttribute
      public static void M1(this int x) {}
    }

    [Test][AttributeTest("[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 1)]")]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 1)]
    struct StructLayout1 {
      [Test][AttributeTest("[System.Runtime.InteropServices.FieldOffset(0)]")]
      [System.Runtime.InteropServices.FieldOffset(0)]
      public byte F0;

      [Test(WithNamespace = false)][AttributeTest("[FieldOffset(1)]")]
      [System.Runtime.InteropServices.FieldOffset(1)]
      public byte F1;
    }

    [Test][AttributeTest("[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 1)]")]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 1)]
    struct StructLayout2 {
      [System.Runtime.InteropServices.FieldOffset(0)] public byte F0;
    }

    [Test][AttributeTest("")]
    struct NoStructLayout {}
  }

  namespace TypeNames {
    class PrimitiveTypes {
      [Test("public void MV() {}")] public void MV() {} // System.Void -> void

      [Test("public void M(object x) {}")]    public void M(object x) {}
      [Test("public void M(byte x) {}")]      public void M(byte x) {}
      [Test("public void M(sbyte x) {}")]     public void M(sbyte x) {}
      [Test("public void M(ushort x) {}")]    public void M(ushort x) {}
      [Test("public void M(short x) {}")]     public void M(short x) {}
      [Test("public void M(uint x) {}")]      public void M(uint x) {}
      [Test("public void M(int x) {}")]       public void M(int x) {}
      [Test("public void M(ulong x) {}")]     public void M(ulong x) {}
      [Test("public void M(long x) {}")]      public void M(long x) {}
      [Test("public void M(float x) {}")]     public void M(float x) {}
      [Test("public void M(double x) {}")]    public void M(double x) {}
      [Test("public void M(decimal x) {}")]   public void M(decimal x) {}
      [Test("public void M(char x) {}")]      public void M(char x) {}
      [Test("public void M(string x) {}")]    public void M(string x) {}
      [Test("public void M(bool x) {}")]      public void M(bool x) {}
    }

    class CombinedTypes {
      [Test("public void M(int x) {}")] public void M(int x) {}
      [Test("public void M(int[] x) {}")] public void M(int[] x) {}
      [Test("public unsafe void M(int* x) {}")] public unsafe void M(int* x) {}
      [Test("public void M0(int? x) {}")] public void M0(int? x) {}
      [Test("public void M1(int? x) {}")] public void M1(Nullable<int> x) {}
      [Test("public void M(System.Collections.Generic.IEnumerable<int> x) {}")] public void M(IEnumerable<int> x) {}
      [Test("public void M0(System.Collections.Generic.IEnumerable<int?> x) {}")] public void M0(IEnumerable<int?> x) {}
      [Test("public void M1(System.Collections.Generic.IEnumerable<int?> x) {}")] public void M1(IEnumerable<Nullable<int>> x) {}
      [Test("public void M(System.Collections.Generic.KeyValuePair<int, int> x) {}")] public void M(KeyValuePair<int, int> x) {}
      [Test("public void M(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int, int>> x) {}")] public void M(IEnumerable<KeyValuePair<int, int>> x) {}

      [Test("public void M(System.Guid x) {}")] public void M(Guid x) {}
      [Test("public void M(System.Guid[] x) {}")] public void M(Guid[] x) {}
      [Test("public unsafe void M(System.Guid* x) {}")] public unsafe void M(Guid* x) {}
      [Test("public void M0(System.Guid? x) {}")] public void M0(Guid? x) {}
      [Test("public void M1(System.Guid? x) {}")] public void M1(Nullable<Guid> x) {}
      [Test("public void M(System.Collections.Generic.IEnumerable<System.Guid> x) {}")] public void M(IEnumerable<Guid> x) {}
      [Test("public void M0(System.Collections.Generic.IEnumerable<System.Guid?> x) {}")] public void M0(IEnumerable<System.Guid?> x) {}
      [Test("public void M1(System.Collections.Generic.IEnumerable<System.Guid?> x) {}")] public void M1(IEnumerable<Nullable<System.Guid>> x) {}
      [Test("public void M(System.Collections.Generic.KeyValuePair<System.Guid, System.Guid> x) {}")] public void M(KeyValuePair<Guid, Guid> x) {}
      [Test("public void M(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.Guid, System.Guid>> x) {}")] public void M(IEnumerable<KeyValuePair<Guid, Guid>> x) {}

      [Test("public void M(int[,] x) {}")] public void M(int[,] x) {}
      [Test("public void M(int[,,] x) {}")] public void M(int[,,] x) {}

      [Test("public void M(int[][] x) {}")] public void M(int[][] x) {}
      [Test("public void M(int[][][] x) {}")] public void M(int[][][] x) {}

      [Test("public void M<T>(System.Tuple<T> x) {}")] public void M<T>(Tuple<T> x) {}
      [Test("public void M<T1, T2>(System.Tuple<T1, T2> x) {}")] public void M<T1, T2>(Tuple<T1, T2> x) {}
      [Test("public void M<T1, T2, T3>(System.Tuple<T1, T2, T3> x) {}")] public void M<T1, T2, T3>(Tuple<T1, T2, T3> x) {}
    }

    namespace NestedTypes {
      public class C1 {
        public class C2 {
          public class C3 {
          }

          [Test("public void M(C1.C2.C3 x) {}", WithNamespace = false)] public void M(C3 x) {}
        }

        [Test("public void M(C1.C2 x) {}", WithNamespace = false)] public void M(C2 x) {}
        [Test("public void M(C1.C2.C3 x) {}", WithNamespace = false)] public void M(C2.C3 x) {}
      }

      class C {
        [Test("public void M(C1.C2 x) {}", WithNamespace = false)] public void M(C1.C2 x) {}
        [Test("public void M(C1.C2.C3 x) {}", WithNamespace = false)] public void M(C1.C2.C3 x) {}
      }
    }
  }

  namespace TypeDefinitions {
    namespace Delegates {
      [Test("internal delegate void D0();")] internal delegate void D0();
      [Test("public delegate void D1();")] public delegate void D1();
      [Test("public delegate void D2(int x);")] public delegate void D2(int x);
      [Test("public delegate int D3();")] public delegate int D3();
      [Test("public delegate int D4(int x);")] public delegate int D4(int x);
      [Test("public delegate int D5(int x, int y);")] public delegate int D5(int x, int y);

      [Test("public delegate Guid D61();", WithNamespace = false)] public delegate Guid D61();
      [Test("public delegate System.Guid D62();", WithNamespace = true)] public delegate Guid D62();

      class Accessibilities {
        [Test("public delegate void D1();")] public delegate void D1();
        [Test("internal delegate void D2();")] internal delegate void D2();
        [Test("protected delegate void D3();")] protected delegate void D3();
        [Test("internal protected delegate void D4();")] protected internal delegate void D4();
        [Test("internal protected delegate void D5();")] internal protected delegate void D5();
        [Test("private protected delegate void D6();")] private protected delegate void D6();
        [Test("private protected delegate void D7();")] protected private delegate void D7();
        [Test("private delegate void D8();")] private delegate void D8();
      }
    }

    namespace Enums {
      [Test("internal enum E0 : int")] internal enum E0 {};
      [Test("public enum E1 : int")] public enum E1 {};

      class Accessibilities {
        [Test("public enum E1 : int")] public enum E1 {}
        [Test("internal enum E2 : int")] internal enum E2 {}
        [Test("protected enum E3 : int")] protected enum E3 {}
        [Test("internal protected enum E4 : int")] protected internal enum E4 {}
        [Test("internal protected enum E5 : int")] internal protected enum E5 {}
        [Test("private protected enum E6 : int")] private protected enum E6 {}
        [Test("private protected enum E7 : int")] protected private enum E7 {}
        [Test("private enum E8 : int")] private enum E8 {}
      }
    }

    namespace Interfaces {
      [Test("internal interface I0")] internal interface I0 {};
      [Test("public interface I1")] public interface I1 {};

      class Accessibilities {
        [Test("public interface I1")] public interface I1 {}
        [Test("internal interface I2")] internal interface I2 {}
        [Test("protected interface I3")] protected interface I3 {}
        [Test("internal protected interface I4")] protected internal interface I4 {}
        [Test("internal protected interface I5")] internal protected interface I5 {}
        [Test("private protected interface I6")] private protected interface I6 {}
        [Test("private protected interface I7")] protected private interface I7 {}
        [Test("private interface I8")] private interface I8 {}
      }
    }

    namespace Structures {
      [Test("internal struct S0")] internal struct S0 {};
      [Test("public struct S1")] public struct S1 {};
      [Test("public readonly struct S2")] public readonly struct S2 {}
      [Test("public ref struct S3")] public ref struct S3 {}
      [Test("public readonly ref struct S4")] public readonly ref struct S4 {}

      class Accessibilities {
        [Test("public struct S1")] public struct S1 {}
        [Test("internal struct S2")] internal struct S2 {}
        [Test("protected struct S3")] protected struct S3 {}
        [Test("internal protected struct S4")] protected internal struct S4 {}
        [Test("internal protected struct S5")] internal protected struct S5 {}
        [Test("private protected struct S6")] private protected struct S6 {}
        [Test("private protected struct S7")] protected private struct S7 {}
        [Test("private struct S8")] private struct S8 {}
      }
    }

    namespace Classes {
      [Test("internal class C0")] internal class C0 {};
      [Test("public class C1")] public class C1 {};

      class Accessibilities {
        [Test("public class C1")] public class C1 {}
        [Test("internal class C2")] internal class C2 {}
        [Test("protected class C3")] protected class C3 {}
        [Test("internal protected class C4")] protected internal class C4 {}
        [Test("internal protected class C5")] internal protected class C5 {}
        [Test("private protected class C6")] private protected class C6 {}
        [Test("private protected class C7")] protected private class C7 {}
        [Test("private class C8")] private class C8 {}
      }

      namespace Modifiers {
        [Test("public abstract class C1")] public abstract class C1 {}
        [Test("public static class C2")] public static class C2 {}
        [Test("public sealed class C3")] public sealed class C3 {}
      }
    }

    namespace GenericTypes {
      [Test("public class C1<T>")] public class C1<T> {}
      [Test("public class C2<T1, T2>")] public class C2<T1, T2> {}
      [Test("public class C3<T1, T2, T3>")] public class C3<T1, T2, T3> {}

      [Test("public struct S1<T>")] public struct S1<T> {}
      [Test("public struct S2<T1, T2>")] public struct S2<T1, T2> {}
      [Test("public struct S3<T1, T2, T3>")] public struct S3<T1, T2, T3> {}

      [Test("public interface I1<T>")] public interface I1<T> {}
      [Test("public interface I2<T1, T2>")] public interface I2<T1, T2> {}
      [Test("public interface I3<T1, T2, T3>")] public interface I3<T1, T2, T3> {}

      [Test("public delegate void D1<T>(T x);")] public delegate void D1<T>(T x);
      [Test("public delegate void D2<T1, T2>(T1 x, T2 y);")] public delegate void D2<T1, T2>(T1 x, T2 y);
      [Test("public delegate void D3<T1, T2, T3>(T1 x, T2 y, T3 z);")] public delegate void D3<T1, T2, T3>(T1 x, T2 y, T3 z);

      namespace Constraints1 {
        [Test("public class C1<T> where T : new()")] public class C1<T> where T : new() {}
        [Test("public class C2<T> where T : struct")] public class C2<T> where T : struct {}
        [Test("public class C3<T> where T : class")] public class C3<T> where T : class {}
        [Test("public class C4<T> where T : System.IDisposable")] public class C4<T> where T : IDisposable {}
        [Test("public class C5<T> where T : System.ICloneable, System.IDisposable")] public class C5<T> where T : IDisposable, ICloneable {}
        [Test("public class C6<T> where T : System.ICloneable, new()")] public class C6<T> where T : ICloneable, new() {}
        [Test("public class C7<T> where T : class, new()")] public class C7<T> where T : class, new() {}
        [Test("public class C8<T> where T : class, System.ICloneable, new()")] public class C8<T> where T : class, ICloneable, new() {}
        [Test("public class C9<T> where T : class, System.ICloneable, System.IDisposable, new()")] public class C9<T> where T : class, ICloneable, IDisposable, new() {}

        [Test("public class C10<T> where T : CBase", WithNamespace = false)]
        public class C10<T> where T : CBase {}
        public class CBase {}

        [Test("public class C11<T> where T : System.Enum")] public class C11<T> where T : System.Enum {}
        [Test("public class C12<T> where T : System.Delegate")] public class C12<T> where T : System.Delegate {}
        [Test("public class C13<T> where T : System.MulticastDelegate")] public class C13<T> where T : System.MulticastDelegate {}

        [Test("public struct S1<T> where T : new()")] public struct S1<T> where T : new() {}

        [Test("public interface I1<T> where T : new()")] public interface I1<T> where T : new() {}

        [Test("public delegate void D1<T>() where T : new();")] public delegate void D1<T>() where T : new();
      }

      namespace Constraints2 {
        [Test("public class C1<T1, T2> where T1 : new() where T2 : new()")]
        public class C1<T1, T2>
          where T1 : new()
          where T2 : new()
        {}

        [Test("public class C2<T1, T2> where T1 : new() where T2 : new()")]
        public class C2<T1, T2>
          where T2 : new()
          where T1 : new()
        {}

        [Test("public class C3<T1, T2> where T1 : System.ICloneable where T2 : System.IDisposable")]
        public class C3<T1, T2>
          where T1 : ICloneable
          where T2 : IDisposable
        {}

        [Test("public class C4<T1, T2> where T1 : class, System.ICloneable where T2 : System.IDisposable, new()")]
        public class C4<T1, T2>
          where T1 : class, ICloneable
          where T2 : IDisposable, new()
        {}
      }

      namespace VariantTypeParameters {
        [Test("public interface I1<in T>")] public interface I1<in T> {}
        [Test("public interface I2<out T>")] public interface I2<out T> {}

        [Test("public delegate void D1<in T>(T x);")] public delegate void D1<in T>(T x);
        [Test("public delegate T D2<out T>();")] public delegate T D2<out T>();
      }
    }

#if false
    // TODO: 'new' modifier
    class C {
      class C0 {
        protected class CX {
        }
      }
      class C1 : C0 {
        new protected class CX {
        }
      }
    }
#endif

    namespace ClassAndInterfaceTypeLists {
      [Test(WithNamespace = false)]
      [BaseTypeTest("")]
      public abstract class C {}

      [Test(WithNamespace = false)]
      [BaseTypeTest("C")]
      public abstract class CC : C {}

      [Test(WithNamespace = false)]
      [BaseTypeTest("CC")]
      public abstract class CCC : CC {}



      [Test]
      [BaseTypeTest("System.IDisposable")]
      public abstract class C0 : IDisposable {
        public abstract void Dispose();
      }

      [Test(WithNamespace = false)]
      [BaseTypeTest("C0")]
      public abstract class C1 : C0 {
      }

      [Test(WithNamespace = false)]
      [BaseTypeTest("C0, ICloneable")]
      public abstract class C2 : C0, ICloneable {
        public abstract object Clone();
      }

      [Test(WithNamespace = false)]
      // TODO: reimplemented interface? [BaseTypeTest("C0, ICloneable, IDisposable")]
      [BaseTypeTest("C0, ICloneable")]
      public abstract class C3 : C0, IDisposable, ICloneable {
        public abstract object Clone();
        void IDisposable.Dispose() {}
      }

      [Test(WithNamespace = false)]
      [BaseTypeTest("C0, ICloneable, IEquatable<C0>")]
      public abstract class C4 : C0, IEquatable<C0>, ICloneable {
        public abstract object Clone();
        public abstract bool Equals(C0 other);
      }

      [Test(WithNamespace = false)]
      [BaseTypeTest("List<int>")]
      public abstract class CList : List<int> {
      }



      [Test]
      [BaseTypeTest("System.IDisposable")]
      public abstract class X0 : IDisposable {
        public abstract void Dispose();
      }

      [Test(WithNamespace = false)]
      [BaseTypeTest("X0, ICloneable")]
      public abstract class X1 : X0, ICloneable {
        public abstract object Clone();
      }



      [Test]
      [BaseTypeTest("System.IDisposable")]
      public struct S0 : IDisposable {
        public void Dispose() {}
      }

      [Test(WithNamespace = false)]
      [BaseTypeTest("IEquatable<S0>")]
      public struct S1 : IEquatable<S0> {
        public bool Equals(S0 other) => throw new NotImplementedException();
      }

      [Test(WithNamespace = false)]
      [BaseTypeTest("IDisposable, IEquatable<S0>")]
      public struct S2 : IEquatable<S0>, IDisposable {
        public void Dispose() {}
        public bool Equals(S0 other) => throw new NotImplementedException();
      }


      [Test(WithNamespace = false)]
      [BaseTypeTest("")]
      public interface I {}

      [Test(WithNamespace = false)]
      [BaseTypeTest("I")]
      public interface II : I {}

      [Test(WithNamespace = false)]
      [BaseTypeTest("II")]
      public interface III : II {}


      [Test]
      [BaseTypeTest("System.IDisposable")]
      public interface I0 : IDisposable {}

      [Test(WithNamespace = false)]
      [BaseTypeTest("I0")]
      public interface I1 : I0 {}

      [Test(WithNamespace = false)]
      [BaseTypeTest("I1, ICloneable")]
      public interface I2 : I1, ICloneable {}

      [Test(WithNamespace = false)]
      // TODO: reimplemented interface? [BaseTypeTest("I1, ICloneable, IDisposable")]
      [BaseTypeTest("I1, ICloneable")]
      public interface I3 : I1, IDisposable, ICloneable {}

      [Test(WithNamespace = false)]
      [BaseTypeTest("I1, ICloneable, IEquatable<I0>")]
      public interface I4 : I1,  IEquatable<I0>, ICloneable {}
    }
  }

  namespace MemberDefinitions {
    namespace Fields {
      public class Types {
        [Test("public int F1;")] public int F1;
        [Test("public float F2;")] public float F2;
        [Test("public string F3;")] public string F3;
        [Test("public System.Guid F4;")] public Guid F4;
      }

      namespace EnumFields {
        public enum Ints : int {
          [Test("A = 0,")] A = 0,
          [Test("B = 1,")] B = 1,
          [Test("C = 2,")] C = 2,
        }

        public enum Bytes : byte {
          [Test("A = 0,")] A = 0,
          [Test("B = 1,")] B = 1,
          [Test("C = 2,")] C = 2,
        }

        namespace Flags {
          [Flags()]
          public enum Ints : int {
            [Test("A = 0x00000000,")] A = 0x00000000,
            [Test("B = 0x00000001,")] B = 0x00000001,
            [Test("C = 0x00000010,")] C = 0x00000010,
            [Test("Z = 0x7fffffff,")] Z = 0x7fffffff,
          }

          [Flags()]
          public enum Shorts : short {
            [Test("A = 0x0000,")] A = 0x0000,
            [Test("B = 0x0001,")] B = 0x0001,
            [Test("C = 0x0010,")] C = 0x0010,
            [Test("Z = 0x7fff,")] Z = 0x7fff,
          }

          [Flags()]
          public enum Bytes : byte {
            [Test("A = 0x00,")] A = 0x00,
            [Test("B = 0x01,")] B = 0x01,
            [Test("C = 0x10,")] C = 0x10,
            [Test("Z = 0xff,")] Z = 0xff,
          }
        }
      }

      public class Accessibilities {
        [Test("public int F1;")] public int F1;
        [Test("internal int F2;")] internal int F2;
        [Test("protected int F3;")] protected int F3;
        [Test("internal protected int F4;")] protected internal int F4;
        [Test("internal protected int F5;")] internal protected int F5;
        [Test("private protected int F6;")] private protected int F6;
        [Test("private protected int F7;")] protected private int F7;
        [Test("private int F8;")] private int F8;
      }

      public class Modifiers {
        [Test("public int F4;")] public int F4;
        [Test("public readonly int F5;")] public readonly int F5;
        [Test("public const int F6 = 123;")] public const int F6 = 123;
        [Test("public static int F7;")] public static int F7;
        [Test("public static readonly int F8 = 0;")]  public static readonly int F8;
        [Test("public static readonly int F9 = 0;")]  static readonly public int F9;
        [Test("public static readonly int F10 = 0;")] readonly public static int F10;
      }

      namespace StaticValues {
        public class ValueTypes {
          [Test("public int F1;")] public int F1 = 1;
          [Test("public readonly int F2;")] public readonly int F2 = 2;
          [Test("public const int F3 = 3;")] public const int F3 = 3;
          [Test("public static int F4;")] public static int F4 = 4;
          [Test("public static readonly int F5 = 5;")] public static readonly int F5 = 5;
          [Test("public static readonly int F6 = int.MaxValue;")] public static readonly int F6 = int.MaxValue;
          [Test("public static readonly int F7 = int.MinValue;")] public static readonly int F7 = int.MinValue;
        }

        public class Enums {
          [Test("public const System.DateTimeKind F1 = System.DateTimeKind.Unspecified;")] public const DateTimeKind F1 = DateTimeKind.Unspecified;
          [Test("public const System.DateTimeKind F2 = System.DateTimeKind.Unspecified;")] public const DateTimeKind F2 = default(DateTimeKind);
          [Test("public const System.DateTimeKind F3 = System.DateTimeKind.Local;")] public const DateTimeKind F3 = DateTimeKind.Local;
          [Test("public const System.DateTimeKind F4 = (System.DateTimeKind)42;")] public const DateTimeKind F4 =(DateTimeKind)42;
        }

        public class EnumFlags {
          [Test("public const System.AttributeTargets F1 = System.AttributeTargets.All;")] public const AttributeTargets F1 = AttributeTargets.All;
          [Test("public const System.AttributeTargets F2 = default(System.AttributeTargets);")] public const AttributeTargets F2 = default(AttributeTargets);
          [Test("public const System.AttributeTargets F3 = System.AttributeTargets.Assembly;")] public const AttributeTargets F3 = AttributeTargets.Assembly;
          [Test("public const System.AttributeTargets F4 = (System.AttributeTargets)3;")] public const AttributeTargets F4 = AttributeTargets.Assembly | AttributeTargets.Module;
          [Test("public const System.AttributeTargets F5 = (System.AttributeTargets)42;")] public const AttributeTargets F5 =(AttributeTargets)42;
        }

        public class Booleans {
          [Test("public const bool F1 = false;")] public const bool F1 = false;
          [Test("public const bool F2 = true;")] public const bool F2 = true;
        }

        public class Chars {
          [Test("public const char F1 = 'A';")] public const char F1 = 'A';
          [Test("public const char F2 = '\\u0000';")] public const char F2 = '\0';
          [Test("public const char F3 = '\\u000A';")] public const char F3 = '\n';
          [Test("public const char F4 = '\\u000A';")] public const char F4 = '\u000A';
          [Test("public const char F5 = '\\'';")] public const char F5 = '\'';
          [Test("public const char F6 = '\"';")] public const char F6 = '"';
        }

        public class Strings {
          [Test("public const string F1 = \"string\";")] public const string F1 = "string";
          [Test("public const string F2 = \"\";")] public const string F2 = "";
          [Test("public const string F3 = null;")] public const string F3 = null;
          [Test("public const string F4 = \"\\u0000\";")] public const string F4 = "\0";
          [Test("public const string F5 = \"hello\\u000A\";")] public const string F5 = "hello\n";
          [Test("public const string F6 = \"\\\"\";")] public const string F6 = "\"";
        }

        public class NonPrimitiveValueTypes {
          [Test("public static readonly System.Guid F1 = System.Guid.Empty;")] public static readonly Guid F1 = Guid.Empty;
          [Test("public static readonly System.Guid F2 = System.Guid.Empty;")] public static readonly Guid F2 = default(Guid);

          [Test("public static readonly System.DateTimeOffset F3 = System.DateTimeOffset.MinValue;")] public static readonly DateTimeOffset F3 = default(DateTimeOffset);
          [Test("public static readonly System.DateTimeOffset F4 = System.DateTimeOffset.MinValue;")] public static readonly DateTimeOffset F4 = DateTimeOffset.MinValue;
          [Test("public static readonly System.DateTimeOffset F5 = System.DateTimeOffset.MaxValue;")] public static readonly DateTimeOffset F5 = DateTimeOffset.MaxValue;
          [Test("public static readonly System.DateTimeOffset F6; // = \"2018/10/31 21:00:00 +09:00\"")] public static readonly DateTimeOffset F6 = new DateTimeOffset(2018, 10, 31, 21, 0, 0, 0, TimeSpan.FromHours(+9.0));

          // XXX: System.Threading.CancellationToken.None is a property
          [Test("public static readonly System.Threading.CancellationToken F7 = default(System.Threading.CancellationToken);")] public static readonly CancellationToken F7 = CancellationToken.None;
          [Test("public static readonly System.Threading.CancellationToken F8 = default(System.Threading.CancellationToken);")] public static readonly CancellationToken F8 = default(CancellationToken);
        }

        namespace NonPrimitiveValueTypes_FieldOfDeclaringType {
          public struct S1 {
            [Test("public static readonly S1 Empty; // = \"foo\"", WithNamespace = false)]
            public static readonly S1 Empty = default(S1);

            public override string ToString() => "foo";
          }

          public struct S2 {
            [Test("public static readonly S2 Empty; // = \"\\\"\\u0000\\\"\"", WithNamespace = false)]
            public static readonly S2 Empty = default(S2);

            public override string ToString() => "\"\0\"";
          }
        }

        public class ReferenceTypes {
          [Test("public static readonly System.Uri F1 = null;")] public static readonly Uri F1 = null;
          [Test("public static readonly System.Uri F2; // = \"http://example.com/\"")] public static readonly Uri F2 = new Uri("http://example.com/");

          [Test("public static readonly System.Collections.Generic.IEnumerable<int> F3 = null;")] public static readonly IEnumerable<int> F3 = null;
          [Test("public static readonly System.Collections.Generic.IEnumerable<int> F4; // = \"System.Int32[]\"")] public static readonly IEnumerable<int> F4 = new int[] {0, 1, 2, 3};
        }
      }
    }

    namespace Properties {
      public class Accessors {
        [Test("public int P1 { get; set; }")] public int P1 { get; set; }
        [Test("public int P2 { get; }")] public int P2 { get; }
        [Test("public int P3 { set; }")] public int P3 { set { int x = value; } }
      }

      public class Modifiers1 {
        [Test("public static int P1 { get; set; }")] public static int P1 { get; set; }
      }

      public abstract class Modifiers_Abstract {
        [Test("public abstract int P1 { get; set; }")] public abstract int P1 { get; set; }
        [Test("public virtual int P2 { get; set; }")] public virtual int P2 { get; set; }
      }

      public abstract class Modifiers_Override : Modifiers_Abstract {
        [Test("public override int P2 { get; set; }")] public override int P2 { get; set; }
      }

      public abstract class Modifiers_New : Modifiers_Abstract {
        [Test("public int P2 { get; set; }")] public new int P2 { get; set; }
      }

      public abstract class Modifiers_NewVirtual : Modifiers_Abstract {
        [Test("public virtual int P2 { get; set; }")] public new virtual int P2 { get; set; }
      }

      public class Indexers1 {
        [IndexerName("Indexer")]
        [Test("public int this[int x] { get; set; }")] public int this[int x] { get { return 0; } set { int val = value; } }
      }

      public class Indexers2 {
        [IndexerName("Indexer")]
        [Test("public int this[int x, int y] { get; set; }")] public int this[int x, int y] { get { return 0; } set { int val = value; } }
      }

      public class Indexers3 {
        [IndexerName("Indexer")]
        [Test("public int this[int @in] { get; set; }")] public int this[int @in] { get { return 0; } set { int val = value; } }
      }

      public class AccessibilitiesOfProperty {
        [Test("public int P1 { get; set; }")] public int P1 { get; set; }
        [Test("internal int P2 { get; set; }")] internal int P2 { get; set; }
        [Test("protected int P3 { get; set; }")] protected int P3 { get; set; }
        [Test("internal protected int P4 { get; set; }")] protected internal int P4 { get; set; }
        [Test("internal protected int P5 { get; set; }")] internal protected int P5 { get; set; }
        [Test("private protected int P6 { get; set; }")] private protected int P6 { get; set; }
        [Test("private protected int P7 { get; set; }")] protected private int P7 { get; set; }
        [Test("private int P8 { get; set; }")] private int P8 { get; set; }
      }

      public class AccessibilitiesOfAccessors_Public {
        [Test("public int P1 { get; internal set; }")] public int P1 { get; internal set; }
        [Test("public int P2 { get; protected set; }")] public int P2 { get; protected set; }
        [Test("public int P3 { get; internal protected set; }")] public int P3 { get; protected internal set; }
        [Test("public int P4 { get; internal protected set; }")] public int P4 { get; internal protected set; }
        [Test("public int P5 { get; private protected set; }")] public int P5 { get; private protected set; }
        [Test("public int P6 { get; private protected set; }")] public int P6 { get; protected private set; }
        [Test("public int P7 { get; private set; }")] public int P7 { get; private set; }
      }

      public class AccessibilitiesOfAccessors_FamilyOrAssembly {
        [Test("internal protected int P0 { get; set; }")] internal protected int P0 { get; set; }
        [Test("internal protected int P1 { get; internal set; }")] internal protected int P1 { get; internal set; }
        [Test("internal protected int P2 { get; protected set; }")] internal protected int P2 { get; protected set; }
        [Test("internal protected int P3 { get; private protected set; }")] internal protected int P3 { get; private protected set; }
        [Test("internal protected int P4 { get; private protected set; }")] internal protected int P4 { get; protected private set; }
        [Test("internal protected int P5 { get; private set; }")] internal protected int P5 { get; private set; }
      }

#if false
      namespace StaticValues {
        class Accessors {
          [Test("public int P1 { get; } = 1;")] public int P1 { get; } = 1;
          [Test("public int P2 { get; set; } = 2;")] public int P2 { get; set; } = 2;
          [Test("public int P3 { get; private set; } = 3;")] public int P3 { get; private set; } = 3;
        }
      }
#endif
    }

    namespace Methods {
      namespace Modifiers {
        public class Static {
          [Test("public static void M1() {}")] public static void M1() { }
        }

        public abstract class Abstract {
          [Test("public abstract void M();")] public abstract void M();
        }

        public class Virtual {
          [Test("public virtual void M() {}")] public virtual void M() { }
        }

        public class Override : Virtual {
          [Test("public override void M() {}")] public override void M() { }
        }

        public class SealedOverride1 : Virtual {
          [Test("public sealed override void M() {}")] public sealed override void M() { }
        }

        public class SealedOverride2 : Virtual {
          [Test("public sealed override void M() {}")] public override sealed void M() { }
        }

        public abstract class New : Virtual {
          [Test("public void M() {}")] public new void M() { }
        }

        public abstract class NewVirtual : Virtual {
          [Test("public virtual void M() {}")] public new virtual void M() { }
        }
      }

      public class Accessibilities {
        [Test("public void M1() {}")] public void M1() { }
        [Test("internal void M2() {}")] internal void M2() { }
        [Test("protected void M3() {}")] protected void M3() { }
        [Test("internal protected void M4() {}")] protected internal void M4() { }
        [Test("internal protected void M5() {}")] internal protected void M5() { }
        [Test("private protected void M6() {}")] private protected void M6() { }
        [Test("private protected void M7() {}")] protected private void M7() { }
        [Test("private void M8() {}")] private void M8() { }
      }

      public class ParameterNames {
        [Test("public void M1(int @value) {}")] public void M1(int @value) {}
        [Test("public void M2(int @readonly) {}")] public void M2(int @readonly) {}
        [Test("public void M3(int @where) {}")] public void M3(int @where) {}
        [Test("public void M4(int @var) {}")] public void M4(int @var) {}
      }

      public static class ExtensionMethods {
        [Test("public static void M(this int x) {}")] public static void M(this int x) { }
        [Test("public static void M(this int x, int y) {}")] public static void M(this int x, int y) { }
      }

      public static class ReferenceReturnValues {
        [Test("public static ref int M() {}")] public static ref int M() => throw new NotImplementedException();
      }

      public static class TupleReturnValues {
        [Test("public static (int, int) M1() {}")] public static (int, int) M1() => throw new NotImplementedException();
        [Test("public static (int x, int y) M2() {}")] public static (int x, int y) M2() => throw new NotImplementedException();
        [Test("public static (int, int, int) M3() {}")] public static (int, int, int) M3() => throw new NotImplementedException();
        [Test("public static (string, string) M4() {}")] public static (string, string) M4() => throw new NotImplementedException();
      }

      namespace Constructors {
        public class TypeInitializers {
          [Test("static TypeInitializers() {}")] static TypeInitializers() { }
        }

        public class Accessibilities {
          [Test("public Accessibilities(int p) {}")] public Accessibilities(int p) { throw new NotImplementedException(); }
          [Test("internal Accessibilities(short p) {}")] internal Accessibilities(short p) { throw new NotImplementedException(); }
          [Test("protected Accessibilities(byte p) {}")] protected Accessibilities(byte p) { throw new NotImplementedException(); }
          [Test("internal protected Accessibilities(uint p) {}")] protected internal Accessibilities(uint p) { throw new NotImplementedException(); }
          [Test("internal protected Accessibilities(ulong p) {}")] internal protected Accessibilities(ulong p) { throw new NotImplementedException(); }
          [Test("private protected Accessibilities(ushort p) {}")] private protected Accessibilities(ushort p) { throw new NotImplementedException(); }
          [Test("private protected Accessibilities(sbyte p) {}")] protected private Accessibilities(sbyte p) { throw new NotImplementedException(); }
          [Test("private Accessibilities(bool p) {}")] private Accessibilities(bool p) { throw new NotImplementedException(); }
        }
      }

      public class Destructors {
        [Test("~Destructors() {}")] ~Destructors() { throw new NotImplementedException(); }
      }

      namespace Operators {
        namespace UnaryOperators {
          public class C {
            [Test("public static C operator + (C c) {}", WithNamespace = false)]
            public static C operator + (C c) => throw new NotImplementedException();

            [Test("public static C operator - (C c) {}", WithNamespace = false)]
            public static C operator - (C c) => throw new NotImplementedException();

            [Test("public static C operator ! (C c) {}", WithNamespace = false)]
            public static C operator ! (C c) => throw new NotImplementedException();

            [Test("public static C operator ~ (C c) {}", WithNamespace = false)]
            public static C operator ~ (C c) => throw new NotImplementedException();

            [Test("public static bool operator true (C c) {}", WithNamespace = false)]
            public static bool operator true (C c) => throw new NotImplementedException();

            [Test("public static bool operator false (C c) {}", WithNamespace = false)]
            public static bool operator false (C c) => throw new NotImplementedException();

            [Test("public static C operator ++ (C c) {}", WithNamespace = false)]
            public static C operator ++ (C c) => throw new NotImplementedException();

            [Test("public static C operator -- (C c) {}", WithNamespace = false)]
            public static C operator -- (C c) => throw new NotImplementedException();
          }
        }

        namespace BinaryOperators {
          public class C {
            [Test("public static C operator + (C x, C y) {}", WithNamespace = false)]
            public static C operator + (C x, C y) => throw new NotImplementedException();

            [Test("public static C operator - (C x, C y) {}", WithNamespace = false)]
            public static C operator - (C x, C y) => throw new NotImplementedException();

            [Test("public static C operator * (C x, C y) {}", WithNamespace = false)]
            public static C operator * (C x, C y) => throw new NotImplementedException();

            [Test("public static C operator / (C x, C y) {}", WithNamespace = false)]
            public static C operator / (C x, C y) => throw new NotImplementedException();

            [Test("public static C operator % (C x, C y) {}", WithNamespace = false)]
            public static C operator % (C x, C y) => throw new NotImplementedException();

            [Test("public static C operator & (C x, C y) {}", WithNamespace = false)]
            public static C operator & (C x, C y) => throw new NotImplementedException();

            [Test("public static C operator | (C x, C y) {}", WithNamespace = false)]
            public static C operator | (C x, C y) => throw new NotImplementedException();

            [Test("public static C operator ^ (C x, C y) {}", WithNamespace = false)]
            public static C operator ^ (C x, C y) => throw new NotImplementedException();

            [Test("public static C operator >> (C x, int y) {}", WithNamespace = false)]
            public static C operator >> (C x, int y) => throw new NotImplementedException();

            [Test("public static C operator << (C x, int y) {}", WithNamespace = false)]
            public static C operator << (C x, int y) => throw new NotImplementedException();
          }
        }

        namespace Comparison {
          public class C : IEquatable<C>, IComparable<C> {
            [Test("public static bool operator == (C x, C y) {}", WithNamespace = false)]
            public static bool operator == (C x, C y) => throw new NotImplementedException();

            [Test("public static bool operator != (C x, C y) {}", WithNamespace = false)]
            public static bool operator != (C x, C y) => throw new NotImplementedException();

            [Test("public static bool operator < (C x, C y) {}", WithNamespace = false)]
            public static bool operator < (C x, C y) => throw new NotImplementedException();

            [Test("public static bool operator > (C x, C y) {}", WithNamespace = false)]
            public static bool operator > (C x, C y) => throw new NotImplementedException();

            [Test("public static bool operator <= (C x, C y) {}", WithNamespace = false)]
            public static bool operator <= (C x, C y) => throw new NotImplementedException();

            [Test("public static bool operator >= (C x, C y) {}", WithNamespace = false)]
            public static bool operator >= (C x, C y) => throw new NotImplementedException();

            public bool Equals(C other) => throw new NotImplementedException();
            public int CompareTo(C other) => throw new NotImplementedException();
            public override bool Equals(object other) => throw new NotImplementedException();
            public override int GetHashCode() => throw new NotImplementedException();
          }
        }

        namespace TypeCasts {
          public class V {}
          public class W {}

          public class C {
            [Test("public static explicit operator C(V v) {}", WithNamespace = false)]
            public static explicit operator C(V v) => throw new NotImplementedException();

            [Test("public static explicit operator V(C c) {}", WithNamespace = false)]
            public static explicit operator V(C c) => throw new NotImplementedException();


            [Test("public static implicit operator C(W w) {}", WithNamespace = false)]
            public static implicit operator C(W w) => throw new NotImplementedException();

            [Test("public static implicit operator W(C c) {}", WithNamespace = false)]
            public static implicit operator W(C c) => throw new NotImplementedException();
          }
        }
      }

      namespace GenericMethods {
        public class C<T> {
          [Test("public T M(T x) {}")] public T M(T x) => throw new NotImplementedException();
        }

        public class C<T1, T2> {
          [Test("public T1 M(T2 x) {}")] public T1 M(T2 x) => throw new NotImplementedException();
        }

        public class C {
          [Test("public T M<T>(T x) {}")] public T M<T>(T x) => throw new NotImplementedException();
          [Test("public T1 M<T1, T2>(T2 x) {}")] public T1 M<T1, T2>(T2 x) => throw new NotImplementedException();
        }

        public abstract class COpen<T1, T2> {
          [Test("public abstract void M(T1 t1, T2 t2);")] public abstract void M(T1 t1, T2 t2);
        }

        public class CClose1<T2> : COpen<int, T2> {
          [Test("public override void M(int t1, T2 t2) {}")] public override void M(int t1, T2 t2) {}
        }

        public class CClose2 : CClose1<int> {
          [Test("public override void M(int t1, int t2) {}")] public override void M(int t1, int t2) {}
        }

        public class Constraints1 {
          [Test("public T M1<T>(T x) where T : new() {}")]    public T M1<T>(T x) where T : new() => throw new NotImplementedException();
          [Test("public T M2<T>(T x) where T : struct {}")]   public T M2<T>(T x) where T : struct => throw new NotImplementedException();
          [Test("public T M3<T>(T x) where T : class {}")]    public T M3<T>(T x) where T : class => throw new NotImplementedException();
          [Test("public T M4<T>(T x) where T : System.IDisposable {}")]                      public T M4<T>(T x) where T : IDisposable => throw new NotImplementedException();
          [Test("public T M5<T>(T x) where T : System.ICloneable, System.IDisposable {}")]   public T M5<T>(T x) where T : IDisposable, ICloneable => throw new NotImplementedException();
          [Test("public T M6<T>(T x) where T : System.ICloneable, new() {}")]                public T M6<T>(T x) where T : ICloneable, new() => throw new NotImplementedException();
          [Test("public T M7<T>(T x) where T : class, new() {}")]                            public T M7<T>(T x) where T : class, new() => throw new NotImplementedException();
          [Test("public T M8<T>(T x) where T : class, System.ICloneable, new() {}")]                       public T M8<T>(T x) where T : class, ICloneable, new() => throw new NotImplementedException();
          [Test("public T M9<T>(T x) where T : class, System.ICloneable, System.IDisposable, new() {}")]   public T M9<T>(T x) where T : class, ICloneable, IDisposable, new() => throw new NotImplementedException();

          [Test("public T M10<T>(T x) where T : Constraints1.CBase {}", WithNamespace = false)]
          public T M10<T>(T x) where T : CBase => throw new NotImplementedException();
          public class CBase {}

          [Test("public T M11<T>(T x) where T : System.Enum {}")]                 public T M11<T>(T x) where T : System.Enum => throw new NotImplementedException();
          [Test("public T M12<T>(T x) where T : System.Delegate {}")]             public T M12<T>(T x) where T : System.Delegate => throw new NotImplementedException();
          [Test("public T M13<T>(T x) where T : System.MulticastDelegate {}")]    public T M13<T>(T x) where T : System.MulticastDelegate => throw new NotImplementedException();
        }

        class Constraints2 {
          [Test("public T1 M1<T1, T2>(T2 x) where T1 : new() where T2 : new() {}")]
          public T1 M1<T1, T2>(T2 x)
            where T1 : new()
            where T2 : new()
            => throw new NotImplementedException();

          [Test("public T1 M2<T1, T2>(T2 x) where T1 : new() where T2 : new() {}")]
          public T1 M2<T1, T2>(T2 x)
            where T2 : new()
            where T1 : new()
            => throw new NotImplementedException();

          [Test("public T1 M3<T1, T2>(T2 x) where T1 : System.ICloneable where T2 : System.IDisposable {}")]
          public T1 M3<T1, T2>(T2 x)
            where T1 : ICloneable
            where T2 : IDisposable
            => throw new NotImplementedException();

          [Test("public T1 M4<T1, T2>(T2 x) where T1 : class, System.ICloneable where T2 : System.IDisposable, new() {}")]
          public T1 M4<T1, T2>(T2 x)
            where T1 : class, ICloneable
            where T2 : IDisposable, new()
            => throw new NotImplementedException();
        }
      }
    }

    namespace Events {
      public class Modifiers {
        [Test("public event System.EventHandler E1;")] public event EventHandler E1;
        [Test("public static event System.EventHandler E2;")] public static event EventHandler E2;
        [Test("public static event System.EventHandler E3;")] static public event EventHandler E3;
      }

      public class Accessibilities {
        [Test("public event System.EventHandler E1;")] public event EventHandler E1;
        [Test("internal event System.EventHandler E2;")] internal event EventHandler E2;
        [Test("protected event System.EventHandler E3;")] protected event EventHandler E3;
        [Test("internal protected event System.EventHandler E4;")] protected internal event EventHandler E4;
        [Test("internal protected event System.EventHandler E5;")] internal protected event EventHandler E5;
        [Test("private protected System.EventHandler E6;")] private protected EventHandler E6;
        [Test("private protected System.EventHandler E7;")] protected private EventHandler E7;
        [Test("private event System.EventHandler E8;")] private event EventHandler E8;
      }
    }

    public interface InterfaceMembers {
      [Test("int P1 { get; set; }")] int P1 { get; set; }
      [Test("int P2 { get; }")] int P2 { get; }
      [Test("int P3 { set; }")] int P3 { set; }
      [Test("event System.EventHandler E;")] event EventHandler E;
      [Test("void M();")] void M();
      [Test("int M(int x);")] int M(int x);
    }

    namespace ImplementedInterfaceMembers {
      class ImplicitMethod : IDisposable, ICloneable {
        [Test("public void Dispose() {}")] public void Dispose() {}

        [Test("public object Clone() {}", WithNamespace = false)]
        public object Clone() => throw new NotImplementedException();
      }

      class ExplicitMethod : IDisposable, ICloneable {
        [Test("void System.IDisposable.Dispose() {}")] void IDisposable.Dispose() {}

        [Test("object ICloneable.Clone() {}", WithNamespace = false)]
        object ICloneable.Clone() => throw new NotImplementedException();
      }

      interface IProperty {
        int P1 { get; set; }
        int P2 { get; }
        int P3 { set; }
      }

      class ImplicitProperty1 : IProperty {
        [Test("public int P1 { get; set; }", WithNamespace = false)]
        public int P1 { get; set; }

        [Test("public int P2 { get; }", WithNamespace = false)]
        public int P2 { get { throw new NotImplementedException(); } }

        [Test("public int P3 { set; }", WithNamespace = false)]
        public int P3 { set { throw new NotImplementedException(); } }
      }

      class ExplicitProperty1 : IProperty {
        [Test("int IProperty.P1 { get; set; }", WithNamespace = false)]
        int IProperty.P1 { get; set; }

        [Test("int IProperty.P2 { get; }", WithNamespace = false)]
        int IProperty.P2 { get { throw new NotImplementedException(); } }

        [Test("int IProperty.P3 { set; }", WithNamespace = false)]
        int IProperty.P3 { set { throw new NotImplementedException(); } }
      }

      class ImplicitProperty2 : IAsyncResult {
        [Test("public object AsyncState { get; }")]
        public object AsyncState { get => throw new NotImplementedException(); }

        [Test("public System.Threading.WaitHandle AsyncWaitHandle { get; }")]
        public System.Threading.WaitHandle AsyncWaitHandle { get => throw new NotImplementedException(); }

        [Test("public bool CompletedSynchronously { get; }")]
        public bool CompletedSynchronously { get => throw new NotImplementedException(); }

        [Test("public bool IsCompleted { get; }")]
        public bool IsCompleted { get => throw new NotImplementedException(); }
      }

      class ExplicitProperty2 : IAsyncResult {
        [Test("object System.IAsyncResult.AsyncState { get; }")]
        object IAsyncResult.AsyncState { get => throw new NotImplementedException(); }

        [Test("System.Threading.WaitHandle System.IAsyncResult.AsyncWaitHandle { get; }")]
        System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle { get => throw new NotImplementedException(); }

        [Test("bool System.IAsyncResult.CompletedSynchronously { get; }")]
        bool IAsyncResult.CompletedSynchronously { get => throw new NotImplementedException(); }

        [Test("bool System.IAsyncResult.IsCompleted { get; }")]
        bool IAsyncResult.IsCompleted { get => throw new NotImplementedException(); }
      }

      class ExplicitProperty2WithoutNamespace : IAsyncResult {
        [Test("object IAsyncResult.AsyncState { get; }", WithNamespace = false)]
        object IAsyncResult.AsyncState { get => throw new NotImplementedException(); }

        [Test("WaitHandle IAsyncResult.AsyncWaitHandle { get; }", WithNamespace = false)]
        System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle { get => throw new NotImplementedException(); }

        [Test("bool IAsyncResult.CompletedSynchronously { get; }", WithNamespace = false)]
        bool IAsyncResult.CompletedSynchronously { get => throw new NotImplementedException(); }

        [Test("bool IAsyncResult.IsCompleted { get; }", WithNamespace = false)]
        bool IAsyncResult.IsCompleted { get => throw new NotImplementedException(); }
      }

      interface IEvent {
        event EventHandler E;
      }

      class ImplicitEvent : IEvent {
        [Test("public event EventHandler E;", WithNamespace = false)]
        public event EventHandler E;
      }

      class ExplicitEvent : IEvent {
        [Test("event EventHandler IEvent.E;", WithNamespace = false)]
        event EventHandler IEvent.E {
          add { throw new NotImplementedException(); }
          remove { throw new NotImplementedException(); }
        }
      }
    }

    namespace ParameterLists {
      public class Standard {
        [Test("public void M() {}")] public void M() {}
        [Test("public void M(int x) {}")] public void M(int x) {}
        [Test("public void M(int x, int y) {}")] public void M(int x, int y) {}
      }

      public class InOutRef {
        [Test("public void M1(in int x) {}")]  public void M1(in int x) {}
        [Test("public void M2(out int x) {}")] public void M2(out int x) => throw new NotImplementedException();
        [Test("public void M3(ref int x) {}")] public void M3(ref int x) => throw new NotImplementedException();
      }

      public class Params {
        [Test("public void M(params int[] p) {}")] public void M(params int[] p) {}
        [Test("public void M(int p0, params int[] p) {}")] public void M(int p0, params int[] p) {}
      }

      namespace ExtensionMethods {
        public class C {}

        public static class Ex {
          [Test("public static void M(this C c) {}", WithNamespace = false)]
          public static void M(this C c) {}

          [Test("public static void M(this C c, int i) {}", WithNamespace = false)]
          public static void M(this C c, int i) {}
        }
      }

      public class Tuples {
        [Test("public void M1((int, int) arg) {}")] public void M1((int, int) arg) {}
        [Test("public void M2((int x, int y) arg) {}")] public void M2((int x, int y) arg) {}
        [Test("public void M3((int, int, int) arg) {}")] public void M3((int, int, int) arg) {}
        [Test("public void M4((string, string) arg) {}")] public void M4((string, string) arg) {}
      }

      namespace Optionals {
        public class Primitives1 {
          [Test("public void M1(int x = 0) {}")] public void M1(int x = 0) {}
          [Test("public void M2(int x = 0) {}")] public void M2(int x = default(int)) {}
          [Test("public void M3(int x = 1) {}")] public void M3(int x = 1) {}
          [Test("public void M4(int x = int.MaxValue) {}")] public void M4(int x = int.MaxValue) {}
          [Test("public void M5(int x = int.MinValue) {}")] public void M5(int x = int.MinValue) {}
        }

        public class Primitives2 {
          [Test("public void M1(int x, int y = 0) {}")] public void M1(int x, int y = 0) {}
          [Test("public void M2(int x, int y = 0) {}")] public void M2(int x, int y = default(int)) {}
          [Test("public void M3(int x, int y = 1) {}")] public void M3(int x, int y = 1) {}
          [Test("public void M4(int x, int y = int.MaxValue) {}")] public void M4(int x, int y = int.MaxValue) {}
          [Test("public void M5(int x, int y = int.MinValue) {}")] public void M5(int x, int y = int.MinValue) {}
        }

        public class Booleans {
          [Test("public void M1(bool x = true) {}")] public void M1(bool x = true) {}
          [Test("public void M2(bool x = false) {}")] public void M2(bool x = false) {}
          [Test("public void M3(bool x = false) {}")] public void M3(bool x = default(bool)) {}
        }

        public class Strings {
          [Test("public void M1(string x = null) {}")] public void M1(string x = null) {}
          [Test("public void M2(string x = \"\") {}")] public void M2(string x = "") {}
          [Test("public void M3(string x = \"str\") {}")] public void M3(string x = "str") {}
          [Test("public void M4(string x = \"\\u0000\") {}")] public void M4(string x = "\0") {}
        }

        public class Enums {
          [Test("public void M11(System.DateTimeKind x = System.DateTimeKind.Local) {}")] public void M11(DateTimeKind x = DateTimeKind.Local) {}
          [Test("public void M12(System.DateTimeKind x = System.DateTimeKind.Unspecified) {}")] public void M12(DateTimeKind x = DateTimeKind.Unspecified) {}
          [Test("public void M13(System.DateTimeKind x = System.DateTimeKind.Unspecified) {}")] public void M13(DateTimeKind x = default(DateTimeKind)) {}
          [Test("public void M14(System.DateTimeKind x = (System.DateTimeKind)42) {}")] public void M14(DateTimeKind x = (DateTimeKind)42) {}

          [Test("public void M21(System.AttributeTargets x = System.AttributeTargets.All) {}")] public void M21(AttributeTargets x = AttributeTargets.All) {}
          [Test("public void M22(System.AttributeTargets x = System.AttributeTargets.Assembly) {}")] public void M22(AttributeTargets x = AttributeTargets.Assembly) {}
          [Test("public void M23(System.AttributeTargets x = default(System.AttributeTargets)) {}")] public void M23(AttributeTargets x = default(AttributeTargets)) {}
          [Test("public void M24(System.AttributeTargets x = (System.AttributeTargets)42) {}")] public void M24(AttributeTargets x = (System.AttributeTargets)42) {}
          [Test("public void M25(System.AttributeTargets x = (System.AttributeTargets)3) {}")] public void M25(AttributeTargets x = AttributeTargets.Assembly | AttributeTargets.Module) {}
        }

        public class ValueTypes {
          [Test("public void M1(System.DateTimeOffset x = default(System.DateTimeOffset)) {}")]
          public void M1(DateTimeOffset x = default(DateTimeOffset)) {}

          [Test("public void M2(System.Guid x = default(System.Guid)) {}")]
          public void M2(Guid x = default(Guid)) {}

          [Test("public void M3(System.Threading.CancellationToken x = default(System.Threading.CancellationToken)) {}")]
          public void M3(CancellationToken x = default(CancellationToken)) {}
        }
      }
    }
  }
}

