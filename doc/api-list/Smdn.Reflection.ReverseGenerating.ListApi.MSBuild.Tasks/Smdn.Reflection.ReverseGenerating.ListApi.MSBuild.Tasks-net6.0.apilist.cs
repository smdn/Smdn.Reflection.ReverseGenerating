// Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks.dll (Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks-1.0.0 (net6.0))
//   Name: Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks
//   AssemblyVersion: 1.0.0.0
//   InformationalVersion: 1.0.0 (net6.0)
//   TargetFramework: .NETCoreApp,Version=v6.0
//   Configuration: Release

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks {
  [Nullable(byte.MinValue)]
  [NullableContext(2)]
  public class GenerateApiList : Task {
    public GenerateApiList() {}

    [Required]
    [Nullable]
    public ITaskItem[] Assemblies { get; set; }
    public bool GenerateFullTypeName { get; set; }
    public string GenerateMethodBody { get; set; }
    public bool GenerateStaticMembersFirst { get; set; }
    [Output]
    [Nullable]
    public ITaskItem[] GeneratedFiles { get; }

    public override bool Execute() {}
  }
}

