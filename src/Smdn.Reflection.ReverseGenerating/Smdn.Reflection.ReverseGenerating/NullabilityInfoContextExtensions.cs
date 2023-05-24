// SPDX-FileCopyrightText: 2023 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
using System;
using System.Reflection;

namespace Smdn.Reflection.ReverseGenerating;

/// <summary>
/// Provides extension methods for <see cref="NullabilityInfoContext"/>.
/// </summary>
/// <remarks>
/// <see cref="NullabilityInfoContext"/> internally caches created <see cref="NullabilityInfo"/> using <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>.
/// This may cause an exception to be thrown if <see cref="NullabilityInfoContext.Create"/> is called concurrently.
/// So, this class provides extension methods that uses a lock object to lock and then calls <see cref="NullabilityInfoContext.Create"/>.
/// </remarks>
internal static class NullabilityInfoContextExtensions {
  internal static NullabilityInfo Create(this NullabilityInfoContext context, FieldInfo field, object? lockObject)
  {
    if (lockObject is null)
      return context.Create(field);

    lock (lockObject) {
      return context.Create(field);
    }
  }

  internal static NullabilityInfo Create(this NullabilityInfoContext context, PropertyInfo property, object? lockObject)
  {
    if (lockObject is null)
      return context.Create(property);

    lock (lockObject) {
      return context.Create(property);
    }
  }

  internal static NullabilityInfo Create(this NullabilityInfoContext context, ParameterInfo parameter, object? lockObject)
  {
    if (lockObject is null)
      return context.Create(parameter);

    lock (lockObject) {
      return context.Create(parameter);
    }
  }

  internal static NullabilityInfo Create(this NullabilityInfoContext context, EventInfo @event, object? lockObject)
  {
    if (lockObject is null)
      return context.Create(@event);

    lock (lockObject) {
      return context.Create(@event);
    }
  }
}
#endif
