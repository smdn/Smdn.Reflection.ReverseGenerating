using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Smdn.Reflection {
  [TestFixture()]
  public class EventInfoExtensionsTests {
#pragma warning disable 0067
    public event EventHandler E1;

    public event EventHandler E2 {
      add {
        throw new NotImplementedException();
      }
      remove {
        throw new NotImplementedException();
      }
    }

    private event EventHandler E3;
#pragma warning restore 0067

    [Test]
    public void TestGetMethods()
    {
      var e1 = GetType().GetEvent("E1", BindingFlags.Instance | BindingFlags.Public);

      CollectionAssert.IsNotEmpty(e1.GetMethods());
      Assert.AreEqual(2, e1.GetMethods().Count());

      var e2 = GetType().GetEvent("E2", BindingFlags.Instance | BindingFlags.Public);

      CollectionAssert.IsNotEmpty(e2.GetMethods());
      Assert.AreEqual(2, e2.GetMethods().Count());

      var e3 = GetType().GetEvent("E3", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

      CollectionAssert.IsEmpty(e3.GetMethods(false));
      CollectionAssert.IsNotEmpty(e3.GetMethods(true));
    }

#if false
Public Custom Event E As EventHandler
  AddHandler(value As EventHandler)
    Throw New NotImplementedException()
  End AddHandler

  RemoveHandler(value as EventHandler)
    Throw New NotImplementedException()
  End RemoveHandler

  RaiseEvent(sender As Object, e As EventArgs)
    Throw New NotImplementedException()
  End RaiseEvent
End Event
#endif
  }
}
