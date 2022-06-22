// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT

#nullable enable annotations

using System;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating.TestCases.TypeDeclaration.Delegates;

public class NullabilityAnnotations {
  class Parameters {
    [TypeDeclarationTestCase($"public delegate void {nameof(ValueType)}(int x);")] public delegate void ValueType(int x);
    [TypeDeclarationTestCase($"public delegate void {nameof(NullableValueType)}(int? x);")] public delegate void NullableValueType(int? x);

    [TypeDeclarationTestCase($"public delegate void {nameof(RefType)}(string x);")] public delegate void RefType(string x);
    [TypeDeclarationTestCase($"public delegate void {nameof(NullableRefType)}(string? x);")] public delegate void NullableRefType(string? x);
  }

  class ReturnParameters {
    [TypeDeclarationTestCase($"public delegate int {nameof(ValueType)}();")] public delegate int ValueType();
    [TypeDeclarationTestCase($"public delegate int? {nameof(NullableValueType)}();")] public delegate int? NullableValueType();

    [TypeDeclarationTestCase($"public delegate string {nameof(RefType)}();")] public delegate string RefType();
    [TypeDeclarationTestCase($"public delegate string? {nameof(NullableRefType)}();")] public delegate string? NullableRefType();
  }

  class NullabilityAnnotationOptions {
    class Parameters {
      [TypeDeclarationTestCase($"public delegate void {nameof(ValueType)}(int x);", TypeEnableNullabilityAnnotations = false)]
      [TypeDeclarationTestCase($"public delegate void {nameof(ValueType)}(int x);", TypeEnableNullabilityAnnotations = true)]
      public delegate void ValueType(int x);

      [TypeDeclarationTestCase($"public delegate void {nameof(NullableValueType)}(int? x);", TypeEnableNullabilityAnnotations = false)]
      [TypeDeclarationTestCase($"public delegate void {nameof(NullableValueType)}(int? x);", TypeEnableNullabilityAnnotations = true)]
      public delegate void NullableValueType(int? x);

      [TypeDeclarationTestCase($"public delegate void {nameof(RefType)}(string x);", TypeEnableNullabilityAnnotations = false)]
      [TypeDeclarationTestCase($"public delegate void {nameof(RefType)}(string x);", TypeEnableNullabilityAnnotations = true)]
      public delegate void RefType(string x);

      [TypeDeclarationTestCase($"public delegate void {nameof(NullableRefType)}(string x);", TypeEnableNullabilityAnnotations = false)]
      [TypeDeclarationTestCase($"public delegate void {nameof(NullableRefType)}(string? x);", TypeEnableNullabilityAnnotations = true)]
      public delegate void NullableRefType(string? x);
    }

    class ReturnParameters {
      [TypeDeclarationTestCase($"public delegate int {nameof(ValueType)}();", TypeEnableNullabilityAnnotations = false)]
      [TypeDeclarationTestCase($"public delegate int {nameof(ValueType)}();", TypeEnableNullabilityAnnotations = true)]
      public delegate int ValueType();

      [TypeDeclarationTestCase($"public delegate int? {nameof(NullableValueType)}();", TypeEnableNullabilityAnnotations = false)]
      [TypeDeclarationTestCase($"public delegate int? {nameof(NullableValueType)}();", TypeEnableNullabilityAnnotations = true)]
      public delegate int? NullableValueType();

      [TypeDeclarationTestCase($"public delegate string {nameof(RefType)}();", TypeEnableNullabilityAnnotations = false)]
      [TypeDeclarationTestCase($"public delegate string {nameof(RefType)}();", TypeEnableNullabilityAnnotations = true)]
      public delegate string RefType();

      [TypeDeclarationTestCase($"public delegate string {nameof(NullableRefType)}();", TypeEnableNullabilityAnnotations = false)]
      [TypeDeclarationTestCase($"public delegate string? {nameof(NullableRefType)}();", TypeEnableNullabilityAnnotations = true)]
      public delegate string? NullableRefType();
    }
  }
}
#endif
