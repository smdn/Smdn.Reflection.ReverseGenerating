// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;

namespace Smdn.Reflection;

internal static class MethodInfoMethodImplAsyncExtensions {
  extension(MethodInfo method) {
    public bool IsRuntimeAsync {
      get {
        if (method is null)
          throw new ArgumentNullException(nameof(method));

#if NET11_0_OR_GREATER
        return method.MethodImplementationFlags.HasFlag(MethodImplAttributes.Async);
#else
        return false;
#endif
      }
    }
  }
}
