// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CS8597

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

        class AttributeTargetsPropertyAccessor {
          [MemberDeclarationTestCase(
            "public int P0 { get; set; }",
            // "public int P0 { [Obsolete] get; [Obsolete] set; }", // TODO: generate accessor attributes
            AttributeWithNamespace = false
          )]
          public int P0 {
            [AttributeTestCase("[System.Obsolete]")]
            [Obsolete]
            get => throw null;

            [AttributeTestCase("[System.Obsolete]")]
            [Obsolete]
            set => throw null;
          }
        }

        class AttributeTargetsPropertyBackingField {
          [MemberDeclarationTestCase(
            "public int P0 { get; set; }",
            // "[field: DebuggerBrowsable(DebuggerBrowsableState.Never), Obsolete, CompilerGenerated] public int P0 { get; set; }", // TODO: generate backing field attributes
            AttributeWithNamespace = false
          )]
          [field: AttributeTestCase("[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)], [System.Obsolete], [System.Runtime.CompilerServices.CompilerGenerated]")]
          [field: Obsolete]
          public int P0 { get; set; }
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

          [MemberDeclarationTestCase(
            "[return: XmlIgnore] public bool M1()",
            AttributeWithNamespace = false,
            MethodBody = MethodBodyOption.None
          )]
          [Obsolete]
          [return: AttributeTestCase("[return: System.Xml.Serialization.XmlIgnore]")]
          [return: System.Xml.Serialization.XmlIgnore]
          public bool M1() => throw new NotImplementedException();

          [MemberDeclarationTestCase(
            "[return: XmlIgnore] public bool M1()",
            AttributeWithNamespace = false,
            MethodBody = MethodBodyOption.None
          )]
          [Obsolete]
          [return: AttributeTestCase("[return: System.Xml.Serialization.XmlIgnore]")]
          [return: System.Xml.Serialization.XmlIgnore]
          public delegate bool D0();
        }

        class AttributeTargetsParameter {
          [MemberDeclarationTestCase(
            "public bool M([CallerFilePath] [Optional] string sourceFilePath = null, [CallerLineNumber] [Optional] int sourceLineNumber = 0)",
            AttributeWithNamespace = false,
            MethodBody = MethodBodyOption.None
          )]
          [Obsolete]
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
            "public delegate bool D([Optional] int arg = 0);",
            AttributeWithNamespace = false
          )]
          [Obsolete]
          public delegate bool D(
            [AttributeTestCase(
              "[System.Runtime.InteropServices.Optional]",
              AttributeWithNamespace = true
            )]
            [AttributeTestCase(
              "[Optional]",
              AttributeWithNamespace = false
            )]
            int arg = 0
          );
        }

        public class AttributeTargetsReturnParameter {
          [MemberDeclarationTestCase(
            "[return: XmlIgnore] public string M()",
            AttributeWithNamespace = false,
            MethodBody = MethodBodyOption.None
          )]
          [return: AttributeTestCase(
            "[return: System.Xml.Serialization.XmlIgnore]",
            AttributeWithNamespace = true
          )]
          [Obsolete]
          [return: System.Xml.Serialization.XmlIgnore]
          public string M() => throw new NotImplementedException();

          [TypeDeclarationTestCase(
            "[return: XmlIgnore] public delegate bool D();",
            AttributeWithNamespace = false
          )]
          [AttributeTestCase(
            "[System.Obsolete]",
            AttributeWithNamespace = true
          )]
          [return: AttributeTestCase(
            "[return: System.Xml.Serialization.XmlIgnore]",
            AttributeWithNamespace = true
          )]
          [Obsolete]
          [return: System.Xml.Serialization.XmlIgnore]
          public delegate bool D();
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
    {
      //type.Namespace!.StartsWith("System", StringComparison.Ordinal)
      if (type.IsAssignableTo(typeof(ITestCaseAttribute)))
        return false;

      return true;
    }

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
