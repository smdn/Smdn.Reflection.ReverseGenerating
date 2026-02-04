// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;

using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating;

// Since the functionality of the implementation is covered by calls from other methods
// and their test cases, there is no need to test Generator.GenerateParameterDeclaration() here.
partial class GeneratorTests {
  [Test]
  public void GenerateParameterDeclaration_ArgumentParameterNull()
    => Assert.That(
      () => Generator.GenerateParameterDeclaration(
        parameter: null!,
        options: new()
      ),
      Throws
        .ArgumentNullException
        .With
        .Property(nameof(ArgumentNullException.ParamName))
        .EqualTo("parameter")
    );

  [Test]
  public void GenerateParameterDeclaration_ArgumentOptionsNull()
  {
    static ParameterInfo GetParameterInfo()
      => typeof(object).GetMethod(nameof(ReferenceEquals))!.GetParameters()[0];

    Assert.That(
      () => Generator.GenerateParameterDeclaration(
        parameter: GetParameterInfo(),
        options: null!
      ),
      Throws
        .ArgumentNullException
        .With
        .Property(nameof(ArgumentNullException.ParamName))
        .EqualTo("options")
    );
  }
}
