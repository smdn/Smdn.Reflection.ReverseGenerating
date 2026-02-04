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

#if NET9_0_OR_GREATER
    [GenericParameterConstraintTestCase("where T : allows ref struct")]
    public void AllowsRefStructAntiConstraint<T>(scoped T x) where T : allows ref struct => throw null;

    [GenericParameterConstraintTestCase("where T : new(), allows ref struct")]
    public void DefaultConstructorAndAllowsRefStructAntiConstraint<T>(scoped T x) where T : new(), allows ref struct => throw null;

    [GenericParameterConstraintTestCase("where T : System.IDisposable, new(), allows ref struct", MemberWithNamespace = true)]
    public void IDisposableWithDefaultConstructorAndAllowsRefStructAntiConstraint<T>(scoped T x) where T : IDisposable, new(), allows ref struct => throw null;
#endif
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

  // ref: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/unconstrained-type-parameter-annotations
  namespace Overrides {
#nullable enable annotations
    // C# 8.0
    namespace ExplicitClassConstraintsInBaseClass {
      public class CBase {
        [GenericParameterConstraintTestCase("where T : struct")]
        public virtual void M<T>(T? t) where T : struct => throw new NotImplementedException();
        [GenericParameterConstraintTestCase("where T : class")]
        public virtual void M<T>(T? t) where T : class => throw new NotImplementedException();
      }

      public class CImplicitStructConstraints : CBase {
        [SkipTestCase("implicit generic constraint is not supported")]
        [GenericParameterConstraintTestCase("")]
        public override void M<T>(T? t) /* where T : struct */ => throw new NotImplementedException();
        [GenericParameterConstraintTestCase("where T : class")]
        public override void M<T>(T? t) where T : class => throw new NotImplementedException();
      }

#if false // error CS0111
      public class CImplicitClassConstraints : CBase {
        [GenericParameterConstraintTestCase("where T : struct")]
        public override void M<T>(T? t) where T : struct => throw new NotImplementedException();
        [GenericParameterConstraintTestCase("where T : class")]
        public override void M<T>(T? t) /* where T : class */ => throw new NotImplementedException(); // CS0111
      }
#endif

      public class CExplicitConstraints : CBase {
        [GenericParameterConstraintTestCase("where T : struct")]
        public override void M<T>(T? t) where T : struct => throw new NotImplementedException();
        [GenericParameterConstraintTestCase("where T : class")]
        public override void M<T>(T? t) where T : class => throw new NotImplementedException();
      }
    }

    // C# 9.0
    namespace ImplicitConstraintsInBaseClass {
      public class CBase {
        [GenericParameterConstraintTestCase("where T : struct")]
        public virtual void M<T>(T? t) where T : struct => throw new NotImplementedException();
        [GenericParameterConstraintTestCase("")]
        public virtual void M<T>(T? t) => throw new NotImplementedException();
      }

      public class CExplicitDefaultConstraintsAndOverrideAsImplicitConstraint : CBase {
        [GenericParameterConstraintTestCase("where T : struct")]
        public override void M<T>(T? t) /*where T : struct*/ => throw new NotImplementedException();
        [SkipTestCase("'default' generic constraint is not supported")]
        [GenericParameterConstraintTestCase("where T : default")]
        public override void M<T>(T? t) where T : default => throw new NotImplementedException();
      }

      public class CExplicitDefaultConstraintsAndOverrideAsExplicitConstraint : CBase {
        [GenericParameterConstraintTestCase("where T : struct")]
        public override void M<T>(T? t) where T : struct => throw new NotImplementedException();
        [SkipTestCase("'default' generic constraint is not supported")]
        [GenericParameterConstraintTestCase("where T : default")]
        public override void M<T>(T? t) where T : default => throw new NotImplementedException();
      }

#if false // error CS8665
      public class CExplicitClassConstraints : CBase {
        [GenericParameterConstraintTestCase("where T : struct")]
        public override void M<T>(T? t) where T : struct => throw new NotImplementedException();
        [GenericParameterConstraintTestCase("where T : default")]
        public override void M<T>(T? t) where T : class => throw new NotImplementedException(); // CS8665
      }
#endif

#if false // error CS0111
      public class CKeepImplicitConstraints : CBase {
        [GenericParameterConstraintTestCase("where T : struct")]
        public override void M<T>(T? t) where T : struct => throw new NotImplementedException();
        [GenericParameterConstraintTestCase("where T : default")]
        public override void M<T>(T? t) => throw new NotImplementedException(); // CS0111
      }
#endif
    }
#nullable restore
  }
}
