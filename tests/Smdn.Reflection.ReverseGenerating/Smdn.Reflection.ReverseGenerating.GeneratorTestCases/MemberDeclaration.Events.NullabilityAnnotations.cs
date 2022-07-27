// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT

#nullable enable annotations
#pragma warning disable CS8618, CS0067

using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Events;

public class NullabilityAnnotations {
  [MemberDeclarationTestCase($"public event System.EventHandler {nameof(EventHandler)};")] public event EventHandler EventHandler;
  [MemberDeclarationTestCase($"public event System.EventHandler? {nameof(NullableEventHandler)};")] public event EventHandler? NullableEventHandler;

  [MemberDeclarationTestCase($"public event System.ComponentModel.PropertyChangedEventHandler {nameof(PropertyChangedEventHandler)};")] public event PropertyChangedEventHandler PropertyChangedEventHandler;
  [MemberDeclarationTestCase($"public event System.ComponentModel.PropertyChangedEventHandler? {nameof(NullablePropertyChangedEventHandler)};")] public event PropertyChangedEventHandler? NullablePropertyChangedEventHandler;

  [MemberDeclarationTestCase($"public event System.EventHandler<int> {nameof(ValTypeArgEventHandler)};")] public event EventHandler<int> ValTypeArgEventHandler;
  [MemberDeclarationTestCase($"public event System.EventHandler<int>? {nameof(ValTypeArgNullableEventHandler)};")] public event EventHandler<int>? ValTypeArgNullableEventHandler;
  [MemberDeclarationTestCase($"public event System.EventHandler<int?> {nameof(NullableValTypeArgEventHandler)};")] public event EventHandler<int?> NullableValTypeArgEventHandler;
  [MemberDeclarationTestCase($"public event System.EventHandler<int?>? {nameof(NullableValTypeArgNullableEventHandler)};")] public event EventHandler<int?>? NullableValTypeArgNullableEventHandler;

  [MemberDeclarationTestCase($"public event System.EventHandler<string> {nameof(RefTypeArgEventHandler)};")] public event EventHandler<string> RefTypeArgEventHandler;
  [MemberDeclarationTestCase($"public event System.EventHandler<string>? {nameof(RefTypeArgNullableEventHandler)};")] public event EventHandler<string>? RefTypeArgNullableEventHandler;
  [MemberDeclarationTestCase($"public event System.EventHandler<string?> {nameof(NullableRefTypeArgEventHandler)};")] public event EventHandler<string?> NullableRefTypeArgEventHandler;
  [MemberDeclarationTestCase($"public event System.EventHandler<string?>? {nameof(NullableRefTypeArgNullableEventHandler)};")] public event EventHandler<string?>? NullableRefTypeArgNullableEventHandler;

  [MemberDeclarationTestCase($"public event System.EventHandler<(string, string)> {nameof(ValueTupleOfRefTypeArgEventHandler)};")] public event EventHandler<(string, string)> ValueTupleOfRefTypeArgEventHandler;
  [MemberDeclarationTestCase($"public event System.EventHandler<(string, string?)> {nameof(ValueTupleOfNullableRefTypeArgEventHandler)};")] public event EventHandler<(string, string?)> ValueTupleOfNullableRefTypeArgEventHandler;
  [MemberDeclarationTestCase($"public event System.EventHandler<(string, string?)>? {nameof(ValueTupleOfNullableRefTypeArgNullableEventHandler)};")] public event EventHandler<(string, string?)>? ValueTupleOfNullableRefTypeArgNullableEventHandler;
  [MemberDeclarationTestCase($"public event System.EventHandler<(string, string)?> {nameof(NullableValueTupleOfRefTypeArgEventHandler)};")] public event EventHandler<(string, string)?> NullableValueTupleOfRefTypeArgEventHandler;
  [MemberDeclarationTestCase($"public event System.EventHandler<(string, string)?>? {nameof(NullableValueTupleOfRefTypeArgNullableEventHandler)};")] public event EventHandler<(string, string)?>? NullableValueTupleOfRefTypeArgNullableEventHandler;
  [MemberDeclarationTestCase($"public event System.EventHandler<(string, string?)?> {nameof(NullableValueTupleOfNullableRefTypeArgEventHandler)};")] public event EventHandler<(string, string?)?> NullableValueTupleOfNullableRefTypeArgEventHandler;
  [MemberDeclarationTestCase($"public event System.EventHandler<(string, string?)?>? {nameof(NullableValueTupleOfNullableRefTypeArgNullableEventHandler)};")] public event EventHandler<(string, string?)?>? NullableValueTupleOfNullableRefTypeArgNullableEventHandler;

  [MemberDeclarationTestCase($"public event EventHandler<List<string>> {nameof(ListOfRefTypeArgEventHandler)};", MemberWithNamespace = false)] public event EventHandler<List<string>> ListOfRefTypeArgEventHandler;
  [MemberDeclarationTestCase($"public event EventHandler<List<string?>> {nameof(ListOfNullableRefTypeArgEventHandler)};", MemberWithNamespace = false)] public event EventHandler<List<string?>> ListOfNullableRefTypeArgEventHandler;
  [MemberDeclarationTestCase($"public event EventHandler<List<string?>>? {nameof(ListOfNullableRefTypeArgNullableEventHandler)};", MemberWithNamespace = false)] public event EventHandler<List<string?>>? ListOfNullableRefTypeArgNullableEventHandler;
  [MemberDeclarationTestCase($"public event EventHandler<List<string>?> {nameof(NullableListOfRefTypeArgEventHandler)};", MemberWithNamespace = false)] public event EventHandler<List<string>?> NullableListOfRefTypeArgEventHandler;
  [MemberDeclarationTestCase($"public event EventHandler<List<string>?>? {nameof(NullableListOfRefTypeArgNullableEventHandler)};", MemberWithNamespace = false)] public event EventHandler<List<string>?>? NullableListOfRefTypeArgNullableEventHandler;
  [MemberDeclarationTestCase($"public event EventHandler<List<string?>?> {nameof(NullableListOfNullableRefTypeArgEventHandler)};", MemberWithNamespace = false)] public event EventHandler<List<string?>?> NullableListOfNullableRefTypeArgEventHandler;
  [MemberDeclarationTestCase($"public event EventHandler<List<string?>?>? {nameof(NullableListOfNullableRefTypeArgNullableEventHandler)};", MemberWithNamespace = false)] public event EventHandler<List<string?>?>? NullableListOfNullableRefTypeArgNullableEventHandler;

  class NullabilityAnnotationOptions {
    [MemberDeclarationTestCase($"public event System.EventHandler {nameof(EventHandler)};", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public event System.EventHandler {nameof(EventHandler)};", MemberEnableNullabilityAnnotations = true)]
    public event EventHandler EventHandler;

    [MemberDeclarationTestCase($"public event System.EventHandler {nameof(NullableEventHandler)};", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public event System.EventHandler? {nameof(NullableEventHandler)};", MemberEnableNullabilityAnnotations = true)]
    public event EventHandler? NullableEventHandler;

    [MemberDeclarationTestCase($"public event System.EventHandler<int> {nameof(ValTypeArgNullableEventHandler)};", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public event System.EventHandler<int>? {nameof(ValTypeArgNullableEventHandler)};", MemberEnableNullabilityAnnotations = true)]
    public event EventHandler<int>? ValTypeArgNullableEventHandler;

    [MemberDeclarationTestCase($"public event System.EventHandler<int?> {nameof(NullableValTypeArgEventHandler)};", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public event System.EventHandler<int?> {nameof(NullableValTypeArgEventHandler)};", MemberEnableNullabilityAnnotations = true)]
    public event EventHandler<int?> NullableValTypeArgEventHandler;

    [MemberDeclarationTestCase($"public event System.EventHandler<int?> {nameof(NullableValTypeArgNullableEventHandler)};", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public event System.EventHandler<int?>? {nameof(NullableValTypeArgNullableEventHandler)};", MemberEnableNullabilityAnnotations = true)]
    public event EventHandler<int?>? NullableValTypeArgNullableEventHandler;

    [MemberDeclarationTestCase($"public event System.EventHandler<string> {nameof(RefTypeArgNullableEventHandler)};", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public event System.EventHandler<string>? {nameof(RefTypeArgNullableEventHandler)};", MemberEnableNullabilityAnnotations = true)]
    public event EventHandler<string>? RefTypeArgNullableEventHandler;

    [MemberDeclarationTestCase($"public event System.EventHandler<string> {nameof(NullableRefTypeArgEventHandler)};", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public event System.EventHandler<string?> {nameof(NullableRefTypeArgEventHandler)};", MemberEnableNullabilityAnnotations = true)]
    public event EventHandler<string?> NullableRefTypeArgEventHandler;

    [MemberDeclarationTestCase($"public event System.EventHandler<string> {nameof(NullableRefTypeArgNullableEventHandler)};", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public event System.EventHandler<string?>? {nameof(NullableRefTypeArgNullableEventHandler)};", MemberEnableNullabilityAnnotations = true)]
    public event EventHandler<string?>? NullableRefTypeArgNullableEventHandler;
  }
}
#endif
