// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class CommandOperationNotSupportedException : NotSupportedException {
  public CommandOperationNotSupportedException()
    : base()
  {
  }

  public CommandOperationNotSupportedException(string? message)
    : base(message)
  {
  }

  public CommandOperationNotSupportedException(string? message, Exception? innerException)
    : base(message, innerException)
  {
  }
}
