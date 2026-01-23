// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cSpell:ignore accessibilities,nullabilities
using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.Classes {
  [TypeDeclarationTestCase("internal class C0")] internal class C0 { };
  [TypeDeclarationTestCase("public class C1")] public class C1 { };

  class Accessibilities {
    [TypeDeclarationTestCase("public class C1")] public class C1 { }
    [TypeDeclarationTestCase("internal class C2")] internal class C2 { }
    [TypeDeclarationTestCase("protected class C3")] protected class C3 { }
    [TypeDeclarationTestCase("internal protected class C4")] protected internal class C4 { }
    [TypeDeclarationTestCase("internal protected class C5")] internal protected class C5 { }
    [TypeDeclarationTestCase("private protected class C6")] private protected class C6 { }
    [TypeDeclarationTestCase("private protected class C7")] protected private class C7 { }
    [TypeDeclarationTestCase("private class C8")] private class C8 { }
  }

  namespace Modifiers {
    [TypeDeclarationTestCase("public abstract class C1")] public abstract class C1 { }
    [TypeDeclarationTestCase("public static class C2")] public static class C2 { }
    [TypeDeclarationTestCase("public sealed class C3")] public sealed class C3 { }

    namespace NewModifier {
      class C {
        class C0 {
          protected class CProtected { }
          private protected class CPrivateProtected { }
          internal protected class CInternalProtected { }
          public class CPublic { }

          public struct S { }
          public enum E : int { }
          public delegate void D();
          public interface I { }
        }

        class C1 : C0 {
          [TypeDeclarationTestCase("new protected class CProtected")] protected new class CProtected { }
          [TypeDeclarationTestCase("new private protected class CPrivateProtected")] private protected new class CPrivateProtected { }
          [TypeDeclarationTestCase("new internal protected class CInternalProtected")] internal protected new class CInternalProtected { }
          [TypeDeclarationTestCase("new public class CPublic")] public new class CPublic { }

          [TypeDeclarationTestCase("new public readonly struct S")] public new readonly struct S { }
          [TypeDeclarationTestCase("new public enum E : short")] public new enum E : short { }
          [TypeDeclarationTestCase("new public delegate int D(int x);")] public new delegate int D(int x);
          [TypeDeclarationTestCase("new public interface I")] public new interface I { }
        }

        class C2 : C0 {
          [TypeDeclarationTestCase("new protected class CProtected")] new protected class CProtected { }
          [TypeDeclarationTestCase("new private protected class CPrivateProtected")] new private protected class CPrivateProtected { }
          [TypeDeclarationTestCase("new internal protected class CInternalProtected")] new internal protected class CInternalProtected { }
          [TypeDeclarationTestCase("new public class CPublic")] new public class CPublic { }
        }

        class C00 : C0 { }
        class C3 : C00 {
          [TypeDeclarationTestCase("new protected class CProtected")] new protected class CProtected { }
          [TypeDeclarationTestCase("new public class CPublic")] new public class CPublic { }
        }
      }
    }

    namespace Unsafe {
#pragma warning disable CS0649
      [TypeDeclarationTestCase("public class UnsafeClassPublicPointerField", TypeDetectUnsafe = false)]
      [TypeDeclarationTestCase("public unsafe class UnsafeClassPublicPointerField", TypeDetectUnsafe = true)]
      public unsafe class UnsafeClassPublicPointerField {
        public int* F;
      }

      [TypeDeclarationTestCase("public class UnsafeClassProtectedPointerField", TypeDetectUnsafe = false)]
      [TypeDeclarationTestCase("public unsafe class UnsafeClassProtectedPointerField", TypeDetectUnsafe = true)]
      public unsafe class UnsafeClassProtectedPointerField {
        protected int* F;
      }

      [TypeDeclarationTestCase("public class UnsafeClassPrivatePointerField", TypeDetectUnsafe = false)]
      [TypeDeclarationTestCase("public unsafe class UnsafeClassPrivatePointerField", TypeDetectUnsafe = true)]
      public unsafe class UnsafeClassPrivatePointerField {
#pragma warning disable CS0169
        private int* F;
#pragma warning restore CS0169
      }

      [TypeDeclarationTestCase("public class SafeClassInheritsUnsafeClass", TypeDetectUnsafe = false)]
      [TypeDeclarationTestCase("public class SafeClassInheritsUnsafeClass", TypeDetectUnsafe = true)]
      public /*safe*/ class SafeClassInheritsUnsafeClass : UnsafeClassPublicPointerField { }

      [TypeDeclarationTestCase("public class UnsafeClassWithStaticPointerField", TypeDetectUnsafe = false)]
      [TypeDeclarationTestCase("public unsafe class UnsafeClassWithStaticPointerField", TypeDetectUnsafe = true)]
      public unsafe class UnsafeClassWithStaticPointerField {
        public static int* F;
      }

      [TypeDeclarationTestCase("public abstract class UnsafeAbstractClass", TypeDetectUnsafe = false)]
      [TypeDeclarationTestCase("public abstract unsafe class UnsafeAbstractClass", TypeDetectUnsafe = true)]
      public unsafe abstract class UnsafeAbstractClass {
        public int* F;
      }

      [TypeDeclarationTestCase("public sealed class UnsafeSealedClass", TypeDetectUnsafe = false)]
      [TypeDeclarationTestCase("public sealed unsafe class UnsafeSealedClass", TypeDetectUnsafe = true)]
      public unsafe sealed class UnsafeSealedClass {
        public int* F;
      }

      [TypeDeclarationTestCase("public static class UnsafeStaticClass", TypeDetectUnsafe = false)]
      [TypeDeclarationTestCase("public static unsafe class UnsafeStaticClass", TypeDetectUnsafe = true)]
      public unsafe static class UnsafeStaticClass {
        public static int* F;
      }

      [TypeDeclarationTestCase("public class ClassWithNoPointerFields", TypeDetectUnsafe = false)]
      [TypeDeclarationTestCase("public class ClassWithNoPointerFields", TypeDetectUnsafe = true)]
      public /*safe*/ class ClassWithNoPointerFields {
        public unsafe int* P {
          get => throw new NotImplementedException();
          set => throw new NotImplementedException();
        }

        public unsafe void M(int* p) => throw new NotImplementedException();
      }

      [TypeDeclarationTestCase("public class UnsafeClassWithNoPointerFields", TypeDetectUnsafe = false)]
      [TypeDeclarationTestCase("public class UnsafeClassWithNoPointerFields", TypeDetectUnsafe = true)]
      public unsafe class UnsafeClassWithNoPointerFields {
        public unsafe int* P {
          get => throw new NotImplementedException();
          set => throw new NotImplementedException();
        }

        public unsafe void M(int* p) => throw new NotImplementedException();
      }
#pragma warning restore CS0649
    }
  }
}
