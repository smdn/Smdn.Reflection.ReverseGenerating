// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.GeneratorOptions.TranslateLanguagePrimitiveType;

#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
[TypeDeclarationTestCase("public readonly struct int", typeof(int), TranslateLanguagePrimitiveTypeDeclaration = true)]
[TypeDeclarationTestCase("public readonly struct Int32", typeof(int), TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = true)]
[TypeDeclarationTestCase("public readonly struct Int32", typeof(int), TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]
#elif NETFRAMEWORK && false
[TypeDeclarationTestCase("public struct int", typeof(int), TranslateLanguagePrimitiveTypeDeclaration = true)]
[TypeDeclarationTestCase("public struct Int32", typeof(int), TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = true)]
[TypeDeclarationTestCase("public struct Int32", typeof(int), TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]
#endif

[TypeDeclarationTestCase("public sealed class string", typeof(string), TranslateLanguagePrimitiveTypeDeclaration = true)]
[TypeDeclarationTestCase("public sealed class String", typeof(string), TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = true)]
[TypeDeclarationTestCase("public sealed class String", typeof(string), TranslateLanguagePrimitiveTypeDeclaration = false, TypeWithNamespace = false)]

#if NET6_0_OR_GREATER
[TypeDeclarationTestCase("public readonly struct Guid", typeof(Guid), TranslateLanguagePrimitiveTypeDeclaration = true)]
[TypeDeclarationTestCase("public readonly struct Guid", typeof(Guid), TranslateLanguagePrimitiveTypeDeclaration = false)]
#else
[TypeDeclarationTestCase("public struct Guid", typeof(Guid), TranslateLanguagePrimitiveTypeDeclaration = true)]
[TypeDeclarationTestCase("public struct Guid", typeof(Guid), TranslateLanguagePrimitiveTypeDeclaration = false)]
#endif

class Placeholder {}
