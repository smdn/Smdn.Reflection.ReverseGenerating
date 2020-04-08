using System;

namespace Smdn.Reflection.ReverseGenerating {
  public class GeneratorOptions {
    public string Indent { get; set; } = new string(' ', 2);

    public bool IgnorePrivateOrAssembly { get; set; } = true;
    public bool TypeDeclarationWithNamespace { get; set; } = false;

    public bool MemberDeclarationWithNamespace { get; set; } = false;
    public bool MemberDeclarationUseDefaultLiteral { get; set; } = true;
  }
}
