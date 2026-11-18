// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

using Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

namespace Smdn.Reflection.ReverseGenerating;

[TestFixture]
public partial class GeneratorTests {
  private static IEnumerable<Type> GetTestCaseTypes()
    =>
      new Assembly[] {
        Assembly.GetExecutingAssembly(),
#if NET11_0_OR_GREATER
        typeof(Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.RuntimeAsync.AsyncTaskMethods).Assembly,
#endif
      }
      .SelectMany(
        static assm => assm
          .GetTypes()
          .Where(static t => t.FullName!.StartsWith(GeneratorTestCases.NS.Namespace, StringComparison.Ordinal))
      );

  internal static bool ExceptTestCaseAttributeFilter(Type type, ICustomAttributeProvider _)
    => !typeof(ITestCaseAttribute).IsAssignableFrom(type);
}
