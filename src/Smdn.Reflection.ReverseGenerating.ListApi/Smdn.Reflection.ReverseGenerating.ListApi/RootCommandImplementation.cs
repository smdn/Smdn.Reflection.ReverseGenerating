// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#if FEATURE_BUILD_PROJ
using Smdn.Reflection.ReverseGenerating.ListApi.Build;
#endif

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class RootCommandImplementation {
  public static readonly string DefaultBuildConfiguration = "Release";

  private static readonly Argument<FileSystemInfo> argumentInput = new(
    name: "input",
#if FEATURE_BUILD_PROJ
    description: "Path to project/solution/assembly file to generate the API list. The command will search for an project file from the current directory if not specified, or search from the directory if a directory is specified.",
#else
    description: "Path to an assembly file to generate the API list.",
#endif
    getDefaultValue: static () => new DirectoryInfo(Environment.CurrentDirectory)
  ) {
    // Arity = ArgumentArity.OneOrMore
    Arity = ArgumentArity.ExactlyOne,
  };

  private static readonly Option<string?> optionConfiguration = new(
    aliases: new[] { "-c", "--configuration" },
    description: "The 'build configuration' option passed to `Build` target when the project will be built.",
    getDefaultValue: static () => DefaultBuildConfiguration
  );
  private static readonly Option<string?> optionTargetFramework = new(
    aliases: new[] { "-f", "--framework" },
    description: "The 'target framework' option passed to `Build` target when the project will be built.",
    getDefaultValue: static () => null
  );
  private static readonly Option<string?> optionRuntimeIdentifier = new(
    aliases: new[] { "-r", "--runtime" },
    description: "The 'target runtime' option passed to `Build` target when the project will be built.",
    getDefaultValue: static () => null
  );
  private static readonly Option<string?> optionOS = new(
    aliases: new[] { "--os" },
    description: "The 'target operating system' option passed to `Build` target when the project will be built.",
    getDefaultValue: static () => null
  );
  private static readonly Option<DirectoryInfo> optionOutputDirectory = new(
    aliases: new[] { "-o", "--output-directory" },
    description: "Path to output directory.",
    getDefaultValue: static () => new DirectoryInfo(Environment.CurrentDirectory)
  );
  private static readonly Option<bool> optionLoadAssemblyIntoReflectionOnlyContext = new(
    aliases: new[] { "--load-reflection-only" },
    description: "Loads and processes input assemblies in the reflection-only context.",
    getDefaultValue: static () =>
#if NETFRAMEWORK
      true
#else
      false
#endif
  );
  private static readonly Option<bool> optionGenerateFullTypeName = new(
    aliases: new[] { "--generate-fulltypename" },
    description: "Generates declarations with full type name.",
    getDefaultValue: static () => false
  );
  private static readonly Option<MethodBodyOption> optionGenerateMethodBody = new(
    aliases: new[] { "--generate-methodbody" },
    description: "Generates method body with specified type of implementation.",
    getDefaultValue: static () => MethodBodyOption.EmptyImplementation
  );
  private static readonly Option<bool> optionGenerateStaticMembersFirst = new(
    aliases: new[] { "--generate-staticmembersfirst" },
    description: "Generates member declarations in the order of the static members first.",
    getDefaultValue: static () => false
  );
  private static readonly Option<bool> optionGenerateNullableAnnotations = new(
    aliases: new[] { "--generate-nullableannotations" },
    description: "Generates declarations with nullable annotations.",
    getDefaultValue: static () => true
  );

  private readonly IServiceProvider? serviceProvider;
  private readonly Microsoft.Extensions.Logging.ILogger? logger;

  public RootCommandImplementation(IServiceProvider? serviceProvider = null)
  {
    this.serviceProvider = serviceProvider;
    this.logger = serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger(Program.LoggerCategoryName);
  }

  internal Command CreateCommand()
  {
    var rootCommand = new RootCommand {
      argumentInput,
#if FEATURE_BUILD_PROJ
      optionConfiguration,
      optionTargetFramework,
      // optionOS
      optionRuntimeIdentifier,
#endif
      optionOutputDirectory,
      VerbosityOption.Option,
      optionLoadAssemblyIntoReflectionOnlyContext,
      optionGenerateFullTypeName,
      optionGenerateMethodBody,
      optionGenerateStaticMembersFirst,
      optionGenerateNullableAnnotations,
    };

    rootCommand.Handler = CommandHandler.Create<ParseResult, IConsole>(CommandMain);

    return rootCommand;
  }

  private ParseResult ParseCommandLineArgs(string[] args)
    => new CommandLineBuilder(CreateCommand()).UseDefaults().Build().Parse(args);

  // <remarks>This method is for testing purposes.</remarks>
  public ApiListWriterOptions GetApiListWriterOptions(string[] args)
    => GetApiListWriterOptions(ParseCommandLineArgs(args));

  private static ApiListWriterOptions GetApiListWriterOptions(ParseResult parseResult)
  {
    var options = new ApiListWriterOptions();

#pragma warning disable IDE0055
    options.TypeDeclaration.WithNamespace       = parseResult.ValueForOption(optionGenerateFullTypeName);
    options.MemberDeclaration.WithNamespace     = parseResult.ValueForOption(optionGenerateFullTypeName);
    options.AttributeDeclaration.WithNamespace  = parseResult.ValueForOption(optionGenerateFullTypeName);

    var methodBody = parseResult.ValueForOption(optionGenerateMethodBody);

    options.MemberDeclaration.MethodBody        = methodBody;
    options.MemberDeclaration.AccessorBody      = methodBody;

    options.Writer.OrderStaticMembersFirst          = parseResult.ValueForOption(optionGenerateStaticMembersFirst);
    options.Writer.WriteNullableAnnotationDirective = parseResult.ValueForOption(optionGenerateNullableAnnotations);

    options.AttributeDeclaration.TypeFilter     = AttributeFilter.Default;

    options.ValueDeclaration.UseDefaultLiteral  = true;
#pragma warning restore IDE0055

    return options;
  }

  private static DirectoryInfo GetOutputDirectory(ParseResult parseResult)
    => parseResult.ValueForOption(optionOutputDirectory) ?? new(Environment.CurrentDirectory);

  private void CommandMain(ParseResult parseResult, IConsole console)
  {
#pragma warning disable CA2254
    logger?.LogDebug(parseResult.ToString());
#pragma warning restore CA2254

    // ref: https://docs.microsoft.com/en-us/dotnet/core/dependency-loading/default-probing
    logger?.LogDebug("APP_PATHS={AppContext_APP_PATHS}", AppContext.GetData("APP_PATHS"));
    logger?.LogDebug("APP_CONTEXT_DEPS_FILES={AppContext_APP_CONTEXT_DEPS_FILES}", AppContext.GetData("APP_CONTEXT_DEPS_FILES"));
    logger?.LogDebug("TRUSTED_PLATFORM_ASSEMBLIES={AppContext_TRUSTED_PLATFORM_ASSEMBLIES}", AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"));

    var options = GetApiListWriterOptions(parseResult);
    var outputDirectory = GetOutputDirectory(parseResult);
    var loadAssemblyIntoReflectionOnlyContext = parseResult.ValueForOption(optionLoadAssemblyIntoReflectionOnlyContext);
    var enableNullabilityAnnotations = parseResult.ValueForOption(optionGenerateNullableAnnotations);

    foreach (var inputAssemblyFile in GetInputAssemblyFiles(parseResult)) {
      AssemblyLoader.UsingAssembly(
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
        actionWithLoadedAssembly: static (assm, arg) => {
          try {
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
            var nullabilityInfoContext = arg.enableNullabilityAnnotations
              ? new NullabilityInfoContext()
              : null;

            // assign NullabilityInfoContext to each assembly
            arg.options.TypeDeclaration.NullabilityInfoContext = nullabilityInfoContext;
            arg.options.MemberDeclaration.NullabilityInfoContext = nullabilityInfoContext;
#endif
            var outputFilePath = GetOutputFilePathOf(assm, arg.outputDirectory);

            // ensure the output directory existing
            arg.outputDirectory.Create();

            using var outputWriter = new StreamWriter(outputFilePath, append: false, new UTF8Encoding(false));

            var writer = new ApiListWriter(outputWriter, assm, arg.options);

            writer.WriteAssemblyInfoHeader();
            writer.WriteExportedTypes();

            arg.logger?.LogDebug("generated API list {OutputFilePath}", outputFilePath);
            arg.logger?.LogInformation("{AssemblyFilePath} -> {OutputFilePath}", assm.Location, outputFilePath);

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

      // wait for the context to be collected
      if (context is null)
        continue;

      while (context.IsAlive) {
        GC.Collect();
        GC.WaitForPendingFinalizers();
      }
    }
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
        actionWithLoadedAssembly: static (assm, outdir) => GetOutputFilePathOf(assm, outdir)
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

  private static string GetOutputFilePathOf(Assembly assm, DirectoryInfo outputDirectory)
  {
#pragma warning restore SA1114
#if SYSTEM_IO_PATH_JOIN
    return Path.Join(
#else
    return Path.Combine(
#endif
#pragma warning disable SA1114
      outputDirectory.FullName, $"{GetOutputFileName(assm)}.apilist.cs"
    );

    static string GetOutputFileName(Assembly a)
    {
      var prefix = a.GetName().Name!;
      var targetFramework = a.GetAssemblyMetadataAttributeValue<TargetFrameworkAttribute, string>();

      if (targetFramework is null)
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
    var input = parseResult.ValueForArgument(argumentInput);
    FileInfo inputFile;

    if (input is null)
      throw new CommandOperationNotSupportedException("input file or directory not specified");

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
      // TODO: dereference symlink
      throw new CommandOperationNotSupportedException($"unsupported input: {input}");
    }

    logger?.LogDebug("input file: '{InputFilePath}'", inputFile.FullName);

    IEnumerable<FileInfo> inputAssemblyFiles = Enumerable.Empty<FileInfo>();

    if (
      string.Equals(".dll", inputFile.Extension, StringComparison.OrdinalIgnoreCase) ||
      string.Equals(".exe", inputFile.Extension, StringComparison.OrdinalIgnoreCase)
    ) {
      inputAssemblyFiles = Enumerable.Repeat(inputFile, 1);
    }
    else if (string.Equals(".sln", inputFile.Extension, StringComparison.OrdinalIgnoreCase)) {
      // TODO
      throw new CommandOperationNotSupportedException("generating from the solution file is not supported currently");
    }
    else if (inputFile.Extension.EndsWith("proj", StringComparison.OrdinalIgnoreCase)) {
#if FEATURE_BUILD_PROJ
      MSBuildExePath.EnsureSetEnvVar(logger);

      inputAssemblyFiles = ProjectBuilder.Build(
        inputFile,
        options: new() {
          Configuration = parseResult.ValueForOption(optionConfiguration),
          TargetFramework = parseResult.ValueForOption(optionTargetFramework),
          // OS: parseResult.ValueForOption(optionOS),
          RuntimeIdentifier = parseResult.ValueForOption(optionRuntimeIdentifier),
          LoggerVerbosity = VerbosityOption.ParseLoggerVerbosity(parseResult),
        },
        logger: logger
      );
#else
      throw new CommandOperationNotSupportedException("generating from the project file is not supported currently");
#endif
    }
    else {
      logger?.LogWarning("unknown type of file: {InputFilePath}", inputFile);

      // try process as an assembly file
      inputAssemblyFiles = Enumerable.Repeat(inputFile, 1);
    }

    return inputAssemblyFiles;
  }
}
