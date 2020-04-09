using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Smdn.Reflection;
using Smdn.Reflection.ReverseGenerating;

public class Test {
  class TestResult {
    public int Success = 0;
    public int Failed = 0;
    public int Exception = 0;
  }

  public static void RunTests()
  {
    var result = new TestResult();

    Console.WriteLine("******** generate declarations ********");

    RunTestGenerateDeclarations(result);

    Console.WriteLine("******** test result ********");

    var initialConsoleColor = Console.ForegroundColor;

    try {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.Write("Success: ");
      Console.ForegroundColor = initialConsoleColor;
      Console.WriteLine(result.Success);

      Console.ForegroundColor = ConsoleColor.Red;
      Console.Write("Failed: ");
      Console.ForegroundColor = initialConsoleColor;
      Console.WriteLine(result.Failed);

      Console.ForegroundColor = ConsoleColor.Magenta;
      Console.Write("Exception: ");
      Console.ForegroundColor = initialConsoleColor;
      Console.WriteLine(result.Exception);
    }
    finally {
      Console.ForegroundColor = initialConsoleColor;
    }

    Console.WriteLine("done");
  }

  static void RunTestGenerateDeclarations(TestResult result)
  {
    var types = Assembly.GetExecutingAssembly().GetTypes();
    var options = new Options() {
      Indent = string.Empty,
      IgnorePrivateOrAssembly = false,
      TypeDeclarationWithNamespace = true,
      MemberDeclarationWithNamespace = true,
      MemberDeclarationUseDefaultLiteral = false,
    };

    foreach (var type in types.Where(t => t.FullName.StartsWith("TestCases", StringComparison.Ordinal))) {
      /* test type */
      Console.WriteLine(type);

      var testOfType = type.GetCustomAttribute<TestCases.TestAttribute>();

      if (testOfType != null) {
        if (testOfType.Expected != null)
          EvaluateTypeTest(result, options, testOfType, type);

        var testOfBaseType = type.GetCustomAttribute<TestCases.BaseTypeTestAttribute>();

        if (testOfBaseType != null && testOfBaseType.Expected != null)
          EvaluateBaseTypeTest(result, options, testOfType, testOfBaseType, type);

        var testOfNamespaces = type.GetCustomAttribute<TestCases.NamespacesTestAttribute>();

        if (testOfNamespaces != null && testOfNamespaces.Expected != null)
          EvaluateNamespacesTest(result, options, testOfType, testOfNamespaces, type);

        // TODO: AttributeTargets.GenericParameter
        EvaluateAttributeTest(result, options, testOfType, type);
      }

      /* test members */
      const BindingFlags memberBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

      foreach (var member in type.GetMembers(memberBindingFlags)) {
        if (member is Type)
          continue; // ignore nested types

        var testOfMember = member.GetCustomAttribute<TestCases.TestAttribute>();

        if (testOfMember != null) {
          if (testOfMember.Expected != null)
            EvaluateMemberTest(result, options, testOfMember, member);

          var testOfNamespaces = member.GetCustomAttribute<TestCases.NamespacesTestAttribute>();

          if (testOfNamespaces != null && testOfNamespaces.Expected != null)
            EvaluateNamespacesTest(result, options, testOfMember, testOfNamespaces, member);

          EvaluateAttributeTest(result, options, testOfMember, member);

          if (member is MethodInfo method) {
            // TODO: AttributeTargets.ReturnValue, AttributeTargets.Parameter
            EvaluateAttributeTest(result, options, testOfMember, method.ReturnTypeCustomAttributes);
          }
        }
      }
    }
  }

  static void EvaluateTypeTest(TestResult result, Options options, TestCases.TestAttribute test, Type type)
  {
    var opts = options.Clone();

    opts.TypeDeclarationWithNamespace = test.WithNamespace;

    EvaluateTest(result,
                 type.ToString(),
                 test.Expected,
                 () => string.Join("\n", Generator.GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(type, null, opts)));
  }

  static void EvaluateMemberTest(TestResult result, Options options, TestCases.TestAttribute test, MemberInfo member)
  {
    var opts = options.Clone();

    opts.MemberDeclarationWithNamespace = test.WithNamespace;
    opts.MemberDeclarationUseDefaultLiteral = test.UseDefaultLiteral;

    EvaluateTest(result,
                 member.ToString(),
                 test.Expected,
                 () => Generator.GenerateMemberDeclaration(member, null, opts));
  }

  static void EvaluateBaseTypeTest(TestResult result, Options options, TestCases.TestAttribute testOfType, TestCases.BaseTypeTestAttribute testOfBaseType, Type type)
  {
    var opts = options.Clone();

    opts.TypeDeclarationWithNamespace = testOfType.WithNamespace;

    EvaluateTest(result,
                 "base types of " + type.ToString(),
                 testOfBaseType.Expected,
                 () => string.Join(", ", Generator.GenerateExplicitBaseTypeAndInterfaces(type, null, opts)));
  }

  static void EvaluateAttributeTest(TestResult result, Options options, TestCases.TestAttribute testOfType, ICustomAttributeProvider attributeProvider)
  {
    var testOfAttribute = attributeProvider?.GetCustomAttributes(typeof(TestCases.AttributeTestAttribute), false)?.Cast<TestCases.AttributeTestAttribute>()?.FirstOrDefault();

    if (testOfAttribute?.Expected == null)
      return;

    var opts = options.Clone();

    opts.TypeDeclarationWithNamespace = testOfType.WithNamespace;

    EvaluateTest(result,
                 "attributes of " + attributeProvider.ToString(),
                 testOfAttribute.Expected,
                 () => string.Join(", ", Generator.GenerateAttributeList(attributeProvider, null, opts)));
  }

  static void EvaluateNamespacesTest(TestResult result, Options options, TestCases.TestAttribute testOfTypeOrMember, TestCases.NamespacesTestAttribute testOfNamespaces, MemberInfo memberOrType)
  {
    var opts = options.Clone();

    opts.TypeDeclarationWithNamespace = testOfTypeOrMember.WithNamespace;
    opts.MemberDeclarationWithNamespace = testOfTypeOrMember.WithNamespace;
    opts.MemberDeclarationUseDefaultLiteral = testOfTypeOrMember.UseDefaultLiteral;

    string GenerateNamespaceList()
    {
      var namespaces = new SortedSet<string>(StringComparer.Ordinal);

      if (memberOrType is Type t)
        ListApi.GenerateTypeAndMemberDeclarations(0, t, namespaces, opts);
      else
        Generator.GenerateMemberDeclaration(memberOrType, namespaces, opts);

      return string.Join(", ", namespaces);
    }

    EvaluateTest(result,
                 "namespaces of " + memberOrType.ToString(),
                 testOfNamespaces.Expected,
                 GenerateNamespaceList);
  }

  static void EvaluateTest(TestResult result, string identifier, string expected, Func<string> getActual)
  {
    var initialConsoleColor = Console.ForegroundColor;

    try {
      Console.Write("  {0,-80} : ", identifier);

      try {
        var actual = getActual();

        if (string.Equals(expected, actual, StringComparison.Ordinal)) {
          result.Success++;

          Console.ForegroundColor = ConsoleColor.Green;
          Console.Write("success! ");
          Console.ForegroundColor = initialConsoleColor;

          Console.WriteLine();
        }
        else {
          result.Failed++;

          Console.ForegroundColor = ConsoleColor.Red;
          Console.Write($"failed! ");
          Console.ForegroundColor = initialConsoleColor;

          Console.WriteLine();

          Console.WriteLine($"        expected: '{expected}'");
          Console.WriteLine($"        actual  : '{actual}'");
        }
      }
      catch (Exception ex) {
        result.Exception++;

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write($"{ex.GetType().Name}: {ex.Message}");
        Console.ForegroundColor = initialConsoleColor;

        Console.WriteLine();
      }
    }
    finally {
      Console.ForegroundColor = initialConsoleColor;
    }
  }
}
