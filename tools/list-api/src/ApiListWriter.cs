using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;

using Smdn.Reflection;
using Smdn.Reflection.ReverseGenerating;

class ApiListWriter {
  public TextWriter BaseWriter { get; }

  private readonly Assembly assembly;
  private readonly ApiListWriterOptions options;

  public ApiListWriter(TextWriter baseWriter, Assembly assembly, ApiListWriterOptions options)
  {
    this.BaseWriter = baseWriter;
    this.assembly = assembly;
    this.options = options ?? new();
  }

  public void WriteAssemblyInfoHeader()
  {
    BaseWriter.WriteLine($"// {assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product}");
    BaseWriter.WriteLine($"//   Name: {assembly.GetName().Name}");
    BaseWriter.WriteLine($"//   AssemblyVersion: {assembly.GetName().Version}");
    BaseWriter.WriteLine($"//   InformationalVersion: {assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
    BaseWriter.WriteLine($"//   TargetFramework: {assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName}");
    BaseWriter.WriteLine($"//   Configuration: {assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration}");
    BaseWriter.WriteLine();
  }

  public void WriteExportedTypes()
  {
    var types = assembly.GetExportedTypes().Union(assembly.GetForwardedTypes());
    var typeDeclarations = new StringBuilder(10240);
    var referencingNamespaces = new HashSet<string>(StringComparer.Ordinal);

    foreach (var ns in types.Select(t => t.Namespace).Distinct().OrderBy(n => n, StringComparer.Ordinal)) {
      if (0 < typeDeclarations.Length)
        typeDeclarations.AppendLine();

      typeDeclarations.AppendLine($"namespace {ns} {{");

      typeDeclarations.Append(
        GenerateTypeAndMemberDeclarations(
          nestLevel: 1,
          types
            .Where(type => type.Namespace.Equals(ns, StringComparison.Ordinal))
            .Where(type => !type.IsNested),
          referencingNamespaces,
          options
        )
      );

      typeDeclarations.AppendLine("}");
    }

    static int OrderOfRootNamespace(string ns)
      => ns.Split('.')[0] switch {
        "System" => 0,
        "Microsoft" => 1,
        _ => int.MaxValue,
      };

    var orderedReferencingNamespaces = referencingNamespaces
      .OrderBy(OrderOfRootNamespace)
      .ThenBy(ns => ns, StringComparer.Ordinal);

    foreach (var ns in orderedReferencingNamespaces) {
      BaseWriter.WriteLine($"using {ns};");
    }

    BaseWriter.WriteLine();
    BaseWriter.WriteLine(typeDeclarations);
  }

  static string GenerateTypeAndMemberDeclarations(
    int nestLevel,
    IEnumerable<Type> types,
    ISet<string> referencingNamespaces,
    ApiListWriterOptions options
  )
  {
    var ret = new StringBuilder(10240);
    var isPrevDelegate = false;

    static int OrderOfType(Type t)
    {
      if (t.IsDelegate())   return 1;
      if (t.IsInterface)    return 2;
      if (t.IsEnum)         return 3;
      if (t.IsClass)        return 4;
      if (t.IsValueType)    return 5;

      return int.MaxValue;
    }

    var orderedTypes = types
      .OrderBy(OrderOfType)
      .ThenBy(type => type.FullName, StringComparer.Ordinal);

    foreach (var type in orderedTypes) {
      var isDelegate = type.IsDelegate();

      if (0 < ret.Length && !(isPrevDelegate && isDelegate))
        ret.AppendLine();

      try {
        ret.Append(
          GenerateTypeAndMemberDeclarations(
            nestLevel,
            type,
            referencingNamespaces,
            options
          )
        );
      }
      catch (Exception ex) {
        throw new InvalidOperationException($"generator error on type '{type.FullName}'", ex);
      }

      isPrevDelegate = isDelegate;
    }

    return ret.ToString();
  }

  // TODO: unsafe types
  static string GenerateTypeAndMemberDeclarations(
    int nestLevel,
    Type t,
    ISet<string> referencingNamespaces,
    ApiListWriterOptions options
  )
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

  static string GenerateTypeContentDeclarations(
    int nestLevel,
    Type t,
    ISet<string> referencingNamespaces,
    ApiListWriterOptions options
  )
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
    var generatedNestedTypeDeclarations = false;

    if (0 < nestedTypes.Count) {
      ret.Append(
        GenerateTypeAndMemberDeclarations(
          nestLevel,
          nestedTypes.Where(nestedType => !(options.IgnorePrivateOrAssembly && (nestedType.IsNestedPrivate || nestedType.IsNestedAssembly))),
          referencingNamespaces,
          options
        )
      );

      if (0 < ret.Length)
        generatedNestedTypeDeclarations = true;
    }

    var indent = string.Concat(Enumerable.Repeat(options.Indent, nestLevel));
    var memberAndDeclarations = new List<(MemberInfo member, string declaration)>();

    foreach (var member in members.Except(exceptingMembers)) {
      string declaration = null;

      try {
        declaration = Generator.GenerateMemberDeclaration(member, referencingNamespaces, options);
      }
      catch (Exception ex) {
        throw new InvalidOperationException($"generator error on member '{t.FullName}.{member.Name}'", ex);
      }

      if (declaration == null)
        continue; // is private or assembly

      memberAndDeclarations.Add((member, declaration));
    }

    var memberComparer = options.OrderStaticMembersFirst
      ? MemberInfoComparer.StaticMembersFirst
      : MemberInfoComparer.Default;
    var orderedMemberAndDeclarations = memberAndDeclarations
      .Select(t => (t.member, t.declaration, order: memberComparer.GetOrder(t.member)))
      .OrderBy(t => t.order)
      .ThenBy(t => t.member.Name, StringComparer.Ordinal)
      .ThenBy(t => t.declaration, StringComparer.Ordinal);
    int? prevOrder = 0 < generatedNestedTypeDeclarations ? int.MinValue : (int?)null;

    foreach (var (member, declaration, order) in orderedMemberAndDeclarations) {
      if (prevOrder != null && prevOrder.Value != order)
        ret.AppendLine();

      // TODO: AttributeTargets.ReturnValue, AttributeTargets.Parameter
      foreach (var attr in Generator.GenerateAttributeList(member, null, options)) {
        ret.Append(indent).AppendLine(attr);
      }

      ret.Append(indent).AppendLine(declaration);

      prevOrder = order;
    }

    return ret.ToString();
  }
}