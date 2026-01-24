// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class FieldInfoRequiredMemberExtensions {
  /// <param name="member">Must be an instance of <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</param>
  internal static bool HasRequiredMemberAttribute(MemberInfo member)
    => member.GetCustomAttributesData().Any(
      static d => "System.Runtime.CompilerServices.RequiredMemberAttribute".Equals(d.AttributeType.FullName, StringComparison.Ordinal)
    );

  /// <seealso href="https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-11.0/required-members">
  /// Feature specifications - Required Members
  /// </seealso>
  public static bool IsRequired(this FieldInfo f)
  {
    if (f is null)
      throw new ArgumentNullException(nameof(f));

    // following modifiers cannot be combined with `required`
    if (f.IsStatic) // `static`
      return false;
    if (f.IsInitOnly) // `readonly`
      return false;
    if (f.IsLiteral) // `const`
      return false;
    if (f.FieldType.IsByRef) // `ref` and `ref readonly`
      return false;

    return HasRequiredMemberAttribute(f);
  }
}
