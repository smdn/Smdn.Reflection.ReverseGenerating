// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
  class ExplicitBaseTypeAndInterfacesTestCaseAttribute : GeneratorTestCaseAttribute {
    public ExplicitBaseTypeAndInterfacesTestCaseAttribute(
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
    namespace ExplicitBaseTypeAndInterfaces {

      namespace ClassAndInterfaceTypeLists {
        [ExplicitBaseTypeAndInterfacesTestCase("", TypeWithNamespace = false)]
        public abstract class C { }

        [ExplicitBaseTypeAndInterfacesTestCase("C", TypeWithNamespace = false)]
        public abstract class CC : C { }

        [ExplicitBaseTypeAndInterfacesTestCase("CC", TypeWithNamespace = false)]
        public abstract class CCC : CC { }

        [ExplicitBaseTypeAndInterfacesTestCase("IDisposable", TypeWithNamespace = false)]
        [ExplicitBaseTypeAndInterfacesTestCase("System.IDisposable", TypeWithNamespace = true)]
        public abstract class C0 : IDisposable {
          public abstract void Dispose();
        }

        [ExplicitBaseTypeAndInterfacesTestCase("IEquatable<int>", TypeWithNamespace = false)]
        [ExplicitBaseTypeAndInterfacesTestCase("System.IEquatable<int>", TypeWithNamespace = true)]
        public abstract class C0_1 : IEquatable<int> {
          public abstract void Dispose();
          public abstract bool Equals(int other);
        }

        [ExplicitBaseTypeAndInterfacesTestCase("C0", TypeWithNamespace = false)]
        public abstract class C1 : C0 {
        }

        [ExplicitBaseTypeAndInterfacesTestCase("C0, ICloneable", TypeWithNamespace = false)]
        public abstract class C2 : C0, ICloneable {
          public abstract object Clone();
        }

        [ExplicitBaseTypeAndInterfacesTestCase("C0, ICloneable", TypeWithNamespace = false)]
        // TODO: reimplemented interface? [BaseTypeTest("C0, ICloneable, IDisposable")]
        public abstract class C3 : C0, IDisposable, ICloneable {
          public abstract object Clone();
          void IDisposable.Dispose() { }
        }

        [ExplicitBaseTypeAndInterfacesTestCase("C0, ICloneable, IEquatable<C0>", TypeWithNamespace = false)]
        public abstract class C4 : C0, IEquatable<C0>, ICloneable {
          public abstract object Clone();
          public abstract bool Equals(C0 other);
        }

        [ExplicitBaseTypeAndInterfacesTestCase("List<int>", TypeWithNamespace = false)]
        public abstract class CList : List<int> {
        }

        [ExplicitBaseTypeAndInterfacesTestCase("System.IDisposable")]
        public abstract class X0 : IDisposable {
          public abstract void Dispose();
        }

        [ExplicitBaseTypeAndInterfacesTestCase("X0, ICloneable", TypeWithNamespace = false)]
        public abstract class X1 : X0, ICloneable {
          public abstract object Clone();
        }



        [ExplicitBaseTypeAndInterfacesTestCase("System.IDisposable")]
        public struct S0 : IDisposable {
          public void Dispose() { }
        }

        [ExplicitBaseTypeAndInterfacesTestCase("IEquatable<S0>", TypeWithNamespace = false)]
        public struct S1 : IEquatable<S0> {
          public bool Equals(S0 other) => throw new NotImplementedException();
        }

        [ExplicitBaseTypeAndInterfacesTestCase("IDisposable, IEquatable<S0>", TypeWithNamespace = false)]
        public struct S2 : IEquatable<S0>, IDisposable {
          public void Dispose() { }
          public bool Equals(S0 other) => throw new NotImplementedException();
        }


        [ExplicitBaseTypeAndInterfacesTestCase("", TypeWithNamespace = false)]
        public interface I { }

        [ExplicitBaseTypeAndInterfacesTestCase("I", TypeWithNamespace = false)]
        public interface II : I { }

        [ExplicitBaseTypeAndInterfacesTestCase("II", TypeWithNamespace = false)]
        public interface III : II { }


        [ExplicitBaseTypeAndInterfacesTestCase("System.IDisposable")]
        public interface I0 : IDisposable { }

        [ExplicitBaseTypeAndInterfacesTestCase("I0", TypeWithNamespace = false)]
        public interface I1 : I0 { }

        [ExplicitBaseTypeAndInterfacesTestCase("I1, ICloneable", TypeWithNamespace = false)]
        public interface I2 : I1, ICloneable { }

        // TODO: reimplemented interface? [BaseTypeTest("I1, ICloneable, IDisposable")]
        [ExplicitBaseTypeAndInterfacesTestCase("I1, ICloneable", TypeWithNamespace = false)]
        public interface I3 : I1, IDisposable, ICloneable { }

        [ExplicitBaseTypeAndInterfacesTestCase("I1, ICloneable, IEquatable<I0>", TypeWithNamespace = false)]
        public interface I4 : I1, IEquatable<I0>, ICloneable { }

        namespace NestedTypes {
          interface I { }

          class C {
            class CN {
              public interface INN { }
            }
            interface IN { }

            [ExplicitBaseTypeAndInterfacesTestCase("CN", TypeWithNamespace = false, TypeWithDeclaringTypeName = false)]
            [ExplicitBaseTypeAndInterfacesTestCase("C.CN", TypeWithNamespace = false, TypeWithDeclaringTypeName = true)]
            class C1 : CN { }

            [ExplicitBaseTypeAndInterfacesTestCase("CN, I, IN", TypeWithNamespace = false, TypeWithDeclaringTypeName = false)]
            [ExplicitBaseTypeAndInterfacesTestCase("C.CN, C.IN, I", TypeWithNamespace = false, TypeWithDeclaringTypeName = true)]
            class C2 : CN, I, IN { }

            [ExplicitBaseTypeAndInterfacesTestCase("CN, I, INN", TypeWithNamespace = false, TypeWithDeclaringTypeName = false)]
            [ExplicitBaseTypeAndInterfacesTestCase("C.CN, C.CN.INN, I", TypeWithNamespace = false, TypeWithDeclaringTypeName = true)]
            class C3 : CN, I, CN.INN { }
          }
        }
      }
    }
  }

  partial class GeneratorTests {
    [Test]
    public void TestGenerateExplicitBaseTypeAndInterfaces()
    {
      foreach (var type in FindTypes(t => t.FullName.Contains(".TestCases.ExplicitBaseTypeAndInterfaces."))) {
        foreach (var attr in type.GetCustomAttributes<ExplicitBaseTypeAndInterfacesTestCaseAttribute>()) {
          Assert.AreEqual(
            attr.Expected,
            string.Join(", ", Generator.GenerateExplicitBaseTypeAndInterfaces(type, null, GetGeneratorOptions(attr))),
            message: $"{attr.SourceLocation} ({type.FullName})"
          );
        }
      }
    }
  }
}
