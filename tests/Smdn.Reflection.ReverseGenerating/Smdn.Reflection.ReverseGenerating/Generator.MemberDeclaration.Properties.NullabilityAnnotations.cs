// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT

#nullable enable annotations
#pragma warning disable CS8618, CS8597

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Properties;

public class NullabilityAnnotations {
  [MemberDeclarationTestCase($"public int {nameof(ValueType)} {{ get; }}")] public int ValueType => throw null;
  [MemberDeclarationTestCase($"public int? {nameof(NullableValueType)} {{ get; }}")] public int? NullableValueType => throw null;

  [MemberDeclarationTestCase($"public string {nameof(RefType)} {{ get; }}")] public string RefType => throw null;
  [MemberDeclarationTestCase($"public string? {nameof(NullableRefType)} {{ get; }}")] public string? NullableRefType => throw null;

  [MemberDeclarationTestCase($"public System.Guid {nameof(NonLanguagePrimitiveValueType)} {{ get; }}")]
  public Guid NonLanguagePrimitiveValueType => throw null;
  [MemberDeclarationTestCase($"public System.Guid? {nameof(NullableNonLanguagePrimitiveValueType)} {{ get; }}")]
  [MemberDeclarationTestCase($"public Guid? {nameof(NullableNonLanguagePrimitiveValueType)} {{ get; }}", MemberWithNamespace = false)]
  [MemberDeclarationTestCase($"System.Guid? {nameof(NullableNonLanguagePrimitiveValueType)} {{ get; }}", MemberWithAccessibility = false)]
  [MemberDeclarationTestCase($"public Guid? {nameof(NullabilityAnnotations)}.{nameof(NullableNonLanguagePrimitiveValueType)} {{ get; }}", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  public Guid? NullableNonLanguagePrimitiveValueType => throw null;

  [MemberDeclarationTestCase($"public System.Uri {nameof(NonLanguagePrimitiveRefType)} {{ get; }}")]
  public Uri NonLanguagePrimitiveRefType => throw null;
  [MemberDeclarationTestCase($"public System.Uri? {nameof(NullableNonLanguagePrimitiveRefType)} {{ get; }}")]
  [MemberDeclarationTestCase($"public Uri? {nameof(NullableNonLanguagePrimitiveRefType)} {{ get; }}", MemberWithNamespace = false)]
  [MemberDeclarationTestCase($"System.Uri? {nameof(NullableNonLanguagePrimitiveRefType)} {{ get; }}", MemberWithAccessibility = false)]
  [MemberDeclarationTestCase($"public Uri? {nameof(NullabilityAnnotations)}.{nameof(NullableNonLanguagePrimitiveRefType)} {{ get; }}", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  public Uri? NullableNonLanguagePrimitiveRefType => throw null;

  [MemberDeclarationTestCase($"public int[] {nameof(ArrayOfValueType)} {{ get; }}")] public int[] ArrayOfValueType => throw null;
  [MemberDeclarationTestCase($"public int[]? {nameof(NullableArrayOfValueType)} {{ get; }}")] public int[]? NullableArrayOfValueType => throw null;
  [MemberDeclarationTestCase($"public int?[] {nameof(ArrayOfNullableValueType)} {{ get; }}")] public int?[] ArrayOfNullableValueType => throw null;
  [MemberDeclarationTestCase($"public int?[]? {nameof(NullableArrayOfNullableValueType)} {{ get; }}")] public int?[]? NullableArrayOfNullableValueType => throw null;

  [MemberDeclarationTestCase($"public string[] {nameof(ArrayOfRefType)} {{ get; }}")] public string[] ArrayOfRefType => throw null;
  [MemberDeclarationTestCase($"public string[]? {nameof(NullableArrayOfRefType)} {{ get; }}")] public string[]? NullableArrayOfRefType => throw null;
  [MemberDeclarationTestCase($"public string?[] {nameof(ArrayOfNullableRefType)} {{ get; }}")] public string?[] ArrayOfNullableRefType => throw null;
  [MemberDeclarationTestCase($"public string?[]? {nameof(NullableArrayOfNullableRefType)} {{ get; }}")] public string?[]? NullableArrayOfNullableRefType => throw null;

  [MemberDeclarationTestCase($"public int[,] {nameof(TwoDimArrayOfValueType)} {{ get; }}")] public int[,] TwoDimArrayOfValueType => throw null;
  [MemberDeclarationTestCase($"public int?[,] {nameof(TwoDimArrayOfNullableValueType)} {{ get; }}")] public int?[,] TwoDimArrayOfNullableValueType => throw null;
  [MemberDeclarationTestCase($"public int?[,,] {nameof(ThreeDimArrayOfNullableValueType)} {{ get; }}")] public int?[,,] ThreeDimArrayOfNullableValueType => throw null;
  [MemberDeclarationTestCase($"public int[,]? {nameof(NullableTwoDimArrayOfValueType)} {{ get; }}")] public int[,]? NullableTwoDimArrayOfValueType => throw null;
  [MemberDeclarationTestCase($"public int?[,]? {nameof(NullableTwoDimArrayOfNullableValueType)} {{ get; }}")] public int?[,]? NullableTwoDimArrayOfNullableValueType => throw null;
  [MemberDeclarationTestCase($"public int?[,,]? {nameof(NullableThreeDimArrayOfNullableValueType)} {{ get; }}")] public int?[,,]? NullableThreeDimArrayOfNullableValueType => throw null;

  [MemberDeclarationTestCase($"public string[,] {nameof(TwoDimArrayOfRefType)} {{ get; }}")] public string[,] TwoDimArrayOfRefType => throw null;
  [MemberDeclarationTestCase($"public string?[,] {nameof(TwoDimArrayOfNullableRefType)} {{ get; }}")] public string?[,] TwoDimArrayOfNullableRefType => throw null;
  [MemberDeclarationTestCase($"public string?[,,] {nameof(ThreeDimArrayOfNullableRefType)} {{ get; }}")] public string?[,,] ThreeDimArrayOfNullableRefType => throw null;
  [MemberDeclarationTestCase($"public string[,]? {nameof(NullableTwoDimArrayOfRefType)} {{ get; }}")] public string[,]? NullableTwoDimArrayOfRefType => throw null;
  [MemberDeclarationTestCase($"public string?[,]? {nameof(NullableTwoDimArrayOfNullableRefType)} {{ get; }}")] public string?[,]? NullableTwoDimArrayOfNullableRefType => throw null;
  [MemberDeclarationTestCase($"public string?[,,]? {nameof(NullableThreeDimArrayOfNullableRefType)} {{ get; }}")] public string?[,,]? NullableThreeDimArrayOfNullableRefType => throw null;

  [MemberDeclarationTestCase($"public int[][] {nameof(ArrayOfArrayOfValueType)} {{ get; }}")] public int[][] ArrayOfArrayOfValueType => throw null;
  [MemberDeclarationTestCase($"public int?[][] {nameof(ArrayOfArrayOfNullableValueType)} {{ get; }}")] public int?[][] ArrayOfArrayOfNullableValueType => throw null;
  [MemberDeclarationTestCase($"public int?[]?[] {nameof(ArrayOfNullableArrayOfNullableValueType)} {{ get; }}")] public int?[]?[] ArrayOfNullableArrayOfNullableValueType => throw null;
  [MemberDeclarationTestCase($"public int?[][]? {nameof(NullableArrayOfArrayOfNullableValueType)} {{ get; }}")] public int?[][]? NullableArrayOfArrayOfNullableValueType => throw null;
  [MemberDeclarationTestCase($"public int?[]?[]? {nameof(NullableArrayOfNullableArrayOfNullableValueType)} {{ get; }}")] public int?[]?[]? NullableArrayOfNullableArrayOfNullableValueType => throw null;

  [MemberDeclarationTestCase($"public string[][] {nameof(ArrayOfArrayOfRefType)} {{ get; }}")] public string[][] ArrayOfArrayOfRefType => throw null;
  [MemberDeclarationTestCase($"public string?[][] {nameof(ArrayOfArrayOfNullableRefType)} {{ get; }}")] public string?[][] ArrayOfArrayOfNullableRefType => throw null;
  [MemberDeclarationTestCase($"public string?[]?[] {nameof(ArrayOfNullableArrayOfNullableRefType)} {{ get; }}")] public string?[]?[] ArrayOfNullableArrayOfNullableRefType => throw null;
  [MemberDeclarationTestCase($"public string?[][]? {nameof(NullableArrayOfArrayOfNullableRefType)} {{ get; }}")] public string?[][]? NullableArrayOfArrayOfNullableRefType => throw null;
  [MemberDeclarationTestCase($"public string?[]?[]? {nameof(NullableArrayOfNullableArrayOfNullableRefType)} {{ get; }}")] public string?[]?[]? NullableArrayOfNullableArrayOfNullableRefType => throw null;

  [MemberDeclarationTestCase($"public (int, int) {nameof(ValueTupleOfValueType)} {{ get; }}")] public (int, int) ValueTupleOfValueType => throw null;
  [MemberDeclarationTestCase($"public (int, int?) {nameof(ValueTupleOfNullableValueType)} {{ get; }}")] public (int, int?) ValueTupleOfNullableValueType => throw null;
  [MemberDeclarationTestCase($"public (int, int[]) {nameof(ValueTupleOfArrayOValueType)} {{ get; }}")] public (int, int[]) ValueTupleOfArrayOValueType => throw null;
  [MemberDeclarationTestCase($"public (int, int?[]) {nameof(ValueTupleOfArrayOfNullableValueType)} {{ get; }}")] public (int, int?[]) ValueTupleOfArrayOfNullableValueType => throw null;
  [MemberDeclarationTestCase($"public (int, int[]?) {nameof(ValueTupleOfNullableArrayOfValueType)} {{ get; }}")] public (int, int[]?) ValueTupleOfNullableArrayOfValueType => throw null;
  [MemberDeclarationTestCase($"public (int, int?[]?) {nameof(ValueTupleOfNullableArrayOfNullableValueType)} {{ get; }}")] public (int, int?[]?) ValueTupleOfNullableArrayOfNullableValueType => throw null;

  [MemberDeclarationTestCase($"public (int, string) {nameof(ValueTupleOfRefType)} {{ get; }}")] public (int, string) ValueTupleOfRefType => throw null;
  [MemberDeclarationTestCase($"public (int, string?) {nameof(ValueTupleOfNullableRefType)} {{ get; }}")] public (int, string?) ValueTupleOfNullableRefType => throw null;
  [MemberDeclarationTestCase($"public (int, string[]) {nameof(ValueTupleOfArrayORefType)} {{ get; }}")] public (int, string[]) ValueTupleOfArrayORefType => throw null;
  [MemberDeclarationTestCase($"public (int, string?[]) {nameof(ValueTupleOfArrayOfNullableRefType)} {{ get; }}")] public (int, string?[]) ValueTupleOfArrayOfNullableRefType => throw null;
  [MemberDeclarationTestCase($"public (int, string[]?) {nameof(ValueTupleOfNullableArrayOfRefType)} {{ get; }}")] public (int, string[]?) ValueTupleOfNullableArrayOfRefType => throw null;
  [MemberDeclarationTestCase($"public (int, string?[]?) {nameof(ValueTupleOfNullableArrayOfNullableRefType)} {{ get; }}")] public (int, string?[]?) ValueTupleOfNullableArrayOfNullableRefType => throw null;

  [MemberDeclarationTestCase($"public (string, string) {nameof(ValueTupleOfRefTypeAndRefType)} {{ get; }}")] public (string, string) ValueTupleOfRefTypeAndRefType => throw null;
  [MemberDeclarationTestCase($"public (string?, string) {nameof(ValueTupleOfNullableRefTypeAndRefType)} {{ get; }}")] public (string?, string) ValueTupleOfNullableRefTypeAndRefType => throw null;
  [MemberDeclarationTestCase($"public (string, string?) {nameof(ValueTupleOfRefTypeAndNullableRefType)} {{ get; }}")] public (string, string?) ValueTupleOfRefTypeAndNullableRefType => throw null;
  [MemberDeclarationTestCase($"public (string?, string?) {nameof(ValueTupleOfNullableRefTypeAndNullableRefType)} {{ get; }}")] public (string?, string?) ValueTupleOfNullableRefTypeAndNullableRefType => throw null;
  [MemberDeclarationTestCase($"public (string?, string?)? {nameof(NullableValueTupleOfNullableRefTypeAndNullableRefType)} {{ get; }}")] public (string?, string?)? NullableValueTupleOfNullableRefTypeAndNullableRefType => throw null;

  [MemberDeclarationTestCase($"public (string X, string Y) {nameof(ValueTupleOfRefTypeAndRefTypeWithElementNames)} {{ get; }}")] public (string X, string Y) ValueTupleOfRefTypeAndRefTypeWithElementNames => throw null;
  [MemberDeclarationTestCase($"public (string? X, string? Y)? {nameof(NullableValueTupleOfNullableRefTypeAndNullableRefTypeWithElementNames)} {{ get; }}")] public (string? X, string? Y)? NullableValueTupleOfNullableRefTypeAndNullableRefTypeWithElementNames => throw null;

  [MemberDeclarationTestCase($"public List<int> {nameof(ListOfValueType)} {{ get; }}", MemberWithNamespace = false)] public List<int> ListOfValueType => throw null;
  [MemberDeclarationTestCase($"public List<int?> {nameof(ListOfNullableValueType)} {{ get; }}", MemberWithNamespace = false)] public List<int?> ListOfNullableValueType => throw null;
  [MemberDeclarationTestCase($"public List<int>? {nameof(ListOfValueTypeNullable)} {{ get; }}", MemberWithNamespace = false)] public List<int>? ListOfValueTypeNullable => throw null;
  [MemberDeclarationTestCase($"public List<int?>? {nameof(NullableListOfNullableValueType)} {{ get; }}", MemberWithNamespace = false)] public List<int?>? NullableListOfNullableValueType => throw null;

  [MemberDeclarationTestCase($"public List<string> {nameof(ListOfRefType)} {{ get; }}", MemberWithNamespace = false)] public List<string> ListOfRefType => throw null;
  [MemberDeclarationTestCase($"public List<string?> {nameof(ListOfNullableRefType)} {{ get; }}", MemberWithNamespace = false)] public List<string?> ListOfNullableRefType => throw null;
  [MemberDeclarationTestCase($"public List<string>? {nameof(ListOfRefTypeNullable)} {{ get; }}", MemberWithNamespace = false)] public List<string>? ListOfRefTypeNullable => throw null;
  [MemberDeclarationTestCase($"public List<string?>? {nameof(NullableListOfNullableRefType)} {{ get; }}", MemberWithNamespace = false)] public List<string?>? NullableListOfNullableRefType => throw null;

  [MemberDeclarationTestCase($"public Dictionary<string, string> {nameof(DictionaryOfRefTypeValue)} {{ get; }}", MemberWithNamespace = false)] public Dictionary<string, string> DictionaryOfRefTypeValue => throw null;
  [MemberDeclarationTestCase($"public Dictionary<string, string?> {nameof(DictionaryOfNullableRefTypeValue)} {{ get; }}", MemberWithNamespace = false)] public Dictionary<string, string?> DictionaryOfNullableRefTypeValue => throw null;
  [MemberDeclarationTestCase($"public Dictionary<string, string>? {nameof(DictionaryOfRefTypeValueNullable)} {{ get; }}", MemberWithNamespace = false)] public Dictionary<string, string>? DictionaryOfRefTypeValueNullable => throw null;
  [MemberDeclarationTestCase($"public Dictionary<string, string?>? {nameof(NullableDictionaryOfNullableRefTypeValue)} {{ get; }}", MemberWithNamespace = false)] public Dictionary<string, string?>? NullableDictionaryOfNullableRefTypeValue => throw null;

  public class Indexers {
    public class IndexTypes {
      public abstract class ValueType {
        [MemberDeclarationTestCase("public abstract int this[int x] { get; set; }")]
        public abstract int this[int x] { get; set; }
      }
      public abstract class NullableValueType {
        [MemberDeclarationTestCase("public abstract int this[int? x] { get; set; }")]
        public abstract int this[int? x] { get; set; }
      }
      public abstract class RefType {
        [MemberDeclarationTestCase("public abstract int this[string x] { get; set; }")]
        public abstract int this[string x] { get; set; }
      }
      public abstract class NullableRefType {
        [MemberDeclarationTestCase("public abstract int this[string? x] { get; set; }")]
        public abstract int this[string? x] { get; set; }
      }
    }

    public class IndexerValueTypes {
      public abstract class ValueType {
        [MemberDeclarationTestCase("public abstract int this[int x] { get; set; }")]
        public abstract int this[int x] { get; set; }
      }
      public abstract class NullableValueType {
        [MemberDeclarationTestCase("public abstract int? this[int x] { get; set; }")]
        public abstract int? this[int x] { get; set; }
      }
      public abstract class RefType {
        [MemberDeclarationTestCase("public abstract string this[int x] { get; set; }")]
        public abstract string this[int x] { get; set; }
      }
      public abstract class NullableRefType {
        [MemberDeclarationTestCase("public abstract string? this[int x] { get; set; }")]
        public abstract string? this[int x] { get; set; }
      }
    }
  }

  class NullabilityAnnotationOptions {
    [MemberDeclarationTestCase($"public int {nameof(ValueType)} {{ get; }}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public int {nameof(ValueType)} {{ get; }}", MemberEnableNullabilityAnnotations = true)]
    public int ValueType => throw null;

    [MemberDeclarationTestCase($"public int? {nameof(NullableValueType)} {{ get; }}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public int? {nameof(NullableValueType)} {{ get; }}", MemberEnableNullabilityAnnotations = true)]
    public int? NullableValueType => throw null;

    [MemberDeclarationTestCase($"public string {nameof(RefType)} {{ get; }}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public string {nameof(RefType)} {{ get; }}", MemberEnableNullabilityAnnotations = true)]
    public string RefType => throw null;

    [MemberDeclarationTestCase($"public string {nameof(NullableRefType)} {{ get; }}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public string? {nameof(NullableRefType)} {{ get; }}", MemberEnableNullabilityAnnotations = true)]
    public string? NullableRefType => throw null;
  }
}
#endif
