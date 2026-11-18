// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class ExtensionGroupingTypeReferencingNamespacesTestCaseAttribute : GeneratorTestCaseAttribute {
  public ExtensionGroupingTypeReferencingNamespacesTestCaseAttribute(
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
