// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection.ReverseGenerating;

#pragma warning disable IDE0040
static partial class CSharpFormatter {
#pragma warning restore IDE0040
  internal readonly record struct FormatTypeNameOptions(
#pragma warning disable SA1313
    ICustomAttributeProvider AttributeProvider,
    bool TypeWithNamespace,
    bool WithDeclaringTypeName,
    bool TranslateLanguagePrimitiveType,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    NullabilityInfoContext? NullabilityInfoContext = null,
#endif
    Func<Type, string, string>? GenericParameterNameModifier = null,
    bool OmitAttributeSuffix = false
#pragma warning restore SA1313
  );

  public static string FormatTypeName(
    this Type t,
    ICustomAttributeProvider? attributeProvider = null,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => FormatTypeNameCore(
      t,
      options: new(
        AttributeProvider: attributeProvider ?? t,
        TypeWithNamespace: typeWithNamespace,
        WithDeclaringTypeName: withDeclaringTypeName,
        TranslateLanguagePrimitiveType: translateLanguagePrimitiveType
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
      : FormatTypeNameWithNullabilityAnnotation(
        target: nullabilityInfoContext.Create(f ?? throw new ArgumentNullException(nameof(f)), nullabilityInfoContextLockObject),
        builder: new(capacity: 32),
        options: new(
          AttributeProvider: f,
          TypeWithNamespace: typeWithNamespace,
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
    => FormatTypeNameCore(
      (f ?? throw new ArgumentNullException(nameof(f))).FieldType,
      options: new(
        AttributeProvider: f,
        TypeWithNamespace: typeWithNamespace,
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
      : FormatTypeNameWithNullabilityAnnotation(
        target: nullabilityInfoContext.Create(p ?? throw new ArgumentNullException(nameof(p)), nullabilityInfoContextLockObject),
        builder: new(capacity: 32),
        options: new(
          AttributeProvider: p,
          TypeWithNamespace: typeWithNamespace,
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
    => FormatTypeNameCore(
      (p ?? throw new ArgumentNullException(nameof(p))).PropertyType,
      options: new(
        AttributeProvider: p,
        TypeWithNamespace: typeWithNamespace,
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
      : FormatTypeNameWithNullabilityAnnotation(
        target: nullabilityInfoContext.Create(p ?? throw new ArgumentNullException(nameof(p)), nullabilityInfoContextLockObject),
        builder: new(capacity: 32),
        options: new(
          AttributeProvider: p,
          TypeWithNamespace: typeWithNamespace,
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
    => FormatTypeNameCore(
      (p ?? throw new ArgumentNullException(nameof(p))).ParameterType,
      options: new(
        AttributeProvider: p,
        TypeWithNamespace: typeWithNamespace,
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
      : FormatTypeNameWithNullabilityAnnotation(
        target: nullabilityInfoContext.Create(ev ?? throw new ArgumentNullException(nameof(ev)), nullabilityInfoContextLockObject),
        builder: new(capacity: 32),
        options: new(
          AttributeProvider: ev,
          TypeWithNamespace: typeWithNamespace,
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
    => FormatTypeNameCore(
      (ev ?? throw new ArgumentNullException(nameof(ev))).GetEventHandlerTypeOrThrow(),
      options: new(
        AttributeProvider: ev,
        TypeWithNamespace: typeWithNamespace,
        WithDeclaringTypeName: withDeclaringTypeName,
        TranslateLanguagePrimitiveType: translateLanguagePrimitiveType
      )
    );

  private static string GetTypeName(Type t, FormatTypeNameOptions options)
  {
    var typeName = t.IsGenericType
      ? t.GetGenericTypeName()
      : t.Name;

    if (
      options.OmitAttributeSuffix &&
      typeName.EndsWith("Attribute", StringComparison.Ordinal) &&
      (typeof(Attribute).IsAssignableFrom(t) || IsAttributeType(t)) &&
      t != typeof(Attribute)
    ) {
      const int LengthOfAttributeSuffix = 9; // "Attribute".Length

      typeName = typeName.Substring(0, typeName.Length - LengthOfAttributeSuffix);
    }

    return typeName;

    // alternative method to Type.IsAssignableTo(typeof(Attribute)) for the
    // reflection-only context types
    static bool IsAttributeType(Type maybeReflectionOnlyType)
    {
      var t = maybeReflectionOnlyType.BaseType;

      for (; ; ) {
        if (t is null)
          return false;
        if (string.Equals(t.FullName, "System.Attribute", StringComparison.Ordinal))
          return true;

        t = t.BaseType;
      }
    }
  }

  private static string? GetByRefParameterModifier(ParameterInfo parameter)
  {
    static bool HasRequiresLocationAttribute(ParameterInfo p)
      => p
        .GetCustomAttributesData()
        .Any(static d => "System.Runtime.CompilerServices.RequiresLocationAttribute".Equals(d.AttributeType.FullName, StringComparison.Ordinal));

    static bool HasIsReadOnlyAttribute(ParameterInfo p)
      => p
        .GetCustomAttributesData()
        .Any(static d => "System.Runtime.CompilerServices.IsReadOnlyAttribute".Equals(d.AttributeType.FullName, StringComparison.Ordinal));

    var isScoped = parameter
      .GetCustomAttributesData()
      .Any(static d => "System.Runtime.CompilerServices.ScopedRefAttribute".Equals(d.AttributeType.FullName, StringComparison.Ordinal));

    if (parameter.IsIn) {
      var isRefReadOnly = HasRequiresLocationAttribute(parameter);

      if (isScoped)
        return isRefReadOnly ? "scoped ref readonly " : "scoped in ";
      else
        return isRefReadOnly ? "ref readonly " : "in ";
    }
    else if (parameter.IsOut) {
      return isScoped ? "scoped out " : "out ";
    }
    else {
      var isRefReadOnly = HasIsReadOnlyAttribute(parameter);

      if (isScoped)
        return isRefReadOnly ? "scoped ref readonly " : "scoped ref ";
      else
        return isRefReadOnly ? "ref readonly " : "ref ";
    }
  }
}
