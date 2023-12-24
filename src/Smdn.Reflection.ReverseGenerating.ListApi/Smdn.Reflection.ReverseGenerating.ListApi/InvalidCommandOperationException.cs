// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class InvalidCommandOperationException : InvalidOperationException {
  public InvalidCommandOperationException()
    : base()
  {
  }

  public InvalidCommandOperationException(string? message)
    : base(message)
  {
  }

  public InvalidCommandOperationException(string? message, Exception? innerException)
    : base(message, innerException)
  {
  }
}
