using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Smdn.Reflection {
  [TestFixture()]
  public class MethodBaseExtensionsTests {
    class C1 {
      public void M1() => throw new NotImplementedException();
      public int M2() => throw new NotImplementedException();
      public void M3(int p1) => throw new NotImplementedException();
      public int M4(int p1) => throw new NotImplementedException();
      public int M5(int p1, int p2) => throw new NotImplementedException();
    }

    [TestCase(typeof(C1), nameof(C1.M1), new[] { typeof(void) })]
    [TestCase(typeof(C1), nameof(C1.M2), new[] { typeof(int) })]
    [TestCase(typeof(C1), nameof(C1.M3), new[] { typeof(int), typeof(void) })]
    [TestCase(typeof(C1), nameof(C1.M4), new[] { typeof(int), typeof(int) })]
    [TestCase(typeof(C1), nameof(C1.M5), new[] { typeof(int), typeof(int), typeof(int) })]
    public void TestGetSignatureTypes(Type type, string methodName, Type[] expected)
    {
      CollectionAssert.AreEqual(expected, type.GetMethod(methodName).GetSignatureTypes());
    }

    class C2 : ICloneable, IDisposable {
      public void M() => throw new NotImplementedException();
      public object Clone() => throw new NotImplementedException();
      void IDisposable.Dispose() => throw new NotImplementedException();
    }

    [TestCase(typeof(C2), nameof(C2.M), null, null)]
    [TestCase(typeof(C2), nameof(C2.Clone), null, null)]
    [TestCase(typeof(C2), "System.IDisposable.Dispose", typeof(IDisposable), nameof(IDisposable.Dispose))]
    public void TestFindExplicitInterfaceMethod(Type type, string methodName, Type expectedInterface, string expectedMethodName)
    {
      var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      var expectedMethod = expectedInterface?.GetMethod(expectedMethodName);

      Assert.AreEqual(expectedMethod, method.FindExplicitInterfaceMethod());
    }

    [TestCase(typeof(C2), nameof(C2.M), false)]
    [TestCase(typeof(C2), nameof(C2.Clone), false)]
    [TestCase(typeof(C2), "System.IDisposable.Dispose", true)]
    public void TestIsExplicitlyImplemented(Type type, string methodName, bool expected)
    {
      var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

      Assert.AreEqual(expected, method.IsExplicitlyImplemented());
    }

    [TestCase(typeof(NS.C1), "NS.I1.M", "M")]
    [TestCase(typeof(NS.C1), "NS.I2.M", null)]
    [TestCase(typeof(NS.C1), "NS.C1.I3.M", "M")]
    [TestCase(typeof(NS.C1), "NS.C1.I4.M", null)]
    [TestCase(typeof(NS.C1), "NS.C1.I5.M", "M")]
    [TestCase(typeof(NS.C1), "NS.C1.I6.M", "M")]
    [TestCase(typeof(NS.C1), "NS.C1.I7.M", null)]
    [TestCase(typeof(NS.C1), "NS.C1.I8.M", null)]
    public void TestFindExplicitInterfaceMethod_PublicInterfaceOnly(Type type, string methodName, string expectedMethodName)
    {
      var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      var explicitInterfaceMethod = method.FindExplicitInterfaceMethod(findOnlyPublicInterfaces: true);

      Assert.AreEqual(expectedMethodName, explicitInterfaceMethod?.Name, methodName);
    }
  }
}

namespace NS {
  public interface I1 {
    void M();
  }

  internal interface I2 {
    void M();
  }

  public class C1 : I1, I2, C1.I3, C1.I4, C1.I5, C1.I6, C1.I7, C1.I8 {
    void I1.M() => throw new NotImplementedException();
    void I2.M() => throw new NotImplementedException();
    void I3.M() => throw new NotImplementedException();
    void I4.M() => throw new NotImplementedException();
    void I5.M() => throw new NotImplementedException();
    void I6.M() => throw new NotImplementedException();
    void I7.M() => throw new NotImplementedException();
    void I8.M() => throw new NotImplementedException();

    public interface I3 {
      void M();
    }

    internal interface I4 {
      void M();
    }

    protected interface I5 {
      void M();
    }

    internal protected interface I6 {
      void M();
    }

    private protected interface I7 {
      void M();
    }

    private interface I8 {
      void M();
    }
  }
}

