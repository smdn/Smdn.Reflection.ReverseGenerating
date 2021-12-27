// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks;

public class GenerateApiList : Task {
  [Required]
  public ITaskItem[]? Assemblies { get; set; }

  public bool GenerateFullTypeName { get; set; }
  public string? GenerateMethodBody { get; set; }
  public bool GenerateStaticMembersFirst { get; set; }

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
    var options = new ApiListWriterOptions();

#pragma warning disable IDE0055
    options.TypeDeclaration.WithNamespace       = GenerateFullTypeName;
    options.MemberDeclaration.WithNamespace     = GenerateFullTypeName;
    options.AttributeDeclaration.WithNamespace  = GenerateFullTypeName;

    if (Enum.TryParse<MethodBodyOption>(GenerateMethodBody, out var methodBody))
      options.MemberDeclaration.MethodBody = methodBody;

    options.Writer.OrderStaticMembersFirst      = GenerateStaticMembersFirst;

    options.AttributeDeclaration.TypeFilter     = AttributeFilter.Default;

    options.ValueDeclaration.UseDefaultLiteral  = true;
#pragma warning restore IDE0055

    return options;
  }

  private static void WriteApiListFile(
    [NotNull] string inputAssemblyFilePath,
    [NotNull] string outputApiListFilePath,
    [NotNull] ApiListWriterOptions options,
    bool loadIntoReflectionOnlyContext,
    Microsoft.Extensions.Logging.ILogger? logger
  )
  {
    AssemblyLoader.UsingAssembly(
      new FileInfo(inputAssemblyFilePath ?? throw new ArgumentNullException(nameof(inputAssemblyFilePath))),
      arg: (
        outputApiListFilePath: outputApiListFilePath ?? throw new ArgumentNullException(nameof(outputApiListFilePath)),
        options: options ?? throw new ArgumentNullException(nameof(options))
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

        var writer = new ApiListWriter(outputWriter, assm, arg.options);

        writer.WriteAssemblyInfoHeader();
        writer.WriteExportedTypes();

        return arg.outputApiListFilePath;
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
