// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class CommandOperationNotSupportedException : NotSupportedException {
  public CommandOperationNotSupportedException(string message)
    : base(message)
  {
  }
}