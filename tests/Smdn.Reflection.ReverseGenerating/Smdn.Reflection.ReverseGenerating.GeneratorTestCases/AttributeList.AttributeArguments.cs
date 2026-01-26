// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.AttributeList.AttributeArguments;

public class TypeValueAttribute : Attribute {
  public Type Value { get; }
  public TypeValueAttribute(Type value)
  {
    this.Value = value;
  }
}

class TypeArgument {
  [AttributeListTestCase("[TypeValue(typeof(System.Guid))]", AttributeWithNamespace = false, ValueWithNamespace = true)]
  [AttributeListTestCase("[TypeValue(typeof(Guid))]", AttributeWithNamespace = false, ValueWithNamespace = false)]
  [TypeValue(typeof(Guid))]
  public int Value = 0;

  [AttributeListTestCase("[TypeValue(typeof(System.Environment.SpecialFolder))]", AttributeWithNamespace = false, ValueWithNamespace = true, ValueWithDeclaringTypeName = true)]
  [AttributeListTestCase("[TypeValue(typeof(Environment.SpecialFolder))]", AttributeWithNamespace = false, ValueWithNamespace = false, ValueWithDeclaringTypeName = true)]
  [AttributeListTestCase("[TypeValue(typeof(System.SpecialFolder))]", AttributeWithNamespace = false, ValueWithNamespace = true, ValueWithDeclaringTypeName = false)]
  [AttributeListTestCase("[TypeValue(typeof(SpecialFolder))]", AttributeWithNamespace = false, ValueWithNamespace = false, ValueWithDeclaringTypeName = false)]
  [TypeValue(typeof(Environment.SpecialFolder))]
  public int ValueNestedType = 0;

  [AttributeListTestCase("[TypeValue(typeof(int))]", AttributeWithNamespace = false, ValueWithNamespace = false, TranslateLanguagePrimitiveTypeDeclaration = true)]
  [AttributeListTestCase("[TypeValue(typeof(Int32))]", AttributeWithNamespace = false, ValueWithNamespace = false, TranslateLanguagePrimitiveTypeDeclaration = false)]
  [AttributeListTestCase("[TypeValue(typeof(System.Int32))]", AttributeWithNamespace = false, ValueWithNamespace = true, TranslateLanguagePrimitiveTypeDeclaration = false)]
  [TypeValue(typeof(int))]
  public int ValueLanguagePrimitive = 0;

  [AttributeListTestCase("[TypeValue(typeof(System.Tuple<int, System.Guid>))]", AttributeWithNamespace = false, ValueWithNamespace = true, TranslateLanguagePrimitiveTypeDeclaration = true)]
  [AttributeListTestCase("[TypeValue(typeof(System.Tuple<System.Int32, System.Guid>))]", AttributeWithNamespace = false, ValueWithNamespace = true, TranslateLanguagePrimitiveTypeDeclaration = false)]
  [AttributeListTestCase("[TypeValue(typeof(Tuple<int, Guid>))]", AttributeWithNamespace = false, ValueWithNamespace = false, TranslateLanguagePrimitiveTypeDeclaration = true)]
  [TypeValue(typeof(Tuple<int, Guid>))]
  public int ValueGenericType = 0;
}

public class ObjectValueAttribute : Attribute {
  public object Value { get; }
  public ObjectValueAttribute(object value)
  {
    this.Value = value;
  }
}

class ObjectArgument {
  [AttributeListTestCase("[ObjectValue(null)]", AttributeWithNamespace = false)]
  [ObjectValue(null)]
  public int NullValue = 0;

  [AttributeListTestCase("[ObjectValue('a')]", AttributeWithNamespace = false)]
  [ObjectValue('a')]
  public int CharValue = 0;

  [AttributeListTestCase(@"[ObjectValue('\\')]", AttributeWithNamespace = false)]
  [ObjectValue('\\')]
  public int CharValueBackslash = 0;

  [AttributeListTestCase(@"[ObjectValue('\'')]", AttributeWithNamespace = false)]
  [ObjectValue('\'')]
  public int CharValueSingleQuote = 0;

  [AttributeListTestCase(@"[ObjectValue(""str"")]", AttributeWithNamespace = false)]
  [ObjectValue("str")]
  public int StringValue = 0;

  [AttributeListTestCase(@"[ObjectValue(""\u000A"")]", AttributeWithNamespace = false)]
  [ObjectValue("\n")]
  public int StringValueContainsEscapeSequence = 0;

  [AttributeListTestCase(@"[ObjectValue(""abc\u0000"")]", AttributeWithNamespace = false)]
  [ObjectValue("abc\0")]
  public int StringValueContainsEscapeSequenceNull = 0;

  [AttributeListTestCase(@"[ObjectValue(""\\"")]", AttributeWithNamespace = false)]
  [ObjectValue("\\")]
  public int StringValueContainsEscapeSequenceBackslash = 0;

  [AttributeListTestCase(@"[ObjectValue(""\u000A"")]", AttributeWithNamespace = false)]
  [ObjectValue("\x0a")]
  public int StringValueContainsEscapeSequenceAsciiHexadecimalNotationControlChar = 0;

  [AttributeListTestCase(@"[ObjectValue(""a"")]", AttributeWithNamespace = false)]
  [ObjectValue("\x61")]
  public int StringValueContainsEscapeSequenceAsciiHexadecimalNotationPrintableChar = 0;

  [AttributeListTestCase(@"[ObjectValue(""あ"")]", AttributeWithNamespace = false)]
  [ObjectValue("\u3042")]
  public int StringValueContainsEscapeSequenceUnicodeCharacterHexadecimalNotation = 0;

  [AttributeListTestCase(@"[ObjectValue(""'hello'"")]", AttributeWithNamespace = false)]
  [ObjectValue(@"'hello'")]
  public int StringValueContainsSingleQuote = 0;

  [AttributeListTestCase(@"[ObjectValue(""\""hello\"""")]", AttributeWithNamespace = false)]
  [ObjectValue(@"""hello""")]
  public int StringValueContainsDoubleQuote = 0;

  [AttributeListTestCase("[ObjectValue(0)]", AttributeWithNamespace = false)]
  [ObjectValue((byte)0)]
  public int ByteValue = 0;

  [AttributeListTestCase("[ObjectValue(0)]", AttributeWithNamespace = false)]
  [ObjectValue((int)0)]
  public int IntValue = 0;

  [AttributeListTestCase("[ObjectValue(0)]", AttributeWithNamespace = false)]
  [ObjectValue((double)0.0)]
  public int DoubleValue = 0;

  [AttributeListTestCase("[ObjectValue(DayOfWeek.Sunday)]", AttributeWithNamespace = false, ValueWithNamespace = false)]
  [AttributeListTestCase("[ObjectValue(System.DayOfWeek.Sunday)]", AttributeWithNamespace = false, ValueWithNamespace = true)]
  [ObjectValue(DayOfWeek.Sunday)]
  public int EnumValue = 0;

  [AttributeListTestCase("[ObjectValue((DayOfWeek)999)]", AttributeWithNamespace = false, ValueWithNamespace = false)]
  [AttributeListTestCase("[ObjectValue((System.DayOfWeek)999)]", AttributeWithNamespace = false, ValueWithNamespace = true)]
  [ObjectValue((DayOfWeek)999)]
  public int EnumValueUndefined = 0;

  [AttributeListTestCase("[ObjectValue(typeof(System.Guid))]", AttributeWithNamespace = false, ValueWithNamespace = true)]
  [AttributeListTestCase("[ObjectValue(typeof(Guid))]", AttributeWithNamespace = false, ValueWithNamespace = false)]
  [ObjectValue(typeof(Guid))]
  public int TypeValue = 0;

  [AttributeListTestCase("[ObjectValue(typeof(int))]", AttributeWithNamespace = false, ValueWithNamespace = false, TranslateLanguagePrimitiveTypeDeclaration = true)]
  [AttributeListTestCase("[ObjectValue(typeof(Int32))]", AttributeWithNamespace = false, ValueWithNamespace = false, TranslateLanguagePrimitiveTypeDeclaration = false)]
  [AttributeListTestCase("[ObjectValue(typeof(System.Int32))]", AttributeWithNamespace = false, ValueWithNamespace = true, TranslateLanguagePrimitiveTypeDeclaration = false)]
  [ObjectValue(typeof(int))]
  public int TypeValueLanguagePrimitive = 0;

  [AttributeListTestCase("[ObjectValue(typeof(System.Tuple<int, System.Guid>))]", AttributeWithNamespace = false, ValueWithNamespace = true, TranslateLanguagePrimitiveTypeDeclaration = true)]
  [AttributeListTestCase("[ObjectValue(typeof(System.Tuple<System.Int32, System.Guid>))]", AttributeWithNamespace = false, ValueWithNamespace = true, TranslateLanguagePrimitiveTypeDeclaration = false)]
  [AttributeListTestCase("[ObjectValue(typeof(Tuple<int, Guid>))]", AttributeWithNamespace = false, ValueWithNamespace = false, TranslateLanguagePrimitiveTypeDeclaration = true)]
  [ObjectValue(typeof(Tuple<int, Guid>))]
  public int TypeValueGenericType = 0;
}

public class ParamArray {
  public class ObjectParamArrayArgumentAttribute : Attribute {
    public object[] Values { get; }
    public ObjectParamArrayArgumentAttribute(params object[] values)
    {
      this.Values = values;
    }
  }

  [AttributeListTestCase(
    "[ObjectParamArrayArgument(new object[] { 0, 1, 2 })]",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false
  )]
  [ObjectParamArrayArgument(0, 1, 2)]
  public void ParamArrayValue() { }

  [AttributeListTestCase(
    "[ObjectParamArrayArgument(new object[] { new int[] { 0 }, new string[] { \"foo\", \"bar\" }, new bool[0] })]",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false
  )]
  [ObjectParamArrayArgument(new int[] { 0 }, new string[] { "foo", "bar" }, new bool[0])]
  public void ParamArrayOfArrays() { }

  [AttributeListTestCase(
    "[ObjectParamArrayArgument(new object[] { 42, 3.14, \"str\", typeof(int) })]",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false
  )]
  [ObjectParamArrayArgument(42, 3.14, "str", typeof(int))]
  public void MixedTypeParamArrayValue() { }

  [AttributeListTestCase(
    "[ObjectParamArrayArgument(new object[0])]",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false
  )]
  [ObjectParamArrayArgument()]
  public void EmptyArrayValue() { }

  public class ParamArrayNamedArgumentAttribute : Attribute {
    public string Value { get; }
    public int[] Values { get; set; }
    public ParamArrayNamedArgumentAttribute(string value, params int[] values)
    {
      this.Value = value;
      this.Values = values;
    }
  }

  [AttributeListTestCase(
    "[ParamArrayNamedArgument(\"str\", new int[] { 0, 1, 2 })]",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeWithNamedArguments = false
  )]
  [AttributeListTestCase(
    "[ParamArrayNamedArgument(value: \"str\", values: new int[] { 0, 1, 2 })]",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeWithNamedArguments = true
  )]
  [ParamArrayNamedArgument("str", 0, 1, 2)]
  public void NamedParamArray_CtorParamArray() { }

  [AttributeListTestCase(
    "[ParamArrayNamedArgument(\"str\", new int[] { 0, 1, 2 })]",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false
  )]
  [ParamArrayNamedArgument("str", new[] { 0, 1, 2 })]
  public void NamedParamArray_CtorArray() { }

  [AttributeListTestCase(
    "[ParamArrayNamedArgument(\"str\", new int[0], Values = new int[] { 0, 1, 2 })]",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false
  )]
  [ParamArrayNamedArgument("str", Values = new[] { 0, 1, 2 })]
  public void NamedParamArray_NamedArgument() { }

  [AttributeListTestCase(
    "[ParamArrayNamedArgument(\"str\", new int[0])]",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeWithNamedArguments = false
  )]
  [AttributeListTestCase(
    "[ParamArrayNamedArgument(value: \"str\", values: new int[0])]",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeWithNamedArguments = true
  )]
  [ParamArrayNamedArgument("str")]
  public void NamedParamArray_Empty_CtorParamArray() { }

  [AttributeListTestCase(
    "[ParamArrayNamedArgument(\"str\", new int[0], Values = new int[0])]",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeWithNamedArguments = false
  )]
  [AttributeListTestCase(
    "[ParamArrayNamedArgument(value: \"str\", values: new int[0], Values = new int[0])]",
    AttributeWithNamespace = false,
    AttributeWithDeclaringTypeName = false,
    AttributeWithNamedArguments = true
  )]
  [ParamArrayNamedArgument("str", Values = new int[] { })]
  public void NamedParamArray_Empty_NamedArgument() { }
}
