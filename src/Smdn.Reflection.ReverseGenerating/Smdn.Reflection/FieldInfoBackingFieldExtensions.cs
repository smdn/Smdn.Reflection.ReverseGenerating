// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class FieldInfoBackingFieldExtensions {
  public static bool IsPropertyBackingField(this FieldInfo f)
    => f is null
      ? throw new ArgumentNullException(nameof(f))
      : f.DeclaringType is not null &&
        f.DeclaringType.GetProperties().Any(p => p.GetBackingField() == f); // TODO: optimize
}
