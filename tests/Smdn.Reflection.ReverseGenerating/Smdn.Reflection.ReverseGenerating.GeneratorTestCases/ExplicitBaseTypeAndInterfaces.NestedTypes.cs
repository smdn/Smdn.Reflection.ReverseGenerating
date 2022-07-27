// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.ExplicitBaseTypeAndInterfaces.NestedTypes;

interface I { }

class C {
  class CN {
    public interface INN { }
  }
  interface IN { }

  [ExplicitBaseTypeAndInterfacesTestCase("CN", TypeWithNamespace = false, TypeWithDeclaringTypeName = false)]
  [ExplicitBaseTypeAndInterfacesTestCase("C.CN", TypeWithNamespace = false, TypeWithDeclaringTypeName = true)]
  class C1 : CN { }

  [ExplicitBaseTypeAndInterfacesTestCase("CN, I, IN", TypeWithNamespace = false, TypeWithDeclaringTypeName = false)]
  [ExplicitBaseTypeAndInterfacesTestCase("C.CN, C.IN, I", TypeWithNamespace = false, TypeWithDeclaringTypeName = true)]
  class C2 : CN, I, IN { }

  [ExplicitBaseTypeAndInterfacesTestCase("CN, I, INN", TypeWithNamespace = false, TypeWithDeclaringTypeName = false)]
  [ExplicitBaseTypeAndInterfacesTestCase("C.CN, C.CN.INN, I", TypeWithNamespace = false, TypeWithDeclaringTypeName = true)]
  class C3 : CN, I, CN.INN { }
}
