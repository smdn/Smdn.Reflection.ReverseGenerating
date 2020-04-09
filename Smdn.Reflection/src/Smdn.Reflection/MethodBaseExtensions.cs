// 
// Copyright (c) 2018 smdn <smdn@smdn.jp>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Smdn.Reflection {
  public static class MethodBaseExtensions {
    public static IEnumerable<Type> GetSignatureTypes(this MethodBase m)
    {
      foreach (var p in m.GetParameters())
        yield return p.ParameterType;

      if (m is MethodInfo mm)
        yield return mm.ReturnType;
    }

    public static bool IsExplicitlyImplemented(this MethodBase m)
    {
      return FindExplicitInterfaceMethod(m, findOnlyPublicInterfaces: false) != null;
    }

    public static MethodInfo FindExplicitInterfaceMethod(this MethodBase m, bool findOnlyPublicInterfaces = false)
    {
      if (m is MethodInfo im && im.IsFinal && im.IsPrivate) {
        foreach (var iface in im.DeclaringType.GetInterfaces()) {
          if (findOnlyPublicInterfaces && !(iface.IsPublic || iface.IsNestedPublic || iface.IsNestedFamily || iface.IsNestedFamORAssem))
            continue;

          var interfaceMap = im.DeclaringType.GetInterfaceMap(iface);

          for (var index = 0; index < interfaceMap.TargetMethods.Length; index++) {
            if (interfaceMap.TargetMethods[index] == im)
              return interfaceMap.InterfaceMethods[index];
          }
        }
      }

      return null;
    }

    private static readonly Dictionary<string, MethodSpecialName> specialMethodNames = new Dictionary<string, MethodSpecialName>(StringComparer.Ordinal) {
      // comparison
      {"op_Equality", MethodSpecialName.Equality},
      {"op_Inequality", MethodSpecialName.Inequality},
      {"op_LessThan", MethodSpecialName.LessThan},
      {"op_GreaterThan", MethodSpecialName.GreaterThan},
      {"op_LessThanOrEqual", MethodSpecialName.LessThanOrEqual},
      {"op_GreaterThanOrEqual", MethodSpecialName.GreaterThanOrEqual},

      // unary
      {"op_UnaryPlus", MethodSpecialName.UnaryPlus},
      {"op_UnaryNegation", MethodSpecialName.UnaryNegation},
      {"op_LogicalNot", MethodSpecialName.LogicalNot},
      {"op_OnesComplement", MethodSpecialName.OnesComplement},
      {"op_True", MethodSpecialName.True},
      {"op_False", MethodSpecialName.False},
      {"op_Increment", MethodSpecialName.Increment},
      {"op_Decrement", MethodSpecialName.Decrement},

      // binary
      {"op_Addition", MethodSpecialName.Addition},
      {"op_Subtraction", MethodSpecialName.Subtraction},
      {"op_Multiply", MethodSpecialName.Multiply},
      {"op_Division", MethodSpecialName.Division},
      {"op_Modulus", MethodSpecialName.Modulus},
      {"op_BitwiseAnd", MethodSpecialName.BitwiseAnd},
      {"op_BitwiseOr", MethodSpecialName.BitwiseOr},
      {"op_ExclusiveOr", MethodSpecialName.ExclusiveOr},
      {"op_RightShift", MethodSpecialName.RightShift},
      {"op_LeftShift", MethodSpecialName.LeftShift},

      // type cast
      {"op_Explicit", MethodSpecialName.Explicit},
      {"op_Implicit", MethodSpecialName.Implicit},
    };

    public static MethodSpecialName GetNameType(this MethodBase m)
    {
      if (!m.IsSpecialName)
        return MethodSpecialName.None;

      if (specialMethodNames.TryGetValue(m.Name, out var methodSpecialName))
        return methodSpecialName;

      if (m is ConstructorInfo)
        return MethodSpecialName.Constructor;

      return MethodSpecialName.Unknown;
    }
  }
}
