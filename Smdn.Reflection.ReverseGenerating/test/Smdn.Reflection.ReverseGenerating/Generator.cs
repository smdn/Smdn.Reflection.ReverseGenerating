using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  [TestFixture]
  public partial class GeneratorTests {
    private static IEnumerable<Type> FindTypes(Func<Type, bool> predicate)
      => Assembly.GetExecutingAssembly().GetTypes().Where(predicate);
  }
}
