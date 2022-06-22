// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;

namespace Smdn.Reflection.ReverseGenerating;

#pragma warning disable IDE0040
static partial class CSharpFormatter {
#pragma warning restore IDE0040
  public static string FormatTypeName(
    this Type t,
    ICustomAttributeProvider? attributeProvider = null,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => FormatTypeNameCore(
      t,
      showVariance: false,
      options: new FormatTypeNameOptions(
        attributeProvider: attributeProvider ?? t,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      )
    );

#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
  public static string FormatTypeName(
    this FieldInfo f,
    NullabilityInfoContext? nullabilityInfoContext,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => nullabilityInfoContext is null
      ? FormatTypeName(
        f: f,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      )
      : FormatTypeNameWithNullabilityAnnotation(
        nullabilityInfoContext.Create(f ?? throw new ArgumentNullException(nameof(f))),
        builder: new(capacity: 32),
        options: new(
          attributeProvider: f,
          typeWithNamespace: typeWithNamespace,
          withDeclaringTypeName: withDeclaringTypeName,
          translateLanguagePrimitiveType: translateLanguagePrimitiveType
        )
      ).ToString();
#endif

  public static string FormatTypeName(
    this FieldInfo f,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => FormatTypeNameCore(
      (f ?? throw new ArgumentNullException(nameof(f))).FieldType,
      showVariance: false,
      options: new(
        attributeProvider: f,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      )
    );

#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
  public static string FormatTypeName(
    this PropertyInfo p,
    NullabilityInfoContext? nullabilityInfoContext,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => nullabilityInfoContext is null
      ? FormatTypeName(
        p: p,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      )
      : FormatTypeNameWithNullabilityAnnotation(
        nullabilityInfoContext.Create(p ?? throw new ArgumentNullException(nameof(p))),
        builder: new(capacity: 32),
        options: new(
          attributeProvider: p,
          typeWithNamespace: typeWithNamespace,
          withDeclaringTypeName: withDeclaringTypeName,
          translateLanguagePrimitiveType: translateLanguagePrimitiveType
        )
      ).ToString();
#endif

  public static string FormatTypeName(
    this PropertyInfo p,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => FormatTypeNameCore(
      (p ?? throw new ArgumentNullException(nameof(p))).PropertyType,
      showVariance: false,
      options: new(
        attributeProvider: p,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      )
    );

#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
  public static string FormatTypeName(
    this ParameterInfo p,
    NullabilityInfoContext? nullabilityInfoContext,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => nullabilityInfoContext is null
      ? FormatTypeName(
        p: p,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      )
      : FormatTypeNameWithNullabilityAnnotation(
        nullabilityInfoContext.Create(p ?? throw new ArgumentNullException(nameof(p))),
        builder: new(capacity: 32),
        options: new(
          attributeProvider: p,
          typeWithNamespace: typeWithNamespace,
          withDeclaringTypeName: withDeclaringTypeName,
          translateLanguagePrimitiveType: translateLanguagePrimitiveType
        )
      ).ToString();
#endif

  public static string FormatTypeName(
    this ParameterInfo p,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => FormatTypeNameCore(
      (p ?? throw new ArgumentNullException(nameof(p))).ParameterType,
      showVariance: false,
      options: new(
        attributeProvider: p,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      )
    );

#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
  public static string FormatTypeName(
    this EventInfo ev,
    NullabilityInfoContext? nullabilityInfoContext,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => nullabilityInfoContext is null
      ? FormatTypeName(
        ev: ev,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      )
      : FormatTypeNameWithNullabilityAnnotation(
        nullabilityInfoContext.Create(ev ?? throw new ArgumentNullException(nameof(ev))),
        builder: new(capacity: 32),
        options: new(
          attributeProvider: ev,
          typeWithNamespace: typeWithNamespace,
          withDeclaringTypeName: withDeclaringTypeName,
          translateLanguagePrimitiveType: translateLanguagePrimitiveType
        )
      ).ToString();
#endif

  public static string FormatTypeName(
    this EventInfo ev,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => FormatTypeNameCore(
      (ev ?? throw new ArgumentNullException(nameof(ev))).GetEventHandlerTypeOrThrow(),
      showVariance: false,
      options: new(
        attributeProvider: ev,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      )
    );
}
