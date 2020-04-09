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

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    class ExpectedMethodSpecialNameAttribute : Attribute {
      public MethodSpecialName Expected { get; }

      public ExpectedMethodSpecialNameAttribute(MethodSpecialName expected)
      {
        this.Expected = expected;
      }
    }

    class SpecialMethods {
      public class V { }
      public class W { }

      public class C : IDisposable {
        [ExpectedMethodSpecialName(MethodSpecialName.Constructor)] public C() { }
        [ExpectedMethodSpecialName(MethodSpecialName.None)] ~C() { }

        [ExpectedMethodSpecialName(MethodSpecialName.None)] public void Deconstruct(out int x, out int y, out int z) => throw new NotImplementedException();

        [ExpectedMethodSpecialName(MethodSpecialName.None)] void M() => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.None)] void IDisposable.Dispose() => throw new NotImplementedException();

        // unary operators
        [ExpectedMethodSpecialName(MethodSpecialName.UnaryPlus)] public static C operator +(C c) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.UnaryNegation)] public static C operator -(C c) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.LogicalNot)] public static C operator !(C c) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.OnesComplement)] public static C operator ~(C c) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.True)] public static bool operator true(C c) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.False)] public static bool operator false(C c) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.Increment)] public static C operator ++(C c) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.Decrement)] public static C operator --(C c) => throw new NotImplementedException();

        // binary operators
        [ExpectedMethodSpecialName(MethodSpecialName.Addition)] public static C operator +(C x, C y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.Subtraction)] public static C operator -(C x, C y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.Multiply)] public static C operator *(C x, C y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.Division)] public static C operator /(C x, C y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.Modulus)] public static C operator %(C x, C y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.BitwiseAnd)] public static C operator &(C x, C y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.BitwiseOr)] public static C operator |(C x, C y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.ExclusiveOr)] public static C operator ^(C x, C y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.RightShift)] public static C operator >>(C x, int y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.LeftShift)] public static C operator <<(C x, int y) => throw new NotImplementedException();

        // type cast
        [ExpectedMethodSpecialName(MethodSpecialName.Explicit)] public static explicit operator C(V v) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.Explicit)] public static explicit operator V(C c) => throw new NotImplementedException();

        [ExpectedMethodSpecialName(MethodSpecialName.Implicit)] public static implicit operator C(W w) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.Implicit)] public static implicit operator W(C c) => throw new NotImplementedException();
      }

      public class P : IEquatable<P>, IComparable<P> {
        // comparison
        [ExpectedMethodSpecialName(MethodSpecialName.Equality)] public static bool operator ==(P x, P y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.Inequality)] public static bool operator !=(P x, P y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.LessThan)] public static bool operator <(P x, P y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.GreaterThan)] public static bool operator >(P x, P y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.LessThanOrEqual)] public static bool operator <=(P x, P y) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.GreaterThanOrEqual)] public static bool operator >=(P x, P y) => throw new NotImplementedException();

        [ExpectedMethodSpecialName(MethodSpecialName.None)] public bool Equals(P other) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.None)] public int CompareTo(P other) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.None)] public override bool Equals(object obj) => throw new NotImplementedException();
        [ExpectedMethodSpecialName(MethodSpecialName.None)] public override int GetHashCode() => throw new NotImplementedException();
      }
    }

    [TestCase(typeof(SpecialMethods.C))]
    [TestCase(typeof(SpecialMethods.P))]
    public void TestGetNameType(Type type)
    {
      foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
        if (member is MethodBase method) {
          var attr = method.GetCustomAttribute<ExpectedMethodSpecialNameAttribute>();

          if (attr == null)
            continue;

          var expected = method.GetCustomAttribute<ExpectedMethodSpecialNameAttribute>().Expected;

          Assert.AreEqual(expected, method.GetNameType(), $"{type.FullName} : {method.Name}");
        }
      }
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

