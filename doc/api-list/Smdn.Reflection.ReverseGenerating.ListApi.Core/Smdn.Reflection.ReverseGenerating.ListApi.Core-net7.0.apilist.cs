// Smdn.Reflection.ReverseGenerating.ListApi.Core.dll (Smdn.Reflection.ReverseGenerating.ListApi.Core-1.1.3)
//   Name: Smdn.Reflection.ReverseGenerating.ListApi.Core
//   AssemblyVersion: 1.1.3.0
//   InformationalVersion: 1.1.3+838f22ef3a8aec668070f1ca9e1e4688974dad9c
//   TargetFramework: .NETCoreApp,Version=v7.0
//   Configuration: Release
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
    public ApiListWriter(TextWriter baseWriter, Assembly assembly, ApiListWriterOptions options) {}

    public TextWriter BaseWriter { get; }

    public void WriteAssemblyInfoHeader() {}
    public void WriteExportedTypes() {}
  }

  public class ApiListWriterOptions : GeneratorOptions {
    public class WriterOptions {
      public WriterOptions() {}

      public bool OrderStaticMembersFirst { get; set; }
      public bool WriteEmbeddedResources { get; set; }
      public bool WriteNullableAnnotationDirective { get; set; }
    }

    public ApiListWriterOptions() {}

    public ApiListWriterOptions.WriterOptions Writer { get; }
  }

  public static class AssemblyExtensions {
    [return: MaybeNull] public static TValue GetAssemblyMetadataAttributeValue<TAssemblyMetadataAttribute, TValue>(this Assembly assm) where TAssemblyMetadataAttribute : Attribute {}
  }

  public static class AssemblyLoader {
    [return: MaybeNull] public static TResult UsingAssembly<TArg, TResult>(FileInfo assemblyFile, bool loadIntoReflectionOnlyContext, TArg arg, Func<Assembly, TArg, TResult> actionWithLoadedAssembly, out WeakReference? context, ILogger? logger = null) {}
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