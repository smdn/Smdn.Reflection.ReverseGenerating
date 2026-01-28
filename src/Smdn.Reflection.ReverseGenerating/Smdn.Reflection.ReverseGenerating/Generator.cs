// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using Smdn.Reflection.Attributes;

namespace Smdn.Reflection.ReverseGenerating;

public static partial class Generator {
  private static Type GenerateDeclarationValidateTypeArgument(Type? t, string paramName)
  {
    if (t is null)
      throw new ArgumentNullException(paramName);
    if (t.IsConstructedGenericType)
      throw new ArgumentException($"can not generate declaration of constructed generic types (type: {t})");

    return t;
  }

  private static Type GenerateDeclarationValidateGenericParameterArgument(Type? genericParameter, string paramName)
  {
    if (genericParameter is null)
      throw new ArgumentNullException(paramName);
    if (!genericParameter.IsGenericParameter)
      throw new ArgumentException($"can not generate declaration of types which does not represent generic parameter ({genericParameter.FullName})");

    return genericParameter;
  }

  public static string GenerateTypeDeclaration(
    Type t,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  ) =>
    GenerateTypeDeclaration(
      GenerateDeclarationValidateTypeArgument(t, nameof(t)),
      false,
      referencingNamespaces,
      options ?? throw new ArgumentNullException(nameof(options))
    ).First();

  public static IEnumerable<string> GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(
    Type t,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  ) =>
    GenerateTypeDeclaration(
      GenerateDeclarationValidateTypeArgument(t, nameof(t)),
      true,
      referencingNamespaces,
      options ?? throw new ArgumentNullException(nameof(options))
    );

#pragma warning disable CA1502 // TODO: simplify and refactor
  private static IEnumerable<string> GenerateTypeDeclaration(
    Type t,
    bool generateExplicitBaseTypeAndInterfaces,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    var accessibilityList = options.TypeDeclaration.WithAccessibility
      ? CSharpFormatter.FormatAccessibility(t.GetAccessibility()) + " "
      : string.Empty;
    var typeName = CSharpTypeNameFormatter.Format(
      type: t,
      options: new(
        AttributeProvider: t,
        WithNamespace: false,
        WithDeclaringTypeName: options.TypeDeclaration.WithDeclaringTypeName,
        TranslateLanguagePrimitiveType: options.TranslateLanguagePrimitiveTypeDeclaration,
        PrependGenericParameterAttributes: (genericParam, builder) => {
          foreach (var attr in GenerateAttributeList(genericParam, referencingNamespaces, options)) {
            builder.Append(attr).Append(' ');
          }
        }
      )
    );

    if (t.IsConcreteDelegate()) {
      yield return GenerateDelegateDeclaration(t, referencingNamespaces, options)!;
      yield break;
    }

    var genericParameterConstraints = t
      .GetGenericArguments()
      .Select(
        param => GenerateGenericParameterConstraintDeclaration(param, referencingNamespaces, options)
      )
      .Where(static d => !string.IsNullOrEmpty(d))
      .ToList();

    string GetSingleLineGenericParameterConstraintsDeclaration()
      => genericParameterConstraints.Count == 0 ? string.Empty : " " + string.Join(" ", genericParameterConstraints);

    var modifierNew = t.IsHidingInheritedType(nonPublic: true) ? "new " : null;

    if (t.IsEnum) {
      string? underlyingTypeDeclaration = null;
      var underlyingType = t.GetEnumUnderlyingType();
      var omitEnumUnderlyingType =
        options.TypeDeclaration.OmitEnumUnderlyingTypeIfPossible &&
        string.Equals(underlyingType.FullName, "System.Int32", StringComparison.Ordinal);

      if (!omitEnumUnderlyingType) {
        underlyingTypeDeclaration = string.Concat(
          " : ",
          underlyingType.FormatTypeName(
            typeWithNamespace: options.TypeDeclaration.WithNamespace,
            translateLanguagePrimitiveType: options.TranslateLanguagePrimitiveTypeDeclaration
          )
        );
      }

      yield return $"{modifierNew}{accessibilityList}enum {typeName}{underlyingTypeDeclaration}";
      yield break;
    }

    static bool HasUnsafeFields(Type t)
      => t
        .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
        .Any(static f => f.FieldType.IsPointer || f.IsFixedBuffer());

    string typeDeclaration;

    if (t.IsInterface) {
      typeDeclaration = $"{modifierNew}{accessibilityList}interface {typeName}";
    }
    else if (t.IsValueType) {
      var isReadOnly = t.IsReadOnlyValueType() ? "readonly " : string.Empty;
      var isUnsafe = (options.TypeDeclaration.DetectUnsafe && HasUnsafeFields(t)) ? "unsafe " : string.Empty;
      var isByRefLike = t.IsByRefLikeValueType() ? "ref " : string.Empty;
      var isRecord = (options.TypeDeclaration.EnableRecordTypes && t.IsRecord()) ? "record " : string.Empty;

      typeDeclaration = $"{modifierNew}{accessibilityList}{isReadOnly}{isByRefLike}{isUnsafe}{isRecord}struct {typeName}";
    }
    else {
      string? modifier = null;

      if (t.IsAbstract && t.IsSealed)
        modifier = "static ";
      else if (t.IsAbstract)
        modifier = "abstract ";
      else if (t.IsSealed)
        modifier = "sealed ";

      var isUnsafe = (options.TypeDeclaration.DetectUnsafe && HasUnsafeFields(t)) ? "unsafe " : string.Empty;
      var isRecord = (options.TypeDeclaration.EnableRecordTypes && t.IsRecord()) ? "record " : string.Empty;

      typeDeclaration = $"{modifierNew}{accessibilityList}{modifier}{isUnsafe}{isRecord}class {typeName}";
    }

    if (!generateExplicitBaseTypeAndInterfaces) {
      yield return typeDeclaration + GetSingleLineGenericParameterConstraintsDeclaration();
      yield break;
    }

    var baseTypeList = GenerateExplicitBaseTypeAndInterfaces(t, referencingNamespaces, options).ToList();

    if (baseTypeList.Count <= 1) {
      var baseTypeDeclaration = baseTypeList.Count == 0
        ? string.Empty
        : " : " + baseTypeList[0];
      var genericParameterConstraintsDeclaration = GetSingleLineGenericParameterConstraintsDeclaration();

      yield return typeDeclaration + baseTypeDeclaration + genericParameterConstraintsDeclaration;
    }
    else {
      yield return typeDeclaration + " :";

      for (var index = 0; index < baseTypeList.Count; index++) {
        if (index == baseTypeList.Count - 1)
          yield return options.Indent + baseTypeList[index];
        else
          yield return options.Indent + baseTypeList[index] + ",";
      }

      foreach (var constraint in genericParameterConstraints) {
        yield return options.Indent + constraint;
      }
    }
  }
#pragma warning restore CA1502

  [Obsolete($"Use {nameof(GenerateGenericParameterConstraintDeclaration)} instead.")]
  public static string GenerateGenericArgumentConstraintDeclaration(
    Type genericArgument,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
    => GenerateGenericParameterConstraintDeclaration(
      genericParameter: GenerateDeclarationValidateGenericParameterArgument(genericArgument, nameof(genericArgument)),
      referencingNamespaces: referencingNamespaces,
      options: options
    );

  private static string GenerateGenericParameterConstraintClause(
    IEnumerable<Type> genericParameters,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
    => string.Join(
      " ",
      genericParameters
        .Select(
          param => GenerateGenericParameterConstraintDeclaration(param, referencingNamespaces, options)
        )
        .Where(static d => !string.IsNullOrEmpty(d))
    );

  public static string GenerateGenericParameterConstraintDeclaration(
    Type genericParameter,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    GenerateDeclarationValidateGenericParameterArgument(genericParameter, nameof(genericParameter));

    if (options is null)
      throw new ArgumentNullException(nameof(options));

    static bool ConstraintTypesContainsValueType(Type genericParameter, out IEnumerable<Type> constraintTypesExceptForValueType)
    {
      var constraintTypes = genericParameter.GetGenericParameterConstraints();

      var indexOfValueType = Array.FindIndex(
        constraintTypes,
        static t => string.Equals(t.FullName, typeof(ValueType).FullName, StringComparison.Ordinal)
      );

      if (indexOfValueType < 0) {
        constraintTypesExceptForValueType = constraintTypes;
        return false;
      }

      constraintTypesExceptForValueType = constraintTypes
        .Take(indexOfValueType)
        .Concat(constraintTypes.Skip(indexOfValueType + 1));

      return true;
    }

    static IEnumerable<string> GetGenericParameterConstraintsOf(
      Type genericParameter,
      ISet<string>? referencingNns,
      bool typeWithNamespace,
      bool translateLanguagePrimitiveTypes
    )
    {
      const GenericParameterAttributes AllowByRefLike =
#if NET9_0_OR_GREATER
        GenericParameterAttributes.AllowByRefLike;
#else
        (GenericParameterAttributes)32;
#endif
      var constraintAttrs = genericParameter.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
      var hasDefaultConstructorConstraint = constraintAttrs.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint);
      IEnumerable<Type>? constraintTypes = null;

      if (constraintAttrs == GenericParameterAttributes.None) {
        if (genericParameter.HasGenericParameterNotNullConstraint())
          yield return "notnull";
      }
      else if (constraintAttrs.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint)) {
        yield return genericParameter.HasGenericParameterUnmanagedConstraint()
          ? "unmanaged"
          : "struct";

        if (hasDefaultConstructorConstraint && ConstraintTypesContainsValueType(genericParameter, out constraintTypes))
          hasDefaultConstructorConstraint = false; // constraint type of System.ValueType implies `new()`
      }
      else if (constraintAttrs.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint)) {
        yield return genericParameter.GetNullableAttributeMetadataValue() == NullableMetadataValue.Annotated
          ? "class?"
          : "class";
      }

      constraintTypes ??= genericParameter.GetGenericParameterConstraints();

      var orderedConstraintTypeNames = constraintTypes
        .Select(constraintType => constraintType.FormatTypeName(typeWithNamespace: typeWithNamespace))
        .OrderBy(static name => name, StringComparer.Ordinal);

      foreach (var ctn in orderedConstraintTypeNames)
        yield return ctn;

      if (hasDefaultConstructorConstraint)
        yield return "new()";

      if (genericParameter.GenericParameterAttributes.HasFlag(AllowByRefLike))
        yield return "allows ref struct";

      referencingNns?.UnionWith(
        constraintTypes.SelectMany(
          t => CSharpFormatter.ToNamespaceList(
            t,
            translateLanguagePrimitiveTypes: translateLanguagePrimitiveTypes
          )
        )
      );
    }

    var constraints = string.Join(
      ", ",
      GetGenericParameterConstraintsOf(
        genericParameter,
        referencingNamespaces,
        genericParameter.DeclaringMethod == null
          ? options.TypeDeclaration.WithNamespace
          : options.MemberDeclaration.WithNamespace,
        translateLanguagePrimitiveTypes: options.TranslateLanguagePrimitiveTypeDeclaration
      )
    );

    if (0 < constraints.Length)
      return $"where {genericParameter.FormatTypeName(typeWithNamespace: false)} : {constraints}";

    return string.Empty;
  }

  public static IEnumerable<string> GenerateExplicitBaseTypeAndInterfaces(
    Type t,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    if (t is null)
      throw new ArgumentNullException(nameof(t));
    if (options == null)
      throw new ArgumentNullException(nameof(options));

    var isRecord = options.TypeDeclaration.EnableRecordTypes && options.TypeDeclaration.OmitRecordImplicitInterface && t.IsRecord();
    var typeOfIEquatableOfRecord = isRecord
      ? typeof(IEquatable<>).MakeGenericType(t) // IEquatable<TRecord>
      : null;

    return t
      .GetExplicitBaseTypeAndInterfaces()
      .Where(type => !(options.IgnorePrivateOrAssembly && type.IsPrivateOrAssembly()))
      .Where(type => type != typeOfIEquatableOfRecord)
      .Select(type => {
        referencingNamespaces?.UnionWith(
          CSharpFormatter.ToNamespaceList(
            type,
            translateLanguagePrimitiveTypes: options.TranslateLanguagePrimitiveTypeDeclaration
          )
        );

        return new {
          type.IsInterface,
          Name = type.FormatTypeName(
            typeWithNamespace: options.TypeDeclaration.WithNamespace,
            withDeclaringTypeName: options.TypeDeclaration.WithDeclaringTypeName
          ),
        };
      })
      .OrderBy(static type => type.IsInterface)
      .ThenBy(static type => type.Name, StringComparer.Ordinal)
      .Select(static type => type.Name);
  }

  public static string? GenerateMemberDeclaration(
    MemberInfo member,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    if (member == null)
      throw new ArgumentNullException(nameof(member));
    if (options == null)
      throw new ArgumentNullException(nameof(options));

    switch (member) {
      case FieldInfo f: return GenerateFieldDeclaration(f, referencingNamespaces, options);
      case PropertyInfo p: return GeneratePropertyDeclaration(p, referencingNamespaces, options);
      case MethodBase m: return GenerateMethodBaseDeclaration(m, referencingNamespaces, options);
      case EventInfo ev: return GenerateEventDeclaration(ev, referencingNamespaces, options);

      default:
        if (member.MemberType == MemberTypes.NestedType)
          throw new ArgumentException("can not generate nested type declarations");
        else
          throw new NotSupportedException($"unsupported member type: {member.MemberType}");
    }
  }

  private static string? GenerateFieldDeclaration(
    FieldInfo field,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    if (options.IgnorePrivateOrAssembly && field.IsPrivateOrAssembly())
      return null;

    var declaringType = field.GetDeclaringTypeOrThrow();
    var sb = new StringBuilder();
    var memberOptions = options.MemberDeclaration;

    if (declaringType.IsEnum) {
      if (field.IsStatic) {
        sb.Append(GenerateMemberName(field, options));

        if (field.TryGetValue(null, out var fieldValue)) {
          sb.Append(" = ");

          var val = Convert.ChangeType(fieldValue, declaringType.GetEnumUnderlyingType(), provider: null);

          if (val is not null && declaringType.IsEnumFlags())
            sb.Append("0x").AppendFormat(null, "{0:x" + (Marshal.SizeOf(val) * 2).ToString("D", null) + "}", val);
          else
            sb.Append(val);
        }

        if (!memberOptions.OmitEndOfStatement)
          sb.Append(',');
      }
      else {
        return null; // ignores backing field __value
      }
    }
    else {
      referencingNamespaces?.UnionWith(
        CSharpFormatter.ToNamespaceList(
          field.FieldType,
          translateLanguagePrimitiveTypes: options.TranslateLanguagePrimitiveTypeDeclaration
        )
      );

      AppendMemberModifiers(
        sb,
        field,
        asExplicitInterfaceMember: false,
        options: options
      );

      var isFixedBuffer = field.TryGetFixedBufferAttributeArgs(
        out var fixedBufferElementType,
        out var fixedBufferLength
      );

      if (isFixedBuffer) {
        sb.Append("fixed ").Append(
          fixedBufferElementType!.FormatTypeName(
            typeWithNamespace: options.MemberDeclaration.WithNamespace,
            translateLanguagePrimitiveType: options.TranslateLanguagePrimitiveTypeDeclaration
          )
        );
      }
      else {
        sb.Append(
          field.FormatTypeName(
#pragma warning disable SA1114
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
            nullabilityInfoContext: memberOptions.NullabilityInfoContext,
            nullabilityInfoContextLockObject: memberOptions.NullabilityInfoContextLockObject,
#endif
            typeWithNamespace: memberOptions.WithNamespace
#pragma warning restore SA1114
          )
        );
      }

      sb
        .Append(' ')
        .Append(GenerateMemberName(field, options));

      if (isFixedBuffer)
        sb.Append('[').Append(fixedBufferLength).Append(']');

      TryAppendFieldStaticValue(
        field,
        asPropertyDeclaration: false,
        sb,
        options
      );
    }

    return sb.ToString();
  }

  private static void TryAppendFieldStaticValue(
    FieldInfo field,
    bool asPropertyDeclaration,
    StringBuilder builder,
    GeneratorOptions options
  )
  {
    var canGetStaticValue =
      field.IsStatic && // must be `static` or `const`
      (field.IsLiteral || field.IsInitOnly) && // must be `const` or `readonly`
      !field.FieldType.ContainsGenericParameters; // type of field must not be is generic type parameter

    if (!(canGetStaticValue && field.TryGetValue(null, out var val))) {
      if (!asPropertyDeclaration && !options.MemberDeclaration.OmitEndOfStatement)
        builder.Append(';');

      return;
    }

    var valueDeclaration = CSharpFormatter.FormatValueDeclaration(
      val: val,
      typeOfValue: field.FieldType,
      options: CSharpFormatter.ValueFormatOptions.FromGeneratorOptions(
        options: options,
        tryFindConstantField: field.FieldType != field.DeclaringType
      )
    );

    if (string.IsNullOrEmpty(valueDeclaration)) {
      var stringifiedValue = val switch {
        string s => s,
        DateTime dt => dt.ToString("o"),
        DateTimeOffset dto => dto.ToString("o"),
        IFormattable formattable => formattable.ToString(
          format: null,
          formatProvider: CultureInfo.InvariantCulture // TODO: specific culture
        ),
        null => "null",
        _ => val.ToString() ?? string.Empty,
      };

      builder
        .Append(asPropertyDeclaration ? " // = \"" : "; // = \"")
        .Append(CSharpFormatter.EscapeString(stringifiedValue, escapeDoubleQuote: true))
        .Append('"');

      return;
    }

    builder.Append(" = ").Append(valueDeclaration);

    if (!options.MemberDeclaration.OmitEndOfStatement)
      builder.Append(';');
  }

#pragma warning disable CA1502 // TODO: reduce code complexity
  private static string? GeneratePropertyDeclaration(
    PropertyInfo property,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    var explicitInterfaceMethod = property
      .GetAccessors(true)
      .Select(
        a => a.TryFindExplicitInterfaceMethod(
          out var eim,
          findOnlyPublicInterfaces: options.IgnorePrivateOrAssembly
        )
          ? eim
          : null // Type.GetInterfaceMap() is not supported in reflection-only context
      )
      .FirstOrDefault(static a => a is not null);

    var explicitInterface = explicitInterfaceMethod?.DeclaringType;

    if (
      explicitInterface == null &&
      options.IgnorePrivateOrAssembly &&
      property.GetAccessors(true).All(static a => a.IsPrivateOrAssembly())
    ) {
      return null;
    }

    var emitGetAccessor =
      property.GetMethod != null &&
      !(explicitInterface == null && options.IgnorePrivateOrAssembly && property.GetMethod.IsPrivateOrAssembly());
    var emitSetAccessor =
      property.SetMethod != null &&
      !(explicitInterface == null && options.IgnorePrivateOrAssembly && property.SetMethod.IsPrivateOrAssembly());

    var indexParameters = property.GetIndexParameters();

    referencingNamespaces?.UnionWith(
      CSharpFormatter.ToNamespaceList(
        property.PropertyType,
        translateLanguagePrimitiveTypes: options.TranslateLanguagePrimitiveTypeDeclaration
      )
    );
    referencingNamespaces?.UnionWith(
      indexParameters.SelectMany(
        ip => CSharpFormatter.ToNamespaceList(
          ip.ParameterType,
          translateLanguagePrimitiveTypes: options.TranslateLanguagePrimitiveTypeDeclaration
        )
      )
    );

    var sb = new StringBuilder();
    var memberOptions = options.MemberDeclaration;

    AppendAttributeList(
      property.GetBackingField(),
      sb,
      referencingNamespaces,
      options
    );

    AppendMemberModifiers(
      sb,
      property,
      asExplicitInterfaceMember: explicitInterfaceMethod is not null,
      options: options,
      setMethodAccessibility: out var setAccessibility,
      getMethodAccessibility: out var getAccessibility
    );

    sb.Append(
      property.FormatTypeName(
#pragma warning disable SA1114
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
        nullabilityInfoContext: memberOptions.NullabilityInfoContext,
        nullabilityInfoContextLockObject: memberOptions.NullabilityInfoContextLockObject,
#endif
        typeWithNamespace: memberOptions.WithNamespace
#pragma warning restore SA1114
      )
    ).Append(' ');

    var defaultMemberName = property
      .GetDeclaringTypeOrThrow()
      .GetCustomAttributesData()
      .FirstOrDefault(
        static d => string.Equals(typeof(DefaultMemberAttribute).FullName, d.AttributeType.FullName, StringComparison.Ordinal)
      )
      ?.ConstructorArguments
      ?.FirstOrDefault()
      .Value
      as string;

    if (0 < indexParameters.Length && string.Equals(property.Name, defaultMemberName, StringComparison.Ordinal)) {
      // indexer
      sb.Append(
        GenerateMemberName(
          property,
          memberOptions.WithDeclaringTypeName ? defaultMemberName! : "this",
          options
        )
      );
    }
    else if (explicitInterface == null) {
      sb.Append(
        GenerateMemberName(
          property,
          options
        )
      );
    }
    else {
      sb.Append(
        GenerateMemberName(
          property,
          string.Concat(
            explicitInterface.FormatTypeName(typeWithNamespace: memberOptions.WithNamespace),
            ".",
#if SYSTEM_STRING_CONCAT_READONLYSPAN_OF_CHAR
            property.Name.AsSpan(property.Name.LastIndexOf('.') + 1)
#else
            property.Name.Substring(property.Name.LastIndexOf('.') + 1)
#endif
          ),
          options
        )
      );
    }

    if (0 < indexParameters.Length) {
      sb
        .Append('[')
        .Append(
          CSharpFormatter.FormatParameterList(
            indexParameters,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
            nullabilityInfoContext: memberOptions.NullabilityInfoContext,
#endif
            typeWithNamespace: memberOptions.WithNamespace,
            useDefaultLiteral: options.ValueDeclaration.UseDefaultLiteral
          )
        )
        .Append("] ");
    }
    else {
      sb.Append(' ');
    }

    sb.Append("{ ");

    if (emitGetAccessor) {
      GenerateAccessorDeclaration(
        "get",
        property.GetMethod!,
        explicitInterface is null ? getAccessibility : null,
        sb,
        referencingNamespaces,
        options
      );
    }

    if (emitSetAccessor) {
      GenerateAccessorDeclaration(
        property.IsSetMethodInitOnly() ? "init" : "set",
        property.SetMethod!,
        explicitInterface is null ? setAccessibility : null,
        sb,
        referencingNamespaces,
        options
      );
    }

    sb.Append('}');

    if (emitGetAccessor && property.GetBackingField() is { } backingField) {
      TryAppendFieldStaticValue(
        backingField,
        asPropertyDeclaration: true,
        sb,
        options
      );
    }

    return sb.ToString();
  }
#pragma warning restore CA1502

  private static void GenerateAccessorDeclaration(
    string accessor,
    MethodInfo accessorMethod,
    string? accessibilityModifier,
    StringBuilder builder,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    // accessor parameter attribute list
    AppendAttributeList(
      accessorMethod.ReturnParameter.ParameterType == typeof(void)
        ? accessorMethod.GetParameters().First() // property set/init accessor or event add/remove accessor
        : accessorMethod.ReturnParameter, // property get accessor
      builder,
      referencingNamespaces,
      options
    );

    // accessor method attribute list
    AppendAttributeList(
      accessorMethod,
      builder,
      referencingNamespaces,
      options
    );

    if (!string.IsNullOrEmpty(accessibilityModifier))
      builder.Append(accessibilityModifier).Append(' ');

    builder.Append(accessor);

    var bodyOption = accessorMethod.IsAbstract
      ? MethodBodyOption.EmptyImplementation
      : options.MemberDeclaration.AccessorBody;

    builder.Append(bodyOption switch {
      MethodBodyOption.ThrowNotImplementedException => " => throw new NotImplementedException(); ",
      MethodBodyOption.ThrowNull => " => throw null; ",

      // MethodBodyOption.None or
      // MethodBodyOption.EmptyImplementation or
      _ => "; ",
    });
  }

  private static StringBuilder AppendAttributeList(
    ICustomAttributeProvider? attributeProvider,
    StringBuilder builder,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    if (attributeProvider is null)
      return builder;

    var initialLength = builder.Length;
    var attributeList = GenerateAttributeList(
      attributeProvider,
      referencingNamespaces,
      options
    );

    builder
#if SYSTEM_TEXT_STRINGBUILDER_APPENDJOIN
      .AppendJoin(' ', attributeList);
#else
      .Append(string.Join(" ", attributeList));
#endif

    if (initialLength < builder.Length)
      builder.Append(' ');

    return builder;
  }

  private static string? GenerateDelegateDeclaration(
    Type d,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
    => GenerateMethodOrDelegateDeclaration(
      m: d.GetDelegateSignatureMethod() ?? throw new InvalidOperationException("can not get signature of the delegate"),
      asDelegateDeclaration: true,
      referencingNamespaces: referencingNamespaces,
      options: options
    );

  private static string? GenerateMethodBaseDeclaration(
    MethodBase m,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
    => GenerateMethodOrDelegateDeclaration(
      m: m,
      asDelegateDeclaration: false,
      referencingNamespaces: referencingNamespaces,
      options: options
    );

#pragma warning disable CA1502 // TODO: reduce code complexity
  private static string? GenerateMethodOrDelegateDeclaration(
    MethodBase m,
    bool asDelegateDeclaration,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    MethodInfo? explicitInterfaceMethod = null;

    if (asDelegateDeclaration) {
      GenerateDeclarationValidateTypeArgument(m.GetDeclaringTypeOrThrow(), nameof(m));
    }
    else {
      var isExplicitInterfaceMethod = m.TryFindExplicitInterfaceMethod(
        out explicitInterfaceMethod,
        findOnlyPublicInterfaces: options.IgnorePrivateOrAssembly
      );

      if (!isExplicitInterfaceMethod && options.IgnorePrivateOrAssembly && m.IsPrivateOrAssembly())
        return null;
    }

    var formattingOptions = new {
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      NullabilityInfoContext = asDelegateDeclaration
        ? options.TypeDeclaration.NullabilityInfoContext
        : options.MemberDeclaration.NullabilityInfoContext,
      NullabilityInfoContextLockObject = asDelegateDeclaration
        ? options.TypeDeclaration.NullabilityInfoContextLockObject
        : options.MemberDeclaration.NullabilityInfoContextLockObject,
#endif
      FormatTypeWithNamespace = asDelegateDeclaration
        ? options.TypeDeclaration.WithNamespace
        : options.MemberDeclaration.WithNamespace,
      FormatTypeWithDeclaringTypeName = asDelegateDeclaration
        ? options.TypeDeclaration.WithDeclaringTypeName
        : options.MemberDeclaration.WithDeclaringTypeName,
      OmitEndOfStatement = asDelegateDeclaration
        ? options.TypeDeclaration.OmitEndOfStatement
        : options.MemberDeclaration.OmitEndOfStatement,
      TranslateLanguagePrimitiveType = options.TranslateLanguagePrimitiveTypeDeclaration,
    };

    var method = m as MethodInfo;
    var methodReturnParameter = method?.ReturnParameter;
    string? methodReturnType;

    try {
      methodReturnType = methodReturnParameter?.FormatTypeName(
#pragma warning disable SA1114
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
        nullabilityInfoContext: formattingOptions.NullabilityInfoContext,
        nullabilityInfoContextLockObject: formattingOptions.NullabilityInfoContextLockObject,
#endif
        typeWithNamespace: formattingOptions.FormatTypeWithNamespace
#pragma warning restore SA1114
      );

      if (methodReturnParameter is not null) {
        referencingNamespaces?.UnionWith(
          CSharpFormatter.ToNamespaceList(
            methodReturnParameter.ParameterType,
            translateLanguagePrimitiveTypes: options.TranslateLanguagePrimitiveTypeDeclaration
          )
        );
      }
    }
    catch (TypeLoadException) {
      // FIXME: https://github.com/smdn/Smdn.Reflection.ReverseGenerating/issues/31
      methodReturnType = "<unknown>";
    }

    string? methodReturnTypeAttributes;

    try {
      methodReturnTypeAttributes = methodReturnParameter is null
        ? null
        : GenerateParameterAttributeList(methodReturnParameter, referencingNamespaces, options);
    }
    catch (TypeLoadException) {
      // FIXME: https://github.com/smdn/Smdn.Reflection.ReverseGenerating/issues/31
      methodReturnTypeAttributes = "[...]";
    }

    var methodGenericParameters = m.IsGenericMethod
      ? GenerateTypeParameterDeclaration(
          genericArguments: m.GetGenericArguments(),
          referencingNamespaces: referencingNamespaces,
          options: options,
          formatTypeWithNamespace: formattingOptions.FormatTypeWithNamespace
        )
      : null;

    string? methodParameterList;

    try {
      methodParameterList = string.Join(
        ", ",
        m.GetParameters().Select(
          p => GenerateParameterDeclaration(p, referencingNamespaces, options)
        )
      );
    }
    catch (TypeLoadException) {
      // FIXME: https://github.com/smdn/Smdn.Reflection.ReverseGenerating/issues/31
      methodParameterList = "...";
    }

    var genericParameters = method is null
      ? null
      : asDelegateDeclaration
        ? m.GetDeclaringTypeOrThrow().GetGenericArguments()
        : method.IsGenericMethodDefinition
          ? method.GetGenericArguments()
          : null;
    var methodConstraints = genericParameters is null
      ? null
      : GenerateGenericParameterConstraintClause(
          genericParameters: genericParameters,
          referencingNamespaces: referencingNamespaces,
          options: options
        );
    string? methodName = null;
    var isFinalizer = false;

    if (asDelegateDeclaration) {
      methodName = m.GetDeclaringTypeOrThrow().FormatTypeName(
        attributeProvider: null,
        typeWithNamespace: false,
        withDeclaringTypeName: formattingOptions.FormatTypeWithDeclaringTypeName,
        translateLanguagePrimitiveType: formattingOptions.TranslateLanguagePrimitiveType
      );
    }
    else if (m.IsSpecialName) {
      // constructors, operator overloads, etc
      methodName = CSharpFormatter.FormatSpecialNameMethod(m, out var nameType);

      switch (nameType) {
        case MethodSpecialName.None: break;
        case MethodSpecialName.Unknown: break;
        case MethodSpecialName.Explicit: methodName += " " + methodReturnType; methodReturnType = null; break;
        case MethodSpecialName.CheckedExplicit: methodName += " " + methodReturnType; methodReturnType = null; break;
        case MethodSpecialName.Implicit: methodName += " " + methodReturnType; methodReturnType = null; break;
        case MethodSpecialName.Constructor: methodReturnType = null; methodName = GenerateMemberName(m, methodName, options); break;
        default: methodName += " "; break;
      }
    }
    else if (
      method is not null &&
      method.IsFamily &&
      string.Equals(method.Name, "Finalize", StringComparison.Ordinal)
    ) {
      // destructors
      isFinalizer = true;

      var declaringType = m.GetDeclaringTypeOrThrow();

      methodName = GenerateMemberName(
        m,
        "~" + (declaringType.IsGenericType ? declaringType.GetGenericTypeName() : declaringType.Name),
        options
      );
      methodReturnType = null;
      methodReturnTypeAttributes = null;
      methodParameterList = null;
      methodConstraints = null;
    }
    else if (explicitInterfaceMethod is not null) {
      methodName = GenerateMemberName(
        m,
        string.Concat(
          explicitInterfaceMethod
            .GetDeclaringTypeOrThrow()
            .FormatTypeName(typeWithNamespace: formattingOptions.FormatTypeWithNamespace),
          ".",
          explicitInterfaceMethod.Name
        ),
        options
      );
    }
    else {
      // standard methods
      methodName = GenerateMemberName(m, options);
    }

    var sb = new StringBuilder();

    if (!string.IsNullOrEmpty(methodReturnTypeAttributes))
      sb.Append(methodReturnTypeAttributes).Append(' ');

    if (asDelegateDeclaration && m.GetDeclaringTypeOrThrow().IsHidingInheritedType(nonPublic: true))
      sb.Append("new ");

    if (!isFinalizer) {
      AppendMemberModifiers(
        sb,
        m,
        asExplicitInterfaceMember: explicitInterfaceMethod is not null,
        options: options
      );
    }

    if (asDelegateDeclaration)
      sb.Append("delegate ");

    if (!string.IsNullOrEmpty(methodReturnType))
      sb.Append(methodReturnType).Append(' ');

    sb.Append(methodName);

    if (!string.IsNullOrEmpty(methodGenericParameters))
      sb.Append(methodGenericParameters);

    sb.Append('(');

    if (!string.IsNullOrEmpty(methodParameterList))
      sb.Append(methodParameterList);

    sb.Append(')');

    if (!string.IsNullOrEmpty(methodConstraints))
      sb.Append(' ').Append(methodConstraints);

    var endOfStatement = formattingOptions.OmitEndOfStatement
      ? null
      : ";";

    if (asDelegateDeclaration)
      return sb.Append(endOfStatement).ToString();

    var (methodBody, endOfMethodBody) = m.IsAbstract
      ? options.MemberDeclaration.MethodBody switch {
        MethodBodyOption.None => (null, null),
        MethodBodyOption.EmptyImplementation or
        MethodBodyOption.ThrowNotImplementedException or
        MethodBodyOption.ThrowNull => (null, endOfStatement),
        _ => throw new InvalidOperationException($"invalid value of {nameof(MethodBodyOption)} ({options.MemberDeclaration.MethodBody})"),
      }
      : options.MemberDeclaration.MethodBody switch {
        MethodBodyOption.None => (null, null),
        MethodBodyOption.EmptyImplementation => (" {}", null),
        MethodBodyOption.ThrowNotImplementedException => (" => throw new NotImplementedException()", endOfStatement),
        MethodBodyOption.ThrowNull => (" => throw null", endOfStatement),
        _ => throw new InvalidOperationException($"invalid value of {nameof(MethodBodyOption)} ({options.MemberDeclaration.MethodBody})"),
      };

    return sb.Append(methodBody).Append(endOfMethodBody).ToString();
  }
#pragma warning restore CA1502

  private static string GenerateTypeParameterDeclaration(
    IEnumerable<Type> genericArguments,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options,
    bool formatTypeWithNamespace
  )
    => string.Concat(
      "<",
      string.Join(
        ", ",
        genericArguments.Select(p => {
          var name = p.FormatTypeName(typeWithNamespace: formatTypeWithNamespace);
          var attributeList = GenerateAttributeList(p, referencingNamespaces, options);

          return attributeList.Any()
            ? string.Join(" ", attributeList) + " " + name
            : name;
        })
      ),
      ">"
    );

  private static string GenerateParameterDeclaration(
    ParameterInfo p,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    var isDelegate = p.Member == p.Member.DeclaringType?.GetDelegateSignatureMethod();
#endif
    var param = CSharpFormatter.FormatParameterCore(
      p,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      nullabilityInfoContext: isDelegate
        ? options.TypeDeclaration.NullabilityInfoContext
        : options.MemberDeclaration.NullabilityInfoContext,
      nullabilityInfoContextLockObject: isDelegate
        ? options.TypeDeclaration.NullabilityInfoContextLockObject
        : options.MemberDeclaration.NullabilityInfoContextLockObject,
#endif
      typeWithNamespace: options.ParameterDeclaration.WithNamespace,
      typeWithDeclaringTypeName: options.ParameterDeclaration.WithDeclaringTypeName,
      valueFormatOptions: CSharpFormatter.ValueFormatOptions.FromGeneratorOptions(options, tryFindConstantField: true)
    );
    var paramAttributeList = GenerateParameterAttributeList(
      p,
      referencingNamespaces,
      options
    );

    referencingNamespaces?.UnionWith(
      CSharpFormatter.ToNamespaceList(
        p.ParameterType,
        translateLanguagePrimitiveTypes: options.TranslateLanguagePrimitiveTypeDeclaration
      )
    );

    if (string.IsNullOrEmpty(paramAttributeList))
      return param;
    else
      return string.Concat(paramAttributeList, " ", param);
  }

  private static string GenerateParameterAttributeList(
    ParameterInfo p,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
    => string.Join(
      " ",
      GenerateAttributeList(
        p,
        referencingNamespaces,
        options
      )
    );

  private static string? GenerateEventDeclaration(
    EventInfo ev,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    var explicitInterfaceMethod = ev
      .GetMethods(true)
      .Select(
        evm => evm.TryFindExplicitInterfaceMethod(
          out var eim,
          findOnlyPublicInterfaces: options.IgnorePrivateOrAssembly
        )
          ? eim
          : null // Type.GetInterfaceMap() is not supported in reflection-only context
      )
      .FirstOrDefault(static evm => evm is not null);

    var explicitInterface = explicitInterfaceMethod?.DeclaringType;

    if (
      explicitInterface == null &&
      options.IgnorePrivateOrAssembly &&
      ev.GetMethods(true).All(static m => m.IsPrivateOrAssembly())
    ) {
      return null;
    }

    referencingNamespaces?.UnionWith(
      CSharpFormatter.ToNamespaceList(
        ev.GetEventHandlerTypeOrThrow(),
        translateLanguagePrimitiveTypes: options.TranslateLanguagePrimitiveTypeDeclaration
      )
    );

    var sb = new StringBuilder();
    var memberOptions = options.MemberDeclaration;

    AppendAttributeList(
      ev.GetBackingField(),
      sb,
      referencingNamespaces,
      options
    );

    AppendMemberModifiers(
      sb,
      ev,
      asExplicitInterfaceMember: explicitInterface is not null,
      options: options
    );

    sb
      .Append("event ")
      .Append(
        ev.FormatTypeName(
#pragma warning disable SA1114
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
          nullabilityInfoContext: memberOptions.NullabilityInfoContext,
          nullabilityInfoContextLockObject: memberOptions.NullabilityInfoContextLockObject,
#endif
          typeWithNamespace: memberOptions.WithNamespace
#pragma warning restore SA1114
        )
      )
      .Append(' ');

    if (explicitInterface == null) {
      sb.Append(
        GenerateMemberName(
          ev,
          options
        )
      );
    }
    else {
      sb.Append(
        GenerateMemberName(
          ev,
          string.Concat(
            explicitInterface.FormatTypeName(typeWithNamespace: memberOptions.WithNamespace),
            ".",
#if SYSTEM_STRING_CONCAT_READONLYSPAN_OF_CHAR
            ev.Name.AsSpan(ev.Name.LastIndexOf('.') + 1)
#else
            ev.Name.Substring(ev.Name.LastIndexOf('.') + 1)
#endif
          ),
          options
        )
      );
    }

    static bool IsCompilerGeneratedAttribute(CustomAttributeData d)
      => d.AttributeType == typeof(CompilerGeneratedAttribute)
      || string.Equals(d.AttributeType.FullName, "System.Runtime.CompilerServices.CompilerGeneratedAttribute", StringComparison.Ordinal);

    static bool HasAttributeExceptCompilerGeneratedAttribute(MethodInfo? accessor, AttributeTypeFilter? filter)
    {
      if (accessor is null)
        return false;

      var attributesExceptCompilerGenerated = accessor
        .GetCustomAttributesData()
        .Where(static d => !IsCompilerGeneratedAttribute(d));

      if (filter is null)
        return attributesExceptCompilerGenerated.Any();
      else
        return attributesExceptCompilerGenerated.Any(d => filter(d.AttributeType, accessor));
    }

    var compilerGeneratedAccessors =
      (ev.AddMethod?.HasCompilerGeneratedAttribute() ?? false) &&
      (ev.RemoveMethod?.HasCompilerGeneratedAttribute() ?? false);

    var emitAccessor = !ev.GetDeclaringTypeOrThrow().IsInterface && !compilerGeneratedAccessors;

    emitAccessor |= HasAttributeExceptCompilerGeneratedAttribute(ev.AddMethod, options.AttributeDeclaration.TypeFilter);
    emitAccessor |= HasAttributeExceptCompilerGeneratedAttribute(ev.RemoveMethod, options.AttributeDeclaration.TypeFilter);

    if (!emitAccessor) {
      if (!memberOptions.OmitEndOfStatement)
        sb.Append(';');

      return sb.ToString();
    }

    sb.Append(" { ");

    GenerateAccessorDeclaration(
      "add",
      ev.AddMethod!,
      accessibilityModifier: null,
      sb,
      referencingNamespaces,
      options
    );

    GenerateAccessorDeclaration(
      "remove",
      ev.RemoveMethod!,
      accessibilityModifier: null,
      sb,
      referencingNamespaces,
      options
    );

    return sb.Append('}').ToString();
  }

  private static string GenerateMemberName(
    MemberInfo member,
    GeneratorOptions options
  ) =>
    GenerateMemberName(
      member,
      member.Name,
      options
    );

  private static string GenerateMemberName(
    MemberInfo member,
    string memberName,
    GeneratorOptions options
  )
  {
    var withDeclaringTypeName = options.MemberDeclaration.WithDeclaringTypeName;

    if (member is FieldInfo field && field.GetDeclaringTypeOrThrow().IsEnum)
      withDeclaringTypeName = options.MemberDeclaration.WithEnumTypeName;

    if (withDeclaringTypeName) {
      return member.GetDeclaringTypeOrThrow().FormatTypeName(
        typeWithNamespace: options.MemberDeclaration.WithNamespace,
        translateLanguagePrimitiveType: options.TranslateLanguagePrimitiveTypeDeclaration
      ) + "." + memberName;
    }

    return memberName;
  }

  private static void AppendMemberModifiers(
    StringBuilder sb,
    MemberInfo member,
    bool asExplicitInterfaceMember,
    GeneratorOptions options
  )
    => AppendMemberModifiers(
      sb,
      member,
      asExplicitInterfaceMember,
      options,
      out _,
      out _
    );

#pragma warning disable CA1502 // TODO: reduce code complexity
  // TODO: extern, volatile
  private static void AppendMemberModifiers(
    StringBuilder sb,
    MemberInfo member,
    bool asExplicitInterfaceMember,
    GeneratorOptions options,
    out string setMethodAccessibility,
    out string getMethodAccessibility
  )
  {
    setMethodAccessibility = string.Empty;
    getMethodAccessibility = string.Empty;

    Accessibility? propertyGetMethodAccessibility = null;
    Accessibility? propertySetMethodAccessibility = null;

    static void AppendAccessibility(StringBuilder sb, MemberInfo? member, Accessibility accessibility)
    {
      var isInterfaceMember = member is not null && member.GetDeclaringTypeOrThrow().IsInterface;

      if (accessibility == Accessibility.Public && isInterfaceMember)
        return;

      sb.Append(CSharpFormatter.FormatAccessibility(accessibility)).Append(' ');
    }

    static void AppendMethodModifiers(StringBuilder sb, MethodBase? m)
    {
      if (m == null)
        return;

      var method = m as MethodInfo;

      if (method is null || !method.IsDelegateSignatureMethod()) {
        var isInterfaceMethod = m.GetDeclaringTypeOrThrow().IsInterface;

        if (m.IsStatic)
          sb.Append("static ");

        if (m.IsAbstract) {
          if (isInterfaceMethod) {
            if (m.IsStatic)
              sb.Append("abstract ");
          }
          else {
            sb.Append("abstract ");
          }
        }
        else if (!isInterfaceMethod && method is not null && method.IsOverride()) {
          if (m.IsFinal)
            sb.Append("sealed ");

          sb.Append("override ");
        }
        else if (m.IsVirtual && !m.IsFinal) {
          sb.Append("virtual ");
        }
        else if (
          m is MethodInfo methodMayBeAccessor &&
          methodMayBeAccessor.TryGetPropertyFromAccessorMethod(out var p) &&
#if !NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
#pragma warning disable CS8602
#endif
          p.GetAccessors(nonPublic: true).Any(static a => a.IsVirtual && !a.IsFinal)
#pragma warning restore CS8602
        ) {
          sb.Append("virtual ");
        }
      }

      if (method is not null && !method.IsPropertyAccessorMethod() && method.IsReadOnly())
        sb.Append("readonly ");

      var isAsyncStateMachine = m.GetCustomAttributesData().Any(
        static d => string.Equals(d.AttributeType.FullName, "System.Runtime.CompilerServices.AsyncStateMachineAttribute", StringComparison.Ordinal)
      );

      if (isAsyncStateMachine)
        sb.Append("async ");

      try {
        if (method is not null && method.GetParameters().Any(IsParameterUnsafe))
          sb.Append("unsafe ");
      }
      catch (TypeLoadException) {
        // FIXME: https://github.com/smdn/Smdn.Reflection.ReverseGenerating/issues/31
      }
    }

    static bool IsParameterUnsafe(ParameterInfo p)
    {
      if (p.ParameterType.IsPointer)
        return true;
      if (p.ParameterType.IsByRef && p.ParameterType.HasElementType && p.ParameterType.GetElementType()!.IsPointer)
        return true;

      return false;
    }

    try {
      if (member.IsHidingInheritedMember(nonPublic: true))
        sb.Append("new ");
    }
    catch (TypeLoadException) {
      // FIXME: https://github.com/smdn/Smdn.Reflection.ReverseGenerating/issues/31
    }

  SWITCH_MEMBER_TYPE:
    switch (member) {
      case EventInfo ev:
        member = ev.GetMethods(true).First();
        goto SWITCH_MEMBER_TYPE;

      case FieldInfo f:
        if (!asExplicitInterfaceMember && options.MemberDeclaration.WithAccessibility)
          AppendAccessibility(sb, f, f.GetAccessibility());

        if (f.IsStatic && !f.IsLiteral)
          sb.Append("static ");

        if (f.IsInitOnly)
          sb.Append("readonly ");

        if (f.IsLiteral)
          sb.Append("const ");

        if (f.IsRequired())
          sb.Append("required ");

        break;

      case PropertyInfo p:
        var mostOpenAccessibility = p.GetAccessors(true).Select(Smdn.Reflection.MemberInfoExtensions.GetAccessibility).Max();

        if (!asExplicitInterfaceMember && options.MemberDeclaration.WithAccessibility)
          AppendAccessibility(sb, p, mostOpenAccessibility);

        if (p.GetMethod != null) {
          var accessorAccessibility = p.GetMethod.GetAccessibility();

          if (accessorAccessibility < mostOpenAccessibility)
            propertyGetMethodAccessibility = accessorAccessibility;
        }

        if (p.SetMethod != null) {
          var accessorAccessibility = p.SetMethod.GetAccessibility();

          if (accessorAccessibility < mostOpenAccessibility)
            propertySetMethodAccessibility = accessorAccessibility;
        }

        AppendMethodModifiers(sb, p.GetAccessors(true).FirstOrDefault());

        if (p.IsRequired())
          sb.Append("required ");
        else if (p.IsAccessorReadOnly())
          sb.Append("readonly ");

        break;

      case MethodBase m:
        if (!asExplicitInterfaceMember && m != m.DeclaringType?.TypeInitializer) {
          if (m is MethodInfo method && method.IsDelegateSignatureMethod()) {
            if (options.TypeDeclaration.WithAccessibility)
              AppendAccessibility(sb, null, m.GetDeclaringTypeOrThrow().GetAccessibility());
          }
          else {
            if (options.MemberDeclaration.WithAccessibility)
              AppendAccessibility(sb, m, m.GetAccessibility());
          }
        }

        AppendMethodModifiers(sb, m);

        break;
    }

    if (propertyGetMethodAccessibility.HasValue)
      getMethodAccessibility = CSharpFormatter.FormatAccessibility(propertyGetMethodAccessibility.Value);
    if (propertySetMethodAccessibility.HasValue)
      setMethodAccessibility = CSharpFormatter.FormatAccessibility(propertySetMethodAccessibility.Value);
  }
#pragma warning restore CA1502
}
