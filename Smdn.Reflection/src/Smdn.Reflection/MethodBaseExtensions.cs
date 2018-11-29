// Author:
//       smdn <smdn@smdn.jp>
// 
// Copyright (c) 2018 smdn
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
      return FindExplicitInterfaceMethod(m) != null;
    }

    public static MethodInfo FindExplicitInterfaceMethod(this MethodBase m)
    {
      if (m is MethodInfo im && im.IsFinal && im.IsPrivate) {
        foreach (var iface in im.DeclaringType.GetInterfaces()) {
          var interfaceMap = im.DeclaringType.GetInterfaceMap(iface);

          for (var index = 0; index < interfaceMap.TargetMethods.Length; index++) {
            if (interfaceMap.TargetMethods[index] == im)
              return interfaceMap.InterfaceMethods[index];
          }
        }
      }

      return null;
    }
  }
}
