using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  [TestFixture]
  public class CSharpTypeFormatterTests {
    [TestCase(typeof(int), "int")]
    [TestCase(typeof(int[]), "int[]")]
    [TestCase(typeof(int[,]), "int[,]")]
    [TestCase(typeof(int[][]), "int[][]")]
    [TestCase(typeof(int?), "int?")]
    [TestCase(typeof(Guid), "Guid")]
    [TestCase(typeof(Guid?), "Guid?")]
    [TestCase(typeof(Tuple<>), "Tuple<T1>")]
    [TestCase(typeof(Tuple<int>), "Tuple<int>")]
    [TestCase(typeof(Tuple<int[]>), "Tuple<int[]>")]
    [TestCase(typeof(Tuple<int?>), "Tuple<int?>")]
    [TestCase(typeof(Tuple<,>), "Tuple<T1, T2>")]
    [TestCase(typeof(Tuple<int, int>), "Tuple<int, int>")]
    [TestCase(typeof((int, int)), "(int, int)")]
    [TestCase(typeof((int x, int y)), "(int, int)")]
    [TestCase(typeof((int, (int, int))), "(int, (int, int))")]
    [TestCase(typeof(KeyValuePair<,>), "KeyValuePair<TKey, TValue>")]
    [TestCase(typeof(KeyValuePair<string, int>), "KeyValuePair<string, int>")]
    [TestCase(typeof(Tuple<Tuple<int>>), "Tuple<Tuple<int>>")]
    [TestCase(typeof(Tuple<Tuple<int>, Tuple<int>>), "Tuple<Tuple<int>, Tuple<int>>")]
    [TestCase(typeof(Converter<,>), "Converter<in TInput, out TOutput>")]
    [TestCase(typeof(Converter<int, string>), "Converter<int, string>")]
    [TestCase(typeof(Tuple<Converter<int, string>>), "Tuple<Converter<int, string>>")]
    public void TestFormatTypeName_WithoutNamespace(Type type, string expected)
    {
      Assert.AreEqual(
        expected,
        type.FormatTypeName(null, typeWithNamespace: false)
      );
    }

    [TestCase(typeof(int), "int")]
    [TestCase(typeof(System.Guid), "System.Guid")]
    [TestCase(typeof(System.Guid?), "System.Guid?")]
    public void TestFormatTypeName_WithNamespace(Type type, string expected)
    {
      Assert.AreEqual(
        expected,
        type.FormatTypeName(null, typeWithNamespace: true)
      );
    }

    [TestCase(typeof(int), "int*")]
    [TestCase(typeof(long), "long*")]
    [TestCase(typeof(bool), "bool*")]
    [TestCase(typeof(void), "void*")]
    public void TestFormatTypeName_PrimitivePointerType(Type type, string expected)
    {
      Assert.AreEqual(
        expected,
        type.MakePointerType().FormatTypeName(null)
      );
    }

    [TestCase(typeof(int), "int&")]
    [TestCase(typeof(long), "long&")]
    [TestCase(typeof(bool), "bool&")]
    public void TestFormatTypeName_PrimitiveByRefType(Type type, string expected)
    {
      Assert.AreEqual(
        expected,
        type.MakeByRefType().FormatTypeName(null)
      );
    }
  }
}
