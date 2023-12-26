// Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks.dll (Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks-1.3.2)
//   Name: Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks
//   AssemblyVersion: 1.3.2.0
//   InformationalVersion: 1.3.2+daebfb2f6d9c9523e7683bb2079085163a8f5c67
//   TargetFramework: .NETCoreApp,Version=v3.1
//   Configuration: Release
//   Referenced assemblies:
//     Microsoft.Build.Framework, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     Microsoft.Build.Utilities.Core, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     Microsoft.Extensions.Logging.Abstractions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
//     Smdn.Reflection.ReverseGenerating, Version=1.2.0.0, Culture=neutral
//     Smdn.Reflection.ReverseGenerating.ListApi.Core, Version=1.2.0.0, Culture=neutral
//     System.Collections, Version=4.1.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.IO.FileSystem, Version=4.1.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Runtime.Extensions, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Text.Encoding.Extensions, Version=4.1.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
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
// API list generated by Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks v1.3.2.0.
// Smdn.Reflection.ReverseGenerating.ListApi.Core v1.2.0.0 (https://github.com/smdn/Smdn.Reflection.ReverseGenerating)
