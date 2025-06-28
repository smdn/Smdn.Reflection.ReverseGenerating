// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

internal sealed class Program {
  internal static string LoggerCategoryName => Constants.ToolCommandName;

  private static int Main(string[] args)
  {
    var logLevel = VerbosityOption.ParseLogLevel(args);
    var services = new ServiceCollection();

    services.AddLogging(
      builder => builder
        .AddSimpleConsole(static options => options.SingleLine = true)
        .AddFilter(level => logLevel <= level)
    );

    using var serviceProvider = services.BuildServiceProvider();

    var rootCommand = new RootCommandImplementation(serviceProvider).CreateCommand();

    try {
      return rootCommand.Parse(args).Invoke();
    }
    catch (InvalidCommandOperationException ex) {
      Console.Error.WriteLine(ex.Message);
      return -1;
    }
    catch (CommandOperationNotSupportedException ex) {
      Console.Error.WriteLine(ex.Message);
      return -1;
    }
  }
}
