// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
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
      [SkipTestCase("`unsafe struct` is not supported currently")]
      [TypeDeclarationTestCase("public unsafe struct Unsafe")]
      public unsafe struct Unsafe {
        public int* F;
      }

      [TypeDeclarationTestCase("public struct Unsafe_CurrentImplementation")]
      public unsafe struct Unsafe_CurrentImplementation {
        public int* F;
      }

      [SkipTestCase("`unsafe struct` is not supported currently")]
      [TypeDeclarationTestCase("public readonly unsafe struct ReadOnlyUnsafe")]
      public readonly unsafe struct ReadOnlyUnsafe {
        public readonly int* F;
      }

      [TypeDeclarationTestCase("public readonly struct ReadOnlyUnsafe_CurrentImplementation")]
      public readonly unsafe struct ReadOnlyUnsafe_CurrentImplementation {
        public readonly int* F;
      }
#pragma warning restore CS0649
    }
  }
}
