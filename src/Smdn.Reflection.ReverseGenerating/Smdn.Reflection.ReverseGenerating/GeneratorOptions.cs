// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
using System.Reflection;
#endif

namespace Smdn.Reflection.ReverseGenerating;

public class GeneratorOptions : ICloneable {
  public string? Indent { get; set; } = new string(' ', 2);

  public bool IgnorePrivateOrAssembly { get; set; } = true;

  public bool TranslateLanguagePrimitiveTypeDeclaration { get; set; } = false;

  object ICloneable.Clone() => Clone();

  public virtual GeneratorOptions Clone()
    => new() {
      Indent = (string?)this.Indent?.Clone(),
      IgnorePrivateOrAssembly = this.IgnorePrivateOrAssembly,
      TranslateLanguagePrimitiveTypeDeclaration = this.TranslateLanguagePrimitiveTypeDeclaration,
      TypeDeclaration = this.TypeDeclaration.Clone(),
      MemberDeclaration = this.MemberDeclaration.Clone(),
      AttributeDeclaration = this.AttributeDeclaration.Clone(),
      ValueDeclaration = this.ValueDeclaration.Clone(),
    };

  public TypeDeclarationOptions TypeDeclaration { get; init; } = new();

  public class TypeDeclarationOptions {
    public bool WithNamespace { get; set; } = false;
    public bool WithDeclaringTypeName { get; set; } = false;
    public bool WithAccessibility { get; set; } = true;
    public bool OmitEndOfStatement { get; set; } = false;
    public bool OmitEnumUnderlyingTypeIfPossible { get; set; } = false;
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    public NullabilityInfoContext? NullabilityInfoContext { get; set; } = new();
#endif

    internal TypeDeclarationOptions Clone()
      => (TypeDeclarationOptions)MemberwiseClone();
  }

  public MemberDeclarationOptions MemberDeclaration { get; init; } = new();

  public class MemberDeclarationOptions {
    public bool WithNamespace { get; set; } = false;
    public bool WithDeclaringTypeName { get; set; } = false;
    public bool WithEnumTypeName { get; set; } = false;
    public bool WithAccessibility { get; set; } = true;
    public bool OmitEndOfStatement { get; set; } = false;
    public MethodBodyOption MethodBody { get; set; } = MethodBodyOption.EmptyImplementation;
    public MethodBodyOption AccessorBody { get; set; } = MethodBodyOption.EmptyImplementation;
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    public NullabilityInfoContext? NullabilityInfoContext { get; set; } = new();
#endif

    internal MemberDeclarationOptions Clone()
      => (MemberDeclarationOptions)MemberwiseClone();
  }

  public AttributeDeclarationOptions AttributeDeclaration { get; init; } = new();

  public class AttributeDeclarationOptions {
    public bool WithNamespace { get; set; } = false;
    public bool WithDeclaringTypeName { get; set; } = false;
    public bool WithNamedArguments { get; set; } = false;
    public bool OmitAttributeSuffix { get; set; } = true;
    public AttributeTypeFilter? TypeFilter { get; set; } = null;
    public AttributeSectionFormat AccessorFormat { get; set; } = AttributeSectionFormat.List;
    public AttributeSectionFormat AccessorParameterFormat { get; set; } = AttributeSectionFormat.List;
    public AttributeSectionFormat BackingFieldFormat { get; set; } = AttributeSectionFormat.List;
    public AttributeSectionFormat GenericParameterFormat { get; set; } = AttributeSectionFormat.List;
    public AttributeSectionFormat MethodParameterFormat { get; set; } = AttributeSectionFormat.List;
    public AttributeSectionFormat DelegateParameterFormat { get; set; } = AttributeSectionFormat.List;

    internal AttributeDeclarationOptions Clone()
      => (AttributeDeclarationOptions)MemberwiseClone();
  }

  public ValueDeclarationOptions ValueDeclaration { get; init; } = new();

  public class ValueDeclarationOptions {
    public bool UseDefaultLiteral { get; set; } = false;
    public bool WithNamespace { get; set; } = false;
    public bool WithDeclaringTypeName { get; set; } = true;

    internal ValueDeclarationOptions Clone()
      => (ValueDeclarationOptions)MemberwiseClone();
  }

  public ParameterDeclarationOptions ParameterDeclaration { get; init; } = new();

  public class ParameterDeclarationOptions {
    public bool WithNamespace { get; set; } = false;
    public bool WithDeclaringTypeName { get; set; } = false;

    internal ParameterDeclarationOptions Clone()
      => (ParameterDeclarationOptions)MemberwiseClone();
  }
}
