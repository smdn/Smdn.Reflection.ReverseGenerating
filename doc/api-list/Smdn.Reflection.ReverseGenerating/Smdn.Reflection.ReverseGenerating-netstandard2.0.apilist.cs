// Smdn.Reflection.ReverseGenerating.dll (Smdn.Reflection.ReverseGenerating-1.0.5)
//   Name: Smdn.Reflection.ReverseGenerating
//   AssemblyVersion: 1.0.5.0
//   InformationalVersion: 1.0.5+dd9591cbed8623f61349f46c833f14f1578db406
//   TargetFramework: .NETStandard,Version=v2.0
//   Configuration: Release

using System;
using System.Collections.Generic;
using System.Reflection;
using Smdn.Reflection;
using Smdn.Reflection.ReverseGenerating;

namespace Smdn.Reflection.ReverseGenerating {
  public delegate bool AttributeTypeFilter(Type type, ICustomAttributeProvider attributeProvider);

  public enum MethodBodyOption : int {
    EmptyImplementation = 1,
    None = 0,
    ThrowNotImplementedException = 2,
  }

  public static class CSharpFormatter {
    public static string EscapeString(string s, bool escapeSingleQuote = false, bool escapeDoubleQuote = false) {}
    public static string FormatAccessibility(Accessibility accessibility) {}
    public static string FormatParameter(ParameterInfo p, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatParameterList(MethodBase m, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatParameterList(ParameterInfo[] parameterList, bool typeWithNamespace = true, bool useDefaultLiteral = false) {}
    public static string FormatSpecialNameMethod(MethodBase methodOrConstructor, out MethodSpecialName nameType) {}
    public static string FormatTypeName(this Type t, ICustomAttributeProvider attributeProvider = null, bool typeWithNamespace = true, bool withDeclaringTypeName = true, bool translateLanguagePrimitiveType = true) {}
    public static string FormatValueDeclaration(object val, Type typeOfValue, bool typeWithNamespace = true, bool findConstantField = false, bool useDefaultLiteral = false) {}
    public static bool IsLanguagePrimitiveType(Type t, out string primitiveTypeName) {}
    public static IEnumerable<string> ToNamespaceList(Type t) {}
  }

  public static class Generator {
    public static IEnumerable<string> GenerateAttributeList(ICustomAttributeProvider attributeProvider, ISet<string> referencingNamespaces, GeneratorOptions options) {}
    public static IEnumerable<string> GenerateExplicitBaseTypeAndInterfaces(Type t, ISet<string> referencingNamespaces, GeneratorOptions options) {}
    public static string GenerateGenericArgumentConstraintDeclaration(Type genericArgument, ISet<string> referencingNamespaces, GeneratorOptions options) {}
    public static string GenerateMemberDeclaration(MemberInfo member, ISet<string> referencingNamespaces, GeneratorOptions options) {}
    public static string GenerateTypeDeclaration(Type t, ISet<string> referencingNamespaces, GeneratorOptions options) {}
    public static IEnumerable<string> GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(Type t, ISet<string> referencingNamespaces, GeneratorOptions options) {}
  }

  public class GeneratorOptions {
    public class AttributeDeclarationOptions {
      public AttributeDeclarationOptions() {}

      public AttributeTypeFilter TypeFilter { get; set; }
      public bool WithNamedArguments { get; set; }
      public bool WithNamespace { get; set; }
    }

    public class MemberDeclarationOptions {
      public MemberDeclarationOptions() {}

      public MethodBodyOption MethodBody { get; set; }
      public bool OmitEndOfStatement { get; set; }
      public bool WithAccessibility { get; set; }
      public bool WithDeclaringTypeName { get; set; }
      public bool WithNamespace { get; set; }
    }

    public class TypeDeclarationOptions {
      public TypeDeclarationOptions() {}

      public bool OmitEndOfStatement { get; set; }
      public bool WithAccessibility { get; set; }
      public bool WithDeclaringTypeName { get; set; }
      public bool WithNamespace { get; set; }
    }

    public class ValueDeclarationOptions {
      public ValueDeclarationOptions() {}

      public bool UseDefaultLiteral { get; set; }
    }

    public GeneratorOptions() {}

    public GeneratorOptions.AttributeDeclarationOptions AttributeDeclaration { get; }
    public bool IgnorePrivateOrAssembly { get; set; }
    public string Indent { get; set; }
    public GeneratorOptions.MemberDeclarationOptions MemberDeclaration { get; }
    public bool TranslateLanguagePrimitiveTypeDeclaration { get; set; }
    public GeneratorOptions.TypeDeclarationOptions TypeDeclaration { get; }
    public GeneratorOptions.ValueDeclarationOptions ValueDeclaration { get; }
  }
}

