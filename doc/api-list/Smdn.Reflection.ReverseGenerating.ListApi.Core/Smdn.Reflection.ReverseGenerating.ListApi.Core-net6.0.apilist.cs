// Smdn.Reflection.ReverseGenerating.ListApi.Core.dll (Smdn.Reflection.ReverseGenerating.ListApi.Core-1.0.0 (net6.0))
//   Name: Smdn.Reflection.ReverseGenerating.ListApi.Core
//   AssemblyVersion: 1.0.0.0
//   InformationalVersion: 1.0.0 (net6.0)
//   TargetFramework: .NETCoreApp,Version=v6.0
//   Configuration: Release

using System;
using System.Collections.Generic;
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
    }

    public ApiListWriterOptions() {}

    public ApiListWriterOptions.WriterOptions Writer { get; }
  }

  public static class AssemblyExtensions {
    public static TValue GetAssemblyMetadataAttributeValue<TAssemblyMetadataAttribute, TValue>(this Assembly assm) where TAssemblyMetadataAttribute : Attribute {}
  }

  public static class AssemblyLoader {
    public static TResult UsingAssembly<TArg, TResult>(FileInfo assemblyFile, bool loadIntoReflectionOnlyContext, TArg arg, Func<Assembly, TArg, TResult> actionWithLoadedAssembly, out WeakReference context, ILogger logger = null) {}
  }

  public static class AttributeFilter {
    public static readonly AttributeTypeFilter Default; // = "Smdn.Reflection.ReverseGenerating.AttributeTypeFilter"
  }

  public static class FrameworkMonikers {
    public static bool TryGetMoniker(FrameworkName frameworkName, string osSpecifier, out string frameworkMoniker) {}
  }

  public class MemberInfoComparer : IComparer<MemberInfo> {
    public static readonly MemberInfoComparer Default; // = "Smdn.Reflection.ReverseGenerating.ListApi.MemberInfoComparer"
    public static readonly MemberInfoComparer StaticMembersFirst; // = "Smdn.Reflection.ReverseGenerating.ListApi.MemberInfoComparer"

    public MemberInfoComparer(int orderOfStaticMember, int orderOfInstanceMember) {}

    public int Compare(MemberInfo x, MemberInfo y) {}
    public int GetOrder(MemberInfo member) {}
  }
}

