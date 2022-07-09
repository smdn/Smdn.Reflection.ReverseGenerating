// Smdn.Reflection.ReverseGenerating.ListApi.Core.dll (Smdn.Reflection.ReverseGenerating.ListApi.Core-1.1.1)
//   Name: Smdn.Reflection.ReverseGenerating.ListApi.Core
//   AssemblyVersion: 1.1.1.0
//   InformationalVersion: 1.1.1+c427f8c147936a9aec90fa59918071264114150c
//   TargetFramework: .NETCoreApp,Version=v6.0
//   Configuration: Release

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Smdn.Reflection.ReverseGenerating;
using Smdn.Reflection.ReverseGenerating.ListApi;

namespace Smdn.Reflection.ReverseGenerating.ListApi {
  [Nullable(byte.MinValue)]
  [NullableContext(1)]
  public class ApiListWriter {
    public ApiListWriter(TextWriter baseWriter, Assembly assembly, ApiListWriterOptions options) {}

    public TextWriter BaseWriter { get; }

    public void WriteAssemblyInfoHeader() {}
    public void WriteExportedTypes() {}
  }

  [Nullable(byte.MinValue)]
  [NullableContext(1)]
  public class ApiListWriterOptions : GeneratorOptions {
    [NullableContext(byte.MinValue)]
    public class WriterOptions {
      public WriterOptions() {}

      public bool OrderStaticMembersFirst { get; set; }
      public bool WriteNullableAnnotationDirective { get; set; }
    }

    public ApiListWriterOptions() {}

    public ApiListWriterOptions.WriterOptions Writer { get; }
  }

  public static class AssemblyExtensions {
    [NullableContext(1)]
    [return: MaybeNull] public static TValue GetAssemblyMetadataAttributeValue<TAssemblyMetadataAttribute, TValue>(this Assembly assm) where TAssemblyMetadataAttribute : Attribute {}
  }

  [Nullable(byte.MinValue)]
  [NullableContext(1)]
  public static class AssemblyLoader {
    [return: MaybeNull] public static TResult UsingAssembly<TArg, TResult>(FileInfo assemblyFile, bool loadIntoReflectionOnlyContext, TArg arg, Func<Assembly, TArg, TResult> actionWithLoadedAssembly, [Nullable(2)] out WeakReference context, [Nullable(2)] ILogger logger = null) {}
  }

  [Nullable(byte.MinValue)]
  [NullableContext(1)]
  public static class AttributeFilter {
    public static readonly AttributeTypeFilter Default; // = "Smdn.Reflection.ReverseGenerating.AttributeTypeFilter"
  }

  public static class FrameworkMonikers {
    [NullableContext(2)]
    public static bool TryGetMoniker([Nullable(1)] FrameworkName frameworkName, string osSpecifier, [NotNullWhen(true)] out string frameworkMoniker) {}
  }

  [Nullable(byte.MinValue)]
  [NullableContext(1)]
  public class MemberInfoComparer : IComparer<MemberInfo> {
    public static readonly MemberInfoComparer Default; // = "Smdn.Reflection.ReverseGenerating.ListApi.MemberInfoComparer"
    public static readonly MemberInfoComparer StaticMembersFirst; // = "Smdn.Reflection.ReverseGenerating.ListApi.MemberInfoComparer"

    public MemberInfoComparer(int orderOfStaticMember, int orderOfInstanceMember) {}

    [NullableContext(2)]
    public int Compare(MemberInfo x, MemberInfo y) {}
    [NullableContext(2)]
    public int GetOrder(MemberInfo member) {}
  }
}

