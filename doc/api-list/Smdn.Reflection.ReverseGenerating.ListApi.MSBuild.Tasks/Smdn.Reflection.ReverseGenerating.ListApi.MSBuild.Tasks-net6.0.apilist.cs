// Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks.dll (Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks-1.1.7)
//   Name: Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks
//   AssemblyVersion: 1.1.7.0
//   InformationalVersion: 1.1.7+87a650b48f0ea257aa3448bcfafcd60232369ad5
//   TargetFramework: .NETCoreApp,Version=v6.0
//   Configuration: Release
//   Referenced assemblies:
//     Microsoft.Build.Framework, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     Microsoft.Build.Utilities.Core, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     Microsoft.Extensions.Logging.Abstractions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Smdn.Reflection.ReverseGenerating, Version=1.1.4.0, Culture=neutral
//     Smdn.Reflection.ReverseGenerating.ListApi.Core, Version=1.2.0.0, Culture=neutral
//     System.Collections, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Text.Encoding.Extensions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
#nullable enable annotations

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks {
  public class GenerateApiList : Task {
    public GenerateApiList() {}

    [Required]
    public ITaskItem[]? Assemblies { get; set; }
    public bool GenerateAssemblyInfo { get; set; }
    public bool GenerateAttributeWithNamedArguments { get; set; }
    public bool GenerateEmbeddedResources { get; set; }
    public bool GenerateFullTypeName { get; set; }
    public bool GenerateLanguagePrimitiveType { get; set; }
    public string? GenerateMethodBody { get; set; }
    public bool GenerateNullableAnnotations { get; set; }
    public bool GenerateReferencedAssemblies { get; set; }
    public bool GenerateStaticMembersFirst { get; set; }
    public bool GenerateTypeNameWithDeclaringTypeName { get; set; }
    public bool GenerateValueWithDefaultLiteral { get; set; }
    [Output]
    public ITaskItem[]? GeneratedFiles { get; }

    public override bool Execute() {}
  }
}
// API list generated by Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks v1.1.7.0.
// Smdn.Reflection.ReverseGenerating.ListApi.Core v1.2.0.0 (https://github.com/smdn/Smdn.Reflection.ReverseGenerating)
