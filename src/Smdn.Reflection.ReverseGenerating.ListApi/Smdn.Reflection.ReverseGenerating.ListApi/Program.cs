// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

internal class Program {
  internal static string LoggerCategoryName => typeof(Program).Assembly.GetName().Name;

  private static int Main(string[] args)
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

#if false
    // [BUG?] System.CommandLine 2.0.0-beta1.21308.1 ignores first <FileSystemInfo> argument?
    // `--version`    --- works
    // `-v n foo.dll` --- works
    // `foo.dll -v n` --- ignores argument `foo.dll`
    // `foo.dll`      --- ignores argument `foo.dll`
    // `--foo`        --- parses as filename `--foo`

    return builder.Build().Invoke(args);
#else
    return builder.Build().Invoke(Environment.GetCommandLineArgs());
#endif
  }
}
