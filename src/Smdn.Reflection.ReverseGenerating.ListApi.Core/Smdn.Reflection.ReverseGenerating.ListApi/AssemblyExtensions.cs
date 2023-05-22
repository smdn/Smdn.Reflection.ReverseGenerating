// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static class AssemblyExtensions {
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
  [return: MaybeNull]
#endif
  public static TValue GetAssemblyMetadataAttributeValue<TAssemblyMetadataAttribute, TValue>(this Assembly assm)
    where TAssemblyMetadataAttribute : Attribute
    => (TValue)GetAssemblyMetadataAttributeValue<TAssemblyMetadataAttribute>(
        assm ?? throw new ArgumentNullException(nameof(assm))
      )!;

  private static object? GetAssemblyMetadataAttributeValue<TAssemblyMetadataAttribute>(Assembly assm)
    where TAssemblyMetadataAttribute : Attribute
    => assm
      .GetCustomAttributesData()
      .FirstOrDefault(static d => ROCType.FullNameEquals(typeof(TAssemblyMetadataAttribute), d.AttributeType))
      ?.ConstructorArguments
      ?.FirstOrDefault()
      .Value;
}
