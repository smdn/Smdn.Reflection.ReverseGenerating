// Smdn.Reflection.ReverseGenerating.dll (Smdn.Reflection.ReverseGenerating-1.1.6)
//   Name: Smdn.Reflection.ReverseGenerating
//   AssemblyVersion: 1.1.6.0
//   InformationalVersion: 1.1.6+407f242e4289093d014484d0f1940c407f03b605
//   TargetFramework: .NETCoreApp,Version=v6.0
//   Configuration: Release
//   Referenced assemblies:
//     Smdn.Fundamental.Reflection, Version=3.3.2.0, Culture=neutral
//     System.Collections, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Linq, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Memory, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
//     System.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Runtime.InteropServices, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//     System.Threading, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
#nullable enable annotations

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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

  public static class CSharpFormatter {
    public static string EscapeString(string s, bool escapeSingleQuote = false, bool escapeDoubleQuote = false) {}
    public static string FormatAccessibility(Accessibility accessibility) {}
    public static string FormatParameter(ParameterInfo p, NullabilityInfoContext? nullabilityInfoContext, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatParameter(ParameterInfo p, NullabilityInfoContext? nullabilityInfoContext, object? nullabilityInfoContextLockObject, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatParameter(ParameterInfo p, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatParameterList(MethodBase m, NullabilityInfoContext? nullabilityInfoContext, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatParameterList(MethodBase m, NullabilityInfoContext? nullabilityInfoContext, object? nullabilityInfoContextLockObject, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatParameterList(MethodBase m, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatParameterList(ParameterInfo[] parameterList, NullabilityInfoContext? nullabilityInfoContext, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatParameterList(ParameterInfo[] parameterList, NullabilityInfoContext? nullabilityInfoContext, object? nullabilityInfoContextLockObject, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatParameterList(ParameterInfo[] parameterList, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatSpecialNameMethod(MethodBase methodOrConstructor, out MethodSpecialName nameType) {}
    public static string FormatTypeName(this EventInfo ev, NullabilityInfoContext? nullabilityInfoContext, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this EventInfo ev, NullabilityInfoContext? nullabilityInfoContext, object? nullabilityInfoContextLockObject, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this EventInfo ev, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this FieldInfo f, NullabilityInfoContext? nullabilityInfoContext, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this FieldInfo f, NullabilityInfoContext? nullabilityInfoContext, object? nullabilityInfoContextLockObject, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this FieldInfo f, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this ParameterInfo p, NullabilityInfoContext? nullabilityInfoContext, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this ParameterInfo p, NullabilityInfoContext? nullabilityInfoContext, object? nullabilityInfoContextLockObject, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this ParameterInfo p, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this PropertyInfo p, NullabilityInfoContext? nullabilityInfoContext, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this PropertyInfo p, NullabilityInfoContext? nullabilityInfoContext, object? nullabilityInfoContextLockObject, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this PropertyInfo p, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatTypeName(this Type t, ICustomAttributeProvider? attributeProvider = null, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatValueDeclaration(object? val, Type typeOfValue, bool typeWithNamespace = true, bool findConstantField = false, bool useDefaultLiteral = false) {}
    public static bool IsLanguagePrimitiveType(Type t, [MaybeNullWhen(false)] out string? primitiveTypeName) {}
    public static IEnumerable<string> ToNamespaceList(Type t) {}
  }

  public static class Generator {
    public static IEnumerable<string> GenerateAttributeList(ICustomAttributeProvider attributeProvider, ISet<string>? referencingNamespaces, GeneratorOptions options) {}
    public static IEnumerable<string> GenerateExplicitBaseTypeAndInterfaces(Type t, ISet<string>? referencingNamespaces, GeneratorOptions options) {}
    [Obsolete("Use GenerateGenericParameterConstraintDeclaration instead.")]
    public static string GenerateGenericArgumentConstraintDeclaration(Type genericArgument, ISet<string>? referencingNamespaces, GeneratorOptions options) {}
    public static string GenerateGenericParameterConstraintDeclaration(Type genericParameter, ISet<string>? referencingNamespaces, GeneratorOptions options) {}
    public static string? GenerateMemberDeclaration(MemberInfo member, ISet<string>? referencingNamespaces, GeneratorOptions options) {}
    public static string GenerateTypeDeclaration(Type t, ISet<string>? referencingNamespaces, GeneratorOptions options) {}
    public static IEnumerable<string> GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(Type t, ISet<string>? referencingNamespaces, GeneratorOptions options) {}
  }

  public class GeneratorOptions : ICloneable {
    public class AttributeDeclarationOptions {
      public AttributeDeclarationOptions() {}

      public AttributeSectionFormat AccessorFormat { get; set; }
      public AttributeSectionFormat AccessorParameterFormat { get; set; }
      public AttributeSectionFormat BackingFieldFormat { get; set; }
      public AttributeSectionFormat DelegateParameterFormat { get; set; }
      public AttributeSectionFormat GenericParameterFormat { get; set; }
      public AttributeSectionFormat MethodParameterFormat { get; set; }
      public bool OmitAttributeSuffix { get; set; }
      public AttributeTypeFilter? TypeFilter { get; set; }
      public bool WithDeclaringTypeName { get; set; }
      public bool WithNamedArguments { get; set; }
      public bool WithNamespace { get; set; }
    }

    public class MemberDeclarationOptions {
      public MemberDeclarationOptions() {}

      public MethodBodyOption AccessorBody { get; set; }
      public MethodBodyOption MethodBody { get; set; }
      public NullabilityInfoContext? NullabilityInfoContext { get; set; }
      public object? NullabilityInfoContextLockObject { get; set; }
      public bool OmitEndOfStatement { get; set; }
      public bool WithAccessibility { get; set; }
      public bool WithDeclaringTypeName { get; set; }
      public bool WithEnumTypeName { get; set; }
      public bool WithNamespace { get; set; }
    }

    public class ParameterDeclarationOptions {
      public ParameterDeclarationOptions() {}

      public bool WithDeclaringTypeName { get; set; }
      public bool WithNamespace { get; set; }
    }

    public class TypeDeclarationOptions {
      public TypeDeclarationOptions() {}

      public NullabilityInfoContext? NullabilityInfoContext { get; set; }
      public object? NullabilityInfoContextLockObject { get; set; }
      public bool OmitEndOfStatement { get; set; }
      public bool OmitEnumUnderlyingTypeIfPossible { get; set; }
      public bool WithAccessibility { get; set; }
      public bool WithDeclaringTypeName { get; set; }
      public bool WithNamespace { get; set; }
    }

    public class ValueDeclarationOptions {
      public ValueDeclarationOptions() {}

      public bool UseDefaultLiteral { get; set; }
      public bool WithDeclaringTypeName { get; set; }
      public bool WithNamespace { get; set; }
    }

    public GeneratorOptions() {}

    public GeneratorOptions.AttributeDeclarationOptions AttributeDeclaration { get; init; }
    public bool IgnorePrivateOrAssembly { get; set; }
    public string? Indent { get; set; }
    public GeneratorOptions.MemberDeclarationOptions MemberDeclaration { get; init; }
    public GeneratorOptions.ParameterDeclarationOptions ParameterDeclaration { get; init; }
    public bool TranslateLanguagePrimitiveTypeDeclaration { get; set; }
    public GeneratorOptions.TypeDeclarationOptions TypeDeclaration { get; init; }
    public GeneratorOptions.ValueDeclarationOptions ValueDeclaration { get; init; }

    public virtual GeneratorOptions Clone() {}
    object ICloneable.Clone() {}
  }
}
// API list generated by Smdn.Reflection.ReverseGenerating.ListApi.MSBuild.Tasks v1.2.2.0.
// Smdn.Reflection.ReverseGenerating.ListApi.Core v1.2.0.0 (https://github.com/smdn/Smdn.Reflection.ReverseGenerating)
