// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;

namespace Smdn.Reflection;

internal static class MemberInfoDeclaringTypeExtensions {
  public static Type GetDeclaringTypeOrThrow(this MemberInfo memberInfo)
  {
    if (memberInfo is null)
      throw new ArgumentNullException(nameof(memberInfo));

    return memberInfo.DeclaringType
      ?? throw new InvalidOperationException($"can not get {nameof(MemberInfo.DeclaringType)} of {memberInfo}");
  }
}
