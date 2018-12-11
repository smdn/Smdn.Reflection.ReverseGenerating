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
  public static class EventInfoExtensions {
    public static IEnumerable<MethodInfo> GetMethods(this EventInfo ev)
    {
      return GetMethods(ev, false);
    }

    public static IEnumerable<MethodInfo> GetMethods(this EventInfo ev, bool nonPublic)
    {
      var methodAdd = ev.GetAddMethod(nonPublic);       if (methodAdd != null) yield return methodAdd;
      var methodRemove = ev.GetRemoveMethod(nonPublic); if (methodRemove != null) yield return methodRemove;
      var methodRaise = ev.GetRaiseMethod(nonPublic);   if (methodRaise != null) yield return methodRaise;

      IEnumerable<MethodInfo> otherMethods = null;

      try {
        otherMethods = ev.GetOtherMethods(nonPublic);
      }
      catch (NullReferenceException) { // MonoEvent.GetOtherMethods throws NullReferenceException
        // ignore exceptions
      }

      if (otherMethods == null)
        yield break;

      foreach (var m in otherMethods) {
        if (m != null)
          yield return m;
      }
    }
  }
}
