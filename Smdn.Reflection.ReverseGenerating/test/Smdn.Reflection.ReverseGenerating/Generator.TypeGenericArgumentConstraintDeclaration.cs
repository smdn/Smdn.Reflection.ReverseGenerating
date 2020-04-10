using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  class TypeGenericArgumentConstraintTestCaseAttribute : GeneratorTestCaseAttribute {
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
    [Test]
    public void TestGenerateGenericArgumentConstraintDeclaration_OfType()
    {
      foreach (var type in FindTypes(t => t.FullName.Contains(".TestCases.TypeGenericArgumentConstraints."))) {
        var attr = type.GetCustomAttribute<TypeGenericArgumentConstraintTestCaseAttribute>();

        if (attr == null)
          continue;

        Assert.AreEqual(
          attr.Expected,
          Generator.GenerateGenericArgumentConstraintDeclaration(type.GetGenericArguments().First(), null, GetGeneratorOptions(attr)),
          message: $"{attr.SourceLocation} ({type.FullName})"
        );
      }
    }
  }
}
