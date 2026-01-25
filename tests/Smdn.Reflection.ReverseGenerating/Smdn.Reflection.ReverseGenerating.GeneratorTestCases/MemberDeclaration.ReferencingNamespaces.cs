// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cSpell:ignore browsable
#pragma warning disable CS0649, CS0067, CS8597

using System;
using System.Collections.Generic;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.ReferencingNamespaces;

public delegate void EventHandlerNotInSystemNamespace<T>(T arg);

class C {
  [ReferencingNamespacesTestCase("", TranslateLanguagePrimitiveTypeDeclaration = true)]
  [ReferencingNamespacesTestCase("System", TranslateLanguagePrimitiveTypeDeclaration = false)]
  public int F0;

  [ReferencingNamespacesTestCase("", TranslateLanguagePrimitiveTypeDeclaration = true)]
  [ReferencingNamespacesTestCase("System", TranslateLanguagePrimitiveTypeDeclaration = false)]
  public (int, int) F1;

  [ReferencingNamespacesTestCase("")] public int? F2;

  [ReferencingNamespacesTestCase("System", TranslateLanguagePrimitiveTypeDeclaration = true)]
  [ReferencingNamespacesTestCase("System", TranslateLanguagePrimitiveTypeDeclaration = false)]
  public Guid F3;

  [ReferencingNamespacesTestCase("System.Collections.Generic", TranslateLanguagePrimitiveTypeDeclaration = true)]
  [ReferencingNamespacesTestCase("System, System.Collections.Generic", TranslateLanguagePrimitiveTypeDeclaration = false)]
  public List<int> F4;

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
    "Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.ReferencingNamespaces",
    TranslateLanguagePrimitiveTypeDeclaration = true
  )]
  [ReferencingNamespacesTestCase(
    "System, Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.ReferencingNamespaces",
    TranslateLanguagePrimitiveTypeDeclaration = false
  )]
  public event EventHandlerNotInSystemNamespace<int> E3 {
    add => throw null;
    remove => throw null;
  }

  [ReferencingNamespacesTestCase(
    // System.DiagnosticsDebuggerBrowsableAttribute
    // System.Runtime.CompilerServices.CompilerGeneratedAttribute
    "System.Diagnostics, System.Runtime.CompilerServices",
    TranslateLanguagePrimitiveTypeDeclaration = true
  )]
  [ReferencingNamespacesTestCase(
    // System.DiagnosticsDebuggerBrowsableAttribute
    // System.Runtime.CompilerServices.CompilerGeneratedAttribute
    "System, System.Diagnostics, System.Runtime.CompilerServices",
    TranslateLanguagePrimitiveTypeDeclaration = false
  )]
  public int P0 { get; set; }

  [ReferencingNamespacesTestCase("")] public int P1 { get => throw null; set => throw null; }
  [ReferencingNamespacesTestCase("")] public int? P2 { get => throw null; set => throw null; }
  [ReferencingNamespacesTestCase("System")] public Guid P3 { get => throw null; set => throw null; }
  [ReferencingNamespacesTestCase("System.Collections.Generic")] public IList<int> P4 { get => throw null; set => throw null; }
  [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public IList<Guid> P5 { get => throw null; set => throw null; }

  [ReferencingNamespacesTestCase("System.IO", TranslateLanguagePrimitiveTypeDeclaration = true)]
  [ReferencingNamespacesTestCase("System, System.IO", TranslateLanguagePrimitiveTypeDeclaration = false)]
  public System.IO.Stream this[int x] {
    get => throw new NotImplementedException();
    set => throw new NotImplementedException();
  }

  [ReferencingNamespacesTestCase("")] public void M0() {}

  [ReferencingNamespacesTestCase("", TranslateLanguagePrimitiveTypeDeclaration = true)]
  [ReferencingNamespacesTestCase("System", TranslateLanguagePrimitiveTypeDeclaration = false)]
  public void M1(int x) {}

  [ReferencingNamespacesTestCase("")] public void M2(int? x) {}
  [ReferencingNamespacesTestCase("System")] public void M3(Guid x) {}
  [ReferencingNamespacesTestCase("System.Collections.Generic")] public void M4(List<int> x) {}
  [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public void M5(List<Guid> x) {}
  [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public void M6(Guid x, List<int> y) {}
  [ReferencingNamespacesTestCase("")] public unsafe void M7(int* x) {}

  [ReferencingNamespacesTestCase("System")] public void ParameterValueTupleSingleElement(ValueTuple<int> x) {}
  [ReferencingNamespacesTestCase("")] public void ParameterValueTupleTranslated((int, int) x) {}
  [ReferencingNamespacesTestCase("System.Runtime.CompilerServices")] public void ParameterValueTupleWithElementNames((int X0, int X1) x) {} // includes namespace of TupleElementNamesAttribute
  [ReferencingNamespacesTestCase("System, System.Collections.Generic, System.Runtime.CompilerServices")] public void ParameterValueTupleNestedTypeWithElementNames((Guid X0, List<int> X1) x) {} // includes namespace of TupleElementNamesAttribute

  [ReferencingNamespacesTestCase("", TranslateLanguagePrimitiveTypeDeclaration = true)]
  [ReferencingNamespacesTestCase("System", TranslateLanguagePrimitiveTypeDeclaration = false)]
  public int M1() => throw new NotImplementedException();

  [ReferencingNamespacesTestCase("")] public int? M2() => throw new NotImplementedException();
  [ReferencingNamespacesTestCase("System")] public Guid M3() => throw new NotImplementedException();
  [ReferencingNamespacesTestCase("System.Collections.Generic")] public List<int> M4() => throw new NotImplementedException();
  [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public List<Guid> M5() => throw new NotImplementedException();
  [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public (Guid, List<int>) M6() => throw new NotImplementedException();

  [ReferencingNamespacesTestCase("System")] public ValueTuple<int> ReturnParameterValueTupleSingleElement() => throw new NotImplementedException();
  [ReferencingNamespacesTestCase("")] public (int, int) ReturnParameterValueTupleTranslated() => throw new NotImplementedException();
  [ReferencingNamespacesTestCase("System.Runtime.CompilerServices")] public (int X0, int X1) ReturnParameterValueTupleWithElementNames() => throw new NotImplementedException(); // includes namespace of TupleElementNamesAttribute
  [ReferencingNamespacesTestCase("System, System.Collections.Generic, System.Runtime.CompilerServices")] public (Guid X0, List<int> X1) ParameterValueTupleNestedTypeWithElementNames() => throw new NotImplementedException(); // includes namespace of TupleElementNamesAttribute

#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
#nullable enable
  [ReferencingNamespacesTestCase("")]
  public bool ParameterWithNoAttributes(string s) => throw new NotImplementedException();

  [ReferencingNamespacesTestCase("System.Diagnostics.CodeAnalysis")]
  public bool ParameterWithNotNullWhenAttribute([NotNullWhen(false)] string? s) => throw new NotImplementedException();

  [ReferencingNamespacesTestCase("")]
  public string ReturnParameterWithNoAttributes() => throw new NotImplementedException();

  [ReferencingNamespacesTestCase("System.Diagnostics.CodeAnalysis")]
  [return: NotNullIfNotNull(nameof(s))]
  public string? ReturnParameterWithNotNullIfNotNullAttribute(string? s) => throw new NotImplementedException();
#nullable restore
#endif
}
