// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1816

using System;
using System.Collections.Generic;
using System.IO;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclarationWithExplicitBaseTypeAndInterfaces.GenericTypes;

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase("public class C1<T> : System.Collections.Generic.List<T> where T : class")]
public class C1<T> :
  List<T>
  where T : class
{
  public void Dispose() => throw new NotImplementedException();
}

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(@"public class C2<T> :
  System.Collections.Generic.List<T>,
  System.ICloneable
  where T : class")]
public class C2<T> :
  List<T>,
  ICloneable
  where T : class
{
  public object Clone() => throw new NotImplementedException();
}

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(@"public class C3<TKey, TValue> :
  System.Collections.Generic.Dictionary<TKey, TValue>,
  System.ICloneable
  where TKey : class
  where TValue : struct")]
public class C3<TKey, TValue> :
  Dictionary<TKey, TValue>,
  ICloneable
  where TKey : class
  where TValue : struct
{
  public object Clone() => throw new NotImplementedException();
}

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public record class R0<TValue> where TValue : struct",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = true
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public record class R0<TValue> : IEquatable<R0<TValue>> where TValue : struct",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = false
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public class R0<TValue> : IEquatable<R0<TValue>> where TValue : struct",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = false,
  TypeOmitRecordImplicitInterface = true
)]
public record R0<TValue>(TValue Value) where TValue : struct;

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public record struct RS0<TValue> where TValue : struct",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = true
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public record struct RS0<TValue> : IEquatable<RS0<TValue>> where TValue : struct",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = false
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public struct RS0<TValue> : IEquatable<RS0<TValue>> where TValue : struct",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = false,
  TypeOmitRecordImplicitInterface = true
)]
public record struct RS0<TValue>(TValue Value) where TValue : struct;

public interface ICovariant<out R> { }
public interface IContravariant<in A> { }
public interface IVariant<out R, in A> { }

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase("public class CCovariantGenericTypeDefinition<R> : ICovariant<R>", TypeWithNamespace = false)]
public class CCovariantGenericTypeDefinition<R> : ICovariant<R> { }

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase("public class CContravariantGenericTypeDefinition<A> : IContravariant<A>", TypeWithNamespace = false)]
public class CContravariantGenericTypeDefinition<A> : IContravariant<A> { }

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase("public class CVariantGenericTypeDefinition<R, A> : IVariant<R, A>", TypeWithNamespace = false)]
public class CVariantGenericTypeDefinition<R, A> : IVariant<R, A> { }

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase("public class CCovariant : ICovariant<Stream>", TypeWithNamespace = false)]
public class CCovariant : ICovariant<Stream> { }

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase("public class CContravariant : IContravariant<Stream>", TypeWithNamespace = false)]
public class CContravariant : IContravariant<Stream> { }

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase("public class CVariant : IVariant<Stream, Stream>", TypeWithNamespace = false)]
public class CVariant : IVariant<Stream, Stream> { }
