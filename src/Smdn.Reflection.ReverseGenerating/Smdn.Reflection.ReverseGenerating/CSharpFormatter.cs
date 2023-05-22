// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Smdn.Reflection.ReverseGenerating;

public static partial class CSharpFormatter /* ITypeFormatter */ {
  private static readonly Dictionary<Accessibility, string> accessibilities = new() {
    { Accessibility.Public,            "public" },
    { Accessibility.Assembly,          "internal" },
    { Accessibility.Family,            "protected" },
    { Accessibility.FamilyOrAssembly,  "internal protected" },
    { Accessibility.FamilyAndAssembly, "private protected" },
    { Accessibility.Private,           "private" },
  };

  private static readonly Dictionary<string, string> primitiveTypes = new(StringComparer.Ordinal) {
    { typeof(void).FullName!, "void" },
    { typeof(sbyte).FullName!, "sbyte" },
    { typeof(short).FullName!, "short" },
    { typeof(int).FullName!, "int" },
    { typeof(long).FullName!, "long" },
    { typeof(byte).FullName!, "byte" },
    { typeof(ushort).FullName!, "ushort" },
    { typeof(uint).FullName!, "uint" },
    { typeof(ulong).FullName!, "ulong" },
    { typeof(float).FullName!, "float" },
    { typeof(double).FullName!, "double" },
    { typeof(decimal).FullName!, "decimal" },
    { typeof(char).FullName!, "char" },
    { typeof(string).FullName!, "string" },
    { typeof(object).FullName!, "object" },
    { typeof(bool).FullName!, "bool" },
  };

  private static readonly HashSet<string> keywords = new(StringComparer.Ordinal) {
    "abstract",
    "as",
    "base",
    "bool",
    "break",
    "byte",
    "case",
    "catch",
    "char",
    "checked",
    "class",
    "const",
    "continue",
    "decimal",
    "default",
    "delegate",
    "do",
    "double",
    "else",
    "enum",
    "event",
    "explicit",
    "extern",
    "false",
    "finally",
    "fixed",
    "float",
    "for",
    "foreach",
    "goto",
    "if",
    "implicit",
    "in",
    "int",
    "interface",
    "internal",
    "is",
    "lock",
    "long",
    "namespace",
    "new",
    "null",
    "object",
    "operator",
    "out",
    "override",
    "params",
    "private",
    "protected",
    "public",
    "readonly",
    "ref",
    "return",
    "sbyte",
    "sealed",
    "short",
    "sizeof",
    "stackalloc",
    "static",
    "string",
    "struct",
    "switch",
    "this",
    "throw",
    "true",
    "try",
    "typeof",
    "uint",
    "ulong",
    "unchecked",
    "unsafe",
    "ushort",
    "using",
    "virtual",
    "void",
    "volatile",
    "while",

    // context keywords
    "add",
    "async",
    "await",
    "dynamic",
    "get",
    "global",
    "partial",
    "remove",
    "set",
    "value",
    "var",
    "when",
    "where",
    "yield",
  };

  private static readonly Dictionary<MethodSpecialName, string> specialMethodNames = new() {
    // comparison
    { MethodSpecialName.Equality, "operator ==" },
    { MethodSpecialName.Inequality, "operator !=" },
    { MethodSpecialName.LessThan, "operator <" },
    { MethodSpecialName.GreaterThan, "operator >" },
    { MethodSpecialName.LessThanOrEqual, "operator <=" },
    { MethodSpecialName.GreaterThanOrEqual, "operator >=" },

    // unary
    { MethodSpecialName.UnaryPlus, "operator +" },
    { MethodSpecialName.UnaryNegation, "operator -" },
    { MethodSpecialName.LogicalNot, "operator !" },
    { MethodSpecialName.OnesComplement, "operator ~" },
    { MethodSpecialName.True, "operator true" },
    { MethodSpecialName.False, "operator false" },
    { MethodSpecialName.Increment, "operator ++" },
    { MethodSpecialName.Decrement, "operator --" },

    // binary
    { MethodSpecialName.Addition, "operator +" },
    { MethodSpecialName.Subtraction, "operator -" },
    { MethodSpecialName.Multiply, "operator *" },
    { MethodSpecialName.Division, "operator /" },
    { MethodSpecialName.Modulus, "operator %" },
    { MethodSpecialName.BitwiseAnd, "operator &" },
    { MethodSpecialName.BitwiseOr, "operator |" },
    { MethodSpecialName.ExclusiveOr, "operator ^" },
    { MethodSpecialName.RightShift, "operator >>" },
    { MethodSpecialName.LeftShift, "operator <<" },

    // type cast
    { MethodSpecialName.Explicit, "explicit operator" },
    { MethodSpecialName.Implicit, "implicit operator" },
  };

  public static string FormatAccessibility(Accessibility accessibility)
    => accessibilities.TryGetValue(accessibility, out var ret) ? ret : string.Empty;

  public static bool IsLanguagePrimitiveType(
    Type t,
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
    [MaybeNullWhen(false)]
#endif
    out string primitiveTypeName
  )
    => primitiveTypes.TryGetValue((t ?? throw new ArgumentNullException(nameof(t))).FullName ?? string.Empty, out primitiveTypeName);

  public static IEnumerable<string> ToNamespaceList(Type t)
    => (t ?? throw new ArgumentNullException(nameof(t))).GetNamespaces(static type => IsLanguagePrimitiveType(type, out _));

  public static string FormatSpecialNameMethod(
    MethodBase methodOrConstructor,
    out MethodSpecialName nameType
  )
  {
    if (methodOrConstructor is null)
      throw new ArgumentNullException(nameof(methodOrConstructor));

    nameType = methodOrConstructor.GetNameType();

    if (specialMethodNames.TryGetValue(nameType, out var name))
      return name;

    if (nameType == MethodSpecialName.Constructor) {
      var declaringType = methodOrConstructor.GetDeclaringTypeOrThrow();

      return declaringType.IsGenericType
        ? declaringType.GetGenericTypeName()
        : declaringType.Name;
    }

    return methodOrConstructor.Name; // as default
  }

  public static string FormatParameterList(
    MethodBase m,
    bool typeWithNamespace = true,
    bool useDefaultLiteral = false
  )
    => FormatParameterListCore(
      parameterList: (m ?? throw new ArgumentNullException(nameof(m))).GetParameters(),
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      nullabilityInfoContext: null,
#endif
      typeWithNamespace: typeWithNamespace,
      useDefaultLiteral: useDefaultLiteral
    );

#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
  public static string FormatParameterList(
    MethodBase m,
    NullabilityInfoContext? nullabilityInfoContext,
    bool typeWithNamespace = true,
    bool useDefaultLiteral = false
  )
    => FormatParameterListCore(
      parameterList: (m ?? throw new ArgumentNullException(nameof(m))).GetParameters(),
      nullabilityInfoContext: nullabilityInfoContext,
      typeWithNamespace: typeWithNamespace,
      useDefaultLiteral: useDefaultLiteral
    );
#endif

  public static string FormatParameterList(
    ParameterInfo[] parameterList,
    bool typeWithNamespace = true,
    bool useDefaultLiteral = false
  )
    => FormatParameterListCore(
      parameterList: parameterList ?? throw new ArgumentNullException(nameof(parameterList)),
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      nullabilityInfoContext: null,
#endif
      typeWithNamespace: typeWithNamespace,
      useDefaultLiteral: useDefaultLiteral
    );

#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
  public static string FormatParameterList(
    ParameterInfo[] parameterList,
    NullabilityInfoContext? nullabilityInfoContext,
    bool typeWithNamespace = true,
    bool useDefaultLiteral = false
  )
    => FormatParameterListCore(
      parameterList: parameterList ?? throw new ArgumentNullException(nameof(parameterList)),
      nullabilityInfoContext: nullabilityInfoContext,
      typeWithNamespace: typeWithNamespace,
      useDefaultLiteral: useDefaultLiteral
    );
#endif

  private static string FormatParameterListCore(
    ParameterInfo[] parameterList,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    NullabilityInfoContext? nullabilityInfoContext,
#endif
    bool typeWithNamespace = true,
    bool useDefaultLiteral = false
  )
    => string.Join(
      ", ",
      parameterList.Select(
        p => FormatParameter(
          p,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
          nullabilityInfoContext: nullabilityInfoContext,
#endif
          typeWithNamespace: typeWithNamespace,
          useDefaultLiteral: useDefaultLiteral
        )
      )
    );

  public static string FormatParameter(
    ParameterInfo p,
    bool typeWithNamespace = true,
    bool useDefaultLiteral = false
  )
    => FormatParameter(
      p: p ?? throw new ArgumentNullException(nameof(p)),
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      nullabilityInfoContext: null,
#endif
      typeWithNamespace: typeWithNamespace,
      typeWithDeclaringTypeName: true,
      valueFormatOptions: new(
        TranslateLanguagePrimitiveType: true,
        TryFindConstantField: true,
        UseDefaultLiteral: useDefaultLiteral,
        WithNamespace: typeWithNamespace,
        WithDeclaringTypeName: true
      )
    );

#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
  public static string FormatParameter(
    ParameterInfo p,
    NullabilityInfoContext? nullabilityInfoContext,
    bool typeWithNamespace = true,
    bool useDefaultLiteral = false
  )
    => FormatParameter(
      p: p ?? throw new ArgumentNullException(nameof(p)),
      nullabilityInfoContext: nullabilityInfoContext,
      typeWithNamespace: typeWithNamespace,
      typeWithDeclaringTypeName: true,
      valueFormatOptions: new(
        TranslateLanguagePrimitiveType: true,
        TryFindConstantField: true,
        UseDefaultLiteral: useDefaultLiteral,
        WithNamespace: typeWithNamespace,
        WithDeclaringTypeName: true
      )
    );
#endif

  internal static string FormatParameter(
    ParameterInfo p,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    NullabilityInfoContext? nullabilityInfoContext,
#endif
    bool typeWithNamespace,
    bool typeWithDeclaringTypeName,
    ValueFormatOptions valueFormatOptions
  )
  {
    var ret = new StringBuilder(capacity: 64);

    if (
      p.Position == 0 &&
      p.Member.GetCustomAttributesData().Any(
        static d => string.Equals(typeof(ExtensionAttribute).FullName, d.AttributeType.FullName, StringComparison.Ordinal)
      )
    ) {
      ret.Append("this ");
    }
    else if (
      p.GetCustomAttributesData().Any(
        static d => string.Equals(typeof(ParamArrayAttribute).FullName, d.AttributeType.FullName, StringComparison.Ordinal)
      )
    ) {
      ret.Append("params ");
    }

    ret.Append(
      p.FormatTypeName(
#pragma warning disable SA1114
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
        nullabilityInfoContext: nullabilityInfoContext,
#endif
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: typeWithDeclaringTypeName
#pragma warning restore SA1114
      )
    );

    AppendName(ret, p);

    if (p.HasDefaultValue) {
      ret.Append(" = ");
      ret.Append(
        FormatValueDeclaration(
          val: p.GetDefaultValue(),
          typeOfValue: p.ParameterType,
          options: valueFormatOptions
        )
      );
    }

    return ret.ToString();

    static StringBuilder AppendName(StringBuilder sb, ParameterInfo p)
    {
      if (p.Name is null)
        return sb;

      sb.Append(' ');

      if (keywords.Contains(p.Name))
        sb.Append('@'); // to verbatim

      return sb.Append(p.Name);
    }
  }

  public static string EscapeString(
    string s,
    bool escapeSingleQuote = false,
    bool escapeDoubleQuote = false
  )
  {
    if (s == null)
      throw new ArgumentNullException(nameof(s));

    var chars = s.ToCharArray();
    var buf = new StringBuilder(chars.Length);

    for (var i = 0; i < chars.Length; i++) {
      if (char.IsControl(chars[i]))
        buf.Append("\\u").Append(((int)chars[i]).ToString("X4", provider: null));
      else if (escapeSingleQuote && chars[i] == '\'')
        buf.Append("\\\'");
      else if (escapeDoubleQuote && chars[i] == '"')
        buf.Append("\\\"");
      else
        buf.Append(chars[i]);
    }

    return buf.ToString();
  }

  public static string FormatValueDeclaration(
    object? val,
    Type typeOfValue,
    bool typeWithNamespace = true,
    bool findConstantField = false, // TODO: rename parameter
    bool useDefaultLiteral = false
  )
    => FormatValueDeclaration(
      val: val,
      typeOfValue: typeOfValue ?? throw new ArgumentNullException(nameof(typeOfValue)),
      options: new(
        TranslateLanguagePrimitiveType: true,
        TryFindConstantField: findConstantField,
        UseDefaultLiteral: useDefaultLiteral,
        WithNamespace: typeWithNamespace,
        WithDeclaringTypeName: true
      )
    );

  internal readonly record struct ValueFormatOptions(
#pragma warning disable SA1313
    bool TranslateLanguagePrimitiveType,
    bool TryFindConstantField,
    bool UseDefaultLiteral,
    bool WithNamespace,
    bool WithDeclaringTypeName
#pragma warning restore SA1313
  ) {
    public static ValueFormatOptions FromGeneratorOptions(GeneratorOptions options, bool tryFindConstantField)
      => new(
        TranslateLanguagePrimitiveType: options.TranslateLanguagePrimitiveTypeDeclaration,
        TryFindConstantField: tryFindConstantField,
        UseDefaultLiteral: options.ValueDeclaration.UseDefaultLiteral,
        WithNamespace: options.ValueDeclaration.WithNamespace,
        WithDeclaringTypeName: options.ValueDeclaration.WithDeclaringTypeName
      );
  }

  internal static string FormatValueDeclaration(
    object? val,
    Type typeOfValue,
    ValueFormatOptions options
  )
  {
    static string ToString(Type t, ValueFormatOptions opts)
      => FormatTypeName(
        t,
        typeWithNamespace: opts.WithNamespace,
        withDeclaringTypeName: opts.WithDeclaringTypeName,
        translateLanguagePrimitiveType: opts.TranslateLanguagePrimitiveType
      );

    static string ToDefaultValue(Type t, ValueFormatOptions opts)
      => opts.UseDefaultLiteral
        ? "default"
        : "default(" + ToString(t, opts) + ")";

    if (val == null) {
      return (typeOfValue.IsValueType && Nullable.GetUnderlyingType(typeOfValue) is null)
        ? ToDefaultValue(typeOfValue, options)
        : "null";
    }

    if (string.Equals(typeOfValue.FullName, typeof(string).FullName, StringComparison.Ordinal))
      // System.String
      return "\"" + EscapeString((string)val, escapeDoubleQuote: true) + "\"";
    else if (string.Equals(typeOfValue.FullName, typeof(Type).FullName, StringComparison.Ordinal))
      // System.Type
      return "typeof(" + ToString((Type)val, options) + ")";
#if NETFRAMEWORK
    else if (val is Type t)
      // System.Type
      return "typeof(" + ToString(t, options) + ")";
#endif

    typeOfValue = Nullable.GetUnderlyingType(typeOfValue) ?? typeOfValue;

    if (string.Equals(typeOfValue.FullName, typeof(bool).FullName, StringComparison.Ordinal)) {
      // System.Boolean
      return (bool)val ? "true" : "false";
    }
    else if (string.Equals(typeOfValue.FullName, typeof(char).FullName, StringComparison.Ordinal)) {
      // System.Char
      return "\'" + EscapeString(((char)val).ToString(), escapeSingleQuote: true) + "\'";
    }
    else if (typeOfValue.IsEnum || (typeOfValue.IsValueType && options.TryFindConstantField)) {
      // try to find constant field
      foreach (var f in typeOfValue.GetFields(BindingFlags.Static | BindingFlags.Public)) {
        var isConstantField = f.IsLiteral || f.IsInitOnly;

        if (isConstantField && f.TryGetValue(null, out var constantFieldValue) && val.Equals(constantFieldValue))
          return ToString(typeOfValue, options) + "." + f.Name;
      }

      if (!typeOfValue.IsPrimitive && val.Equals(Activator.CreateInstance(typeOfValue)))
        // format as 'default'
        return ToDefaultValue(typeOfValue, options);
    }

    if (typeOfValue.IsEnum)
      return "(" + ToString(typeOfValue, options) + ")" + Convert.ChangeType(val, typeOfValue.GetEnumUnderlyingType(), provider: null);

    if (typeOfValue.IsArray)
      return FormatArrayValueDeclaration(val, typeOfValue, options);

    if (typeOfValue.IsPrimitive && typeOfValue.IsValueType)
      return val.ToString() ?? string.Empty;

    return string.Empty;

    static string FormatArrayValueDeclaration(
      object arr,
      Type typeOfArray,
      ValueFormatOptions opts
    )
    {
      if (arr is not System.Collections.IEnumerable enumerable)
        return string.Empty;

      var content = new StringBuilder(capacity: 32);
      var elementType = typeOfArray.GetElementType() ?? typeof(object);

      foreach (object e in enumerable) {
        if (0 < content.Length)
          content.Append(", ");

        var (v, t) = e switch {
          CustomAttributeTypedArgument typedArg => (typedArg.Value, typedArg.ArgumentType),
          _ => (e, e?.GetType() ?? elementType),
        };

        content.Append(FormatValueDeclaration(v, t, opts));
      }

      if (content.Length == 0)
        return "new " + ToString(elementType, opts) + "[0]";

      return string.Concat(
        "new ",
        ToString(elementType, opts),
        "[] { ",
        content.ToString(),
        " }"
      );
    }
  }
}
