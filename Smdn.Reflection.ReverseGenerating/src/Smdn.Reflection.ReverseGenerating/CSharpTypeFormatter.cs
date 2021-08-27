// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Smdn.Reflection;

namespace Smdn.Reflection.ReverseGenerating {
  public static class CSharpFormatter /* ITypeFormatter */ {
    private static readonly Dictionary<Accessibility, string> accessibilities = new Dictionary<Accessibility, string>() {
      {Accessibility.Public,            "public"},
      {Accessibility.Assembly,          "internal"},
      {Accessibility.Family,            "protected"},
      {Accessibility.FamilyOrAssembly,  "internal protected"},
      {Accessibility.FamilyAndAssembly, "private protected"},
      {Accessibility.Private,           "private"},
    };

    private static readonly Dictionary<Type, string> primitiveTypes = new Dictionary<Type, string>() {
      {typeof(void), "void"},

      {typeof(sbyte), "sbyte"},
      {typeof(short), "short"},
      {typeof(int), "int"},
      {typeof(long), "long"},

      {typeof(byte), "byte"},
      {typeof(ushort), "ushort"},
      {typeof(uint), "uint"},
      {typeof(ulong), "ulong"},

      {typeof(float), "float"},
      {typeof(double), "double"},
      {typeof(decimal), "decimal"},

      {typeof(char), "char"},
      {typeof(string), "string"},

      {typeof(object), "object"},
      {typeof(bool), "bool"},
    };

    private static readonly HashSet<string> keywords = new HashSet<string>(StringComparer.Ordinal) {
      "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
      "char", "checked", "class", "const", "continue", "decimal", "default",
      "delegate", "do", "double", "else", "enum", "event", "explicit",
      "extern", "false", "finally", "fixed", "float", "for", "foreach",
      "goto", "if", "implicit", "in", "int", "interface", "internal",
      "is", "lock", "long", "namespace", "new", "null", "object", "operator",
      "out", "override", "params", "private", "protected", "public","readonly",
      "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc",
      "static", "string", "struct", "switch", "this", "throw", "true",
      "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
      "using", "virtual", "void", "volatile", "while",

      // context keywords
      "add", "async", "await", "dynamic", "get", "global", "partial",
      "remove", "set", "value", "var", "when", "where", "yield"
    };

    private static readonly Dictionary<MethodSpecialName, string> specialMethodNames = new Dictionary<MethodSpecialName, string>() {
      // comparison
      {MethodSpecialName.Equality, "operator =="},
      {MethodSpecialName.Inequality, "operator !="},
      {MethodSpecialName.LessThan, "operator <"},
      {MethodSpecialName.GreaterThan, "operator >"},
      {MethodSpecialName.LessThanOrEqual, "operator <="},
      {MethodSpecialName.GreaterThanOrEqual, "operator >="},

      // unary
      {MethodSpecialName.UnaryPlus, "operator +"},
      {MethodSpecialName.UnaryNegation, "operator -"},
      {MethodSpecialName.LogicalNot, "operator !"},
      {MethodSpecialName.OnesComplement, "operator ~"},
      {MethodSpecialName.True, "operator true"},
      {MethodSpecialName.False, "operator false"},
      {MethodSpecialName.Increment, "operator ++"},
      {MethodSpecialName.Decrement, "operator --"},

      // binary
      {MethodSpecialName.Addition, "operator +"},
      {MethodSpecialName.Subtraction, "operator -"},
      {MethodSpecialName.Multiply, "operator *"},
      {MethodSpecialName.Division, "operator /"},
      {MethodSpecialName.Modulus, "operator %"},
      {MethodSpecialName.BitwiseAnd, "operator &"},
      {MethodSpecialName.BitwiseOr, "operator |"},
      {MethodSpecialName.ExclusiveOr, "operator ^"},
      {MethodSpecialName.RightShift, "operator >>"},
      {MethodSpecialName.LeftShift, "operator <<"},

      // type cast
      {MethodSpecialName.Explicit, "explicit operator"},
      {MethodSpecialName.Implicit, "implicit operator"},
    };

    public static string FormatAccessibility(Accessibility accessibility)
      => accessibilities.TryGetValue(accessibility, out var ret) ? ret : null;

    public static bool IsLanguagePrimitiveType(Type t, out string primitiveTypeName)
      => primitiveTypes.TryGetValue(t, out primitiveTypeName);

    public static IEnumerable<string> ToNamespaceList(Type t)
      => t.GetNamespaces(type => IsLanguagePrimitiveType(type, out _));

    public static string FormatSpecialNameMethod(MethodBase methodOrConstructor, out MethodSpecialName nameType)
    {
      nameType = methodOrConstructor.GetNameType();

      if (specialMethodNames.TryGetValue(nameType, out var name))
        return name;

      if (nameType == MethodSpecialName.Constructor)
        return methodOrConstructor.DeclaringType.IsGenericType ? methodOrConstructor.DeclaringType.GetGenericTypeName() : methodOrConstructor.DeclaringType.Name;

      return methodOrConstructor.Name; // as default
    }

    public static string FormatParameterList(MethodBase m, bool typeWithNamespace = true, bool useDefaultLiteral = false)
    {
      return FormatParameterList(m.GetParameters(), typeWithNamespace, useDefaultLiteral);
    }

    public static string FormatParameterList(ParameterInfo[] parameterList, bool typeWithNamespace = true, bool useDefaultLiteral = false)
    {
      return string.Join(", ", parameterList.Select(ToString));

      static string ToVerbatim(string name)
      {
        if (keywords.Contains(name))
          return "@" + name;
        else
          return name;
      }

      string ToString(ParameterInfo p)
      {
        var ret = new StringBuilder();

        if (p.ParameterType.IsByRef) {
          var typeAndName = p.ParameterType.GetElementType().FormatTypeName(attributeProvider: p, typeWithNamespace: typeWithNamespace) + " " + ToVerbatim(p.Name);

          if (p.IsIn)
            ret.Append("in ").Append(typeAndName);
          else if (p.IsOut)
            ret.Append("out ").Append(typeAndName);
          else
            ret.Append("ref ").Append(typeAndName);
        }
        else {
          var typeAndName = p.ParameterType.FormatTypeName(attributeProvider: p, typeWithNamespace: typeWithNamespace) + " " + ToVerbatim(p.Name);

          if (p.Position == 0 && p.Member.GetCustomAttribute<ExtensionAttribute>() != null) {
            ret.Append("this ").Append(typeAndName);
          }
          else if (p.GetCustomAttribute<ParamArrayAttribute>() != null) {
            ret.Append("params ").Append(typeAndName);
          }
          else {
            ret.Append(typeAndName);
          }
        }

        if (p.HasDefaultValue) {
          var defaultValueDeclaration = FormatValueDeclaration(
            p.DefaultValue,
            p.ParameterType,
            typeWithNamespace: typeWithNamespace,
            findConstantField: true,
            useDefaultLiteral: useDefaultLiteral
          );

          ret.Append(" = ");
          ret.Append(defaultValueDeclaration);
        }

        return ret.ToString();
      }
    }

    public static string FormatTypeName(
      this Type t,
      ICustomAttributeProvider attributeProvider = null,
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
      if (t.IsArray)
        return FormatTypeName(t.GetElementType(), showVariance: false, options) + "[" + new string(',', t.GetArrayRank() - 1) + "]";

      if (t.IsByRef)
        return FormatTypeName(t.GetElementType(), showVariance: false, options) + "&";

      if (t.IsPointer)
        return FormatTypeName(t.GetElementType(), showVariance: false, options) + "*";

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

        if (t.IsConstructedGenericType && "System".Equals(t.Namespace, StringComparison.Ordinal) && t.GetGenericTypeName().Equals("ValueTuple", StringComparison.Ordinal)) {
          var tupleItemNames = options.AttributeProvider?.GetCustomAttributes(typeof(TupleElementNamesAttribute), inherit: false)?.Cast<TupleElementNamesAttribute>()?.FirstOrDefault()?.TransformNames.ToList();

          if (tupleItemNames != null) {
            for (var index = 0; index < tupleItemNames.Count; index++) {
              tupleItemNames[index] = " " + tupleItemNames[index]; // append delimiter between type and name
            }
          }

          sb.Append("(")
            .Append(string.Join(", ", t.GetGenericArguments().Select((arg, index) => FormatTypeName(arg, showVariance: true, options) + tupleItemNames?[index])))
            .Append(")");
        }
        else {
          if (options.TypeWithNamespace && !t.IsNested)
            sb.Append(t.Namespace).Append(".");

          IEnumerable<Type> genericArgs = t.GetGenericArguments();

          if (t.IsNested) {
            var genericArgsOfDeclaringType = t.DeclaringType.GetGenericArguments();

            if (options.WithDeclaringTypeName) {
              var declaringType = t.DeclaringType.IsGenericTypeDefinition
                ? t.DeclaringType.MakeGenericType(genericArgs.Take(genericArgsOfDeclaringType.Length).ToArray())
                : t.DeclaringType;

              sb.Append(FormatTypeName(declaringType, showVariance: true, options)).Append(".");
            }

            genericArgs = genericArgs.Skip(genericArgsOfDeclaringType.Length);
          }

          sb.Append(t.GetGenericTypeName());

          var formattedGenericArgs = string.Join(", ", genericArgs.Select(arg => FormatTypeName(arg, showVariance: true, options)));

          if (0 < formattedGenericArgs.Length)
            sb.Append("<").Append(formattedGenericArgs).Append(">");
        }

        return sb.ToString();
      }

      if (options.TranslateLanguagePrimitiveType && IsLanguagePrimitiveType(t, out var n))
        return n;

      if (options.WithDeclaringTypeName && t.IsNested)
        return FormatTypeName(t.DeclaringType, showVariance, options) + "." + t.Name;
      if (options.TypeWithNamespace)
        return t.Namespace + "." + t.Name;

      return t.Name;
    }

    public static string EscapeString(string s, bool escapeSingleQuote = false, bool escapeDoubleQuote = false)
    {
      if (s == null)
        throw new ArgumentNullException(nameof(s));

      var chars = s.ToCharArray();
      var buf = new StringBuilder(chars.Length);

      for (var i = 0; i < chars.Length; i++) {
        if (Char.IsControl(chars[i]))
          buf.Append("\\u").Append(((int)chars[i]).ToString("X4"));
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
      object val,
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

      if (typeOfValue == typeof(string)) {
        return "\"" + EscapeString((string)val, escapeDoubleQuote: true) + "\"";
      }
      else if (typeOfValue == typeof(char)) {
        return "\'" + EscapeString(((char)val).ToString(), escapeSingleQuote: true) + "\'";
      }
      else if (typeOfValue == typeof(bool)) {
        if ((bool)val == true)
          return "true";
        else
          return "false";
      }
      else {
        if (typeOfValue.IsValueType && findConstantField) {
          // try to find constant field
          foreach (var f in typeOfValue.GetFields(BindingFlags.Static | BindingFlags.Public)) {
            if ((f.IsLiteral || f.IsInitOnly) && val.Equals(f.GetValue(null)))
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
          return $"({typeOfValue.FormatTypeName(typeWithNamespace: typeWithNamespace)}){Convert.ChangeType(val, typeOfValue.GetEnumUnderlyingType())}";

        if (typeOfValue.IsPrimitive && typeOfValue.IsValueType)
          return val.ToString();
        else
          return null;
      }
    }
  }
}
