using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

#pragma warning disable 0169
namespace TestCases {
  class TestAttribute : Attribute {
    public string Expected { get; private set; }

    public bool WithNamespace { get; set; } = true;
    public bool UseDefaultLiteral { get; set; } = false;

    public TestAttribute()
      : this(null)
    {
    }

    public TestAttribute(string expected)
    {
      this.Expected = expected;
    }
  }
}

