// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;

namespace Smdn.Reflection.ReverseGenerating {
  public static partial class Generator {
    public static string GenerateTypeDeclaration(
      Type t,
      ISet<string> referencingNamespaces,
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
      ISet<string> referencingNamespaces,
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
      ISet<string> referencingNamespaces,
      GeneratorOptions options
    )
    {
      var accessibilities = options.TypeDeclarationWithAccessibility ? CSharpFormatter.FormatAccessibility(t.GetAccessibility()) + " " : string.Empty;
      var typeName = t.FormatTypeName(
        typeWithNamespace: false,
        withDeclaringTypeName: options.TypeDeclarationWithDeclaringTypeName,
        translateLanguagePrimitiveType: options.TranslateLanguagePrimitiveTypeDeclaration
      );

      var genericArgumentConstraints = t
        .GetGenericArguments()
        .Select(arg => GenerateGenericArgumentConstraintDeclaration(arg, referencingNamespaces, options))
        .Where(d => d != null)
        .ToList();

      string GetSingleLineGenericArgumentConstraintsDeclaration()
        => genericArgumentConstraints.Count == 0 ? string.Empty : " " + string.Join(" ", genericArgumentConstraints);

      if (t.IsEnum) {
        yield return $"{accessibilities}enum {typeName} : {t.GetEnumUnderlyingType().FormatTypeName()}";
        yield break;
      }

      if (t.IsConcreteDelegate()) {
        var signatureInfo = t.GetDelegateSignatureMethod();

        referencingNamespaces?.UnionWith(signatureInfo.GetSignatureTypes().Where(mpt => !mpt.ContainsGenericParameters).SelectMany(CSharpFormatter.ToNamespaceList));

        var genericArgumentConstraintDeclaration = genericArgumentConstraints.Count == 0 ? string.Empty : " " + string.Join(" ", genericArgumentConstraints);
        var returnType = signatureInfo.ReturnType.FormatTypeName(
          attributeProvider: signatureInfo.ReturnTypeCustomAttributes,
          typeWithNamespace: options.TypeDeclarationWithNamespace
        );
        var parameterList = CSharpFormatter.FormatParameterList(
          signatureInfo,
          typeWithNamespace: options.TypeDeclarationWithNamespace,
          useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral
        );
        var endOfStatement = options.TypeDeclarationOmitEndOfStatement
          ? string.Empty
          : ";";

        yield return $"{accessibilities}delegate {returnType} {typeName}({parameterList}){genericArgumentConstraintDeclaration}{endOfStatement}";
        yield break;
      }

      string typeDeclaration = null;

      if (t.IsInterface) {
        typeDeclaration = $"{accessibilities}interface {typeName}";
      }
      else if (t.IsValueType) {
        var isReadOnly = t.IsReadOnlyValueType() ? "readonly " : string.Empty;
        var isByRefLike = t.IsByRefLikeValueType() ? "ref " : string.Empty;

        typeDeclaration = $"{accessibilities}{isReadOnly}{isByRefLike}struct {typeName}";
      }
      else {
        string modifier = null;

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
        var baseTypeDeclaration = baseTypeList.Count == 0 ? string.Empty : " : " + baseTypeList[0];
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
      ISet<string> referencingNamespaces,
      GeneratorOptions options
    )
    {
      static bool IsUnmanagedTypeArgument(Type argument)
        => argument.GetCustomAttributes().Any(attr => attr.GetType().FullName == "System.Runtime.CompilerServices.IsUnmanagedAttribute");

      static bool IsNotNullTypeArgument(Type argument)
        => argument.GetCustomAttributes().Any(attr => attr.GetType().FullName == "System.Runtime.CompilerServices.NullableAttribute");

      static IEnumerable<string> GetGenericArgumentConstraintsOf(Type argument, ISet<string> _referencingNamespaces, bool typeWithNamespace)
      {
        var constraintAttrs = argument.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
        var constraintTypes = argument.GetGenericParameterConstraints();

        _referencingNamespaces?.UnionWith(constraintTypes.Where(ct => ct != typeof(ValueType)).SelectMany(CSharpFormatter.ToNamespaceList));

        if (
          constraintAttrs.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint) &&
          constraintAttrs.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint) &&
          constraintTypes.Any(ct => ct == typeof(ValueType))
        ) {
          constraintAttrs &= ~GenericParameterAttributes.NotNullableValueTypeConstraint;
          constraintAttrs &= ~GenericParameterAttributes.DefaultConstructorConstraint;
          constraintTypes = constraintTypes.Where(ct => ct != typeof(ValueType)).ToArray();

          if (IsUnmanagedTypeArgument(argument))
            yield return "unmanaged";
          else
            yield return "struct";
        }
        else if (constraintAttrs.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint)) {
          yield return "class";
        }
        else if (constraintAttrs.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint)) {
          if (IsUnmanagedTypeArgument(argument))
            yield return "unmanaged";
          else
            yield return "struct";
        }
        else if (constraintAttrs == GenericParameterAttributes.None && IsNotNullTypeArgument(argument)) {
          yield return "notnull";
        }

        foreach (var ctn in constraintTypes.Select(i => i.FormatTypeName(typeWithNamespace: typeWithNamespace)).OrderBy(name => name, StringComparer.Ordinal))
          yield return ctn;

        if (constraintAttrs.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
          yield return "new()";
      }

      var constraints = string.Join(
        ", ",
        GetGenericArgumentConstraintsOf(
          genericArgument,
          referencingNamespaces,
          genericArgument.DeclaringMethod == null ? options.TypeDeclarationWithNamespace : options.MemberDeclarationWithNamespace
        )
      );

      if (0 < constraints.Length)
        return $"where {genericArgument.FormatTypeName(typeWithNamespace: false)} : {constraints}";
      else
        return null;
    }

    public static IEnumerable<string> GenerateExplicitBaseTypeAndInterfaces(
      Type t,
      ISet<string> referencingNamespaces,
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
            IsInterface = type.IsInterface,
            Name = type.FormatTypeName(
              typeWithNamespace: options.TypeDeclarationWithNamespace,
              withDeclaringTypeName: options.TypeDeclarationWithDeclaringTypeName
            )
          };
        })
        .OrderBy(type => type.IsInterface)
        .ThenBy(type => type.Name, StringComparer.Ordinal)
        .Select(type => type.Name);
    }

    public static string GenerateMemberDeclaration(
      MemberInfo member,
      ISet<string> referencingNamespaces,
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

    private static string GenerateFieldDeclaration(
      FieldInfo field,
      ISet<string> referencingNamespaces,
      GeneratorOptions options
    )
    {
      if (options.IgnorePrivateOrAssembly && field.IsPrivateOrAssembly())
        return null;

      var sb = new StringBuilder();

      if (field.DeclaringType.IsEnum) {
        if (field.IsStatic) {
          var val = Convert.ChangeType(field.GetValue(null), field.DeclaringType.GetEnumUnderlyingType());

          sb.Append(GenerateMemberName(field, options)).Append(" = ");

          if (field.DeclaringType.IsEnumFlags())
            sb.Append("0x").AppendFormat("{0:x" + (Marshal.SizeOf(val) * 2).ToString() + "}", val);
          else
            sb.Append(val);

          if (!options.MemberDeclarationOmitEndOfStatement)
            sb.Append(",");
        }
        else {
          return null; // ignores backing field __value
        }
      }
      else {
        referencingNamespaces?.UnionWith(CSharpFormatter.ToNamespaceList(field.FieldType));

        sb
          .Append(GetMemberModifierOf(field, options))
          .Append(field.FieldType.FormatTypeName(attributeProvider: field, typeWithNamespace: options.MemberDeclarationWithNamespace))
          .Append(" ")
          .Append(GenerateMemberName(field, options));

        if (field.IsStatic && (field.IsLiteral || field.IsInitOnly) && !field.FieldType.ContainsGenericParameters) {
          var val = field.GetValue(null);
          var valueDeclaration = CSharpFormatter.FormatValueDeclaration(
            val,
            field.FieldType,
            typeWithNamespace: options.MemberDeclarationWithNamespace,
            findConstantField: (field.FieldType != field.DeclaringType),
            useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral
          );

          if (valueDeclaration == null) {
            sb
              .Append("; // = \"")
              .Append(CSharpFormatter.EscapeString((val ?? "null").ToString(), escapeDoubleQuote: true))
              .Append("\"");
          }
          else {
            sb.Append(" = ").Append(valueDeclaration);

            if (!options.MemberDeclarationOmitEndOfStatement)
              sb.Append(";");
          }
        }
        else {
          if (!options.MemberDeclarationOmitEndOfStatement)
            sb.Append(";");
        }
      }

      return sb.ToString();
    }

    private static string GeneratePropertyDeclaration(
      PropertyInfo property,
      ISet<string> referencingNamespaces,
      GeneratorOptions options
    )
    {
      var explicitInterface = property
        .GetAccessors(true)
        .Select(a => a.FindExplicitInterfaceMethod(findOnlyPublicInterfaces: options.IgnorePrivateOrAssembly)?.DeclaringType)
        .FirstOrDefault();

      if (
        explicitInterface == null &&
        options.IgnorePrivateOrAssembly &&
        property.GetAccessors(true).All(a => a.IsPrivateOrAssembly())
      )
        return null;

      var emitGetAccessor = property.GetMethod != null && !(explicitInterface == null && options.IgnorePrivateOrAssembly && property.GetMethod.IsPrivateOrAssembly());
      var emitSetAccessor = property.SetMethod != null && !(explicitInterface == null && options.IgnorePrivateOrAssembly && property.SetMethod.IsPrivateOrAssembly());

      var indexParameters = property.GetIndexParameters();

      referencingNamespaces?.UnionWith(CSharpFormatter.ToNamespaceList(property.PropertyType));
      referencingNamespaces?.UnionWith(indexParameters.SelectMany(ip => CSharpFormatter.ToNamespaceList(ip.ParameterType)));

      var sb = new StringBuilder();
      var modifier = GetMemberModifierOf(property, options, out string setAccessibility, out string getAccessibility);

      if (explicitInterface == null)
        sb.Append(modifier);

      sb.Append(property.PropertyType.FormatTypeName(attributeProvider: property, typeWithNamespace: options.MemberDeclarationWithNamespace)).Append(" ");

      var attrDefaultMember = property.DeclaringType.GetCustomAttribute<DefaultMemberAttribute>();

      if (0 < indexParameters.Length && string.Equals(property.Name, attrDefaultMember?.MemberName, StringComparison.Ordinal))
        // indexer
        sb.Append(
          GenerateMemberName(
            property,
            options.MemberDeclarationWithDeclaringTypeName ? attrDefaultMember.MemberName : "this",
            options
          )
        );
      else if (explicitInterface == null)
        sb.Append(
          GenerateMemberName(
            property,
            options
          )
        );
      else
        sb.Append(
          GenerateMemberName(
            property,
            explicitInterface.FormatTypeName(typeWithNamespace: options.MemberDeclarationWithNamespace) + "." + property.Name.Substring(property.Name.LastIndexOf('.') + 1),
            options
          )
        );

      if (0 < indexParameters.Length)
        sb
          .Append("[")
          .Append(
            CSharpFormatter.FormatParameterList(
              indexParameters,
              typeWithNamespace: options.MemberDeclarationWithNamespace,
              useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral
            )
          )
          .Append("] ");
      else
        sb.Append(" ");

      sb.Append("{ ");

      if (emitGetAccessor) {
        if (explicitInterface == null && 0 < getAccessibility.Length)
          sb.Append(getAccessibility).Append(" ");

        sb.Append("get").Append(GenerateAccessorBody(property.GetMethod, options));
      }

      if (emitSetAccessor) {
        if (explicitInterface == null && 0 < setAccessibility.Length)
          sb.Append(setAccessibility).Append(" ");

        if (property.IsSetMethodInitOnly())
          sb.Append("init");
        else
          sb.Append("set");

        sb.Append(GenerateAccessorBody(property.SetMethod, options));
      }

      sb.Append("}");

#if false
        if (p.CanRead)
          sb.Append(" = ").Append(p.GetConstantValue()).Append(";");
#endif

      return sb.ToString();

      static string GenerateAccessorBody(MethodInfo accessor, GeneratorOptions opts)
      {
        switch (accessor.IsAbstract ? MethodBodyOption.EmptyImplementation : opts.MemberDeclarationMethodBody) {
          case MethodBodyOption.ThrowNotImplementedException: return " => throw new NotImplementedException(); ";

          //case MethodBodyOption.None:
          //case MethodBodyOption.EmptyImplementation:
          default: return "; ";
        }
      }
    }

    private static string GenerateMethodBaseDeclaration(
      MethodBase m,
      ISet<string> referencingNamespaces,
      GeneratorOptions options
    )
    {
      var explicitInterfaceMethod = m.FindExplicitInterfaceMethod(findOnlyPublicInterfaces: options.IgnorePrivateOrAssembly);

      if (explicitInterfaceMethod == null && (options.IgnorePrivateOrAssembly && m.IsPrivateOrAssembly()))
        return null;

      var method = m as MethodInfo;
      var methodModifiers = GetMemberModifierOf(m, options);
      var isByRefReturnType = (method != null && method.ReturnType.IsByRef);
      var methodReturnType = isByRefReturnType ? "ref " + method.ReturnType.GetElementType().FormatTypeName(attributeProvider: method.ReturnTypeCustomAttributes, typeWithNamespace: options.MemberDeclarationWithNamespace) : method?.ReturnType?.FormatTypeName(attributeProvider: method?.ReturnTypeCustomAttributes, typeWithNamespace: options.MemberDeclarationWithNamespace);
      var methodGenericParameters = m.IsGenericMethod ? string.Concat("<", string.Join(", ", m.GetGenericArguments().Select(t => t.FormatTypeName(typeWithNamespace: options.MemberDeclarationWithNamespace))), ">") : null;
      var methodParameterList = CSharpFormatter.FormatParameterList(m, typeWithNamespace: options.MemberDeclarationWithNamespace, useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral);
      var methodConstraints = method == null ? null : string.Join(" ", method.GetGenericArguments().Select(arg => Generator.GenerateGenericArgumentConstraintDeclaration(arg, referencingNamespaces, options)).Where(d => d != null));
      string methodName = null;
      string methodBody = null;

      var endOfStatement = options.MemberDeclarationOmitEndOfStatement
        ? string.Empty
        : ";";

      switch (options.MemberDeclarationMethodBody) {
        case MethodBodyOption.None: methodBody = null; break;
        case MethodBodyOption.EmptyImplementation: methodBody = m.IsAbstract ? endOfStatement : " {}"; break;
        case MethodBodyOption.ThrowNotImplementedException: methodBody = m.IsAbstract ? endOfStatement : " => throw new NotImplementedException()" + endOfStatement; break;
      }

      referencingNamespaces?.UnionWith(m.GetSignatureTypes().Where(mpt => !mpt.ContainsGenericParameters).SelectMany(CSharpFormatter.ToNamespaceList));

      if (m.IsSpecialName) {
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
      else if (method.IsFamily && string.Equals(method.Name, "Finalize", StringComparison.Ordinal)) {
        // destructors
        methodName = GenerateMemberName(
          m,
          "~" + (m.DeclaringType.IsGenericType ? m.DeclaringType.GetGenericTypeName() : m.DeclaringType.Name),
          options
        );
        methodModifiers = null;
        methodReturnType = null;
        methodParameterList = null;
        methodConstraints = null;
      }
      else if (explicitInterfaceMethod != null) {
        methodModifiers = null;
        methodName = GenerateMemberName(
          m,
          explicitInterfaceMethod.DeclaringType.FormatTypeName(typeWithNamespace: options.MemberDeclarationWithNamespace) + "." + explicitInterfaceMethod.Name,
          options
        );
      }
      else {
        // standard methods
        methodName = GenerateMemberName(m, options);
      }

      var sb = new StringBuilder();

      sb.Append(methodModifiers);

      if (!string.IsNullOrEmpty(methodReturnType))
        sb.Append(methodReturnType).Append(" ");

      sb.Append(methodName);

      if (!string.IsNullOrEmpty(methodGenericParameters))
        sb.Append(methodGenericParameters);

      sb.Append("(");

      if (!string.IsNullOrEmpty(methodParameterList))
        sb.Append(methodParameterList);

      sb.Append(")");

      if (!string.IsNullOrEmpty(methodConstraints))
        sb.Append(" ").Append(methodConstraints);

      sb.Append(methodBody);

      return sb.ToString();
    }

    private static string GenerateEventDeclaration(
      EventInfo ev,
      ISet<string> referencingNamespaces,
      GeneratorOptions options
    )
    {
      var explicitInterface = ev.GetMethods(true).Select(evm => evm.FindExplicitInterfaceMethod(findOnlyPublicInterfaces: options.IgnorePrivateOrAssembly)?.DeclaringType).FirstOrDefault();

      if (explicitInterface == null && options.IgnorePrivateOrAssembly && ev.GetMethods(true).All(m => m.IsPrivateOrAssembly()))
        return null;

      referencingNamespaces?.UnionWith(CSharpFormatter.ToNamespaceList(ev.EventHandlerType));

      var sb = new StringBuilder();

      if (explicitInterface == null)
        sb.Append(GetMemberModifierOf(ev.GetMethods(true).First(), options));

      sb.Append("event ").Append(ev.EventHandlerType.FormatTypeName(attributeProvider: ev, typeWithNamespace: options.MemberDeclarationWithNamespace)).Append(" ");

      if (explicitInterface == null)
        sb.Append(
          GenerateMemberName(
            ev,
            options
          )
        );
      else
        sb.Append(
          GenerateMemberName(
            ev,
            explicitInterface.FormatTypeName(typeWithNamespace: options.MemberDeclarationWithNamespace) + "." + ev.Name.Substring(ev.Name.LastIndexOf('.') + 1),
            options
          )
        );

      if (!options.MemberDeclarationOmitEndOfStatement)
        sb.Append(";");

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
      if (options.MemberDeclarationWithDeclaringTypeName) {
        return member.DeclaringType.FormatTypeName(
          typeWithNamespace: options.MemberDeclarationWithNamespace,
          translateLanguagePrimitiveType: options.TranslateLanguagePrimitiveTypeDeclaration
        ) + "." + memberName;
      }

      return memberName;
    }

    private static string GetMemberModifierOf(MemberInfo member, GeneratorOptions options)
      => GetMemberModifierOf(member, options, out _, out _);

    // TODO: async, extern, volatile
    private static string GetMemberModifierOf(
      MemberInfo member,
      GeneratorOptions options,
      out string setMethodAccessibility,
      out string getMethodAccessibility
    )
    {
      setMethodAccessibility = string.Empty;
      getMethodAccessibility = string.Empty;

      if (member.DeclaringType.IsInterface)
        return string.Empty;

      var modifiers = new List<string>();
      string accessibility = null;

      modifiers.Add(null); // placeholder for accessibility

      IEnumerable<string> GetModifiersOfMethod(MethodBase m)
      {
        if (m == null)
          yield break;

        var mm = m as MethodInfo;

        if (m.IsStatic)
          yield return "static";

        if (m.IsAbstract) {
          yield return "abstract";
        }
        else if (mm != null && mm.GetBaseDefinition() != mm) {
          if (m.IsFinal)
            yield return "sealed";

          yield return "override";
        }
        else if (m.IsVirtual && !m.IsFinal) {
          yield return "virtual";
        }

        if (mm != null && mm.GetParameters().Any(p => p.ParameterType.IsPointer))
          yield return "unsafe";

        // cannot detect 'new' modifier
        //  yield return "new";
      }

      switch (member) {
        case FieldInfo f:
          accessibility = options.MemberDeclarationWithAccessibility
            ? CSharpFormatter.FormatAccessibility(f.GetAccessibility())
            : null;

          if (f.IsStatic && !f.IsLiteral) modifiers.Add("static");
          if (f.IsInitOnly) modifiers.Add("readonly");
          if (f.IsLiteral) modifiers.Add("const");

          break;

        case PropertyInfo p:
          var mostOpenAccessibility = p.GetAccessors(true).Select(Smdn.Reflection.MemberInfoExtensions.GetAccessibility).Max();

          accessibility = options.MemberDeclarationWithAccessibility
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
          accessibility = options.MemberDeclarationWithAccessibility
            ? CSharpFormatter.FormatAccessibility(m.GetAccessibility())
            : null;

          modifiers.AddRange(GetModifiersOfMethod(m));

          break;
      }

      if (member == member.DeclaringType.TypeInitializer)
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
}
