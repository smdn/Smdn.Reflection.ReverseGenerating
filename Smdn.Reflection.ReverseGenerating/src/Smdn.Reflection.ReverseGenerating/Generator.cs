using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Smdn.Reflection.ReverseGenerating {
  public static class Generator {
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
      var accessibilities = CSharpFormatter.FormatAccessibility(t.GetAccessibility());
      var typeName = t.FormatTypeName(typeWithNamespace: false, withDeclaringTypeName: false);

      var genericArgumentConstraints = t
        .GetGenericArguments()
        .Select(arg => GenerateGenericArgumentConstraintDeclaration(arg, referencingNamespaces, options))
        .Where(d => d != null)
        .ToList();

      string GetSingleLineGenericArgumentConstraintsDeclaration()
        => genericArgumentConstraints.Count == 0 ? string.Empty : " " + string.Join(" ", genericArgumentConstraints);

      if (t.IsEnum) {
        yield return $"{accessibilities} enum {typeName} : {t.GetEnumUnderlyingType().FormatTypeName()}";
        yield break;
      }

      if (t.IsDelegate()) {
        var signatureInfo = t.GetDelegateSignatureMethod();

        referencingNamespaces?.AddRange(signatureInfo.GetSignatureTypes().Where(mpt => !mpt.ContainsGenericParameters).SelectMany(CSharpFormatter.ToNamespaceList));

        var genericArgumentConstraintDeclaration = genericArgumentConstraints.Count == 0 ? string.Empty : " " + string.Join(" ", genericArgumentConstraints);

        yield return $"{accessibilities} delegate {signatureInfo.ReturnType.FormatTypeName(attributeProvider: signatureInfo.ReturnTypeCustomAttributes, typeWithNamespace: options.TypeDeclarationWithNamespace)} {typeName}({CSharpFormatter.FormatParameterList(signatureInfo, typeWithNamespace: options.TypeDeclarationWithNamespace, useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral)}){genericArgumentConstraintDeclaration};";
        yield break;
      }

      string typeDeclaration = null;

      if (t.IsInterface) {
        typeDeclaration = $"{accessibilities} interface {typeName}";
      }
      else if (t.IsValueType) {
        var isReadOnly = t.IsReadOnlyValueType() ? " readonly" : string.Empty;
        var isByRefLike = t.IsByRefLikeValueType() ? " ref" : string.Empty;

        typeDeclaration = $"{accessibilities}{isReadOnly}{isByRefLike} struct {typeName}";
      }
      else {
        string modifier = null;

        if (t.IsAbstract && t.IsSealed)
          modifier = " static";
        else if (t.IsAbstract)
          modifier = " abstract";
        else if (t.IsSealed)
          modifier = " sealed";

        typeDeclaration = $"{accessibilities}{modifier} class {typeName}";
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
      static IEnumerable<string> GetGenericArgumentConstraintsOf(Type argument, ISet<string> _referencingNamespaces, bool typeWithNamespace)
      {
        var constraintAttrs = argument.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
        var constraintTypes = argument.GetGenericParameterConstraints();

        _referencingNamespaces?.AddRange(constraintTypes.Where(ct => ct != typeof(ValueType)).SelectMany(CSharpFormatter.ToNamespaceList));

        if (
          constraintAttrs.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint) &&
          constraintAttrs.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint) &&
          constraintTypes.Any(ct => ct == typeof(ValueType))
        ) {
          constraintAttrs &= ~GenericParameterAttributes.NotNullableValueTypeConstraint;
          constraintAttrs &= ~GenericParameterAttributes.DefaultConstructorConstraint;
          constraintTypes = constraintTypes.Where(ct => ct != typeof(ValueType)).ToArray();

          yield return "struct";
        }

        if (constraintAttrs.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
          yield return "class";
        if (constraintAttrs.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
          yield return "struct"; // XXX

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

    public static IEnumerable<string> GenerateExplicitBaseTypeAndInterfaces(Type t, ISet<string> referencingNamespaces, GeneratorOptions options)
    {
      if (options == null)
        throw new ArgumentNullException(nameof(options));

      return t
        .GetExplicitBaseTypeAndInterfaces()
        .Where(type => !(options.IgnorePrivateOrAssembly && (type.IsNotPublic || type.IsNestedAssembly || type.IsNestedFamily || type.IsNestedFamANDAssem || type.IsNestedPrivate)))
        .Select(type => {
          referencingNamespaces?.AddRange(CSharpFormatter.ToNamespaceList(type));
          return new { IsInterface = type.IsInterface, Name = type.FormatTypeName(typeWithNamespace: options.TypeDeclarationWithNamespace) };
        })
        .OrderBy(type => type.IsInterface)
        .ThenBy(type => type.Name, StringComparer.Ordinal)
        .Select(type => type.Name);
    }

    internal struct DefaultLayoutStruct {
      // XXX
      public static readonly StructLayoutAttribute Attribute = typeof(DefaultLayoutStruct).StructLayoutAttribute;
    }

    public static IEnumerable<string> GenerateAttributeList(ICustomAttributeProvider attributeProvider, ISet<string> referencingNamespaces, GeneratorOptions options)
    {
      return GetAttributes()
        .OrderBy(attr => attr.GetType().FullName)
        .Select(attr => new { Name = ConvertAttributeName(attr), Params = ConvertAttributeParameters(attr) })
        .Select(a => "[" + a.Name + (string.IsNullOrEmpty(a.Params) ? string.Empty : "(" + a.Params + ")") + "]");

      IEnumerable<Attribute> GetAttributes()
      {
        foreach (var attr in attributeProvider.GetCustomAttributes(typeof(Attribute), false)) {
          if (attr is System.CLSCompliantAttribute)
            continue; // ignore
          if (attr is System.Reflection.DefaultMemberAttribute)
            continue; // ignore

          var nsAttr = attr.GetType().Namespace;

          if (string.Equals("System.Runtime.CompilerServices", nsAttr, StringComparison.Ordinal))
            continue; // ignore

          if (string.Equals("System", nsAttr.Split('.')[0], StringComparison.Ordinal))
            yield return attr as Attribute;
        }

        if (attributeProvider is Type t && t.IsValueType && !t.IsEnum) {
          if (
            t.StructLayoutAttribute.Value != DefaultLayoutStruct.Attribute.Value ||
            t.StructLayoutAttribute.Pack != DefaultLayoutStruct.Attribute.Pack ||
            t.StructLayoutAttribute.CharSet != DefaultLayoutStruct.Attribute.CharSet
          ) {
            yield return t.StructLayoutAttribute;
          }
        }
      }

      string ConvertAttributeName(Attribute attr)
      {
        var typeOfAttr = attr.GetType();

        referencingNamespaces?.Add(typeOfAttr.Namespace);

        var nameOfAttr = typeOfAttr.FormatTypeName(typeWithNamespace: options.TypeDeclarationWithNamespace);

        if (nameOfAttr.EndsWith("Attribute", StringComparison.Ordinal))
          nameOfAttr = nameOfAttr.Substring(0, nameOfAttr.Length - 9);

        return nameOfAttr;
      }

      string ConvertAttributeParameters(Attribute attr)
      {
        switch (attr) {
          case System.AttributeUsageAttribute aua:
            var allowMultiple = aua.AllowMultiple ? ", AllowMultiple = " + ConvertValue(aua.AllowMultiple) : null;
            var inherited =  aua.Inherited ? ", Inherited = " + ConvertValue(aua.Inherited) : null;

            return ConvertValue(aua.ValidOn) + allowMultiple + inherited;

          case System.FlagsAttribute fa:
            return null;

          case System.ObsoleteAttribute oa:
            var isError = oa.IsError ? ", " + ConvertValue(oa.IsError) : null;

            if (oa.Message != null || isError != null)
              return ConvertValue(oa.Message) + isError;
            else
              return null;

          case System.Diagnostics.DebuggerHiddenAttribute dha:
            return null;

          case System.SerializableAttribute sa:
            return null;

          case System.Diagnostics.ConditionalAttribute ca:
            if (string.IsNullOrEmpty(ca.ConditionString))
              return null;
            else
              return ConvertValue(ca.ConditionString);

          case System.Runtime.InteropServices.FieldOffsetAttribute foa:
            return ConvertValue(foa.Value);

          case System.Runtime.InteropServices.StructLayoutAttribute sla:
            var pack = sla.Pack == DefaultLayoutStruct.Attribute.Pack ? null : ", Pack = " + ConvertValue(sla.Pack);
            var size = sla.Size == 0 ? null : ", Size = " + ConvertValue(sla.Size);
            var charset = sla.CharSet == DefaultLayoutStruct.Attribute.CharSet ? null : ", CharSet = " + ConvertValue(sla.CharSet);

            return ConvertValue(sla.Value) + pack + size + charset;
        }

        throw new NotSupportedException($"unsupported attribute type: {attr.GetType().FullName}");
      }

      string ConvertValue(object @value)
      {
        return CSharpFormatter.FormatValueDeclaration(
          @value,
          @value?.GetType(),
          typeWithNamespace: options.MemberDeclarationWithNamespace,
          findConstantField: true,
          useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral
        );
      }
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
      if (options.IgnorePrivateOrAssembly && (field.IsPrivate || field.IsAssembly || field.IsFamilyAndAssembly))
        return null;

      var sb = new StringBuilder();

      if (field.DeclaringType.IsEnum) {
        if (field.IsStatic) {
          var val = Convert.ChangeType(field.GetValue(null), field.DeclaringType.GetEnumUnderlyingType());

          sb.Append(field.Name).Append(" = ");

          if (field.DeclaringType.IsEnumFlags())
            sb.Append("0x").AppendFormat("{0:x" + (Marshal.SizeOf(val) * 2).ToString() + "}", val);
          else
            sb.Append(val);

          sb.Append(",");
        }
        else {
          return null; // ignores backing field __value
        }
      }
      else {
        referencingNamespaces?.AddRange(CSharpFormatter.ToNamespaceList(field.FieldType));

        sb
          .Append(GetMemberModifierOf(field))
          .Append(" ")
          .Append(field.FieldType.FormatTypeName(attributeProvider: field, typeWithNamespace: options.MemberDeclarationWithNamespace))
          .Append(" ")
          .Append(field.Name);

        if (field.IsStatic && (field.IsLiteral || field.IsInitOnly) && !field.FieldType.ContainsGenericParameters) {
          var val = field.GetValue(null);
          var valueDeclaration = CSharpFormatter.FormatValueDeclaration(
            val,
            field.FieldType,
            typeWithNamespace: options.MemberDeclarationWithNamespace,
            findConstantField: (field.FieldType != field.DeclaringType),
            useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral
          );

          if (valueDeclaration == null)
            sb
              .Append("; // = \"")
              .Append(CSharpFormatter.EscapeString((val ?? "null").ToString(), escapeDoubleQuote: true))
              .Append("\"");
          else
            sb
              .Append(" = ")
              .Append(valueDeclaration)
              .Append(";");
        }
        else {
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
        property.GetAccessors(true).All(a => a.IsPrivate || a.IsAssembly || a.IsFamilyAndAssembly)
      )
        return null;

      var emitGetAccessor = property.GetMethod != null && !(explicitInterface == null && options.IgnorePrivateOrAssembly && (property.GetMethod.IsPrivate || property.GetMethod.IsAssembly || property.GetMethod.IsFamilyAndAssembly));
      var emitSetAccessor = property.SetMethod != null && !(explicitInterface == null && options.IgnorePrivateOrAssembly && (property.SetMethod.IsPrivate || property.SetMethod.IsAssembly || property.SetMethod.IsFamilyAndAssembly));

      var indexParameters = property.GetIndexParameters();

      referencingNamespaces?.AddRange(CSharpFormatter.ToNamespaceList(property.PropertyType));
      referencingNamespaces?.AddRange(indexParameters.SelectMany(ip => CSharpFormatter.ToNamespaceList(ip.ParameterType)));

      var sb = new StringBuilder();
      var modifier = GetMemberModifierOf(property, out string setAccessibility, out string getAccessibility);

      if (explicitInterface == null && 0 < modifier.Length)
        sb.Append(modifier).Append(" ");

      sb.Append(property.PropertyType.FormatTypeName(attributeProvider: property, typeWithNamespace: options.MemberDeclarationWithNamespace)).Append(" ");

      var propertyName = explicitInterface == null ? property.Name : property.Name.Substring(property.Name.LastIndexOf('.') + 1);

      if (0 < indexParameters.Length &&
          string.Equals(property.Name, property.DeclaringType.GetCustomAttribute<DefaultMemberAttribute>()?.MemberName, StringComparison.Ordinal))
        sb.Append("this"); // indexer
      else if (explicitInterface == null)
        sb.Append(propertyName);
      else
        sb
          .Append(
            explicitInterface.FormatTypeName(
              attributeProvider: property,
              typeWithNamespace: options.MemberDeclarationWithNamespace
            )
          )
          .Append(".")
          .Append(propertyName);

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

        sb.Append("get");

        if (options.GenerateEmptyImplementation && !property.GetMethod.IsAbstract)
          sb.Append(" => throw new NotImplementedException(); ");
        else
          sb.Append("; ");
      }

      if (emitSetAccessor) {
        if (explicitInterface == null && 0 < setAccessibility.Length)
          sb.Append(setAccessibility).Append(" ");

        sb.Append("set");

        if (options.GenerateEmptyImplementation && !property.SetMethod.IsAbstract)
          sb.Append(" => throw new NotImplementedException(); ");
        else
          sb.Append("; ");
      }

      sb.Append("}");

#if false
        if (p.CanRead)
          sb.Append(" = ").Append(p.GetConstantValue()).Append(";");
#endif

      return sb.ToString();
    }

    private static string GenerateMethodBaseDeclaration(
      MethodBase m,
      ISet<string> referencingNamespaces,
      GeneratorOptions options
    )
    {
      var explicitInterfaceMethod = m.FindExplicitInterfaceMethod(findOnlyPublicInterfaces: options.IgnorePrivateOrAssembly);

      if (explicitInterfaceMethod == null && (options.IgnorePrivateOrAssembly && (m.IsPrivate || m.IsAssembly || m.IsFamilyAndAssembly)))
        return null;

      var method = m as MethodInfo;
      var methodModifiers = GetMemberModifierOf(m);
      var isByRefReturnType = (method != null && method.ReturnType.IsByRef);
      var methodReturnType = isByRefReturnType ? "ref " + method.ReturnType.GetElementType().FormatTypeName(attributeProvider: method.ReturnTypeCustomAttributes, typeWithNamespace: options.MemberDeclarationWithNamespace) : method?.ReturnType?.FormatTypeName(attributeProvider: method?.ReturnTypeCustomAttributes, typeWithNamespace: options.MemberDeclarationWithNamespace);
      var methodName = m.Name;
      var methodGenericParameters = m.IsGenericMethod ? string.Concat("<", string.Join(", ", m.GetGenericArguments().Select(t => t.FormatTypeName(typeWithNamespace: options.MemberDeclarationWithNamespace))), ">") : null;
      var methodParameterList = CSharpFormatter.FormatParameterList(m, typeWithNamespace: options.MemberDeclarationWithNamespace, useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral);
      var methodConstraints = method == null ? null : string.Join(" ", method.GetGenericArguments().Select(arg => Generator.GenerateGenericArgumentConstraintDeclaration(arg, referencingNamespaces, options)).Where(d => d != null));
      var methodBody = m.IsAbstract ? ";" : options.GenerateEmptyImplementation ? " => throw new NotImplementedException();" : " {}";

      referencingNamespaces?.AddRange(m.GetSignatureTypes().Where(mpt => !mpt.ContainsGenericParameters).SelectMany(CSharpFormatter.ToNamespaceList));

      if (method == null) {
        // constructors
        methodName = m.DeclaringType.IsGenericType ? m.DeclaringType.GetGenericTypeName() : m.DeclaringType.Name;
        methodReturnType = null;
      }
      else if (method.IsFamily && string.Equals(method.Name, "Finalize", StringComparison.Ordinal)) {
        // destructors
        methodName = "~" + (m.DeclaringType.IsGenericType ? m.DeclaringType.GetGenericTypeName() : m.DeclaringType.Name);
        methodModifiers = null;
        methodReturnType = null;
        methodParameterList = null;
        methodConstraints = null;
      }
      else if (method.IsSpecialName) {
        // operator overloads, etc
        switch (method.Name) {
          // comparison
          case "op_Equality": methodName = "operator == "; break;
          case "op_Inequality": methodName = "operator != "; break;
          case "op_LessThan": methodName = "operator < "; break;
          case "op_GreaterThan": methodName = "operator > "; break;
          case "op_LessThanOrEqual": methodName = "operator <= "; break;
          case "op_GreaterThanOrEqual": methodName = "operator >= "; break;

          // unary
          case "op_UnaryPlus": methodName = "operator + "; break;
          case "op_UnaryNegation": methodName = "operator - "; break;
          case "op_LogicalNot": methodName = "operator ! "; break;
          case "op_OnesComplement": methodName = "operator ~ "; break;
          case "op_True": methodName = "operator true "; break;
          case "op_False": methodName = "operator false "; break;
          case "op_Increment": methodName = "operator ++ "; break;
          case "op_Decrement": methodName = "operator -- "; break;

          // binary
          case "op_Addition": methodName = "operator + "; break;
          case "op_Subtraction": methodName = "operator - "; break;
          case "op_Multiply": methodName = "operator * "; break;
          case "op_Division": methodName = "operator / "; break;
          case "op_Modulus": methodName = "operator % "; break;
          case "op_BitwiseAnd": methodName = "operator & "; break;
          case "op_BitwiseOr": methodName = "operator | "; break;
          case "op_ExclusiveOr": methodName = "operator ^ "; break;
          case "op_RightShift": methodName = "operator >> "; break;
          case "op_LeftShift": methodName = "operator << "; break;

          // type cast
          case "op_Explicit": methodName = "explicit operator " + methodReturnType; methodReturnType = null; break;
          case "op_Implicit": methodName = "implicit operator " + methodReturnType; methodReturnType = null; break;
        }
      }
      else if (explicitInterfaceMethod != null) {
        methodModifiers = null;
        methodName = explicitInterfaceMethod.DeclaringType.FormatTypeName(typeWithNamespace: options.MemberDeclarationWithNamespace) + "." + explicitInterfaceMethod.Name;
      }
      else {
        // standard methods
      }

      var sb = new StringBuilder();

      if (!string.IsNullOrEmpty(methodModifiers))
        sb.Append(methodModifiers).Append(" ");

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

      if (explicitInterface == null && options.IgnorePrivateOrAssembly && ev.GetMethods(true).All(m => m.IsPrivate || m.IsAssembly || m.IsFamilyAndAssembly))
        return null;

      referencingNamespaces?.AddRange(CSharpFormatter.ToNamespaceList(ev.EventHandlerType));

      var sb = new StringBuilder();
      var modifier = GetMemberModifierOf(ev.GetMethods(true).First());

      if (explicitInterface == null && 0 < modifier.Length)
        sb.Append(modifier).Append(" ");

      sb.Append("event ").Append(ev.EventHandlerType.FormatTypeName(attributeProvider: ev, typeWithNamespace: options.MemberDeclarationWithNamespace)).Append(" ");

      var eventName = explicitInterface == null ? ev.Name : ev.Name.Substring(explicitInterface.FullName.Length + 1);

      if (explicitInterface == null)
        sb.Append(eventName);
      else
        sb.Append(explicitInterface.FormatTypeName(attributeProvider: ev, typeWithNamespace: options.MemberDeclarationWithNamespace)).Append(".").Append(eventName);

      sb.Append(";");

      return sb.ToString();
    }

    private static string GetMemberModifierOf(MemberInfo member)
      => GetMemberModifierOf(member, out _, out _);

    // TODO: async, extern, volatile
    private static string GetMemberModifierOf(MemberInfo member, out string setMethodAccessibility, out string getMethodAccessibility)
    {
      setMethodAccessibility = string.Empty;
      getMethodAccessibility = string.Empty;

      if (member.DeclaringType.IsInterface)
        return string.Empty;

      var modifiers = new List<string>();
      string accessibility = null;

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
          accessibility = CSharpFormatter.FormatAccessibility(f.GetAccessibility());

          if (f.IsStatic && !f.IsLiteral) modifiers.Add("static");
          if (f.IsInitOnly) modifiers.Add("readonly");
          if (f.IsLiteral) modifiers.Add("const");

          break;

        case PropertyInfo p:
          var mostOpenAccessibility = p.GetAccessors(true).Select(Smdn.Reflection.MemberInfoExtensions.GetAccessibility).Max();

          accessibility = CSharpFormatter.FormatAccessibility(mostOpenAccessibility);

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
          accessibility = CSharpFormatter.FormatAccessibility(m.GetAccessibility());

          modifiers.AddRange(GetModifiersOfMethod(m));

          break;
      }

      var joinedModifiers = string.Join(" ", modifiers);

      if (member == member.DeclaringType.TypeInitializer || string.IsNullOrEmpty(accessibility))
        return string.Join(" ", modifiers);
      else if (string.IsNullOrEmpty(joinedModifiers))
        return accessibility;
      else
        return accessibility + " " + joinedModifiers;
    }
  }
}
