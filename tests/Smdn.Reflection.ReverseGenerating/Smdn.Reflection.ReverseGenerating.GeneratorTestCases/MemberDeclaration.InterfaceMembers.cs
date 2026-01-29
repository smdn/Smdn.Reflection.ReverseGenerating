// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CS0067

using System;
using System.Collections.Generic;
using System.IO;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers;

public class C {
  public interface INestedWithMethod {
    void M();
  }

  public interface INestedWithProperty {
    int P { get; }
  }

  public interface INestedWithEvent {
    event EventHandler E;
  }
}

public interface I {
  [MemberDeclarationTestCase("int this[int index] { get; set; }")] int this[int index] { get; set; }
  [MemberDeclarationTestCase("int P1 { get; set; }")] int P1 { get; set; }
  [MemberDeclarationTestCase("int P2 { get; }")] int P2 { get; }
  [MemberDeclarationTestCase("int P3 { set; }")] int P3 { set; }
  [MemberDeclarationTestCase("event System.EventHandler E;")] event EventHandler E;
  [MemberDeclarationTestCase("void M();")] void M();
  [MemberDeclarationTestCase("int M(int x);")] int M(int x);
}

class ImplicitMethod : IDisposable, ICloneable, C.INestedWithMethod {
  [MemberDeclarationTestCase("public void Dispose() {}")]
  [MemberDeclarationTestCase("public void ImplicitMethod.Dispose()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("public void Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ImplicitMethod.Dispose()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
  public void Dispose() { }

  [MemberDeclarationTestCase("public object Clone() {}", MemberWithNamespace = false)]
  public object Clone() => throw new NotImplementedException();

  [MemberDeclarationTestCase("public void M() {}")]
  [MemberDeclarationTestCase("public void ImplicitMethod.M()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("public void Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ImplicitMethod.M()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
  public void M() => throw new NotImplementedException();
}

class ExplicitMethod : IDisposable, ICloneable, C.INestedWithMethod {
  [MemberDeclarationTestCase("void System.IDisposable.Dispose() {}", MemberWithNamespace = true)]
  [MemberDeclarationTestCase("void IDisposable.Dispose() {}", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("void ExplicitMethod.IDisposable.Dispose()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("void Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ExplicitMethod.System.IDisposable.Dispose()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
  void IDisposable.Dispose() { }

  [MemberDeclarationTestCase("object ICloneable.Clone() {}", MemberWithNamespace = false)]
  object ICloneable.Clone() => throw new NotImplementedException();

  [MemberDeclarationTestCase("void C.INestedWithMethod.M() {}", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("void ExplicitMethod.C.INestedWithMethod.M()", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, MethodBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("void Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ExplicitMethod.Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.C.INestedWithMethod.M()", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
  void C.INestedWithMethod.M() => throw new NotImplementedException();
}

class ExplicitMethodGenericInterface : IEnumerable<int> {
  [MemberDeclarationTestCase("System.Collections.Generic.IEnumerator<int> System.Collections.Generic.IEnumerable<int>.GetEnumerator() {}")]
  IEnumerator<int> System.Collections.Generic.IEnumerable<int>.GetEnumerator() => throw new NotImplementedException();

  [MemberDeclarationTestCase("System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {}")]
  System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new NotImplementedException();
}

interface IProperty {
  int P1 { get; set; }
  int P2 { get; }
  int P3 { set; }
  int P4 { init; }
}

class ImplicitProperty1 : IProperty, C.INestedWithProperty {
  [MemberDeclarationTestCase("public int P1 { get; set; }", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public int ImplicitProperty1.P1 { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("public int ImplicitProperty1.P1 { get => throw null; set => throw null; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.ThrowNull)]
  [MemberDeclarationTestCase("public int Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ImplicitProperty1.P1 { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, AccessorBody = MethodBodyOption.None)]
  public int P1 { get; set; }

  [MemberDeclarationTestCase("public int P2 { get; }", MemberWithNamespace = false)]
  public int P2 { get { throw new NotImplementedException(); } }

  [MemberDeclarationTestCase("public int P3 { set; }", MemberWithNamespace = false)]
  public int P3 { set { throw new NotImplementedException(); } }

  [MemberDeclarationTestCase("public int P4 { init; }", MemberWithNamespace = false)]
  public int P4 { init { throw new NotImplementedException(); } }

  [MemberDeclarationTestCase("public int P { get; }", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public int ImplicitProperty1.P { get; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("public int ImplicitProperty1.P { get => throw null; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.ThrowNull)]
  [MemberDeclarationTestCase("public int Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ImplicitProperty1.P { get; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, AccessorBody = MethodBodyOption.None)]
  public int P { get => throw new NotImplementedException(); }
}

class ExplicitProperty1 : IProperty, C.INestedWithProperty {
  [MemberDeclarationTestCase("int IProperty.P1 { get; set; }", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("int ExplicitProperty1.IProperty.P1 { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("int ExplicitProperty1.IProperty.P1 { get => throw null; set => throw null; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.ThrowNull)]
  [MemberDeclarationTestCase("int Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ExplicitProperty1.Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.IProperty.P1 { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, MethodBody = MethodBodyOption.None)]
  int IProperty.P1 { get; set; }

  [MemberDeclarationTestCase("int IProperty.P2 { get; }", MemberWithNamespace = false)]
  int IProperty.P2 { get { throw new NotImplementedException(); } }

  [MemberDeclarationTestCase("int IProperty.P3 { set; }", MemberWithNamespace = false)]
  int IProperty.P3 { set { throw new NotImplementedException(); } }

  [MemberDeclarationTestCase("int IProperty.P4 { init; }", MemberWithNamespace = false)]
  int IProperty.P4 { init { throw new NotImplementedException(); } }

  [MemberDeclarationTestCase("int C.INestedWithProperty.P { get; }", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("int ExplicitProperty1.C.INestedWithProperty.P { get; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("int ExplicitProperty1.C.INestedWithProperty.P { get => throw null; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.ThrowNull)]
  [MemberDeclarationTestCase("int Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ExplicitProperty1.Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.C.INestedWithProperty.P { get; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, AccessorBody = MethodBodyOption.None)]
  int C.INestedWithProperty.P { get => throw new NotImplementedException(); }
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
  [MemberDeclarationTestCase("event EventHandler E1;", MemberWithNamespace = false)]
  event EventHandler E1;

  [MemberDeclarationTestCase("event EventHandler<EventArgs> E2;", MemberWithNamespace = false)]
  event EventHandler<EventArgs> E2;
}

class ImplicitEvent : IEvent, C.INestedWithEvent {
  [MemberDeclarationTestCase("public event EventHandler E1;", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public event EventHandler E1", MemberWithNamespace = false, MemberOmitEndOfStatement = true)]
  [MemberDeclarationTestCase("public event EventHandler ImplicitEvent.E1;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public event System.EventHandler Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ImplicitEvent.E1;", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
  public event EventHandler E1;

  [MemberDeclarationTestCase("public event EventHandler<EventArgs> E2;", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public event EventHandler<EventArgs> ImplicitEvent.E2;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public event System.EventHandler<System.EventArgs> Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ImplicitEvent.E2;", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
  public event EventHandler<EventArgs> E2;

  [MemberDeclarationTestCase("public event EventHandler E;", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public event EventHandler E", MemberWithNamespace = false, MemberOmitEndOfStatement = true)]
  [MemberDeclarationTestCase("public event EventHandler ImplicitEvent.E;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public event System.EventHandler Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ImplicitEvent.E;", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
  public event EventHandler E;
}

class ExplicitEvent : IEvent, C.INestedWithEvent {
  [MemberDeclarationTestCase("event EventHandler IEvent.E1 { add; remove; }", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("event EventHandler IEvent.E1 { add; remove; }", MemberWithNamespace = false, MemberOmitEndOfStatement = true)]
  [MemberDeclarationTestCase("event EventHandler ExplicitEvent.IEvent.E1 { add; remove; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("event System.EventHandler Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ExplicitEvent.Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.IEvent.E1 { add; remove; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
  event EventHandler IEvent.E1 {
    add { throw new NotImplementedException(); }
    remove { throw new NotImplementedException(); }
  }

  [MemberDeclarationTestCase("event EventHandler<EventArgs> IEvent.E2 { add; remove; }", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("event EventHandler<EventArgs> ExplicitEvent.IEvent.E2 { add; remove; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("event System.EventHandler<System.EventArgs> Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ExplicitEvent.Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.IEvent.E2 { add; remove; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
  event EventHandler<EventArgs> IEvent.E2 {
    add { throw new NotImplementedException(); }
    remove { throw new NotImplementedException(); }
  }

  [MemberDeclarationTestCase("event EventHandler C.INestedWithEvent.E { add; remove; }", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("event EventHandler C.INestedWithEvent.E { add; remove; }", MemberWithNamespace = false, MemberOmitEndOfStatement = true)]
  [MemberDeclarationTestCase("event EventHandler ExplicitEvent.C.INestedWithEvent.E { add; remove; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("event System.EventHandler Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.ExplicitEvent.Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.C.INestedWithEvent.E { add; remove; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
  event EventHandler C.INestedWithEvent.E {
    add { throw new NotImplementedException(); }
    remove { throw new NotImplementedException(); }
  }
}

public delegate R EventHandlerCovariant<out R>();
public delegate void EventHandlerContravariant<in A>(A a);

public interface ICovariant<out R> {
  [MemberDeclarationTestCase("R M();", MemberWithNamespace = false)]
  R M();

  [MemberDeclarationTestCase("R P { get; }", MemberWithNamespace = false)]
  R P { get; }

#if false // CS1961
  [MemberDeclarationTestCase("event EventHandlerCovariant<R> ECovariant;", MemberWithNamespace = false)]
  event EventHandlerCovariant<R> ECovariant;
#endif

  [MemberDeclarationTestCase("event EventHandlerContravariant<R> EContravariant;", MemberWithNamespace = false)]
  event EventHandlerContravariant<R> EContravariant;
}

public interface IContravariant<in A> {
  [MemberDeclarationTestCase("void M(A a);", MemberWithNamespace = false)]
  void M(A a);

  [MemberDeclarationTestCase("A P { set; }", MemberWithNamespace = false)]
  A P { set; }

  [MemberDeclarationTestCase("event EventHandlerCovariant<A> ECovariant;", MemberWithNamespace = false)]
  event EventHandlerCovariant<A> ECovariant;

#if false // CS1961
  [MemberDeclarationTestCase("event EventHandlerContravariant<A> EContravariant;", MemberWithNamespace = false)]
  event EventHandlerContravariant<A> EContravariant;
#endif
}

public interface IVariant<out R, in A> {
  [MemberDeclarationTestCase("R M();", MemberWithNamespace = false)]
  R M();

  [MemberDeclarationTestCase("void M(A a);", MemberWithNamespace = false)]
  void M(A a);

  [MemberDeclarationTestCase("R POut { get; }", MemberWithNamespace = false)]
  R POut { get; }

  [MemberDeclarationTestCase("A PIn { set; }", MemberWithNamespace = false)]
  A PIn { set; }

  [MemberDeclarationTestCase("event EventHandlerContravariant<R> EContravariant;", MemberWithNamespace = false)]
  event EventHandlerContravariant<R> EContravariant;

  [MemberDeclarationTestCase("event EventHandlerCovariant<A> ECovariant;", MemberWithNamespace = false)]
  event EventHandlerCovariant<A> ECovariant;
}

public class CovariantInterfaceMemberImplementationGenericDefinition<R> : ICovariant<R> {
  [MemberDeclarationTestCase("public R M() {}", MemberWithNamespace = false)]
  public R M() => throw new NotImplementedException();

  [MemberDeclarationTestCase("public R P { get; }", MemberWithNamespace = false)]
  public R P { get => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("public event EventHandlerContravariant<R> EContravariant;", MemberWithNamespace = false)]
  public event EventHandlerContravariant<R> EContravariant;
}

public class ContravariantInterfaceMemberImplementationGenericDefinition<A> : IContravariant<A> {
  [MemberDeclarationTestCase("public void M(A a) {}", MemberWithNamespace = false)]
  public void M(A a) => throw new NotImplementedException();

  [MemberDeclarationTestCase("public A P { set; }", MemberWithNamespace = false)]
  public A P { set => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("public event EventHandlerCovariant<A> ECovariant;", MemberWithNamespace = false)]
  public event EventHandlerCovariant<A> ECovariant;
}

public class CovariantInterfaceMemberExplicitImplementationGenericDefinition<R> : ICovariant<R> {
  [MemberDeclarationTestCase("R ICovariant<R>.M() {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
  R ICovariant<R>.M() => throw new NotImplementedException();

  [MemberDeclarationTestCase("R ICovariant<R>.P { get; }", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
  R ICovariant<R>.P { get => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("event EventHandlerContravariant<R> ICovariant<R>.EContravariant { add; remove; }", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
  event EventHandlerContravariant<R> ICovariant<R>.EContravariant {
    add => throw new NotImplementedException();
    remove => throw new NotImplementedException();
  }
}

public class ContravariantInterfaceMemberExplicitImplementationGenericDefinition<A> : IContravariant<A> {
  [MemberDeclarationTestCase("void IContravariant<A>.M(A a) {}", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
  void IContravariant<A>.M(A a) => throw new NotImplementedException();

  [MemberDeclarationTestCase("A IContravariant<A>.P { set; }", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
  A IContravariant<A>.P { set => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("event EventHandlerCovariant<A> IContravariant<A>.ECovariant { add; remove; }", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
  event EventHandlerCovariant<A> IContravariant<A>.ECovariant {
    add => throw new NotImplementedException();
    remove => throw new NotImplementedException();
  }
}

public class CovariantInterfaceMemberImplementation : ICovariant<Stream> {
  [MemberDeclarationTestCase("public Stream M() {}", MemberWithNamespace = false)]
  public Stream M() => throw new NotImplementedException();

  [MemberDeclarationTestCase("public Stream P { get; }", MemberWithNamespace = false)]
  public Stream P { get => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("public event EventHandlerContravariant<Stream> EContravariant;", MemberWithNamespace = false)]
  public event EventHandlerContravariant<Stream> EContravariant;
}

public class ContravariantInterfaceMemberImplementation : IContravariant<Stream> {
  [MemberDeclarationTestCase("public void M(System.IO.Stream c) {}", MemberWithNamespace = false)]
  public void M(Stream c) => throw new NotImplementedException();

  [MemberDeclarationTestCase("public Stream P { set; }", MemberWithNamespace = false)]
  public Stream P { set => throw new NotImplementedException(); }

  [MemberDeclarationTestCase("public event EventHandlerCovariant<Stream> ECovariant;", MemberWithNamespace = false)]
  public event EventHandlerCovariant<Stream> ECovariant;
}
