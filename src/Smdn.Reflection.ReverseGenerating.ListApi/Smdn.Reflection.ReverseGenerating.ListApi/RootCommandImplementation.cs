// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

using System;
using System.Collections.Generic;
using System.CommandLine;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public sealed class RootCommandImplementation {
  public static readonly string DefaultBuildConfiguration = "Release";

  private static readonly Argument<FileSystemInfo> ArgumentInput = new Argument<FileSystemInfo>("input") {
    Description = "Path to an assembly file to generate the API list.",
    DefaultValueFactory = static _ => new DirectoryInfo(Environment.CurrentDirectory),
    // Arity = ArgumentArity.OneOrMore
    Arity = ArgumentArity.ExactlyOne,
  }
  .AcceptExistingOnly();
  private static readonly Option<DirectoryInfo> OptionOutputDirectory = new("--output-directory", "-o") {
    Description = "Path to output directory.",
    DefaultValueFactory = static _ => new DirectoryInfo(Environment.CurrentDirectory),
  };
  private static readonly Option<bool> OptionLoadAssemblyIntoReflectionOnlyContext = new("--load-reflection-only") {
    Description = "Loads and processes input assemblies in the reflection-only context.",
    DefaultValueFactory = static _ =>
#if NETFRAMEWORK
      true,
#else
      false,
#endif
  };
  // cSpell:disable
  private static readonly Option<bool> OptionGenerateFullTypeName = new("--generate-fulltypename") {
    Description = "Generates declarations with full type name.",
    DefaultValueFactory = static _ => false,
  };
  private static readonly Option<MethodBodyOption> OptionGenerateMethodBody = new("--generate-methodbody") {
    Description = "Generates method body with specified type of implementation.",
    DefaultValueFactory = static _ => MethodBodyOption.EmptyImplementation,
  };
  private static readonly Option<bool> OptionGenerateStaticMembersFirst = new("--generate-staticmembersfirst") {
    Description = "Generates member declarations in the order of the static members first.",
    DefaultValueFactory = static _ => false,
  };
  private static readonly Option<bool> OptionGenerateNullableAnnotations = new("--generate-nullableannotations") {
    Description = "Generates declarations with nullable annotations.",
    DefaultValueFactory = static _ => true,
  };
  private static readonly Option<bool> OptionGenerateRecords = new("--generate-records") {
    Description = "Generates record type declarations and hides compiler-generated members.",
    DefaultValueFactory = static _ => true,
  };
  private static readonly Option<bool> OptionTranslateLanguagePrimitiveType = new("--use-built-in-type-alias") {
    Description = "Use aliases for C# built-in types rather than .NET type names.",
    DefaultValueFactory = static _ => false,
  };
  // cSpell:enable

  private readonly Microsoft.Extensions.Logging.ILogger? logger;

  public RootCommandImplementation(IServiceProvider? serviceProvider = null)
  {
    this.logger = serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger(Program.LoggerCategoryName);
  }

  internal Command CreateCommand()
  {
    var rootCommand = new RootCommand {
      ArgumentInput,
      OptionOutputDirectory,
      VerbosityOption.Option,
      OptionLoadAssemblyIntoReflectionOnlyContext,
      OptionGenerateFullTypeName,
      OptionGenerateMethodBody,
      OptionGenerateStaticMembersFirst,
      OptionGenerateNullableAnnotations,
      OptionGenerateRecords,
      OptionTranslateLanguagePrimitiveType,
    };

    rootCommand.SetAction(RunAsync);

    return rootCommand;
  }

  private ParseResult ParseCommandLineArgs(string[] args)
    => CreateCommand().Parse(args);

  // <remarks>This method is for testing purposes.</remarks>
  public ApiListWriterOptions GetApiListWriterOptions(string[] args)
    => GetApiListWriterOptions(ParseCommandLineArgs(args));

  private static ApiListWriterOptions GetApiListWriterOptions(ParseResult parseResult)
  {
    var options = new ApiListWriterOptions();

#pragma warning disable IDE0055
    options.TranslateLanguagePrimitiveTypeDeclaration = parseResult.GetValue(OptionTranslateLanguagePrimitiveType);

    options.TypeDeclaration.WithNamespace       = parseResult.GetValue(OptionGenerateFullTypeName);
    options.MemberDeclaration.WithNamespace     = parseResult.GetValue(OptionGenerateFullTypeName);
    options.AttributeDeclaration.WithNamespace  = parseResult.GetValue(OptionGenerateFullTypeName);

    var methodBody = parseResult.GetValue(OptionGenerateMethodBody);

    options.MemberDeclaration.MethodBody        = methodBody;
    options.MemberDeclaration.AccessorBody      = methodBody;

    options.Writer.OrderStaticMembersFirst          = parseResult.GetValue(OptionGenerateStaticMembersFirst);
    options.Writer.WriteNullableAnnotationDirective = parseResult.GetValue(OptionGenerateNullableAnnotations);

    options.TypeDeclaration.EnableRecordTypes =
    options.TypeDeclaration.OmitRecordImplicitInterface =
    options.Writer.OmitCompilerGeneratedRecordEqualityMethods = parseResult.GetValue(OptionGenerateRecords);

    options.AttributeDeclaration.TypeFilter     = AttributeFilter.Default;

    options.ValueDeclaration.UseDefaultLiteral  = true;
#pragma warning restore IDE0055

    return options;
  }

  private static DirectoryInfo GetOutputDirectory(ParseResult parseResult)
    => parseResult.GetValue(OptionOutputDirectory) ?? new(Environment.CurrentDirectory);

  private Task<int> RunAsync(ParseResult parseResult, CancellationToken cancellationToken)
    => Task.FromResult(Run(parseResult, cancellationToken));

  private int Run(ParseResult parseResult, CancellationToken cancellationToken)
  {
#pragma warning disable CA2254
    logger?.LogDebug(parseResult.ToString());
#pragma warning restore CA2254

    // https://learn.microsoft.com/dotnet/core/versions/selection
    // https://learn.microsoft.com/dotnet/core/tools/dotnet-environment-variables#dotnet_roll_forward
    logger?.LogDebug("DOTNET_ROLL_FORWARD={EnvVar_DOTNET_ROLL_FORWARD}", Environment.GetEnvironmentVariable("DOTNET_ROLL_FORWARD"));

    // ref: https://docs.microsoft.com/en-us/dotnet/core/dependency-loading/default-probing
    logger?.LogDebug("APP_PATHS={AppContext_APP_PATHS}", AppContext.GetData("APP_PATHS"));
    logger?.LogDebug("APP_CONTEXT_DEPS_FILES={AppContext_APP_CONTEXT_DEPS_FILES}", AppContext.GetData("APP_CONTEXT_DEPS_FILES"));
    logger?.LogDebug("TRUSTED_PLATFORM_ASSEMBLIES={AppContext_TRUSTED_PLATFORM_ASSEMBLIES}", AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"));

    var options = GetApiListWriterOptions(parseResult);
    var outputDirectory = GetOutputDirectory(parseResult);
    var loadAssemblyIntoReflectionOnlyContext = parseResult.GetValue(OptionLoadAssemblyIntoReflectionOnlyContext);
    var enableNullabilityAnnotations = parseResult.GetValue(OptionGenerateNullableAnnotations);
    var hasError = false;

    foreach (var inputAssemblyFile in GetInputAssemblyFiles(parseResult)) {
      logger?.LogInformation("generating API list: '{InputAssemblyFile}'", inputAssemblyFile);

      var outputFilePath = AssemblyLoader.UsingAssembly(
        inputAssemblyFile,
        arg: (
          outputDirectory,
          options,
          enableNullabilityAnnotations,
          logger
        ),
        logger: logger,
        context: out var context,
        loadIntoReflectionOnlyContext: loadAssemblyIntoReflectionOnlyContext,
        actionWithLoadedAssembly: static (assembly, arg) => {
          try {
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
            var nullabilityInfoContext = arg.enableNullabilityAnnotations
              ? new NullabilityInfoContext()
              : null;

            // assign NullabilityInfoContext to each assembly
            arg.options.TypeDeclaration.NullabilityInfoContext = nullabilityInfoContext;
            arg.options.MemberDeclaration.NullabilityInfoContext = nullabilityInfoContext;
#endif
            var outputFilePath = GetOutputFilePathOf(assembly, arg.outputDirectory);

            // ensure the output directory existing
            arg.outputDirectory.Create();

            using var outputWriter = new StreamWriter(outputFilePath, append: false, new UTF8Encoding(false));

            var writer = new ApiListWriter(
              outputWriter,
              assembly,
              arg.options,
              arg.logger
            );

            try {
              writer.WriteHeader();
              writer.WriteExportedTypes();
              writer.WriteFooter();
            }
            catch (AssemblyFileNotFoundException ex) {
              arg.logger?.LogCritical(ex, "Could not load depending assembly '{FileName}', referenced from the assembly '{AssemblyName}'", ex.FileName, assembly.GetName());
              arg.logger?.LogError("If you are trying to load an assembly with an SDK version that is not currently installed, install that version of the SDK, or specify the 'DOTNET_ROLL_FORWARD' environment variable and try again.");
              return null;
            }

            arg.logger?.LogDebug("generated API list {OutputFilePath}", outputFilePath);
            arg.logger?.LogInformation("{AssemblyFilePath} -> {OutputFilePath}", assembly.Location, outputFilePath);

            return outputFilePath;
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

      hasError |= string.IsNullOrEmpty(outputFilePath);

      // wait for the context to be collected
      if (context is null)
        continue;

      while (context.IsAlive) {
        GC.Collect();
        GC.WaitForPendingFinalizers();
      }
    }

    return hasError ? 1 : 0;
  }

  // <remarks>This method is for testing purposes.</remarks>
  public IEnumerable<string> GetOutputFilePaths(string[] args)
  {
    var parseResult = ParseCommandLineArgs(args);
    var outputDirectory = GetOutputDirectory(parseResult);

    foreach (var inputAssemblyFile in GetInputAssemblyFiles(parseResult)) {
      yield return AssemblyLoader.UsingAssembly(
        inputAssemblyFile,
        loadIntoReflectionOnlyContext: true,
        arg: outputDirectory,
        logger: logger,
        context: out var context,
        actionWithLoadedAssembly: static (assembly, outdir) => GetOutputFilePathOf(assembly, outdir)
      )!;

      // wait for the context to be collected
      if (context is null)
        continue;

      while (context.IsAlive) {
        GC.Collect();
        GC.WaitForPendingFinalizers();
      }
    }
  }

  private static string GetOutputFilePathOf(Assembly assembly, DirectoryInfo outputDirectory)
  {
#pragma warning restore SA1114
#if SYSTEM_IO_PATH_JOIN
    return Path.Join(
#else
    return Path.Combine(
#endif
#pragma warning disable SA1114
      outputDirectory.FullName, $"{GetOutputFileName(assembly)}.apilist.cs"
    );

    static string GetOutputFileName(Assembly a)
    {
      var prefix = a.GetName().Name!;
      string? targetFramework;

      try {
        targetFramework = a.GetAssemblyMetadataAttributeValue<TargetFrameworkAttribute, string>();
      }
      catch (AssemblyFileNotFoundException) {
        // If the target framework cannot be obtained due to failure to load reference assembly, treat as 'unknown'(null).
        targetFramework = null;
      }

      if (string.IsNullOrEmpty(targetFramework))
        return prefix;

      // TODO: osSpecifier
      if (
        TryParseFrameworkName(targetFramework, out var targetFrameworkName) &&
        FrameworkMonikers.TryGetMoniker(targetFrameworkName, osSpecifier: null, out var targetFrameworkMoniker)
      ) {
        return $"{prefix}-{targetFrameworkMoniker}";
      }

      return $"{prefix}-{targetFramework}";
    }

    static bool TryParseFrameworkName(
      string name,
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
      [NotNullWhen(true)]
#endif
      out FrameworkName? frameworkName
    )
    {
      frameworkName = default;

      if (name is null)
        return false;

      try {
        frameworkName = new(name);
        return true;
      }
      catch (ArgumentException) {
        return false;
      }
    }
  }

  // <remarks>This method is for testing purposes.</remarks>
  public IEnumerable<FileInfo> GetInputAssemblyFiles(string[] args)
    => GetInputAssemblyFiles(ParseCommandLineArgs(args));

  private IEnumerable<FileInfo> GetInputAssemblyFiles(ParseResult parseResult)
  {
    var input = parseResult.GetRequiredValue(ArgumentInput);
    FileInfo inputFile;

    if (input is null)
      throw new CommandOperationNotSupportedException("input file or directory not specified");

#pragma warning disable IDE0045
    if (input is DirectoryInfo inputDirectory) {
      try {
        inputFile = ProjectFinder.FindSingleProjectOrSolution(inputDirectory);
      }
      catch (InvalidOperationException ex) {
        throw new InvalidCommandOperationException(ex.Message);
      }
      catch (FileNotFoundException ex) {
        throw new InvalidCommandOperationException(ex.Message);
      }
    }
    else if (input is FileInfo f) {
      inputFile = f;
    }
    else {
      throw new CommandOperationNotSupportedException($"unsupported input: {input}");
    }
#pragma warning restore IDE0045

    logger?.LogDebug("input file: '{InputFilePath}'", inputFile.FullName);

    IEnumerable<FileInfo> inputAssemblyFiles;

    if (
      string.Equals(".dll", inputFile.Extension, StringComparison.OrdinalIgnoreCase) ||
      string.Equals(".exe", inputFile.Extension, StringComparison.OrdinalIgnoreCase)
    ) {
      inputAssemblyFiles = Enumerable.Repeat(inputFile, 1);
    }
    else {
      logger?.LogWarning("unknown type of file: {InputFilePath}", inputFile);

      // try process as an assembly file
      inputAssemblyFiles = Enumerable.Repeat(inputFile, 1);
    }

    return inputAssemblyFiles;
  }
}
