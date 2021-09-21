// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating {
  public class GeneratorOptions {
    public string Indent { get; set; } = new string(' ', 2);

    public bool IgnorePrivateOrAssembly { get; set; } = true;

    public bool TranslateLanguagePrimitiveTypeDeclaration { get; set; } = false;

    public TypeDeclarationOptions TypeDeclaration { get; } = new();

    public class TypeDeclarationOptions {
      public bool WithNamespace { get; set; } = false;
      public bool WithDeclaringTypeName { get; set; } = false;
      public bool WithAccessibility { get; set; } = true;
      public bool OmitEndOfStatement { get; set; } = false;
    }

    public MemberDeclarationOptions MemberDeclaration { get; } = new();

    public class MemberDeclarationOptions {
      public bool WithNamespace { get; set; } = false;
      public bool WithDeclaringTypeName { get; set; } = false;
      public bool WithAccessibility { get; set; } = true;
      public bool UseDefaultLiteral { get; set; } = true;
      public bool OmitEndOfStatement { get; set; } = false;
      public MethodBodyOption MethodBody { get; set; } = MethodBodyOption.EmptyImplementation;
    }
  }
}
