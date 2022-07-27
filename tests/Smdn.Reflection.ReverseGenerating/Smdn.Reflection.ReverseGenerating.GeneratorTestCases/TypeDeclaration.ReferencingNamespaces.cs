// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.ReferencingNamespaces;

[ReferencingNamespacesTestCase("")] delegate void D0();
[ReferencingNamespacesTestCase("")] delegate void D1(int x);
[ReferencingNamespacesTestCase("")] delegate void D2(int? x);
[ReferencingNamespacesTestCase("")] delegate void D3(int[] x);
[ReferencingNamespacesTestCase("")] unsafe delegate void D4(int* x);
[ReferencingNamespacesTestCase("System")] delegate void D5(Guid x);
[ReferencingNamespacesTestCase("System")] delegate void D6(Guid? x);
[ReferencingNamespacesTestCase("System")] delegate void D7(Guid[] x);
[ReferencingNamespacesTestCase("System.Collections.Generic", TranslateLanguagePrimitiveTypeDeclaration = true)] delegate void D8(List<int> x);
[SkipTestCase("TODO: consider TranslateLanguagePrimitive")][ReferencingNamespacesTestCase("System, System.Collections.Generic", TranslateLanguagePrimitiveTypeDeclaration = false)] delegate void D9(List<int> x);
[ReferencingNamespacesTestCase("System, System.Collections.Generic")] delegate void D10(List<Guid> x);

[ReferencingNamespacesTestCase("", WithExplicitBaseTypeAndInterfaces = false)]
[ReferencingNamespacesTestCase("System.Collections.Generic", WithExplicitBaseTypeAndInterfaces = true, TranslateLanguagePrimitiveTypeDeclaration = true)]
[ReferencingNamespacesTestCase("System.Collections.Generic", WithExplicitBaseTypeAndInterfaces = true, TranslateLanguagePrimitiveTypeDeclaration = false)]
class ListEx1 : List<int> {}

[ReferencingNamespacesTestCase("", WithExplicitBaseTypeAndInterfaces = false)]
[ReferencingNamespacesTestCase("System, System.Collections.Generic", WithExplicitBaseTypeAndInterfaces = true)]
class ListEx2 : List<Guid> {}

[ReferencingNamespacesTestCase("", WithExplicitBaseTypeAndInterfaces = false)]
[ReferencingNamespacesTestCase("System.Collections.Generic", WithExplicitBaseTypeAndInterfaces = true)]
class ListEx3<T> : List<T> {}
