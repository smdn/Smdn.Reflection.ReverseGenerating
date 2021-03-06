// Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks.dll (Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks-1.1.4)
//   Name: Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks
//   AssemblyVersion: 1.1.4.0
//   InformationalVersion: 1.1.4+474aa1799ff8a98167dc9760f2acafaa15b81d4c
//   TargetFramework: .NETCoreApp,Version=v3.1
//   Configuration: Release
#nullable enable annotations

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks {
  public class GenerateApiList : Task {
    public GenerateApiList() {}

    [Required]
    public ITaskItem[]? Assemblies { get; set; }
    public bool GenerateAttributeWithNamedArguments { get; set; }
    public bool GenerateFullTypeName { get; set; }
    public bool GenerateLanguagePrimitiveType { get; set; }
    public string? GenerateMethodBody { get; set; }
    public bool GenerateNullableAnnotations { get; set; }
    public bool GenerateStaticMembersFirst { get; set; }
    public bool GenerateTypeNameWithDeclaringTypeName { get; set; }
    public bool GenerateValueWithDefaultLiteral { get; set; }
    [Output]
    public ITaskItem[]? GeneratedFiles { get; }

    public override bool Execute() {}
  }
}
