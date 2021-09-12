using System;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class InvalidCommandOperationException : InvalidOperationException {
  public InvalidCommandOperationException(string message)
    : base(message)
  {
  }
}