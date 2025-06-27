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

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public record class R0",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = true
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public record class R0 : IEquatable<R0>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = false
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public class R0 : IEquatable<R0>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = false,
  TypeOmitRecordImplicitInterface = true
)]
public record R0(int X, string Y);

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public record class R1 : IDisposable",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = true
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  @"public record class R1 :
  IDisposable,
  IEquatable<R1>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = false
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  @"public class R1 :
  IDisposable,
  IEquatable<R1>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = false,
  TypeOmitRecordImplicitInterface = true
)]
public record R1(int X, string Y) : IDisposable {
  public void Dispose() => throw new NotImplementedException();
}

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  @"public record class R2 : IEquatable<R1>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = true
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  @"public record class R2 :
  IEquatable<R1>,
  IEquatable<R2>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = false
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  @"public class R2 :
  IEquatable<R1>,
  IEquatable<R2>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = false,
  TypeOmitRecordImplicitInterface = true
)]
public record R2(int X, string Y) : IEquatable<R1> {
  public bool Equals(R1 other) => throw new NotImplementedException();
}

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public record struct RS0",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = true
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public record struct RS0 : IEquatable<RS0>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = false
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public struct RS0 : IEquatable<RS0>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = false,
  TypeOmitRecordImplicitInterface = true
)]
public record struct RS0(int X, string Y);

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public record struct RS1 : IDisposable",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = true
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  @"public record struct RS1 :
  IDisposable,
  IEquatable<RS1>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = false
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  @"public struct RS1 :
  IDisposable,
  IEquatable<RS1>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = false,
  TypeOmitRecordImplicitInterface = true
)]
public record struct RS1(int X, string Y) : IDisposable {
  public void Dispose() => throw new NotImplementedException();
}

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  "public record class RX0 : R0",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = true
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  @"public record class RX0 :
  R0,
  IEquatable<RX0>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = false
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  @"public class RX0 :
  R0,
  IEquatable<RX0>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = false,
  TypeOmitRecordImplicitInterface = true
)]
public record RX0(int X, string Y, double Z) : R0(X, Y);

[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  @"public record class RX1 :
  R0,
  IDisposable",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = true
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  @"public record class RX1 :
  R0,
  IDisposable,
  IEquatable<RX1>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = true,
  TypeOmitRecordImplicitInterface = false
)]
[TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCase(
  @"public class RX1 :
  R0,
  IDisposable,
  IEquatable<RX1>",
  TypeWithNamespace = false,
  TypeEnableRecordTypes = false,
  TypeOmitRecordImplicitInterface = true
)]
public record RX1(int X, string Y, double Z) : R0(X, Y), IDisposable {
  public void Dispose() => throw new NotImplementedException();
}
