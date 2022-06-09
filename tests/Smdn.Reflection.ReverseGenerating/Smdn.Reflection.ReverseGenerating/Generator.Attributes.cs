// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
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
      namespace AttributeTargets {
        [AttributeTestCase("[System.Obsolete]")][Obsolete] class C { }
        [AttributeTestCase("[System.Obsolete]")][Obsolete] struct S { }
        [AttributeTestCase("[System.Obsolete]")][Obsolete] delegate void D();
        [AttributeTestCase("[System.Obsolete]")][Obsolete] interface I { }
        [AttributeTestCase("[System.Obsolete]")][Obsolete] enum E { }

        [AttributeTestCase("")] class N { }

#pragma warning disable CS0414
        class AttributeTargetsMember {
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public AttributeTargetsMember() { }
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public void M() { }
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public int P { get; set; }
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public event EventHandler E = null;
          [AttributeTestCase("[System.Obsolete]")][Obsolete] public int F = default;
        }

        class AttributeTargetsReturnValue {
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
            ExpectedValueGeneratorType = typeof(AttributeTargetsReturnValue),
            ExpectedValueGeneratorMemberName = nameof(M0_ExpectedResult)
          )]
          [return: MarshalAs(UnmanagedType.Bool)]
          public bool M0() => throw new NotImplementedException();
#endif

          [return: AttributeTestCase("[return: System.Xml.Serialization.XmlIgnore]")]
          [return: System.Xml.Serialization.XmlIgnore]
          public bool M1() => throw new NotImplementedException();
        }

        class AttributeTargetsParameter {
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
        }

        public class AttributeTargetsReturnParameter {
          [MemberDeclarationTestCase(
            "[return: System.Xml.Serialization.XmlIgnore] public string M()",
            AttributeWithNamespace = true,
            MethodBody = MethodBodyOption.None
          )]
          [MemberDeclarationTestCase(
            "[return: XmlIgnore] public string M()",
            AttributeWithNamespace = false,
            MethodBody = MethodBodyOption.None
          )]
          [return: System.Xml.Serialization.XmlIgnore]
          public string M() => throw new NotImplementedException();
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
    }
  }

  partial class GeneratorTests {
    private static System.Collections.IEnumerable YieldAttributeListTestCase()
      => FindTypes(t => t.FullName!.Contains(".TestCases.Attributes."))
        .SelectMany(t => t
          .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
          .Prepend((MemberInfo)t) // prepend type itself as a test target
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

      options.AttributeDeclaration.TypeFilter = static (type, _) => type.Namespace!.StartsWith("System", StringComparison.Ordinal);

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
      => FindTypes(t => t.FullName!.Contains(".TestCases.Attributes."))
        .SelectMany(t =>
          t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
        )
        .SelectMany(method =>
          method.GetParameters().Prepend(method.ReturnParameter)
        )
        .SelectMany(
          p => p.GetCustomAttributes<AttributeTestCaseAttribute>().Select(
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

      options.AttributeDeclaration.TypeFilter = static (type, _) => type.Namespace!.StartsWith("System", StringComparison.Ordinal);

      Assert.AreEqual(
        attrTestCase.Expected,
        string.Join(", ", Generator.GenerateAttributeList(param, null, options)),
        message: $"{attrTestCase.SourceLocation} ({param.Member.DeclaringType!.FullName}.{param.Member.Name} {(param.Name ?? "return value")})"
      );
    }
  }
}
