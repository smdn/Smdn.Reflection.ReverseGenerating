// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class ApiListWriterOptions : GeneratorOptions {
  public WriterOptions Writer { get; } = new();

  public class WriterOptions {
    public bool OrderStaticMembersFirst { get; set; } = false;
    public bool WriteNullableAnnotationDirective { get; set; } = true;
    public bool WriteEmbeddedResources { get; set; } = true;
    public bool WriteReferencedAssemblies { get; set; } = true;
    public bool WriteFooter { get; set; } = true;
  }
}
