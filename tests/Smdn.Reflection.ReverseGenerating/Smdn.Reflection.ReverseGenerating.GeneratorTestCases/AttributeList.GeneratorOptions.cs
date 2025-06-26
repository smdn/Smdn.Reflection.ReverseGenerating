// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CS8597

using System;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.AttributeList.GeneratorOptions {
  namespace WithNamespace {
    [AttributeListTestCase("[System.Obsolete]", AttributeWithNamespace = true)]
    [Obsolete] class C0 { }

    [AttributeListTestCase("[Obsolete]", AttributeWithNamespace = false)]
    [Obsolete] class C1 { }
  }

  namespace WithNamedArguments {
    [AttributeListTestCase("[Obsolete]", AttributeWithNamedArguments = true, AttributeWithNamespace = false)]
    [Obsolete] class C0 { }

    [AttributeListTestCase("[Obsolete(message: \"message\")]", AttributeWithNamedArguments = true, AttributeWithNamespace = false)]
    [Obsolete(message: "message")] class C1 { }

    [AttributeListTestCase("[Obsolete(\"message\")]", AttributeWithNamedArguments = false, AttributeWithNamespace = false)]
    [Obsolete(message: "message")] class C2 { }
  }

  namespace WithDeclaringTypeName {
    public class C {
      [AttributeUsage(System.AttributeTargets.Class)]
      public class ClassAttribute : Attribute { }

      [AttributeListTestCase("[C.Class]", AttributeWithDeclaringTypeName = true, AttributeWithNamespace = false)]
      [Class] public class C0 { }

      [AttributeListTestCase("[Class]", AttributeWithDeclaringTypeName = false, AttributeWithNamespace = false)]
      [Class] public class C1 { }

      [AttributeUsage(System.AttributeTargets.Method)]
      public class MethodAttribute : Attribute { }

      [AttributeListTestCase("[C.Method]", AttributeWithDeclaringTypeName = true, AttributeWithNamespace = false)]
      [Method] public void M0() => throw null;

      [AttributeListTestCase("[Method]", AttributeWithDeclaringTypeName = false, AttributeWithNamespace = false)]
      [Method] public void M1() => throw null;
    }
  }

  namespace OmitAttributeSuffix {
    [AttributeListTestCase("[System.Obsolete]", AttributeWithNamespace = true, AttributeOmitAttributeSuffix = true)]
    [AttributeListTestCase("[Obsolete]", AttributeWithNamespace = false, AttributeOmitAttributeSuffix = true)]
    [AttributeListTestCase("[System.ObsoleteAttribute]", AttributeWithNamespace = true, AttributeOmitAttributeSuffix = false)]
    [AttributeListTestCase("[ObsoleteAttribute]", AttributeWithNamespace = false, AttributeOmitAttributeSuffix = false)]
    [Obsolete] public class AttributeName { }

#if NET7_0_OR_GREATER
    [AttributeUsage(System.AttributeTargets.Class)]
    public class GenericAttributeNameAttribute<T> : Attribute { }

    [AttributeListTestCase("[GenericAttributeName<int>]", AttributeWithNamespace = false, AttributeOmitAttributeSuffix = true)]
    [AttributeListTestCase("[GenericAttributeNameAttribute<int>]", AttributeWithNamespace = false, AttributeOmitAttributeSuffix = false)]
    [GenericAttributeName<int>] public class GenericAttributeName { }
#endif
  }

#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
#nullable enable
  namespace OmitInaccessibleMembersInNullStateAttribute {
    public class NullStateAttribute {
#pragma warning disable CS0169
      private string? F1;
      protected string? F2;
      public string? F3;
#pragma warning restore CS0169

      private string? P1 { get; }
      protected string? P2 { get; }
      public string? P3 { get; }

#pragma warning disable CS8774
      [MemberNotNull(nameof(F1))]
      [AttributeListTestCase(@"[MemberNotNull(""F1"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNull]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public void MemberNotNullWithNonPublicField() { }

      [MemberNotNull(nameof(F2))]
      [AttributeListTestCase(@"[MemberNotNull(""F2"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNull(""F2"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public void MemberNotNullWithPublicField_Protected() { }

      [MemberNotNull(nameof(F3))]
      [AttributeListTestCase(@"[MemberNotNull(""F3"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNull(""F3"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public void MemberNotNullWithPublicField() { }

      [MemberNotNull(nameof(P1))]
      [AttributeListTestCase(@"[MemberNotNull(""P1"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNull]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public void MemberNotNullWithNonPublicProperty() { }

      [MemberNotNull(nameof(P2))]
      [AttributeListTestCase(@"[MemberNotNull(""P2"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNull(""P2"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public void MemberNotNullWithPublicProperty_Protected() { }

      [MemberNotNull(nameof(P3))]
      [AttributeListTestCase(@"[MemberNotNull(""P3"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNull(""P3"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public void MemberNotNullWithPublicProperty() { }

      [MemberNotNull(nameof(F1), nameof(F2), nameof(F3), nameof(P1), nameof(P2), nameof(P3))]
      [AttributeListTestCase(@"[MemberNotNull(new string[] { ""F1"", ""F2"", ""F3"", ""P1"", ""P2"", ""P3"" })]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNull(new string[] { ""F2"", ""F3"", ""P2"", ""P3"" })]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public void MemberNotNullWithMembers() { }

      [MemberNotNull(nameof(F1), nameof(P1))]
      [AttributeListTestCase(@"[MemberNotNull(new string[] { ""F1"", ""P1"" })]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNull]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public bool MemberNotNullWithMembersAllNonPublic()
        => true;
#pragma warning restore CS8774

#pragma warning disable CS8775
      [MemberNotNullWhen(true, nameof(F1))]
      [AttributeListTestCase(@"[MemberNotNullWhen(true, ""F1"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNullWhen(true)]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public bool MemberNotNullWhenWithNonPublicField()
        => true;

      [MemberNotNullWhen(true, nameof(F2))]
      [AttributeListTestCase(@"[MemberNotNullWhen(true, ""F2"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNullWhen(true, ""F2"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public bool MemberNotNullWhenWithPublicField_Protected()
        => true;

      [MemberNotNullWhen(true, nameof(F3))]
      [AttributeListTestCase(@"[MemberNotNullWhen(true, ""F3"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNullWhen(true, ""F3"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public bool MemberNotNullWhenWithPublicField()
        => true;

      [MemberNotNullWhen(true, nameof(P1))]
      [AttributeListTestCase(@"[MemberNotNullWhen(true, ""P1"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNullWhen(true)]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public bool MemberNotNullWhenWithNonPublicProperty()
        => true;

      [MemberNotNullWhen(true, nameof(P2))]
      [AttributeListTestCase(@"[MemberNotNullWhen(true, ""P2"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNullWhen(true, ""P2"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public bool MemberNotNullWhenWithPublicProperty_Protected()
        => true;

      [MemberNotNullWhen(true, nameof(P3))]
      [AttributeListTestCase(@"[MemberNotNullWhen(true, ""P3"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNullWhen(true, ""P3"")]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public bool MemberNotNullWhenWithPublicProperty()
        => true;

      [MemberNotNullWhen(true, nameof(F1), nameof(F2), nameof(F3), nameof(P1), nameof(P2), nameof(P3))]
      [AttributeListTestCase(@"[MemberNotNullWhen(true, new string[] { ""F1"", ""F2"", ""F3"", ""P1"", ""P2"", ""P3"" })]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNullWhen(true, new string[] { ""F2"", ""F3"", ""P2"", ""P3"" })]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public bool MemberNotNullWhenWithMembers()
        => true;

      [MemberNotNullWhen(true, nameof(F1), nameof(P1))]
      [AttributeListTestCase(@"[MemberNotNullWhen(true, new string[] { ""F1"", ""P1"" })]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = false)]
      [AttributeListTestCase(@"[MemberNotNullWhen(true)]", AttributeWithNamespace = false, AttributeOmitInaccessibleMembersInNullStateAttribute = true)]
      public bool MemberNotNullWhenWithMembersAllNonPublic()
        => true;
#pragma warning restore CS8775
    }
  }
#nullable restore
#endif
}
