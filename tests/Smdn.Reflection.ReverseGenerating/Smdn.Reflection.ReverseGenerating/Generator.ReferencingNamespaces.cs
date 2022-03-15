// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating {
  [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
  public class ReferencingNamespacesTestCaseAttribute : GeneratorTestCaseAttribute {
    public ReferencingNamespacesTestCaseAttribute(
      string expected,
      [CallerFilePath] string sourceFilePath = null,
      [CallerLineNumber] int lineNumber = 0
    )
      : base(
        expected,
        sourceFilePath,
        lineNumber
      )
    {
      base.MemberWithNamespace = true;
      base.UseDefaultLiteral = false;
    }

    public bool TypeDeclarationWithExplicitBaseTypeAndInterfaces { get; set; } = false;

    public IReadOnlyList<string> GetExpectedSet()
    {
#if SYSTEM_STRINGSPLITOPTIONS_TRIMENTRIES
      return Expected.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
#else
      return Expected.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
#endif
    }
  }

  namespace TestCases {
    namespace ReferencingNamespaces {
      namespace Types {
        [ReferencingNamespacesTestCase("")] delegate void D0();
        [ReferencingNamespacesTestCase("")] delegate void D1(int x);
        [ReferencingNamespacesTestCase("")] delegate void D2(int? x);
        [ReferencingNamespacesTestCase("")] delegate void D3(int[] x);
        [ReferencingNamespacesTestCase("")] unsafe delegate void D4(int* x);
        [ReferencingNamespacesTestCase("System")] delegate void D5(Guid x);
        [ReferencingNamespacesTestCase("System")] delegate void D6(Guid? x);
        [ReferencingNamespacesTestCase("System")] delegate void D7(Guid[] x);
        [ReferencingNamespacesTestCase("System.Collections.Generic")] delegate void D8(List<int> x);
        [ReferencingNamespacesTestCase("System, System.Collections.Generic")] delegate void D9(List<Guid> x);

        [ReferencingNamespacesTestCase("", TypeDeclarationWithExplicitBaseTypeAndInterfaces = false)]
        [ReferencingNamespacesTestCase("System.Collections.Generic", TypeDeclarationWithExplicitBaseTypeAndInterfaces = true, TranslateLanguagePrimitiveTypeDeclaration = true)]
        [ReferencingNamespacesTestCase("System.Collections.Generic", TypeDeclarationWithExplicitBaseTypeAndInterfaces = true, TranslateLanguagePrimitiveTypeDeclaration = false)]
        class ListEx1 : List<int> {}

        [ReferencingNamespacesTestCase("", TypeDeclarationWithExplicitBaseTypeAndInterfaces = false)]
        [ReferencingNamespacesTestCase("System, System.Collections.Generic", TypeDeclarationWithExplicitBaseTypeAndInterfaces = true)]
        class ListEx2 : List<Guid> {}

        [ReferencingNamespacesTestCase("", TypeDeclarationWithExplicitBaseTypeAndInterfaces = false)]
        [ReferencingNamespacesTestCase("System.Collections.Generic", TypeDeclarationWithExplicitBaseTypeAndInterfaces = true)]
        class ListEx3<T> : List<T> {}
      }

      namespace Members {
#pragma warning disable 0649, 0067
        class C {
          [ReferencingNamespacesTestCase("")] public int F1;
          [ReferencingNamespacesTestCase("")] public int? F2;
          [ReferencingNamespacesTestCase("System")] public Guid F3;
          [ReferencingNamespacesTestCase("System.Collections.Generic")] public List<int> F4;
          [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public List<Guid> F5;
          [ReferencingNamespacesTestCase("")] public int[] F6;
          [ReferencingNamespacesTestCase("")] public Nullable<int> F7;
          [ReferencingNamespacesTestCase("System")] public Action<int> F8;
          [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public List<Action<int>> F9;
          [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public Action<List<int>> F10;
          [ReferencingNamespacesTestCase("System, System.Collections.Generic, System.Collections.ObjectModel")] public System.Collections.ObjectModel.Collection<List<Action<int>>> F11;
          [ReferencingNamespacesTestCase("System, System.Collections.Generic, System.Collections.ObjectModel")] public Action<System.Collections.ObjectModel.Collection<List<int>>> F12;

          [ReferencingNamespacesTestCase("System")] public event EventHandler E1;
          [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public event EventHandler<IList<int>> E2;

          [ReferencingNamespacesTestCase("")] public int P1 { get; set; }
          [ReferencingNamespacesTestCase("")] public int? P2 { get; set; }
          [ReferencingNamespacesTestCase("System")] public Guid P3 { get; set; }
          [ReferencingNamespacesTestCase("System.Collections.Generic")] public IList<int> P4 { get; set; }
          [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public IList<Guid> P5 { get; set; }

          [ReferencingNamespacesTestCase("")] public void M0() {}
          [ReferencingNamespacesTestCase("")] public void M1(int x) {}
          [ReferencingNamespacesTestCase("")] public void M2(int? x) {}
          [ReferencingNamespacesTestCase("System")] public void M3(Guid x) {}
          [ReferencingNamespacesTestCase("System.Collections.Generic")] public void M4(List<int> x) {}
          [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public void M5(List<Guid> x) {}
          [ReferencingNamespacesTestCase("")] public unsafe void M6(int* x) {}

          [ReferencingNamespacesTestCase("")] public int M1() => throw new NotImplementedException();
          [ReferencingNamespacesTestCase("")] public int? M2() => throw new NotImplementedException();
          [ReferencingNamespacesTestCase("System")] public Guid M3() => throw new NotImplementedException();
          [ReferencingNamespacesTestCase("System.Collections.Generic")] public List<int> M4() => throw new NotImplementedException();
          [ReferencingNamespacesTestCase("System, System.Collections.Generic")] public List<Guid> M5() => throw new NotImplementedException();
        }
#pragma warning restore 0649, 0067
      }
    }
  }

  partial class GeneratorTests {
    private static System.Collections.IEnumerable YieldReferencingNamespacesTestCase(
      string namespaceOfTestCase,
      Func<MemberInfo, bool> predicate
    )
      => FindTypes(t => t.FullName.Contains(namespaceOfTestCase))
        .SelectMany(t => t
          .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
          .Prepend((MemberInfo)t) // prepend type itself as a test target
        )
        .Where(predicate)
        .SelectMany(
          m => m.GetCustomAttributes<ReferencingNamespacesTestCaseAttribute>().Select(
            a => new object[] { a, m }
          )
        );

    private static System.Collections.IEnumerable YieldReferencingNamespacesOfTypesTestCase()
      => YieldReferencingNamespacesTestCase(
        ".TestCases.ReferencingNamespaces.Types.",
        m => m is Type
      );

    [TestCaseSource(nameof(YieldReferencingNamespacesOfTypesTestCase))]
    public void TestReferencingNamespacesOfType(
      ReferencingNamespacesTestCaseAttribute attrTestCase,
      Type type
    )
    {
      var namespaces = new HashSet<string>();

      if (attrTestCase.TypeDeclarationWithExplicitBaseTypeAndInterfaces)
        Generator.GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(type, namespaces, GetGeneratorOptions(attrTestCase)).ToList();
      else
        Generator.GenerateTypeDeclaration(type, namespaces, GetGeneratorOptions(attrTestCase));

      Assert.That(
        namespaces,
        Is.EquivalentTo(attrTestCase.GetExpectedSet()),
        message: $"{attrTestCase.SourceLocation} ({type.FullName})"
      );
    }

    private static System.Collections.IEnumerable YieldReferencingNamespacesOfMembersTestCase()
      => YieldReferencingNamespacesTestCase(
        ".TestCases.ReferencingNamespaces.Members.",
        m => m is not Type
      );

    [TestCaseSource(nameof(YieldReferencingNamespacesOfMembersTestCase))]
    public void TestReferencingNamespacesOfMembers(
      ReferencingNamespacesTestCaseAttribute attrTestCase,
      MemberInfo member
    )
    {
      var namespaces = new HashSet<string>();

      Generator.GenerateMemberDeclaration(member, namespaces, GetGeneratorOptions(attrTestCase));

      Assert.That(
        namespaces,
        Is.EquivalentTo(attrTestCase.GetExpectedSet()),
        message: $"{attrTestCase.SourceLocation} ({member.DeclaringType?.FullName}.{member.Name})"
      );
    }
  }
}
