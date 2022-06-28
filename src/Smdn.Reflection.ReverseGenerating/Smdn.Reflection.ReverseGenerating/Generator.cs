// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
#define SYSTEM_STRING_CONCAT_READONLYSPAN_OF_CHAR
#endif

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Smdn.Reflection.ReverseGenerating;

public static partial class Generator {
  public static string GenerateTypeDeclaration(
    Type t,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  ) =>
    GenerateTypeDeclaration(
      t,
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
      t,
      true,
      referencingNamespaces,
      options ?? throw new ArgumentNullException(nameof(options))
    );

  private static IEnumerable<string> GenerateTypeDeclaration(
    Type t,
    bool generateExplicitBaseTypeAndInterfaces,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    var accessibilities = options.TypeDeclaration.WithAccessibility
      ? CSharpFormatter.FormatAccessibility(t.GetAccessibility()) + " "
      : string.Empty;
    var typeName = t.FormatTypeName(
      typeWithNamespace: false,
      withDeclaringTypeName: options.TypeDeclaration.WithDeclaringTypeName,
      translateLanguagePrimitiveType: options.TranslateLanguagePrimitiveTypeDeclaration
    );

    if (t.IsConcreteDelegate()) {
      yield return GenerateDelegateDeclaration(t, referencingNamespaces, options)!;
      yield break;
    }

    var genericArgumentConstraints = t
      .GetGenericArguments()
      .Select(
        arg => GenerateGenericArgumentConstraintDeclaration(arg, referencingNamespaces, options)
      )
      .Where(static d => !string.IsNullOrEmpty(d))
      .ToList();

    string GetSingleLineGenericArgumentConstraintsDeclaration()
      => genericArgumentConstraints.Count == 0 ? string.Empty : " " + string.Join(" ", genericArgumentConstraints);

    if (t.IsEnum) {
      yield return $"{accessibilities}enum {typeName} : {t.GetEnumUnderlyingType().FormatTypeName()}";
      yield break;
    }

    string typeDeclaration;

    if (t.IsInterface) {
      typeDeclaration = $"{accessibilities}interface {typeName}";
    }
    else if (t.IsValueType) {
      var isReadOnly = t.IsReadOnlyValueType() ? "readonly " : string.Empty;
      var isByRefLike = t.IsByRefLikeValueType() ? "ref " : string.Empty;

      typeDeclaration = $"{accessibilities}{isReadOnly}{isByRefLike}struct {typeName}";
    }
    else {
      string? modifier = null;

      if (t.IsAbstract && t.IsSealed)
        modifier = "static ";
      else if (t.IsAbstract)
        modifier = "abstract ";
      else if (t.IsSealed)
        modifier = "sealed ";

      typeDeclaration = $"{accessibilities}{modifier}class {typeName}";
    }

    if (!generateExplicitBaseTypeAndInterfaces) {
      yield return typeDeclaration + GetSingleLineGenericArgumentConstraintsDeclaration();
      yield break;
    }

    var baseTypeList = GenerateExplicitBaseTypeAndInterfaces(t, referencingNamespaces, options).ToList();

    if (baseTypeList.Count <= 1) {
      var baseTypeDeclaration = baseTypeList.Count == 0
        ? string.Empty
        : " : " + baseTypeList[0];
      var genericArgumentConstraintDeclaration = GetSingleLineGenericArgumentConstraintsDeclaration();

      yield return typeDeclaration + baseTypeDeclaration + genericArgumentConstraintDeclaration;
    }
    else {
      yield return typeDeclaration + " :";

      for (var index = 0; index < baseTypeList.Count; index++) {
        if (index == baseTypeList.Count - 1)
          yield return options.Indent + baseTypeList[index];
        else
          yield return options.Indent + baseTypeList[index] + ",";
      }

      foreach (var constraint in genericArgumentConstraints) {
        yield return options.Indent + constraint;
      }
    }
  }

  public static string GenerateGenericArgumentConstraintDeclaration(
    Type genericArgument,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    static bool HasUnmanagedConstraint(Type genericParameter)
      => genericParameter.CustomAttributes.Any(
        static attr => attr.AttributeType.FullName.Equals("System.Runtime.CompilerServices.IsUnmanagedAttribute", StringComparison.Ordinal)
      );

    static bool IsNullableAttribute(CustomAttributeData attr)
      => attr.AttributeType.FullName.Equals("System.Runtime.CompilerServices.NullableAttribute", StringComparison.Ordinal);

    static bool IsNullableContextAttribute(CustomAttributeData attr)
      => attr.AttributeType.FullName.Equals("System.Runtime.CompilerServices.NullableContextAttribute", StringComparison.Ordinal);

    // ref: https://github.com/dotnet/roslyn/blob/main/docs/features/nullable-metadata.md#type-parameters
    static bool HasNotNullConstraint(Type genericParameter)
    {
      var attrNullable = genericParameter.CustomAttributes.FirstOrDefault(IsNullableAttribute);
      var attrNullableContext =
        genericParameter.DeclaringMethod?.CustomAttributes?.FirstOrDefault(IsNullableContextAttribute) ??
        genericParameter.DeclaringType.CustomAttributes.FirstOrDefault(IsNullableContextAttribute);

      const byte notAnnotated = 1;

      if (attrNullableContext is not null && notAnnotated.Equals(attrNullableContext.ConstructorArguments[0].Value))
        // `#nullable enable` context
        return attrNullable is null;
      else
        // `#nullable disable` context
        return attrNullable is not null && notAnnotated.Equals(attrNullable.ConstructorArguments[0].Value);
    }

    static bool IsValueType(Type t) => string.Equals(t.FullName, typeof(ValueType).FullName, StringComparison.Ordinal);
    static bool IsNotValueType(Type t) => !string.Equals(t.FullName, typeof(ValueType).FullName, StringComparison.Ordinal);

    static IEnumerable<string> GetGenericArgumentConstraintsOf(
      Type genericParameter,
      ISet<string>? referencingNns,
      bool typeWithNamespace
    )
    {
      var constraintAttrs = genericParameter.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
      IEnumerable<Type> constraintTypes = genericParameter.GetGenericParameterConstraints();
      IEnumerable<Type> constraintTypesExceptValueType = constraintTypes;

      referencingNns?.UnionWith(constraintTypes.Where(IsNotValueType).SelectMany(CSharpFormatter.ToNamespaceList));

      if (
        constraintAttrs == GenericParameterAttributes.None &&
        HasNotNullConstraint(genericParameter)
      ) {
        yield return "notnull";
      }

      if (
        constraintAttrs.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint) &&
        constraintAttrs.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint) &&
        constraintTypes.Any(IsValueType)
      ) {
        constraintAttrs &= ~GenericParameterAttributes.NotNullableValueTypeConstraint;
        constraintAttrs &= ~GenericParameterAttributes.DefaultConstructorConstraint;
        constraintTypesExceptValueType = constraintTypes.Where(IsNotValueType);

        if (HasUnmanagedConstraint(genericParameter))
          yield return "unmanaged";
        else
          yield return "struct";
      }
      else if (constraintAttrs.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint)) {
        yield return "class";
      }
      else if (constraintAttrs.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint)) {
        if (HasUnmanagedConstraint(genericParameter))
          yield return "unmanaged";
        else
          yield return "struct";
      }

      var orderedConstraintTypeNames = constraintTypesExceptValueType
        .Select(i => i.FormatTypeName(typeWithNamespace: typeWithNamespace))
        .OrderBy(static name => name, StringComparer.Ordinal);

      foreach (var ctn in orderedConstraintTypeNames)
        yield return ctn;

      if (constraintAttrs.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
        yield return "new()";
    }

    var constraints = string.Join(
      ", ",
      GetGenericArgumentConstraintsOf(
        genericArgument,
        referencingNamespaces,
        genericArgument.DeclaringMethod == null
          ? options.TypeDeclaration.WithNamespace
          : options.MemberDeclaration.WithNamespace
      )
    );

    if (0 < constraints.Length)
      return $"where {genericArgument.FormatTypeName(typeWithNamespace: false)} : {constraints}";

    return string.Empty;
  }

  public static IEnumerable<string> GenerateExplicitBaseTypeAndInterfaces(
    Type t,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    if (options == null)
      throw new ArgumentNullException(nameof(options));

    return t
      .GetExplicitBaseTypeAndInterfaces()
      .Where(type => !(options.IgnorePrivateOrAssembly && type.IsPrivateOrAssembly()))
      .Select(type => {
        referencingNamespaces?.UnionWith(CSharpFormatter.ToNamespaceList(type));
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
      referencingNamespaces?.UnionWith(CSharpFormatter.ToNamespaceList(field.FieldType));

      sb
        .Append(GetMemberModifierOf(field, options))
        .Append(
          field.FormatTypeName(
#pragma warning disable SA1114
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
            nullabilityInfoContext: memberOptions.NullabilityInfoContext,
#endif
            typeWithNamespace: memberOptions.WithNamespace
#pragma warning restore SA1114
          )
        )
        .Append(' ')
        .Append(GenerateMemberName(field, options));

      if (field.IsStatic && (field.IsLiteral || field.IsInitOnly) && !field.FieldType.ContainsGenericParameters) {
        if (field.TryGetValue(null, out var val)) {
          var valueDeclaration = CSharpFormatter.FormatValueDeclaration(
            val,
            field.FieldType,
            typeWithNamespace: memberOptions.WithNamespace,
            findConstantField: field.FieldType != field.DeclaringType,
            useDefaultLiteral: options.ValueDeclaration.UseDefaultLiteral
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

            sb
              .Append("; // = \"")
              .Append(CSharpFormatter.EscapeString(stringifiedValue, escapeDoubleQuote: true))
              .Append('"');
          }
          else {
            sb.Append(" = ").Append(valueDeclaration);

            if (!memberOptions.OmitEndOfStatement)
              sb.Append(';');
          }
        }
        else {
          if (!memberOptions.OmitEndOfStatement)
            sb.Append(';');
        }
      }
      else {
        if (!memberOptions.OmitEndOfStatement)
          sb.Append(';');
      }
    }

    return sb.ToString();
  }

  private static string? GeneratePropertyDeclaration(
    PropertyInfo property,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    var explicitInterface = property
      .GetAccessors(true)
      .Select(
        a => a.FindExplicitInterfaceMethod(
          findOnlyPublicInterfaces: options.IgnorePrivateOrAssembly
        )?.DeclaringType
      )
      .FirstOrDefault();

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
      CSharpFormatter.ToNamespaceList(property.PropertyType)
    );
    referencingNamespaces?.UnionWith(
      indexParameters.SelectMany(static ip => CSharpFormatter.ToNamespaceList(ip.ParameterType))
    );

    var sb = new StringBuilder();
    var memberOptions = options.MemberDeclaration;

    var backingField = property.GetBackingField();
    var backingFieldAttributeList = backingField is null
      ? Enumerable.Empty<string>()
      : GenerateAttributeList(backingField, referencingNamespaces, options);

    if (backingFieldAttributeList.Any()) {
      sb
#if SYSTEM_TEXT_STRINGBUILDER_APPENDJOIN
        .AppendJoin(' ', backingFieldAttributeList)
#else
        .Append(string.Join(" ", backingFieldAttributeList))
#endif
        .Append(' ');
    }

    var modifier = GetMemberModifierOf(
      property,
      options,
      out string setAccessibility,
      out string getAccessibility
    );

    if (explicitInterface == null)
      sb.Append(modifier);

    sb.Append(
      property.FormatTypeName(
#pragma warning disable SA1114
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
        nullabilityInfoContext: memberOptions.NullabilityInfoContext,
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
      if (explicitInterface == null && 0 < getAccessibility.Length)
        sb.Append(getAccessibility).Append(' ');

      var returnParameterAttributeList = GenerateAttributeList(
        property.GetMethod!.ReturnParameter,
        referencingNamespaces,
        options
      );

      if (returnParameterAttributeList.Any()) {
        sb
#if SYSTEM_TEXT_STRINGBUILDER_APPENDJOIN
          .AppendJoin(' ', returnParameterAttributeList)
#else
          .Append(string.Join(" ", returnParameterAttributeList))
#endif
          .Append(' ');
      }

      var accessorMethodAttributeList = GenerateAttributeList(
        property.GetMethod!,
        referencingNamespaces,
        options
      );

      if (accessorMethodAttributeList.Any()) {
        sb
#if SYSTEM_TEXT_STRINGBUILDER_APPENDJOIN
          .AppendJoin(' ', accessorMethodAttributeList)
#else
          .Append(string.Join(" ", accessorMethodAttributeList))
#endif
          .Append(' ');
      }

      sb.Append("get").Append(GenerateAccessorBody(property.GetMethod!, options));
    }

    if (emitSetAccessor) {
      if (explicitInterface == null && 0 < setAccessibility.Length)
        sb.Append(setAccessibility).Append(' ');

      var accessorParameterAttributeList = GenerateAttributeList(
        property.SetMethod!.GetParameters().First(),
        referencingNamespaces,
        options
      );

      if (accessorParameterAttributeList.Any()) {
        sb
#if SYSTEM_TEXT_STRINGBUILDER_APPENDJOIN
          .AppendJoin(' ', accessorParameterAttributeList)
#else
          .Append(string.Join(" ", accessorParameterAttributeList))
#endif
          .Append(' ');
      }

      var accessorMethodAttributeList = GenerateAttributeList(
        property.SetMethod!,
        referencingNamespaces,
        options
      );

      if (accessorMethodAttributeList.Any()) {
        sb
#if SYSTEM_TEXT_STRINGBUILDER_APPENDJOIN
          .AppendJoin(' ', accessorMethodAttributeList)
#else
          .Append(string.Join(" ", accessorMethodAttributeList))
#endif
          .Append(' ');
      }

      if (property.IsSetMethodInitOnly())
        sb.Append("init");
      else
        sb.Append("set");

      sb.Append(GenerateAccessorBody(property.SetMethod!, options));
    }

    sb.Append('}');

#if false
      if (p.CanRead)
        sb.Append(" = ").Append(p.GetConstantValue()).Append(";");
#endif

    return sb.ToString();

    static string GenerateAccessorBody(MethodInfo accessor, GeneratorOptions opts)
    {
      var bodyOption = accessor.IsAbstract
        ? MethodBodyOption.EmptyImplementation
        : opts.MemberDeclaration.MethodBody;

      return bodyOption switch {
        MethodBodyOption.ThrowNotImplementedException => " => throw new NotImplementedException(); ",
        MethodBodyOption.ThrowNull => " => throw null; ",

        // MethodBodyOption.None or
        // MethodBodyOption.EmptyImplementation or
        _ => "; ",
      };
    }
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

  private static string? GenerateMethodOrDelegateDeclaration(
    MethodBase m,
    bool asDelegateDeclaration,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
    MethodInfo? explicitInterfaceMethod = null;

    if (!asDelegateDeclaration) {
      m.TryFindExplicitInterfaceMethod(
        out explicitInterfaceMethod,
        findOnlyPublicInterfaces: options.IgnorePrivateOrAssembly
      );

      if (explicitInterfaceMethod == null && options.IgnorePrivateOrAssembly && m.IsPrivateOrAssembly())
        return null;
    }

    var formattingOptions = new {
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      NullabilityInfoContext = asDelegateDeclaration
        ? options.TypeDeclaration.NullabilityInfoContext
        : options.MemberDeclaration.NullabilityInfoContext,
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
    var methodModifiers = GetMemberModifierOf(m, options);
    var methodReturnType = method?.ReturnParameter?.FormatTypeName(
#pragma warning disable SA1114
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      nullabilityInfoContext: formattingOptions.NullabilityInfoContext,
#endif
      typeWithNamespace: formattingOptions.FormatTypeWithNamespace
#pragma warning restore SA1114
    );
    var methodReturnTypeAttributes = method is null
      ? null
      : GenerateParameterAttributeList(method.ReturnParameter, referencingNamespaces, options);
    var methodGenericParameters = m.IsGenericMethod
      ? string.Concat(
          "<",
          string.Join(
            ", ",
            m.GetGenericArguments().Select(
              t => t.FormatTypeName(typeWithNamespace: formattingOptions.FormatTypeWithNamespace)
            )
          ),
          ">"
        )
      : null;
    var methodParameterList = string.Join(
      ", ",
      m.GetParameters().Select(
        p => GenerateParameterDeclaration(p, referencingNamespaces, options)
      )
    );
    var genericArguments = method is null
      ? null
      : asDelegateDeclaration
        ? m.GetDeclaringTypeOrThrow().GetGenericArguments()
        : method.GetGenericArguments();
    var methodConstraints = genericArguments is null
      ? null
      : string.Join(
          " ",
          genericArguments
            .Select(
              arg => Generator.GenerateGenericArgumentConstraintDeclaration(arg, referencingNamespaces, options)
            )
            .Where(static d => !string.IsNullOrEmpty(d))
        );
    string? methodName = null;

    referencingNamespaces?.UnionWith(
      m
        .GetSignatureTypes()
        .Where(static mpt => !mpt.ContainsGenericParameters)
        .SelectMany(CSharpFormatter.ToNamespaceList)
    );

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
      var declaringType = m.GetDeclaringTypeOrThrow();

      methodName = GenerateMemberName(
        m,
        "~" + (declaringType.IsGenericType ? declaringType.GetGenericTypeName() : declaringType.Name),
        options
      );
      methodModifiers = null;
      methodReturnType = null;
      methodReturnTypeAttributes = null;
      methodParameterList = null;
      methodConstraints = null;
    }
    else if (explicitInterfaceMethod != null) {
      methodModifiers = null;
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

    sb.Append(methodModifiers);

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

  private static string GenerateParameterDeclaration(
    ParameterInfo p,
    ISet<string>? referencingNamespaces,
    GeneratorOptions options
  )
  {
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
    var isDelegate = p.Member == p.Member.DeclaringType?.GetDelegateSignatureMethod();
#endif
    var param = CSharpFormatter.FormatParameter(
      p,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      nullabilityInfoContext: isDelegate
        ? options.TypeDeclaration.NullabilityInfoContext
        : options.MemberDeclaration.NullabilityInfoContext,
#endif
      typeWithNamespace: options.MemberDeclaration.WithNamespace,
      useDefaultLiteral: options.ValueDeclaration.UseDefaultLiteral
    );
    var paramAttributeList = GenerateParameterAttributeList(
      p,
      referencingNamespaces,
      options
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
    var explicitInterface = ev
      .GetMethods(true)
      .Select(evm =>
        evm.FindExplicitInterfaceMethod(
          findOnlyPublicInterfaces: options.IgnorePrivateOrAssembly
        )?.DeclaringType
      )
      .FirstOrDefault();

    if (
      explicitInterface == null &&
      options.IgnorePrivateOrAssembly &&
      ev.GetMethods(true).All(static m => m.IsPrivateOrAssembly())
    ) {
      return null;
    }

    referencingNamespaces?.UnionWith(CSharpFormatter.ToNamespaceList(ev.GetEventHandlerTypeOrThrow()));

    var sb = new StringBuilder();
    var memberOptions = options.MemberDeclaration;

    var backingField = ev.GetBackingField();
    var backingFieldAttributeList = backingField is null
      ? Enumerable.Empty<string>()
      : GenerateAttributeList(backingField, referencingNamespaces, options);

    if (backingFieldAttributeList.Any()) {
      sb
#if SYSTEM_TEXT_STRINGBUILDER_APPENDJOIN
        .AppendJoin(' ', backingFieldAttributeList)
#else
        .Append(string.Join(" ", backingFieldAttributeList))
#endif
        .Append(' ');
    }

    if (explicitInterface == null)
      sb.Append(GetMemberModifierOf(ev.GetMethods(true).First(), options));

    sb
      .Append("event ")
      .Append(
        ev.FormatTypeName(
#pragma warning disable SA1114
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
          nullabilityInfoContext: memberOptions.NullabilityInfoContext,
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

    if (!memberOptions.OmitEndOfStatement)
      sb.Append(';');

    return sb.ToString();
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
    if (options.MemberDeclaration.WithDeclaringTypeName) {
      return member.GetDeclaringTypeOrThrow().FormatTypeName(
        typeWithNamespace: options.MemberDeclaration.WithNamespace,
        translateLanguagePrimitiveType: options.TranslateLanguagePrimitiveTypeDeclaration
      ) + "." + memberName;
    }

    return memberName;
  }

  private static string GetMemberModifierOf(MemberInfo member, GeneratorOptions options)
    => GetMemberModifierOf(member, options, out _, out _);

  // TODO: extern, volatile
  private static string GetMemberModifierOf(
    MemberInfo member,
    GeneratorOptions options,
    out string setMethodAccessibility,
    out string getMethodAccessibility
  )
  {
    setMethodAccessibility = string.Empty;
    getMethodAccessibility = string.Empty;

    if (member.GetDeclaringTypeOrThrow().IsInterface)
      return string.Empty;

    var modifiers = new List<string?>();
    string? accessibility = null;

    modifiers.Add(null); // placeholder for accessibility

    static IEnumerable<string> GetModifiersOfMethod(MethodBase? m)
    {
      if (m == null)
        yield break;

      if (!m.IsDelegateSignatureMethod()) {
        if (m.IsStatic)
          yield return "static";

        if (m.IsAbstract) {
          yield return "abstract";
        }
        else if (m is MethodInfo mi && mi.IsOverridden()) {
          if (m.IsFinal)
            yield return "sealed";

          yield return "override";
        }
        else if (m.IsVirtual && !m.IsFinal) {
          yield return "virtual";
        }
      }

      var isAsyncStateMachine = m.GetCustomAttributesData().Any(
        static d => string.Equals(d.AttributeType.FullName, "System.Runtime.CompilerServices.AsyncStateMachineAttribute", StringComparison.Ordinal)
      );

      if (isAsyncStateMachine)
        yield return "async";

      if (m is MethodInfo method && method.GetParameters().Any(static p => p.ParameterType.IsPointer))
        yield return "unsafe";

      // cannot detect 'new' modifier
      //  yield return "new";
    }

    switch (member) {
      case FieldInfo f:
        accessibility = options.MemberDeclaration.WithAccessibility
          ? CSharpFormatter.FormatAccessibility(f.GetAccessibility())
          : null;

        if (f.IsStatic && !f.IsLiteral) modifiers.Add("static");
        if (f.IsInitOnly) modifiers.Add("readonly");
        if (f.IsLiteral) modifiers.Add("const");

        break;

      case PropertyInfo p:
        var mostOpenAccessibility = p.GetAccessors(true).Select(Smdn.Reflection.MemberInfoExtensions.GetAccessibility).Max();

        accessibility = options.MemberDeclaration.WithAccessibility
          ? CSharpFormatter.FormatAccessibility(mostOpenAccessibility)
          : null;

        if (p.GetMethod != null) {
          var getAccessibility = p.GetMethod.GetAccessibility();

          if (getAccessibility < mostOpenAccessibility)
            getMethodAccessibility = CSharpFormatter.FormatAccessibility(getAccessibility);
        }

        if (p.SetMethod != null) {
          var setAccessibility = p.SetMethod.GetAccessibility();

          if (setAccessibility < mostOpenAccessibility)
            setMethodAccessibility = CSharpFormatter.FormatAccessibility(setAccessibility);
        }

        modifiers.AddRange(GetModifiersOfMethod(p.GetAccessors(true).FirstOrDefault()));

        break;

      case MethodBase m:
        accessibility = null;

        if (m.IsDelegateSignatureMethod()) {
          if (options.TypeDeclaration.WithAccessibility)
            accessibility = CSharpFormatter.FormatAccessibility(m.GetDeclaringTypeOrThrow().GetAccessibility());
        }
        else {
          if (options.MemberDeclaration.WithAccessibility)
            accessibility = CSharpFormatter.FormatAccessibility(m.GetAccessibility());
        }

        modifiers.AddRange(GetModifiersOfMethod(m));

        break;
    }

    if (member == member.DeclaringType?.TypeInitializer)
      accessibility = null;

    if (accessibility == null) {
      if (modifiers.Count <= 1)
        return string.Empty;

      return string.Join(" ", modifiers.Skip(1)) + " ";
    }

    modifiers[0] = accessibility;

    return string.Join(" ", modifiers) + " ";
  }
}
