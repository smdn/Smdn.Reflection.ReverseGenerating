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

internal sealed class Program {
  internal static string LoggerCategoryName => typeof(Program).Assembly.GetName().Name!;

  private static int Main(string[] args)
  {
    var services = new ServiceCollection();

    services.AddLogging(
      builder => builder
        .AddSimpleConsole(static options => options.SingleLine = true)
        .AddFilter(level => VerbosityOption.ParseLogLevel(args) <= level)
    );

    using var serviceProvider = services.BuildServiceProvider();

    var rootCommand = new RootCommandImplementation(serviceProvider).CreateCommand();
    var builder =
      new CommandLineBuilder(rootCommand)
      .UseDefaults()
      .UseExceptionHandler(
        onException: static (ex, context) => {
          Exception? caughtException = ex;

          if (ex is TargetInvocationException exTargetInvocation)
            caughtException = exTargetInvocation.InnerException;

          switch (caughtException) {
            case InvalidCommandOperationException exInvalidCommandOperation:
              Console.Error.WriteLine(exInvalidCommandOperation.Message);
              break;

            case CommandOperationNotSupportedException exCommandOperationNotSupported:
              Console.Error.WriteLine(exCommandOperationNotSupported.Message);
              break;

            default:
              Console.Error.WriteLine(caughtException);
              break;
          }
        },
        errorExitCode: -1
      );

    return builder.Build().Invoke(args);
  }
}
