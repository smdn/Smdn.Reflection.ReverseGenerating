using System;
using System.Collections.Generic;

namespace Smdn.Reflection.ReverseGenerating {
  internal static class ISetExtensions {
    public static void AddRange<T>(this ISet<T> s, IEnumerable<T> sequence)
    {
      foreach (var e in sequence)
        s.Add(e);
    }
  }
}

