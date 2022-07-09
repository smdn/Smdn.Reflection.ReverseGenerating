// Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks.dll (Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks-1.1.0)
//   Name: Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks
//   AssemblyVersion: 1.1.0.0
//   InformationalVersion: 1.1.0+64b5c72527e2ce0705ed5d9ed2328179311a4c94
//   TargetFramework: .NETCoreApp,Version=v6.0
//   Configuration: Release
#nullable enable annotations

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks {
  public class GenerateApiList : Task {
    public GenerateApiList() {}

    [Required]
    public ITaskItem[]? Assemblies { get; set; }
    public bool GenerateFullTypeName { get; set; }
    public string? GenerateMethodBody { get; set; }
    public bool GenerateStaticMembersFirst { get; set; }
    [Output]
    public ITaskItem[]? GeneratedFiles { get; }

    public override bool Execute() {}
  }
}
