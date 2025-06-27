// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
using System.Reflection;
#endif
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks;

public class GenerateApiList : Task {
#pragma warning disable CA1819
  [Required]
  public ITaskItem[]? Assemblies { get; set; }
#pragma warning restore CA1819

  public bool GenerateLanguagePrimitiveType { get; set; } = true;
  public bool GenerateFullTypeName { get; set; }
  public bool GenerateTypeNameWithDeclaringTypeName { get; set; }
  public string? GenerateMethodBody { get; set; } = nameof(MethodBodyOption.EmptyImplementation);
  public bool GenerateAttributeWithNamedArguments { get; set; }
  public bool GenerateStaticMembersFirst { get; set; }
  public bool GenerateNullableAnnotations { get; set; } = true;
  public bool GenerateValueWithDefaultLiteral { get; set; } = true;
  public bool GenerateAssemblyInfo { get; set; } = true;
  public bool GenerateEmbeddedResources { get; set; } = true;
  public bool GenerateReferencedAssemblies { get; set; } = true;
  public bool GenerateRecordTypes { get; set; } = true;
  public bool ThrowIfForwardedTypesCouldNotLoaded { get; set; } = true;

#pragma warning disable CA1819
  [Output]
  public ITaskItem[]? GeneratedFiles { get; private set; }
#pragma warning restore CA1819

  private const string DefaultOutputFileExtension = ".apilist.cs";
  private const bool LoadIntoReflectionOnlyContextDefault =
#if NETFRAMEWORK
    true;
#else
    false;
#endif

  public override bool Execute()
  {
    if (Assemblies is null || Assemblies.Length == 0)
      return true;

    var generatedFileItems = new List<ITaskItem>(capacity: Assemblies.Length);

    foreach (var inputAssembly in Assemblies) {
      if (string.IsNullOrEmpty(inputAssembly.ItemSpec))
        continue;

      var inputAssemblyFilePath = inputAssembly.ItemSpec;
      var outputApiListFilePath = inputAssembly.GetMetadata("OutputFilePath");

      if (string.IsNullOrEmpty(outputApiListFilePath)) {
        // set default output file path
        outputApiListFilePath = Path.Join(
          Path.GetDirectoryName(inputAssemblyFilePath),
          Path.GetFileNameWithoutExtension(inputAssemblyFilePath) + DefaultOutputFileExtension
        );
      }

      if (!bool.TryParse(inputAssembly.GetMetadata("LoadIntoReflectionOnlyContext"), out var loadIntoReflectionOnlyContext))
        loadIntoReflectionOnlyContext = LoadIntoReflectionOnlyContextDefault;

      var options = BuildApiListWriterOptions();

      try {
        WriteApiListFile(
          inputAssemblyFilePath,
          outputApiListFilePath,
          options,
          loadIntoReflectionOnlyContext,
          GenerateNullableAnnotations,
          logger: new LoggerAdapter(Log, inputAssemblyFilePath)
        );
      }
#pragma warning disable CA1031
      catch (Exception ex) {
        Log.LogErrorFromException(
          exception: ex,
          showStackTrace: true,
          showDetail: true,
          file: inputAssemblyFilePath
        );

        continue;
      }
#pragma warning restore CA1031

      Log.LogMessage(MessageImportance.High, $"{inputAssemblyFilePath} -> {outputApiListFilePath}");

      var generatedFileItem = new TaskItem(outputApiListFilePath);

      generatedFileItem.SetMetadata("SourceAssembly", inputAssemblyFilePath);

      generatedFileItems.Add(generatedFileItem);
    }

    GeneratedFiles = generatedFileItems.ToArray();

    return !Log.HasLoggedErrors;
  }

  private ApiListWriterOptions BuildApiListWriterOptions()
  {
#pragma warning disable IDE0017
    var options = new ApiListWriterOptions();
#pragma warning restore IDE0017

#pragma warning disable IDE0017, IDE0055
    options.TranslateLanguagePrimitiveTypeDeclaration = GenerateLanguagePrimitiveType;

    options.TypeDeclaration.WithNamespace         = false;
    options.MemberDeclaration.WithNamespace       = false;
    options.AttributeDeclaration.WithNamespace    = GenerateFullTypeName;
    options.ValueDeclaration.WithNamespace        = GenerateFullTypeName;
    options.ParameterDeclaration.WithNamespace    = GenerateFullTypeName;

    options.TypeDeclaration.WithDeclaringTypeName       = false;
    options.MemberDeclaration.WithEnumTypeName          = false;
    options.MemberDeclaration.WithDeclaringTypeName     = false;
    options.AttributeDeclaration.WithDeclaringTypeName  = GenerateTypeNameWithDeclaringTypeName;
    options.ValueDeclaration.WithDeclaringTypeName      = GenerateTypeNameWithDeclaringTypeName;
    options.ParameterDeclaration.WithDeclaringTypeName  = GenerateTypeNameWithDeclaringTypeName;

    if (Enum.TryParse<MethodBodyOption>(GenerateMethodBody, out var methodBody)) {
      options.MemberDeclaration.MethodBody = methodBody;
      options.MemberDeclaration.AccessorBody = methodBody;
    }

    options.Writer.WriteNullableAnnotationDirective = GenerateNullableAnnotations;
    options.Writer.OrderStaticMembersFirst          = GenerateStaticMembersFirst;
    options.Writer.WriteHeader                      = true;
    options.Writer.WriteAssemblyInfo                = GenerateAssemblyInfo;
    options.Writer.WriteEmbeddedResources           = GenerateEmbeddedResources;
    options.Writer.WriteReferencedAssemblies        = GenerateReferencedAssemblies;
    options.Writer.ThrowIfForwardedTypesCouldNotLoaded = ThrowIfForwardedTypesCouldNotLoaded;

    options.AttributeDeclaration.TypeFilter         = AttributeFilter.Default;
    options.AttributeDeclaration.WithNamedArguments = GenerateAttributeWithNamedArguments;

    options.ValueDeclaration.UseDefaultLiteral = GenerateValueWithDefaultLiteral;

    options.TypeDeclaration.EnableRecordTypes = GenerateRecordTypes;
    options.TypeDeclaration.OmitRecordImplicitInterface = GenerateRecordTypes;
    options.Writer.OmitCompilerGeneratedRecordEqualityMethods = GenerateRecordTypes;
#pragma warning restore IDE0017, IDE0055

    return options;
  }

  private static void WriteApiListFile(
    [NotNull] string inputAssemblyFilePath,
    [NotNull] string outputApiListFilePath,
    [NotNull] ApiListWriterOptions options,
    bool loadIntoReflectionOnlyContext,
    bool generateNullableAnnotations,
    Microsoft.Extensions.Logging.ILogger? logger
  )
  {
    AssemblyLoader.UsingAssembly(
      new FileInfo(inputAssemblyFilePath ?? throw new ArgumentNullException(nameof(inputAssemblyFilePath))),
      arg: (
        outputApiListFilePath: outputApiListFilePath ?? throw new ArgumentNullException(nameof(outputApiListFilePath)),
        options: options ?? throw new ArgumentNullException(nameof(options)),
        generateNullableAnnotations,
        logger
      ),
      logger: logger,
      context: out var context,
      loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
      actionWithLoadedAssembly: static (assembly, arg) => {
        // ensure the output directory existing
        Directory.CreateDirectory(Path.GetDirectoryName(arg.outputApiListFilePath)!);

        using var outputWriter = new StreamWriter(
          path: arg.outputApiListFilePath,
          append: false,
          encoding: new UTF8Encoding(false) // TODO: make encoding configurable
        );

        try {
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
          var nullabilityInfoContext = arg.generateNullableAnnotations
            ? new NullabilityInfoContext()
            : null;

          // assign NullabilityInfoContext to each assembly
          arg.options.TypeDeclaration.NullabilityInfoContext = nullabilityInfoContext;
          arg.options.MemberDeclaration.NullabilityInfoContext = nullabilityInfoContext;
#endif
          var writer = new ApiListWriter(
            outputWriter,
            assembly,
            arg.options,
            arg.logger
          );

          writer.WriteHeader();
          writer.WriteExportedTypes();
          writer.WriteFooter();

          return arg.outputApiListFilePath;
        }
        finally {
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
          // release the references held by the NullabilityInfoContext so that the assembly can be unloaded
          arg.options.TypeDeclaration.NullabilityInfoContext = null;
          arg.options.MemberDeclaration.NullabilityInfoContext = null;
#endif
        }
      }
    );

    // wait for the context to be collected
    if (context is null)
      return;

    while (context.IsAlive) {
      GC.Collect();
      GC.WaitForPendingFinalizers();
    }
  }
}
