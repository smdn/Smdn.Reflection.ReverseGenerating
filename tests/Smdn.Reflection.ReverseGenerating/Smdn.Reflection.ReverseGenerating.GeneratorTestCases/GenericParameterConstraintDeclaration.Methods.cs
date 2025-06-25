// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cSpell:ignore accessibilities,nullabilities
#pragma warning disable CS8597

using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.GenericParameterConstraints.Methods {
  public class C {
    [GenericParameterConstraintTestCase("")]
    public void NoConstraints<T>(T x) => throw null;

    [GenericParameterConstraintTestCase("where T : new()")]
    public void DefaultConstructorConstraint<T>(T x) where T : new() => throw null;

    [GenericParameterConstraintTestCase("where T : class")]
    public void ClassConstraint<T>(T x) where T : class => throw null;

    [GenericParameterConstraintTestCase("where T : struct")]
    public void StructConstraint<T>(T x) where T : struct => throw null;

    [GenericParameterConstraintTestCase("where T : notnull")]
    public void NotNullConstraint<T>(T x) where T : notnull => throw null;

    [GenericParameterConstraintTestCase("where T : unmanaged")]
    public void UnmanagedConstraint<T>(T x) where T : unmanaged => throw null;

    [GenericParameterConstraintTestCase("where T : IDisposable", MemberWithNamespace = false)]
    [GenericParameterConstraintTestCase("where T : System.IDisposable", MemberWithNamespace = true)]
    public void InterfaceTypeConstraint<T>(T x) where T : System.IDisposable => throw null;

    [GenericParameterConstraintTestCase("where T : Enum", MemberWithNamespace = false)]
    [GenericParameterConstraintTestCase("where T : System.Enum", MemberWithNamespace = true)]
    public void EnumTypeConstraint<T>(T x) where T : System.Enum => throw null;

    [GenericParameterConstraintTestCase("where T : Delegate", MemberWithNamespace = false)]
    [GenericParameterConstraintTestCase("where T : System.Delegate", MemberWithNamespace = true)]
    public void DelegateTypeConstraint<T>(T x) where T : System.Delegate => throw null;

    [GenericParameterConstraintTestCase("where T : MulticastDelegate", MemberWithNamespace = false)]
    [GenericParameterConstraintTestCase("where T : System.MulticastDelegate", MemberWithNamespace = true)]
    public void MulticastDelegateTypeConstraint<T>(T x) where T : System.MulticastDelegate => throw null;

    [GenericParameterConstraintTestCase("where T : class, new()", MemberWithNamespace = true)]
    public void ClassWithDefaultConstructorConstraint<T>(T x) where T : class, new() => throw null;

    [GenericParameterConstraintTestCase("where T : class, System.IDisposable, new()", MemberWithNamespace = true)]
    public void IDisposableClassWithDefaultConstructorConstraint<T>(T x) where T : class, IDisposable, new() => throw null;

    [GenericParameterConstraintTestCase("where T : class, System.ICloneable, System.IDisposable, new()", MemberWithNamespace = true)]
    public void IDisposableICloneableClassWithDefaultConstructorConstraint<T>(T x) where T : class, IDisposable, ICloneable, new() => throw null;

    [GenericParameterConstraintTestCase("where T : class, System.ICloneable, System.IDisposable, new()", MemberWithNamespace = true)]
    public void ICloneableIDisposableClassWithDefaultConstructorConstraint<T>(T x) where T : class, ICloneable, IDisposable, new() => throw null;
  }

  namespace NoConstraints {
#nullable enable annotations
    public class CNullableEnableContext {
      [GenericParameterConstraintTestCase("")]
      public void NoConstraints<T>(T x) => throw null;
    }
#nullable restore

#nullable disable annotations
    public class CNullableDisableContext {
      [GenericParameterConstraintTestCase("")]
      public void NoConstraints<T>(T x) => throw null;
    }
#nullable restore

    public class C {
#nullable enable annotations
      [GenericParameterConstraintTestCase("")]
      public void NoConstraints_NullableEnableContext<T>(T x) => throw null;
#nullable restore

#nullable disable annotations
      [GenericParameterConstraintTestCase("")]
      public void NoConstraints_NullableDisableContext<T>(T x) => throw null;
#nullable restore
    }
  }

  namespace NotNullConstraints {
#nullable enable annotations
    public class CNullableEnableContext {
      [GenericParameterConstraintTestCase("where T : notnull")]
      public void NotNullConstraint<T>(T x) where T : notnull => throw null;
    }
#nullable restore

#nullable disable annotations
    public class CNullableDisableContext {
      [GenericParameterConstraintTestCase("where T : notnull")]
      public void NotNullConstraint<T>(T x) where T : notnull => throw null;
    }
#nullable restore

    public class C {
#nullable enable annotations
      [GenericParameterConstraintTestCase("where T : notnull")]
      public void NotNullConstraint_NullableEnableContext<T>(T x) where T : notnull => throw null;
#nullable restore

#nullable disable annotations
      [GenericParameterConstraintTestCase("where T : notnull")]
      public void NotNullConstraint_NullableDisableContext<T>(T x) where T : notnull => throw null;
#nullable restore
    }
  }

  namespace Nullabilities {
#nullable enable annotations
    public class C {
      [GenericParameterConstraintTestCase("where T : class")]
      public void RefType<T>(T x) where T : class => throw null;

      [GenericParameterConstraintTestCase("where T : class?")]
      public void NullableRefType<T>(T x) where T : class? => throw null;

      [GenericParameterConstraintTestCase("where T : class, System.IDisposable")]
      public void RefTypeWithIDisposable<T>(T x) where T : class, IDisposable => throw null;

      [GenericParameterConstraintTestCase("where T : class?, System.IDisposable")]
      public void NullableRefTypeWithIDisposable<T>(T x) where T : class?, IDisposable => throw null;

      [GenericParameterConstraintTestCase("where T : class, System.IDisposable")]
      public void RefTypeWithNullableIDisposable<T>(T x) where T : class, IDisposable? => throw null; // ? nullability annotation will be erased

      [GenericParameterConstraintTestCase("where T : class, System.IDisposable")]
      public void NullableRefTypeWithNullableIDisposable<T>(T x) where T : class?, IDisposable? => throw null; // ? nullability annotation will be erased
    }
#nullable restore
  }
}
