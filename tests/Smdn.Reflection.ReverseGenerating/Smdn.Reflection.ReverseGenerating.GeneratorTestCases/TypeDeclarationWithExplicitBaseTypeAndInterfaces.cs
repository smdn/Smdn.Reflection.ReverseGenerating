// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1816

using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclarationWithExplicitBaseTypeAndInterfaces;

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase("public abstract class C1 : System.IDisposable")]
public abstract class C1 : IDisposable {
  public void Dispose() => throw new NotImplementedException();
}

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(@"public class C2 :
  System.ICloneable,
  System.IDisposable")]
public class C2 : IDisposable, ICloneable {
  public object Clone() => throw new NotImplementedException();
  public void Dispose() => throw new NotImplementedException();
}

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase("public struct S1 : System.IDisposable")]
public struct S1 : IDisposable {
  public void Dispose() => throw new NotImplementedException();
}

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(@"public struct S2 :
  System.ICloneable,
  System.IDisposable")]
public struct S2 : IDisposable, ICloneable {
  public object Clone() => throw new NotImplementedException();
  public void Dispose() => throw new NotImplementedException();
}

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase("public interface I1 : System.IDisposable")]
public interface I1 : IDisposable { }

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(@"public interface I2 :
  System.ICloneable,
  System.IDisposable")]
public interface I2 : IDisposable, ICloneable { }
