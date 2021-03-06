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
  [Required]
  public ITaskItem[]? Assemblies { get; set; }

  public bool GenerateLanguagePrimitiveType { get; set; } = true;
  public bool GenerateFullTypeName { get; set; }
  public bool GenerateTypeNameWithDeclaringTypeName { get; set; }
  public string? GenerateMethodBody { get; set; } = nameof(MethodBodyOption.EmptyImplementation);
  public bool GenerateAttributeWithNamedArguments { get; set; }
  public bool GenerateStaticMembersFirst { get; set; }
  public bool GenerateNullableAnnotations { get; set; } = true;
  public bool GenerateValueWithDefaultLiteral { get; set; } = true;

  [Output]
  public ITaskItem[]? GeneratedFiles { get; private set; }

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
          logger: new LoggerAdapter(Log)
        );
      }
      catch (Exception ex) {
        Log.LogErrorFromException(
          exception: ex,
          showStackTrace: true,
          showDetail: true,
          file: inputAssemblyFilePath
        );

        continue;
      }

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

#pragma warning disable IDE0055
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

    options.AttributeDeclaration.TypeFilter         = AttributeFilter.Default;
    options.AttributeDeclaration.WithNamedArguments = GenerateAttributeWithNamedArguments;

    options.ValueDeclaration.UseDefaultLiteral = GenerateValueWithDefaultLiteral;
#pragma warning restore IDE0055

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
        generateNullableAnnotations
      ),
      logger: logger,
      context: out var context,
      loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
      actionWithLoadedAssembly: static (assm, arg) => {
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
          var writer = new ApiListWriter(outputWriter, assm, arg.options);

          writer.WriteAssemblyInfoHeader();
          writer.WriteExportedTypes();

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
