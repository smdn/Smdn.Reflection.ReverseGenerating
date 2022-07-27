// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CS8597

using System;

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
}
