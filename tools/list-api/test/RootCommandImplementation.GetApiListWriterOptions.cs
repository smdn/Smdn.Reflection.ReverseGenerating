// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using NUnit.Framework;
using Smdn.Reflection.ReverseGenerating;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
class RootCommandImplementationGetApiListWriterOptionsTests {
  private ApiListWriterOptions GetApiListWriterOptions(string args)
  {
    var impl = new RootCommandImplementation();

    return impl.GetApiListWriterOptions(args.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
  }

  [TestCase("--generate-fulltypename", true)]
  [TestCase("--generate-fulltypename=true", true)]
  [TestCase("--generate-fulltypename=false", false)]
  [TestCase("", false)]
  public void GetApiListWriterOptions_GenerateFullTypeName(string args, bool expected)
  {
    var options = GetApiListWriterOptions(args);

    Assert.AreEqual(expected, options.TypeDeclarationWithNamespace, $"args='{args}'");
    Assert.AreEqual(expected, options.MemberDeclarationWithNamespace, $"args='{args}'");
  }

  [TestCase("--generate-methodbody=EmptyImplementation", MethodBodyOption.EmptyImplementation)]
  [TestCase("--generate-methodbody=ThrowNotImplementedException", MethodBodyOption.ThrowNotImplementedException)]
  [TestCase("--generate-methodbody=None", MethodBodyOption.None)]
  [TestCase("", MethodBodyOption.EmptyImplementation)]
  public void GetApiListWriterOptions_GenerateMethodBody(string args, MethodBodyOption expected)
  {
    var options = GetApiListWriterOptions(args);

    Assert.AreEqual(expected, options.MemberDeclarationMethodBody, $"args='{args}'");
  }

  [TestCase("--generate-staticmembersfirst", true)]
  [TestCase("--generate-staticmembersfirst=true", true)]
  [TestCase("--generate-staticmembersfirst=false", false)]
  [TestCase("", false)]
  public void GetApiListWriterOptions_GenerateStaticMembersFirst(string args, bool expected)
  {
    var options = GetApiListWriterOptions(args);

    Assert.AreEqual(expected, options.WriterOrderStaticMembersFirst, $"args='{args}'");
  }
}