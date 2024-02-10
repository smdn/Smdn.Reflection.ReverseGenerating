// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if WORKAROUND_NULLABILITYINFO_BYREFTYPE
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Smdn.Reflection;

// Workaround: The pseudo PropertyInfo type which unwraps 'ByRef' type to its element type
// See https://github.com/dotnet/runtime/issues/72320
internal sealed class UnwrapByRefPropertyInfo : PropertyInfo {
  public PropertyInfo BaseProperty { get; }

  public UnwrapByRefPropertyInfo(PropertyInfo baseProperty)
  {
#if DEBUG
    if (!baseProperty.PropertyType.IsByRef)
      throw new ArgumentException($"{baseProperty.PropertyType} must be by-ref");
#endif

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
