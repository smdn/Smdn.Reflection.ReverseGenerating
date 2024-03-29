// Smdn.Reflection.ReverseGenerating.ListApi.Core.dll (Smdn.Reflection.ReverseGenerating.ListApi.Core-1.3.1)
//   Name: Smdn.Reflection.ReverseGenerating.ListApi.Core
//   AssemblyVersion: 1.3.1.0
//   InformationalVersion: 1.3.1+2045310a22e5222b2462fc1f376ef4522587ec1e
//   TargetFramework: .NETCoreApp,Version=v6.0
//   Configuration: Release
//   Referenced assemblies:
//     Microsoft.Extensions.DependencyModel, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Microsoft.Extensions.Logging.Abstractions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Smdn.Fundamental.Reflection, Version=3.6.0.0, Culture=neutral
//     Smdn.Reflection.ReverseGenerating, Version=1.3.0.0, Culture=neutral
//     System.Collections, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Linq, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Reflection.Metadata, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Reflection.MetadataLoadContext, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
//     System.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Runtime.InteropServices, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Runtime.Loader, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
#nullable enable annotations

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Smdn.Reflection.ReverseGenerating;
using Smdn.Reflection.ReverseGenerating.ListApi;

namespace Smdn.Reflection.ReverseGenerating.ListApi {
  public class ApiListWriter {
    public ApiListWriter(TextWriter baseWriter, Assembly assembly, ApiListWriterOptions? options) {}
    public ApiListWriter(TextWriter baseWriter, Assembly assembly, ApiListWriterOptions? options, ILogger? logger) {}

    public TextWriter BaseWriter { get; }

    [Obsolete("Use WriteHeader")]
    public void WriteAssemblyInfoHeader() {}
    public void WriteExportedTypes() {}
    public void WriteFooter() {}
    public void WriteHeader() {}
  }

  public class ApiListWriterOptions : GeneratorOptions {
    public class WriterOptions {
      public WriterOptions() {}

      public bool OrderStaticMembersFirst { get; set; }
      public bool ThrowIfForwardedTypesCouldNotLoaded { get; set; }
      public bool WriteAssemblyInfo { get; set; }
      public bool WriteEmbeddedResources { get; set; }
      public bool WriteFooter { get; set; }
      public bool WriteHeader { get; set; }
      public bool WriteNullableAnnotationDirective { get; set; }
      public bool WriteReferencedAssemblies { get; set; }
    }

    public ApiListWriterOptions() {}

    public ApiListWriterOptions.WriterOptions Writer { get; }
  }

  public static class AssemblyExtensions {
    [return: MaybeNull] public static TValue GetAssemblyMetadataAttributeValue<TAssemblyMetadataAttribute, TValue>(this Assembly assm) where TAssemblyMetadataAttribute : Attribute {}
  }

  public sealed class AssemblyFileNotFoundException : FileNotFoundException {
    public AssemblyFileNotFoundException() {}
    public AssemblyFileNotFoundException(string? message) {}
    public AssemblyFileNotFoundException(string? message, Exception? innerException) {}
    public AssemblyFileNotFoundException(string? message, string? fileName, Exception? innerException) {}
  }

  public static class AssemblyLoader {
    [return: MaybeNull] public static TResult UsingAssembly<TArg, TResult>(FileInfo assemblyFile, bool loadIntoReflectionOnlyContext, TArg arg, Func<Assembly, TArg, TResult>? actionWithLoadedAssembly, out WeakReference? context, ILogger? logger = null) {}
    [return: MaybeNull] public static TResult UsingAssembly<TArg, TResult>(Stream assemblyStream, string componentAssemblyPath, bool loadIntoReflectionOnlyContext, TArg arg, Func<Assembly, TArg, TResult> actionWithLoadedAssembly, out WeakReference? context, ILogger? logger = null) {}
  }

  public static class AttributeFilter {
    public static readonly AttributeTypeFilter Default; // = "Smdn.Reflection.ReverseGenerating.AttributeTypeFilter"
  }

  public static class FrameworkMonikers {
    public static bool TryGetMoniker(FrameworkName frameworkName, string? osSpecifier, [NotNullWhen(true)] out string? frameworkMoniker) {}
  }

  public class MemberInfoComparer : IComparer<MemberInfo> {
    public static readonly MemberInfoComparer Default; // = "Smdn.Reflection.ReverseGenerating.ListApi.MemberInfoComparer"
    public static readonly MemberInfoComparer StaticMembersFirst; // = "Smdn.Reflection.ReverseGenerating.ListApi.MemberInfoComparer"

    public MemberInfoComparer(int orderOfStaticMember, int orderOfInstanceMember) {}

    public int Compare(MemberInfo? x, MemberInfo? y) {}
    public int GetOrder(MemberInfo? member) {}
  }
}
// API list generated by Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks v1.4.0.0.
// Smdn.Reflection.ReverseGenerating.ListApi.Core v1.3.0.0 (https://github.com/smdn/Smdn.Reflection.ReverseGenerating)
