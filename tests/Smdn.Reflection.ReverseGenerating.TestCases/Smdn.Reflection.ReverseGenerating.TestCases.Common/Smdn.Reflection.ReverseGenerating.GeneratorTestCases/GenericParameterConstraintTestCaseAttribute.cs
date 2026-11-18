// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Runtime.CompilerServices;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

[AttributeUsage(
  AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Delegate | AttributeTargets.Method,
  AllowMultiple = true,
  Inherited = false
)]
public class GenericParameterConstraintTestCaseAttribute : GeneratorTestCaseAttribute {
  public GenericParameterConstraintTestCaseAttribute(
    string expected,
    [CallerFilePath] string? sourceFilePath = null,
    [CallerLineNumber] int lineNumber = 0
  )
    : base(
      expected,
      sourceFilePath,
      lineNumber
    )
  {
  }
}
