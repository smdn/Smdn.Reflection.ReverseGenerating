// SPDX-FileCopyrightText: 2023 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT

#nullable enable annotations

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating;

public partial class CSharpFormatterTests {
#pragma warning disable CS0649
  private class C {
    public string? F0;
    public string? F1;
    public string? F2;
    public string? F3;
    public string? F4;
    public string? F5;
    public string? F6;
    public string? F7;
    public string? F8;
    public string? F9;

    public string? P0 { get; set; }
    public string? P1 { get; set; }
    public string? P2 { get; set; }
    public string? P3 { get; set; }
    public string? P4 { get; set; }
    public string? P5 { get; set; }
    public string? P6 { get; set; }
    public string? P7 { get; set; }
    public string? P8 { get; set; }
    public string? P9 { get; set; }

    public event EventHandler? E0;
    public event EventHandler? E1;
    public event EventHandler? E2;
    public event EventHandler? E3;
    public event EventHandler? E4;
    public event EventHandler? E5;
    public event EventHandler? E6;
    public event EventHandler? E7;
    public event EventHandler? E8;
    public event EventHandler? E9;

    public void M(
      string? p0,
      string? p1,
      string? p2,
      string? p3,
      string? p4,
      string? p5,
      string? p6,
      string? p7,
      string? p8,
      string? p9
    ) => throw new NotImplementedException();
  }
#pragma warning restore CS0649

  [TestCase(true)]
  [TestCase(false)]
  public Task FormatTypeName_OfFieldInfo_Concurrent(bool lockCreatingNullabilityInfo)
    => FormatTypeName_Concurrent(
      lockCreatingNullabilityInfo,
      typeof(C),
      typeof(C).GetField(nameof(C.F0)) ?? throw new InvalidOperationException($"field not found: {nameof(C.F0)}"),
      typeof(C).GetField(nameof(C.F1)) ?? throw new InvalidOperationException($"field not found: {nameof(C.F1)}"),
      typeof(C).GetField(nameof(C.F2)) ?? throw new InvalidOperationException($"field not found: {nameof(C.F2)}"),
      typeof(C).GetField(nameof(C.F3)) ?? throw new InvalidOperationException($"field not found: {nameof(C.F3)}"),
      typeof(C).GetField(nameof(C.F4)) ?? throw new InvalidOperationException($"field not found: {nameof(C.F4)}"),
      typeof(C).GetField(nameof(C.F5)) ?? throw new InvalidOperationException($"field not found: {nameof(C.F5)}"),
      typeof(C).GetField(nameof(C.F6)) ?? throw new InvalidOperationException($"field not found: {nameof(C.F6)}"),
      typeof(C).GetField(nameof(C.F7)) ?? throw new InvalidOperationException($"field not found: {nameof(C.F7)}"),
      typeof(C).GetField(nameof(C.F8)) ?? throw new InvalidOperationException($"field not found: {nameof(C.F8)}"),
      typeof(C).GetField(nameof(C.F9)) ?? throw new InvalidOperationException($"field not found: {nameof(C.F9)}")
    );

  [TestCase(true)]
  [TestCase(false)]
  public Task FormatTypeName_OfPropertyInfo_Concurrent(bool lockCreatingNullabilityInfo)
    => FormatTypeName_Concurrent(
      lockCreatingNullabilityInfo,
      typeof(C),
      typeof(C).GetProperty(nameof(C.P0)) ?? throw new InvalidOperationException($"property not found: {nameof(C.P0)}"),
      typeof(C).GetProperty(nameof(C.P1)) ?? throw new InvalidOperationException($"property not found: {nameof(C.P1)}"),
      typeof(C).GetProperty(nameof(C.P2)) ?? throw new InvalidOperationException($"property not found: {nameof(C.P2)}"),
      typeof(C).GetProperty(nameof(C.P3)) ?? throw new InvalidOperationException($"property not found: {nameof(C.P3)}"),
      typeof(C).GetProperty(nameof(C.P4)) ?? throw new InvalidOperationException($"property not found: {nameof(C.P4)}"),
      typeof(C).GetProperty(nameof(C.P5)) ?? throw new InvalidOperationException($"property not found: {nameof(C.P5)}"),
      typeof(C).GetProperty(nameof(C.P6)) ?? throw new InvalidOperationException($"property not found: {nameof(C.P6)}"),
      typeof(C).GetProperty(nameof(C.P7)) ?? throw new InvalidOperationException($"property not found: {nameof(C.P7)}"),
      typeof(C).GetProperty(nameof(C.P8)) ?? throw new InvalidOperationException($"property not found: {nameof(C.P8)}"),
      typeof(C).GetProperty(nameof(C.P9)) ?? throw new InvalidOperationException($"property not found: {nameof(C.P9)}")
    );

  [TestCase(true)]
  [TestCase(false)]
  public Task FormatTypeName_OfEventInfo_Concurrent(bool lockCreatingNullabilityInfo)
    => FormatTypeName_Concurrent(
      lockCreatingNullabilityInfo,
      typeof(C),
      typeof(C).GetEvent(nameof(C.E0)) ?? throw new InvalidOperationException($"event not found: {nameof(C.E0)}"),
      typeof(C).GetEvent(nameof(C.E1)) ?? throw new InvalidOperationException($"event not found: {nameof(C.E1)}"),
      typeof(C).GetEvent(nameof(C.E2)) ?? throw new InvalidOperationException($"event not found: {nameof(C.E2)}"),
      typeof(C).GetEvent(nameof(C.E3)) ?? throw new InvalidOperationException($"event not found: {nameof(C.E3)}"),
      typeof(C).GetEvent(nameof(C.E4)) ?? throw new InvalidOperationException($"event not found: {nameof(C.E4)}"),
      typeof(C).GetEvent(nameof(C.E5)) ?? throw new InvalidOperationException($"event not found: {nameof(C.E5)}"),
      typeof(C).GetEvent(nameof(C.E6)) ?? throw new InvalidOperationException($"event not found: {nameof(C.E6)}"),
      typeof(C).GetEvent(nameof(C.E7)) ?? throw new InvalidOperationException($"event not found: {nameof(C.E7)}"),
      typeof(C).GetEvent(nameof(C.E8)) ?? throw new InvalidOperationException($"event not found: {nameof(C.E8)}"),
      typeof(C).GetEvent(nameof(C.E9)) ?? throw new InvalidOperationException($"event not found: {nameof(C.E9)}")
    );

  [TestCase(true)]
  [TestCase(false)]
  public Task FormatTypeName_OfParameterInfo_Concurrent(bool lockCreatingNullabilityInfo)
    => FormatTypeName_Concurrent(
      lockCreatingNullabilityInfo,
      typeof(C),
      typeof(C).GetMethod(nameof(C.M))!.GetParameters()
    );

  private async Task FormatTypeName_Concurrent<TMemberInfo>(
    bool lockCreatingNullabilityInfo,
    Type declaringTypeOfTargets,
    params TMemberInfo[] targets
  )
    where TMemberInfo : class
  {
    Func<object, NullabilityInfoContext, object?, string> formatTypeName;

    if (typeof(TMemberInfo) == typeof(FieldInfo)) {
      formatTypeName = lockCreatingNullabilityInfo
        ? new(static (f, ctx, lockObj) => CSharpFormatter.FormatTypeName((FieldInfo)f, ctx, nullabilityInfoContextLockObject: lockObj ?? throw new ArgumentNullException(nameof(lockObj))))
        : new(static (f, ctx, _) => CSharpFormatter.FormatTypeName((FieldInfo)f, ctx));
    }
    else if (typeof(TMemberInfo) == typeof(PropertyInfo)) {
      formatTypeName = lockCreatingNullabilityInfo
        ? new(static (p, ctx, lockObj) => CSharpFormatter.FormatTypeName((PropertyInfo)p, ctx, nullabilityInfoContextLockObject: lockObj ?? throw new ArgumentNullException(nameof(lockObj))))
        : new(static (p, ctx, _) => CSharpFormatter.FormatTypeName((PropertyInfo)p, ctx));
    }
    else if (typeof(TMemberInfo) == typeof(EventInfo)) {
      formatTypeName = lockCreatingNullabilityInfo
        ? new(static (ev, ctx, lockObj) => CSharpFormatter.FormatTypeName((EventInfo)ev, ctx, nullabilityInfoContextLockObject: lockObj ?? throw new ArgumentNullException(nameof(lockObj))))
        : new(static (ev, ctx, _) => CSharpFormatter.FormatTypeName((EventInfo)ev, ctx));
    }
    else if (typeof(TMemberInfo) == typeof(ParameterInfo)) {
      formatTypeName = lockCreatingNullabilityInfo
        ? new(static (para, ctx, lockObj) => CSharpFormatter.FormatTypeName((ParameterInfo)para, ctx, nullabilityInfoContextLockObject: lockObj ?? throw new ArgumentNullException(nameof(lockObj))))
        : new(static (para, ctx, _) => CSharpFormatter.FormatTypeName((ParameterInfo)para, ctx));
    }
    else {
      throw new InvalidOperationException();
    }

    Action<NullabilityInfoContext, object?> testAction = new(
      (ctx, lockObject) => {
        foreach (var target in targets) {
          formatTypeName(target, ctx, lockObject);
        }
      }
    );

    const int maxNumberOfRepeat = 20;

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

  internal static ValueTask AssertNullabilityInfoContextConcurrentOperationWithoutLockMustThrowExceptionAsync(
    Action<NullabilityInfoContext, object?> testAction,
    int maxNumberOfRepeat
  )
    => AssertNullabilityInfoContextConcurrentOperationAsync(
      testAction: testAction,
      enableLockForTestAction: false,
      maxNumberOfRepeat: maxNumberOfRepeat
    );

  internal static ValueTask AssertNullabilityInfoContextConcurrentOperationWithLockMustNotThrowExceptionAsync(
    Action<NullabilityInfoContext, object?> testAction,
    int maxNumberOfRepeat
  )
    => AssertNullabilityInfoContextConcurrentOperationAsync(
      testAction: testAction,
      enableLockForTestAction: true,
      maxNumberOfRepeat: maxNumberOfRepeat
    );

  private static async ValueTask AssertNullabilityInfoContextConcurrentOperationAsync(
    Action<NullabilityInfoContext, object?> testAction,
    bool enableLockForTestAction,
    int maxNumberOfRepeat
  )
  {
    var participantCount = Environment.ProcessorCount * 2 / 3;

    for (var n = 0; n < maxNumberOfRepeat; n++) {
      using var barrier = new Barrier(participantCount);
      using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10.0));

      var parallelOptions = new ParallelOptions() {
        MaxDegreeOfParallelism = participantCount,
        CancellationToken = cancellationTokenSource.Token,
      };

      // confirms that an ArgumentException or IndexOutOfRangeException is thrown when
      // NullabilityInfoContext.Create() is called in a concurrency.
      var ctx = new NullabilityInfoContext();
      var lockObject = enableLockForTestAction ? new object() : null;
      Exception? thrownException = null;

      try {
        await Parallel.ForEachAsync(Enumerable.Range(0, participantCount), parallelOptions, (_, cancellationToken) => {
          barrier.SignalAndWait(cancellationToken);

          testAction(ctx, lockObject);

          return default; // ValueTask
        });
      }
      catch (ArgumentException ex) when (
        (ex.Message ?? string.Empty).Contains("An item with the same key has already been added.", StringComparison.Ordinal)
      ) {
        // excepted exception: "An item with the same key has already been added."
        thrownException = ex;
      }
      catch (IndexOutOfRangeException ex) {
        // excepted exception: "Index was outside the bounds of the array."
        thrownException = ex;
      }
      catch (InvalidOperationException ex) when (
        (ex.Message ?? string.Empty).Contains("Operations that change non-concurrent collections must have exclusive access.", StringComparison.Ordinal)
      ) {
        // excepted exception: "Operations that change non-concurrent collections must have exclusive access. A concurrent update was performed on this collection and corrupted its state. The collection's state is no longer correct."
        thrownException = ex;
      }

      if (thrownException is not null) {
        if (enableLockForTestAction) {
          Assert.Fail($"The operation failed with exception. ({thrownException.GetType().FullName})");
          return;
        }
        else {
          return; // expected exception thrown
        }
      }
    }

    if (!enableLockForTestAction)
      Assert.Warn($"The operation succeeded unexpectedly. ({testAction.Method.Name})");
  }
}
#endif // SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
