// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
class MemberInfoComparerTests {
  class C {
#pragma warning disable CS0067
    static C() {}
    public static int SF = default;
    public static int SP { get; } = default;
    public static event EventHandler SE;
    public static void SM() => throw new NotImplementedException();

    public C() {}
    public int F = default;
    public int P { get; } = default;
    public event EventHandler E;
    public void M() => throw new NotImplementedException();
#pragma warning restore CS0067
  }

  [Test]
  public void Compare_Default()
  {
    var t = typeof(C);
    var members = new MemberInfo[] {
      t.TypeInitializer,
      t.GetField("SF"),
      t.GetProperty("SP"),
      t.GetEvent("SE"),
      t.GetMethod("SM"),
      t.GetConstructor(BindingFlags.Public | BindingFlags.Instance, binder: null, Type.EmptyTypes, modifiers: null),
      t.GetField("F"),
      t.GetProperty("P"),
      t.GetEvent("E"),
      t.GetMethod("M"),
    };

    Assert.That(
      members.OrderBy(m => m, MemberInfoComparer.Default),
      Is.EqualTo(new MemberInfo[] {
        t.GetEvent("SE"),
        t.GetEvent("E"),
        t.GetField("SF"),
        t.GetField("F"),
        t.TypeInitializer,
        t.GetConstructor(BindingFlags.Public | BindingFlags.Instance, binder: null, Type.EmptyTypes, modifiers: null),
        t.GetProperty("SP"),
        t.GetProperty("P"),
        t.GetMethod("SM"),
        t.GetMethod("M"),
      }).AsCollection
    );
  }

  [Test]
  public void Compare_StaticMembersFirst()
  {
    var t = typeof(C);
    var members = new MemberInfo[] {
      t.TypeInitializer,
      t.GetField("SF"),
      t.GetProperty("SP"),
      t.GetEvent("SE"),
      t.GetMethod("SM"),
      t.GetConstructor(BindingFlags.Public | BindingFlags.Instance, binder: null, Type.EmptyTypes, modifiers: null),
      t.GetField("F"),
      t.GetProperty("P"),
      t.GetEvent("E"),
      t.GetMethod("M"),
    };

    Assert.That(
      members.OrderBy(m => m, MemberInfoComparer.StaticMembersFirst),
      Is.EqualTo(new MemberInfo[] {
        t.GetEvent("SE"),
        t.GetField("SF"),
        t.TypeInitializer,
        t.GetProperty("SP"),
        t.GetMethod("SM"),
        t.GetEvent("E"),
        t.GetField("F"),
        t.GetConstructor(BindingFlags.Public | BindingFlags.Instance, binder: null, Type.EmptyTypes, modifiers: null),
        t.GetProperty("P"),
        t.GetMethod("M"),
      }).AsCollection
    );
  }
}
