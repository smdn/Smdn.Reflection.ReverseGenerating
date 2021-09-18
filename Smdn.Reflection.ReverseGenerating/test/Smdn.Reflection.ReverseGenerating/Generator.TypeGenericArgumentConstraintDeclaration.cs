// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  public class TypeGenericArgumentConstraintTestCaseAttribute : GeneratorTestCaseAttribute {
    public TypeGenericArgumentConstraintTestCaseAttribute(
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
    namespace TypeGenericArgumentConstraints {
      [TypeGenericArgumentConstraintTestCase(null)]
      public class C_TNone<T> { }

      [TypeGenericArgumentConstraintTestCase("where T : new()")]
      public class C_TDefaultConstructor<T> where T : new() { }

      [TypeGenericArgumentConstraintTestCase("where T : class")]
      public class C_TClass<T> where T : class { }

      [TypeGenericArgumentConstraintTestCase("where T : struct")]
      public class C_TStruct<T> where T : struct { }

      [TypeGenericArgumentConstraintTestCase("where T : notnull")]
      public class C_TNotNull<T> where T : notnull { }

      [TypeGenericArgumentConstraintTestCase("where T : unmanaged")]
      public class C_TUnmanaged<T> where T : unmanaged { }

      [TypeGenericArgumentConstraintTestCase("where T : IDisposable", TypeWithNamespace = false)]
      public class C_TInterface<T> where T : System.IDisposable { }

      [TypeGenericArgumentConstraintTestCase("where T : System.IDisposable", TypeWithNamespace = true)]
      public class C_TInterface2<T> where T : System.IDisposable { }

      [TypeGenericArgumentConstraintTestCase("where T : Enum", TypeWithNamespace = false)]
      public class C_TEnum1<T> where T : System.Enum { }

      [TypeGenericArgumentConstraintTestCase("where T : System.Enum", TypeWithNamespace = true)]
      public class C_TEnum2<T> where T : System.Enum { }

      [TypeGenericArgumentConstraintTestCase("where T : Delegate", TypeWithNamespace = false)]
      public class C_TDelegate1<T> where T : System.Delegate { }

      [TypeGenericArgumentConstraintTestCase("where T : System.Delegate", TypeWithNamespace = true)]
      public class C_TDelegate2<T> where T : System.Delegate { }

      [TypeGenericArgumentConstraintTestCase("where T : MulticastDelegate", TypeWithNamespace = false)]
      public class C_TMulticastDelegate1<T> where T : System.MulticastDelegate { }

      [TypeGenericArgumentConstraintTestCase("where T : System.MulticastDelegate", TypeWithNamespace = true)]
      public class C_TMulticastDelegate2<T> where T : System.MulticastDelegate { }

      [TypeGenericArgumentConstraintTestCase("where T : class, new()", TypeWithNamespace = true)]
      public class C_TComplex1<T> where T : class, new() { }

      [TypeGenericArgumentConstraintTestCase("where T : class, System.IDisposable, new()", TypeWithNamespace = true)]
      public class C_TComplex2_1<T> where T : class, IDisposable, new() { }

      [TypeGenericArgumentConstraintTestCase("where T : class, System.ICloneable, System.IDisposable, new()", TypeWithNamespace = true)]
      public class C_TComplex2_2<T> where T : class, IDisposable, ICloneable, new() { }

      [TypeGenericArgumentConstraintTestCase("where T : class, System.ICloneable, System.IDisposable, new()", TypeWithNamespace = true)]
      public class C_TComplex2_3<T> where T : class, ICloneable, IDisposable, new() { }
    }
  }

  partial class GeneratorTests {
    private static System.Collections.IEnumerable YieldGenericArgumentConstraintDeclarationOfTypeTestCase()
      => FindTypes(t => t.FullName.Contains(".TestCases.TypeGenericArgumentConstraints."))
        .SelectMany(
          t => t.GetCustomAttributes<TypeGenericArgumentConstraintTestCaseAttribute>().Select(
            a => new object[] { a, t }
          )
        );

    [TestCaseSource(nameof(YieldGenericArgumentConstraintDeclarationOfTypeTestCase))]
    public void TestGenerateGenericArgumentConstraintDeclaration_OfType(
      TypeGenericArgumentConstraintTestCaseAttribute attrTestCase,
      Type type
    )
    {
      Assert.AreEqual(
        attrTestCase.Expected,
        Generator.GenerateGenericArgumentConstraintDeclaration(type.GetGenericArguments().First(), null, GetGeneratorOptions(attrTestCase)),
        message: $"{attrTestCase.SourceLocation} ({type.FullName})"
      );
    }
  }
}
