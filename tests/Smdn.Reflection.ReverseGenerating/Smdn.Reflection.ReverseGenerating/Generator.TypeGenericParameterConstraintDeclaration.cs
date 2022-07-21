// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  public class TypeGenericParameterConstraintTestCaseAttribute : GeneratorTestCaseAttribute {
    public TypeGenericParameterConstraintTestCaseAttribute(
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
    namespace TypeGenericParameterConstraints {
      [TypeGenericParameterConstraintTestCase("")]
      public class C_TNone<T> { }

      [TypeGenericParameterConstraintTestCase("where T : new()")]
      public class C_TDefaultConstructor<T> where T : new() { }

      [TypeGenericParameterConstraintTestCase("where T : class")]
      public class C_TClass<T> where T : class { }

      [TypeGenericParameterConstraintTestCase("where T : struct")]
      public class C_TStruct<T> where T : struct { }

      [TypeGenericParameterConstraintTestCase("where T : notnull")]
      public class C_TNotNull<T> where T : notnull { }

      [TypeGenericParameterConstraintTestCase("where T : unmanaged")]
      public class C_TUnmanaged<T> where T : unmanaged { }

      [TypeGenericParameterConstraintTestCase("where T : IDisposable", TypeWithNamespace = false)]
      public class C_TInterface<T> where T : System.IDisposable { }

      [TypeGenericParameterConstraintTestCase("where T : System.IDisposable", TypeWithNamespace = true)]
      public class C_TInterface2<T> where T : System.IDisposable { }

      [TypeGenericParameterConstraintTestCase("where T : Enum", TypeWithNamespace = false)]
      public class C_TEnum1<T> where T : System.Enum { }

      [TypeGenericParameterConstraintTestCase("where T : System.Enum", TypeWithNamespace = true)]
      public class C_TEnum2<T> where T : System.Enum { }

      [TypeGenericParameterConstraintTestCase("where T : Delegate", TypeWithNamespace = false)]
      public class C_TDelegate1<T> where T : System.Delegate { }

      [TypeGenericParameterConstraintTestCase("where T : System.Delegate", TypeWithNamespace = true)]
      public class C_TDelegate2<T> where T : System.Delegate { }

      [TypeGenericParameterConstraintTestCase("where T : MulticastDelegate", TypeWithNamespace = false)]
      public class C_TMulticastDelegate1<T> where T : System.MulticastDelegate { }

      [TypeGenericParameterConstraintTestCase("where T : System.MulticastDelegate", TypeWithNamespace = true)]
      public class C_TMulticastDelegate2<T> where T : System.MulticastDelegate { }

      [TypeGenericParameterConstraintTestCase("where T : class, new()", TypeWithNamespace = true)]
      public class C_TComplex1<T> where T : class, new() { }

      [TypeGenericParameterConstraintTestCase("where T : class, System.IDisposable, new()", TypeWithNamespace = true)]
      public class C_TComplex2_1<T> where T : class, IDisposable, new() { }

      [TypeGenericParameterConstraintTestCase("where T : class, System.ICloneable, System.IDisposable, new()", TypeWithNamespace = true)]
      public class C_TComplex2_2<T> where T : class, IDisposable, ICloneable, new() { }

      [TypeGenericParameterConstraintTestCase("where T : class, System.ICloneable, System.IDisposable, new()", TypeWithNamespace = true)]
      public class C_TComplex2_3<T> where T : class, ICloneable, IDisposable, new() { }
    }
  }

  partial class GeneratorTests {
    private static System.Collections.IEnumerable YieldGenericParameterConstraintDeclarationOfTypeTestCase()
      => FindTypes(t => t.FullName!.Contains(".TestCases.TypeGenericParameterConstraints."))
        .SelectMany(
          t => t.GetCustomAttributes<TypeGenericParameterConstraintTestCaseAttribute>().Select(
            a => new object[] { a, t }
          )
        );

    [TestCaseSource(nameof(YieldGenericParameterConstraintDeclarationOfTypeTestCase))]
    public void TestGenerateGenericParameterConstraintDeclaration_OfType(
      TypeGenericParameterConstraintTestCaseAttribute attrTestCase,
      Type type
    )
    {
      Assert.AreEqual(
        attrTestCase.Expected,
        Generator.GenerateGenericParameterConstraintDeclaration(type.GetGenericArguments().First(), null, GetGeneratorOptions(attrTestCase)),
        message: $"{attrTestCase.SourceLocation} ({type.FullName})"
      );
    }
  }
}
