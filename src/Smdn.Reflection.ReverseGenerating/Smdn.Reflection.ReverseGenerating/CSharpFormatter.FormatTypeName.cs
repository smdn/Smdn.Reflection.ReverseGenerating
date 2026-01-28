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
    => CSharpTypeNameFormatter.Format(
      type: t,
      options: new(
        AttributeProvider: attributeProvider ?? t,
        WithNamespace: typeWithNamespace,
        WithDeclaringTypeName: withDeclaringTypeName,
        TranslateLanguagePrimitiveType: translateLanguagePrimitiveType
      )
    );

  public static string FormatUnboundTypeName(
    this Type t,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => CSharpTypeNameFormatter.Format(
      type: t ?? throw new ArgumentNullException(nameof(t)),
      options: new(
        AttributeProvider: t,
        WithNamespace: typeWithNamespace,
        WithDeclaringTypeName: withDeclaringTypeName,
        TranslateLanguagePrimitiveType: translateLanguagePrimitiveType,
        AsUnboundTypeName: t.IsGenericTypeDefinition
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
    => FormatTypeName(
      f: f ?? throw new ArgumentNullException(nameof(f)),
      nullabilityInfoContext: nullabilityInfoContext,
      nullabilityInfoContextLockObject: null,
      typeWithNamespace: typeWithNamespace,
      withDeclaringTypeName: withDeclaringTypeName,
      translateLanguagePrimitiveType: translateLanguagePrimitiveType
    );

  public static string FormatTypeName(
    this FieldInfo f,
    NullabilityInfoContext? nullabilityInfoContext,
    object? nullabilityInfoContextLockObject,
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
      : CSharpTypeNameFormatter.Format(
        target: nullabilityInfoContext.Create(f ?? throw new ArgumentNullException(nameof(f)), nullabilityInfoContextLockObject),
        options: new(
          AttributeProvider: f,
          WithNamespace: typeWithNamespace,
          WithDeclaringTypeName: withDeclaringTypeName,
          NullabilityInfoContext: nullabilityInfoContext,
          TranslateLanguagePrimitiveType: translateLanguagePrimitiveType
        )
      ).ToString();
#endif

  public static string FormatTypeName(
    this FieldInfo f,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => CSharpTypeNameFormatter.Format(
      type: (f ?? throw new ArgumentNullException(nameof(f))).FieldType,
      options: new(
        AttributeProvider: f,
        WithNamespace: typeWithNamespace,
        WithDeclaringTypeName: withDeclaringTypeName,
        TranslateLanguagePrimitiveType: translateLanguagePrimitiveType
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
    => FormatTypeName(
      p: p ?? throw new ArgumentNullException(nameof(p)),
      nullabilityInfoContext: nullabilityInfoContext,
      nullabilityInfoContextLockObject: null,
      typeWithNamespace: typeWithNamespace,
      withDeclaringTypeName: withDeclaringTypeName,
      translateLanguagePrimitiveType: translateLanguagePrimitiveType
    );

  public static string FormatTypeName(
    this PropertyInfo p,
    NullabilityInfoContext? nullabilityInfoContext,
    object? nullabilityInfoContextLockObject,
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
      : CSharpTypeNameFormatter.Format(
        target: nullabilityInfoContext.Create(p ?? throw new ArgumentNullException(nameof(p)), nullabilityInfoContextLockObject),
        options: new(
          AttributeProvider: p,
          WithNamespace: typeWithNamespace,
          WithDeclaringTypeName: withDeclaringTypeName,
          NullabilityInfoContext: nullabilityInfoContext,
          TranslateLanguagePrimitiveType: translateLanguagePrimitiveType
        )
      ).ToString();
#endif

  public static string FormatTypeName(
    this PropertyInfo p,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => CSharpTypeNameFormatter.Format(
      type: (p ?? throw new ArgumentNullException(nameof(p))).PropertyType,
      options: new(
        AttributeProvider: p,
        WithNamespace: typeWithNamespace,
        WithDeclaringTypeName: withDeclaringTypeName,
        TranslateLanguagePrimitiveType: translateLanguagePrimitiveType
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
    => FormatTypeName(
      p: p ?? throw new ArgumentNullException(nameof(p)),
      nullabilityInfoContext: nullabilityInfoContext,
      nullabilityInfoContextLockObject: null,
      typeWithNamespace: typeWithNamespace,
      withDeclaringTypeName: withDeclaringTypeName,
      translateLanguagePrimitiveType: translateLanguagePrimitiveType
    );

  public static string FormatTypeName(
    this ParameterInfo p,
    NullabilityInfoContext? nullabilityInfoContext,
    object? nullabilityInfoContextLockObject,
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
      : CSharpTypeNameFormatter.Format(
        target: nullabilityInfoContext.Create(p ?? throw new ArgumentNullException(nameof(p)), nullabilityInfoContextLockObject),
        options: new(
          AttributeProvider: p,
          WithNamespace: typeWithNamespace,
          WithDeclaringTypeName: withDeclaringTypeName,
          NullabilityInfoContext: nullabilityInfoContext,
          TranslateLanguagePrimitiveType: translateLanguagePrimitiveType
        )
      ).ToString();
#endif

  public static string FormatTypeName(
    this ParameterInfo p,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => CSharpTypeNameFormatter.Format(
      type: (p ?? throw new ArgumentNullException(nameof(p))).ParameterType,
      options: new(
        AttributeProvider: p,
        WithNamespace: typeWithNamespace,
        WithDeclaringTypeName: withDeclaringTypeName,
        TranslateLanguagePrimitiveType: translateLanguagePrimitiveType
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
    => FormatTypeName(
      ev: ev,
      nullabilityInfoContext: nullabilityInfoContext,
      nullabilityInfoContextLockObject: null,
      typeWithNamespace: typeWithNamespace,
      withDeclaringTypeName: withDeclaringTypeName,
      translateLanguagePrimitiveType: translateLanguagePrimitiveType
    );

  public static string FormatTypeName(
    this EventInfo ev,
    NullabilityInfoContext? nullabilityInfoContext,
    object? nullabilityInfoContextLockObject,
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
      : CSharpTypeNameFormatter.Format(
        target: nullabilityInfoContext.Create(ev ?? throw new ArgumentNullException(nameof(ev)), nullabilityInfoContextLockObject),
        options: new(
          AttributeProvider: ev,
          WithNamespace: typeWithNamespace,
          WithDeclaringTypeName: withDeclaringTypeName,
          NullabilityInfoContext: nullabilityInfoContext,
          TranslateLanguagePrimitiveType: translateLanguagePrimitiveType
        )
      ).ToString();
#endif

  public static string FormatTypeName(
    this EventInfo ev,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => CSharpTypeNameFormatter.Format(
      type: (ev ?? throw new ArgumentNullException(nameof(ev))).GetEventHandlerTypeOrThrow(),
      options: new(
        AttributeProvider: ev,
        WithNamespace: typeWithNamespace,
        WithDeclaringTypeName: withDeclaringTypeName,
        TranslateLanguagePrimitiveType: translateLanguagePrimitiveType
      )
    );
}
