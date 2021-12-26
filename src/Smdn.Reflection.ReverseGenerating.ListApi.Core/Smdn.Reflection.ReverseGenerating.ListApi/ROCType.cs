// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

internal static class ROCType {
  public static bool FullNameEquals(Type type, Type t)
    => string.Equals(type.FullName, t.FullName, StringComparison.Ordinal);

  public static bool Equals(Type type, Type t)
    =>
      FullNameEquals(type, t) &&
      string.Equals(type.Assembly.GetName().Name, t.Assembly.GetName().Name, StringComparison.Ordinal);
}
