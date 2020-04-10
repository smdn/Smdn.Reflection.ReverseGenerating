using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  public class CNest1 {
    public class CNest2 {
      public class CNest3 {
      }
    }
  }

  [TestFixture]
  public class CSharpTypeFormatterTests {
    [TestCase(typeof(int), "int")]
    [TestCase(typeof(int[]), "int[]")]
    [TestCase(typeof(int[,]), "int[,]")]
    [TestCase(typeof(int[,,]), "int[,,]")]
    [TestCase(typeof(int[][]), "int[][]")]
    [TestCase(typeof(int[][][]), "int[][][]")]
    [TestCase(typeof(int?), "int?")]
    [TestCase(typeof(Nullable<int>), "int?")]
    [TestCase(typeof(Guid), "Guid")]
    [TestCase(typeof(Guid?), "Guid?")]
    [TestCase(typeof(Tuple<>), "Tuple<T1>")]
    [TestCase(typeof(Tuple<int>), "Tuple<int>")]
    [TestCase(typeof(Tuple<int[]>), "Tuple<int[]>")]
    [TestCase(typeof(Tuple<int[,]>), "Tuple<int[,]>")]
    [TestCase(typeof(Tuple<int[][]>), "Tuple<int[][]>")]
    [TestCase(typeof(Tuple<int?>), "Tuple<int?>")]
    [TestCase(typeof(Tuple<,>), "Tuple<T1, T2>")]
    [TestCase(typeof(Tuple<int, int>), "Tuple<int, int>")]
    [TestCase(typeof((int, int)), "(int, int)")]
    [TestCase(typeof((int x, int y)), "(int, int)")]
    [TestCase(typeof((int, int, int)), "(int, int, int)")]
    [TestCase(typeof((int, (int, int))), "(int, (int, int))")]
    [TestCase(typeof(ValueTuple<int>), "(int)")]
    [TestCase(typeof(ValueTuple<int, int>), "(int, int)")]
    [TestCase(typeof(ValueTuple<int, int, int>), "(int, int, int)")]
    [TestCase(typeof(ValueTuple<int, ValueTuple<int, int>>), "(int, (int, int))")]
    [TestCase(typeof(KeyValuePair<,>), "KeyValuePair<TKey, TValue>")]
    [TestCase(typeof(KeyValuePair<string, int>), "KeyValuePair<string, int>")]
    [TestCase(typeof(KeyValuePair<string, Guid>), "KeyValuePair<string, Guid>")]
    [TestCase(typeof(Tuple<Tuple<int>>), "Tuple<Tuple<int>>")]
    [TestCase(typeof(Tuple<Tuple<int>, Tuple<int>>), "Tuple<Tuple<int>, Tuple<int>>")]
    [TestCase(typeof(Converter<,>), "Converter<in TInput, out TOutput>")]
    [TestCase(typeof(Converter<int, string>), "Converter<int, string>")]
    [TestCase(typeof(Tuple<Converter<int, string>>), "Tuple<Converter<int, string>>")]
    [TestCase(typeof(KeyValuePair<Tuple<int, int?>, Tuple<int[], int[,]>>), "KeyValuePair<Tuple<int, int?>, Tuple<int[], int[,]>>")]
    public void TestFormatTypeName(Type type, string expected)
    {
      Assert.AreEqual(
        expected,
        type.FormatTypeName(null, typeWithNamespace: false)
      );
    }

    [TestCase(typeof(int), "int")]
    [TestCase(typeof(System.Guid), "Guid")]
    [TestCase(typeof(System.Guid?), "Guid?")]
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

    [TestCase(typeof(void), "void")]
    [TestCase(typeof(object), "object")]
    [TestCase(typeof(byte), "byte")]
    [TestCase(typeof(sbyte), "sbyte")]
    [TestCase(typeof(ushort), "ushort")]
    [TestCase(typeof(short), "short")]
    [TestCase(typeof(uint), "uint")]
    [TestCase(typeof(int), "int")]
    [TestCase(typeof(ulong), "ulong")]
    [TestCase(typeof(long), "long")]
    [TestCase(typeof(float), "float")]
    [TestCase(typeof(double), "double")]
    [TestCase(typeof(decimal), "decimal")]
    [TestCase(typeof(char), "char")]
    [TestCase(typeof(string), "string")]
    [TestCase(typeof(bool), "bool")]
    public void TestFormatTypeName_PrimitiveType(Type type, string expected)
    {
      Assert.AreEqual(
        expected,
        type.FormatTypeName(null)
      );
    }

    [TestCase(typeof(void), true, "System.Void")]
    [TestCase(typeof(void), false, "Void")]
    [TestCase(typeof(object), true, "System.Object")]
    [TestCase(typeof(object), false, "Object")]
    [TestCase(typeof(byte), true, "System.Byte")]
    [TestCase(typeof(byte), false, "Byte")]
    [TestCase(typeof(sbyte), true, "System.SByte")]
    [TestCase(typeof(sbyte), false, "SByte")]
    [TestCase(typeof(ushort), true, "System.UInt16")]
    [TestCase(typeof(ushort), false, "UInt16")]
    [TestCase(typeof(short), true, "System.Int16")]
    [TestCase(typeof(short), false, "Int16")]
    [TestCase(typeof(uint), true, "System.UInt32")]
    [TestCase(typeof(uint), false, "UInt32")]
    [TestCase(typeof(int), true, "System.Int32")]
    [TestCase(typeof(int), false, "Int32")]
    [TestCase(typeof(ulong), true, "System.UInt64")]
    [TestCase(typeof(ulong), false, "UInt64")]
    [TestCase(typeof(long), true, "System.Int64")]
    [TestCase(typeof(long), false, "Int64")]
    [TestCase(typeof(float), true, "System.Single")]
    [TestCase(typeof(float), false, "Single")]
    [TestCase(typeof(double), true, "System.Double")]
    [TestCase(typeof(double), false, "Double")]
    [TestCase(typeof(decimal), true, "System.Decimal")]
    [TestCase(typeof(decimal), false, "Decimal")]
    [TestCase(typeof(char), true, "System.Char")]
    [TestCase(typeof(char), false, "Char")]
    [TestCase(typeof(string), true, "System.String")]
    [TestCase(typeof(string), false, "String")]
    [TestCase(typeof(bool), true, "System.Boolean")]
    [TestCase(typeof(bool), false, "Boolean")]
    public void TestFormatTypeName_NotTranslateLanguagePrimitiveType_PrimitiveType(
      Type type,
      bool withNamespace,
      string expected
    )
    {
      Assert.AreEqual(
        expected,
        type.FormatTypeName(
          null,
          typeWithNamespace: withNamespace,
          translateLanguagePrimitiveType: false
        )
      );
    }

    [TestCase(typeof(int), "int*")]
    [TestCase(typeof(long), "long*")]
    [TestCase(typeof(bool), "bool*")]
    [TestCase(typeof(void), "void*")]
    [TestCase(typeof(Guid), "System.Guid*")]
    [TestCase(typeof(KeyValuePair<int, int>), "System.Collections.Generic.KeyValuePair<int, int>*")]
    public void TestFormatTypeName_PointerType(Type type, string expected)
    {
      Assert.AreEqual(
        expected,
        type.MakePointerType().FormatTypeName(null)
      );
    }

    [TestCase(typeof(int), true, "System.Int32*")]
    [TestCase(typeof(int), false, "Int32*")]
    [TestCase(typeof(long), true, "System.Int64*")]
    [TestCase(typeof(long), false, "Int64*")]
    [TestCase(typeof(bool), true, "System.Boolean*")]
    [TestCase(typeof(bool), false, "Boolean*")]
    [TestCase(typeof(void), true, "System.Void*")]
    [TestCase(typeof(void), false, "Void*")]
    public void TestFormatTypeName_NotTranslateLanguagePrimitiveType_PointerType(
      Type type,
      bool withNamespace,
      string expected
    )
    {
      Assert.AreEqual(
        expected,
        type.MakePointerType().FormatTypeName(
          null,
          typeWithNamespace: withNamespace,
          translateLanguagePrimitiveType: false
        )
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

    [TestCase(typeof(int), true, "System.Int32&")]
    [TestCase(typeof(int), false, "Int32&")]
    [TestCase(typeof(long), true, "System.Int64&")]
    [TestCase(typeof(long), false, "Int64&")]
    [TestCase(typeof(bool), true, "System.Boolean&")]
    [TestCase(typeof(bool), false, "Boolean&")]
    public void TestFormatTypeName_NotTranslateLanguagePrimitiveType_PrimitiveByRefType(
      Type type,
      bool withNamespace,
      string expected
    )
    {
      Assert.AreEqual(
        expected,
        type.MakeByRefType().FormatTypeName(
          null,
          typeWithNamespace: withNamespace,
          translateLanguagePrimitiveType: false
        )
      );
    }

    [TestCase(typeof(Environment.SpecialFolder), "Environment.SpecialFolder")]
    [TestCase(typeof(CNest1), "CNest1")]
    [TestCase(typeof(CNest1.CNest2), "CNest1.CNest2")]
    [TestCase(typeof(CNest1.CNest2.CNest3), "CNest1.CNest2.CNest3")]
    public void TestFormatTypeName_NestedType(Type type, string expected)
    {
      Assert.AreEqual(
        expected,
        type.FormatTypeName(null, typeWithNamespace: false)
      );
    }
  }
}
