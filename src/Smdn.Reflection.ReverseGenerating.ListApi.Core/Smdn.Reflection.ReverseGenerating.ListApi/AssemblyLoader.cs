// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.IO;
using System.Reflection;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static partial class AssemblyLoader {
  private readonly record struct AssemblySource(
#pragma warning disable SA1313
    string ComponentAssemblyPath,
    FileInfo? File = null,
    Stream? Stream = null
#pragma warning restore SA1313
  );

#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
  [return: MaybeNull]
#endif
  public static TResult UsingAssembly<TArg, TResult>(
    FileInfo assemblyFile,
    bool loadIntoReflectionOnlyContext,
    TArg arg,
    Func<Assembly, TArg, TResult>? actionWithLoadedAssembly,
    out WeakReference? context,
    ILogger? logger = null
  )
  {
    if (assemblyFile is null)
       throw new ArgumentNullException(nameof(assemblyFile));

    return UsingAssemblyCore(
      new(
        ComponentAssemblyPath: assemblyFile.FullName,
        File: assemblyFile
      ),
      loadIntoReflectionOnlyContext,
      arg,
      actionWithLoadedAssembly,
      out context,
      logger
    );
  }

#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
  [return: MaybeNull]
#endif
  public static TResult UsingAssembly<TArg, TResult>(
    Stream assemblyStream,
    string componentAssemblyPath,
    bool loadIntoReflectionOnlyContext,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    out WeakReference? context,
    ILogger? logger = null
  )
  {
    return UsingAssemblyCore(
      new(
        ComponentAssemblyPath: componentAssemblyPath ?? throw new ArgumentNullException(nameof(componentAssemblyPath)),
        Stream: assemblyStream ?? throw new ArgumentNullException(nameof(assemblyStream))
      ),
      loadIntoReflectionOnlyContext,
      arg,
      actionWithLoadedAssembly,
      out context,
      logger
    );
  }
}
