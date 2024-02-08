// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if SYSTEM_REFLECTION_NULLABILITYINFO
using System;
using System.Collections.Generic;
#if WORKAROUND_NULLABILITYINFO_BYREFTYPE
using System.Globalization;
#endif
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Smdn.Reflection.Attributes;

namespace Smdn.Reflection.ReverseGenerating;

#pragma warning disable IDE0040
static partial class CSharpFormatter {
#pragma warning restore IDE0040

#if WORKAROUND_NULLABILITYINFO_BYREFTYPE
  // Workaround: The pseudo ParameterInfo type which unwraps 'ByRef' type to its element type
  // See https://github.com/dotnet/runtime/issues/72320
  private sealed class ByRefElementTypeParameterInfo : ParameterInfo {
    public ParameterInfo BaseParameter { get; }

    public ByRefElementTypeParameterInfo(ParameterInfo baseParam)
    {
      BaseParameter = baseParam;
    }

    public override MemberInfo Member => BaseParameter.Member;
    public override Type ParameterType => BaseParameter.ParameterType.GetElementType()!;
    public override IList<CustomAttributeData> GetCustomAttributesData()
      => BaseParameter.GetCustomAttributesData();
  }

  private sealed class ByRefElementTypePropertyInfo : PropertyInfo {
    public PropertyInfo BaseProperty { get; }

    public ByRefElementTypePropertyInfo(PropertyInfo baseProperty)
    {
      BaseProperty = baseProperty;
    }

    public override string Name => BaseProperty.Name;
    public override PropertyAttributes Attributes => BaseProperty.Attributes;
    public override bool CanRead => BaseProperty.CanRead;
    public override bool CanWrite => BaseProperty.CanWrite;
    public override Type PropertyType => BaseProperty.PropertyType.GetElementType()!;
    public override Type? DeclaringType => BaseProperty.DeclaringType;
    public override Type? ReflectedType => BaseProperty.ReflectedType;
    public override IList<CustomAttributeData> GetCustomAttributesData() => BaseProperty.GetCustomAttributesData();
    public override object[] GetCustomAttributes(bool inherit) => BaseProperty.GetCustomAttributes(inherit);
    public override object[] GetCustomAttributes(Type attributeType, bool inherit) => BaseProperty.GetCustomAttributes(attributeType, inherit);
    public override bool IsDefined(Type attributeType, bool inherit) => BaseProperty.IsDefined(attributeType, inherit);
    public override MethodInfo[] GetAccessors(bool nonPublic) => BaseProperty.GetAccessors(nonPublic);
    public override MethodInfo? GetGetMethod(bool nonPublic) => BaseProperty.GetGetMethod(nonPublic);
    public override MethodInfo? GetSetMethod(bool nonPublic) => BaseProperty.GetSetMethod(nonPublic);
    public override ParameterInfo[] GetIndexParameters() => BaseProperty.GetIndexParameters();
    public override object? GetValue(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? index, CultureInfo? culture)
      => BaseProperty.GetValue(obj, invokeAttr, binder, index, culture);
    public override void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, object?[]? index, CultureInfo? culture)
      => BaseProperty.SetValue(obj, value, invokeAttr, binder, index, culture);
  }
#endif

#if DEBUG
  private static void ThrowIfTypeIsCompoundType(Type type, string name)
  {
    if (type.IsArray || type.IsPointer || type.IsByRef)
      throw new InvalidOperationException($"'{name}' is expected to be non-compound elemental type, but was compound: {type}");
  }
#endif

#pragma warning disable CA1502 // TODO: reduce code complexity
  private static StringBuilder FormatTypeNameWithNullabilityAnnotation(
    NullabilityInfo target,
    StringBuilder builder,
    FormatTypeNameOptions options
  )
  {
    const string NullableAnnotationSyntaxString = "?";

    static string GetNullabilityAnnotation(NullabilityInfo target)
      => target.ReadState == NullabilityState.Nullable || target.WriteState == NullabilityState.Nullable
        ? NullableAnnotationSyntaxString
        : string.Empty;

    if (target.Type.IsByRef) {
#if WORKAROUND_NULLABILITYINFO_BYREFTYPE
      var elementTypeNullabilityInfo = target.ElementType;
#endif

      switch (options.AttributeProvider) {
        case ParameterInfo para:
          // retval/parameter modifiers
          if (para.IsIn)
            builder.Append("in ");
          else if (para.IsOut)
            builder.Append("out ");
          else /*if (para.IsRetval)*/
            builder.Append("ref ");

#if WORKAROUND_NULLABILITYINFO_BYREFTYPE
          // [.net6.0] Currently, NullabilityInfo.ElementType is always null if the type is ByRef.
          // Uses the workaround implementation instead in that case.
          // See https://github.com/dotnet/runtime/issues/72320
          if (options.NullabilityInfoContext is not null && target.ElementType is null && para.ParameterType.HasElementType)
            elementTypeNullabilityInfo = options.NullabilityInfoContext.Create(new ByRefElementTypeParameterInfo(para));
#endif
          break;

        case PropertyInfo p:
          builder.Append("ref ");

#if WORKAROUND_NULLABILITYINFO_BYREFTYPE
          if (options.NullabilityInfoContext is not null && target.ElementType is null && p.PropertyType.HasElementType)
            elementTypeNullabilityInfo = options.NullabilityInfoContext.Create(new ByRefElementTypePropertyInfo(p));
#endif
          break;
      }

#if WORKAROUND_NULLABILITYINFO_BYREFTYPE
      if (elementTypeNullabilityInfo is not null) {
        return FormatTypeNameWithNullabilityAnnotation(
          elementTypeNullabilityInfo,
          builder,
          options
        );
      }
#endif
    }

    if (target.Type.IsArray) {
      // arrays
      return FormatTypeNameWithNullabilityAnnotation(target.ElementType!, builder, options)
        .Append('[')
        .Append(',', target.Type.GetArrayRank() - 1)
        .Append(']')
        .Append(GetNullabilityAnnotation(target));
    }

    var type = target.Type;

#if WORKAROUND_NULLABILITYINFO_BYREFTYPE
    if (type.IsPointer || type.IsByRef)
#else
    if (type.IsByRef)
      type = type.GetElementType()!;

    if (type.IsPointer)
#endif
      // pointer types or by-ref types (exclude ParameterInfo and PropertyInfo)
      return builder.Append(FormatTypeNameCore(type, options));

#if DEBUG
    // where 'type' should be an non-compound elemental type (i.e., the element type of a type of an array, pointer, or by-ref)
    ThrowIfTypeIsCompoundType(type, nameof(type));
#endif

    if (IsValueTupleType(type)) {
      // special case for value tuples (ValueTuple<>)
      return FormatValueTupleType(target, builder, options)
        .Append(GetNullabilityAnnotation(target));
    }

    var isGenericTypeClosedOrDefinition =
      type.IsGenericTypeDefinition ||
      type.IsConstructedGenericType ||
      (type.IsGenericType && type.ContainsGenericParameters);

    if (Nullable.GetUnderlyingType(type) is Type nullableUnderlyingType) {
      // nullable value types (Nullable<>)
      if (IsValueTupleType(nullableUnderlyingType)) {
        // special case for nullable value tuples (Nullable<ValueTuple<>>)
        return FormatValueTupleType(target, builder, options)
          .Append(GetNullabilityAnnotation(target));
      }
      else if (nullableUnderlyingType.IsGenericType) {
        // case for nullable generic value types (Nullable<GenericValueType<>>)
        return FormatNullableGenericValueType(target, builder, options)
          .Append(GetNullabilityAnnotation(target));
      }

      type = nullableUnderlyingType;
    }
    else if (isGenericTypeClosedOrDefinition) {
      // other generic types
      return FormatClosedGenericTypeOrGenericTypeDefinition(target, builder, options)
        .Append(GetNullabilityAnnotation(target));
    }

    if (options.TranslateLanguagePrimitiveType && IsLanguagePrimitiveType(type, out var n)) {
      // language primitive types
      return builder
        .Append(n)
        .Append(GetNullabilityAnnotation(target));
    }

    if (type.IsGenericParameter && type.HasGenericParameterNoConstraints())
      // generic parameter which has no constraints must not have nullability annotation
      return builder.Append(GetTypeName(type, options));

    if (!type.IsGenericParameter) {
      if (type.IsNested && options.WithDeclaringTypeName)
        builder.Append(FormatTypeNameCore(type.GetDeclaringTypeOrThrow(), options)).Append('.');
      else if (options.TypeWithNamespace)
        builder.Append(type.Namespace).Append('.');
    }

    return builder
      .Append(GetTypeName(type, options))
      .Append(GetNullabilityAnnotation(target));
  }
#pragma warning restore CA1502

  private static StringBuilder FormatClosedGenericTypeOrGenericTypeDefinition(
    NullabilityInfo target,
    StringBuilder builder,
    FormatTypeNameOptions options
  )
  {
    var type = target.Type.IsByRef
      ? target.Type.GetElementType()!
      : target.Type;

    if (options.TypeWithNamespace && !type.IsNested)
      builder.Append(type.Namespace).Append('.');

    IEnumerable<NullabilityInfo> genericTypeArguments = target.GenericTypeArguments;

    if (type.IsNested) {
      var declaringType = type.GetDeclaringTypeOrThrow();
      var genericArgsOfDeclaringType = declaringType.GetGenericArguments();

      if (options.WithDeclaringTypeName) {
        if (declaringType.IsGenericTypeDefinition) {
          declaringType = declaringType.MakeGenericType(
            type.GetGenericArguments().Take(genericArgsOfDeclaringType.Length).ToArray()
          );
        }

        builder.Append(FormatTypeNameCore(declaringType, options)).Append('.');
      }

      genericTypeArguments = genericTypeArguments.Skip(genericArgsOfDeclaringType.Length);
    }

    builder.Append(GetTypeName(type, options));

    if (genericTypeArguments.Any()) {
      builder.Append('<');

      foreach (var (arg, i) in genericTypeArguments.Select(static (arg, i) => (arg, i))) {
        if (0 < i)
          builder.Append(", ");

        FormatTypeNameWithNullabilityAnnotation(arg, builder, options);
      }

      builder.Append('>');
    }

    return builder;
  }

  private static StringBuilder FormatValueTupleType(
    NullabilityInfo target,
    StringBuilder builder,
    FormatTypeNameOptions options
  )
  {
    var tupleItemNames = options
      .AttributeProvider
      ?.GetCustomAttributeDataList()
      ?.FirstOrDefault(static d =>
        string.Equals(
          typeof(TupleElementNamesAttribute).FullName,
          d.AttributeType.FullName,
          StringComparison.Ordinal
        )
      )
      ?.ConstructorArguments
      ?.FirstOrDefault()
      .Value
      as IReadOnlyList<CustomAttributeTypedArgument>;

    builder.Append('(');

    for (var i = 0; i < target.GenericTypeArguments.Length; i++) {
      if (0 < i)
        builder.Append(", ");

      FormatTypeNameWithNullabilityAnnotation(target.GenericTypeArguments[i], builder, options);

      if (tupleItemNames is not null)
        builder.Append(' ').Append(tupleItemNames[i].Value);
    }

    return builder.Append(')');
  }

  private static StringBuilder FormatNullableGenericValueType(
    NullabilityInfo target,
    StringBuilder builder,
    FormatTypeNameOptions options
  )
  {
    if (options.TypeWithNamespace && !target.Type.IsNested)
      builder.Append(target.Type.Namespace).Append('.');

    builder
      .Append(
        GetTypeName(
          target.Type.GenericTypeArguments[0], // the type of GenericValueType of Nullable<GenericValueType<>>
          options
        )
      )
      .Append('<');

    for (var i = 0; i < target.GenericTypeArguments.Length; i++) {
      if (0 < i)
        builder.Append(", ");

      FormatTypeNameWithNullabilityAnnotation(target.GenericTypeArguments[i], builder, options);
    }

    return builder.Append('>');
  }
}
#endif // SYSTEM_REFLECTION_NULLABILITYINFO
