using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  class TypeGenericArgumentConstraintTestCaseAttribute : Attribute {
    public string Expected { get; private set; }
    public bool WithNamespace { get; set; } = true;
    public string SourceLocation { get; }

    public TypeGenericArgumentConstraintTestCaseAttribute(
      string expected,
      [CallerFilePath] string sourceFilePath = null,
      [CallerLineNumber] int lineNumber = 0
    )
    {
      this.Expected = expected;
      this.SourceLocation = $"{Path.GetFileName(sourceFilePath)}:{lineNumber}";
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

      [TypeGenericArgumentConstraintTestCase("where T : IDisposable", WithNamespace = false)]
      public class C_TInterface<T> where T : System.IDisposable { }

      [TypeGenericArgumentConstraintTestCase("where T : System.IDisposable", WithNamespace = true)]
      public class C_TInterface2<T> where T : System.IDisposable { }

      [TypeGenericArgumentConstraintTestCase("where T : Enum", WithNamespace = false)]
      public class C_TEnum1<T> where T : System.Enum { }

      [TypeGenericArgumentConstraintTestCase("where T : System.Enum", WithNamespace = true)]
      public class C_TEnum2<T> where T : System.Enum { }

      [TypeGenericArgumentConstraintTestCase("where T : Delegate", WithNamespace = false)]
      public class C_TDelegate1<T> where T : System.Delegate { }

      [TypeGenericArgumentConstraintTestCase("where T : System.Delegate", WithNamespace = true)]
      public class C_TDelegate2<T> where T : System.Delegate { }

      [TypeGenericArgumentConstraintTestCase("where T : MulticastDelegate", WithNamespace = false)]
      public class C_TMulticastDelegate1<T> where T : System.MulticastDelegate { }

      [TypeGenericArgumentConstraintTestCase("where T : System.MulticastDelegate", WithNamespace = true)]
      public class C_TMulticastDelegate2<T> where T : System.MulticastDelegate { }

      [TypeGenericArgumentConstraintTestCase("where T : class, new()", WithNamespace = true)]
      public class C_TComplex1<T> where T : class, new() { }

      [TypeGenericArgumentConstraintTestCase("where T : class, System.IDisposable, new()", WithNamespace = true)]
      public class C_TComplex2_1<T> where T : class, IDisposable, new() { }

      [TypeGenericArgumentConstraintTestCase("where T : class, System.ICloneable, System.IDisposable, new()", WithNamespace = true)]
      public class C_TComplex2_2<T> where T : class, IDisposable, ICloneable, new() { }

      [TypeGenericArgumentConstraintTestCase("where T : class, System.ICloneable, System.IDisposable, new()", WithNamespace = true)]
      public class C_TComplex2_3<T> where T : class, ICloneable, IDisposable, new() { }
    }
  }

  partial class GeneratorTests {
    [Test]
    public void TestGenerateGenericArgumentConstraintDeclaration_Type()
    {
      foreach (var type in FindTypes(t => t.FullName.Contains(".TestCases.TypeGenericArgumentConstraints."))) {
        var attr = type.GetCustomAttribute<TypeGenericArgumentConstraintTestCaseAttribute>();

        if (attr == null)
          return;

        var options = new GeneratorOptions();

        if (attr.WithNamespace)
          options.TypeDeclarationWithNamespace = attr.WithNamespace;

        Assert.AreEqual(
          attr.Expected,
          Generator.GenerateGenericArgumentConstraintDeclaration(type.GetGenericArguments().First(), null, options),
          message: $"{attr.SourceLocation} ({type.FullName})"
        );
      }
    }
  }
}
