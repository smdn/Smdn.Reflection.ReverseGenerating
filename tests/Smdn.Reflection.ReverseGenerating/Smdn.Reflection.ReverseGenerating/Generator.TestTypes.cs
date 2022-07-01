// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Runtime.CompilerServices;

namespace Smdn.Reflection.ReverseGenerating.TestTypes;

public static class NS {
  public const string Namespace = "Smdn.Reflection.ReverseGenerating.TestTypes";

  [ModuleInitializer]
  public static void ValidateConstant()
  {
    if (!string.Equals(Namespace, typeof(NS).Namespace, StringComparison.Ordinal))
      throw new InvalidOperationException("constant value of namespace does not match with its actual declaration");
  }
}

public class C {
  public enum NestedEnumWithoutDefaultValueMember {
    // Default = default,
    NonDefault = 1
  }
}
