// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class MemberInfoExplicitInterfaceMemberExtensions {
  /// <summary>
  /// Determines whether the <see cref="MemberInfo"/> is likely an explicit interface implementation
  /// based on its metadata attributes and naming convention.
  /// </summary>
  /// <param name="member">
  /// The <see cref="MemberInfo"/> to be tested.
  /// This can be <see cref="MethodBase"/>, <see cref="PropertyInfo"/>, or <see cref="EventInfo"/>.
  /// </param>
  /// <returns>
  /// <c>true</c> if the <paramref name="member"/> follows the typical metadata pattern of
  /// an explicit implementation;
  /// otherwise, <c>false</c>.
  /// </returns>
  /// <remarks>
  /// <para>
  /// <b>Heuristic Method:</b> This method performs a heuristic check by verifying that the member
  /// is non-public, virtual, and sealed (final), and that its name contains a period (<c>.</c>).
  /// This naming pattern is a convention used by the C# compiler for explicit implementations.
  /// </para>
  /// <para>
  /// <b>Limitations:</b> This is not a definitive verification. Since it relies on naming
  /// conventions and flags rather than <see cref="Type.GetInterfaceMap"/>, it may produce false
  /// positives in rare cases (e.g., assemblies created with custom IL or specific obfuscation).
  /// However, it is suitable for use in contexts like <c>MetadataLoadContext</c>.
  /// This method is primarily optimized for assemblies compiled with C# or VB.NET.
  /// It may not accurately identify explicit implementations in F# or other languages that
  /// do not embed the interface name into the member name.
  /// </para>
  /// </remarks>
  public static bool IsLikelyExplicitInterfaceImplementation(this MemberInfo member)
    => member switch {
      MethodBase method => IsLikelyExplicitInterfaceImplementationMethod(method),
      PropertyInfo prop => prop.GetAccessors(nonPublic: true).Any(IsLikelyExplicitInterfaceImplementationMethod),
      EventInfo ev => ev.GetMethods(nonPublic: true).Any(IsLikelyExplicitInterfaceImplementationMethod),
      _ => false,
    };

  private static bool IsLikelyExplicitInterfaceImplementationMethod(MethodBase m)
  {
    // Explicit implementations are characterized by:
    // 1. Being Virtual and Final (cannot be overridden further).
    // 2. Being Non-Public (usually private or PrivateScope).
    // 3. Having a name containing a dot (Interface.Member), which is invalid for normal C# identifiers.
    if (!m.IsVirtual)
      return false;
    if (!m.IsFinal)
      return false;
    if (m.IsPublic)
      return false;

    // The presence of a period in the name is the primary indicator.
    return m.Name
#if SYSTEM_STRING_CONTAINS_CHAR_STRINGCOMPARISON
      .Contains('.', StringComparison.Ordinal);
#else
      .Contains('.');
#endif
  }
}
