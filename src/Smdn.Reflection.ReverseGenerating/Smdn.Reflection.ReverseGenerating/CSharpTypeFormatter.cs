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

using Smdn.Reflection.Attributes;

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
    => primitiveTypes.TryGetValue(t.FullName ?? string.Empty, out primitiveTypeName);

  public static IEnumerable<string> ToNamespaceList(Type t)
    => t.GetNamespaces(static type => IsLanguagePrimitiveType(type, out _));

  public static string FormatSpecialNameMethod(
    MethodBase methodOrConstructor,
    out MethodSpecialName nameType
  )
  {
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
    => FormatParameterCore(
      p: p ?? throw new ArgumentNullException(nameof(p)),
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      nullabilityInfoContext: null,
#endif
      typeWithNamespace: typeWithNamespace,
      useDefaultLiteral: useDefaultLiteral
    );

#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
  public static string FormatParameter(
    ParameterInfo p,
    NullabilityInfoContext? nullabilityInfoContext,
    bool typeWithNamespace = true,
    bool useDefaultLiteral = false
  )
    => FormatParameterCore(
      p: p ?? throw new ArgumentNullException(nameof(p)),
      nullabilityInfoContext: nullabilityInfoContext,
      typeWithNamespace: typeWithNamespace,
      useDefaultLiteral: useDefaultLiteral
    );
#endif

  private static string FormatParameterCore(
    ParameterInfo p,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    NullabilityInfoContext? nullabilityInfoContext,
#endif
    bool typeWithNamespace = true,
    bool useDefaultLiteral = false
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
        withDeclaringTypeName: false
#pragma warning restore SA1114
      )
    );

    AppendName(ret, p);

    if (p.HasDefaultValue) {
      var defaultValueDeclaration = FormatValueDeclaration(
        p.GetDefaultValue(),
        p.ParameterType,
        typeWithNamespace: typeWithNamespace,
        findConstantField: true,
        useDefaultLiteral: useDefaultLiteral
      );

      ret.Append(" = ");
      ret.Append(defaultValueDeclaration);
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
    bool findConstantField = false,
    bool useDefaultLiteral = false
  )
  {
    if (val == null) {
      if (Nullable.GetUnderlyingType(typeOfValue) != null) {
        return "null";
      }
      else if (typeOfValue.IsValueType) {
        if (useDefaultLiteral)
          return "default";
        else
          return $"default({FormatTypeName(typeOfValue, typeWithNamespace: typeWithNamespace)})";
      }
      else {
        return "null";
      }
    }

    typeOfValue = Nullable.GetUnderlyingType(typeOfValue) ?? typeOfValue;

    if (string.Equals(typeOfValue.FullName, typeof(string).FullName, StringComparison.Ordinal)) {
      return "\"" + EscapeString((string)val, escapeDoubleQuote: true) + "\"";
    }
    else if (string.Equals(typeOfValue.FullName, typeof(char).FullName, StringComparison.Ordinal)) {
      return "\'" + EscapeString(((char)val).ToString(), escapeSingleQuote: true) + "\'";
    }
    else if (string.Equals(typeOfValue.FullName, typeof(bool).FullName, StringComparison.Ordinal)) {
      if ((bool)val)
        return "true";
      else
        return "false";
    }
    else {
      if (typeOfValue.IsValueType && findConstantField) {
        // try to find constant field
        foreach (var f in typeOfValue.GetFields(BindingFlags.Static | BindingFlags.Public)) {
          var isConstantField = f.IsLiteral || f.IsInitOnly;

          if (isConstantField && f.TryGetValue(null, out var constantFieldValue) && val.Equals(constantFieldValue))
            return FormatTypeName(typeOfValue, typeWithNamespace: typeWithNamespace) + "." + f.Name;
        }

        if (!typeOfValue.IsPrimitive && val.Equals(Activator.CreateInstance(typeOfValue))) {
          if (useDefaultLiteral)
            return "default";
          else
            return $"default({FormatTypeName(typeOfValue, typeWithNamespace: typeWithNamespace)})";
        }
      }

      if (typeOfValue.IsEnum)
        return $"({typeOfValue.FormatTypeName(typeWithNamespace: typeWithNamespace)}){Convert.ChangeType(val, typeOfValue.GetEnumUnderlyingType(), provider: null)}";

      if (typeOfValue.IsPrimitive && typeOfValue.IsValueType)
        return val.ToString() ?? string.Empty;

      return string.Empty;
    }
  }
}
