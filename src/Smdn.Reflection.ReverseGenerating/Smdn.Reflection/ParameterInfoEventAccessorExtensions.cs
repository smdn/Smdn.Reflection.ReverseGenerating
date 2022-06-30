// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

internal static class ParameterInfoEventAccessorExtensions {
  public static EventInfo? GetDeclaringEvent(this ParameterInfo para)
  {
    if (para is null)
      throw new ArgumentNullException(nameof(para));
    if (para.Member is not MethodInfo accessor)
      return null;
    if (!accessor.IsSpecialName) // event accessor method must have special name
      return null;
    if (accessor.DeclaringType is null)
      return null;

    var events = accessor.DeclaringType.GetEvents(
      BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
    );

    return events.FirstOrDefault(ev => accessor == ev.AddMethod || accessor == ev.RemoveMethod);
  }
}
