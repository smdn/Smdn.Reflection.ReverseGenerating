using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
  class TypeDeclarationTestCaseAttribute : GeneratorTestCaseAttribute {
    public Type TestTargetType { get; }

    public TypeDeclarationTestCaseAttribute(
      string expected,
      [CallerFilePath] string sourceFilePath = null,
      [CallerLineNumber] int lineNumber = 0
    )
      : this(
        expected,
        testTargetType: null,
        sourceFilePath,
        lineNumber
      )
    {
    }

    public TypeDeclarationTestCaseAttribute(
      string expected,
      Type testTargetType,
      [CallerFilePath] string sourceFilePath = null,
      [CallerLineNumber] int lineNumber = 0
    )
      : base(
        expected,
        sourceFilePath,
        lineNumber
      )
    {
      this.TestTargetType = testTargetType;
    }
  }

  namespace TestCases {
    namespace TypeDeclaration {
      namespace Options {
        namespace TypeDeclarationWithAccessibility {
          [TypeDeclarationTestCase("public class C1", TypeWithAccessibility = true, TypeWithNamespace = false)]
          [TypeDeclarationTestCase("class C1", TypeWithAccessibility = false, TypeWithNamespace = false)]
          public class C1 { }

          [TypeDeclarationTestCase("internal class C2", TypeWithAccessibility = true, TypeWithNamespace = false)]
          [TypeDeclarationTestCase("class C2", TypeWithAccessibility = false, TypeWithNamespace = false)]
          class C2 { }

          [TypeDeclarationTestCase("delegate void D();", TypeWithAccessibility = false, TypeWithNamespace = false)]
          [TypeDeclarationTestCase("delegate void D()", TypeWithAccessibility = false, TypeWithNamespace = false, TypeOmitEndOfStatement = true)]
          delegate void D();

          [TypeDeclarationTestCase("enum E : int", TypeWithAccessibility = false, TypeWithNamespace = false)] enum E { }
          [TypeDeclarationTestCase("interface I", TypeWithAccessibility = false, TypeWithNamespace = false)] interface I { }
          [TypeDeclarationTestCase("struct S", TypeWithAccessibility = false, TypeWithNamespace = false)] struct S { }
        }

        namespace TranslateLanguagePrimitiveTypeDeclaration {
          [TypeDeclarationTestCase("public readonly struct int", typeof(int), TranslateLanguagePrimitiveTypeDeclaration = true)]
          [TypeDeclarationTestCase("public readonly struct Int32", typeof(int), TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = true)]
          [TypeDeclarationTestCase("public readonly struct Int32", typeof(int), TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]
          [TypeDeclarationTestCase("public sealed class string", typeof(string), TranslateLanguagePrimitiveTypeDeclaration = true)]
          [TypeDeclarationTestCase("public sealed class String", typeof(string), TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = true)]
          [TypeDeclarationTestCase("public sealed class String", typeof(string), TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]
          [TypeDeclarationTestCase("public struct Guid", typeof(Guid), TranslateLanguagePrimitiveTypeDeclaration = true)]
          [TypeDeclarationTestCase("public struct Guid", typeof(Guid), TranslateLanguagePrimitiveTypeDeclaration = false)]
          class Placeholder {}
        }
      }

      namespace Delegates {
        [TypeDeclarationTestCase("internal delegate void D0();")] internal delegate void D0();
        [TypeDeclarationTestCase("public delegate void D1();")] public delegate void D1();
        [TypeDeclarationTestCase("public delegate void D2(int x);")] public delegate void D2(int x);
        [TypeDeclarationTestCase("public delegate int D3();")] public delegate int D3();
        [TypeDeclarationTestCase("public delegate int D4(int x);")] public delegate int D4(int x);
        [TypeDeclarationTestCase("public delegate int D5(int x, int y);")] public delegate int D5(int x, int y);

        [TypeDeclarationTestCase("public delegate Guid D61();", TypeWithNamespace = false)] public delegate Guid D61();
        [TypeDeclarationTestCase("public delegate System.Guid D62();", TypeWithNamespace = true)] public delegate Guid D62();

        class Accessibilities {
          [TypeDeclarationTestCase("public delegate void D1();")] public delegate void D1();
          [TypeDeclarationTestCase("internal delegate void D2();")] internal delegate void D2();
          [TypeDeclarationTestCase("protected delegate void D3();")] protected delegate void D3();
          [TypeDeclarationTestCase("internal protected delegate void D4();")] protected internal delegate void D4();
          [TypeDeclarationTestCase("internal protected delegate void D5();")] internal protected delegate void D5();
          [TypeDeclarationTestCase("private protected delegate void D6();")] private protected delegate void D6();
          [TypeDeclarationTestCase("private protected delegate void D7();")] protected private delegate void D7();
          [TypeDeclarationTestCase("private delegate void D8();")] private delegate void D8();
        }

#if false
        class DelegateClasses {
          [TypeDeclarationTestCase("public delegate void D1();")] public delegate void D1();
          [TypeDeclarationTestCase("public class D2")] public class D2 : MulticastDelegate { }
          [TypeDeclarationTestCase("public class D3")] public class D3 : Delegate {}
        }
#endif
      }

      namespace Enums {
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
      }

      namespace Interfaces {
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

      namespace Structures {
        [TypeDeclarationTestCase("internal struct S0")] internal struct S0 { };
        [TypeDeclarationTestCase("public struct S1")] public struct S1 { };

        class Accessibilities {
          [TypeDeclarationTestCase("public struct S1")] public struct S1 { }
          [TypeDeclarationTestCase("internal struct S2")] internal struct S2 { }
          [TypeDeclarationTestCase("protected struct S3")] protected struct S3 { }
          [TypeDeclarationTestCase("internal protected struct S4")] protected internal struct S4 { }
          [TypeDeclarationTestCase("internal protected struct S5")] internal protected struct S5 { }
          [TypeDeclarationTestCase("private protected struct S6")] private protected struct S6 { }
          [TypeDeclarationTestCase("private protected struct S7")] protected private struct S7 { }
          [TypeDeclarationTestCase("private struct S8")] private struct S8 { }
        }

        class Modifiers {
          [TypeDeclarationTestCase("public ref struct S0")] public ref struct S0 { }
          [TypeDeclarationTestCase("public readonly struct S1")] public readonly struct S1 { }
          [TypeDeclarationTestCase("public readonly ref struct S2")] public readonly ref struct S2 { }
        }
      }

      namespace Classes {
        [TypeDeclarationTestCase("internal class C0")] internal class C0 { };
        [TypeDeclarationTestCase("public class C1")] public class C1 { };

        class Accessibilities {
          [TypeDeclarationTestCase("public class C1")] public class C1 { }
          [TypeDeclarationTestCase("internal class C2")] internal class C2 { }
          [TypeDeclarationTestCase("protected class C3")] protected class C3 { }
          [TypeDeclarationTestCase("internal protected class C4")] protected internal class C4 { }
          [TypeDeclarationTestCase("internal protected class C5")] internal protected class C5 { }
          [TypeDeclarationTestCase("private protected class C6")] private protected class C6 { }
          [TypeDeclarationTestCase("private protected class C7")] protected private class C7 { }
          [TypeDeclarationTestCase("private class C8")] private class C8 { }
        }

        namespace Modifiers {
          [TypeDeclarationTestCase("public abstract class C1")] public abstract class C1 { }
          [TypeDeclarationTestCase("public static class C2")] public static class C2 { }
          [TypeDeclarationTestCase("public sealed class C3")] public sealed class C3 { }

          namespace NewModifier {
            class C {
              class C0 {
                protected class CX {
                }
              }
              class C1 : C0 {
#if false // TODO
                [TypeDeclarationTestCase("new protected class CX")] new protected class CX { }
#endif
              }
            }
          }
        }
      }

      namespace GenericTypes {
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

        namespace Constraints1 {
          [TypeDeclarationTestCase("public class C1<T> where T : new()")] public class C1<T> where T : new() { }
          [TypeDeclarationTestCase("public class C2<T> where T : struct")] public class C2<T> where T : struct { }
          [TypeDeclarationTestCase("public class C3<T> where T : class")] public class C3<T> where T : class { }
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

          [TypeDeclarationTestCase("public struct S1<T> where T : new()")] public struct S1<T> where T : new() { }

          [TypeDeclarationTestCase("public interface I1<T> where T : new()")] public interface I1<T> where T : new() { }

          [TypeDeclarationTestCase("public delegate void D1<T>() where T : new();")] public delegate void D1<T>() where T : new();
        }

        namespace Constraints2 {
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
    }
  }

  partial class GeneratorTests {
    [Test]
    public void TestGenerateTypeDeclaration()
    {
      foreach (var type in FindTypes(t => t.FullName.Contains(".TestCases.TypeDeclaration."))) {
        foreach (var attr in type.GetCustomAttributes<TypeDeclarationTestCaseAttribute>()) {
          Assert.AreEqual(
            attr.Expected,
            Generator.GenerateTypeDeclaration(attr.TestTargetType ?? type, null, GetGeneratorOptions(attr)),
            message: $"{attr.SourceLocation} ({type.FullName})"
          );
        }
      }
    }
  }
}
