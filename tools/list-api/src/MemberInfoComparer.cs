using System;
using System.Collections.Generic;
using System.Reflection;

using Smdn.Reflection;

class MemberInfoComparer : IComparer<MemberInfo> {
  public static readonly MemberInfoComparer Default = new(orderOfStaticMember: 0, orderOfInstanceMember: 0);
  public static readonly MemberInfoComparer StaticMembersFirst = new(orderOfStaticMember: 0, orderOfInstanceMember: 10);

  private readonly int orderOfStaticMember;
  private readonly int orderOfInstanceMember;

  public MemberInfoComparer(int orderOfStaticMember, int orderOfInstanceMember)
  {
    this.orderOfStaticMember = orderOfStaticMember;
    this.orderOfInstanceMember = orderOfInstanceMember;
  }

  public int Compare(MemberInfo x, MemberInfo y)
  {
    var ox = GetOrder(x);
    var oy = GetOrder(y);

    return Comparer<int>.Default.Compare(ox, oy);
  }

  private int GetStaticMemberOrder(bool isStatic) => isStatic ? orderOfStaticMember : orderOfInstanceMember;

  public int GetOrder(MemberInfo member)
    => member switch {
      null                  => 0,
      EventInfo e           => 1 + GetStaticMemberOrder(e.IsStatic()),
      FieldInfo f           => 2 + GetStaticMemberOrder(f.IsStatic),
      ConstructorInfo ctor  => 3 + GetStaticMemberOrder(ctor.IsStatic),
      PropertyInfo p        => 4 + GetStaticMemberOrder(p.IsStatic()),
      MethodInfo m          => 5 + GetStaticMemberOrder(m.IsStatic),
      _                     => 9999,
    };
}