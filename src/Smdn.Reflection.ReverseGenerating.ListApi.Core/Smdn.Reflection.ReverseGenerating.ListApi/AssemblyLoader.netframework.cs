// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if NETFRAMEWORK
using System;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

#pragma warning disable IDE0040
partial class AssemblyLoader {
#pragma warning restore IDE0040
  private abstract class LoadAssemblyAppDomainProxyBase : MarshalByRefObject {
    public abstract Assembly LoadAssembly(FileInfo assemblyFile);
  }

  private class ReflectionOnlyLoadAssemblyAppDomainProxy : LoadAssemblyAppDomainProxyBase {
    public override Assembly LoadAssembly(FileInfo assemblyFile)
    {
      // TODO: logging
      return Assembly.ReflectionOnlyLoadFrom(assemblyFile.FullName);
    }
  }

  private class LoadAssemblyAppDomainProxy : LoadAssemblyAppDomainProxyBase {
    public override Assembly LoadAssembly(FileInfo assemblyFile)
    {
      // TODO: logging
      return Assembly.LoadFrom(assemblyFile.FullName);
    }
  }

  [return: MaybeNull]
  private static TResult UsingAssemblyCore<TArg, TResult>(
    FileInfo assemblyFile,
    bool loadIntoReflectionOnlyContext,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    out WeakReference? context,
    ILogger? logger = null
  )
  {
    context = null;

    var domain = AppDomain.CreateDomain(
      friendlyName: assemblyFile.Name,
      securityInfo: null,
      appBasePath: assemblyFile.Directory.FullName,
      appRelativeSearchPath: ".",
      shadowCopyFiles: false
    );
    var domainWeakReference = new WeakReference(domain);

    context = domainWeakReference;

    string assemblyName = null;

    try {
      var typeOfProxy = loadIntoReflectionOnlyContext
        ? typeof(ReflectionOnlyLoadAssemblyAppDomainProxy)
        : typeof(LoadAssemblyAppDomainProxy);
      var proxy = (LoadAssemblyAppDomainProxyBase)domain.CreateInstanceAndUnwrap(
        assemblyName: typeOfProxy.Assembly.FullName,
        typeName: typeOfProxy.FullName
      );

      logger?.LogDebug($"loading assembly from file '{assemblyFile.FullName}'");

      var assm = proxy.LoadAssembly(assemblyFile);

      if (assm is null) {
        logger?.LogCritical($"failed to load assembly from file '{assemblyFile.FullName}'");

        return default;
      }

      assemblyName = assm.FullName;

      logger?.LogDebug($"loaded assembly '{assemblyName}'");

      return actionWithLoadedAssembly(assm, arg);
    }
    finally {
      AppDomain.Unload(domain);

      logger?.LogDebug($"unloaded assembly '{assemblyName}'");
    }
  }

  [return: MaybeNull]
  private static TResult UsingAssemblyCore<TArg, TResult>(
    Stream assemblyStream,
    string componentAssemblyPath,
    bool loadIntoReflectionOnlyContext,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    out WeakReference? context,
    ILogger? logger = null
  )
    => throw new NotImplementedException();
}
#endif
