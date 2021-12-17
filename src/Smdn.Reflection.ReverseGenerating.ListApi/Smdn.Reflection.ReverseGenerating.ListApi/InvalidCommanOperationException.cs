// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class InvalidCommandOperationException : InvalidOperationException {
  public InvalidCommandOperationException(string message)
    : base(message)
  {
  }
}
