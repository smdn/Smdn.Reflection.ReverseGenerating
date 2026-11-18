// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

public class CNestedEnumWithoutDefaultValueMember {
  public enum E {
    // Default = default,
    NonDefault = 1
  }
}
