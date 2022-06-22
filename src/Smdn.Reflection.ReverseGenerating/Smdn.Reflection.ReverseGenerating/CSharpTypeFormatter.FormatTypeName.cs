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

  public static string FormatTypeName(
    this FieldInfo f,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    NullabilityInfoContext context,
#endif
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    =>
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      context is not null
        ? FormatTypeNameWithNullabilityAnnotation(
            context.Create(f ?? throw new ArgumentNullException(nameof(f))),
            builder: new(capacity: 32),
            options: new FormatTypeNameOptions(
              attributeProvider: f,
              typeWithNamespace: typeWithNamespace,
              withDeclaringTypeName: withDeclaringTypeName,
              translateLanguagePrimitiveType: translateLanguagePrimitiveType
            )
          ).ToString()
        :
#endif
      FormatTypeName(
        (f ?? throw new ArgumentNullException(nameof(f))).FieldType,
        attributeProvider: f,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      );

  public static string FormatTypeName(
    this PropertyInfo p,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    NullabilityInfoContext context,
#endif
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    =>
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      context is not null
        ? FormatTypeNameWithNullabilityAnnotation(
            context.Create(p ?? throw new ArgumentNullException(nameof(p))),
            builder: new(capacity: 32),
            options: new FormatTypeNameOptions(
              attributeProvider: p,
              typeWithNamespace: typeWithNamespace,
              withDeclaringTypeName: withDeclaringTypeName,
              translateLanguagePrimitiveType: translateLanguagePrimitiveType
            )
          ).ToString()
        :
#endif
      FormatTypeName(
        (p ?? throw new ArgumentNullException(nameof(p))).PropertyType,
        attributeProvider: p,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      );

  public static string FormatTypeName(
    this ParameterInfo p,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    NullabilityInfoContext context,
#endif
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    =>
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      context is not null
        ? FormatTypeNameWithNullabilityAnnotation(
            context.Create(p ?? throw new ArgumentNullException(nameof(p))),
            builder: new(capacity: 32),
            options: new FormatTypeNameOptions(
              attributeProvider: p,
              typeWithNamespace: typeWithNamespace,
              withDeclaringTypeName: withDeclaringTypeName,
              translateLanguagePrimitiveType: translateLanguagePrimitiveType
            )
          ).ToString()
        :
#endif
      FormatTypeName(
        (p ?? throw new ArgumentNullException(nameof(p))).ParameterType,
        attributeProvider: p,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      );

  public static string FormatTypeName(
    this EventInfo ev,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    NullabilityInfoContext context,
#endif
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    =>
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      context is not null
        ? FormatTypeNameWithNullabilityAnnotation(
            context.Create(ev ?? throw new ArgumentNullException(nameof(ev))),
            builder: new(capacity: 32),
            options: new FormatTypeNameOptions(
              attributeProvider: ev,
              typeWithNamespace: typeWithNamespace,
              withDeclaringTypeName: withDeclaringTypeName,
              translateLanguagePrimitiveType: translateLanguagePrimitiveType
            )
          ).ToString()
        :
#endif
      FormatTypeName(
        (ev ?? throw new ArgumentNullException(nameof(ev))).GetEventHandlerTypeOrThrow(),
        attributeProvider: ev,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      );
}
