using System;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class CommandOperationNotSupportedException : NotSupportedException {
  public CommandOperationNotSupportedException(string message)
    : base(message)
  {
  }
}