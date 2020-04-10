using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  class AttributeTestCaseAttribute : GeneratorTestCaseAttribute {
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
      namespace AttributeTargets {
        [AttributeTestCase("[System.Obsolete]")][Obsolete] class C { }
        [AttributeTestCase("[System.Obsolete]")][Obsolete] struct S { }
        [AttributeTestCase("[System.Obsolete]")][Obsolete] delegate void D();
        [AttributeTestCase("[System.Obsolete]")][Obsolete] interface I { }
        [AttributeTestCase("[System.Obsolete]")][Obsolete] enum E { }

        [AttributeTestCase("")] class N { }

#pragma warning disable CS0414
        class X {
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public X() { }
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public void M() { }
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public int P { get; set; }
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public event EventHandler E = null;
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public int F = default;
        }
#pragma warning restore CS0414
      }

      namespace AttributeTypes {
        [AttributeTestCase("[System.Flags]")]
        [Flags]
        enum Flags1 : int { }

        [AttributeTestCase("[Flags]", TypeWithNamespace = false)]
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

        [AttributeTestCase("[System.Obsolete(\"deprecated\", true)]")]
        [Obsolete("deprecated", true)]
        class Obsolete3 { }

        [AttributeTestCase("[System.Obsolete(\"deprecated\")]")]
        [Obsolete("deprecated", false)]
        class Obsolete4 { }

        [AttributeTestCase("[System.Serializable]")]
        [Serializable]
        class Serializable1 { }

        class Conditionals {
          [AttributeTestCase("[System.Diagnostics.Conditional(\"DEBUG\")]")]
          [System.Diagnostics.Conditional("DEBUG")]
          public void M1() { }

          [AttributeTestCase("[Conditional(\"DEBUG\")]", TypeWithNamespace = false)]
          [System.Diagnostics.Conditional("DEBUG")]
          public void M2() { }
        }

        static class Extension {
          [AttributeTestCase("")] // does not emit System.Runtime.CompilerServices.ExtensionAttribute
          public static void M1(this int x) { }
        }

        [AttributeTestCase("[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 1)]")]
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 1)]
        struct StructLayout1 {
          [AttributeTestCase("[System.Runtime.InteropServices.FieldOffset(0)]")]
          [System.Runtime.InteropServices.FieldOffset(0)]
          public byte F0;

          [AttributeTestCase("[FieldOffset(1)]", TypeWithNamespace = false)]
          [System.Runtime.InteropServices.FieldOffset(1)]
          public byte F1;
        }

        [AttributeTestCase("[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 1)]")]
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 1)]
        struct StructLayout2 {
          [AttributeTestCase("[System.Runtime.InteropServices.FieldOffset(0)]")][System.Runtime.InteropServices.FieldOffset(0)] public byte F0;
        }

        [AttributeTestCase("[StructLayout(LayoutKind.Explicit, Size = 1)]", TypeWithNamespace = false)]
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 1)]
        struct StructLayout3 {
          [AttributeTestCase("[FieldOffset(0)]", TypeWithNamespace = false)] [System.Runtime.InteropServices.FieldOffset(0)] public byte F0;
        }

        [AttributeTestCase("")]
        struct NoStructLayout { }
      }
    }
  }

  partial class GeneratorTests {
    [Test]
    public void TestGenerateAttributeList()
    {
      foreach (var type in FindTypes(t => t.FullName.Contains(".TestCases.Attributes."))) {
        Test(type);

        foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)) {
          Test(member);
        }
      }

      static void Test(MemberInfo typeOrMember)
      {
        var attr = typeOrMember.GetCustomAttribute<AttributeTestCaseAttribute>();

        if (attr == null)
          return;

        Assert.AreEqual(
          attr.Expected,
          string.Join(", ", Generator.GenerateAttributeList(typeOrMember, null, GetGeneratorOptions(attr))),
          message: $"{attr.SourceLocation} ({typeOrMember.DeclaringType?.FullName}.{typeOrMember.Name})"
        );
      }
    }
  }
}
