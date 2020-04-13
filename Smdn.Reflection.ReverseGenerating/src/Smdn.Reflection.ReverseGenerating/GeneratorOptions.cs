// 
// Copyright (c) 2020 smdn <smdn@smdn.jp>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
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
    public bool MemberDeclarationUseDefaultLiteral { get; set; } = true;
    public bool MemberDeclarationOmitEndOfStatement { get; set; } = false;
    public MethodBodyOption MemberDeclarationMethodBody { get; set; } = MethodBodyOption.EmptyImplementation;
  }
}
