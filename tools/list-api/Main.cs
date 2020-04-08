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
using Smdn.Reflection.ReverseGenerating;

class Options : GeneratorOptions {
  public string Indent = new string(' ', 2);

  public bool GenerateEmptyImplementation = false;
  public bool MemberDeclarationEmitNewLine = true;

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
    foreach (var attr in Generator.GenerateAttributeList(t, null, options)) {
      ret.Append(indent)
         .AppendLine(attr);
    }

    var typeDeclarationLines = Generator.GenerateTypeDeclaration(t, referencingNamespaces, options).ToList();

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

  internal static string GenerateTypeContentDeclarations(int nestLevel, Type t, ISet<string> referencingNamespaces, Options options)
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
      foreach (var attr in Generator.GenerateAttributeList(member, null, options)) {
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
        var methodConstraints = method == null ? null : string.Join(" ", method.GetGenericArguments().Select(arg => Generator.GenerateGenericArgumentConstraintDeclaration(arg, referencingNamespaces, typeWithNamespace: options.MemberDeclarationWithNamespace)).Where(d => d != null));
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
}

