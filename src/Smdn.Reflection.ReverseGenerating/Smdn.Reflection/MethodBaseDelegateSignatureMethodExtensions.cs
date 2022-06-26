// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;

namespace Smdn.Reflection;

internal static class MethodBaseDelegateSignatureMethodExtensions {
  public static bool IsDelegateSignatureMethod(this MethodBase m)
    => (m ?? throw new ArgumentNullException(nameof(m))) == m.DeclaringType?.GetDelegateSignatureMethod();
}
