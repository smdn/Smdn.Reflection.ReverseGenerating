using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

#pragma warning disable 0169, 0649, 0067
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

  [AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Delegate |
    AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Property
  )]
  class NamespacesTestAttribute : Attribute {
    public string Expected { get; private set; }

    public NamespacesTestAttribute(string expected)
    {
      this.Expected = expected;
    }
  }

  namespace Namespaces {
    namespace Types {
      [Test][NamespacesTest("")] delegate void D0();
      [Test][NamespacesTest("")] delegate void D1(int x);
      [Test][NamespacesTest("")] delegate void D2(int? x);
      [Test][NamespacesTest("")] delegate void D3(int[] x);
      [Test][NamespacesTest("")] unsafe delegate void D4(int* x);
      [Test][NamespacesTest("System")] delegate void D5(Guid x);
      [Test][NamespacesTest("System")] delegate void D6(Guid? x);
      [Test][NamespacesTest("System")] delegate void D7(Guid[] x);
      [Test][NamespacesTest("System.Collections.Generic")] delegate void D8(List<int> x);
      [Test][NamespacesTest("System, System.Collections.Generic")] delegate void D9(List<Guid> x);

      [Test][NamespacesTest("System.Collections.Generic")] class ListEx1 : List<int> {}
      [Test][NamespacesTest("System, System.Collections.Generic")] class ListEx2 : List<Guid> {}
      [Test][NamespacesTest("System.Collections.Generic")] class ListEx3<T> : List<T> {}
    }

    namespace Members {
      class C {
        [Test][NamespacesTest("")] public int F1;
        [Test][NamespacesTest("")] public int? F2;
        [Test][NamespacesTest("System")] public Guid F3;
        [Test][NamespacesTest("System.Collections.Generic")] public List<int> F4;
        [Test][NamespacesTest("System, System.Collections.Generic")] public List<Guid> F5;
        [Test][NamespacesTest("")] public int[] F6;
        [Test][NamespacesTest("")] public Nullable<int> F7;
        [Test][NamespacesTest("System")] public Action<int> F8;
        [Test][NamespacesTest("System, System.Collections.Generic")] public List<Action<int>> F9;
        [Test][NamespacesTest("System, System.Collections.Generic")] public Action<List<int>> F10;
        [Test][NamespacesTest("System, System.Collections.Generic, System.Collections.ObjectModel")] public System.Collections.ObjectModel.Collection<List<Action<int>>> F11;
        [Test][NamespacesTest("System, System.Collections.Generic, System.Collections.ObjectModel")] public Action<System.Collections.ObjectModel.Collection<List<int>>> F12;

        [Test][NamespacesTest("System")] public event EventHandler E1;
        [Test][NamespacesTest("System, System.Collections.Generic")] public event EventHandler<IList<int>> E2;

        [Test][NamespacesTest("")] public int P1 { get; set; }
        [Test][NamespacesTest("")] public int? P2 { get; set; }
        [Test][NamespacesTest("System")] public Guid P3 { get; set; }
        [Test][NamespacesTest("System.Collections.Generic")] public IList<int> P4 { get; set; }
        [Test][NamespacesTest("System, System.Collections.Generic")] public IList<Guid> P5 { get; set; }

        [Test][NamespacesTest("")] public void M0() {}
        [Test][NamespacesTest("")] public void M1(int x) {}
        [Test][NamespacesTest("")] public void M2(int? x) {}
        [Test][NamespacesTest("System")] public void M3(Guid x) {}
        [Test][NamespacesTest("System.Collections.Generic")] public void M4(List<int> x) {}
        [Test][NamespacesTest("System, System.Collections.Generic")] public void M5(List<Guid> x) {}
        [Test][NamespacesTest("")] public unsafe void M6(int* x) {}

        [Test][NamespacesTest("")] public int M1() => throw new NotImplementedException();
        [Test][NamespacesTest("")] public int? M2() => throw new NotImplementedException();
        [Test][NamespacesTest("System")] public Guid M3() => throw new NotImplementedException();
        [Test][NamespacesTest("System.Collections.Generic")] public List<int> M4() => throw new NotImplementedException();
        [Test][NamespacesTest("System, System.Collections.Generic")] public List<Guid> M5() => throw new NotImplementedException();
      }
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

