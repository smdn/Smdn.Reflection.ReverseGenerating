// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
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
  }
}
