// SPDX-FileCopyrightText: 2023 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT

#nullable enable annotations

using System;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating;

public partial class CSharpFormatterTests {
  private class TestClassForFormatParameter {
    public void M(
      Tuple<string?>? p1,
      Tuple<string?, string?>? p2,
      Tuple<string?, string?, string?>? p3,
      Tuple<string?, string?, string?, string?>? p4,
      Tuple<string?, string?, string?, string?, string?>? p5,
      Tuple<string?, string?, string?, string?, string?, string?>? p6,
      Tuple<string?, string?, string?, string?, string?, string?, string?>? p7,
      Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?>>? p8,
      Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?>>? p9,
      Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?>>? p10,
      Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?>>? p11,
      Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>? p12,
      Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?, string?>>? p13,
      Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?, string?, string?>>? p14
    ) => throw new NotImplementedException();
  }

  [TestCase(true)]
  [TestCase(false)]
  public async Task FormatParameter_Concurrent(bool lockCreatingNullabilityInfo)
  {
    var parameters = typeof(TestClassForFormatParameter).GetMethod(nameof(TestClassForFormatParameter.M))!.GetParameters();

    Action<NullabilityInfoContext, object?> testAction = new(
      (ctx, lockObject) => {
        foreach (var parameter in parameters) {
          CSharpFormatter.FormatParameter(parameter, ctx, lockObject);
        }
      }
    );

    const int maxNumberOfRepeat = 15;

    if (lockCreatingNullabilityInfo) {
      await AssertNullabilityInfoContextConcurrentOperationWithLockMustNotThrowExceptionAsync(
        testAction,
        maxNumberOfRepeat
      ).ConfigureAwait(false);
    }
    else {
      await AssertNullabilityInfoContextConcurrentOperationWithoutLockMustThrowExceptionAsync(
        testAction,
        maxNumberOfRepeat
      ).ConfigureAwait(false);
    }
  }

  [TestCase(true)]
  [TestCase(false)]
  public async Task FormatParameterList_OfMethodBase_Concurrent(bool lockCreatingNullabilityInfo)
  {
    var method = typeof(TestClassForFormatParameter).GetMethod(nameof(TestClassForFormatParameter.M))!;

    Action<NullabilityInfoContext, object?> testAction = new(
      (ctx, lockObject) => CSharpFormatter.FormatParameterList(method, ctx, lockObject)
    );

    const int maxNumberOfRepeat = 15;

    if (lockCreatingNullabilityInfo) {
      await AssertNullabilityInfoContextConcurrentOperationWithLockMustNotThrowExceptionAsync(
        testAction,
        maxNumberOfRepeat
      ).ConfigureAwait(false);
    }
    else {
      await AssertNullabilityInfoContextConcurrentOperationWithoutLockMustThrowExceptionAsync(
        testAction,
        maxNumberOfRepeat
      ).ConfigureAwait(false);
    }
  }

  [TestCase(true)]
  [TestCase(false)]
  public async Task FormatParameterList_OfArrayOfParameterInfo_Concurrent(bool lockCreatingNullabilityInfo)
  {
    var parameters = typeof(TestClassForFormatParameter).GetMethod(nameof(TestClassForFormatParameter.M))!.GetParameters();

    Action<NullabilityInfoContext, object?> testAction = new(
      (ctx, lockObject) => CSharpFormatter.FormatParameterList(parameters, ctx, lockObject)
    );

    const int maxNumberOfRepeat = 15;

    if (lockCreatingNullabilityInfo) {
      await AssertNullabilityInfoContextConcurrentOperationWithLockMustNotThrowExceptionAsync(
        testAction,
        maxNumberOfRepeat
      ).ConfigureAwait(false);
    }
    else {
      await AssertNullabilityInfoContextConcurrentOperationWithoutLockMustThrowExceptionAsync(
        testAction,
        maxNumberOfRepeat
      ).ConfigureAwait(false);
    }
  }
}
#endif
