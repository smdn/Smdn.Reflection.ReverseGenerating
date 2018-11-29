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
  }
}
