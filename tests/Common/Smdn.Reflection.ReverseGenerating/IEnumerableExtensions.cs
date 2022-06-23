using System;
using System.Collections.Generic;

namespace Smdn.Reflection.ReverseGenerating;

#if !(NET471_OR_GREATER)
internal static class IEnumerableExtensions {
  public static IEnumerable<TSource> Prepend<TSource>(
    this IEnumerable<TSource> sequence,
    TSource element
  )
  {
    yield return element;

    foreach (var e in sequence)
      yield return e;
  }
}
#endif
