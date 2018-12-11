// Author:
//       smdn <smdn@smdn.jp>
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
using System.Reflection;

namespace Smdn.Reflection {
  public static class MemberInfoExtensions {
    public static Accessibility GetAccessibility(this MemberInfo member)
    {
      switch (member) {
        case Type t:
          if (t.IsPublic || t.IsNestedPublic)       return Accessibility.Public;
          if (t.IsNotPublic || t.IsNestedAssembly)  return Accessibility.Assembly;
          if (t.IsNestedFamily)                     return Accessibility.Family;
          if (t.IsNestedFamORAssem)                 return Accessibility.FamilyOrAssembly;
          if (t.IsNestedFamANDAssem)                return Accessibility.FamilyAndAssembly;
          if (t.IsNestedPrivate)                    return Accessibility.Private;
          break;

        case FieldInfo f:
          if (f.IsPublic)               return Accessibility.Public;
          if (f.IsAssembly)             return Accessibility.Assembly;
          if (f.IsFamily)               return Accessibility.Family;
          if (f.IsFamilyOrAssembly)     return Accessibility.FamilyOrAssembly;
          if (f.IsFamilyAndAssembly)    return Accessibility.FamilyAndAssembly;
          if (f.IsPrivate)              return Accessibility.Private;
          break;

        case MethodBase m:
          if (m.IsPublic)               return Accessibility.Public;
          if (m.IsAssembly)             return Accessibility.Assembly;
          if (m.IsFamily)               return Accessibility.Family;
          if (m.IsFamilyOrAssembly)     return Accessibility.FamilyOrAssembly;
          if (m.IsFamilyAndAssembly)    return Accessibility.FamilyAndAssembly;
          if (m.IsPrivate)              return Accessibility.Private;
          break;

        case PropertyInfo p:
          throw new InvalidOperationException("cannot get accessibility of " + nameof(PropertyInfo));

        case EventInfo ev:
          throw new InvalidOperationException("cannot get accessibility of " + nameof(EventInfo));

        default:
          throw new NotSupportedException("unknown member type");
      }

      return Accessibility.Undefined;
    }
  }
}
