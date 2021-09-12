using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

class Program {
  static int Main(string[] args)
  {
    var services = new ServiceCollection();

    services.AddLogging(
      builder => builder
        .AddSimpleConsole(static options => options.SingleLine = true)
        .AddFilter(level => VerbosityOption.ParseLogLevel(args) <= level)
    );

    var rootCommand = new RootCommandImplementation(serviceProvider: services.BuildServiceProvider()).CreateCommand();
    var builder =
      new CommandLineBuilder(rootCommand)
      .UseDefaults()
      .UseExceptionHandler(
        onException: static (ex, context) => {
          if (ex is TargetInvocationException exTargetInvocation)
            ex = exTargetInvocation.InnerException;

          switch (ex) {
            case InvalidCommandOperationException exInvalidCommandOperation: Console.Error.WriteLine(ex.Message); break;
            case CommandOperationNotSupportedException exCommandOperationNotSupported: Console.Error.WriteLine(ex.Message); break;
            default: Console.Error.WriteLine(ex); break;
          }
        },
        errorExitCode: -1
      );

    return builder.Build().Invoke(args);
  }
}