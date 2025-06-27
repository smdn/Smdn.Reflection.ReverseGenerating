// SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;

using Smdn.Reflection.Attributes;

using ROCType = Smdn.Reflection.ReverseGenerating.ListApi.ROCType;

namespace Smdn.Reflection;

internal static class MethodInfoRecordExtensions {
  internal static bool IsCompilerGeneratedRecordEqualityMethod(this MethodInfo m)
  {
    if (m is null)
      throw new ArgumentNullException(nameof(m));

    if (m.GetAccessibility() != Accessibility.Public)
      return false;
    if (!m.HasCompilerGeneratedAttribute())
      return false;

    var parameters = m.GetParameters();

    // is "[CompilerGenerated] public bool Equals" ?
    if (
      string.Equals(m.Name, nameof(Equals), StringComparison.Ordinal) &&
      ROCType.Equals(m.ReturnType, typeof(bool))
    ) {
      if (parameters.Length != 1)
        return false;

      var firstParameterType = parameters[0].ParameterType;

      // is "[CompilerGenerated] public bool Equals(object? obj)" ?
      if (ROCType.Equals(firstParameterType, typeof(object)))
        return true;

      // is "[CompilerGenerated] public bool Equals(TRecord? obj)" ?
      if (firstParameterType == m.DeclaringType)
        return true;

      // is "[CompilerGenerated] public bool Equals(TRecordBase? obj)" ?
      if (
        m.DeclaringType?.BaseType is Type baseType &&
        baseType.IsRecord() &&
        firstParameterType == baseType
      ) {
        return true;
      }
    }

    // is "[CompilerGenerated] public int GetHashCode()" ?
    if (
      string.Equals(m.Name, nameof(GetHashCode), StringComparison.Ordinal) &&
      ROCType.Equals(m.ReturnType, typeof(int)) &&
      parameters.Length == 0
    ) {
      return true;
    }

    // is "[CompilerGenerated] public static bool ??? (T left, T right)" ?
    if (
      m.IsStatic &&
      ROCType.Equals(m.ReturnType, typeof(bool)) &&
      parameters.Length == 2
    ) {
      // is not "(TRecord left, TRecord right)" ?
      if (!(parameters[0].ParameterType == m.DeclaringType && parameters[1].ParameterType == m.DeclaringType))
        return false;

      // [CompilerGenerated] public static bool operator ==(TRecord left, TRecord right)
      if (m.Name.Equals("op_Equality", StringComparison.Ordinal))
        return true;

      // [CompilerGenerated] public static bool operator !=(TRecord left, TRecord right)
      if (m.Name.Equals("op_Inequality", StringComparison.Ordinal))
        return true;
    }

    return false;
  }
}
