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

  namespace TypeDeclarationWithExplicitBaseTypeAndInterfaces {
    [Test("public class C1 : System.IDisposable")]
    public class C1 : IDisposable {
      public void Dispose() => throw new NotImplementedException();
    }

    [Test("public class C2 :\nSystem.ICloneable,\nSystem.IDisposable")]
    public class C2 : IDisposable, ICloneable {
      public object Clone() => throw new NotImplementedException();
      public void Dispose() => throw new NotImplementedException();
    }

    [Test("public struct S1 : System.IDisposable")]
    public struct S1 : IDisposable {
      public void Dispose() => throw new NotImplementedException();
    }

    [Test("public struct S2 :\nSystem.ICloneable,\nSystem.IDisposable")]
    public struct S2 : IDisposable, ICloneable {
      public object Clone() => throw new NotImplementedException();
      public void Dispose() => throw new NotImplementedException();
    }

    [Test("public interface I1 : System.IDisposable")]
    public interface I1 : IDisposable {
    }

    [Test("public interface I2 :\nSystem.ICloneable,\nSystem.IDisposable")]
    public interface I2 : IDisposable, ICloneable {
    }

    namespace WithConstraints {
      [Test("public class C1<T> : System.Collections.Generic.List<T> where T : class")]
      public class C1<T> :
        List<T>
        where T : class
      {
        public void Dispose() => throw new NotImplementedException();
      }

      [Test("public class C2<T> :\nSystem.Collections.Generic.List<T>,\nSystem.ICloneable\nwhere T : class")]
      public class C2<T> :
        List<T>,
        ICloneable
        where T : class
      {
        public object Clone() => throw new NotImplementedException();
      }

      [Test("public class C3<TKey, TValue> :\nSystem.Collections.Generic.Dictionary<TKey, TValue>,\nSystem.ICloneable\nwhere TKey : class\nwhere TValue : struct")]
      public class C3<TKey, TValue> :
        Dictionary<TKey, TValue>,
        ICloneable
        where TKey : class
        where TValue : struct
      {
        public object Clone() => throw new NotImplementedException();
      }
    }
  }
}

