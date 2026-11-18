// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
public class SkipTestCaseAttribute : Attribute, ITestCaseAttribute {
  public string Reason { get; init; }

  public SkipTestCaseAttribute(string reason)
  {
    this.Reason = reason;
  }

  public void Throw() => throw new global::NUnit.Framework.IgnoreException(Reason);
}
