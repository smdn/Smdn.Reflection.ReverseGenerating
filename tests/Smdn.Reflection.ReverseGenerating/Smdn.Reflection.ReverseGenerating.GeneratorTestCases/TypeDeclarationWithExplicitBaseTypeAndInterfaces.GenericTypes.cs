// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1816

using System;
using System.Collections.Generic;

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
