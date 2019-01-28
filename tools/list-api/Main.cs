using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

using Smdn.Reflection;

class Options {
  public string Indent = new string(' ', 2);

  public bool IgnorePrivateOrAssembly = true;
  public bool GenerateEmptyImplementation = false;

  public bool TypeDeclarationWithNamespace = false;

  public bool MemberDeclarationEmitNewLine = true;
  public bool MemberDeclarationWithNamespace = false;
  public bool MemberDeclarationUseDefaultLiteral = true;

  public Options Clone()
  {
    return (Options)MemberwiseClone();
  }

  public static Options Default { get; } = new Options();
}

class ListApi {
  static void Main(string[] args)
  {
    var libs = new List<string>();
    var options = new Options();
    var testMode = false;
    var showUsage = false;
    var stdout = false;

    foreach (var arg in args) {
      switch (arg) {
        case "--generate-impl":
          options.GenerateEmptyImplementation = true;
          break;

        case "--generate-fullname":
          options.TypeDeclarationWithNamespace = true;
          options.MemberDeclarationWithNamespace = true;
          break;

        case "--test":
          testMode = true;
          break;

        case "--stdout":
          stdout = true;
          break;

        case "/?":
        case "-h":
        case "--help":
          showUsage = true;
          break;

        default: {
          libs.Add(arg);
          break;
        }
      }
    }

    if (showUsage) {
      Console.Error.WriteLine("--test: run tests");
      Console.Error.WriteLine("--stdout: output to stdout");
      Console.Error.WriteLine("--generate-fullname: generate type and member declaration with full type name");
      Console.Error.WriteLine("--generate-impl: generate with empty implementation");
      Console.Error.WriteLine();
      return;
    }

    if (testMode) {
      Test.RunTests();
      return;
    }

    foreach (var lib in libs) {
      Assembly assm = null;

      try {
        Console.Error.WriteLine($"loading {lib}");

        //assm = Assembly.ReflectionOnlyLoadFrom(lib);
        assm = Assembly.LoadFrom(lib);

        Console.Error.WriteLine($"loaded '{assm.FullName}'");
      }
      catch (Exception ex) {
        Console.Error.WriteLine($"error: cannot load: {lib}");
        Console.Error.WriteLine(ex);
      }

      if (assm == null)
        continue;

      var frameworkName = assm.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
      var assemblyIdentifier = (frameworkName == null)
        ? assm.GetCustomAttribute<AssemblyProductAttribute>()?.Product
        : $"{assm.GetName().Name}-v{assm.GetName().Version}-{frameworkName}";

      var defaultOutputFileName = $"{assemblyIdentifier}.apilist.cs";

      using (Stream outputStream = stdout ? Console.OpenStandardOutput() : File.Open(defaultOutputFileName, FileMode.Create)) {
        using (var writer = new StreamWriter(outputStream, stdout ? Console.OutputEncoding : new UTF8Encoding(false))) {
          DisplayAssemblyInfo(writer, assm);

          DisplayExportedTypes(writer, assm, options);
        }
      }

      Console.Error.WriteLine("done");
    }
  }

  static void DisplayAssemblyInfo(TextWriter writer, Assembly assm)
  {
    writer.WriteLine($"// {assm.GetCustomAttribute<AssemblyProductAttribute>()?.Product}");
    writer.WriteLine($"//   Name: {assm.GetName().Name}");
    writer.WriteLine($"//   TargetFramework: {assm.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName}");
    writer.WriteLine($"//   AssemblyVersion: {assm.GetName().Version}");
    writer.WriteLine($"//   InformationalVersion: {assm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
    writer.WriteLine();
  }

  static void DisplayExportedTypes(TextWriter writer, Assembly assm, Options options)
  {
    if (options == null)
      throw new ArgumentNullException(nameof(options));

    var types = assm.GetExportedTypes();
    var typeDeclarations = new StringBuilder(10240);
    var referencingNamespaces = new HashSet<string>(StringComparer.Ordinal);

    foreach (var ns in types.Select(t => t.Namespace).Distinct().OrderBy(n => n, StringComparer.Ordinal)) {
      if (0 < typeDeclarations.Length)
        typeDeclarations.AppendLine();

      typeDeclarations.AppendLine($"namespace {ns} {{");

      typeDeclarations.Append(GenerateTypeAndMemberDeclarations(1,
                                                                types.Where(type => type.Namespace.Equals(ns, StringComparison.Ordinal))
                                                                     .Where(type => !type.IsNested),
                                                                referencingNamespaces,
                                                                options));

      typeDeclarations.AppendLine("}");
    }

    foreach (var ns in referencingNamespaces.OrderBy(OrderOfRootNamespace).ThenBy(ns => ns, StringComparer.Ordinal)) {
      writer.WriteLine($"using {ns};");
    }

    writer.WriteLine();
    writer.WriteLine(typeDeclarations);

    int OrderOfRootNamespace(string ns)
    {
      return ns.Split('.')[0].StartsWith("System", StringComparison.Ordinal) ? 0 : 1;
    }
  }

  static string GenerateTypeAndMemberDeclarations(int nestLevel, IEnumerable<Type> types, ISet<string> referencingNamespaces, Options options)
  {
    var ret = new StringBuilder(10240);
    var isPrevDelegate = false;

    foreach (var type in types.OrderBy(OrderOfType).ThenBy(type => type.FullName, StringComparer.Ordinal)) {
      var isDelegate = type.IsDelegate();

      if (0 < ret.Length && !(isPrevDelegate && isDelegate))
        ret.AppendLine();

      ret.Append(GenerateTypeAndMemberDeclarations(nestLevel, type, referencingNamespaces, options));

      isPrevDelegate = isDelegate;
    }

    return ret.ToString();

    int OrderOfType(Type t)
    {
      if (t.IsDelegate())   return 1;
      if (t.IsInterface)    return 2;
      if (t.IsEnum)         return 3;
      if (t.IsClass)        return 4;
      if (t.IsValueType)    return 5;

      return int.MaxValue;
    }
  }

  // TODO: unsafe types
  internal static string GenerateTypeAndMemberDeclarations(int nestLevel, Type t, ISet<string> referencingNamespaces, Options options)
  {
    if (options == null)
      throw new ArgumentNullException(nameof(options));

    var ret = new StringBuilder(1024);
    var indent = string.Concat(Enumerable.Repeat(options.Indent, nestLevel));

    // TODO: AttributeTargets.GenericParameter
    foreach (var attr in GenerateAttributeList(t, null, options)) {
      ret.Append(indent)
         .AppendLine(attr);
    }

    var typeDeclarationLines = GenerateTypeDeclaration(t, referencingNamespaces, options).ToList();

    for (var index = 0; index < typeDeclarationLines.Count; index++) {
      if (0 < index)
        ret.AppendLine();

      ret.Append(indent).Append(typeDeclarationLines[index]);
    }

    var hasContent = !t.IsDelegate();

    if (hasContent) {
      if (1 < typeDeclarationLines.Count) // multiline declaration
        ret.AppendLine().Append(indent).AppendLine("{");
      else
        ret.AppendLine(" {");

      ret.Append(GenerateTypeContentDeclarations(nestLevel + 1, t, referencingNamespaces, options));

      ret.Append(indent).AppendLine("}");
    }
    else {
      ret.AppendLine();
    }

    return ret.ToString();
  }

  internal static IEnumerable<string> GetExplicitBaseTypeAndInterfacesAsString(Type t, ISet<string> referencingNamespaces, Options options)
  {
    if (options == null)
      throw new ArgumentNullException(nameof(options));

    return t.GetExplicitBaseTypeAndInterfaces()
            .Where(type => !(options.IgnorePrivateOrAssembly && (type.IsNotPublic || type.IsNestedAssembly || type.IsNestedFamily || type.IsNestedFamANDAssem || type.IsNestedPrivate)))
            .Select(type => {
              referencingNamespaces?.AddRange(CSharpFormatter.ToNamespaceList(type));
              return new {IsInterface = type.IsInterface, Name = type.FormatTypeName(typeWithNamespace: options.TypeDeclarationWithNamespace)};
            })
            .OrderBy(type => type.IsInterface)
            .ThenBy(type => type.Name, StringComparer.Ordinal)
            .Select(type => type.Name);
  }

  internal static IEnumerable<string> GenerateTypeDeclaration(Type t, ISet<string> referencingNamespaces, Options options)
  {
    if (options == null)
      throw new ArgumentNullException(nameof(options));

    var accessibilities = CSharpFormatter.FormatAccessibility(t.GetAccessibility());
    var typeName = t.FormatTypeName(typeWithNamespace: false, withDeclaringTypeName: false);

    var genericArgumentConstraints = t.GetGenericArguments()
                                      .Select(arg => GenerateGenericArgumentConstraintDeclaration(arg, referencingNamespaces, typeWithNamespace: options.TypeDeclarationWithNamespace))
                                      .Where(d => d != null)
                                      .ToList();

    if (t.IsEnum) {
      yield return $"{accessibilities} enum {typeName} : {t.GetEnumUnderlyingType().FormatTypeName()}";
      yield break;
    }
    else if (t.IsDelegate()) {
      var signatureInfo = t.GetDelegateSignatureMethod();

      referencingNamespaces?.AddRange(signatureInfo.GetSignatureTypes().Where(mpt => !mpt.ContainsGenericParameters).SelectMany(CSharpFormatter.ToNamespaceList));

      var genericArgumentConstraintDeclaration = genericArgumentConstraints.Count == 0 ? string.Empty : " " + string.Join(" ", genericArgumentConstraints);

      yield return $"{accessibilities} delegate {signatureInfo.ReturnType.FormatTypeName(attributeProvider: signatureInfo.ReturnTypeCustomAttributes, typeWithNamespace: options.TypeDeclarationWithNamespace)} {typeName}({CSharpFormatter.FormatParameterList(signatureInfo, useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral)}){genericArgumentConstraintDeclaration};";
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

    var baseTypeList = GetExplicitBaseTypeAndInterfacesAsString(t, referencingNamespaces, options).ToList();

    if (baseTypeList.Count <= 1) {
      var baseTypeDeclaration = baseTypeList.Count == 0 ? string.Empty : " : " + baseTypeList[0];
      var genericArgumentConstraintDeclaration = genericArgumentConstraints.Count == 0 ? string.Empty : " " + string.Join(" ", genericArgumentConstraints);

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

  static string GenerateGenericArgumentConstraintDeclaration(Type genericArgument, ISet<string> referencingNamespaces, bool typeWithNamespace = true)
  {
    IEnumerable<string> GetGenericArgumentConstraintsOf(Type argument)
    {
      var constraintAttrs = argument.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
      var constraintTypes = argument.GetGenericParameterConstraints();

      referencingNamespaces?.AddRange(constraintTypes.Where(ct => ct != typeof(ValueType)).SelectMany(CSharpFormatter.ToNamespaceList));

      if (constraintAttrs.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint) &&
          constraintAttrs.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint) &&
          constraintTypes.Any(ct => ct == typeof(ValueType))) {

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

    var constraints = string.Join(", ", GetGenericArgumentConstraintsOf(genericArgument));

    if (0 < constraints.Length)
      return $"where {genericArgument.FormatTypeName(typeWithNamespace: false)} : {constraints}";
    else
      return null;
  }

  static string GenerateTypeContentDeclarations(int nestLevel, Type t, ISet<string> referencingNamespaces, Options options)
  {
    if (options == null)
      throw new ArgumentNullException(nameof(options));

    const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    var members = t.GetMembers(bindingFlags).OrderBy(f => f.Name, StringComparer.Ordinal).ToList();
    var exceptingMembers = new List<MemberInfo>();
    var nestedTypes = new List<Type>();

    foreach (var member in members) {
      if (member is PropertyInfo p) {
        // remove get/set accessor of properties
        exceptingMembers.AddRange(p.GetAccessors(true));
      }
      else if (member is EventInfo e) {
        // remove add/remove/raise method of events
        exceptingMembers.AddRange(e.GetMethods(true));
      }
      else if (member.MemberType == MemberTypes.NestedType) {
        exceptingMembers.Add(member);
        nestedTypes.Add(member as Type);
      }
    }

    var ret = new StringBuilder(1024);
    var prevMemberType = (MemberTypes?)null;

    if (0 < nestedTypes.Count) {
      ret.Append(GenerateTypeAndMemberDeclarations(nestLevel,
                                                   nestedTypes.Where(nestedType => !(options.IgnorePrivateOrAssembly && (nestedType.IsNestedPrivate || nestedType.IsNestedAssembly))),
                                                   referencingNamespaces,
                                                   options));

      if (0 < ret.Length)
        prevMemberType = MemberTypes.NestedType;
    }

    var indent = string.Concat(Enumerable.Repeat(options.Indent, nestLevel));
    var memberAndDeclarations = new List<(MemberInfo member, string declaration)>();

    foreach (var member in members.Except(exceptingMembers).OrderBy(OrderOfMember).ThenBy(m => m.Name, StringComparer.Ordinal)) {
      try {
        var declaration = GenerateMemberDeclaration(member, referencingNamespaces, options);

        if (declaration == null)
          continue; // is private or assembly

        memberAndDeclarations.Add((member, declaration));
      }
      catch (Exception ex) {
        Console.Error.WriteLine($"reflection error at member {t.FullName}.{member.Name}");
        Console.Error.WriteLine(ex);
      }
    }

    foreach (var (member, declaration) in memberAndDeclarations.OrderBy(p => OrderOfMember(p.member)).ThenBy(p => p.member.Name, StringComparer.Ordinal).ThenBy(p => p.declaration, StringComparer.Ordinal)) {
      if (prevMemberType != null && prevMemberType != member.MemberType)
        ret.AppendLine();

      // TODO: AttributeTargets.ReturnValue, AttributeTargets.Parameter
      foreach (var attr in GenerateAttributeList(member, null, options)) {
        ret.Append(indent).AppendLine(attr);
      }

      ret.Append(indent).Append(declaration);

      prevMemberType = member.MemberType;
    }

    return ret.ToString();

    int OrderOfMember(MemberInfo member)
    {
      switch (member) {
        case EventInfo e:           return 1;
        case FieldInfo f:           return 2;
        case ConstructorInfo ctor:  return 3;
        case PropertyInfo p:        return 4;
        case MethodInfo m:          return 5;
        default:                    return int.MaxValue;
      }
    }
  }

  static string GetMemberModifierOf(MemberInfo member)
  {
    return GetMemberModifierOf(member, out _, out _);
  }

  // TODO: async, extern, volatile
  static string GetMemberModifierOf(MemberInfo member, out string setMethodAccessibility, out string getMethodAccessibility)
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

  internal static string GenerateMemberDeclaration(MemberInfo member, ISet<string> referencingNamespaces, Options options)
  {
    if (options == null)
      throw new ArgumentNullException(nameof(options));

    var sb = new StringBuilder();

    switch (member) {
      case FieldInfo f: {
        if (options.IgnorePrivateOrAssembly && (f.IsPrivate || f.IsAssembly || f.IsFamilyAndAssembly))
          return null;

        if (member.DeclaringType.IsEnum) {
          if (f.IsStatic) {
            var val = Convert.ChangeType(f.GetValue(null), member.DeclaringType.GetEnumUnderlyingType());

            sb.Append(f.Name).Append(" = ");

            if (f.DeclaringType.IsEnumFlags())
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
          referencingNamespaces?.AddRange(CSharpFormatter.ToNamespaceList(f.FieldType));

          sb.Append($"{GetMemberModifierOf(f)} {f.FieldType.FormatTypeName(attributeProvider: f, typeWithNamespace: options.MemberDeclarationWithNamespace)} {f.Name}");

          if (f.IsStatic && (f.IsLiteral || f.IsInitOnly) && !f.FieldType.ContainsGenericParameters) {
            var val = f.GetValue(null);
            var valueDeclaration = CSharpFormatter.FormatValueDeclaration(val,
                                                                          f.FieldType,
                                                                          typeWithNamespace: options.MemberDeclarationWithNamespace,
                                                                          findConstantField: (f.FieldType != f.DeclaringType),
                                                                          useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral);

            if (valueDeclaration == null)
              sb.Append($"; // = \"{CSharpFormatter.EscapeString((val ?? "null").ToString(), escapeDoubleQuote: true)}\"");
            else
              sb.Append($" = {valueDeclaration};");
          }
          else {
            sb.Append(";");
          }
        }

        break;
      }

      case PropertyInfo p: {
        var explicitInterface = p.GetAccessors(true).Select(a => a.FindExplicitInterfaceMethod(findOnlyPublicInterfaces: options.IgnorePrivateOrAssembly)?.DeclaringType).FirstOrDefault();

        if (explicitInterface == null && options.IgnorePrivateOrAssembly && p.GetAccessors(true).All(a => a.IsPrivate || a.IsAssembly || a.IsFamilyAndAssembly))
          return null;

        var emitGetAccessor = p.GetMethod != null && !(explicitInterface == null && options.IgnorePrivateOrAssembly && (p.GetMethod.IsPrivate || p.GetMethod.IsAssembly || p.GetMethod.IsFamilyAndAssembly));
        var emitSetAccessor = p.SetMethod != null && !(explicitInterface == null && options.IgnorePrivateOrAssembly && (p.SetMethod.IsPrivate || p.SetMethod.IsAssembly || p.SetMethod.IsFamilyAndAssembly));

        var indexParameters = p.GetIndexParameters();

        referencingNamespaces?.AddRange(CSharpFormatter.ToNamespaceList(p.PropertyType));
        referencingNamespaces?.AddRange(indexParameters.SelectMany(ip => CSharpFormatter.ToNamespaceList(ip.ParameterType)));

        var modifier = GetMemberModifierOf(p, out string setAccessibility, out string getAccessibility);

        if (explicitInterface == null && 0 < modifier.Length)
          sb.Append($"{modifier} ");

        sb.Append($"{p.PropertyType.FormatTypeName(attributeProvider: p, typeWithNamespace: options.MemberDeclarationWithNamespace)} ");

        var propertyName = explicitInterface == null ? p.Name : p.Name.Substring(p.Name.LastIndexOf('.') + 1);

        if (0 < indexParameters.Length &&
            string.Equals(p.Name, p.DeclaringType.GetCustomAttribute<DefaultMemberAttribute>()?.MemberName, StringComparison.Ordinal))
          sb.Append("this"); // indexer
        else if (explicitInterface == null)
          sb.Append(propertyName);
        else
          sb.Append(explicitInterface.FormatTypeName(attributeProvider: p, typeWithNamespace: options.MemberDeclarationWithNamespace)).Append(".").Append(propertyName);

        if (0 < indexParameters.Length)
          sb.Append($"[{CSharpFormatter.FormatParameterList(indexParameters, typeWithNamespace: options.MemberDeclarationWithNamespace, useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral)}] ");
        else
          sb.Append(" ");

        sb.Append("{ ");

        if (emitGetAccessor) {
          if (explicitInterface == null && 0 < getAccessibility.Length)
            sb.Append(getAccessibility).Append(" ");

          sb.Append("get");

          if (options.GenerateEmptyImplementation && !p.GetMethod.IsAbstract)
            sb.Append(" => throw new NotImplementedException(); ");
          else
            sb.Append("; ");
        }

        if (emitSetAccessor) {
          if (explicitInterface == null && 0 < setAccessibility.Length)
            sb.Append(setAccessibility).Append(" ");

          sb.Append("set");

          if (options.GenerateEmptyImplementation && !p.SetMethod.IsAbstract)
            sb.Append(" => throw new NotImplementedException(); ");
          else
            sb.Append("; ");
        }

        sb.Append("}");

#if false
        if (p.CanRead)
          sb.Append(" = ").Append(p.GetConstantValue()).Append(";");
#endif

        break;
      }

      case MethodBase m: {
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
        var methodConstraints = method == null ? null : string.Join(" ", method.GetGenericArguments().Select(arg => GenerateGenericArgumentConstraintDeclaration(arg, referencingNamespaces, typeWithNamespace: options.MemberDeclarationWithNamespace)).Where(d => d != null));
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
            case "op_Equality":             methodName = "operator == "; break;
            case "op_Inequality":           methodName = "operator != "; break;
            case "op_LessThan":             methodName = "operator < "; break;
            case "op_GreaterThan":          methodName = "operator > "; break;
            case "op_LessThanOrEqual":      methodName = "operator <= "; break;
            case "op_GreaterThanOrEqual":   methodName = "operator >= "; break;

            // unary
            case "op_UnaryPlus":        methodName = "operator + "; break;
            case "op_UnaryNegation":    methodName = "operator - "; break;
            case "op_LogicalNot":       methodName = "operator ! "; break;
            case "op_OnesComplement":   methodName = "operator ~ "; break;
            case "op_True":             methodName = "operator true "; break;
            case "op_False":            methodName = "operator false "; break;
            case "op_Increment":        methodName = "operator ++ "; break;
            case "op_Decrement":        methodName = "operator -- "; break;

            // binary
            case "op_Addition":     methodName = "operator + "; break;
            case "op_Subtraction":  methodName = "operator - "; break;
            case "op_Multiply":     methodName = "operator * "; break;
            case "op_Division":     methodName = "operator / "; break;
            case "op_Modulus":      methodName = "operator % "; break;
            case "op_BitwiseAnd":   methodName = "operator & "; break;
            case "op_BitwiseOr":    methodName = "operator | "; break;
            case "op_ExclusiveOr":  methodName = "operator ^ "; break;
            case "op_RightShift":   methodName = "operator >> "; break;
            case "op_LeftShift":    methodName = "operator << "; break;

            // type cast
            case "op_Explicit":     methodName = "explicit operator " + methodReturnType; methodReturnType = null; break;
            case "op_Implicit":     methodName = "implicit operator " + methodReturnType; methodReturnType = null; break;
          }
        }
        else if (explicitInterfaceMethod != null) {
          methodModifiers = null;
          methodName = explicitInterfaceMethod.DeclaringType.FormatTypeName(typeWithNamespace: options.MemberDeclarationWithNamespace) + "." + explicitInterfaceMethod.Name;
        }
        else {
          // standard methods
        }

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

        break;
      }

      case EventInfo ev: {
        var explicitInterface = ev.GetMethods(true).Select(evm => evm.FindExplicitInterfaceMethod(findOnlyPublicInterfaces: options.IgnorePrivateOrAssembly)?.DeclaringType).FirstOrDefault();

        if (explicitInterface == null && options.IgnorePrivateOrAssembly && ev.GetMethods(true).All(m => m.IsPrivate || m.IsAssembly || m.IsFamilyAndAssembly))
          return null;

        referencingNamespaces?.AddRange(CSharpFormatter.ToNamespaceList(ev.EventHandlerType));

        var modifier = GetMemberModifierOf(ev.GetMethods(true).First());

        if (explicitInterface == null && 0 < modifier.Length)
          sb.Append($"{modifier} ");

        sb.Append($"event {ev.EventHandlerType.FormatTypeName(attributeProvider: ev, typeWithNamespace: options.MemberDeclarationWithNamespace)} ");

        var eventName = explicitInterface == null ? ev.Name : ev.Name.Substring(explicitInterface.FullName.Length + 1);

        if (explicitInterface == null)
          sb.Append(eventName);
        else
          sb.Append(explicitInterface.FormatTypeName(attributeProvider: ev, typeWithNamespace: options.MemberDeclarationWithNamespace)).Append(".").Append(eventName);

        sb.Append(";");

        break;
      }

      default:
        if (member.MemberType == MemberTypes.NestedType)
          throw new ArgumentException("can not generate nested type declarations");
        else
          throw new NotSupportedException($"unsupported member type: {member.MemberType}");
    }

    if (options.MemberDeclarationEmitNewLine)
      sb.AppendLine();

    return sb.ToString();
  }

  internal static IEnumerable<string> GenerateAttributeList(ICustomAttributeProvider attributeProvider, ISet<string> referencingNamespaces, Options options)
  {
    return GetAttributes().OrderBy(attr => attr.GetType().FullName)
                          .Select(attr => new {Name = ConvertAttributeName(attr), Params = ConvertAttributeParameters(attr)})
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
        if (t.StructLayoutAttribute.Value != DefaultLayoutStruct.Attribute.Value ||
            t.StructLayoutAttribute.Pack != DefaultLayoutStruct.Attribute.Pack ||
            t.StructLayoutAttribute.CharSet != DefaultLayoutStruct.Attribute.CharSet)
          yield return t.StructLayoutAttribute;
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
      return CSharpFormatter.FormatValueDeclaration(@value,
                                                    @value?.GetType(),
                                                    typeWithNamespace: options.MemberDeclarationWithNamespace,
                                                    findConstantField: true,
                                                    useDefaultLiteral: options.MemberDeclarationUseDefaultLiteral);
    }
  }

  internal struct DefaultLayoutStruct {
    // XXX
    public static readonly StructLayoutAttribute Attribute = typeof(DefaultLayoutStruct).StructLayoutAttribute;
  }
}

