// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#nullable enable

using System;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating;

[TestFixture]
public class GeneratorOptionsTests {
  [Test]
  public void Clone()
  {
    var options = new GeneratorOptions();
    var clone = options.Clone();

    Assert.That(clone, Is.Not.Null);
    Assert.That(options, Is.Not.SameAs(clone));

    options.Indent = "1";

    Assert.That(options.Indent, Is.EqualTo("1"));

    clone.Indent = "2";

    Assert.That(options.Indent, Is.EqualTo("1"));
    Assert.That(clone.Indent, Is.EqualTo("2"));

    options.Indent = "3";

    Assert.That(options.Indent, Is.EqualTo("3"));
    Assert.That(clone.Indent, Is.EqualTo("2"));

    clone.Indent = "4";

    Assert.That(options.Indent, Is.EqualTo("3"));
    Assert.That(clone.Indent, Is.EqualTo("4"));

    Assert.That(options.TypeDeclaration, Is.Not.SameAs(clone.TypeDeclaration), nameof(GeneratorOptions.TypeDeclaration));
    Assert.That(options.MemberDeclaration, Is.Not.SameAs(clone.MemberDeclaration), nameof(GeneratorOptions.MemberDeclaration));
    Assert.That(options.AttributeDeclaration, Is.Not.SameAs(clone.AttributeDeclaration), nameof(GeneratorOptions.AttributeDeclaration));
    Assert.That(options.ValueDeclaration, Is.Not.SameAs(clone.ValueDeclaration), nameof(GeneratorOptions.ValueDeclaration));
    Assert.That(options.ParameterDeclaration, Is.Not.SameAs(clone.ParameterDeclaration), nameof(GeneratorOptions.ParameterDeclaration));
  }
}
