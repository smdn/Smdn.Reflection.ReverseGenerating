// SPDX-FileCopyrightText: 2024 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Reflection;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public sealed class AssemblyFileNotFoundException : FileNotFoundException {
  internal static AssemblyFileNotFoundException Create(AssemblyName? assemblyName, FileNotFoundException ex)
    => new(
      message: $"Could not load assembly '{assemblyName}'.",
      fileName: ex.FileName,
      innerException: ex
    );

  public AssemblyFileNotFoundException()
    : base()
  {
  }

  public AssemblyFileNotFoundException(string? message)
    : base(message)
  {
  }

  public AssemblyFileNotFoundException(string? message, Exception? innerException)
    : base(message, innerException)
  {
  }

  public AssemblyFileNotFoundException(string? message, string? fileName, Exception? innerException)
    : base(message, fileName, innerException)
  {
  }
}
