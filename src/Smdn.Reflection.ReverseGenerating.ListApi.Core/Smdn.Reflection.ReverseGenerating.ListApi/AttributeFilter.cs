// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static class AttributeFilter {
  public static readonly AttributeTypeFilter Default = DefaultImpl;

  private static bool DefaultImpl(Type attrType, ICustomAttributeProvider attrProvider)
  {
    if (ROCType.FullNameEquals(typeof(System.CLSCompliantAttribute), attrType))
      return false;

    switch (attrProvider) {
      case Type:
        if (ROCType.FullNameEquals(typeof(System.Reflection.DefaultMemberAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.IsReadOnlyAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.ExtensionAttribute), attrType))
          return false;
        break;

      case MethodBase:
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.ExtensionAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.IteratorStateMachineAttribute), attrType))
          return false;
        break;

      case FieldInfo:
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.TupleElementNamesAttribute), attrType))
          // TupleElementNamesAttribute from System.Runtime/mscorlib
          // TupleElementNamesAttribute from System.ValueTuple
          return false;
        break;

      case PropertyInfo:
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.TupleElementNamesAttribute), attrType))
          return false;
        break;

      case ParameterInfo:
        if (ROCType.FullNameEquals(typeof(System.Runtime.InteropServices.OptionalAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.Runtime.InteropServices.InAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.Runtime.InteropServices.OutAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.ParamArrayAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.TupleElementNamesAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.IsReadOnlyAttribute), attrType))
          return false;
        break;
    }

    return true;
  }
}
