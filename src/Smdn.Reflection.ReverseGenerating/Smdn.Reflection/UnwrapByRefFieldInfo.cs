// SPDX-FileCopyrightText: 2024 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if WORKAROUND_NULLABILITYINFO_BYREFTYPE
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Smdn.Reflection;

// Workaround: The pseudo FieldInfo type which unwraps 'ByRef' type to its element type
// See https://github.com/dotnet/runtime/issues/72320
internal sealed class UnwrapByRefFieldInfo : FieldInfo {
  public FieldInfo BaseField { get; }

  public UnwrapByRefFieldInfo(FieldInfo baseField)
  {
#if DEBUG
    if (!baseField.FieldType.IsByRef)
      throw new ArgumentException($"{baseField.FieldType} must be by-ref");
#endif

    BaseField = baseField;
  }

  public override string Name => BaseField.Name;
  public override FieldAttributes Attributes => BaseField.Attributes;
  public override Type FieldType => BaseField.FieldType.GetElementType()!;
  public override Type? DeclaringType => BaseField.DeclaringType;
  public override Type? ReflectedType => BaseField.ReflectedType;
  public override RuntimeFieldHandle FieldHandle => BaseField.FieldHandle;
  public override IList<CustomAttributeData> GetCustomAttributesData() => BaseField.GetCustomAttributesData();
  public override object[] GetCustomAttributes(bool inherit) => BaseField.GetCustomAttributes(inherit);
  public override object[] GetCustomAttributes(Type attributeType, bool inherit) => BaseField.GetCustomAttributes(attributeType, inherit);
  public override bool IsDefined(Type attributeType, bool inherit) => BaseField.IsDefined(attributeType, inherit);
  public override object? GetValue(object? obj) => BaseField.GetValue(obj);
  public override void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, CultureInfo? culture)
    => BaseField.SetValue(obj, value, invokeAttr, binder, culture);
}
#endif
