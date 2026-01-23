// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cSpell:ignore accessibilities,nullabilities
using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.Structures {
  [TypeDeclarationTestCase("internal struct S0")] internal struct S0 { };
  [TypeDeclarationTestCase("public struct S1")] public struct S1 { };

  class Accessibilities {
    [TypeDeclarationTestCase("public struct S1")] public struct S1 { }
    [TypeDeclarationTestCase("internal struct S2")] internal struct S2 { }
    [TypeDeclarationTestCase("protected struct S3")] protected struct S3 { }
    [TypeDeclarationTestCase("internal protected struct S4")] protected internal struct S4 { }
    [TypeDeclarationTestCase("internal protected struct S5")] internal protected struct S5 { }
    [TypeDeclarationTestCase("private protected struct S6")] private protected struct S6 { }
    [TypeDeclarationTestCase("private protected struct S7")] protected private struct S7 { }
    [TypeDeclarationTestCase("private struct S8")] private struct S8 { }
  }

  namespace Modifiers {
    class New : Accessibilities {
      [TypeDeclarationTestCase("new public struct S3")] new public struct S3 { }
    }

    namespace Ref {
      [TypeDeclarationTestCase("public ref struct S0")] public ref struct S0 { }
    }

    namespace ReadOnly {
      [TypeDeclarationTestCase("public readonly struct S1")] public readonly struct S1 { }
      [TypeDeclarationTestCase("public readonly ref struct S2")] public readonly ref struct S2 { }
    }

    namespace Unsafe {
#pragma warning disable CS0649
      [TypeDeclarationTestCase("public unsafe struct UnsafeStructPublicPointerField", TypeDetectUnsafe = true)]
      [TypeDeclarationTestCase("public struct UnsafeStructPublicPointerField", TypeDetectUnsafe = false)]
      public unsafe struct UnsafeStructPublicPointerField {
        public int* F;
      }

      [TypeDeclarationTestCase("public unsafe struct UnsafeStructPrivatePointerField", TypeDetectUnsafe = true)]
      [TypeDeclarationTestCase("public struct UnsafeStructPrivatePointerField", TypeDetectUnsafe = false)]
      public unsafe struct UnsafeStructPrivatePointerField {
#pragma warning disable CS0169
        private int* F;
#pragma warning restore CS0169
      }

      [SkipTestCase("`fixed` field is not supported currently")]
      [TypeDeclarationTestCase("public unsafe struct UnsafeStructFixedField", TypeDetectUnsafe = true)]
      [TypeDeclarationTestCase("public struct UnsafeStructFixedField", TypeDetectUnsafe = false)]
      public unsafe struct UnsafeStructFixedField {
        public fixed int F[1];
      }

      [TypeDeclarationTestCase("public readonly unsafe struct ReadOnlyUnsafeStructPointerField", TypeDetectUnsafe = true)]
      [TypeDeclarationTestCase("public readonly struct ReadOnlyUnsafeStructPointerField", TypeDetectUnsafe = false)]
      public readonly unsafe struct ReadOnlyUnsafeStructPointerField {
        public readonly int* F;
      }

#if false // CS0106
      [TypeDeclarationTestCase("public readonly unsafe struct ReadOnlyUnsafeStructFixedField", TypeDetectUnsafe = true)]
      public readonly unsafe struct ReadOnlyUnsafeStructFixedField {
        public readonly fixed int F[1];
      }
#endif

      [TypeDeclarationTestCase("public struct StructWithNoPointerFields", TypeDetectUnsafe = true)]
      [TypeDeclarationTestCase("public struct StructWithNoPointerFields", TypeDetectUnsafe = false)]
      public /*safe*/ struct StructWithNoPointerFields {
        public unsafe int* P {
          get => throw new NotImplementedException();
          set => throw new NotImplementedException();
        }

        public unsafe void M(int* p) => throw new NotImplementedException();
      }

      [TypeDeclarationTestCase("public struct UnsafeStructWithNoPointerFields", TypeDetectUnsafe = true)]
      [TypeDeclarationTestCase("public struct UnsafeStructWithNoPointerFields", TypeDetectUnsafe = false)]
      public unsafe struct UnsafeStructWithNoPointerFields {
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
