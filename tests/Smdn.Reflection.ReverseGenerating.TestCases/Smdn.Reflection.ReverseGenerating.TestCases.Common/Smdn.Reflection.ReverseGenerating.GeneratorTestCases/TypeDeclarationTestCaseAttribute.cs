// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Runtime.CompilerServices;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class TypeDeclarationTestCaseAttribute : GeneratorTestCaseAttribute {
  public Type? TestTargetType { get; }

  public TypeDeclarationTestCaseAttribute(
    string expected,
    [CallerFilePath] string? sourceFilePath = null,
    [CallerLineNumber] int lineNumber = 0
  )
    : this(
      expected,
      testTargetType: null,
      sourceFilePath,
      lineNumber
    )
  {
  }

  public TypeDeclarationTestCaseAttribute(
    string expected,
    Type? testTargetType,
    [CallerFilePath] string? sourceFilePath = null,
    [CallerLineNumber] int lineNumber = 0
  )
    : base(
      expected,
      sourceFilePath,
      lineNumber
    )
  {
    this.TestTargetType = testTargetType;
  }
}
