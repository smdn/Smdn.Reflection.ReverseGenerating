using System;
using System.Collections.Generic;

public static class ISetExtensions {
  public static void AddRange<T>(this ISet<T> s, IEnumerable<T> sequence)
  {
    foreach (var e in sequence)
      s.Add(e);
  }
}
