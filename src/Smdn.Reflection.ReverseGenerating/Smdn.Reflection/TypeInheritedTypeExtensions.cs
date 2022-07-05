// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class TypeInheritedTypeExtensions {
  public static bool IsHidingInheritedType(this Type t, bool nonPublic)
    => MemberInfoInheritedMemberExtensions.IsHidingInheritedMember(t, nonPublic);
}
