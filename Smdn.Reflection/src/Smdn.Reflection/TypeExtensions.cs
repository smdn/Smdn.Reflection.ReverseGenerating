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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection {
  public static class TypeExtensions {
    public static bool IsDelegate(this Type t) => t.IsSubclassOf(typeof(System.Delegate)) || t == typeof(System.Delegate);
    public static bool IsConcreteDelegate(this Type t) => t != typeof(System.Delegate) && t != typeof(System.MulticastDelegate) && t.IsSubclassOf(typeof(System.Delegate));

    public static bool IsEnumFlags(this Type t) => t.IsEnum && t.GetCustomAttribute<FlagsAttribute>() != null;

    public static bool IsReadOnlyValueType(this Type t) => t.IsValueType && t.GetCustomAttributes(false).Any(a => string.Equals(a.GetType().FullName, "System.Runtime.CompilerServices.IsReadOnlyAttribute", StringComparison.Ordinal));
    public static bool IsByRefLikeValueType(this Type t) => t.IsValueType && t.GetCustomAttributes(false).Any(a => string.Equals(a.GetType().FullName, "System.Runtime.CompilerServices.IsByRefLikeAttribute", StringComparison.Ordinal));

    public static MethodInfo GetDelegateSignatureMethod(this Type t)
    {
      if (IsDelegate(t))
        return t.GetMethod("Invoke");
      else
        return null;
    }

    public static IEnumerable<Type> GetExplicitBaseTypeAndInterfaces(this Type t)
    {
      if (t.IsEnum || t.IsDelegate())
        yield break;

      // explicit base type
      if (t.BaseType != null && t.BaseType != typeof(object) && t.BaseType != typeof(ValueType))
        yield return t.BaseType;

      // interfaces
      var allInterfaces = t.GetInterfaces();
      var interfaces = allInterfaces.Except(allInterfaces.SelectMany(i => i.GetInterfaces())) // flatten
                                    .Except(t.BaseType?.GetInterfaces() ?? Type.EmptyTypes);

      foreach (var iface in interfaces)
        yield return iface;
    }

    public static IEnumerable<string> GetNamespaces(this Type t)
    {
      return GetAllNamespaces(t, type => false).Distinct();
    }

    public static IEnumerable<string> GetNamespaces(this Type t, Func<Type, bool> isLanguagePrimitive)
    {
      return GetAllNamespaces(t, isLanguagePrimitive).Distinct();
    }

    private static IEnumerable<string> GetAllNamespaces(this Type t, Func<Type, bool> isLanguagePrimitive)
    {
      Type elementType = null;

      if (t.IsArray || t.IsByRef || t.IsPointer)
        elementType = t.GetElementType();
      else
        elementType = Nullable.GetUnderlyingType(t);

      if (elementType != null) {
        foreach (var ns in GetNamespaces(elementType, isLanguagePrimitive))
          yield return ns;

        yield break;
      }

      if (t.IsGenericParameter && t.ContainsGenericParameters)
        yield break;

      if (t.IsConstructedGenericType || (t.IsGenericType && t.ContainsGenericParameters)) {
        yield return t.Namespace;

        foreach (var ns in t.GetGenericArguments().SelectMany(type => GetNamespaces(type, isLanguagePrimitive)))
          yield return ns;

        yield break;
      }

      if (!isLanguagePrimitive(t))
        yield return t.Namespace;
    }

    public static string GetGenericTypeName(this Type t)
    {
      if (!t.IsGenericType)
        throw new ArgumentException($"{t} is not a generic type", nameof(t));

      var name = t.GetGenericTypeDefinition().Name;

      return name.Substring(0, name.LastIndexOf('`'));
    }
  }
}
