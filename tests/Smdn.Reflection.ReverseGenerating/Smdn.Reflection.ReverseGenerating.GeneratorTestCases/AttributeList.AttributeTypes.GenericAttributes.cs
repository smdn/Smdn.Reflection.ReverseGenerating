// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if NET7_0_OR_GREATER
using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.AttributeList.AttributeTypes.GenericAttributes;

[AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
public class GenericAttribute<T> : Attribute { }

[AttributeListTestCase(
  "[Generic<int>], [Generic<string>]",
  AttributeWithNamespace = false,
  TranslateLanguagePrimitiveTypeDeclaration = true
)]
[AttributeListTestCase(
  "[Generic<Int32>], [Generic<String>]",
  AttributeWithNamespace = false,
  TranslateLanguagePrimitiveTypeDeclaration = false
)]
[Generic<int>]
[Generic<string>]
public class CGenericAttributeOfInt32AndString {}

[AttributeListTestCase("[Generic<List<int>>]", AttributeWithNamespace = false)]
[Generic<System.Collections.Generic.List<int>>]
public class CGenericAttributeOfListOfInt32 {}

#endif // NET7_0_OR_GREATER
