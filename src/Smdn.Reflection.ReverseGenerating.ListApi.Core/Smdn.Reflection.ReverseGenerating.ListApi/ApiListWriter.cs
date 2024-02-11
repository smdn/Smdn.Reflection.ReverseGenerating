// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class ApiListWriter {
  private static readonly Action<ILogger, string?, Exception?> LoggerMessageGeneratorErrorOnType = LoggerMessage.Define<string?>(
    LogLevel.Error,
    new(1, nameof(LoggerMessageGeneratorErrorOnType)),
    "generator error on type '{TypeFullName}'"
  );
  private static readonly Action<ILogger, string?, string, Exception?> LoggerMessageGeneratorErrorOnMember = LoggerMessage.Define<string?, string>(
    LogLevel.Error,
    new(1, nameof(LoggerMessageGeneratorErrorOnMember)),
    "generator error on member '{TypeFullName}.{MemberName}'"
  );

  public TextWriter BaseWriter { get; }

  private readonly Assembly assembly;
  private readonly ApiListWriterOptions options;
  private readonly ILogger? logger;

  public ApiListWriter(
    TextWriter baseWriter,
    Assembly assembly,
    ApiListWriterOptions? options
  )
    : this(
      baseWriter: baseWriter ?? throw new ArgumentNullException(nameof(baseWriter)),
      assembly: assembly ?? throw new ArgumentNullException(nameof(assembly)),
      options: options ?? new(),
      logger: null
    )
  {
  }

  public ApiListWriter(
    TextWriter baseWriter,
    Assembly assembly,
    ApiListWriterOptions? options,
    ILogger? logger
  )
  {
    this.BaseWriter = baseWriter ?? throw new ArgumentNullException(nameof(baseWriter));
    this.assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
    this.options = options ?? new();
    this.logger = logger;
  }

  public void WriteHeader()
  {
    if (!options.Writer.WriteHeader)
      return;

    if (options.Writer.WriteAssemblyInfo)
      WriteAssemblyInfo();

    if (options.Writer.WriteReferencedAssemblies)
      WriteReferencedAssemblies();

    if (options.Writer.WriteEmbeddedResources)
      WriteEmbeddedResources();
  }

  [Obsolete($"Use {nameof(WriteHeader)}")]
  public void WriteAssemblyInfoHeader()
    => WriteAssemblyInfo();

  private void WriteAssemblyInfo()
  {
    BaseWriter.WriteLine($"// {Path.GetFileName(assembly.Location)} ({assembly.GetAssemblyMetadataAttributeValue<AssemblyProductAttribute, string>()})");
    BaseWriter.WriteLine($"//   Name: {assembly.GetName().Name}");
    BaseWriter.WriteLine($"//   AssemblyVersion: {assembly.GetName().Version}");
    BaseWriter.WriteLine($"//   InformationalVersion: {assembly.GetAssemblyMetadataAttributeValue<AssemblyInformationalVersionAttribute, string>()}");
    BaseWriter.WriteLine($"//   TargetFramework: {assembly.GetAssemblyMetadataAttributeValue<TargetFrameworkAttribute, string>()}");
    BaseWriter.WriteLine($"//   Configuration: {assembly.GetAssemblyMetadataAttributeValue<AssemblyConfigurationAttribute, string>()}");
  }

  private unsafe void WriteReferencedAssemblies()
  {
    BaseWriter.WriteLine("//   Referenced assemblies:");

    if (assembly.TryGetRawMetadata(out var blobPtr, out var blobLength))
      WriteReferencedAssembliesFromRawMetadata(blobPtr, blobLength);
    else
      WriteReferencedAssembliesUsingGetReferencedAssemblies();
  }

  private void WriteReferencedAssembliesUsingGetReferencedAssemblies()
  {
    var orderedReferencedAssemblies = assembly
      .GetReferencedAssemblies()
      .OrderBy(static refassm => refassm.Name, StringComparer.Ordinal);

    foreach (var referencedAssembly in orderedReferencedAssemblies) {
      BaseWriter.WriteLine($"//     {referencedAssembly.FullName}");
    }
  }

  private unsafe void WriteReferencedAssembliesFromRawMetadata(byte* blobPtr, int blobLength)
  {
    var reader = new MetadataReader(blobPtr, blobLength);
    var assemblyReferences = reader
      .AssemblyReferences
      .Select(reader.GetAssemblyReference)
      .ToDictionary(
        assmRef => reader.GetString(assmRef.Name),
        assmRef => assmRef
      );

    foreach (var (name, assmRef) in assemblyReferences.OrderBy(static pair => pair.Key, StringComparer.Ordinal)) {
      var culture = reader.GetString(assmRef.Culture);

      if (string.IsNullOrEmpty(culture))
        culture = "neutral";

      var publicKeyOrToken = reader.GetBlobBytes(assmRef.PublicKeyOrToken);
      var publicKeyOrTokenString = publicKeyOrToken is null || publicKeyOrToken.Length == 0
        ? string.Empty
        : $", PublicKeyToken={string.Concat(publicKeyOrToken.Select(static b => b.ToString("x2", provider: null)))}";

      BaseWriter.WriteLine($"//     {name}, Version={assmRef.Version}, Culture={culture}{publicKeyOrTokenString}");
    }
  }

  private void WriteEmbeddedResources()
  {
    var manifestResourceNames = assembly.GetManifestResourceNames();

    if (manifestResourceNames.Length <= 0)
      return;

    BaseWriter.WriteLine("//   Embedded resources:");

    foreach (var name in manifestResourceNames) {
      var info = assembly.GetManifestResourceInfo(name);

      if (info is not null && info.ResourceLocation.HasFlag(ResourceLocation.Embedded)) {
        using var stream = assembly.GetManifestResourceStream(name);

        var length = stream?.Length ?? 0L;

        BaseWriter.WriteLine($"//     {name} ({length:N0} bytes, {info.ResourceLocation})");
      }
    }
  }

  public void WriteFooter()
  {
    if (!options.Writer.WriteFooter)
      return;

    var callingAssemblyName = Assembly.GetCallingAssembly().GetName();

    BaseWriter.WriteLine($"// API list generated by {callingAssemblyName?.Name} v{callingAssemblyName?.Version}.");

    var executingAssembly = Assembly.GetExecutingAssembly();
    var executingAssemblyName = executingAssembly?.GetName();
    var repositoryUrl = executingAssembly
      ?.GetCustomAttributes<AssemblyMetadataAttribute>()
      ?.First(static a => a.Key.StartsWith("RepositoryUrl", StringComparison.Ordinal))
      ?.Value;

    BaseWriter.WriteLine($"// {executingAssemblyName?.Name} v{executingAssemblyName?.Version} ({repositoryUrl})");
  }

  public void WriteExportedTypes()
  {
    IReadOnlyList<Type> types;

    try {
      types = assembly.GetExportedTypes();
    }
    catch (FileNotFoundException ex) { // when (!string.IsNullOrEmpty(ex.FusionLog))
      // in the case of reference assembly cannot be loaded
      throw AssemblyFileNotFoundException.Create(assembly.GetName(), ex);
    }

#if SYSTEM_ASSEMBLY_GETFORWARDEDTYPES
    try {
      types = types.Union(assembly.GetForwardedTypes()).ToList();
    }
    catch (FileNotFoundException ex) { // when (!string.IsNullOrEmpty(ex.FusionLog))
      // in the case of reference assembly cannot be loaded
      throw AssemblyFileNotFoundException.Create(assembly.GetName(), ex);
    }
#endif

    var typeDeclarations = new StringBuilder(10240);
    var referencingNamespaces = new HashSet<string>(StringComparer.Ordinal);

    foreach (var ns in types.Select(static t => t.Namespace).Distinct().OrderBy(static n => n, StringComparer.Ordinal)) {
      if (0 < typeDeclarations.Length)
        typeDeclarations.AppendLine();

      var noNamespace = string.IsNullOrEmpty(ns);

      if (!noNamespace) {
        typeDeclarations
          .Append("namespace ")
          .Append(ns)
          .Append(" {")
          .AppendLine();
      }

      typeDeclarations.Append(
        GenerateTypeAndMemberDeclarations(
          nestLevel: noNamespace ? 0 : 1,
          assembly,
          types
            .Where(type => string.Equals(type.Namespace, ns, StringComparison.Ordinal))
            .Where(static type => !type.IsNested),
          referencingNamespaces,
          options,
          logger
        )
      );

      if (!noNamespace)
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

    var hasUsingDirectivesWritten = false;

    foreach (var ns in orderedReferencingNamespaces) {
      BaseWriter.WriteLine($"using {ns};");

      hasUsingDirectivesWritten = true;
    }

    if (hasUsingDirectivesWritten)
      BaseWriter.WriteLine();

    BaseWriter.Write(typeDeclarations);
  }

  private static string GenerateTypeAndMemberDeclarations(
    int nestLevel,
    Assembly assm,
    IEnumerable<Type> types,
    ISet<string> referencingNamespaces,
    ApiListWriterOptions options,
    ILogger? logger
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
            enableNullableAnnotationsOnlyOnMembers,
            logger
          )
        );
      }
      catch (MemberDeclarationException) {
        // just rethrow since the log has been output already
        throw;
      }
      catch (Exception ex) {
        if (logger is not null)
          LoggerMessageGeneratorErrorOnType(logger, type.FullName, ex);

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
    bool enableNullableAnnotationsOnlyOnMembers,
    ILogger? logger
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
          options,
          logger
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

#pragma warning disable CA1032
  private sealed class MemberDeclarationException : Exception {
    public MemberDeclarationException(string? message, Exception? innerException)
      : base(message, innerException)
    { }
  }
#pragma warning restore CA1032

  private static string GenerateTypeContentDeclarations(
    int nestLevel,
    Assembly assm,
    Type t,
    ISet<string> referencingNamespaces,
    ApiListWriterOptions options,
    ILogger? logger
  )
  {
    if (options == null)
      throw new ArgumentNullException(nameof(options));

    const BindingFlags MembersBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    var members = t.GetMembers(MembersBindingFlags).OrderBy(static f => f.Name, StringComparer.Ordinal).ToList();
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
          options,
          logger
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
        if (logger is not null)
          LoggerMessageGeneratorErrorOnMember(logger, t.FullName, member.Name, ex);

        throw new MemberDeclarationException($"generator error on member '{t.FullName}.{member.Name}'", ex);
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
