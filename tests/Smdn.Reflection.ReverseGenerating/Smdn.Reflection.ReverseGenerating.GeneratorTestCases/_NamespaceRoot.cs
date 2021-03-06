// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
#if SYSTEM_RUNTIME_COMPILERSERVICES_MODULEINITIALIZERATTRIBUTE
using System.Runtime.CompilerServices;
#endif

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

public static class NS {
  public const string Namespace = "Smdn.Reflection.ReverseGenerating.GeneratorTestCases";

#if SYSTEM_RUNTIME_COMPILERSERVICES_MODULEINITIALIZERATTRIBUTE
  [ModuleInitializer]
  public static void ValidateConstant()
  {
    if (!string.Equals(Namespace, typeof(NS).Namespace, StringComparison.Ordinal))
      throw new InvalidOperationException("constant value of namespace does not match with its actual declaration");
  }
#endif
}

public class CNestedEnumWithoutDefaultValueMember {
  public enum E {
    // Default = default,
    NonDefault = 1
  }
}
