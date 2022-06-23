// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#nullable enable annotations

using System;
using NUnit.Framework;

using Smdn.Reflection.ReverseGenerating;

namespace Smdn.Reflection.ReverseGenerating.ListApi.AttributeFilterTestCases.NullableAttributes;

[TypeAttributeFilterTestCaseAttribute("[Nullable(byte.MinValue)], [NullableContext(1)]")]
[TypeAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
public class NullableGenericParam<T> where T : class {
  [MemberAttributeFilterTestCaseAttribute("")]
  [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  [return: MemberAttributeFilterTestCaseAttribute("")]
  [return: MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  public T NotAnnotated(
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    T arg
  ) => throw null;

  [MemberAttributeFilterTestCaseAttribute("[NullableContext(2)]")]
  [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  [return: MemberAttributeFilterTestCaseAttribute("")]
  [return: MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  public T? Annotated(
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    T? arg
  ) => throw null;
}

[TypeAttributeFilterTestCaseAttribute("[Nullable(byte.MinValue)], [NullableContext(1)]")]
[TypeAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
public class NotNullGenericParam<T> where T : notnull {
  [MemberAttributeFilterTestCaseAttribute("")]
  [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  [return: MemberAttributeFilterTestCaseAttribute("")]
  [return: MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  public T NotAnnotated(
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    T arg
  ) => throw null;

  [MemberAttributeFilterTestCaseAttribute("[NullableContext(2)]")]
  [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  [return: MemberAttributeFilterTestCaseAttribute("")]
  [return: MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  public T? Annotated(
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    T? arg
  ) => throw null;
}

[TypeAttributeFilterTestCaseAttribute("[Extension]")]
[TypeAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
public static class ExtensionClass {
  [MemberAttributeFilterTestCaseAttribute("[Extension], [NullableContext(1)]")]
  [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  public static void ExtensionMethod(this string x) => throw null;
}

[TypeAttributeFilterTestCaseAttribute("")]
[TypeAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
public class Members {
  [TypeAttributeFilterTestCaseAttribute("")]
  [TypeAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  public class Fields {
    [MemberAttributeFilterTestCaseAttribute("[Nullable(1)]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public string RefTypeField;

    [MemberAttributeFilterTestCaseAttribute("[Nullable(2)]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public string? NullableRefTypeField;
  }

  [TypeAttributeFilterTestCaseAttribute("[Nullable(byte.MinValue)], [NullableContext(1)]")]
  [TypeAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  public class Properties {
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public string RefTypeProperty => throw null;

    [MemberAttributeFilterTestCaseAttribute("[Nullable(2)]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public string? NullableRefTypeProperty => throw null;
  }

  [TypeAttributeFilterTestCaseAttribute("")]
  [TypeAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  public class Events {
    [MemberAttributeFilterTestCaseAttribute("[Nullable(1)]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public EventHandler Event;

    [MemberAttributeFilterTestCaseAttribute("[Nullable(2)]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public EventHandler? NullableEvent;
  }

  [TypeAttributeFilterTestCaseAttribute("[Nullable(byte.MinValue)], [NullableContext(1)]")]
  [TypeAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  public class Methods {
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public void Parameters(
      [MemberAttributeFilterTestCaseAttribute("")]
      [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
      string refType,
      [MemberAttributeFilterTestCaseAttribute("[Nullable(2)]")]
      [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
      string? nullableRefType
    ) => throw null;

    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    [return: MemberAttributeFilterTestCaseAttribute("")]
    [return: MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public string RefTypeReturnParameter() => throw null;

    [MemberAttributeFilterTestCaseAttribute("[NullableContext(2)]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    [return: MemberAttributeFilterTestCaseAttribute("")]
    [return: MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public string? NullableRefTypeReturnParameter() => throw null;

    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public string RefTypeParameterAndRefTypeReturnParameter(
      [MemberAttributeFilterTestCaseAttribute("")]
      [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
      string refType
    ) => throw null;

    [MemberAttributeFilterTestCaseAttribute("[NullableContext(2)]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public string? NullableRefTypeParameterAndNullableRefTypeReturnParameter(
      [MemberAttributeFilterTestCaseAttribute("")]
      [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
      string? refType
    ) => throw null;
  }
}
