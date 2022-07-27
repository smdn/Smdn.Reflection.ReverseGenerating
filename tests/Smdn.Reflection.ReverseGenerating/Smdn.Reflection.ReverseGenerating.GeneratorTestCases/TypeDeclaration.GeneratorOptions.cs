// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.GeneratorOptions;

[TypeDeclarationTestCase("public class C1", TypeWithAccessibility = true, TypeWithNamespace = false)]
[TypeDeclarationTestCase("class C1", TypeWithAccessibility = false, TypeWithNamespace = false)]
public class C1 { }

[TypeDeclarationTestCase("internal class C2", TypeWithAccessibility = true, TypeWithNamespace = false)]
[TypeDeclarationTestCase("class C2", TypeWithAccessibility = false, TypeWithNamespace = false)]
class C2 { }

[TypeDeclarationTestCase("delegate void D();", TypeWithAccessibility = false, TypeWithNamespace = false)]
[TypeDeclarationTestCase("delegate void D()", TypeWithAccessibility = false, TypeWithNamespace = false, TypeOmitEndOfStatement = true)]
delegate void D();

[TypeDeclarationTestCase("enum E : int", TypeWithAccessibility = false, TypeWithNamespace = false)] enum E { }
[TypeDeclarationTestCase("interface I", TypeWithAccessibility = false, TypeWithNamespace = false)] interface I { }
[TypeDeclarationTestCase("struct S", TypeWithAccessibility = false, TypeWithNamespace = false)] struct S { }
