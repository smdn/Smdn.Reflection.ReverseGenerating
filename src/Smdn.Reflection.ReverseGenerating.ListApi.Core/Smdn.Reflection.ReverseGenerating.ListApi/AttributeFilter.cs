// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

using Smdn.Reflection.ReverseGenerating;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static class AttributeFilter {
  public static readonly AttributeTypeFilter Default = DefaultImpl;

  private static bool DefaultImpl(Type attrType, ICustomAttributeProvider attrProvider)
  {
    if (ROCType.FullNameEquals(typeof(System.CLSCompliantAttribute), attrType))
      return false;
    if (ROCType.FullNameEquals(typeof(System.Reflection.DefaultMemberAttribute), attrType))
      return false;

    switch (attrProvider) {
      case Type t:
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.IsReadOnlyAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.ExtensionAttribute), attrType))
          return false;
        break;

      case MethodBase m:
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.ExtensionAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.IteratorStateMachineAttribute), attrType))
          return false;
        break;

      case ParameterInfo para:
        if (ROCType.FullNameEquals(typeof(System.Runtime.InteropServices.OptionalAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.Runtime.InteropServices.InAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.Runtime.InteropServices.OutAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.ParamArrayAttribute), attrType))
          return false;
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.TupleElementNamesAttribute), attrType))
          // TupleElementNamesAttribute from System.Runtime/mscorlib
          // TupleElementNamesAttribute from System.ValueTuple
          return false;
        if (ROCType.FullNameEquals(typeof(System.Runtime.CompilerServices.IsReadOnlyAttribute), attrType))
          return false;
        break;
    }

    return true;
  }
}