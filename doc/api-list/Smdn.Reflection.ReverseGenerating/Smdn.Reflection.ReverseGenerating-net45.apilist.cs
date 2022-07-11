// Smdn.Reflection.ReverseGenerating.dll (Smdn.Reflection.ReverseGenerating-1.1.1)
//   Name: Smdn.Reflection.ReverseGenerating
//   AssemblyVersion: 1.1.1.0
//   InformationalVersion: 1.1.1+9c1ed7379f1b7fe2562a51f0283b02332d84584f
//   TargetFramework: .NETFramework,Version=v4.5
//   Configuration: Release

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Smdn.Reflection;
using Smdn.Reflection.ReverseGenerating;

namespace Smdn.Reflection.ReverseGenerating {
  public delegate bool AttributeTypeFilter(Type type, ICustomAttributeProvider attributeProvider);

  public enum AttributeSectionFormat : int {
    Discrete = 1,
    List = 0,
  }

  public enum MethodBodyOption : int {
    EmptyImplementation = 1,
    None = 0,
    ThrowNotImplementedException = 2,
    ThrowNull = 3,
  }

  [Nullable(byte.MinValue)]
  [NullableContext(1)]
  public static class CSharpFormatter {
    public static string EscapeString(string s, bool escapeSingleQuote = false, bool escapeDoubleQuote = false) {}
    public static string FormatAccessibility(Accessibility accessibility) {}
    public static string FormatParameter(ParameterInfo p, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatParameterList(MethodBase m, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatParameterList(ParameterInfo[] parameterList, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatSpecialNameMethod(MethodBase methodOrConstructor, out MethodSpecialName nameType) {}
    public static string FormatTypeName(this EventInfo ev, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this FieldInfo f, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this ParameterInfo p, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this PropertyInfo p, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this Type t, [Nullable(2)] ICustomAttributeProvider attributeProvider = null, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatValueDeclaration([Nullable(2)] object val, Type typeOfValue, bool typeWithNamespace = true, bool findConstantField = false, bool useDefaultLiteral = false) {}
    public static bool IsLanguagePrimitiveType(Type t, out string primitiveTypeName) {}
    public static IEnumerable<string> ToNamespaceList(Type t) {}
  }

  [Nullable(byte.MinValue)]
  [NullableContext(1)]
  public static class Generator {
    public static IEnumerable<string> GenerateAttributeList(ICustomAttributeProvider attributeProvider, [Nullable] ISet<string> referencingNamespaces, GeneratorOptions options) {}
    public static IEnumerable<string> GenerateExplicitBaseTypeAndInterfaces(Type t, [Nullable] ISet<string> referencingNamespaces, GeneratorOptions options) {}
    public static string GenerateGenericArgumentConstraintDeclaration(Type genericArgument, [Nullable] ISet<string> referencingNamespaces, GeneratorOptions options) {}
    [return: Nullable(2)] public static string GenerateMemberDeclaration(MemberInfo member, [Nullable] ISet<string> referencingNamespaces, GeneratorOptions options) {}
    public static string GenerateTypeDeclaration(Type t, [Nullable] ISet<string> referencingNamespaces, GeneratorOptions options) {}
    public static IEnumerable<string> GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(Type t, [Nullable] ISet<string> referencingNamespaces, GeneratorOptions options) {}
  }

  [Nullable(byte.MinValue)]
  [NullableContext(1)]
  public class GeneratorOptions : ICloneable {
    [Nullable(byte.MinValue)]
    [NullableContext(2)]
    public class AttributeDeclarationOptions {
      public AttributeDeclarationOptions() {}

      public AttributeSectionFormat AccessorFormat { get; set; }
      public AttributeSectionFormat AccessorParameterFormat { get; set; }
      public AttributeSectionFormat BackingFieldFormat { get; set; }
      public AttributeSectionFormat DelegateParameterFormat { get; set; }
      public AttributeSectionFormat GenericParameterFormat { get; set; }
      public AttributeSectionFormat MethodParameterFormat { get; set; }
      public bool OmitAttributeSuffix { get; set; }
      public AttributeTypeFilter TypeFilter { get; set; }
      public bool WithDeclaringTypeName { get; set; }
      public bool WithNamedArguments { get; set; }
      public bool WithNamespace { get; set; }
    }

    [NullableContext(byte.MinValue)]
    public class MemberDeclarationOptions {
      public MemberDeclarationOptions() {}

      public MethodBodyOption AccessorBody { get; set; }
      public MethodBodyOption MethodBody { get; set; }
      public bool OmitEndOfStatement { get; set; }
      public bool WithAccessibility { get; set; }
      public bool WithDeclaringTypeName { get; set; }
      public bool WithNamespace { get; set; }
    }

    [NullableContext(byte.MinValue)]
    public class ParameterDeclarationOptions {
      public ParameterDeclarationOptions() {}

      public bool WithDeclaringTypeName { get; set; }
      public bool WithNamespace { get; set; }
    }

    [NullableContext(byte.MinValue)]
    public class TypeDeclarationOptions {
      public TypeDeclarationOptions() {}

      public bool OmitEndOfStatement { get; set; }
      public bool WithAccessibility { get; set; }
      public bool WithDeclaringTypeName { get; set; }
      public bool WithNamespace { get; set; }
    }

    [NullableContext(byte.MinValue)]
    public class ValueDeclarationOptions {
      public ValueDeclarationOptions() {}

      public bool UseDefaultLiteral { get; set; }
      public bool WithDeclaringTypeName { get; set; }
      public bool WithNamespace { get; set; }
    }

    public GeneratorOptions() {}

    public GeneratorOptions.AttributeDeclarationOptions AttributeDeclaration { get; init; }
    public bool IgnorePrivateOrAssembly { get; set; }
    [Nullable(2)]
    public string Indent { get; set; }
    public GeneratorOptions.MemberDeclarationOptions MemberDeclaration { get; init; }
    public GeneratorOptions.ParameterDeclarationOptions ParameterDeclaration { get; init; }
    public bool TranslateLanguagePrimitiveTypeDeclaration { get; set; }
    public GeneratorOptions.TypeDeclarationOptions TypeDeclaration { get; init; }
    public GeneratorOptions.ValueDeclarationOptions ValueDeclaration { get; init; }

    public virtual GeneratorOptions Clone() {}
    object ICloneable.Clone() {}
  }
}

