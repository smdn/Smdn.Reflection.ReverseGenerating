// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT

#nullable enable annotations
#pragma warning disable CS8618

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Fields;

public class NullabilityAnnotations {
  [MemberDeclarationTestCase($"public int {nameof(ValueType)};")] public int ValueType;
  [MemberDeclarationTestCase($"public int? {nameof(NullableValueType)};")] public int? NullableValueType;

  [MemberDeclarationTestCase($"public string {nameof(RefType)};")] public string RefType;
  [MemberDeclarationTestCase($"public string? {nameof(NullableRefType)};")] public string? NullableRefType;

  [MemberDeclarationTestCase($"public System.Guid {nameof(NonLanguagePrimitiveValueType)};")]
  public Guid NonLanguagePrimitiveValueType;
  [MemberDeclarationTestCase($"public System.Guid? {nameof(NullableNonLanguagePrimitiveValueType)};")]
  [MemberDeclarationTestCase($"public Guid? {nameof(NullableNonLanguagePrimitiveValueType)};", MemberWithNamespace = false)]
  [MemberDeclarationTestCase($"System.Guid? {nameof(NullableNonLanguagePrimitiveValueType)};", MemberWithAccessibility = false)]
  [MemberDeclarationTestCase($"public Guid? {nameof(NullabilityAnnotations)}.{nameof(NullableNonLanguagePrimitiveValueType)};", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  public Guid? NullableNonLanguagePrimitiveValueType;

  [MemberDeclarationTestCase($"public System.Uri {nameof(NonLanguagePrimitiveRefType)};")]
  public Uri NonLanguagePrimitiveRefType;
  [MemberDeclarationTestCase($"public System.Uri? {nameof(NullableNonLanguagePrimitiveRefType)};")]
  [MemberDeclarationTestCase($"public Uri? {nameof(NullableNonLanguagePrimitiveRefType)};", MemberWithNamespace = false)]
  [MemberDeclarationTestCase($"System.Uri? {nameof(NullableNonLanguagePrimitiveRefType)};", MemberWithAccessibility = false)]
  [MemberDeclarationTestCase($"public Uri? {nameof(NullabilityAnnotations)}.{nameof(NullableNonLanguagePrimitiveRefType)};", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  public Uri? NullableNonLanguagePrimitiveRefType;

  [MemberDeclarationTestCase($"public int[] {nameof(ArrayOfValueType)};")] public int[] ArrayOfValueType;
  [MemberDeclarationTestCase($"public int[]? {nameof(NullableArrayOfValueType)};")] public int[]? NullableArrayOfValueType;
  [MemberDeclarationTestCase($"public int?[] {nameof(ArrayOfNullableValueType)};")] public int?[] ArrayOfNullableValueType;
  [MemberDeclarationTestCase($"public int?[]? {nameof(NullableArrayOfNullableValueType)};")] public int?[]? NullableArrayOfNullableValueType;

  [MemberDeclarationTestCase($"public string[] {nameof(ArrayOfRefType)};")] public string[] ArrayOfRefType;
  [MemberDeclarationTestCase($"public string[]? {nameof(NullableArrayOfRefType)};")] public string[]? NullableArrayOfRefType;
  [MemberDeclarationTestCase($"public string?[] {nameof(ArrayOfNullableRefType)};")] public string?[] ArrayOfNullableRefType;
  [MemberDeclarationTestCase($"public string?[]? {nameof(NullableArrayOfNullableRefType)};")] public string?[]? NullableArrayOfNullableRefType;

  [MemberDeclarationTestCase($"public int[,] {nameof(TwoDimArrayOfValueType)};")] public int[,] TwoDimArrayOfValueType;
  [MemberDeclarationTestCase($"public int?[,] {nameof(TwoDimArrayOfNullableValueType)};")] public int?[,] TwoDimArrayOfNullableValueType;
  [MemberDeclarationTestCase($"public int?[,,] {nameof(ThreeDimArrayOfNullableValueType)};")] public int?[,,] ThreeDimArrayOfNullableValueType;
  [MemberDeclarationTestCase($"public int[,]? {nameof(NullableTwoDimArrayOfValueType)};")] public int[,]? NullableTwoDimArrayOfValueType;
  [MemberDeclarationTestCase($"public int?[,]? {nameof(NullableTwoDimArrayOfNullableValueType)};")] public int?[,]? NullableTwoDimArrayOfNullableValueType;
  [MemberDeclarationTestCase($"public int?[,,]? {nameof(NullableThreeDimArrayOfNullableValueType)};")] public int?[,,]? NullableThreeDimArrayOfNullableValueType;

  [MemberDeclarationTestCase($"public string[,] {nameof(TwoDimArrayOfRefType)};")] public string[,] TwoDimArrayOfRefType;
  [MemberDeclarationTestCase($"public string?[,] {nameof(TwoDimArrayOfNullableRefType)};")] public string?[,] TwoDimArrayOfNullableRefType;
  [MemberDeclarationTestCase($"public string?[,,] {nameof(ThreeDimArrayOfNullableRefType)};")] public string?[,,] ThreeDimArrayOfNullableRefType;
  [MemberDeclarationTestCase($"public string[,]? {nameof(NullableTwoDimArrayOfRefType)};")] public string[,]? NullableTwoDimArrayOfRefType;
  [MemberDeclarationTestCase($"public string?[,]? {nameof(NullableTwoDimArrayOfNullableRefType)};")] public string?[,]? NullableTwoDimArrayOfNullableRefType;
  [MemberDeclarationTestCase($"public string?[,,]? {nameof(NullableThreeDimArrayOfNullableRefType)};")] public string?[,,]? NullableThreeDimArrayOfNullableRefType;

  [MemberDeclarationTestCase($"public int[][] {nameof(ArrayOfArrayOfValueType)};")] public int[][] ArrayOfArrayOfValueType;
  [MemberDeclarationTestCase($"public int?[][] {nameof(ArrayOfArrayOfNullableValueType)};")] public int?[][] ArrayOfArrayOfNullableValueType;
  [MemberDeclarationTestCase($"public int?[]?[] {nameof(ArrayOfNullableArrayOfNullableValueType)};")] public int?[]?[] ArrayOfNullableArrayOfNullableValueType;
  [MemberDeclarationTestCase($"public int?[][]? {nameof(NullableArrayOfArrayOfNullableValueType)};")] public int?[][]? NullableArrayOfArrayOfNullableValueType;
  [MemberDeclarationTestCase($"public int?[]?[]? {nameof(NullableArrayOfNullableArrayOfNullableValueType)};")] public int?[]?[]? NullableArrayOfNullableArrayOfNullableValueType;

  [MemberDeclarationTestCase($"public string[][] {nameof(ArrayOfArrayOfRefType)};")] public string[][] ArrayOfArrayOfRefType;
  [MemberDeclarationTestCase($"public string?[][] {nameof(ArrayOfArrayOfNullableRefType)};")] public string?[][] ArrayOfArrayOfNullableRefType;
  [MemberDeclarationTestCase($"public string?[]?[] {nameof(ArrayOfNullableArrayOfNullableRefType)};")] public string?[]?[] ArrayOfNullableArrayOfNullableRefType;
  [MemberDeclarationTestCase($"public string?[][]? {nameof(NullableArrayOfArrayOfNullableRefType)};")] public string?[][]? NullableArrayOfArrayOfNullableRefType;
  [MemberDeclarationTestCase($"public string?[]?[]? {nameof(NullableArrayOfNullableArrayOfNullableRefType)};")] public string?[]?[]? NullableArrayOfNullableArrayOfNullableRefType;

  [MemberDeclarationTestCase($"public (int, int) {nameof(ValueTupleOfValueType)};")] public (int, int) ValueTupleOfValueType;
  [MemberDeclarationTestCase($"public (int, int?) {nameof(ValueTupleOfNullableValueType)};")] public (int, int?) ValueTupleOfNullableValueType;
  [MemberDeclarationTestCase($"public (int, int[]) {nameof(ValueTupleOfArrayOValueType)};")] public (int, int[]) ValueTupleOfArrayOValueType;
  [MemberDeclarationTestCase($"public (int, int?[]) {nameof(ValueTupleOfArrayOfNullableValueType)};")] public (int, int?[]) ValueTupleOfArrayOfNullableValueType;
  [MemberDeclarationTestCase($"public (int, int[]?) {nameof(ValueTupleOfNullableArrayOfValueType)};")] public (int, int[]?) ValueTupleOfNullableArrayOfValueType;
  [MemberDeclarationTestCase($"public (int, int?[]?) {nameof(ValueTupleOfNullableArrayOfNullableValueType)};")] public (int, int?[]?) ValueTupleOfNullableArrayOfNullableValueType;

  [MemberDeclarationTestCase($"public (int, string) {nameof(ValueTupleOfRefType)};")] public (int, string) ValueTupleOfRefType;
  [MemberDeclarationTestCase($"public (int, string?) {nameof(ValueTupleOfNullableRefType)};")] public (int, string?) ValueTupleOfNullableRefType;
  [MemberDeclarationTestCase($"public (int, string[]) {nameof(ValueTupleOfArrayORefType)};")] public (int, string[]) ValueTupleOfArrayORefType;
  [MemberDeclarationTestCase($"public (int, string?[]) {nameof(ValueTupleOfArrayOfNullableRefType)};")] public (int, string?[]) ValueTupleOfArrayOfNullableRefType;
  [MemberDeclarationTestCase($"public (int, string[]?) {nameof(ValueTupleOfNullableArrayOfRefType)};")] public (int, string[]?) ValueTupleOfNullableArrayOfRefType;
  [MemberDeclarationTestCase($"public (int, string?[]?) {nameof(ValueTupleOfNullableArrayOfNullableRefType)};")] public (int, string?[]?) ValueTupleOfNullableArrayOfNullableRefType;

  [MemberDeclarationTestCase($"public (string, string) {nameof(ValueTupleOfRefTypeAndRefType)};")] public (string, string) ValueTupleOfRefTypeAndRefType;
  [MemberDeclarationTestCase($"public (string?, string) {nameof(ValueTupleOfNullableRefTypeAndRefType)};")] public (string?, string) ValueTupleOfNullableRefTypeAndRefType;
  [MemberDeclarationTestCase($"public (string, string?) {nameof(ValueTupleOfRefTypeAndNullableRefType)};")] public (string, string?) ValueTupleOfRefTypeAndNullableRefType;
  [MemberDeclarationTestCase($"public (string?, string?) {nameof(ValueTupleOfNullableRefTypeAndNullableRefType)};")] public (string?, string?) ValueTupleOfNullableRefTypeAndNullableRefType;
  [MemberDeclarationTestCase($"public (string?, string?)? {nameof(NullableValueTupleOfNullableRefTypeAndNullableRefType)};")] public (string?, string?)? NullableValueTupleOfNullableRefTypeAndNullableRefType;

  [MemberDeclarationTestCase($"public (string X, string Y) {nameof(ValueTupleOfRefTypeAndRefTypeWithElementNames)};")] public (string X, string Y) ValueTupleOfRefTypeAndRefTypeWithElementNames;
  [MemberDeclarationTestCase($"public (string? X, string? Y)? {nameof(NullableValueTupleOfNullableRefTypeAndNullableRefTypeWithElementNames)};")] public (string? X, string? Y)? NullableValueTupleOfNullableRefTypeAndNullableRefTypeWithElementNames;

  [MemberDeclarationTestCase($"public List<int> {nameof(ListOfValueType)};", MemberWithNamespace = false)] public List<int> ListOfValueType;
  [MemberDeclarationTestCase($"public List<int?> {nameof(ListOfNullableValueType)};", MemberWithNamespace = false)] public List<int?> ListOfNullableValueType;
  [MemberDeclarationTestCase($"public List<int>? {nameof(ListOfValueTypeNullable)};", MemberWithNamespace = false)] public List<int>? ListOfValueTypeNullable;
  [MemberDeclarationTestCase($"public List<int?>? {nameof(NullableListOfNullableValueType)};", MemberWithNamespace = false)] public List<int?>? NullableListOfNullableValueType;

  [MemberDeclarationTestCase($"public List<string> {nameof(ListOfRefType)};", MemberWithNamespace = false)] public List<string> ListOfRefType;
  [MemberDeclarationTestCase($"public List<string?> {nameof(ListOfNullableRefType)};", MemberWithNamespace = false)] public List<string?> ListOfNullableRefType;
  [MemberDeclarationTestCase($"public List<string>? {nameof(ListOfRefTypeNullable)};", MemberWithNamespace = false)] public List<string>? ListOfRefTypeNullable;
  [MemberDeclarationTestCase($"public List<string?>? {nameof(NullableListOfNullableRefType)};", MemberWithNamespace = false)] public List<string?>? NullableListOfNullableRefType;

  [MemberDeclarationTestCase($"public Dictionary<string, string> {nameof(DictionaryOfRefTypeValue)};", MemberWithNamespace = false)] public Dictionary<string, string> DictionaryOfRefTypeValue;
  [MemberDeclarationTestCase($"public Dictionary<string, string?> {nameof(DictionaryOfNullableRefTypeValue)};", MemberWithNamespace = false)] public Dictionary<string, string?> DictionaryOfNullableRefTypeValue;
  [MemberDeclarationTestCase($"public Dictionary<string, string>? {nameof(DictionaryOfRefTypeValueNullable)};", MemberWithNamespace = false)] public Dictionary<string, string>? DictionaryOfRefTypeValueNullable;
  [MemberDeclarationTestCase($"public Dictionary<string, string?>? {nameof(NullableDictionaryOfNullableRefTypeValue)};", MemberWithNamespace = false)] public Dictionary<string, string?>? NullableDictionaryOfNullableRefTypeValue;
}
#endif
