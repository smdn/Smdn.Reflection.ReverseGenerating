// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Runtime.CompilerServices;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class ExtensionGroupingTypeDeclarationTestCaseAttribute : GeneratorTestCaseAttribute {
  public ExtensionGroupingTypeDeclarationTestCaseAttribute(
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
