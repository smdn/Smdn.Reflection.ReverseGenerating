// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.Delegates {
  [TypeDeclarationTestCase("internal delegate void D0();")] internal delegate void D0();
  [TypeDeclarationTestCase("public delegate void D1();")] public delegate void D1();
  [TypeDeclarationTestCase("public delegate void D2(int x);")] public delegate void D2(int x);
  [TypeDeclarationTestCase("public delegate int D3();")] public delegate int D3();
  [TypeDeclarationTestCase("public delegate int D4(int x);")] public delegate int D4(int x);
  [TypeDeclarationTestCase("public delegate int D5(int x, int y);")] public delegate int D5(int x, int y);

  [TypeDeclarationTestCase("public delegate Guid D61();", TypeWithNamespace = false)] public delegate Guid D61();
  [TypeDeclarationTestCase("public delegate System.Guid D62();", TypeWithNamespace = true)] public delegate Guid D62();

  [TypeDeclarationTestCase("public unsafe delegate void Unsafe(int* x);")] public unsafe delegate void Unsafe(int* x);

  class Accessibilities {
    [TypeDeclarationTestCase("public delegate void D1();")] public delegate void D1();
    [TypeDeclarationTestCase("internal delegate void D2();")] internal delegate void D2();
    [TypeDeclarationTestCase("protected delegate void D3();")] protected delegate void D3();
    [TypeDeclarationTestCase("internal protected delegate void D4();")] protected internal delegate void D4();
    [TypeDeclarationTestCase("internal protected delegate void D5();")] internal protected delegate void D5();
    [TypeDeclarationTestCase("private protected delegate void D6();")] private protected delegate void D6();
    [TypeDeclarationTestCase("private protected delegate void D7();")] protected private delegate void D7();
    [TypeDeclarationTestCase("private delegate void D8();")] private delegate void D8();
  }

  class ModifierNew : Accessibilities {
    [TypeDeclarationTestCase("new public delegate int D3(int x);")] new public delegate int D3(int x);
  }

  namespace DelegateClasses {
#if false
    // CS0644
    [TypeDeclarationTestCase("public class CMulticastDelegate")] public class CMulticastDelegate : MulticastDelegate { }

    // CS0644
    [TypeDeclarationTestCase("public class CDelegate")] public class CDelegate : Delegate {}
#endif
  }
}
