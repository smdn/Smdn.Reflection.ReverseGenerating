// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;

namespace Smdn.Reflection;

internal static class ParameterInfoDefaultValueExtensions {
  public static object? GetDefaultValue(this ParameterInfo p)
  {
    try {
      return p.DefaultValue;
    }
    catch (InvalidOperationException) {
      // InvalidOperationException will be thrown in case of loading with MetadataLoadContext.
      return p.RawDefaultValue;
    }
  }
}
