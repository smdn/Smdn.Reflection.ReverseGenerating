// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Smdn.Reflection;

internal static class CustomAttributeDataExtensions {
  public static Type GetAttributeType(this CustomAttributeData attributeData)
#if CAN_OVERRIDE_CUSTOMATTRIBUTEDATA_ATTRIBUTETYPE
    => attributeData.AttributeType;
#else
    => attributeData is StructLayoutCustomAttributeData
      ? typeof(StructLayoutAttribute) // cannot override CustomAttributeData.AttributeType
      : attributeData.AttributeType;
#endif
}
