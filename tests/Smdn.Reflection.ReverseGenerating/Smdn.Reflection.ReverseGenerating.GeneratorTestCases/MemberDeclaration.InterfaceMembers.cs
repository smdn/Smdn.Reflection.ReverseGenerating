// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CS0067

using System;
using System.Collections.Generic;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers;

public interface I {
  [MemberDeclarationTestCase("int this[int index] { get; set; }")] int this[int index] { get; set; }
  [MemberDeclarationTestCase("int P1 { get; set; }")] int P1 { get; set; }
  [MemberDeclarationTestCase("int P2 { get; }")] int P2 { get; }
  [MemberDeclarationTestCase("int P3 { set; }")] int P3 { set; }
  [MemberDeclarationTestCase("event System.EventHandler E;")] event EventHandler E;
  [MemberDeclarationTestCase("void M();")] void M();
  [MemberDeclarationTestCase("int M(int x);")] int M(int x);
}

class ImplicitMethod : IDisposable, ICloneable {
  [MemberDeclarationTestCase("public void Dispose() {}")]
  [MemberDeclarationTestCase("public void ImplicitMethod.Dispose()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("public void Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ImplicitMethod.Dispose()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
  public void Dispose() { }

  [MemberDeclarationTestCase("public object Clone() {}", MemberWithNamespace = false)]
  public object Clone() => throw new NotImplementedException();
}

class ExplicitMethod : IDisposable, ICloneable {
  [MemberDeclarationTestCase("void System.IDisposable.Dispose() {}", MemberWithNamespace = true)]
  [MemberDeclarationTestCase("void IDisposable.Dispose() {}", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("void ExplicitMethod.IDisposable.Dispose()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("void Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ExplicitMethod.System.IDisposable.Dispose()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
  void IDisposable.Dispose() { }

  [MemberDeclarationTestCase("object ICloneable.Clone() {}", MemberWithNamespace = false)]
  object ICloneable.Clone() => throw new NotImplementedException();
}

class ExplicitMethodGenericInterface : IEnumerable<int> {
  [MemberDeclarationTestCase("System.Collections.Generic.IEnumerator<int> System.Collections.Generic.IEnumerable<int>.GetEnumerator() {}")]
  IEnumerator<int> System.Collections.Generic.IEnumerable<int>.GetEnumerator() => throw new NotImplementedException();
  System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new NotImplementedException();
}

interface IProperty {
  int P1 { get; set; }
  int P2 { get; }
  int P3 { set; }
}

class ImplicitProperty1 : IProperty {
  [MemberDeclarationTestCase("public int P1 { get; set; }", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public int ImplicitProperty1.P1 { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("public int ImplicitProperty1.P1 { get => throw null; set => throw null; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.ThrowNull)]
  [MemberDeclarationTestCase("public int Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ImplicitProperty1.P1 { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, AccessorBody = MethodBodyOption.None)]
  public int P1 { get; set; }

  [MemberDeclarationTestCase("public int P2 { get; }", MemberWithNamespace = false)]
  public int P2 { get { throw new NotImplementedException(); } }

  [MemberDeclarationTestCase("public int P3 { set; }", MemberWithNamespace = false)]
  public int P3 { set { throw new NotImplementedException(); } }
}

class ExplicitProperty1 : IProperty {
  [MemberDeclarationTestCase("int IProperty.P1 { get; set; }", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("int ExplicitProperty1.IProperty.P1 { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("int ExplicitProperty1.IProperty.P1 { get => throw null; set => throw null; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.ThrowNull)]
  [MemberDeclarationTestCase("int Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ExplicitProperty1.Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.IProperty.P1 { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
  int IProperty.P1 { get; set; }

  [MemberDeclarationTestCase("int IProperty.P2 { get; }", MemberWithNamespace = false)]
  int IProperty.P2 { get { throw new NotImplementedException(); } }

  [MemberDeclarationTestCase("int IProperty.P3 { set; }", MemberWithNamespace = false)]
  int IProperty.P3 { set { throw new NotImplementedException(); } }
}

class ImplicitProperty2 : IAsyncResult {
  [MemberDeclarationTestCase("public object AsyncState { get; }")]
  public object AsyncState { get => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("public System.Threading.WaitHandle AsyncWaitHandle { get; }")]
  public System.Threading.WaitHandle AsyncWaitHandle { get => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("public bool CompletedSynchronously { get; }")]
  public bool CompletedSynchronously { get => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("public bool IsCompleted { get; }")]
  public bool IsCompleted { get => throw new NotImplementedException(); }
}

class ExplicitProperty2 : IAsyncResult {
  [MemberDeclarationTestCase("object System.IAsyncResult.AsyncState { get; }")]
  object IAsyncResult.AsyncState { get => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("System.Threading.WaitHandle System.IAsyncResult.AsyncWaitHandle { get; }")]
  System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle { get => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("bool System.IAsyncResult.CompletedSynchronously { get; }")]
  bool IAsyncResult.CompletedSynchronously { get => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("bool System.IAsyncResult.IsCompleted { get; }")]
  bool IAsyncResult.IsCompleted { get => throw new NotImplementedException(); }
}

class ExplicitPropertyGenericInterface : IReadOnlyCollection<string> {
  [MemberDeclarationTestCase("int System.Collections.Generic.IReadOnlyCollection<string>.Count { get; }")]
  int IReadOnlyCollection<string>.Count { get => throw new NotImplementedException(); }

  public IEnumerator<string> GetEnumerator() => throw new NotImplementedException();
  System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new NotImplementedException();
}

class ExplicitProperty2WithoutNamespace : IAsyncResult {
  [MemberDeclarationTestCase("object IAsyncResult.AsyncState { get; }", MemberWithNamespace = false)]
  object IAsyncResult.AsyncState { get => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("WaitHandle IAsyncResult.AsyncWaitHandle { get; }", MemberWithNamespace = false)]
  System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle { get => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("bool IAsyncResult.CompletedSynchronously { get; }", MemberWithNamespace = false)]
  bool IAsyncResult.CompletedSynchronously { get => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("bool IAsyncResult.IsCompleted { get; }", MemberWithNamespace = false)]
  bool IAsyncResult.IsCompleted { get => throw new NotImplementedException(); }
}

interface IEvent {
  [MemberDeclarationTestCase("event EventHandler E;", MemberWithNamespace = false)]
  event EventHandler E;
}

class ImplicitEvent : IEvent {
  [MemberDeclarationTestCase("public event EventHandler E;", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public event EventHandler E", MemberWithNamespace = false, MemberOmitEndOfStatement = true)]
  [MemberDeclarationTestCase("public event EventHandler ImplicitEvent.E;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public event System.EventHandler Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ImplicitEvent.E;", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
  public event EventHandler E;
}

class ExplicitEvent : IEvent {
  [MemberDeclarationTestCase("event EventHandler IEvent.E { add; remove; }", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("event EventHandler IEvent.E { add; remove; }", MemberWithNamespace = false, MemberOmitEndOfStatement = true)]
  [MemberDeclarationTestCase("event EventHandler ExplicitEvent.IEvent.E { add; remove; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("event System.EventHandler Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ExplicitEvent.Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.IEvent.E { add; remove; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
  event EventHandler IEvent.E {
    add { throw new NotImplementedException(); }
    remove { throw new NotImplementedException(); }
  }
}
