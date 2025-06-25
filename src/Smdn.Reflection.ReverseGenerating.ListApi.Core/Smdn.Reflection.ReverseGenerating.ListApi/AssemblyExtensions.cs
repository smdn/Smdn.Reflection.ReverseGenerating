// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.IO;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static class AssemblyExtensions {
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
  [return: MaybeNull]
#endif
  // cSpell:ignore assm
  // TODO: change assm -> assembly
  public static TValue GetAssemblyMetadataAttributeValue<TAssemblyMetadataAttribute, TValue>(this Assembly assm)
    where TAssemblyMetadataAttribute : Attribute
    => (TValue)GetAssemblyMetadataAttributeValue<TAssemblyMetadataAttribute>(
        assm ?? throw new ArgumentNullException(nameof(assm))
      )!;

  private static object? GetAssemblyMetadataAttributeValue<TAssemblyMetadataAttribute>(Assembly assm)
    where TAssemblyMetadataAttribute : Attribute
  {
    IList<System.Reflection.CustomAttributeData> attributesData;

    try {
      attributesData = assm.GetCustomAttributesData();
    }
    catch (FileNotFoundException ex) { // when (!string.IsNullOrEmpty(ex.FusionLog))
      // in the case of reference assembly cannot be loaded
      throw AssemblyFileNotFoundException.Create(assm.GetName(), ex);
    }

    return attributesData
      .FirstOrDefault(static d => ROCType.FullNameEquals(typeof(TAssemblyMetadataAttribute), d.AttributeType))
      ?.ConstructorArguments
      ?.FirstOrDefault()
      .Value;
  }
}
