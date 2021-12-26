// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;
using NUnit.Framework;

[assembly: Smdn.Reflection.ReverseGenerating.ListApi.AssemblyStringMetadata("foo")]
[assembly: Smdn.Reflection.ReverseGenerating.ListApi.AssemblyIntMetadata(42)]

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[AttributeUsage(AttributeTargets.Assembly)]
internal class AssemblyStringMetadataAttribute : Attribute {
  public string Value { get; }

  public AssemblyStringMetadataAttribute(string value)
  {
    Value = value;
  }
}

[AttributeUsage(AttributeTargets.Assembly)]
internal class AssemblyIntMetadataAttribute : Attribute {
  public int Value { get; }

  public AssemblyIntMetadataAttribute(int value)
  {
    Value = value;
  }
}

[TestFixture]
class AssemblyExtensionsTests {
  [Test]
  public void GetAssemblyMetadataAttributeValue()
  {
    var assm = Assembly.GetExecutingAssembly();

    Assert.AreEqual("foo", assm.GetAssemblyMetadataAttributeValue<AssemblyStringMetadataAttribute, string>(), nameof(AssemblyStringMetadataAttribute));
    Assert.AreEqual(42, assm.GetAssemblyMetadataAttributeValue<AssemblyIntMetadataAttribute, int>(), nameof(AssemblyIntMetadataAttribute));

    Assert.IsNotNull(assm.GetAssemblyMetadataAttributeValue<System.Runtime.Versioning.TargetFrameworkAttribute, string>(), nameof(System.Runtime.Versioning.TargetFrameworkAttribute));
    Assert.IsNotNull(assm.GetAssemblyMetadataAttributeValue<System.Reflection.AssemblyConfigurationAttribute, string>(), nameof(System.Reflection.AssemblyConfigurationAttribute));
  }
}
