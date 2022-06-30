// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class ParameterInfoPropertyAccessorExtensions {
  public static PropertyInfo? GetDeclaringProperty(this ParameterInfo para)
  {
    if (para is null)
      throw new ArgumentNullException(nameof(para));
    if (para.Member is not MethodInfo accessor)
      return null;
    if (!accessor.IsSpecialName) // property accessor method must have special name
      return null;
    if (accessor.DeclaringType is null)
      return null;

    var properties = accessor.DeclaringType.GetProperties(
      BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
    );
    var isReturnParameter =
#if NETFRAMEWORK
      para.Position < 0 && para.ParameterType.Equals(accessor.ReturnType);
#else
      para == accessor.ReturnParameter;
#endif

    return isReturnParameter
      ? properties.FirstOrDefault(property => accessor == property.GetMethod)
      : properties.FirstOrDefault(property => accessor == property.SetMethod);
  }
}
