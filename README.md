[![GitHub license](https://img.shields.io/github/license/smdn/Smdn.Reflection.ReverseGenerating)](https://github.com/smdn/Smdn.Reflection.ReverseGenerating/blob/main/LICENSE.txt)
[![tests/main](https://img.shields.io/github/workflow/status/smdn/Smdn.Reflection.ReverseGenerating/Run%20tests/main?label=tests%2Fmain)](https://github.com/smdn/Smdn.Reflection.ReverseGenerating/actions/workflows/test.yml)
[![CodeQL](https://github.com/smdn/Smdn.Reflection.ReverseGenerating/actions/workflows/codeql-analysis.yml/badge.svg?branch=main)](https://github.com/smdn/Smdn.Reflection.ReverseGenerating/actions/workflows/codeql-analysis.yml)

# Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks
[![NuGet Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks](https://img.shields.io/nuget/v/Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks.svg)](https://www.nuget.org/packages/Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks/)

This package provides `GenerateApiList` MSBuild task.

## Usage
```xml
  <ItemGroup>
    <!-- Add package reference of Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks. -->
    <PackageReference
      Include="Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks"
      Version="1.1.1"
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

## Installation
Use `dotnet tool` to install or update `list-api` command.

```sh
# install
dotnet tool install -g Smdn.Reflection.ReverseGenerating.ListApi

# update
dotnet tool update -g Smdn.Reflection.ReverseGenerating.ListApi
```

## Usage
```sh
# The file 'Program-net6.0.apilist.cs' will be generated.
list-api Program/bin/Release/net6.0/Program.dll
```

Type `list-api --help` to show usage about command line arguments and options.

## Limitations
This tool does not provide any options for code styles including spaces, indents, and new lines. Use `dotnet format` after generating to format such code styles.

# Smdn.Reflection.ReverseGenerating
[![NuGet Smdn.Reflection.ReverseGenerating](https://img.shields.io/nuget/v/Smdn.Reflection.ReverseGenerating.svg)](https://www.nuget.org/packages/Smdn.Reflection.ReverseGenerating/)

This package provides APIs common to reverse generating. See [examples](examples/Smdn.Reflection.ReverseGenerating/) or [API list of Smdn.Reflection.ReverseGenerating](doc/api-list/Smdn.Reflection.ReverseGenerating/).

# Example of output
The files in [API list directory](/doc/api-list/) is generated using the artifacts of this repository.

