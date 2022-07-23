using System;
using System.Collections.Generic;

namespace ClassLibrary;

public class Class1 {
  public void Method1(string? input, out int output) => throw new NotImplementedException();

#if NET6_0_OR_GREATER
  public IEnumerable<string> Method2() => throw new NotImplementedException();
#elif NET48_OR_GREATER
  public IReadOnlyList<string> Method2() => throw new NotImplementedException();
#endif
}
