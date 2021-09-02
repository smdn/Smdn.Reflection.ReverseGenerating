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
    var showUsage = false;
    var stdout = false;

    foreach (var arg in args) {
      switch (arg) {
        case "--generate-impl":
          options.MemberDeclarationMethodBody = MethodBodyOption.ThrowNotImplementedException;
          break;

        case "--generate-fullname":
          options.TypeDeclarationWithNamespace = true;
          options.MemberDeclarationWithNamespace = true;
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
      Console.Error.WriteLine("--stdout: output to stdout");
      Console.Error.WriteLine("--generate-fullname: generate type and member declaration with full type name");
      Console.Error.WriteLine("--generate-impl: generate with empty implementation");
      Console.Error.WriteLine();
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

    var types = assm.GetExportedTypes().Union(assm.GetForwardedTypes());
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

    if (t.GetCustomAttribute<TypeForwardedFromAttribute>() is not null) {
      ret.Append(indent)
         .AppendLine($"// Forwarded to \"{t.Assembly.GetName().FullName}\"");
    }

    // TODO: AttributeTargets.GenericParameter, AttributeTargets.ReturnValue, AttributeTargets.Parameter
    foreach (var attr in Generator.GenerateAttributeList(t, null, options)) {
      ret.Append(indent)
         .AppendLine(attr);
    }

    var typeDeclarationLines = Generator.GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(t, referencingNamespaces, options).ToList();

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
        var declaration = Generator.GenerateMemberDeclaration(member, referencingNamespaces, options);

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

      ret.Append(indent).AppendLine(declaration);

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
}

