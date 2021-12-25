// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static partial class AssemblyLoader {
  public static TResult UsingAssembly<TArg, TResult>(
    FileInfo assemblyFile,
    bool loadIntoReflectionOnlyContext,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    out WeakReference context,
    ILogger logger = null
  )
  {
    return UsingAssemblyCore(
      assemblyFile,
      loadIntoReflectionOnlyContext,
      arg,
      actionWithLoadedAssembly,
      out context,
      logger
    );
  }
}
