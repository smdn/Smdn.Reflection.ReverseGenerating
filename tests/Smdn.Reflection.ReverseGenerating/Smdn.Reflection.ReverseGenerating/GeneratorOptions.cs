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

    Assert.IsNotNull(clone);
    Assert.AreNotSame(clone, options);

    options.Indent = "1";

    Assert.AreEqual("1", options.Indent);

    clone.Indent = "2";

    Assert.AreEqual("1", options.Indent);
    Assert.AreEqual("2", clone.Indent);

    options.Indent = "3";

    Assert.AreEqual("3", options.Indent);
    Assert.AreEqual("2", clone.Indent);

    clone.Indent = "4";

    Assert.AreEqual("3", options.Indent);
    Assert.AreEqual("4", clone.Indent);

    Assert.AreNotSame(clone.TypeDeclaration, options.TypeDeclaration, nameof(GeneratorOptions.TypeDeclaration));
    Assert.AreNotSame(clone.MemberDeclaration, options.MemberDeclaration, nameof(GeneratorOptions.MemberDeclaration));
    Assert.AreNotSame(clone.AttributeDeclaration, options.AttributeDeclaration, nameof(GeneratorOptions.AttributeDeclaration));
    Assert.AreNotSame(clone.ValueDeclaration, options.ValueDeclaration, nameof(GeneratorOptions.ValueDeclaration));
    Assert.AreNotSame(clone.ParameterDeclaration, options.ParameterDeclaration, nameof(GeneratorOptions.ParameterDeclaration));
  }
}
