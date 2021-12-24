// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#nullable enable

using System;
using System.Reflection;

namespace Smdn.Reflection;

internal static class FieldInfoValueExtensions {
  public static bool TryGetValue(this FieldInfo f, object? obj, out object? value)
  {
    value = default;

    try {
      value = f.GetValue(obj);

      return true;
    }
    catch (InvalidOperationException) {
      // InvalidOperationException will be thrown in case of loading with MetadataLoadContext.
      return false;
    }
  }
}
