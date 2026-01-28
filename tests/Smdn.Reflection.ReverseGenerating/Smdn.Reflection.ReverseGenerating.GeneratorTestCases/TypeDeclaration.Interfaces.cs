// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cSpell:ignore accessibilities,nullabilities
namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.Interfaces {
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

  class ModifierNew : Accessibilities {
    [TypeDeclarationTestCase("new public interface I3")] new public interface I3 { }
  }

  namespace Unsafe {
    [TypeDeclarationTestCase("public interface InterfaceWithNoPointerFields", TypeDetectUnsafe = true)]
    [TypeDeclarationTestCase("public interface InterfaceWithNoPointerFields", TypeDetectUnsafe = false)]
    public /*safe*/ interface InterfaceWithNoPointerFields {
      unsafe int* P { get; set; }
      unsafe void M(int* p);
    }

    [TypeDeclarationTestCase("public interface UnsafeInterfaceWithNoPointerFields", TypeDetectUnsafe = true)]
    [TypeDeclarationTestCase("public interface UnsafeInterfaceWithNoPointerFields", TypeDetectUnsafe = false)]
    public unsafe interface UnsafeInterfaceWithNoPointerFields {
      unsafe int* P { get; set; }
      unsafe void M(int* p);
    }
  }

  namespace GenericParameterVariance {
    [TypeDeclarationTestCase($"public interface ICovariant<out R>")]
    public interface ICovariant<out R> { }

    [TypeDeclarationTestCase("public interface IContravariant<in A>")]
    public interface IContravariant<in A> { }

    [TypeDeclarationTestCase("public interface IVariant<out R, in A>")]
    public interface IVariant<out R, in A> { }

    namespace ExtendingCovariant {
      [TypeDeclarationTestCase($"public interface IExtendedInvariant<T>")]
      public interface IExtendedInvariant<T> : ICovariant<T> { }

      [TypeDeclarationTestCase($"public interface IExtendedCovariant<out T>")]
      public interface IExtendedCovariant<out T> : ICovariant<T> { }
    }

    namespace ExtendingContravariant {
      [TypeDeclarationTestCase($"public interface IExtendedInvariant<T>")]
      public interface IExtendedInvariant<T> : IContravariant<T> { }
    }
  }
}
