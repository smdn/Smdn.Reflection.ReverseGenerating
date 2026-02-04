// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class ApiListWriterOptions : GeneratorOptions {
  public WriterOptions Writer { get; } = new();

#pragma warning disable CA1034
  public class WriterOptions {
#pragma warning restore CA1034
    public bool OrderStaticMembersFirst { get; set; } = false;
    public bool OmitCompilerGeneratedRecordEqualityMethods { get; set; } = false;
    public bool ThrowIfForwardedTypesCouldNotLoaded { get; set; } = false;
    public bool WriteNullableAnnotationDirective { get; set; } = true;
    public bool ExcludeFixedBufferFieldTypes { get; set; } = true;
    public bool ReconstructExtensionDeclarations { get; set; } = true;
    public bool OrderExtensionDeclarationsFirst { get; set; } = true;

    /* options relevant to header */
    public bool WriteHeader { get; set; } = true;
    public bool WriteAssemblyInfo { get; set; } = true;
    public bool WriteEmbeddedResources { get; set; } = true;
    public bool WriteReferencedAssemblies { get; set; } = true;

    /* options relevant to footer */
    public bool WriteFooter { get; set; } = true;
  }
}
