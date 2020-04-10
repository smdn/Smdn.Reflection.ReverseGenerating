using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace TestTypesForMemberInfoExtensionsTests {
  public class C1 { }
  internal class C2 { }

  class C {
    public class C3 { }
    internal class C4 { }
    protected class C5 { }
    protected internal class C6 { }
    protected private class C7 { }
    private class C8 {}

    public void M1() => throw new NotImplementedException();
    internal void M2() => throw new NotImplementedException();
    protected void M3() => throw new NotImplementedException();
    protected internal void M4() => throw new NotImplementedException();
    protected private void M5() => throw new NotImplementedException();
    private void M6() => throw new NotImplementedException();

#pragma warning disable 0649, 0169, 0410
    public int F1;
    internal int F2;
    protected int F3;
    protected internal int F4;
    protected private int F5;
    private int F6;
#pragma warning restore 0649, 0169, 0410

    public int P1 { get; set; }

#pragma warning disable 0067
    public event EventHandler E1;
#pragma warning restore 0067
  }
}

namespace Smdn.Reflection {
  [TestFixture()]
  public class MemberInfoExtensionsTests {
    private void TestGetAccessibility(MemberInfo member, Accessibility expected)
    {
      Assert.AreEqual(expected,
                      member.GetAccessibility());
    }

    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C1), null, Accessibility.Public)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C2), null, Accessibility.Assembly)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C.C3), null, Accessibility.Public)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C.C4), null, Accessibility.Assembly)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "C5", Accessibility.Family)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C.C6), null, Accessibility.FamilyOrAssembly)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "C7", Accessibility.FamilyAndAssembly)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "C8", Accessibility.Private)]
    public void TestGetAccessibility_Types(Type type, string nestedTypeName, Accessibility expected)
    {
      if (nestedTypeName != null)
        type = type.GetNestedType(nestedTypeName, BindingFlags.Public | BindingFlags.NonPublic);

      TestGetAccessibility(type, expected);
    }

    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), nameof(TestTypesForMemberInfoExtensionsTests.C.M1), Accessibility.Public)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), nameof(TestTypesForMemberInfoExtensionsTests.C.M2), Accessibility.Assembly)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "M3", Accessibility.Family)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), nameof(TestTypesForMemberInfoExtensionsTests.C.M4), Accessibility.FamilyOrAssembly)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "M5", Accessibility.FamilyAndAssembly)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "M6", Accessibility.Private)]
    public void TestGetAccessibility_Methods(Type type, string memberName, Accessibility expected)
    {
      TestGetAccessibility(type.GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).First(),
                           expected);
    }

    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), nameof(TestTypesForMemberInfoExtensionsTests.C.F1), Accessibility.Public)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), nameof(TestTypesForMemberInfoExtensionsTests.C.F2), Accessibility.Assembly)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "F3", Accessibility.Family)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), nameof(TestTypesForMemberInfoExtensionsTests.C.F4), Accessibility.FamilyOrAssembly)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "F5", Accessibility.FamilyAndAssembly)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "F6", Accessibility.Private)]
    public void TestGetAccessibility_Fields(Type type, string memberName, Accessibility expected)
    {
      TestGetAccessibility(type.GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).First(),
                           expected);
    }

    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "P1")]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "E1")]
    public void TestGetAccessibility_OtherMembers(Type type, string memberName)
    {
      var member = type.GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).First();

      Assert.Throws<InvalidOperationException>(() => member.GetAccessibility());
    }

    [Test]
    public void TestGetAccessibility_ArgumentNull()
    {
      MemberInfo member = null;

      Assert.Throws<ArgumentNullException>(() => member.GetAccessibility());
    }

    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C1), null, null, false)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C2), null, null, true)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C.C3), null, null, false)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C.C4), null, null, true)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "C5", null, false)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C.C6), null, null, false)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "C7", null, true)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), "C8", null, true)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), null, nameof(TestTypesForMemberInfoExtensionsTests.C.M1), false)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), null, nameof(TestTypesForMemberInfoExtensionsTests.C.M2), true)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), null, "M3", false)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), null, nameof(TestTypesForMemberInfoExtensionsTests.C.M4), false)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), null, "M5", true)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), null, "M6", true)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), null, nameof(TestTypesForMemberInfoExtensionsTests.C.F1), false)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), null, nameof(TestTypesForMemberInfoExtensionsTests.C.F2), true)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), null, "F3", false)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), null, nameof(TestTypesForMemberInfoExtensionsTests.C.F4), false)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), null, "F5", true)]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), null, "F6", true)]
    public void TestIsPrivateOrAssembly(Type type, string nestedTypeName, string memberName, bool expected)
    {
      if (nestedTypeName != null)
        type = type.GetNestedType(nestedTypeName, BindingFlags.Public | BindingFlags.NonPublic);

      if (memberName == null) {
        Assert.AreEqual(expected, type.IsPrivateOrAssembly(), type.FullName);
      }
      else {
        var member = type.GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).First();

        Assert.AreEqual(expected, member.IsPrivateOrAssembly(), $"{type.FullName}.{member.Name}");
      }
    }

    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), nameof(TestTypesForMemberInfoExtensionsTests.C.P1))]
    [TestCase(typeof(TestTypesForMemberInfoExtensionsTests.C), nameof(TestTypesForMemberInfoExtensionsTests.C.E1))]
    public void TestIsPrivateOrAssembly_MemberInvalid(Type type, string memberName)
    {
      var member = type.GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).First();

      Assert.Throws<InvalidOperationException>(() => member.IsPrivateOrAssembly(), $"{type.FullName}.{member.Name}");
    }

    [Test]
    public void TestIsPrivateOrAssembly_ArgumentNull()
    {
      MemberInfo member = null;

      Assert.Throws<ArgumentNullException>(() => member.IsPrivateOrAssembly());
    }
  }
}
