// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static class AssemblyExtensions {
  public static TValue GetAssemblyMetadataAttributeValue<TAssemblyMetadataAttribute, TValue>(this Assembly assm)
    where TAssemblyMetadataAttribute : Attribute
    => (TValue)assm
      ?.GetCustomAttributesData()
      ?.FirstOrDefault(static d => ROCType.FullNameEquals(typeof(TAssemblyMetadataAttribute), d.AttributeType))
      ?.ConstructorArguments
      ?.FirstOrDefault()
      .Value;
}
