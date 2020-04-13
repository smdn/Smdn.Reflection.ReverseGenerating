using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
  class MemberDeclarationTestCaseAttribute : GeneratorTestCaseAttribute {
    public MemberDeclarationTestCaseAttribute(
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
    namespace MemberDeclaration {
      namespace MemberOfNestedGenericTypes {
        class C<T> {
          public class CNest { }
          public class CNest<TN> { }

          [MemberDeclarationTestCase("public C<T>.CNest F1;", MemberWithNamespace = false)]
          [MemberDeclarationTestCase("public C<T>.CNest C<T>.F1;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
          public CNest F1 = null;

          [MemberDeclarationTestCase("public C<T>.CNest<T> F2;", MemberWithNamespace = false)]
          [MemberDeclarationTestCase("public C<T>.CNest<T> C<T>.F2;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
          public CNest<T> F2 = null;

          [MemberDeclarationTestCase("public C<T>.CNest<int> F3;", MemberWithNamespace = false)]
          [MemberDeclarationTestCase("public C<T>.CNest<int> C<T>.F3;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
          public CNest<int> F3 = null;
        }
      }

      namespace Fields {
        public class Types {
          [MemberDeclarationTestCase("public int F1;")]
          [MemberDeclarationTestCase("public int F1", MemberOmitEndOfStatement = true)]
          [MemberDeclarationTestCase("public int Types.F1;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
          [MemberDeclarationTestCase("public int Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Fields.Types.F1;", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
          public int F1;

          [MemberDeclarationTestCase("public float F2;")] public float F2;
          [MemberDeclarationTestCase("public string F3;")] public string F3;
          [MemberDeclarationTestCase("public System.Guid F4;")] public Guid F4;
        }

        namespace EnumFields {
          public enum Ints : int {
            [MemberDeclarationTestCase("A = 0,")]
            [MemberDeclarationTestCase("A = 0", MemberOmitEndOfStatement = true)]
            [MemberDeclarationTestCase("Ints.A = 0,", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
            [MemberDeclarationTestCase("Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Fields.EnumFields.Ints.A = 0,", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
            A = 0,

            [MemberDeclarationTestCase("B = 1,")] B = 1,
            [MemberDeclarationTestCase("C = 2,")] C = 2,
          }

          public enum Bytes : byte {
            [MemberDeclarationTestCase("A = 0,")] A = 0,
            [MemberDeclarationTestCase("B = 1,")] B = 1,
            [MemberDeclarationTestCase("C = 2,")] C = 2,
          }

          namespace Flags {
            [Flags()]
            public enum Ints : int {
              [MemberDeclarationTestCase("A = 0x00000000,")] A = 0x00000000,
              [MemberDeclarationTestCase("B = 0x00000001,")] B = 0x00000001,
              [MemberDeclarationTestCase("C = 0x00000010,")] C = 0x00000010,
              [MemberDeclarationTestCase("Z = 0x7fffffff,")] Z = 0x7fffffff,
            }

            [Flags()]
            public enum Shorts : short {
              [MemberDeclarationTestCase("A = 0x0000,")] A = 0x0000,
              [MemberDeclarationTestCase("B = 0x0001,")] B = 0x0001,
              [MemberDeclarationTestCase("C = 0x0010,")] C = 0x0010,
              [MemberDeclarationTestCase("Z = 0x7fff,")] Z = 0x7fff,
            }

            [Flags()]
            public enum Bytes : byte {
              [MemberDeclarationTestCase("A = 0x00,")] A = 0x00,
              [MemberDeclarationTestCase("B = 0x01,")] B = 0x01,
              [MemberDeclarationTestCase("C = 0x10,")] C = 0x10,
              [MemberDeclarationTestCase("Z = 0xff,")] Z = 0xff,
            }
          }
        }

#pragma warning disable CS0649, CS0169
        public class Accessibilities {
          [MemberDeclarationTestCase("public int F1;")] public int F1;
          [MemberDeclarationTestCase("internal int F2;")] internal int F2;
          [MemberDeclarationTestCase("protected int F3;")] protected int F3;
          [MemberDeclarationTestCase("internal protected int F4;")] protected internal int F4;
          [MemberDeclarationTestCase("internal protected int F5;")] internal protected int F5;
          [MemberDeclarationTestCase("private protected int F6;")] private protected int F6;
          [MemberDeclarationTestCase("private protected int F7;")] protected private int F7;
          [MemberDeclarationTestCase("private int F8;")] private int F8;

          [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] internal int F9;
          [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private protected int F10;
          [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private int F11;
        }
#pragma warning restore CS0067, CS0169

        public class Modifiers {
          [MemberDeclarationTestCase("public int F4;")] public int F4;
          [MemberDeclarationTestCase("public readonly int F5;")] public readonly int F5;
          [MemberDeclarationTestCase("public const int F6 = 123;")] public const int F6 = 123;
          [MemberDeclarationTestCase("public static int F7;")] public static int F7;
          [MemberDeclarationTestCase("public static readonly int F8 = 0;")]  public static readonly int F8;
          [MemberDeclarationTestCase("public static readonly int F9 = 0;")]  static readonly public int F9;
          [MemberDeclarationTestCase("public static readonly int F10 = 0;")] readonly public static int F10;
        }

        namespace StaticValues {
          public class ValueTypes {
            [MemberDeclarationTestCase("public int F1;")] public int F1 = 1;
            [MemberDeclarationTestCase("public readonly int F2;")] public readonly int F2 = 2;

            [MemberDeclarationTestCase("public const int F3 = 3;")]
            [MemberDeclarationTestCase("public const int F3 = 3", MemberOmitEndOfStatement = true)]
            public const int F3 = 3;

            [MemberDeclarationTestCase("public static int F4;")] public static int F4 = 4;
            [MemberDeclarationTestCase("public static readonly int F5 = 5;")] public static readonly int F5 = 5;
            [MemberDeclarationTestCase("public static readonly int F6 = int.MaxValue;")] public static readonly int F6 = int.MaxValue;
            [MemberDeclarationTestCase("public static readonly int F7 = int.MinValue;")] public static readonly int F7 = int.MinValue;
          }

          public class Enums {
            [MemberDeclarationTestCase("public const System.DateTimeKind F1 = System.DateTimeKind.Unspecified;")] public const DateTimeKind F1 = DateTimeKind.Unspecified;
            [MemberDeclarationTestCase("public const System.DateTimeKind F2 = System.DateTimeKind.Unspecified;")] public const DateTimeKind F2 = default(DateTimeKind);
            [MemberDeclarationTestCase("public const System.DateTimeKind F3 = System.DateTimeKind.Local;")] public const DateTimeKind F3 = DateTimeKind.Local;
            [MemberDeclarationTestCase("public const System.DateTimeKind F4 = (System.DateTimeKind)42;")] public const DateTimeKind F4 =(DateTimeKind)42;
          }

          public class EnumFlags {
            [MemberDeclarationTestCase("public const System.AttributeTargets F1 = System.AttributeTargets.All;")] public const AttributeTargets F1 = AttributeTargets.All;
            [MemberDeclarationTestCase("public const System.AttributeTargets F2 = default(System.AttributeTargets);")] public const AttributeTargets F2 = default(AttributeTargets);
            [MemberDeclarationTestCase("public const System.AttributeTargets F22 = default;", UseDefaultLiteral = true)] public const AttributeTargets F22 = default(AttributeTargets);
            [MemberDeclarationTestCase("public const System.AttributeTargets F3 = System.AttributeTargets.Assembly;")] public const AttributeTargets F3 = AttributeTargets.Assembly;
            [MemberDeclarationTestCase("public const System.AttributeTargets F4 = (System.AttributeTargets)3;")] public const AttributeTargets F4 = AttributeTargets.Assembly | AttributeTargets.Module;
            [MemberDeclarationTestCase("public const System.AttributeTargets F5 = (System.AttributeTargets)42;")] public const AttributeTargets F5 =(AttributeTargets)42;
          }

          public class Booleans {
            [MemberDeclarationTestCase("public const bool F1 = false;")] public const bool F1 = false;
            [MemberDeclarationTestCase("public const bool F2 = true;")] public const bool F2 = true;
          }

          public class Chars {
            [MemberDeclarationTestCase("public const char F1 = 'A';")] public const char F1 = 'A';
            [MemberDeclarationTestCase("public const char F2 = '\\u0000';")] public const char F2 = '\0';
            [MemberDeclarationTestCase("public const char F3 = '\\u000A';")] public const char F3 = '\n';
            [MemberDeclarationTestCase("public const char F4 = '\\u000A';")] public const char F4 = '\u000A';
            [MemberDeclarationTestCase("public const char F5 = '\\'';")] public const char F5 = '\'';
            [MemberDeclarationTestCase("public const char F6 = '\"';")] public const char F6 = '"';
          }

          public class Strings {
            [MemberDeclarationTestCase("public const string F1 = \"string\";")] public const string F1 = "string";
            [MemberDeclarationTestCase("public const string F2 = \"\";")] public const string F2 = "";
            [MemberDeclarationTestCase("public const string F3 = null;")] public const string F3 = null;
            [MemberDeclarationTestCase("public const string F4 = \"\\u0000\";")] public const string F4 = "\0";
            [MemberDeclarationTestCase("public const string F5 = \"hello\\u000A\";")] public const string F5 = "hello\n";
            [MemberDeclarationTestCase("public const string F6 = \"\\\"\";")] public const string F6 = "\"";
          }

          public class Nullables {
            [MemberDeclarationTestCase("public static readonly int? F1 = 0;")] public static readonly int? F1 = 0;
            [MemberDeclarationTestCase("public static readonly int? F2 = null;")] public static readonly int? F2 = null;
            [MemberDeclarationTestCase("public static readonly int? F3 = null;")] public static readonly int? F3 = default(int?);
            [MemberDeclarationTestCase("public static readonly int? F4 = int.MaxValue;")] public static readonly int? F4 = int.MaxValue;
            [MemberDeclarationTestCase("public static readonly int? F5 = int.MinValue;")] public static readonly int? F5 = int.MinValue;

            [MemberDeclarationTestCase("public static readonly System.DateTimeOffset? F6 = null;")] public static readonly DateTimeOffset? F6 = null;
            [MemberDeclarationTestCase("public static readonly System.DateTimeOffset? F7 = System.DateTimeOffset.MinValue;")] public static readonly DateTimeOffset? F7 = DateTimeOffset.MinValue;
            [MemberDeclarationTestCase("public static readonly System.DateTimeOffset? F8 = System.DateTimeOffset.MaxValue;")] public static readonly DateTimeOffset? F8 = DateTimeOffset.MaxValue;
          }

          public class NonPrimitiveValueTypes {
            [MemberDeclarationTestCase("public static readonly System.Guid F1 = System.Guid.Empty;")] public static readonly Guid F1 = Guid.Empty;
            [MemberDeclarationTestCase("public static readonly System.Guid F2 = System.Guid.Empty;")] public static readonly Guid F2 = default(Guid);

            [MemberDeclarationTestCase("public static readonly System.DateTimeOffset F3 = System.DateTimeOffset.MinValue;")] public static readonly DateTimeOffset F3 = default(DateTimeOffset);
            [MemberDeclarationTestCase("public static readonly System.DateTimeOffset F4 = System.DateTimeOffset.MinValue;")] public static readonly DateTimeOffset F4 = DateTimeOffset.MinValue;
            [MemberDeclarationTestCase("public static readonly System.DateTimeOffset F5 = System.DateTimeOffset.MaxValue;")] public static readonly DateTimeOffset F5 = DateTimeOffset.MaxValue;
            [MemberDeclarationTestCase("public static readonly System.DateTimeOffset F6; // = \"2018/10/31 21:00:00 +09:00\"")] public static readonly DateTimeOffset F6 = new DateTimeOffset(2018, 10, 31, 21, 0, 0, 0, TimeSpan.FromHours(+9.0));

            // XXX: System.Threading.CancellationToken.None is a property
            [MemberDeclarationTestCase("public static readonly System.Threading.CancellationToken F7 = default(System.Threading.CancellationToken);")] public static readonly CancellationToken F7 = CancellationToken.None;
            [MemberDeclarationTestCase("public static readonly System.Threading.CancellationToken F8 = default(System.Threading.CancellationToken);")] public static readonly CancellationToken F8 = default(CancellationToken);
            [MemberDeclarationTestCase("public static readonly System.Threading.CancellationToken F9 = default;", UseDefaultLiteral = true)] public static readonly CancellationToken F9 = default(CancellationToken);
          }

          namespace NonPrimitiveValueTypes_FieldOfDeclaringType {
            public struct S1 {
              [MemberDeclarationTestCase("public static readonly S1 Empty; // = \"foo\"", MemberWithNamespace = false)]
              [MemberDeclarationTestCase("public static readonly S1 Empty; // = \"foo\"", MemberWithNamespace = false, MemberOmitEndOfStatement = true)]
              public static readonly S1 Empty = default(S1);

              public override string ToString() => "foo";
            }

            public struct S2 {
              [MemberDeclarationTestCase("public static readonly S2 Empty; // = \"\\\"\\u0000\\\"\"", MemberWithNamespace = false)]
              public static readonly S2 Empty = default(S2);

              public override string ToString() => "\"\0\"";
            }
          }

          public class ReferenceTypes {
            [MemberDeclarationTestCase("public static readonly System.Uri F1 = null;")]
            [MemberDeclarationTestCase("public static readonly System.Uri F1 = null", MemberOmitEndOfStatement = true)]
            public static readonly Uri F1 = null;

            [MemberDeclarationTestCase("public static readonly System.Uri F2; // = \"http://example.com/\"")]
            [MemberDeclarationTestCase("public static readonly System.Uri F2; // = \"http://example.com/\"", MemberOmitEndOfStatement = true)]
            public static readonly Uri F2 = new Uri("http://example.com/");

            [MemberDeclarationTestCase("public static readonly System.Collections.Generic.IEnumerable<int> F3 = null;")] public static readonly IEnumerable<int> F3 = null;
            [MemberDeclarationTestCase("public static readonly System.Collections.Generic.IEnumerable<int> F4; // = \"System.Int32[]\"")] public static readonly IEnumerable<int> F4 = new int[] {0, 1, 2, 3};
          }
        }
      }

      namespace Properties {
        namespace Options {
          public abstract class Abstract {
            [MemberDeclarationTestCase("public abstract int P { get; set; }", MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public abstract int P { get; set; }", MethodBody = MethodBodyOption.EmptyImplementation)]
            [MemberDeclarationTestCase("public abstract int P { get; set; }", MethodBody = MethodBodyOption.ThrowNotImplementedException)]
            [MemberDeclarationTestCase("public abstract int Abstract.P { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public abstract int Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Properties.Options.Abstract.P { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            public abstract int P { get; set; }
          }

          public abstract class NonAbstract {
            [MemberDeclarationTestCase("public int P { get; set; }", MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public int P { get; set; }", MethodBody = MethodBodyOption.EmptyImplementation)]
            [MemberDeclarationTestCase("public int P { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }", MethodBody = MethodBodyOption.ThrowNotImplementedException)]
            [MemberDeclarationTestCase("public int NonAbstract.P { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public int Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Properties.Options.NonAbstract.P { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            public int P { get; set; }
          }
        }

        public class Accessors {
          [MemberDeclarationTestCase("public int P1 { get; set; }")] public int P1 { get; set; }
          [MemberDeclarationTestCase("public int P2 { get; }")] public int P2 { get; }
          [MemberDeclarationTestCase("public int P3 { set; }")] public int P3 { set { int x = value; } }
        }

        public class Modifiers1 {
          [MemberDeclarationTestCase("public static int P1 { get; set; }")] public static int P1 { get; set; }
        }

        public abstract class Modifiers_Abstract {
          [MemberDeclarationTestCase("public abstract int P1 { get; set; }")] public abstract int P1 { get; set; }
          [MemberDeclarationTestCase("public virtual int P2 { get; set; }")] public virtual int P2 { get; set; }
        }

        public abstract class Modifiers_Override : Modifiers_Abstract {
          [MemberDeclarationTestCase("public override int P2 { get; set; }")] public override int P2 { get; set; }
        }

        public abstract class Modifiers_New : Modifiers_Abstract {
          [MemberDeclarationTestCase("public int P2 { get; set; }")] public new int P2 { get; set; }
        }

        public abstract class Modifiers_NewVirtual : Modifiers_Abstract {
          [MemberDeclarationTestCase("public virtual int P2 { get; set; }")] public new virtual int P2 { get; set; }
        }

        public class Indexers1 {
          [IndexerName("Indexer")]
          [MemberDeclarationTestCase("public int this[int x] { get; set; }")]
          [MemberDeclarationTestCase("public int Indexers1.Indexer[int x] { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
          [MemberDeclarationTestCase("public int Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Properties.Indexers1.Indexer[int x] { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
          public int this[int x] { get { return 0; } set { int val = value; } }
        }

        public class Indexers2 {
          [IndexerName("Indexer")]
          [MemberDeclarationTestCase("public int this[int x, int y] { get; set; }")] public int this[int x, int y] { get { return 0; } set { int val = value; } }
        }

        public class Indexers3 {
          [IndexerName("Indexer")]
          [MemberDeclarationTestCase("public int this[int @in] { get; set; }")] public int this[int @in] { get { return 0; } set { int val = value; } }
        }

        public class Indexers4 {
          [IndexerName("Indexer")]
          [MemberDeclarationTestCase("public int this[string x] { get; set; }")] public int this[string x] { get { return 0; } set { int val = value; } }
        }

        public class IndexerWithoutIndexerNameAttribute {
          [MemberDeclarationTestCase("public int this[int x] { get; set; }")]
          [MemberDeclarationTestCase("public int IndexerWithoutIndexerNameAttribute.Item[int x] { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
          [MemberDeclarationTestCase("public int Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Properties.IndexerWithoutIndexerNameAttribute.Item[int x] { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
          public int this[int x] { get { return 0; } set { int val = value; } }
        }

        public class AccessibilitiesOfProperty {
          [MemberDeclarationTestCase("public int P1 { get; set; }")] public int P1 { get; set; }
          [MemberDeclarationTestCase("internal int P2 { get; set; }")] internal int P2 { get; set; }
          [MemberDeclarationTestCase("protected int P3 { get; set; }")] protected int P3 { get; set; }
          [MemberDeclarationTestCase("internal protected int P4 { get; set; }")] protected internal int P4 { get; set; }
          [MemberDeclarationTestCase("internal protected int P5 { get; set; }")] internal protected int P5 { get; set; }
          [MemberDeclarationTestCase("private protected int P6 { get; set; }")] private protected int P6 { get; set; }
          [MemberDeclarationTestCase("private protected int P7 { get; set; }")] protected private int P7 { get; set; }
          [MemberDeclarationTestCase("private int P8 { get; set; }")] private int P8 { get; set; }

          [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] internal int P9 { get; set; }
          [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private protected int P10 { get; set; }
          [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private int P11 { get; set; }
        }

        public class AccessibilitiesOfAccessors_SetMethod_Public {
          [MemberDeclarationTestCase("public int P1 { get; internal set; }")] public int P1 { get; internal set; }
          [MemberDeclarationTestCase("public int P2 { get; protected set; }")] public int P2 { get; protected set; }
          [MemberDeclarationTestCase("public int P3 { get; internal protected set; }")] public int P3 { get; protected internal set; }
          [MemberDeclarationTestCase("public int P4 { get; internal protected set; }")] public int P4 { get; internal protected set; }
          [MemberDeclarationTestCase("public int P5 { get; private protected set; }")] public int P5 { get; private protected set; }
          [MemberDeclarationTestCase("public int P6 { get; private protected set; }")] public int P6 { get; protected private set; }
          [MemberDeclarationTestCase("public int P7 { get; private set; }")] public int P7 { get; private set; }

          [MemberDeclarationTestCase("public int P8 { get; }", IgnorePrivateOrAssembly = true)] public int P8 { get; internal set; }
          [MemberDeclarationTestCase("public int P9 { get; }", IgnorePrivateOrAssembly = true)] public int P9 { get; private protected set; }
          [MemberDeclarationTestCase("public int P10 { get; }", IgnorePrivateOrAssembly = true)] public int P10 { get; private set; }
        }

        public class AccessibilitiesOfAccessors_GetMethod_Public {
          [MemberDeclarationTestCase("public int P1 { internal get; set; }")] public int P1 { internal get; set; }
          [MemberDeclarationTestCase("public int P2 { protected get; set; }")] public int P2 { protected get; set; }
          [MemberDeclarationTestCase("public int P3 { internal protected get; set; }")] public int P3 { protected internal get; set; }
          [MemberDeclarationTestCase("public int P4 { internal protected get; set; }")] public int P4 { internal protected get; set; }
          [MemberDeclarationTestCase("public int P5 { private protected get; set; }")] public int P5 { private protected get; set; }
          [MemberDeclarationTestCase("public int P6 { private protected get; set; }")] public int P6 { protected private get; set; }
          [MemberDeclarationTestCase("public int P7 { private get; set; }")] public int P7 { private get; set; }

          [MemberDeclarationTestCase("public int P8 { set; }", IgnorePrivateOrAssembly = true)] public int P8 { internal get; set; }
          [MemberDeclarationTestCase("public int P9 { set; }", IgnorePrivateOrAssembly = true)] public int P9 { private protected get; set; }
          [MemberDeclarationTestCase("public int P10 { set; }", IgnorePrivateOrAssembly = true)] public int P10 { private get; set; }
        }

        public class AccessibilitiesOfAccessors_FamilyOrAssembly {
          [MemberDeclarationTestCase("internal protected int P0 { get; set; }")] internal protected int P0 { get; set; }
          [MemberDeclarationTestCase("internal protected int P1 { get; internal set; }")] internal protected int P1 { get; internal set; }
          [MemberDeclarationTestCase("internal protected int P2 { get; protected set; }")] internal protected int P2 { get; protected set; }
          [MemberDeclarationTestCase("internal protected int P3 { get; private protected set; }")] internal protected int P3 { get; private protected set; }
          [MemberDeclarationTestCase("internal protected int P4 { get; private protected set; }")] internal protected int P4 { get; protected private set; }
          [MemberDeclarationTestCase("internal protected int P5 { get; private set; }")] internal protected int P5 { get; private set; }

          [MemberDeclarationTestCase("internal protected int P6 { get; }", IgnorePrivateOrAssembly = true)] internal protected int P6 { get; internal set; }
          [MemberDeclarationTestCase("internal protected int P7 { get; }", IgnorePrivateOrAssembly = true)] internal protected int P7 { get; private protected set; }
          [MemberDeclarationTestCase("internal protected int P8 { get; }", IgnorePrivateOrAssembly = true)] internal protected int P8 { get; private set; }
        }

#if false
      namespace StaticValues {
        class Accessors {
          [MemberDeclarationTestCase("public int P1 { get; } = 1;")]
          [MemberDeclarationTestCase("public int P1 { get; } = 1", MemberOmitEndOfStatement = true)]
          public int P1 { get; } = 1;

          [MemberDeclarationTestCase("public int P2 { get; set; } = 2;")] public int P2 { get; set; } = 2;
          [MemberDeclarationTestCase("public int P3 { get; private set; } = 3;")] public int P3 { get; private set; } = 3;
        }
      }
#endif
      }

      namespace Methods {
        namespace Options {
          public abstract class Abstract {
            [MemberDeclarationTestCase("public abstract void M()", MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public abstract void M();", MethodBody = MethodBodyOption.EmptyImplementation)]
            [MemberDeclarationTestCase("public abstract void M()", MethodBody = MethodBodyOption.EmptyImplementation, MemberOmitEndOfStatement = true)]
            [MemberDeclarationTestCase("public abstract void M();", MethodBody = MethodBodyOption.ThrowNotImplementedException)]
            [MemberDeclarationTestCase("public abstract void M()", MethodBody = MethodBodyOption.ThrowNotImplementedException, MemberOmitEndOfStatement = true)]
            [MemberDeclarationTestCase("public abstract void Abstract.M()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public abstract void Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.Options.Abstract.M()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            public abstract void M();
          }

          public abstract class NonAbstract {
            [MemberDeclarationTestCase("public void M()", MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public void M() {}", MethodBody = MethodBodyOption.EmptyImplementation)]
            [MemberDeclarationTestCase("public void M() {}", MethodBody = MethodBodyOption.EmptyImplementation, MemberOmitEndOfStatement = true)]
            [MemberDeclarationTestCase("public void M() => throw new NotImplementedException();", MethodBody = MethodBodyOption.ThrowNotImplementedException)]
            [MemberDeclarationTestCase("public void M() => throw new NotImplementedException()", MethodBody = MethodBodyOption.ThrowNotImplementedException, MemberOmitEndOfStatement = true)]
            [MemberDeclarationTestCase("public void NonAbstract.M()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public void Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.Options.NonAbstract.M()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
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
          }

          public class Override : Virtual {
            [MemberDeclarationTestCase("public override void M() {}")] public override void M() { }
          }

          public class SealedOverride1 : Virtual {
            [MemberDeclarationTestCase("public sealed override void M() {}")] public sealed override void M() { }
          }

          public class SealedOverride2 : Virtual {
            [MemberDeclarationTestCase("public sealed override void M() {}")] public override sealed void M() { }
          }

          public abstract class New : Virtual {
            [MemberDeclarationTestCase("public void M() {}")] public new void M() { }
          }

          public abstract class NewVirtual : Virtual {
            [MemberDeclarationTestCase("public virtual void M() {}")] public new virtual void M() { }
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
          [MemberDeclarationTestCase("public static ref int M() {}")] public static ref int M() => throw new NotImplementedException();
        }

        public static class TupleReturnValues {
          [MemberDeclarationTestCase("public static (int, int) M1() {}")] public static (int, int) M1() => throw new NotImplementedException();
          [MemberDeclarationTestCase("public static (int x, int y) M2() {}")] public static (int x, int y) M2() => throw new NotImplementedException();
          [MemberDeclarationTestCase("public static (int, int, int) M3() {}")] public static (int, int, int) M3() => throw new NotImplementedException();
          [MemberDeclarationTestCase("public static (string, string) M4() {}")] public static (string, string) M4() => throw new NotImplementedException();
        }

        namespace Constructors {
          public class C {
            [MemberDeclarationTestCase("public C() {}")]
            [MemberDeclarationTestCase("public C.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.Constructors.C.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            public C() => throw new NotImplementedException();

            [MemberDeclarationTestCase("static C() {}")]
            [MemberDeclarationTestCase("static C.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("static Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.Constructors.C.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            static C() { }
          }

          public class C<T> {
            [MemberDeclarationTestCase("public C() {}")]
            [MemberDeclarationTestCase("public C<T>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.Constructors.C<T>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            public C() => throw new NotImplementedException();

            [MemberDeclarationTestCase("static C() {}")]
            [MemberDeclarationTestCase("static C<T>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("static Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.Constructors.C<T>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            static C() { }
          }

          public class C<T1, T2> {
            [MemberDeclarationTestCase("public C() {}")]
            [MemberDeclarationTestCase("public C<T1, T2>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.Constructors.C<T1, T2>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            public C() => throw new NotImplementedException();

            [MemberDeclarationTestCase("static C() {}")]
            [MemberDeclarationTestCase("static C<T1, T2>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("static Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.Constructors.C<T1, T2>.C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
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
            [MemberDeclarationTestCase("Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.Destructors.C.~C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            ~C() => throw new NotImplementedException();
          }

          public class C<T> {
            [MemberDeclarationTestCase("~C() {}")]
            [MemberDeclarationTestCase("C<T>.~C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.Destructors.C<T>.~C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            ~C() => throw new NotImplementedException();
          }

          public class C<T1, T2> {
            [MemberDeclarationTestCase("~C() {}")]
            [MemberDeclarationTestCase("C<T1, T2>.~C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.Destructors.C<T1, T2>.~C()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            ~C() => throw new NotImplementedException();
          }
        }

        namespace Operators {
          namespace UnaryOperators {
            public class C {
              [MemberDeclarationTestCase("public static C operator + (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator + (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator +(C c) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator - (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator - (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator -(C c) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator ! (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator ! (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator !(C c) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator ~ (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator ~ (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator ~(C c) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static bool operator true (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static bool operator true (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static bool operator true(C c) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static bool operator false (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static bool operator false (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static bool operator false(C c) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator ++ (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator ++ (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator ++(C c) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator -- (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator -- (C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator --(C c) => throw new NotImplementedException();
            }
          }

          namespace BinaryOperators {
            public class C {
              [MemberDeclarationTestCase("public static C operator + (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator + (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator +(C x, C y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator - (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator - (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator -(C x, C y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator * (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator * (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator *(C x, C y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator / (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator / (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator /(C x, C y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator % (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator % (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator %(C x, C y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator & (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator & (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator &(C x, C y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator | (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator | (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator |(C x, C y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator ^ (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator ^ (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator ^(C x, C y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator >> (C x, int y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator >> (C x, int y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator >>(C x, int y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static C operator << (C x, int y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static C operator << (C x, int y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static C operator <<(C x, int y) => throw new NotImplementedException();
            }
          }

          namespace Comparison {
            public class C : IEquatable<C>, IComparable<C> {
              [MemberDeclarationTestCase("public static bool operator == (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static bool operator == (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static bool operator ==(C x, C y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static bool operator != (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static bool operator != (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static bool operator !=(C x, C y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static bool operator < (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static bool operator < (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static bool operator <(C x, C y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static bool operator > (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static bool operator > (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static bool operator >(C x, C y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static bool operator <= (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static bool operator <= (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static bool operator <=(C x, C y) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static bool operator >= (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static bool operator >= (C x, C y) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
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
              [MemberDeclarationTestCase("public static explicit operator C(V v) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static explicit operator C(V v) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static explicit operator C(V v) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static explicit operator V(C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static explicit operator V(C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static explicit operator V(C c) => throw new NotImplementedException();


              [MemberDeclarationTestCase("public static implicit operator C(W w) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static implicit operator C(W w) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static implicit operator C(W w) => throw new NotImplementedException();

              [MemberDeclarationTestCase("public static implicit operator W(C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
              [MemberDeclarationTestCase("public static implicit operator W(C c) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = true)]
              public static implicit operator W(C c) => throw new NotImplementedException();
            }
          }
        }

        namespace GenericMethods {
          public class C<T> {
            [MemberDeclarationTestCase("public T M(T x) {}")] public T M(T x) => throw new NotImplementedException();
          }

          public class C<T1, T2> {
            [MemberDeclarationTestCase("public T1 M(T2 x) {}")] public T1 M(T2 x) => throw new NotImplementedException();
          }

          public class C {
            [MemberDeclarationTestCase("public T M<T>(T x) {}")]
            [MemberDeclarationTestCase("public T C.M<T>(T x)", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public T Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.GenericMethods.C.M<T>(T x)", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            public T M<T>(T x) => throw new NotImplementedException();

            [MemberDeclarationTestCase("public T1 M<T1, T2>(T2 x) {}")]
            [MemberDeclarationTestCase("public T1 C.M<T1, T2>(T2 x)", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public T1 Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.GenericMethods.C.M<T1, T2>(T2 x)", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            public T1 M<T1, T2>(T2 x) => throw new NotImplementedException();
          }

          public abstract class COpen<T1, T2> {
            [MemberDeclarationTestCase("public abstract void M(T1 t1, T2 t2);")]
            public abstract void M(T1 t1, T2 t2);
          }

          public class CClose1<T2> : COpen<int, T2> {
            [MemberDeclarationTestCase("public override void M(int t1, T2 t2) {}")]
            [MemberDeclarationTestCase("public override void CClose1<T2>.M(int t1, T2 t2)", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public override void Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.GenericMethods.CClose1<T2>.M(int t1, T2 t2)", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            public override void M(int t1, T2 t2) { }
          }

          public class CClose2 : CClose1<int> {
            [MemberDeclarationTestCase("public override void M(int t1, int t2) {}")]
            [MemberDeclarationTestCase("public override void CClose2.M(int t1, int t2)", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
            [MemberDeclarationTestCase("public override void Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.GenericMethods.CClose2.M(int t1, int t2)", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
            public override void M(int t1, int t2) { }
          }

          public class Constraints1 {
            [MemberDeclarationTestCase("public T M1<T>(T x) where T : new() {}")] public T M1<T>(T x) where T : new() => throw new NotImplementedException();
            [MemberDeclarationTestCase("public T M2<T>(T x) where T : struct {}")] public T M2<T>(T x) where T : struct => throw new NotImplementedException();
            [MemberDeclarationTestCase("public T M3<T>(T x) where T : class {}")] public T M3<T>(T x) where T : class => throw new NotImplementedException();
            [MemberDeclarationTestCase("public T M4<T>(T x) where T : System.IDisposable {}")] public T M4<T>(T x) where T : IDisposable => throw new NotImplementedException();
            [MemberDeclarationTestCase("public T M5<T>(T x) where T : System.ICloneable, System.IDisposable {}")] public T M5<T>(T x) where T : IDisposable, ICloneable => throw new NotImplementedException();
            [MemberDeclarationTestCase("public T M6<T>(T x) where T : System.ICloneable, new() {}")] public T M6<T>(T x) where T : ICloneable, new() => throw new NotImplementedException();
            [MemberDeclarationTestCase("public T M7<T>(T x) where T : class, new() {}")] public T M7<T>(T x) where T : class, new() => throw new NotImplementedException();
            [MemberDeclarationTestCase("public T M8<T>(T x) where T : class, System.ICloneable, new() {}")] public T M8<T>(T x) where T : class, ICloneable, new() => throw new NotImplementedException();
            [MemberDeclarationTestCase("public T M9<T>(T x) where T : class, System.ICloneable, System.IDisposable, new() {}")] public T M9<T>(T x) where T : class, ICloneable, IDisposable, new() => throw new NotImplementedException();

            [MemberDeclarationTestCase("public T M10<T>(T x) where T : Constraints1.CBase {}", MemberWithNamespace = false)]
            public T M10<T>(T x) where T : CBase => throw new NotImplementedException();
            public class CBase { }

            [MemberDeclarationTestCase("public T M11<T>(T x) where T : System.Enum {}")] public T M11<T>(T x) where T : System.Enum => throw new NotImplementedException();
            [MemberDeclarationTestCase("public T M12<T>(T x) where T : System.Delegate {}")] public T M12<T>(T x) where T : System.Delegate => throw new NotImplementedException();
            [MemberDeclarationTestCase("public T M13<T>(T x) where T : System.MulticastDelegate {}")] public T M13<T>(T x) where T : System.MulticastDelegate => throw new NotImplementedException();
          }

          class Constraints2 {
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
        }
      }

      namespace Events {
#pragma warning disable CS0067
        public class Options {
          [MemberDeclarationTestCase("public event System.EventHandler E;")]
          [MemberDeclarationTestCase("public event System.EventHandler E", MemberOmitEndOfStatement = true)]
          [MemberDeclarationTestCase("public event EventHandler Options.E;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
          [MemberDeclarationTestCase("public event System.EventHandler Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Events.Options.E;", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
          public event EventHandler E;
        }

        public class Modifiers {
          [MemberDeclarationTestCase("public event System.EventHandler E1;")] public event EventHandler E1;
          [MemberDeclarationTestCase("public static event System.EventHandler E2;")] public static event EventHandler E2;
          [MemberDeclarationTestCase("public static event System.EventHandler E3;")] static public event EventHandler E3;
        }

        public class Accessibilities {
          [MemberDeclarationTestCase("public event System.EventHandler E1;")] public event EventHandler E1;
          [MemberDeclarationTestCase("internal event System.EventHandler E2;")] internal event EventHandler E2;
          [MemberDeclarationTestCase("protected event System.EventHandler E3;")] protected event EventHandler E3;
          [MemberDeclarationTestCase("internal protected event System.EventHandler E4;")] protected internal event EventHandler E4;
          [MemberDeclarationTestCase("internal protected event System.EventHandler E5;")] internal protected event EventHandler E5;
          [MemberDeclarationTestCase("private protected System.EventHandler E6;")] private protected EventHandler E6;
          [MemberDeclarationTestCase("private protected System.EventHandler E7;")] protected private EventHandler E7;
          [MemberDeclarationTestCase("private event System.EventHandler E8;")] private event EventHandler E8;

          [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] internal event EventHandler E9;
          [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private protected event EventHandler E10;
          [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private event EventHandler E11;
        }
#pragma warning restore CS0067
      }

      public interface InterfaceMembers {
        [MemberDeclarationTestCase("int P1 { get; set; }")] int P1 { get; set; }
        [MemberDeclarationTestCase("int P2 { get; }")] int P2 { get; }
        [MemberDeclarationTestCase("int P3 { set; }")] int P3 { set; }
        [MemberDeclarationTestCase("event System.EventHandler E;")] event EventHandler E;
        [MemberDeclarationTestCase("void M();")] void M();
        [MemberDeclarationTestCase("int M(int x);")] int M(int x);
      }

      namespace ImplementedInterfaceMembers {
        class ImplicitMethod : IDisposable, ICloneable {
          [MemberDeclarationTestCase("public void Dispose() {}")]
          [MemberDeclarationTestCase("public void ImplicitMethod.Dispose()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
          [MemberDeclarationTestCase("public void Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.ImplementedInterfaceMembers.ImplicitMethod.Dispose()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
          public void Dispose() { }

          [MemberDeclarationTestCase("public object Clone() {}", MemberWithNamespace = false)]
          public object Clone() => throw new NotImplementedException();
        }

        class ExplicitMethod : IDisposable, ICloneable {
          [MemberDeclarationTestCase("void System.IDisposable.Dispose() {}", MemberWithNamespace = true)]
          [MemberDeclarationTestCase("void IDisposable.Dispose() {}", MemberWithNamespace = false)]
          [MemberDeclarationTestCase("void ExplicitMethod.IDisposable.Dispose()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
          [MemberDeclarationTestCase("void Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.ImplementedInterfaceMembers.ExplicitMethod.System.IDisposable.Dispose()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
          void IDisposable.Dispose() { }

          [MemberDeclarationTestCase("object ICloneable.Clone() {}", MemberWithNamespace = false)]
          object ICloneable.Clone() => throw new NotImplementedException();
        }

        class ExplicitMethodGenericInterface : IEnumerable<int> {
          [MemberDeclarationTestCase("System.Collections.Generic.IEnumerator<int> System.Collections.Generic.IEnumerable<int>.GetEnumerator() {}")]
          IEnumerator<int> System.Collections.Generic.IEnumerable<int>.GetEnumerator() => throw new NotImplementedException();
          System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new NotImplementedException();
        }

        interface IProperty {
          int P1 { get; set; }
          int P2 { get; }
          int P3 { set; }
        }

        class ImplicitProperty1 : IProperty {
          [MemberDeclarationTestCase("public int P1 { get; set; }", MemberWithNamespace = false)]
          [MemberDeclarationTestCase("public int ImplicitProperty1.P1 { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
          [MemberDeclarationTestCase("public int Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.ImplementedInterfaceMembers.ImplicitProperty1.P1 { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
          public int P1 { get; set; }

          [MemberDeclarationTestCase("public int P2 { get; }", MemberWithNamespace = false)]
          public int P2 { get { throw new NotImplementedException(); } }

          [MemberDeclarationTestCase("public int P3 { set; }", MemberWithNamespace = false)]
          public int P3 { set { throw new NotImplementedException(); } }
        }

        class ExplicitProperty1 : IProperty {
          [MemberDeclarationTestCase("int IProperty.P1 { get; set; }", MemberWithNamespace = false)]
          [MemberDeclarationTestCase("int ExplicitProperty1.IProperty.P1 { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
          [MemberDeclarationTestCase("int Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.ImplementedInterfaceMembers.ExplicitProperty1.Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.ImplementedInterfaceMembers.IProperty.P1 { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
          int IProperty.P1 { get; set; }

          [MemberDeclarationTestCase("int IProperty.P2 { get; }", MemberWithNamespace = false)]
          int IProperty.P2 { get { throw new NotImplementedException(); } }

          [MemberDeclarationTestCase("int IProperty.P3 { set; }", MemberWithNamespace = false)]
          int IProperty.P3 { set { throw new NotImplementedException(); } }
        }

        class ImplicitProperty2 : IAsyncResult {
          [MemberDeclarationTestCase("public object AsyncState { get; }")]
          public object AsyncState { get => throw new NotImplementedException(); }

          [MemberDeclarationTestCase("public System.Threading.WaitHandle AsyncWaitHandle { get; }")]
          public System.Threading.WaitHandle AsyncWaitHandle { get => throw new NotImplementedException(); }

          [MemberDeclarationTestCase("public bool CompletedSynchronously { get; }")]
          public bool CompletedSynchronously { get => throw new NotImplementedException(); }

          [MemberDeclarationTestCase("public bool IsCompleted { get; }")]
          public bool IsCompleted { get => throw new NotImplementedException(); }
        }

        class ExplicitProperty2 : IAsyncResult {
          [MemberDeclarationTestCase("object System.IAsyncResult.AsyncState { get; }")]
          object IAsyncResult.AsyncState { get => throw new NotImplementedException(); }

          [MemberDeclarationTestCase("System.Threading.WaitHandle System.IAsyncResult.AsyncWaitHandle { get; }")]
          System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle { get => throw new NotImplementedException(); }

          [MemberDeclarationTestCase("bool System.IAsyncResult.CompletedSynchronously { get; }")]
          bool IAsyncResult.CompletedSynchronously { get => throw new NotImplementedException(); }

          [MemberDeclarationTestCase("bool System.IAsyncResult.IsCompleted { get; }")]
          bool IAsyncResult.IsCompleted { get => throw new NotImplementedException(); }
        }

        class ExplicitPropertyGenericInterface : IReadOnlyCollection<string> {
          [MemberDeclarationTestCase("int System.Collections.Generic.IReadOnlyCollection<string>.Count { get; }")]
          int IReadOnlyCollection<string>.Count { get => throw new NotImplementedException(); }

          public IEnumerator<string> GetEnumerator() => throw new NotImplementedException();
          System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new NotImplementedException();
        }

        class ExplicitProperty2WithoutNamespace : IAsyncResult {
          [MemberDeclarationTestCase("object IAsyncResult.AsyncState { get; }", MemberWithNamespace = false)]
          object IAsyncResult.AsyncState { get => throw new NotImplementedException(); }

          [MemberDeclarationTestCase("WaitHandle IAsyncResult.AsyncWaitHandle { get; }", MemberWithNamespace = false)]
          System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle { get => throw new NotImplementedException(); }

          [MemberDeclarationTestCase("bool IAsyncResult.CompletedSynchronously { get; }", MemberWithNamespace = false)]
          bool IAsyncResult.CompletedSynchronously { get => throw new NotImplementedException(); }

          [MemberDeclarationTestCase("bool IAsyncResult.IsCompleted { get; }", MemberWithNamespace = false)]
          bool IAsyncResult.IsCompleted { get => throw new NotImplementedException(); }
        }

        interface IEvent {
          event EventHandler E;
        }

#pragma warning disable CS0067
        class ImplicitEvent : IEvent {
          [MemberDeclarationTestCase("public event EventHandler E;", MemberWithNamespace = false)]
          [MemberDeclarationTestCase("public event EventHandler E", MemberWithNamespace = false, MemberOmitEndOfStatement = true)]
          [MemberDeclarationTestCase("public event EventHandler ImplicitEvent.E;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
          [MemberDeclarationTestCase("public event System.EventHandler Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.ImplementedInterfaceMembers.ImplicitEvent.E;", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
          public event EventHandler E;
        }
#pragma warning restore CS0067

        class ExplicitEvent : IEvent {
          [MemberDeclarationTestCase("event EventHandler IEvent.E;", MemberWithNamespace = false)]
          [MemberDeclarationTestCase("event EventHandler IEvent.E", MemberWithNamespace = false, MemberOmitEndOfStatement = true)]
          [MemberDeclarationTestCase("event EventHandler ExplicitEvent.IEvent.E;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
          [MemberDeclarationTestCase("event System.EventHandler Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.ImplementedInterfaceMembers.ExplicitEvent.Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.ImplementedInterfaceMembers.IEvent.E;", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
          event EventHandler IEvent.E {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
          }
        }
      }

      namespace ParameterLists {
        public class Standard {
          [MemberDeclarationTestCase("public void M() {}")] public void M() { }
          [MemberDeclarationTestCase("public void M(int x) {}")] public void M(int x) { }
          [MemberDeclarationTestCase("public void M(int x, int y) {}")] public void M(int x, int y) { }
        }

        public class InOutRef {
          [MemberDeclarationTestCase("public void M1(in int x) {}")] public void M1(in int x) { }
          [MemberDeclarationTestCase("public void M2(out int x) {}")] public void M2(out int x) => throw new NotImplementedException();
          [MemberDeclarationTestCase("public void M3(ref int x) {}")] public void M3(ref int x) => throw new NotImplementedException();
        }

        public class Params {
          [MemberDeclarationTestCase("public void M(params int[] p) {}")] public void M(params int[] p) { }
          [MemberDeclarationTestCase("public void M(int p0, params int[] p) {}")] public void M(int p0, params int[] p) { }
        }

        namespace ExtensionMethods {
          public class C { }

          public static class Ex {
            [MemberDeclarationTestCase("public static void M(this C c) {}", MemberWithNamespace = false)]
            public static void M(this C c) { }

            [MemberDeclarationTestCase("public static void M(this C c, int i) {}", MemberWithNamespace = false)]
            public static void M(this C c, int i) { }
          }
        }

        public class Tuples {
          [MemberDeclarationTestCase("public void M1((int, int) arg) {}")] public void M1((int, int) arg) { }
          [MemberDeclarationTestCase("public void M2((int x, int y) arg) {}")] public void M2((int x, int y) arg) { }
          [MemberDeclarationTestCase("public void M3((int, int, int) arg) {}")] public void M3((int, int, int) arg) { }
          [MemberDeclarationTestCase("public void M4((string, string) arg) {}")] public void M4((string, string) arg) { }
        }

        namespace Optionals {
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
            [MemberDeclarationTestCase("public void M23(System.AttributeTargets x = default(System.AttributeTargets)) {}")] public void M23(AttributeTargets x = default(AttributeTargets)) { }
            [MemberDeclarationTestCase("public void M231(System.AttributeTargets x = default) {}", UseDefaultLiteral = true)] public void M231(AttributeTargets x = default(AttributeTargets)) { }
            [MemberDeclarationTestCase("public void M24(System.AttributeTargets x = (System.AttributeTargets)42) {}")] public void M24(AttributeTargets x = (System.AttributeTargets)42) { }
            [MemberDeclarationTestCase("public void M25(System.AttributeTargets x = (System.AttributeTargets)3) {}")] public void M25(AttributeTargets x = AttributeTargets.Assembly | AttributeTargets.Module) { }
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
            [MemberDeclarationTestCase("public void M1(System.DateTimeOffset x = default(System.DateTimeOffset)) {}")]
            public void M1(DateTimeOffset x = default(DateTimeOffset)) { }

            [MemberDeclarationTestCase("public void M2(System.Guid x = default(System.Guid)) {}")]
            public void M2(Guid x = default(Guid)) { }

            [MemberDeclarationTestCase("public void M3(System.Threading.CancellationToken x = default(System.Threading.CancellationToken)) {}")]
            public void M3(CancellationToken x = default(CancellationToken)) { }

            [MemberDeclarationTestCase("public void M4(System.Threading.CancellationToken x = default) {}", UseDefaultLiteral = true)]
            public void M4(CancellationToken x = default(CancellationToken)) { }
          }
        }
      }
    }
  }

  partial class GeneratorTests {
    [Test]
    public void TestGenerateMemberDeclaration()
    {
      foreach (var type in FindTypes(t => t.FullName.Contains(".TestCases.MemberDeclaration."))) {
        const BindingFlags memberBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        foreach (var member in type.GetMembers(memberBindingFlags).Where(m => !(m is Type /*except nested type*/))) {
          foreach (var attr in member.GetCustomAttributes<MemberDeclarationTestCaseAttribute>()) {
            Assert.AreEqual(
              attr.Expected,
              Generator.GenerateMemberDeclaration(member, null, GetGeneratorOptions(attr)),
              message: $"{attr.SourceLocation} ({type.FullName}.{member.Name})"
            );
          }
        }
      }
    }
  }
}
