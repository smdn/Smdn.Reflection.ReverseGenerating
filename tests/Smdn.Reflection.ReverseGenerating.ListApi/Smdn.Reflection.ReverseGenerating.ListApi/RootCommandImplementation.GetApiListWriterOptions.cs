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

    return impl.GetApiListWriterOptions(
      args.Split(
#if SYSTEM_STRING_SPLIT_STRING && SYSTEM_STRINGSPLITOPTIONS_TRIMENTRIES
        " ",
        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
#else
        new[] { ' ' },
        StringSplitOptions.RemoveEmptyEntries
#endif
      )
    );
  }

  [TestCase("--generate-fulltypename", true)]
  [TestCase("--generate-fulltypename=true", true)]
  [TestCase("--generate-fulltypename=false", false)]
  [TestCase("", false)]
  public void GetApiListWriterOptions_GenerateFullTypeName(string args, bool expected)
  {
    var options = GetApiListWriterOptions(args);

    Assert.AreEqual(expected, options.TypeDeclaration.WithNamespace, $"args='{args}'");
    Assert.AreEqual(expected, options.MemberDeclaration.WithNamespace, $"args='{args}'");
  }

  [TestCase("--generate-methodbody=EmptyImplementation", MethodBodyOption.EmptyImplementation)]
  [TestCase("--generate-methodbody=ThrowNotImplementedException", MethodBodyOption.ThrowNotImplementedException)]
  [TestCase("--generate-methodbody=None", MethodBodyOption.None)]
  [TestCase("", MethodBodyOption.EmptyImplementation)]
  public void GetApiListWriterOptions_GenerateMethodBody(string args, MethodBodyOption expected)
  {
    var options = GetApiListWriterOptions(args);

    Assert.AreEqual(expected, options.MemberDeclaration.MethodBody, $"args='{args}'");
  }

  [TestCase("--generate-staticmembersfirst", true)]
  [TestCase("--generate-staticmembersfirst=true", true)]
  [TestCase("--generate-staticmembersfirst=false", false)]
  [TestCase("", false)]
  public void GetApiListWriterOptions_GenerateStaticMembersFirst(string args, bool expected)
  {
    var options = GetApiListWriterOptions(args);

    Assert.AreEqual(expected, options.Writer.OrderStaticMembersFirst, $"args='{args}'");
  }

  [TestCase("--generate-nullableannotations", true)]
  [TestCase("--generate-nullableannotations=true", true)]
  [TestCase("--generate-nullableannotations=false", false)]
  [TestCase("", true)]
  public void GetApiListWriterOptions_GenerateNullableAnnotations(string args, bool expected)
  {
    var options = GetApiListWriterOptions(args);

    Assert.AreEqual(expected, options.Writer.WriteNullableAnnotationDirective, $"args='{args}'");
  }
}
