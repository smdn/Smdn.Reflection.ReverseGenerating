// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks;

internal class LoggerAdapter : Microsoft.Extensions.Logging.ILogger {
  private class NullScope : IDisposable {
    public static readonly NullScope Instance = new();

    public void Dispose()
    {
      // do nothing
    }
  }

  private readonly TaskLoggingHelper? log;

  public LoggerAdapter(TaskLoggingHelper? log)
  {
    this.log = log;
  }

  public IDisposable BeginScope<TState>(TState state)
    => NullScope.Instance;

  public bool IsEnabled(LogLevel logLevel)
    => log is not null;

  public void Log<TState>(
    LogLevel logLevel,
    EventId eventId,
    TState state,
    Exception? exception,
    Func<TState, Exception?, string> formatter
  )
  {
    switch (logLevel) {
      case LogLevel.Critical:
      case LogLevel.Error:
        log?.LogError(
          subcategory: null,
          errorCode: eventId == default ? null : eventId.ToString(),
          helpKeyword: null,
          helpLink: null,
          file: null,
          lineNumber: 0,
          columnNumber: 0,
          endLineNumber: 0,
          endColumnNumber: 0,
          message: formatter(state, exception),
          messageArgs: null
        );
        break;

      case LogLevel.Warning:
        log?.LogWarning(
          subcategory: null,
          warningCode: eventId == default ? null : eventId.ToString(),
          helpKeyword: null,
          helpLink: null,
          file: null,
          lineNumber: 0,
          columnNumber: 0,
          endLineNumber: 0,
          endColumnNumber: 0,
          message: formatter(state, exception),
          messageArgs: null
        );
        break;

      case LogLevel.Information:
      case LogLevel.Debug:
      case LogLevel.Trace:
        log?.LogMessage(
          subcategory: null,
          code: eventId == default ? null : eventId.ToString(),
          helpKeyword: null,
          file: null,
          lineNumber: 0,
          columnNumber: 0,
          endLineNumber: 0,
          endColumnNumber: 0,
          importance: logLevel switch {
            LogLevel.Information => MessageImportance.High,
            LogLevel.Debug => MessageImportance.Normal,
            LogLevel.Trace => MessageImportance.Low,
            _ => default,
          },
          message: formatter(state, exception),
          messageArgs: null
        );
        break;

      case LogLevel.None:
      default:
        log?.LogMessage(MessageImportance.High, $"log level unknown: {logLevel}");
        break;
    }
  }
}
