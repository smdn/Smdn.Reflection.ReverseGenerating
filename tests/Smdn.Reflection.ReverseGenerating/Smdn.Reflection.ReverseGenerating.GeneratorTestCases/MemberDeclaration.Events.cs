// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CS0067

using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Events;

public class Options {
  [MemberDeclarationTestCase("public event System.EventHandler E;")]
  [MemberDeclarationTestCase("public event System.EventHandler E", MemberOmitEndOfStatement = true)]
  [MemberDeclarationTestCase("public event EventHandler Options.E;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public event System.EventHandler Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Events.Options.E;", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
  public event EventHandler E;
}

public class Modifiers {
  [MemberDeclarationTestCase("public event System.EventHandler E1;")] public event EventHandler E1;
  [MemberDeclarationTestCase("public static event System.EventHandler E2;")] public static event EventHandler E2;
  [MemberDeclarationTestCase("public static event System.EventHandler E3;")] static public event EventHandler E3;

  [MemberDeclarationTestCase("protected virtual event System.EventHandler EProtectedVirtual;")] protected virtual event EventHandler EProtectedVirtual;
}

public class ModifiersOverride : Modifiers {
  [MemberDeclarationTestCase("protected override event System.EventHandler EProtectedVirtual;")] protected override event EventHandler EProtectedVirtual;
}

public class ModifiersNew : Modifiers {
  [MemberDeclarationTestCase("new public event System.EventHandler E1;")] public new event EventHandler E1;

  [MemberDeclarationTestCase("new public event System.EventHandler EProtectedVirtual;")] public new event EventHandler EProtectedVirtual;
}

public class ModifiersNewVirtual : Modifiers {
  [MemberDeclarationTestCase("new public virtual event System.EventHandler EProtectedVirtual;")] public new virtual event EventHandler EProtectedVirtual;
}

public class Accessibilities {
  [MemberDeclarationTestCase("public event System.EventHandler E1;")] public event EventHandler E1;
  [MemberDeclarationTestCase("internal event System.EventHandler E2;")] internal event EventHandler E2;
  [MemberDeclarationTestCase("protected event System.EventHandler E3;")] protected event EventHandler E3;
  [MemberDeclarationTestCase("internal protected event System.EventHandler E4;")] protected internal event EventHandler E4;
  [MemberDeclarationTestCase("internal protected event System.EventHandler E5;")] internal protected event EventHandler E5;
  [MemberDeclarationTestCase("private protected System.EventHandler E6;")] private protected EventHandler E6;
  [MemberDeclarationTestCase("private protected System.EventHandler E7;")] protected private EventHandler E7;
  [MemberDeclarationTestCase("private event System.EventHandler E8;")] private event EventHandler E8;

  [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] internal event EventHandler E9;
  [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private protected event EventHandler E10;
  [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private event EventHandler E11;
}

public class Accessors {
  [MemberDeclarationTestCase("public event System.EventHandler CompilerGeneratedAccessor;", AccessorBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("public event System.EventHandler CompilerGeneratedAccessor;", AccessorBody = MethodBodyOption.EmptyImplementation)]
  [MemberDeclarationTestCase("public event System.EventHandler CompilerGeneratedAccessor", AccessorBody = MethodBodyOption.EmptyImplementation, MemberOmitEndOfStatement = true)]
  [MemberDeclarationTestCase("public event System.EventHandler CompilerGeneratedAccessor;", AccessorBody = MethodBodyOption.EmptyImplementation, MemberOmitEndOfStatement = false)]
  [MemberDeclarationTestCase("public event System.EventHandler CompilerGeneratedAccessor;", AccessorBody = MethodBodyOption.ThrowNotImplementedException)]
  [MemberDeclarationTestCase("public event System.EventHandler CompilerGeneratedAccessor;", AccessorBody = MethodBodyOption.ThrowNull)]
  public event EventHandler CompilerGeneratedAccessor;

  [MemberDeclarationTestCase("public event System.EventHandler CustomAccessor { add; remove; }", AccessorBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("public event System.EventHandler CustomAccessor { add; remove; }", AccessorBody = MethodBodyOption.EmptyImplementation)]
  [MemberDeclarationTestCase("public event System.EventHandler CustomAccessor { add; remove; }", AccessorBody = MethodBodyOption.EmptyImplementation, MemberOmitEndOfStatement = true)]
  [MemberDeclarationTestCase("public event System.EventHandler CustomAccessor { add; remove; }", AccessorBody = MethodBodyOption.EmptyImplementation, MemberOmitEndOfStatement = false)]
  [MemberDeclarationTestCase("public event System.EventHandler CustomAccessor { add => throw new NotImplementedException(); remove => throw new NotImplementedException(); }", AccessorBody = MethodBodyOption.ThrowNotImplementedException)]
  [MemberDeclarationTestCase("public event System.EventHandler CustomAccessor { add => throw null; remove => throw null; }", AccessorBody = MethodBodyOption.ThrowNull)]
  public event EventHandler CustomAccessor {
    add => throw new NotImplementedException();
    remove => throw new NotImplementedException();
  }
}
