// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cSpell:ignore accessibilities,nullabilities
using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.GenericParameterConstraints.Types {
  [GenericParameterConstraintTestCase("")]
  public class NoConstraints<T> { }

  [GenericParameterConstraintTestCase("where T : new()")]
  public class DefaultConstructorConstraint<T> where T : new() { }

  [GenericParameterConstraintTestCase("where T : class")]
  public class ClassConstraint<T> where T : class { }

  [GenericParameterConstraintTestCase("where T : struct")]
  public class StructConstraint<T> where T : struct { }

  [GenericParameterConstraintTestCase("where T : notnull")]
  public class NotNullConstraint<T> where T : notnull { }

  [GenericParameterConstraintTestCase("where T : unmanaged")]
  public class UnmanagedConstraint<T> where T : unmanaged { }

  [GenericParameterConstraintTestCase("where T : IDisposable", TypeWithNamespace = false)]
  [GenericParameterConstraintTestCase("where T : System.IDisposable", TypeWithNamespace = true)]
  public class InterfaceTypeConstraint<T> where T : System.IDisposable { }

  [GenericParameterConstraintTestCase("where T : Enum", TypeWithNamespace = false)]
  [GenericParameterConstraintTestCase("where T : System.Enum", TypeWithNamespace = true)]
  public class EnumTypeConstraint<T> where T : System.Enum { }

  [GenericParameterConstraintTestCase("where T : Delegate", TypeWithNamespace = false)]
  [GenericParameterConstraintTestCase("where T : System.Delegate", TypeWithNamespace = true)]
  public class DelegateTypeConstraint<T> where T : System.Delegate { }

  [GenericParameterConstraintTestCase("where T : MulticastDelegate", TypeWithNamespace = false)]
  [GenericParameterConstraintTestCase("where T : System.MulticastDelegate", TypeWithNamespace = true)]
  public class MulticastDelegateTypeConstraint<T> where T : System.MulticastDelegate { }

  [GenericParameterConstraintTestCase("where T : class, new()", TypeWithNamespace = true)]
  public class ClassWithDefaultConstructorConstraint<T> where T : class, new() { }

  [GenericParameterConstraintTestCase("where T : class, System.IDisposable, new()", TypeWithNamespace = true)]
  public class IDisposableClassWithDefaultConstructorConstraint<T> where T : class, IDisposable, new() { }

  [GenericParameterConstraintTestCase("where T : class, System.ICloneable, System.IDisposable, new()", TypeWithNamespace = true)]
  public class IDisposableICloneableClassWithDefaultConstructorConstraint<T> where T : class, IDisposable, ICloneable, new() { }

  [GenericParameterConstraintTestCase("where T : class, System.ICloneable, System.IDisposable, new()", TypeWithNamespace = true)]
  public class ICloneableIDisposableClassWithDefaultConstructorConstraint<T> where T : class, ICloneable, IDisposable, new() { }

  namespace NoConstraints {
    namespace NullableEnableContext {
#nullable enable annotations
      [GenericParameterConstraintTestCase("")]
      public class NoConstraints<T> { }
#nullable restore
    }

    namespace NullableDisableContext {
#nullable disable annotations
      [GenericParameterConstraintTestCase("")]
      public class NoConstraints<T> { }
#nullable restore
    }
  }

  namespace NotNullConstraints {
    namespace NullableEnableContext {
#nullable enable annotations
      [GenericParameterConstraintTestCase("where T : notnull")]
      public class NotNullConstraint<T> where T : notnull { }

      [GenericParameterConstraintTestCase("where T0 : notnull")]
      public class NotNullConstraint_NoConstraints<T0, T1> where T0 : notnull { }

      [GenericParameterConstraintTestCase("where T1 : notnull")]
      public class NoConstraints_NotNullConstraint<T0, T1> where T1 : notnull { }

      [GenericParameterConstraintTestCase("where T : notnull, System.IDisposable")]
      public class NotNullIDisposableConstraint<T> where T : notnull, IDisposable { }

      [GenericParameterConstraintTestCase("where T : notnull, System.IDisposable")]
      public class NotNullNullableIDisposableConstraint<T> where T : notnull, IDisposable? { } // ? nullability annotation will be erased
#nullable restore
    }

    namespace NullableDisableContext {
#nullable disable annotations
      [GenericParameterConstraintTestCase("where T : notnull")]
      public class NotNullConstraint<T> where T : notnull { }

      [GenericParameterConstraintTestCase("where T0 : notnull")]
      public class NotNullConstraint_NoConstraints<T0, T1> where T0 : notnull { }

      [GenericParameterConstraintTestCase("where T1 : notnull")]
      public class NoConstraints_NotNullConstraint<T0, T1> where T1 : notnull { }

      [GenericParameterConstraintTestCase("where T : notnull, System.IDisposable")]
      public class NotNullIDisposableConstraint<T> where T : notnull, IDisposable { }
#nullable restore
    }
  }

  namespace Nullabilities {
#nullable enable annotations
    [GenericParameterConstraintTestCase("where T : class")]
    public class RefType<T> where T : class { }

    [GenericParameterConstraintTestCase("where T : class?")]
    public class NullableRefType<T> where T : class? { }

    [GenericParameterConstraintTestCase("where TRef : class where TNullableRef : class?")]
    public class RefTypeAndNullableRefType<TRef, TNullableRef> where TRef : class where TNullableRef : class? { }

    [GenericParameterConstraintTestCase("where TNullableRef : class? where TRef : class")]
    public class NullableRefTypeAndRefType<TNullableRef, TRef> where TNullableRef : class? where TRef : class { }

    [GenericParameterConstraintTestCase("where T : class, new()")]
    public class RefTypeWithDefaultConstructor<T> where T : class, new() { }

    [GenericParameterConstraintTestCase("where T : class?, new()")]
    public class NullableRefTypeWithDefaultConstructor<T> where T : class?, new() { }

    [GenericParameterConstraintTestCase("where T : class, System.IDisposable")]
    public class RefTypeWithIDisposable<T> where T : class, IDisposable { }

    [GenericParameterConstraintTestCase("where T : class?, System.IDisposable")]
    public class NullableRefTypeWithIDisposable<T> where T : class?, IDisposable { }

    [GenericParameterConstraintTestCase("where T : class, System.IDisposable")]
    public class RefTypeWithNullableIDisposable<T> where T : class, IDisposable? { } // ? nullability annotation will be erased

    [GenericParameterConstraintTestCase("where T : class, System.IDisposable")]
    public class NullableRefTypeWithNullableIDisposable<T> where T : class?, IDisposable? { } // ? nullability annotation will be erased

    [GenericParameterConstraintTestCase("where T : System.IDisposable")]
    public class IDisposable<T> where T : IDisposable { }

    [GenericParameterConstraintTestCase("where T : System.IDisposable")]
    public class NullableIDisposable<T> where T : IDisposable? { } // ? nullability annotation will be erased
#nullable restore
  }

  namespace BaseTypeConstraints {
    public class CBase { }

    [GenericParameterConstraintTestCase("where T : CBase", TypeWithNamespace = false)]
    public class BaseTypeConstraint<T> where T : CBase { }

    [GenericParameterConstraintTestCase("where T : CBase, IDisposable", TypeWithNamespace = false)]
    public class IDisposableBaseTypeConstraint<T> where T : CBase, IDisposable { }

#nullable enable annotations
    [GenericParameterConstraintTestCase("where T : CBase", TypeWithNamespace = false)]
    public class NullableBaseTypeConstraint<T> where T : CBase? { } // ? nullability annotation will be erased

    [GenericParameterConstraintTestCase("where T : CBase, IDisposable", TypeWithNamespace = false)]
    public class NullableIDisposableNullableBaseTypeConstraint<T> where T : CBase?, IDisposable? { } // ? nullability annotation will be erased
#nullable restore
  }

  namespace TSelfConstraints {
#nullable enable annotations
    [GenericParameterConstraintTestCase("where TSelf : I<TSelf>", TypeWithNamespace = false)]
    public interface I<TSelf> where TSelf : I<TSelf> { }

    [GenericParameterConstraintTestCase("where T : I<T>", TypeWithNamespace = false)]
    public class C<T> where T : I<T> { }

    [GenericParameterConstraintTestCase("where U : I<U> where V : struct", TypeWithNamespace = false)]
    public class C<U, V> where U : I<U> where V : struct { }

#if NET7_0_OR_GREATER
    [GenericParameterConstraintTestCase("where TParsable : IParsable<TParsable>", TypeWithNamespace = false)]
    public class Parsable<TParsable> where TParsable : IParsable<TParsable> { }
#endif // NET7_0_OR_GREATER
#nullable restore
  }
}
