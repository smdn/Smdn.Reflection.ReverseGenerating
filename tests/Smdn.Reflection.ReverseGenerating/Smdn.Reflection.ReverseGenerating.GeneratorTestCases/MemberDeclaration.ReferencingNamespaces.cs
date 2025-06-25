// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cSpell:ignore browsable
#pragma warning disable CS0649, CS0067, CS8597

using System;
using System.Collections.Generic;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.ReferencingNamespaces;

class C {
  [ReferencingNamespacesTestCase("", TranslateLanguagePrimitiveTypeDeclaration = true)] public int F0;
  [SkipTestCase("TODO: consider TranslateLanguagePrimitive")][ReferencingNamespacesTestCase("System", TranslateLanguagePrimitiveTypeDeclaration = false)] public int F1;
  [ReferencingNamespacesTestCase("")] public int? F2;
  [ReferencingNamespacesTestCase("System")] public Guid F3;
  [ReferencingNamespacesTestCase("System.Collections.Generic")] public List<int> F4;
  [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public List<Guid> F5;
  [ReferencingNamespacesTestCase("")] public int[] F6;
  [ReferencingNamespacesTestCase("")] public Nullable<int> F7;
  [ReferencingNamespacesTestCase("System")] public Action<int> F8;
  [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public List<Action<int>> F9;
  [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public Action<List<int>> F10;
  [ReferencingNamespacesTestCase("System, System.Collections.Generic, System.Collections.ObjectModel")] public System.Collections.ObjectModel.Collection<List<Action<int>>> F11;
  [ReferencingNamespacesTestCase("System, System.Collections.Generic, System.Collections.ObjectModel")] public Action<System.Collections.ObjectModel.Collection<List<int>>> F12;

  [ReferencingNamespacesTestCase(
    // System.EventHandler
    // System.DiagnosticsDebuggerBrowsableAttribute
    // System.Runtime.CompilerServices.CompilerGeneratedAttribute
    "System, System.Diagnostics, System.Runtime.CompilerServices"
  )]
  public event EventHandler E0;

  [ReferencingNamespacesTestCase("System")] public event EventHandler E1 { add => throw null; remove => throw null; }
  [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public event EventHandler<IList<int>> E2 { add => throw null; remove => throw null; }

  [ReferencingNamespacesTestCase(
    // System.DiagnosticsDebuggerBrowsableAttribute
    // System.Runtime.CompilerServices.CompilerGeneratedAttribute
    "System.Diagnostics, System.Runtime.CompilerServices"
  )]
  public int P0 { get; set; }

  [ReferencingNamespacesTestCase("")] public int P1 { get => throw null; set => throw null; }
  [ReferencingNamespacesTestCase("")] public int? P2 { get => throw null; set => throw null; }
  [ReferencingNamespacesTestCase("System")] public Guid P3 { get => throw null; set => throw null; }
  [ReferencingNamespacesTestCase("System.Collections.Generic")] public IList<int> P4 { get => throw null; set => throw null; }
  [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public IList<Guid> P5 { get => throw null; set => throw null; }

  [ReferencingNamespacesTestCase("")] public void M0() {}
  [ReferencingNamespacesTestCase("")] public void M1(int x) {}
  [ReferencingNamespacesTestCase("")] public void M2(int? x) {}
  [ReferencingNamespacesTestCase("System")] public void M3(Guid x) {}
  [ReferencingNamespacesTestCase("System.Collections.Generic")] public void M4(List<int> x) {}
  [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public void M5(List<Guid> x) {}
  [ReferencingNamespacesTestCase("")] public unsafe void M6(int* x) {}

  [ReferencingNamespacesTestCase("")] public int M1() => throw new NotImplementedException();
  [ReferencingNamespacesTestCase("")] public int? M2() => throw new NotImplementedException();
  [ReferencingNamespacesTestCase("System")] public Guid M3() => throw new NotImplementedException();
  [ReferencingNamespacesTestCase("System.Collections.Generic")] public List<int> M4() => throw new NotImplementedException();
  [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public List<Guid> M5() => throw new NotImplementedException();
}
