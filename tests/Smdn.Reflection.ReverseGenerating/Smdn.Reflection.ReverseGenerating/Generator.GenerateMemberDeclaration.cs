// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CS8597

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class MemberDeclarationTestCaseAttribute : GeneratorTestCaseAttribute {
  public MemberDeclarationTestCaseAttribute(
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
  }
}

partial class GeneratorTests {
  [Test]
  public void GenerateMemberDeclaration_ArgumentOptionsNull()
    => Assert.Throws<ArgumentNullException>(() => Generator.GenerateMemberDeclaration(member: typeof(int).GetMembers().First(), null, options: null!));

  [Test]
  public void GenerateMemberDeclaration_ArgumentTypeNull()
    => Assert.Throws<ArgumentNullException>(() => Generator.GenerateMemberDeclaration(member: null!, null, options: new()));

  private static System.Collections.IEnumerable YieldTestCases_GenerateMemberDeclaration()
    => GetTestCaseTypes()
      .SelectMany(static t => t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
      .Where(static m => m is not Type) // except nested type
      .SelectMany(
        static m => m.GetCustomAttributes<MemberDeclarationTestCaseAttribute>().Select(
          a => new object[] { a, m }
        )
      );

  [TestCaseSource(nameof(YieldTestCases_GenerateMemberDeclaration))]
  public void GenerateMemberDeclaration(
    MemberDeclarationTestCaseAttribute attrTestCase,
    MemberInfo member
  )
  {
    member.GetCustomAttribute<SkipTestCaseAttribute>()?.Throw();

    var options = GetGeneratorOptions(attrTestCase);

    options.AttributeDeclaration.TypeFilter ??= static (_, _) => false;

    Assert.AreEqual(
      attrTestCase.Expected,
      Generator.GenerateMemberDeclaration(member, null, options),
      message: $"{attrTestCase.SourceLocation} ({member.DeclaringType!.FullName}.{member.Name})"
    );
  }

  private static System.Collections.IEnumerable YieldTestCases_GenerateMemberDeclaration_ReferencingNamespaces()
    => GetTestCaseTypes()
      .SelectMany(static t => t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
      .SelectMany(
        m => m.GetCustomAttributes<ReferencingNamespacesTestCaseAttribute>().Select(
          a => new object[] { a, m }
        )
      );

  [TestCaseSource(nameof(YieldTestCases_GenerateMemberDeclaration_ReferencingNamespaces))]
  public void GenerateMemberDeclaration_ReferencingNamespaces(
    ReferencingNamespacesTestCaseAttribute attrTestCase,
    MemberInfo member
  )
  {
    member.GetCustomAttribute<SkipTestCaseAttribute>()?.Throw();

    var namespaces = new HashSet<string>();

    Generator.GenerateMemberDeclaration(member, namespaces, GetGeneratorOptions(attrTestCase));

    Assert.That(
      namespaces,
      Is.EquivalentTo(attrTestCase.GetExpectedSet()),
      message: $"{attrTestCase.SourceLocation} ({member.DeclaringType?.FullName}.{member.Name})"
    );
  }

#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
#nullable enable annotations
#pragma warning disable CS0649
  private class TestClassForGenerateMemberDeclarationConcurrently {
    public Tuple<string?>? F1;
    public Tuple<string?, string?>? F2;
    public Tuple<string?, string?, string?>? F3;
    public Tuple<string?, string?, string?, string?>? F4;
    public Tuple<string?, string?, string?, string?, string?>? F5;
    public Tuple<string?, string?, string?, string?, string?, string?>? F6;
    public Tuple<string?, string?, string?, string?, string?, string?, string?>? F7;
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?>>? F8;
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?>>? F9;
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?>>? F10;
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?>>? F11;
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>? F12;

    public Tuple<string?>? P1 { get; set; }
    public Tuple<string?, string?>? P2 { get; set; }
    public Tuple<string?, string?, string?>? P3 { get; set; }
    public Tuple<string?, string?, string?, string?>? P4 { get; set; }
    public Tuple<string?, string?, string?, string?, string?>? P5 { get; set; }
    public Tuple<string?, string?, string?, string?, string?, string?>? P6 { get; set; }
    public Tuple<string?, string?, string?, string?, string?, string?, string?>? P7 { get; set; }
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?>>? P8 { get; set; }
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?>>? P9 { get; set; }
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?>>? P10 { get; set; }
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?>>? P11 { get; set; }
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>? P12 { get; set; }

    public Tuple<string?>? MRP1() => throw new NotImplementedException();
    public Tuple<string?, string?>? MRP2() => throw new NotImplementedException();
    public Tuple<string?, string?, string?>? MRP3() => throw new NotImplementedException();
    public Tuple<string?, string?, string?, string?>? MRP4() => throw new NotImplementedException();
    public Tuple<string?, string?, string?, string?, string?>? MRP5() => throw new NotImplementedException();
    public Tuple<string?, string?, string?, string?, string?, string?>? MRP6() => throw new NotImplementedException();
    public Tuple<string?, string?, string?, string?, string?, string?, string?>? MRP7() => throw new NotImplementedException();
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?>>? MRP8() => throw new NotImplementedException();
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?>>? MRP9() => throw new NotImplementedException();
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?>>? MRP10() => throw new NotImplementedException();
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?>>? MRP11() => throw new NotImplementedException();
    public Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>? MRP12() => throw new NotImplementedException();

    public void MP1(Tuple<string?>? p) => throw new NotImplementedException();
    public void MP2(Tuple<string?, string?>? p) => throw new NotImplementedException();
    public void MP3(Tuple<string?, string?, string?>? p) => throw new NotImplementedException();
    public void MP4(Tuple<string?, string?, string?, string?>? p) => throw new NotImplementedException();
    public void MP5(Tuple<string?, string?, string?, string?, string?>? p) => throw new NotImplementedException();
    public void MP6(Tuple<string?, string?, string?, string?, string?, string?>? p) => throw new NotImplementedException();
    public void MP7(Tuple<string?, string?, string?, string?, string?, string?, string?>? p) => throw new NotImplementedException();
    public void MP8(Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?>>? p) => throw new NotImplementedException();
    public void MP9(Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?>>? p) => throw new NotImplementedException();
    public void MP10(Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?>>? p) => throw new NotImplementedException();
    public void MP11(Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?>>? p) => throw new NotImplementedException();
    public void MP12(Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>? p) => throw new NotImplementedException();

    public event EventHandler<Tuple<string?>?>? E1;
    public event EventHandler<Tuple<string?, string?>?>? E2;
    public event EventHandler<Tuple<string?, string?, string?>?>? E3;
    public event EventHandler<Tuple<string?, string?, string?, string?>?>? E4;
    public event EventHandler<Tuple<string?, string?, string?, string?, string?>?>? E5;
    public event EventHandler<Tuple<string?, string?, string?, string?, string?, string?>?>? E6;
    public event EventHandler<Tuple<string?, string?, string?, string?, string?, string?, string?>?>? E7;
    public event EventHandler<Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?>>?>? E8;
    public event EventHandler<Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?>>?>? E9;
    public event EventHandler<Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?>>?>? E10;
    public event EventHandler<Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?>>?>? E11;
    public event EventHandler<Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>?>? E12;
  }
#pragma warning restore CS0649

  private async Task GenerateMemberDeclaration_Concurrent_Async(
    bool lockCreatingNullabilityInfo,
    Action<GeneratorOptions> testAction
  )
  {
    const int maxNumberOfRepeat = 30;

    Action<NullabilityInfoContext, object?> wrappedTestAction = new(
      (ctx, lockObject) => {
        var options = new GeneratorOptions();

        options.MemberDeclaration.NullabilityInfoContext = ctx;
        options.MemberDeclaration.NullabilityInfoContextLockObject = lockObject;

        testAction(options);
      }
    );

    if (lockCreatingNullabilityInfo) {
      await CSharpFormatterTests.AssertNullabilityInfoContextConcurrentOperationWithLockMustNotThrowExceptionAsync(
        wrappedTestAction,
        maxNumberOfRepeat
      ).ConfigureAwait(false);
    }
    else {
      await CSharpFormatterTests.AssertNullabilityInfoContextConcurrentOperationWithoutLockMustThrowExceptionAsync(
        wrappedTestAction,
        maxNumberOfRepeat
      ).ConfigureAwait(false);
    }
  }

  [TestCase(true)]
  [TestCase(false)]
  public Task GenerateMemberDeclaration_Concurrent_FieldInfo(bool lockCreatingNullabilityInfo)
  {
    var t = typeof(TestClassForGenerateMemberDeclarationConcurrently);
    var fields = new[] {
      t.GetField(nameof(TestClassForGenerateMemberDeclarationConcurrently.F1))!,
      t.GetField(nameof(TestClassForGenerateMemberDeclarationConcurrently.F2))!,
      t.GetField(nameof(TestClassForGenerateMemberDeclarationConcurrently.F3))!,
      t.GetField(nameof(TestClassForGenerateMemberDeclarationConcurrently.F4))!,
      t.GetField(nameof(TestClassForGenerateMemberDeclarationConcurrently.F5))!,
      t.GetField(nameof(TestClassForGenerateMemberDeclarationConcurrently.F6))!,
      t.GetField(nameof(TestClassForGenerateMemberDeclarationConcurrently.F7))!,
      t.GetField(nameof(TestClassForGenerateMemberDeclarationConcurrently.F8))!,
      t.GetField(nameof(TestClassForGenerateMemberDeclarationConcurrently.F9))!,
      t.GetField(nameof(TestClassForGenerateMemberDeclarationConcurrently.F10))!,
      t.GetField(nameof(TestClassForGenerateMemberDeclarationConcurrently.F11))!,
      t.GetField(nameof(TestClassForGenerateMemberDeclarationConcurrently.F12))!,
    };

    return GenerateMemberDeclaration_Concurrent_Async(
      lockCreatingNullabilityInfo,
      options => {
        foreach (var field in fields) {
          Generator.GenerateMemberDeclaration(field, null, options);
        }
      }
    );
  }

  [TestCase(true)]
  [TestCase(false)]
  public Task GenerateMemberDeclaration_Concurrent_PropertyInfo(bool lockCreatingNullabilityInfo)
  {
    var t = typeof(TestClassForGenerateMemberDeclarationConcurrently);
    var properties = new[] {
      t.GetProperty(nameof(TestClassForGenerateMemberDeclarationConcurrently.P1))!,
      t.GetProperty(nameof(TestClassForGenerateMemberDeclarationConcurrently.P2))!,
      t.GetProperty(nameof(TestClassForGenerateMemberDeclarationConcurrently.P3))!,
      t.GetProperty(nameof(TestClassForGenerateMemberDeclarationConcurrently.P4))!,
      t.GetProperty(nameof(TestClassForGenerateMemberDeclarationConcurrently.P5))!,
      t.GetProperty(nameof(TestClassForGenerateMemberDeclarationConcurrently.P6))!,
      t.GetProperty(nameof(TestClassForGenerateMemberDeclarationConcurrently.P7))!,
      t.GetProperty(nameof(TestClassForGenerateMemberDeclarationConcurrently.P8))!,
      t.GetProperty(nameof(TestClassForGenerateMemberDeclarationConcurrently.P9))!,
      t.GetProperty(nameof(TestClassForGenerateMemberDeclarationConcurrently.P10))!,
      t.GetProperty(nameof(TestClassForGenerateMemberDeclarationConcurrently.P11))!,
      t.GetProperty(nameof(TestClassForGenerateMemberDeclarationConcurrently.P12))!,
    };

    return GenerateMemberDeclaration_Concurrent_Async(
      lockCreatingNullabilityInfo,
      options => {
        foreach (var property in properties) {
          Generator.GenerateMemberDeclaration(property, null, options);
        }
      }
    );
  }

  [TestCase(true)]
  [TestCase(false)]
  public Task GenerateMemberDeclaration_Concurrent_MethodInfo_Parameter(bool lockCreatingNullabilityInfo)
  {
    var t = typeof(TestClassForGenerateMemberDeclarationConcurrently);
    var methods = new[] {
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MP1))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MP2))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MP3))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MP4))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MP5))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MP6))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MP7))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MP8))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MP9))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MP10))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MP11))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MP12))!,
    };

    return GenerateMemberDeclaration_Concurrent_Async(
      lockCreatingNullabilityInfo,
      options => {
        foreach (var method in methods) {
          Generator.GenerateMemberDeclaration(method, null, options);
        }
      }
    );
  }

  [TestCase(true)]
  [TestCase(false)]
  public Task GenerateMemberDeclaration_Concurrent_MethodInfo_ReturnParameter(bool lockCreatingNullabilityInfo)
  {
    var t = typeof(TestClassForGenerateMemberDeclarationConcurrently);
    var methods = new[] {
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MRP1))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MRP2))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MRP3))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MRP4))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MRP5))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MRP6))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MRP7))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MRP8))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MRP9))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MRP10))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MRP11))!,
      t.GetMethod(nameof(TestClassForGenerateMemberDeclarationConcurrently.MRP12))!,
    };

    return GenerateMemberDeclaration_Concurrent_Async(
      lockCreatingNullabilityInfo,
      options => {
        foreach (var method in methods) {
          Generator.GenerateMemberDeclaration(method, null, options);
        }
      }
    );
  }

  [TestCase(true)]
  [TestCase(false)]
  public Task GenerateMemberDeclaration_Concurrent_EventInfo(bool lockCreatingNullabilityInfo)
  {
    var t = typeof(TestClassForGenerateMemberDeclarationConcurrently);
    var events = new[] {
      t.GetEvent(nameof(TestClassForGenerateMemberDeclarationConcurrently.E1))!,
      t.GetEvent(nameof(TestClassForGenerateMemberDeclarationConcurrently.E2))!,
      t.GetEvent(nameof(TestClassForGenerateMemberDeclarationConcurrently.E3))!,
      t.GetEvent(nameof(TestClassForGenerateMemberDeclarationConcurrently.E4))!,
      t.GetEvent(nameof(TestClassForGenerateMemberDeclarationConcurrently.E5))!,
      t.GetEvent(nameof(TestClassForGenerateMemberDeclarationConcurrently.E6))!,
      t.GetEvent(nameof(TestClassForGenerateMemberDeclarationConcurrently.E7))!,
      t.GetEvent(nameof(TestClassForGenerateMemberDeclarationConcurrently.E8))!,
      t.GetEvent(nameof(TestClassForGenerateMemberDeclarationConcurrently.E9))!,
      t.GetEvent(nameof(TestClassForGenerateMemberDeclarationConcurrently.E10))!,
      t.GetEvent(nameof(TestClassForGenerateMemberDeclarationConcurrently.E11))!,
      t.GetEvent(nameof(TestClassForGenerateMemberDeclarationConcurrently.E12))!,
    };

    return GenerateMemberDeclaration_Concurrent_Async(
      lockCreatingNullabilityInfo,
      options => {
        foreach (var @event in events) {
          Generator.GenerateMemberDeclaration(@event, null, options);
        }
      }
    );
  }
#nullable restore annotations
#endif
}
