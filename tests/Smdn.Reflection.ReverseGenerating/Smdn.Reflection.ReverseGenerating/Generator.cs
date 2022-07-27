// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating;

[TestFixture]
public partial class GeneratorTests {
  private static IEnumerable<Type> GetTestCaseTypes()
    => Assembly
      .GetExecutingAssembly()
      .GetTypes()
      .Where(static t => t.FullName!.StartsWith(GeneratorTestCases.NS.Namespace));

  internal static bool ExceptTestCaseAttributeFilter(Type type, ICustomAttributeProvider _)
    => !typeof(ITestCaseAttribute).IsAssignableFrom(type);
}
