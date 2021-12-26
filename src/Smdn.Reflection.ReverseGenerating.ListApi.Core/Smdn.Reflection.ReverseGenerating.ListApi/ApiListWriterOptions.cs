// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class ApiListWriterOptions : GeneratorOptions {
  public WriterOptions Writer { get; } = new();

  public class WriterOptions {
    public bool OrderStaticMembersFirst { get; set; } = false;
  }
}
