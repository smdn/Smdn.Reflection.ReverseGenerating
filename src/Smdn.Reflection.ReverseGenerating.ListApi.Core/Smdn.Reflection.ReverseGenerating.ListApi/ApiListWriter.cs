// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class ApiListWriter {
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
    BaseWriter.WriteLine($"// {Path.GetFileName(assembly.Location)} ({assembly.GetAssemblyMetadataAttributeValue<AssemblyProductAttribute, string>()})");
    BaseWriter.WriteLine($"//   Name: {assembly.GetName().Name}");
    BaseWriter.WriteLine($"//   AssemblyVersion: {assembly.GetName().Version}");
    BaseWriter.WriteLine($"//   InformationalVersion: {assembly.GetAssemblyMetadataAttributeValue<AssemblyInformationalVersionAttribute, string>()}");
    BaseWriter.WriteLine($"//   TargetFramework: {assembly.GetAssemblyMetadataAttributeValue<TargetFrameworkAttribute, string>()}");
    BaseWriter.WriteLine($"//   Configuration: {assembly.GetAssemblyMetadataAttributeValue<AssemblyConfigurationAttribute, string>()}");
  }

  public void WriteExportedTypes()
  {
    var types = assembly.GetExportedTypes()
#if SYSTEM_ASSEMBLY_GETFORWARDEDTYPES
      .Union(assembly.GetForwardedTypes())
#endif
    ;

    var typeDeclarations = new StringBuilder(10240);
    var referencingNamespaces = new HashSet<string>(StringComparer.Ordinal);

    foreach (var ns in types.Select(static t => t.Namespace).Distinct().OrderBy(static n => n, StringComparer.Ordinal)) {
      if (0 < typeDeclarations.Length)
        typeDeclarations.AppendLine();

      typeDeclarations
        .Append("namespace ")
        .Append(ns)
        .Append(" {")
        .AppendLine();

      typeDeclarations.Append(
        GenerateTypeAndMemberDeclarations(
          nestLevel: 1,
          assembly,
          types
            .Where(type => type.Namespace.Equals(ns, StringComparison.Ordinal))
            .Where(static type => !type.IsNested),
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
      .ThenBy(static ns => ns, StringComparer.Ordinal);

    if (options.Writer.WriteNullableAnnotationDirective) {
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      if (
        options.TypeDeclaration.NullabilityInfoContext is not null &&
        options.MemberDeclaration.NullabilityInfoContext is not null
      ) {
        BaseWriter.WriteLine("#nullable enable annotations");
        BaseWriter.WriteLine();
      }
      else if (
        options.TypeDeclaration.NullabilityInfoContext is null &&
        options.MemberDeclaration.NullabilityInfoContext is null
      ) {
        BaseWriter.WriteLine("#nullable disable annotations");
        BaseWriter.WriteLine();
      }
      else {
        BaseWriter.WriteLine();
      }
#else
      BaseWriter.WriteLine();
#endif
    }
    else {
      BaseWriter.WriteLine();
    }

    foreach (var ns in orderedReferencingNamespaces) {
      BaseWriter.WriteLine($"using {ns};");
    }

    BaseWriter.WriteLine();
    BaseWriter.Write(typeDeclarations);
  }

  private static string GenerateTypeAndMemberDeclarations(
    int nestLevel,
    Assembly assm,
    IEnumerable<Type> types,
    ISet<string> referencingNamespaces,
    ApiListWriterOptions options
  )
  {
    var ret = new StringBuilder(10240);
    var isPrevDelegate = false;

    static int OrderOfType(Type t)
    {
      if (t.IsDelegate()) return 1;
      if (t.IsInterface) return 2;
      if (t.IsEnum) return 3;
      if (t.IsClass) return 4;
      if (t.IsValueType) return 5;

      return int.MaxValue;
    }

    var orderedTypes = types
      .OrderBy(OrderOfType)
      .ThenBy(static type => type.FullName, StringComparer.Ordinal);

    var enableNullableAnnotationsOnlyOnTypes =
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      options.Writer.WriteNullableAnnotationDirective &&
      options.TypeDeclaration.NullabilityInfoContext is not null &&
      options.MemberDeclaration.NullabilityInfoContext is null;
#else
      false;
#endif
    var enableNullableAnnotationsOnlyOnMembers =
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
      options.Writer.WriteNullableAnnotationDirective &&
      options.TypeDeclaration.NullabilityInfoContext is null &&
      options.MemberDeclaration.NullabilityInfoContext is not null;
#else
      false;
#endif

    foreach (var type in orderedTypes) {
      var isDelegate = type.IsDelegate();

      if (0 < ret.Length && !(isPrevDelegate && isDelegate))
        ret.AppendLine();

      try {
        ret.Append(
          GenerateTypeAndMemberDeclarations(
            nestLevel,
            assm,
            type,
            referencingNamespaces,
            options,
            enableNullableAnnotationsOnlyOnTypes,
            enableNullableAnnotationsOnlyOnMembers
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
  private static string GenerateTypeAndMemberDeclarations(
    int nestLevel,
    Assembly assm,
    Type t,
    ISet<string> referencingNamespaces,
    ApiListWriterOptions options,
    bool enableNullableAnnotationsOnlyOnTypes,
    bool enableNullableAnnotationsOnlyOnMembers
  )
  {
    if (options == null)
      throw new ArgumentNullException(nameof(options));

    var ret = new StringBuilder(1024);
    var indent = string.Concat(Enumerable.Repeat(options.Indent, nestLevel));

    var assemblyNameOfTypeForwardedFrom = t
      .GetCustomAttributesData()
      .FirstOrDefault(static d => ROCType.FullNameEquals(typeof(TypeForwardedFromAttribute), d.AttributeType))
      ?.ConstructorArguments
      ?.FirstOrDefault()
      .Value
      as string;

    if (
      assemblyNameOfTypeForwardedFrom is not null &&
      string.Equals(assm.FullName, assemblyNameOfTypeForwardedFrom, StringComparison.Ordinal)
    ) {
      ret
        .Append(indent)
        .Append("// Forwarded to \"")
        .Append(t.Assembly.FullName)
        .Append('"')
        .AppendLine();
    }

    foreach (var attr in Generator.GenerateAttributeList(t, null, options)) {
      ret.Append(indent)
         .AppendLine(attr);
    }

    if (enableNullableAnnotationsOnlyOnTypes)
      ret.AppendLine("#nullable enable annotations");

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

      if (enableNullableAnnotationsOnlyOnTypes)
        ret.AppendLine("#nullable restore annotations");
      if (enableNullableAnnotationsOnlyOnMembers)
        ret.AppendLine("#nullable enable annotations");

      ret.Append(
        GenerateTypeContentDeclarations(
          nestLevel + 1,
          assm,
          t,
          referencingNamespaces,
          options
        )
      );

      if (enableNullableAnnotationsOnlyOnMembers)
        ret.AppendLine("#nullable restore annotations");

      ret.Append(indent).AppendLine("}");
    }
    else {
      ret.AppendLine();
    }

    return ret.ToString();
  }

  private static string GenerateTypeContentDeclarations(
    int nestLevel,
    Assembly assm,
    Type t,
    ISet<string> referencingNamespaces,
    ApiListWriterOptions options
  )
  {
    if (options == null)
      throw new ArgumentNullException(nameof(options));

    const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    var members = t.GetMembers(bindingFlags).OrderBy(static f => f.Name, StringComparer.Ordinal).ToList();
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
      else if (member.MemberType == MemberTypes.NestedType && member is Type nestedType) {
        exceptingMembers.Add(nestedType);
        nestedTypes.Add(nestedType);
      }
    }

    var ret = new StringBuilder(1024);
    var generatedNestedTypeDeclarations = false;

    if (0 < nestedTypes.Count) {
      ret.Append(
        GenerateTypeAndMemberDeclarations(
          nestLevel,
          assm,
          nestedTypes.Where(nestedType => !(options.IgnorePrivateOrAssembly && (nestedType.IsNestedPrivate || nestedType.IsNestedAssembly))),
          referencingNamespaces,
          options
        )
      );

      if (0 < ret.Length)
        generatedNestedTypeDeclarations = true;
    }

    var indent = string.Concat(Enumerable.Repeat(options.Indent, nestLevel));
    var memberAndDeclarations = new List<(MemberInfo Member, string Declaration)>();

    foreach (var member in members.Except(exceptingMembers)) {
      string? declaration = null;

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

    var memberComparer = options.Writer.OrderStaticMembersFirst
      ? MemberInfoComparer.StaticMembersFirst
      : MemberInfoComparer.Default;
    var orderedMemberAndDeclarations = memberAndDeclarations
      .Select(t => (t.Member, t.Declaration, Order: memberComparer.GetOrder(t.Member)))
      .OrderBy(static t => t.Order)
      .ThenBy(static t => t.Member.Name, StringComparer.Ordinal)
      .ThenBy(static t => t.Declaration, StringComparer.Ordinal);
    int? prevOrder = generatedNestedTypeDeclarations ? int.MinValue : null;

    foreach (var (member, declaration, order) in orderedMemberAndDeclarations) {
      if (prevOrder != null && prevOrder.Value != order)
        ret.AppendLine();

      foreach (var attr in Generator.GenerateAttributeList(member, null, options)) {
        ret.Append(indent).AppendLine(attr);
      }

      ret.Append(indent).AppendLine(declaration);

      prevOrder = order;
    }

    return ret.ToString();
  }
}
