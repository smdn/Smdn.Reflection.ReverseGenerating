using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Smdn.Reflection.ReverseGenerating;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class RootCommandImplementation {
  public static readonly string DefaultBuildConfiguration = "Release";

  private static readonly Argument<FileSystemInfo> argumentInput = new(
    name: "input",
    description: "Path to project/solution/assembly file to generate. The command will search for an project file from the current directory if not specified, or search from the directory if a directory is specified.",
    getDefaultValue: () => new DirectoryInfo(Environment.CurrentDirectory)
  );

  private static readonly Option<string> optionConfiguration = new(
    aliases: new[] { "-c", "--configuration" },
    description: "The 'build configuration' option passed to `Build` target when the project will be built.",
    getDefaultValue: () => DefaultBuildConfiguration
  );
  private static readonly Option<string> optionTargetFramework = new(
    aliases: new[] { "-f", "--framework" },
    description: "The 'target framework' option passed to `Build` target when the project will be built.",
    getDefaultValue: () => null
  );
  private static readonly Option<string> optionRuntimeIdentifier = new(
    aliases: new[] { "-r", "--runtime" },
    description: "The 'target runtime' option passed to `Build` target when the project will be built.",
    getDefaultValue: () => null
  );
  private static readonly Option<string> optionOS = new(
    aliases: new[] { "--os" },
    description: "The 'target operating system' option passed to `Build` target when the project will be built.",
    getDefaultValue: () => null
  );
  private static readonly Option<DirectoryInfo> optionOutputDirectory = new(
    aliases: new[] { "-o", "--output-directory" },
    description: "Path to output directory.",
    getDefaultValue: () => new DirectoryInfo(Environment.CurrentDirectory)
  );
  private static readonly Option<bool> optionGenerateFullTypeName = new(
    aliases: new[] { "--generate-fulltypename" },
    description: "Generates declarations with full type name.",
    getDefaultValue: () => false
  );
  private static readonly Option<MethodBodyOption> optionGenerateMethodBody = new(
    aliases: new[] { "--generate-methodbody" },
    description: "Generates method body with specified type of implementation.",
    getDefaultValue: () => MethodBodyOption.EmptyImplementation
  );
  private static readonly Option<bool> optionGenerateStaticMembersFirst = new(
    aliases: new[] { "--generate-staticmembersfirst" },
    description: "Generates member declarations in the order of the static members first.",
    getDefaultValue: () => false
  );

  private readonly IServiceProvider serviceProvider;
  private readonly Microsoft.Extensions.Logging.ILogger logger;

  public RootCommandImplementation(IServiceProvider serviceProvider = null)
  {
    this.serviceProvider = serviceProvider;
    this.logger = serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger<RootCommandImplementation>();
  }

  internal Command CreateCommand()
  {
    var rootCommand = new RootCommand();

    rootCommand.Add(argumentInput);

    rootCommand.Add(optionConfiguration);
    rootCommand.Add(optionTargetFramework);
    //rootCommand.Add(optionOS);
    rootCommand.Add(optionRuntimeIdentifier);
    rootCommand.Add(optionOutputDirectory);
    rootCommand.Add(VerbosityOption.Option);
    rootCommand.Add(optionGenerateFullTypeName);
    rootCommand.Add(optionGenerateMethodBody);
    rootCommand.Add(optionGenerateStaticMembersFirst);

    rootCommand.Handler = CommandHandler.Create<ParseResult, IConsole>(CommandMain);

    return rootCommand;
  }

  private ParseResult ParseCommandLineArgs(string[] args)
    => new CommandLineBuilder(CreateCommand()).UseDefaults().Build().Parse(args);

  // <remarks>This method is for testing purposes.</remarks>
  public ApiListWriterOptions GetApiListWriterOptions(string[] args)
    => GetApiListWriterOptions(ParseCommandLineArgs(args));

  private static ApiListWriterOptions GetApiListWriterOptions(ParseResult parseResult)
    => new() {
      TypeDeclarationWithNamespace    = parseResult.ValueForOption(optionGenerateFullTypeName),
      MemberDeclarationWithNamespace  = parseResult.ValueForOption(optionGenerateFullTypeName),

      MemberDeclarationMethodBody     = parseResult.ValueForOption(optionGenerateMethodBody),

      WriterOrderStaticMembersFirst   = parseResult.ValueForOption(optionGenerateStaticMembersFirst),
    };

  private static DirectoryInfo GetOutputDirectory(ParseResult parseResult)
    => parseResult.ValueForOption(optionOutputDirectory) ?? new(Environment.CurrentDirectory);

  private void CommandMain(ParseResult parseResult, IConsole console)
  {
    var options = GetApiListWriterOptions(parseResult);
    var outputDirectory = GetOutputDirectory(parseResult);

    foreach (var inputAssemblyFile in GetInputAssemblyFiles(parseResult)) {
      AssemblyLoader.UsingAssembly(
        inputAssemblyFile,
        arg: (outputDirectory, options, logger),
        logger: logger,
        context: out var context,
        actionWithLoadedAssembly: static (assm, arg) => {
          var outputFilePath = GetOutputFilePathOf(assm, arg.outputDirectory);

          // ensure the output directory existing
          arg.outputDirectory.Create();

          using var outputWriter = new StreamWriter(outputFilePath, append: false, new UTF8Encoding(false));

          var writer = new ApiListWriter(outputWriter, assm, arg.options);

          writer.WriteAssemblyInfoHeader();
          writer.WriteExportedTypes();

          arg.logger.LogInformation($"generated API list '{outputFilePath}' from assembly '{assm.Location}'");

          return outputFilePath;
        }
      );

      // wait for the context to be collected
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
        arg: outputDirectory,
        logger: logger,
        context: out var context,
        actionWithLoadedAssembly: static (assm, outdir) => GetOutputFilePathOf(assm, outdir)
      );

      // wait for the context to be collected
      while (context.IsAlive) {
        GC.Collect();
        GC.WaitForPendingFinalizers();
      }
    }
  }

  private static string GetOutputFilePathOf(Assembly assm, DirectoryInfo outputDirectory)
  {
    return Path.Join(outputDirectory.FullName, $"{GetOutputFileName(assm)}.apilist.cs");

    static string GetOutputFileName(Assembly a)
    {
      var prefix = $"{a.GetName().Name}-{a.GetName().Version}";
      var targetFramework = GetTargetFramework(a);

      if (targetFramework is null)
        return prefix;

      // TODO: osSpecifier
      if (
        TryParseFrameworkName(targetFramework, out var targetFrameworkName) &&
        FrameworkMonikers.TryGetMoniker(targetFrameworkName, osSpecifier: null, out var targetFrameworkMoniker)
      )
      {
        return $"{prefix}-{targetFrameworkMoniker}";
      }

      return $"{prefix}-{targetFramework}";
    }

    static string GetTargetFramework(Assembly assm)
    {
      var frameworkName = assm.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;

      if (frameworkName is not null)
        return frameworkName;

      // for in case of executable assembly
      return assm
        .GetCustomAttributesData()
        ?.FirstOrDefault(d => d.AttributeType == typeof(TargetFrameworkAttribute))
        ?.ConstructorArguments
        ?.FirstOrDefault()
        .Value
        as string;
    }

    static bool TryParseFrameworkName(string name, out FrameworkName frameworkName)
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
    FileInfo inputFile = null;

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
      logger?.LogDebug($"input file: '{f.FullName}'");

      inputFile = f;
    }
    else {
      // TODO: dereference symlink
      throw new CommandOperationNotSupportedException($"unsupported input: {input}");
    }

    IEnumerable<FileInfo> inputAssemblyFiles = null;

    if (
      string.Equals(".dll", inputFile.Extension, StringComparison.OrdinalIgnoreCase) ||
      string.Equals(".exe", inputFile.Extension, StringComparison.OrdinalIgnoreCase)
    ) {
      inputAssemblyFiles = Enumerable.Repeat(inputFile, 1);
    }
    else if (string.Equals(".sln", inputFile.Extension, StringComparison.OrdinalIgnoreCase)) {
      // TODO
      throw new CommandOperationNotSupportedException("solution file is not supported currently");
    }
    else if (inputFile.Extension.EndsWith("proj", StringComparison.OrdinalIgnoreCase)) {
      MSBuildExePath.EnsureSetEnvVar(logger);

      inputAssemblyFiles = ProjectBuilder.Build(
        inputFile,
        options: new() {
          Configuration = parseResult.ValueForOption(optionConfiguration),
          TargetFramework = parseResult.ValueForOption(optionTargetFramework),
          //OS: parseResult.ValueForOption(optionOS),
          RuntimeIdentifier = parseResult.ValueForOption(optionRuntimeIdentifier),
          LoggerVerbosity = VerbosityOption.ParseLoggerVerbosity(parseResult),
        },
        logger: logger
      );
    }
    else {
      logger.LogWarning($"unknown type of file: {inputFile}");

      // try process as an assembly file
      inputAssemblyFiles = Enumerable.Repeat(inputFile, 1);
    }

    return inputAssemblyFiles;
  }
}