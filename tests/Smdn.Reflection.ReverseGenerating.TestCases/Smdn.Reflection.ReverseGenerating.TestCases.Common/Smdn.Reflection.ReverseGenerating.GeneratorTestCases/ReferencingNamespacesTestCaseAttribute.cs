// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class ReferencingNamespacesTestCaseAttribute : GeneratorTestCaseAttribute {
  public ReferencingNamespacesTestCaseAttribute(
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
    base.MemberWithNamespace = true;
    base.ValueUseDefaultLiteral = false;
  }

  public bool WithExplicitBaseTypeAndInterfaces { get; set; } = false;

  public IReadOnlyList<string> GetExpectedSet()
  {
    if (Expected is null)
      throw new InvalidOperationException("The expected value is set to null.");

#if SYSTEM_STRINGSPLITOPTIONS_TRIMENTRIES
    return Expected.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
#else
    return Expected.Split([','], StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
#endif
  }
}
