// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating {
  public class GeneratorOptions {
    public string Indent { get; set; } = new string(' ', 2);

    public bool IgnorePrivateOrAssembly { get; set; } = true;

    public bool TranslateLanguagePrimitiveTypeDeclaration { get; set; } = false;

    public bool TypeDeclarationWithNamespace { get; set; } = false;
    public bool TypeDeclarationWithDeclaringTypeName { get; set; } = false;
    public bool TypeDeclarationWithAccessibility { get; set; } = true;
    public bool TypeDeclarationOmitEndOfStatement { get; set; } = false;

    public bool MemberDeclarationWithNamespace { get; set; } = false;
    public bool MemberDeclarationWithDeclaringTypeName { get; set; } = false;
    public bool MemberDeclarationWithAccessibility { get; set; } = true;
    public bool MemberDeclarationUseDefaultLiteral { get; set; } = true;
    public bool MemberDeclarationOmitEndOfStatement { get; set; } = false;
    public MethodBodyOption MemberDeclarationMethodBody { get; set; } = MethodBodyOption.EmptyImplementation;
  }
}
