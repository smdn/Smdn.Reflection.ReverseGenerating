// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#nullable enable annotations

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.ExplicitBaseTypeAndInterfaces.TSelf;

[ExplicitBaseTypeAndInterfacesTestCase("", TypeWithNamespace = false, TypeWithDeclaringTypeName = false)]
public interface I<TSelf> where TSelf : I<TSelf> { }

[ExplicitBaseTypeAndInterfacesTestCase("I<C>", TypeWithNamespace = false, TypeWithDeclaringTypeName = false)]
public class C : I<C> { }

[ExplicitBaseTypeAndInterfacesTestCase("I<C<T>>", TypeWithNamespace = false, TypeWithDeclaringTypeName = false)]
public class C<T> : I<C<T>> { }

#if NET7_0_OR_GREATER
[ExplicitBaseTypeAndInterfacesTestCase("IParsable<CParsable>", TypeWithNamespace = false, TypeWithDeclaringTypeName = false)]
public class CParsable : IParsable<CParsable> {
  public static CParsable Parse(string s, IFormatProvider? provider) => throw null;
  public static bool TryParse(string? s, IFormatProvider? provider, out CParsable result) => throw null;
}
#endif // NET7_0_OR_GREATER
