// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Threading;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.Parameters;

public class Standard {
  [MemberDeclarationTestCase("public void M() {}")] public void M() { }
  [MemberDeclarationTestCase("public void M(int x) {}")] public void M(int x) { }
  [MemberDeclarationTestCase("public void M(int x, int y) {}")] public void M(int x, int y) { }
}

public class Modifiers {
  [MemberDeclarationTestCase("public void In(in int x) {}")] public void In(in int x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public void Out(out int x) {}")] public void Out(out int x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public void Ref(ref int x) {}")] public void Ref(ref int x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public void RefReadOnly(ref readonly System.ReadOnlySpan<int> x) {}")] public void RefReadOnly(ref readonly ReadOnlySpan<int> x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public ref int RefReturn() {}")] public ref int RefReturn() => throw new NotImplementedException();
  [MemberDeclarationTestCase("public ref readonly int RefReadOnlyReturn() {}")] public ref readonly int RefReadOnlyReturn() => throw new NotImplementedException();

  [MemberDeclarationTestCase("public void Scoped(System.ReadOnlySpan<int> x) {}")] public void Scoped(scoped ReadOnlySpan<int> x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public void ScopedIn(scoped in int x) {}")] public void ScopedIn(scoped in int x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public void ScopedOut(out int x) {}")] public void ScopedOut(scoped out int x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public void ScopedRef(scoped ref int x) {}")] public void ScopedRef(scoped ref int x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public void ScopedRefReadOnly(scoped ref readonly System.ReadOnlySpan<int> x) {}")] public void ScopedRefReadOnly(scoped ref readonly ReadOnlySpan<int> x) => throw new NotImplementedException();
}

public class ModifiersWithVirtual {
  [MemberDeclarationTestCase("public virtual void In(in int x) {}")] public virtual void In(in int x) { }
  [MemberDeclarationTestCase("public virtual void Out(out int x) {}")] public virtual void Out(out int x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public virtual void Ref(ref int x) {}")] public virtual void Ref(ref int x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public virtual void RefReadOnly(ref readonly System.ReadOnlySpan<int> x) {}")] public virtual void RefReadOnly(ref readonly ReadOnlySpan<int> x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public virtual ref int RefReturn() {}")] public virtual ref int RefReturn() => throw new NotImplementedException();
  [MemberDeclarationTestCase("public virtual ref readonly int RefReadOnlyReturn() {}")] public virtual ref readonly int RefReadOnlyReturn() => throw new NotImplementedException();

  [MemberDeclarationTestCase("public virtual void Scoped(System.ReadOnlySpan<int> x) {}")] public virtual void Scoped(scoped ReadOnlySpan<int> x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public virtual void ScopedIn(scoped in int x) {}")] public virtual void ScopedIn(scoped in int x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public virtual void ScopedOut(out int x) {}")] public virtual void ScopedOut(scoped out int x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public virtual void ScopedRef(scoped ref int x) {}")] public virtual void ScopedRef(scoped ref int x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public virtual void ScopedRefReadOnly(scoped ref readonly System.ReadOnlySpan<int> x) {}")] public virtual void ScopedRefReadOnly(scoped ref readonly ReadOnlySpan<int> x) => throw new NotImplementedException();
}

public class Params {
  [MemberDeclarationTestCase("public void M(params int[] p) {}")] public void M(params int[] p) { }
  [MemberDeclarationTestCase("public void M(int p0, params int[] p) {}")] public void M(int p0, params int[] p) { }
}

public class ParamsCollection {
  [MemberDeclarationTestCase("public void Span(params Span<int> p) {}", ParameterWithNamespace = false)]
  [MemberDeclarationTestCase("public void Span(params System.Span<int> p) {}", ParameterWithNamespace = true)]
  public void Span(params Span<int> p) { }

  [MemberDeclarationTestCase("public void ReadOnlySpan(params ReadOnlySpan<int> p) {}", ParameterWithNamespace = false)] public void ReadOnlySpan(params ReadOnlySpan<int> p) { }
  [MemberDeclarationTestCase("public void IEnumerable(params IEnumerable<int> p) {}", ParameterWithNamespace = false)] public void IEnumerable(params IEnumerable<int> p) { }
  [MemberDeclarationTestCase("public void ICollection(params ICollection<int> p) {}", ParameterWithNamespace = false)] public void ICollection(params ICollection<int> p) { }
  [MemberDeclarationTestCase("public void IReadOnlyCollection(params IReadOnlyCollection<int> p) {}", ParameterWithNamespace = false)] public void IReadOnlyCollection(params IReadOnlyCollection<int> p) { }
  [MemberDeclarationTestCase("public void IList(params IList<int> p) {}", ParameterWithNamespace = false)] public void IList(params IList<int> p) { }
  [MemberDeclarationTestCase("public void IReadOnlyList(params IReadOnlyList<int> p) {}", ParameterWithNamespace = false)] public void IReadOnlyList(params IReadOnlyList<int> p) { }
  [MemberDeclarationTestCase("public void List(params List<int> p) {}", ParameterWithNamespace = false)] public void List(params List<int> p) { }
}

public class CParam { }
public struct SParam { }

public static class ExtensionMethods {
  [MemberDeclarationTestCase("public static void M(this int i) {}")]
  public static void M(this int i) { }

  [MemberDeclarationTestCase("public static void M(this IEnumerable<int> enumerable) {}", ParameterWithNamespace = false)]
  public static void M(this IEnumerable<int> enumerable) { }

  [MemberDeclarationTestCase("public static void M(this CParam c) {}", ParameterWithNamespace = false)]
  public static void M(this CParam c) { }

  [MemberDeclarationTestCase("public static void M(this CParam c, int i) {}", ParameterWithNamespace = false)]
  public static void M(this CParam c, int i) { }

  [MemberDeclarationTestCase("public static void M(this SParam s) {}", ParameterWithNamespace = false)]
  public static void M(this SParam s) { }
}

public class ValueTuples {
  [MemberDeclarationTestCase("public void M1((int, int) arg) {}")] public void M1((int, int) arg) { }
  [MemberDeclarationTestCase("public void M2((int x, int y) arg) {}")] public void M2((int x, int y) arg) { }
  [MemberDeclarationTestCase("public void M3((int, int, int) arg) {}")] public void M3((int, int, int) arg) { }
  [MemberDeclarationTestCase("public void M4((string x, string y) arg) {}")] public void M4((string x, string y) arg) { }
  [MemberDeclarationTestCase("public void M5(System.ValueTuple<string> arg) {}")] public void M5(ValueTuple<string> arg) { }
  [MemberDeclarationTestCase("public void M6((string, string) arg) {}")] public void M6(ValueTuple<string, string> arg) { }
}
