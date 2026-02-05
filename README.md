[![GitHub license](https://img.shields.io/github/license/smdn/Smdn.Reflection.ReverseGenerating)](https://github.com/smdn/Smdn.Reflection.ReverseGenerating/blob/main/LICENSE.txt)
[![tests/main](https://img.shields.io/github/actions/workflow/status/smdn/Smdn.Reflection.ReverseGenerating/test.yml?branch=main&label=tests%2Fmain)](https://github.com/smdn/Smdn.Reflection.ReverseGenerating/actions/workflows/test.yml)
[![CodeQL](https://github.com/smdn/Smdn.Reflection.ReverseGenerating/actions/workflows/codeql-analysis.yml/badge.svg?branch=main)](https://github.com/smdn/Smdn.Reflection.ReverseGenerating/actions/workflows/codeql-analysis.yml)

This repository provides the libraries and tools to reverse-generate C# code that describes the type and member information, from the .NET assemblies.

By generating an API list from the library, the types and members provided by the library can be presented in C# code format. This allows you to present an outline of the library API to users in addition to explaining the API through text or sample code.

![Example of output: System.Exception, .NET 8.0](https://github.com/smdn/Smdn.Reflection.ReverseGenerating/blob/main/doc/image/output-example-System.Exception-net8.0.png)

# Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks
[![NuGet Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks](https://img.shields.io/nuget/v/Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks.svg)](https://www.nuget.org/packages/Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks/)

This package provides `GenerateApiList` MSBuild task.

## Usage
See [examples](examples/Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks/) for details and more usage examples.

```xml
  <ItemGroup>
    <!-- Add package reference of Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks. -->
    <PackageReference
      Include="Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks"
      Version="*"
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

# install and update (specifies the target framework explicitly)
dotnet tool install --framework net10.0 -g Smdn.Reflection.ReverseGenerating.ListApi
dotnet tool update --framework net8.0 -g Smdn.Reflection.ReverseGenerating.ListApi
```

## Usage
```sh
# Generates the API list.
# The file 'Library-net10.0.apilist.cs' will be generated.
list-api Library/bin/Release/net10.0/Library.dll

# Generates the API list with loading the assembly in the reflection-only context.
list-api --load-reflection-only Library/bin/Release/net10.0/Library.dll
```

Type `list-api --help` to show usage about command line arguments and options.



If you are loading an assembly that targets a newer version than the runtime version that the `list-api` command targets, try running it with the [DOTNET_ROLL_FORWARD](https://learn.microsoft.com/dotnet/core/versions/selection) environment variable.

```sh
# Installs list-api targeting .NET 8.0
dotnet tool install --framework net8.0 -g Smdn.Reflection.ReverseGenerating.ListApi

# Uses runtime roll forwarding to load the assembly targeting .NET 10.0
DOTNET_ROLL_FORWARD=LatestMajor list-api Library/bin/Release/net10.0/Library.dll
```

## Limitations
This tool does not provide any options for code styles including spaces, indents, and new lines. Use `dotnet format` after generating to format such code styles.

# Smdn.Reflection.ReverseGenerating
[![NuGet Smdn.Reflection.ReverseGenerating](https://img.shields.io/nuget/v/Smdn.Reflection.ReverseGenerating.svg)](https://www.nuget.org/packages/Smdn.Reflection.ReverseGenerating/)

This package provides APIs common to reverse generating. See [examples](examples/Smdn.Reflection.ReverseGenerating/) or [API list of Smdn.Reflection.ReverseGenerating](doc/api-list/Smdn.Reflection.ReverseGenerating/).

# Example of output
The files in [API list directory](/doc/api-list/) is generated using the artifacts of this repository.

# For contributers
Contributions are appreciated!

If there's a feature you would like to add or a bug you would like to fix, please read [Contribution guidelines](./CONTRIBUTING.md) and create an Issue or Pull Request.

IssueやPull Requestを送る際は、[Contribution guidelines](./CONTRIBUTING.md)をご覧頂ください。　可能なら英語が望ましいですが、日本語で構いません。
