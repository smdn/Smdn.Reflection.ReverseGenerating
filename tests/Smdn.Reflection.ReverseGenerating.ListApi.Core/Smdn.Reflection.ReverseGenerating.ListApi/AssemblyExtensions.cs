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

    Assert.That(assm.GetAssemblyMetadataAttributeValue<AssemblyStringMetadataAttribute, string>(), Is.EqualTo("foo"), nameof(AssemblyStringMetadataAttribute));
    Assert.That(assm.GetAssemblyMetadataAttributeValue<AssemblyIntMetadataAttribute, int>(), Is.EqualTo(42), nameof(AssemblyIntMetadataAttribute));

    Assert.That(assm.GetAssemblyMetadataAttributeValue<System.Runtime.Versioning.TargetFrameworkAttribute, string>(), Is.Not.Null, nameof(System.Runtime.Versioning.TargetFrameworkAttribute));
    Assert.That(assm.GetAssemblyMetadataAttributeValue<System.Reflection.AssemblyConfigurationAttribute, string>(), Is.Not.Null, nameof(System.Reflection.AssemblyConfigurationAttribute));
  }
}
