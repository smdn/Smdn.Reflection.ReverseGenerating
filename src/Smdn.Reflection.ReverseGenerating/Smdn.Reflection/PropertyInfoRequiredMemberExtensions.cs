// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;

namespace Smdn.Reflection;

internal static class PropertyInfoRequiredMemberExtensions {
  /// <seealso href="https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-11.0/required-members">
  /// Feature specifications - Required Members
  /// </seealso>
  public static bool IsRequired(this PropertyInfo p)
  {
    if (p is null)
      throw new ArgumentNullException(nameof(p));

    if (p.SetMethod is not { } setMethod)
      return false; // read-only property

    // following modifiers cannot be combined with `required`
    if (setMethod.IsStatic) // `static`
      return false;
    if (p.PropertyType.IsByRef) // `ref` and `ref readonly`
      return false;

    return FieldInfoRequiredMemberExtensions.HasRequiredMemberAttribute(p);
  }
}
