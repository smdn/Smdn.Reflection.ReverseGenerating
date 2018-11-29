using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Smdn.Reflection {
  [TestFixture()]
  public class TypeExtensionsTests {
    private delegate void D();

    [TestCase(typeof(D), true)]
    [TestCase(typeof(Guid), false)]
    [TestCase(typeof(System.Delegate), true)]
    [TestCase(typeof(System.MulticastDelegate), true)]
    public void TestIsDelegate(Type type, bool expected)
    {
      Assert.AreEqual(expected, type.IsDelegate());
    }

    enum E1 { }
    [Flags] enum E2 { }

    [TestCase(typeof(E1), false)]
    [TestCase(typeof(E2), true)]
    [TestCase(typeof(System.IO.FileAttributes), true)]
    [TestCase(typeof(System.DateTimeKind), false)]
    public void TestIsEnumFlags(Type type, bool expected)
    {
      Assert.AreEqual(expected, type.IsEnumFlags());
    }

    struct SReadOnly1 {}
    readonly struct SReadOnly2 { }

    [TestCase(typeof(SReadOnly1), false)]
    [TestCase(typeof(SReadOnly2), true)]
    public void TestIsReadOnlyValueType(Type type, bool expected)
    {
      Assert.AreEqual(expected, type.IsReadOnlyValueType());
    }

    struct SByRefLike1 { }
    ref struct SByRefLike2 { }

    [TestCase(typeof(SByRefLike1), false)]
    [TestCase(typeof(SByRefLike2), true)]
    public void TestIsByRefLikeValueType(Type type, bool expected)
    {
      Assert.AreEqual(expected, type.IsByRefLikeValueType());
    }

    [TestCase(typeof(Action), new Type[] { }, typeof(void))]
    [TestCase(typeof(Action<int>), new[] { typeof(int) }, typeof(void))]
    [TestCase(typeof(Func<int, string>), new[] { typeof(int) }, typeof(string))]
    public void TestGetDelegateSignatureMethod(Type type, Type[] expectedParameterTypes, Type expectedReturnType)
    {
      var m = type.GetDelegateSignatureMethod();

      Assert.AreEqual(expectedReturnType, m.ReturnType, "return type");
      CollectionAssert.AreEqual(expectedParameterTypes, m.GetParameters().Select(p => p.ParameterType), "parameter types");
    }

    struct STest1 {}

    struct STest2 : IDisposable {
      public void Dispose() => throw new NotImplementedException();
    }

    class CTestBase : ICloneable {
      public object Clone() => throw new NotImplementedException();
    }

    class CTest : CTestBase, IDisposable {
      void IDisposable.Dispose() => throw new NotImplementedException();
    }

    [TestCase(typeof(DayOfWeek), new Type[] { })]
    [TestCase(typeof(Action), new Type[] { })]
    [TestCase(typeof(STest1), new Type[] { })]
    [TestCase(typeof(STest2), new[] { typeof(IDisposable) })]
    [TestCase(typeof(CTestBase), new[] { typeof(ICloneable) })]
    [TestCase(typeof(CTest), new[] { typeof(CTestBase), typeof(IDisposable) })]
    public void TestGetExplicitBaseTypeAndInterfaces(Type type, Type[] expectedBaseTypes)
    {
      CollectionAssert.AreEquivalent(expectedBaseTypes,
                                     type.GetExplicitBaseTypeAndInterfaces());
    }

    [TestCase(typeof(int), new[] { "System" })]
    [TestCase(typeof(int*), new[] { "System" })]
    [TestCase(typeof(int[]), new[] { "System" })]
    [TestCase(typeof(int?), new[] { "System" })]
    [TestCase(typeof((int, int)), new[] { "System" })]
    [TestCase(typeof((int, System.IO.Stream)), new[] { "System", "System.IO" })]
    [TestCase(typeof(Action), new[] { "System" })]
    [TestCase(typeof(List<>), new[] { "System.Collections.Generic" })]
    [TestCase(typeof(List<int>), new[] { "System", "System.Collections.Generic" })]
    [TestCase(typeof(List<int?>), new[] { "System", "System.Collections.Generic" })]
    [TestCase(typeof(List<int[]>), new[] { "System", "System.Collections.Generic" })]
    [TestCase(typeof(List<KeyValuePair<int, int>>), new[] { "System", "System.Collections.Generic" })]
    public void TestGetNamespaces(Type type, string[] expected)
    {
      CollectionAssert.AreEquivalent(expected,
                                     type.GetNamespaces());
    }

    [TestCase(typeof(int), new string[] { })]
    [TestCase(typeof(int[]), new string[] { })]
    [TestCase(typeof(List<>), new[] { "System.Collections.Generic" })]
    [TestCase(typeof(List<int>), new[] { "System.Collections.Generic" })]
    public void TestGetNamespacesWithPrimitiveType(Type type, string[] expected)
    {
      CollectionAssert.AreEquivalent(expected,
                                     type.GetNamespaces(t => t == typeof(int)));
    }
  }
}
