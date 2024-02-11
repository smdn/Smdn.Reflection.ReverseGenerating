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
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#if FEATURE_BUILD_PROJ
using Smdn.Reflection.ReverseGenerating.ListApi.Build;
#endif

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public sealed class RootCommandImplementation : ICommandHandler {
  public static readonly string DefaultBuildConfiguration = "Release";

  private static readonly Argument<FileSystemInfo> ArgumentInput = new Argument<FileSystemInfo>(
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
  }
  .ExistingOnly();

  private static readonly Option<string?> OptionConfiguration = new(
    aliases: new[] { "-c", "--configuration" },
    description: "The 'build configuration' option passed to `Build` target when the project will be built.",
    getDefaultValue: static () => DefaultBuildConfiguration
  );
  private static readonly Option<string?> OptionTargetFramework = new(
    aliases: new[] { "-f", "--framework" },
    description: "The 'target framework' option passed to `Build` target when the project will be built.",
    getDefaultValue: static () => null
  );
  private static readonly Option<string?> OptionRuntimeIdentifier = new(
    aliases: new[] { "-r", "--runtime" },
    description: "The 'target runtime' option passed to `Build` target when the project will be built.",
    getDefaultValue: static () => null
  );
#if false
  private static readonly Option<string?> OptionOS = new(
    aliases: new[] { "--os" },
    description: "The 'target operating system' option passed to `Build` target when the project will be built.",
    getDefaultValue: static () => null
  );
#endif
  private static readonly Option<DirectoryInfo> OptionOutputDirectory = new(
    aliases: new[] { "-o", "--output-directory" },
    description: "Path to output directory.",
    getDefaultValue: static () => new DirectoryInfo(Environment.CurrentDirectory)
  );
  private static readonly Option<bool> OptionLoadAssemblyIntoReflectionOnlyContext = new(
    aliases: new[] { "--load-reflection-only" },
    description: "Loads and processes input assemblies in the reflection-only context.",
    getDefaultValue: static () =>
#if NETFRAMEWORK
      true
#else
      false
#endif
  );
  private static readonly Option<bool> OptionGenerateFullTypeName = new(
    aliases: new[] { "--generate-fulltypename" },
    description: "Generates declarations with full type name.",
    getDefaultValue: static () => false
  );
  private static readonly Option<MethodBodyOption> OptionGenerateMethodBody = new(
    aliases: new[] { "--generate-methodbody" },
    description: "Generates method body with specified type of implementation.",
    getDefaultValue: static () => MethodBodyOption.EmptyImplementation
  );
  private static readonly Option<bool> OptionGenerateStaticMembersFirst = new(
    aliases: new[] { "--generate-staticmembersfirst" },
    description: "Generates member declarations in the order of the static members first.",
    getDefaultValue: static () => false
  );
  private static readonly Option<bool> OptionGenerateNullableAnnotations = new(
    aliases: new[] { "--generate-nullableannotations" },
    description: "Generates declarations with nullable annotations.",
    getDefaultValue: static () => true
  );

  private readonly Microsoft.Extensions.Logging.ILogger? logger;

  public RootCommandImplementation(IServiceProvider? serviceProvider = null)
  {
    this.logger = serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger(Program.LoggerCategoryName);
  }

  internal Command CreateCommand()
  {
    var rootCommand = new RootCommand {
      ArgumentInput,
#if FEATURE_BUILD_PROJ
      OptionConfiguration,
      OptionTargetFramework,
      // OptionOS
      OptionRuntimeIdentifier,
#endif
      OptionOutputDirectory,
      VerbosityOption.Option,
      OptionLoadAssemblyIntoReflectionOnlyContext,
      OptionGenerateFullTypeName,
      OptionGenerateMethodBody,
      OptionGenerateStaticMembersFirst,
      OptionGenerateNullableAnnotations,
    };

    rootCommand.Handler = this;

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
    options.TypeDeclaration.WithNamespace       = parseResult.GetValueForOption(OptionGenerateFullTypeName);
    options.MemberDeclaration.WithNamespace     = parseResult.GetValueForOption(OptionGenerateFullTypeName);
    options.AttributeDeclaration.WithNamespace  = parseResult.GetValueForOption(OptionGenerateFullTypeName);

    var methodBody = parseResult.GetValueForOption(OptionGenerateMethodBody);

    options.MemberDeclaration.MethodBody        = methodBody;
    options.MemberDeclaration.AccessorBody      = methodBody;

    options.Writer.OrderStaticMembersFirst          = parseResult.GetValueForOption(OptionGenerateStaticMembersFirst);
    options.Writer.WriteNullableAnnotationDirective = parseResult.GetValueForOption(OptionGenerateNullableAnnotations);

    options.AttributeDeclaration.TypeFilter     = AttributeFilter.Default;

    options.ValueDeclaration.UseDefaultLiteral  = true;
#pragma warning restore IDE0055

    return options;
  }

  private static DirectoryInfo GetOutputDirectory(ParseResult parseResult)
    => parseResult.GetValueForOption(OptionOutputDirectory) ?? new(Environment.CurrentDirectory);

  Task<int> ICommandHandler.InvokeAsync(InvocationContext invocationContext)
#pragma warning disable CA1849
    => Task.FromResult((this as ICommandHandler).Invoke(invocationContext));
#pragma warning restore CA1849

  int ICommandHandler.Invoke(InvocationContext invocationContext)
  {
    var parseResult = invocationContext.ParseResult;

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
    var loadAssemblyIntoReflectionOnlyContext = parseResult.GetValueForOption(OptionLoadAssemblyIntoReflectionOnlyContext);
    var enableNullabilityAnnotations = parseResult.GetValueForOption(OptionGenerateNullableAnnotations);
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

            var writer = new ApiListWriter(
              outputWriter,
              assm,
              arg.options,
              arg.logger
            );

            try {
              writer.WriteHeader();
              writer.WriteExportedTypes();
              writer.WriteFooter();
            }
            catch (AssemblyFileNotFoundException ex) {
              arg.logger?.LogCritical(ex, "Could not load depending assembly '{FileName}', referenced from the assembly '{AssemblyName}'", ex.FileName, assm.GetName());
              arg.logger?.LogError("If you are trying to load an assembly with an SDK version that is not currently installed, install that version of the SDK, or specify the 'DOTNET_ROLL_FORWARD' environment variable and try again.");
              return null;
            }

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
    var input = parseResult.GetValueForArgument(ArgumentInput);
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
          Configuration = parseResult.GetValueForOption(OptionConfiguration),
          TargetFramework = parseResult.GetValueForOption(OptionTargetFramework),
          // OS: parseResult.GetValueForOption(OptionOS),
          RuntimeIdentifier = parseResult.GetValueForOption(OptionRuntimeIdentifier),
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
