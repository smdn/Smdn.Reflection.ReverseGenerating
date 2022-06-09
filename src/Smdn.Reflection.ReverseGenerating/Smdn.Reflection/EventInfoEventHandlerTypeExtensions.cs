// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;

namespace Smdn.Reflection;

internal static class EventInfoEventHandlerTypeExtensions {
  public static Type GetEventHandlerTypeOrThrow(this EventInfo ev)
  {
    if (ev is null)
      throw new ArgumentNullException(nameof(ev));

    return ev.EventHandlerType
      ?? throw new InvalidOperationException($"can not get event handler type of {ev}");
  }
}
