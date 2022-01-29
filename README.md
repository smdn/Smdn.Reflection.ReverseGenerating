[![GitHub license](https://img.shields.io/github/license/smdn/Smdn.Reflection.ReverseGenerating)](https://github.com/smdn/Smdn.Reflection.ReverseGenerating/blob/main/LICENSE.txt)

# Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks
[![NuGet Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks](https://img.shields.io/nuget/v/Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks.svg)](https://www.nuget.org/packages/Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks/)

This package provides `GenerateApiList` MSBuild task.

## Usage
```xml
  <ItemGroup>
    <!-- Add package reference of Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks. -->
    <PackageReference
      Include="Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks"
      Version="1.0.3"
      PrivateAssets="all"
      IncludeAssets="build"
    />
  </ItemGroup>

  <ItemGroup>
    <!-- Specify target assemblies to generate API list. -->
    <GenerateApiListTargetAssemblies Include="..." />
  </ItemGroup>

  <PropertyGroup>
    <!-- (Optional) If you want to import 'Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks.targets' even if the build target is `pack`. -->
    <ExcludeRestorePackageImports>false</ExcludeRestorePackageImports>
  </PropertyGroup>

  <Target Name="GenerateApiListFromAssemblies" AfterTargets="Pack">
    <!-- Call `GenerateApiList` task -->
    <GenerateApiList Assemblies="@(GenerateApiListTargetAssemblies)">
      <Output TaskParameter="GeneratedFiles" ItemName="GeneratedApiListFiles" />
    </GenerateApiList>

    <Message Text="generated API list: @(GeneratedApiListFiles)" Importance="high" />
  </Target>
```

# Smdn.Reflection.ReverseGenerating.ListApi
[![NuGet Smdn.Reflection.ReverseGenerating.ListApi](https://img.shields.io/nuget/v/Smdn.Reflection.ReverseGenerating.ListApi.svg)](https://www.nuget.org/packages/Smdn.Reflection.ReverseGenerating.ListApi/)

This package provides `list-api` command line tool.

This tool does not provide any code-formatting options.

# Smdn.Reflection.ReverseGenerating
[![NuGet Smdn.Reflection.ReverseGenerating](https://img.shields.io/nuget/v/Smdn.Reflection.ReverseGenerating.svg)](https://www.nuget.org/packages/Smdn.Reflection.ReverseGenerating/)
