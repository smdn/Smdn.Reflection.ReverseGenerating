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
    => FormatParameterList(
      m.GetParameters(),
      typeWithNamespace,
      useDefaultLiteral
    );

  public static string FormatParameterList(
    ParameterInfo[] parameterList,
    bool typeWithNamespace = true,
    bool useDefaultLiteral = false
  )
    => string.Join(
      ", ",
      parameterList.Select(p => FormatParameter(p, typeWithNamespace, useDefaultLiteral))
    );

  public static string FormatParameter(
    ParameterInfo p,
    bool typeWithNamespace = true,
    bool useDefaultLiteral = false
  )
  {
    var ret = new StringBuilder(capacity: 64);

    if (p.ParameterType.IsByRef) {
      var typeAndName = string.Concat(
        p
          .ParameterType
          .GetElementTypeOrThrow()
          .FormatTypeName(attributeProvider: p, typeWithNamespace: typeWithNamespace),
        " ",
        ToVerbatim(p.Name)
      );

      if (p.IsIn)
        ret.Append("in ").Append(typeAndName);
      else if (p.IsOut)
        ret.Append("out ").Append(typeAndName);
      else
        ret.Append("ref ").Append(typeAndName);
    }
    else {
      var typeAndName = string.Concat(
        p.ParameterType.FormatTypeName(attributeProvider: p, typeWithNamespace: typeWithNamespace),
        " ",
        ToVerbatim(p.Name)
      );

      if (
        p.Position == 0 &&
        p.Member.GetCustomAttributesData().Any(
          static d => string.Equals(typeof(ExtensionAttribute).FullName, d.AttributeType.FullName, StringComparison.Ordinal)
        )
      ) {
        ret.Append("this ").Append(typeAndName);
      }
      else if (
        p.GetCustomAttributesData().Any(
          static d => string.Equals(typeof(ParamArrayAttribute).FullName, d.AttributeType.FullName, StringComparison.Ordinal)
        )
      ) {
        ret.Append("params ").Append(typeAndName);
      }
      else {
        ret.Append(typeAndName);
      }
    }

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

    static string? ToVerbatim(string? name)
    {
      if (name is null)
        return null;
      if (keywords.Contains(name))
        return "@" + name;

      return name;
    }
  }

  public static string FormatTypeName(
    this Type t,
    ICustomAttributeProvider? attributeProvider = null,
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    => FormatTypeName(
      t,
      showVariance: false,
      options: new FormatTypeNameOptions(
        attributeProvider: attributeProvider ?? t,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      )
    );

  private readonly /*ref*/ struct FormatTypeNameOptions {
    public readonly ICustomAttributeProvider AttributeProvider;
    public readonly bool TypeWithNamespace;
    public readonly bool WithDeclaringTypeName;
    public readonly bool TranslateLanguagePrimitiveType;

    public FormatTypeNameOptions(
      ICustomAttributeProvider attributeProvider,
      bool typeWithNamespace,
      bool withDeclaringTypeName,
      bool translateLanguagePrimitiveType
    )
    {
      this.AttributeProvider = attributeProvider;
      this.TypeWithNamespace = typeWithNamespace;
      this.WithDeclaringTypeName = withDeclaringTypeName;
      this.TranslateLanguagePrimitiveType = translateLanguagePrimitiveType;
    }
  }

  private static string FormatTypeName(
    Type t,
    bool showVariance,
    FormatTypeNameOptions options
  )
  {
    if (t.IsArray) {
      return string.Concat(
        FormatTypeName(t.GetElementTypeOrThrow(), showVariance: false, options),
        "[",
        new string(',', t.GetArrayRank() - 1),
        "]"
      );
    }

    if (t.IsByRef)
      return FormatTypeName(t.GetElementTypeOrThrow(), showVariance: false, options) + "&";

    if (t.IsPointer)
      return FormatTypeName(t.GetElementTypeOrThrow(), showVariance: false, options) + "*";

    var nullableUnderlyingType = Nullable.GetUnderlyingType(t);

    if (nullableUnderlyingType != null)
      return FormatTypeName(nullableUnderlyingType, showVariance: false, options) + "?";

    if (t.IsGenericParameter) {
      if (showVariance && t.ContainsGenericParameters) {
        var variance = t.GenericParameterAttributes & GenericParameterAttributes.VarianceMask;

        switch (variance) {
          case GenericParameterAttributes.Contravariant:
            return "in " + t.Name;
          case GenericParameterAttributes.Covariant:
            return "out " + t.Name;
        }
      }

      return t.Name;
    }

    if (t.IsGenericTypeDefinition || t.IsConstructedGenericType || (t.IsGenericType && t.ContainsGenericParameters)) {
      var sb = new StringBuilder();
      var isValueTuple =
        t.IsConstructedGenericType &&
        "System".Equals(t.Namespace, StringComparison.Ordinal) &&
        t.GetGenericTypeName().Equals("ValueTuple", StringComparison.Ordinal);

      if (isValueTuple) {
        var tupleItemNames = options
          .AttributeProvider
          ?.GetCustomAttributeDataList()
          ?.FirstOrDefault(static d =>
            string.Equals(
              typeof(TupleElementNamesAttribute).FullName,
              d.AttributeType.FullName,
              StringComparison.Ordinal
            )
          )
          ?.ConstructorArguments
          ?.FirstOrDefault()
          .Value
          as IReadOnlyList<CustomAttributeTypedArgument>;

        sb.Append('(')
          .Append(
            string.Join(
              ", ",
              t
                .GetGenericArguments()
                .Select((arg, index) => string.Concat(
                  FormatTypeName(arg, showVariance: true, options),
                  tupleItemNames is null ? null : " ", // append delimiter between type and name
                  tupleItemNames?[index].Value
                ))
            )
          )
          .Append(')');
      }
      else {
        if (options.TypeWithNamespace && !t.IsNested)
          sb.Append(t.Namespace).Append('.');

        IEnumerable<Type> genericArgs = t.GetGenericArguments();

        if (t.IsNested) {
          var declaringType = t.GetDeclaringTypeOrThrow();
          var genericArgsOfDeclaringType = declaringType.GetGenericArguments();

          if (options.WithDeclaringTypeName) {
            if (declaringType.IsGenericTypeDefinition)
              declaringType = declaringType.MakeGenericType(genericArgs.Take(genericArgsOfDeclaringType.Length).ToArray());

            sb.Append(FormatTypeName(declaringType, showVariance: true, options)).Append('.');
          }

          genericArgs = genericArgs.Skip(genericArgsOfDeclaringType.Length);
        }

        sb.Append(t.GetGenericTypeName());

        var formattedGenericArgs = string.Join(", ", genericArgs.Select(arg => FormatTypeName(arg, showVariance: true, options)));

        if (0 < formattedGenericArgs.Length)
          sb.Append('<').Append(formattedGenericArgs).Append('>');
      }

      return sb.ToString();
    }

    if (options.TranslateLanguagePrimitiveType && IsLanguagePrimitiveType(t, out var n))
      return n;

    if (options.WithDeclaringTypeName && t.IsNested)
      return FormatTypeName(t.GetDeclaringTypeOrThrow(), showVariance, options) + "." + t.Name;
    if (options.TypeWithNamespace)
      return t.Namespace + "." + t.Name;

    return t.Name;
  }

  public static string FormatTypeName(
    this FieldInfo f,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    NullabilityInfoContext context,
#endif
    bool typeWithNamespace = true,
    bool withDeclaringTypeName = true,
    bool translateLanguagePrimitiveType = true
  )
    =>
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      context is not null
        ? FormatTypeNameWithNullabilityAnnotation(
            context.Create(f ?? throw new ArgumentNullException(nameof(f))),
            builder: new(capacity: 32),
            options: new FormatTypeNameOptions(
              attributeProvider: f,
              typeWithNamespace: typeWithNamespace,
              withDeclaringTypeName: withDeclaringTypeName,
              translateLanguagePrimitiveType: translateLanguagePrimitiveType
            )
          ).ToString()
        :
#endif
      FormatTypeName(
        (f ?? throw new ArgumentNullException(nameof(f))).FieldType,
        attributeProvider: null,
        typeWithNamespace: typeWithNamespace,
        withDeclaringTypeName: withDeclaringTypeName,
        translateLanguagePrimitiveType: translateLanguagePrimitiveType
      );

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
