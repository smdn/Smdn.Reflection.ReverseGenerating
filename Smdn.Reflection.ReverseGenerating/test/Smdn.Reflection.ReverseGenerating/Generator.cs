using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  internal abstract class GeneratorTestCaseAttribute : Attribute {
    public string Expected { get; private set; }
    public bool WithNamespace { get; set; } = true;
    public string SourceLocation { get; }

    public GeneratorTestCaseAttribute(
      string expected,
      string sourceFilePath,
      int lineNumber
    )
    {
      this.Expected = expected;
      this.SourceLocation = $"{Path.GetFileName(sourceFilePath)}:{lineNumber}";
    }
  }

  [TestFixture]
  public partial class GeneratorTests {
    private static IEnumerable<Type> FindTypes(Func<Type, bool> predicate)
      => Assembly.GetExecutingAssembly().GetTypes().Where(predicate);
  }
}
