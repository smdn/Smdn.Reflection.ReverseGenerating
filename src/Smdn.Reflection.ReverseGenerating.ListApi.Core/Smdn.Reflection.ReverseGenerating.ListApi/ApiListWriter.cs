// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

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

    foreach (var (key, value) in assembly.GetAssemblyMetadataAttributes<AssemblyMetadataAttribute, (object?, object?)>(
      static constructorArguments => (
        constructorArguments.FirstOrDefault().Value,
        constructorArguments.Skip(1).FirstOrDefault().Value
      )
    )) {
      BaseWriter.WriteLine($"//   Metadata: {key}={value}");
    }
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
      .OrderBy(static referencedAssembly => referencedAssembly.Name, StringComparer.Ordinal);

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
        assemblyReference => reader.GetString(assemblyReference.Name),
        assemblyReference => assemblyReference
      );

    foreach (var (name, assemblyReference) in assemblyReferences.OrderBy(static pair => pair.Key, StringComparer.Ordinal)) {
      var culture = reader.GetString(assemblyReference.Culture);

      if (string.IsNullOrEmpty(culture))
        culture = "neutral";

      var publicKeyOrToken = reader.GetBlobBytes(assemblyReference.PublicKeyOrToken);
      var publicKeyOrTokenString = publicKeyOrToken is null || publicKeyOrToken.Length == 0
        ? string.Empty
        : $", PublicKeyToken={string.Concat(publicKeyOrToken.Select(static b => b.ToString("x2", provider: null)))}";

      BaseWriter.WriteLine($"//     {name}, Version={assemblyReference.Version}, Culture={culture}{publicKeyOrTokenString}");
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
    catch (ReflectionTypeLoadException ex) {
      // in the case of the forwarded type could not load or not found in assembly
      if (options.Writer.ThrowIfForwardedTypesCouldNotLoaded)
        throw;

      foreach (var exLoader in ex.LoaderExceptions) {
        if (exLoader is not null && !string.IsNullOrEmpty(exLoader.Message))
          logger?.LogWarning("LoaderException: {ExceptionMessage}", exLoader.Message);
      }

      logger?.LogWarning("ReflectionTypeLoadException: Could not load one or more forwarded types. If you are trying to load an assembly with an SDK version that is not currently installed, install that version of the SDK, or specify the 'DOTNET_ROLL_FORWARD' environment variable and try again.");

      // append successfully loaded types
      if (ex.Types is not null) {
        types = types
          .Union(ex.Types.Where(static t => t is not null).Select(static t => t!))
          .ToList();
      }
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
    Assembly assembly,
    IEnumerable<Type> types,
    ISet<string> referencingNamespaces,
    ApiListWriterOptions options,
    ILogger? logger
  )
  {
    var ret = new StringBuilder(10240);
    var isPrevDelegate = false;

    static int OrderOfType(Type t, bool orderExtensionDeclarationsFirst)
    {
      if (t.IsDelegate())
        return 1;
      if (t.IsInterface)
        return 2;
      if (t.IsEnum)
        return 3;
      if (t.IsClass)
        return (orderExtensionDeclarationsFirst && t.IsExtensionGroupingType()) ? 4 : 5;
      if (t.IsValueType)
        return 6;

      return int.MaxValue;
    }

    static string? GetOrderKeyForExtensionGroupingType(Type extensionGroupingType, GeneratorOptions options)
      => extensionGroupingType
        .EnumerateExtensionMarkerTypeAndParameterPairs()
        .Where(
          static pair
            // exclude extension grouping type which has no extension members
            => pair.ExtensionParameter is not null
        )
        .Select(
          pair
            // select the declaration of first extension parameter as the sort key
            => Generator.GenerateParameterDeclaration(
              parameter: pair.ExtensionParameter!,
              options: options
            )
        )
        .FirstOrDefault();

    var orderExtensionDeclarationsFirst =
      options.Writer.ReconstructExtensionDeclarations &&
      options.Writer.OrderExtensionDeclarationsFirst;
    var orderedTypes = types
      .OrderBy(type => OrderOfType(type, orderExtensionDeclarationsFirst))
      .ThenBy(
        type => options.Writer.ReconstructExtensionDeclarations && type.IsExtensionGroupingType()
          ? GetOrderKeyForExtensionGroupingType(type, options)
          : type.FullName,
        StringComparer.Ordinal
      );

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
            assembly,
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

  private static string GenerateTypeAndMemberDeclarations(
    int nestLevel,
    Assembly assembly,
    Type t,
    ISet<string> referencingNamespaces,
    ApiListWriterOptions options,
    bool enableNullableAnnotationsOnlyOnTypes,
    bool enableNullableAnnotationsOnlyOnMembers,
    ILogger? logger
  )
  {
    var ret = new StringBuilder(1024);
    var indent = string.Concat(Enumerable.Repeat(options.Indent, nestLevel));

    if (options.Writer.ReconstructExtensionDeclarations && t.IsExtensionGroupingType()) {
      AppendExtensionDeclarations(
        builder: ret,
        indent: indent,
        nestLevel: nestLevel,
        assembly: assembly,
        extensionGroupingType: t,
        referencingNamespaces: referencingNamespaces,
        options: options,
        enableNullableAnnotationsOnlyOnTypes: enableNullableAnnotationsOnlyOnTypes,
        enableNullableAnnotationsOnlyOnMembers: enableNullableAnnotationsOnlyOnMembers,
        logger: logger
      );

      return ret.ToString();
    }

    AppendTypeForwardingInformation(ret, assembly, t, indent);

    foreach (var attr in Generator.GenerateAttributeList(t, null, options)) {
      ret.Append(indent).AppendLine(attr);
    }

    if (enableNullableAnnotationsOnlyOnTypes)
      ret.AppendLine("#nullable enable annotations");

    var typeDeclarationLines = Generator.GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(t, referencingNamespaces, options).ToList();

    for (var index = 0; index < typeDeclarationLines.Count; index++) {
      if (0 < index)
        ret.AppendLine();

      ret.Append(indent).Append(typeDeclarationLines[index]);
    }

    if (t.IsDelegate()) {
      ret.AppendLine();

      if (enableNullableAnnotationsOnlyOnTypes)
        ret.AppendLine("#nullable restore annotations");

      return ret.ToString();
    }

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
        assembly,
        t,
        referencingNamespaces,
        options,
        logger
      )
    );

    if (enableNullableAnnotationsOnlyOnMembers)
      ret.AppendLine("#nullable restore annotations");

    ret.Append(indent).AppendLine("}");

    return ret.ToString();
  }

  private static StringBuilder AppendTypeForwardingInformation(
    StringBuilder builder,
    Assembly assembly,
    Type t,
    string indent
  )
  {
    var ctorArgForTypeForwardedFrom = t
      .GetCustomAttributesData()
      .FirstOrDefault(static d => ROCType.FullNameEquals(typeof(TypeForwardedFromAttribute), d.AttributeType))
      ?.ConstructorArguments
      ?.FirstOrDefault()
      .Value;

    if (ctorArgForTypeForwardedFrom is not string assemblyNameOfTypeForwardedFrom)
      return builder;

    if (!string.Equals(assembly.FullName, assemblyNameOfTypeForwardedFrom, StringComparison.Ordinal))
      return builder;

    return builder
      .Append(indent)
      .Append("// Forwarded to \"")
      .Append(t.Assembly.FullName)
      .Append('"')
      .AppendLine();
  }

  private static void AppendExtensionDeclarations(
    StringBuilder builder,
    string indent,
    int nestLevel,
    Assembly assembly,
    Type extensionGroupingType,
    ISet<string> referencingNamespaces,
    ApiListWriterOptions options,
    bool enableNullableAnnotationsOnlyOnTypes,
    bool enableNullableAnnotationsOnlyOnMembers,
    ILogger? logger
  )
  {
    var orderedExtensionDeclarations = extensionGroupingType
      .EnumerateExtensionMarkerTypeAndParameterPairs()
      .Where(
        static pair
          // exclude extension grouping type which has no extension members
          => pair.ExtensionParameter is not null
      )
      .Select(
        pair => (
          ExtensionDeclaration: Generator.GenerateExtensionDeclaration(
            extensionMarkerType: pair.ExtensionMarkerType,
            extensionParameter: pair.ExtensionParameter!,
            referencingNamespaces: referencingNamespaces,
            options: options
          ),
          // select the declaration of extension parameter as the sort key
          ExtensionParameterDeclaration: Generator.GenerateParameterDeclaration(
            parameter: pair.ExtensionParameter!,
            options: options
          )
        )
      )
      .OrderBy(static pair => pair.ExtensionParameterDeclaration, StringComparer.Ordinal)
      .Select(static pair => pair.ExtensionDeclaration );
    var isFirstExtensionGroup = true;

    foreach (var extensionDeclaration in orderedExtensionDeclarations) {
      if (isFirstExtensionGroup)
        isFirstExtensionGroup = false;
      else
        builder.AppendLine();

      if (enableNullableAnnotationsOnlyOnTypes)
        builder.Append(indent).AppendLine("#nullable enable annotations");

      builder
        .Append(indent)
        .Append(extensionDeclaration)
        .AppendLine(" {");

      if (enableNullableAnnotationsOnlyOnTypes)
        builder.Append(indent).AppendLine("#nullable restore annotations");
      if (enableNullableAnnotationsOnlyOnMembers)
        builder.Append(indent).AppendLine("#nullable enable annotations");

      builder.Append(
        GenerateTypeContentDeclarations(
          nestLevel + 1,
          assembly,
          extensionGroupingType,
          referencingNamespaces,
          options,
          logger
        )
      );

      if (enableNullableAnnotationsOnlyOnMembers)
        builder.Append(indent).AppendLine("#nullable restore annotations");

      builder.Append(indent).AppendLine("}");
    }
  }

#pragma warning disable CA1032
  private sealed class MemberDeclarationException : Exception {
    public MemberDeclarationException(string? message, Exception? innerException)
      : base(message, innerException)
    { }
  }
#pragma warning restore CA1032

#pragma warning disable CA1502 // TODO: reduce complexity
  private static string GenerateTypeContentDeclarations(
    int nestLevel,
    Assembly assembly,
    Type t,
    ISet<string> referencingNamespaces,
    ApiListWriterOptions options,
    ILogger? logger
  )
  {
    const BindingFlags MembersBindingFlags =
      BindingFlags.Public |
      BindingFlags.NonPublic |
      BindingFlags.Instance |
      BindingFlags.Static |
      BindingFlags.DeclaredOnly;

    var isRecord = options.TypeDeclaration.EnableRecordTypes && t.IsRecord();
    var members = t.GetMembers(MembersBindingFlags).OrderBy(static m => m.Name, StringComparer.Ordinal).ToList();
    var membersToBeExcluded = new List<MemberInfo>();
    var nestedTypes = new List<Type>();
    IReadOnlyList<MemberInfo> extensionImplMethods = [];

    if (options.Writer.ReconstructExtensionDeclarations && t.IsExtensionEnclosingClass()) {
      extensionImplMethods = t
        .EnumerateExtensionMemberAndImplementationPairs()
        .Select(static pair => pair.ImplementationMethod)
        .ToList();
    }

    foreach (var member in members) {
      if (member is PropertyInfo p) {
        // exclude get/set accessor of properties
        membersToBeExcluded.AddRange(p.GetAccessors(true));
      }
      else if (member is EventInfo e) {
        // exclude add/remove/raise method of events
        membersToBeExcluded.AddRange(e.GetMethods(true));
      }
      else if (member.MemberType == MemberTypes.NestedType && member is Type nestedType) {
        // process nested types recursively
        membersToBeExcluded.Add(nestedType);

        if (options.IgnorePrivateOrAssembly && (nestedType.IsNestedPrivate || nestedType.IsNestedAssembly))
          continue; // exclude private or assembly nested types
        if (options.Writer.ExcludeFixedBufferFieldTypes && nestedType.IsFixedBufferFieldType())
          continue; // exclude nested types of fixed buffer fields
        if (options.Writer.ReconstructExtensionDeclarations && nestedType.IsExtensionMarkerType())
          continue; // exclude extension marker types

        nestedTypes.Add(nestedType);
      }
      else if (member is MethodInfo m) {
        if (
          options.Writer.OmitCompilerGeneratedRecordEqualityMethods &&
          isRecord &&
          m.IsCompilerGeneratedRecordEqualityMethod()
        ) {
          // remove compiler-generated record equality methods
          membersToBeExcluded.Add(m);
        }
        else if (
          options.Writer.ReconstructExtensionDeclarations &&
          extensionImplMethods.Contains(m)
        ) {
          // exclude implementation methods corresponding to the extension members
          membersToBeExcluded.Add(m);
        }
      }
    }

    var ret = new StringBuilder(1024);
    var generatedNestedTypeDeclarations = false;

    if (0 < nestedTypes.Count) {
      ret.Append(
        GenerateTypeAndMemberDeclarations(
          nestLevel,
          assembly,
          nestedTypes,
          referencingNamespaces,
          options,
          logger
        )
      );

      if (0 < ret.Length)
        generatedNestedTypeDeclarations = true;
    }

    var indent = string.Concat(Enumerable.Repeat(options.Indent, nestLevel));
    var memberAndDeclarationPairs = new List<(MemberInfo Member, string Declaration)>();

    foreach (var member in members.Except(membersToBeExcluded)) {
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

      memberAndDeclarationPairs.Add((member, declaration));
    }

    var memberComparer = options.Writer.OrderStaticMembersFirst
      ? MemberInfoComparer.StaticMembersFirst
      : MemberInfoComparer.Default;
    var orderedMemberAndDeclarations = memberAndDeclarationPairs
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
#pragma warning restore CA1502 // TODO: reduce complexity
}
