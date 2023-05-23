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

  [Test]
  public Task FormatTypeName_FieldInfo_Concurrent()
    => FormatTypeName_Concurrent(
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

  [Test]
  public Task FormatTypeName_PropertyInfo_Concurrent()
    => FormatTypeName_Concurrent(
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

  [Test]
  public Task FormatTypeName_EventInfo_Concurrent()
    => FormatTypeName_Concurrent(
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

  [Test]
  public Task FormatTypeName_ParameterInfo_Concurrent()
    => FormatTypeName_Concurrent(
      typeof(C),
      typeof(C).GetMethod(nameof(C.M))!.GetParameters()
    );

  private async Task FormatTypeName_Concurrent<TMemberInfo>(Type declaringTypeOfTargets, params TMemberInfo[] targets)
    where TMemberInfo : class
  {
    Func<object, NullabilityInfoContext, string> formatTypeName;

    if (typeof(TMemberInfo) == typeof(FieldInfo))
      formatTypeName = new Func<object, NullabilityInfoContext, string>(static (f, ctx) => CSharpFormatter.FormatTypeName((FieldInfo)f, ctx));
    else if (typeof(TMemberInfo) == typeof(PropertyInfo))
      formatTypeName = new Func<object, NullabilityInfoContext, string>(static (p, ctx) => CSharpFormatter.FormatTypeName((PropertyInfo)p, ctx));
    else if (typeof(TMemberInfo) == typeof(EventInfo))
      formatTypeName = new Func<object, NullabilityInfoContext, string>(static (ev, ctx) => CSharpFormatter.FormatTypeName((EventInfo)ev, ctx));
    else if (typeof(TMemberInfo) == typeof(ParameterInfo))
      formatTypeName = new Func<object, NullabilityInfoContext, string>(static (para, ctx) => CSharpFormatter.FormatTypeName((ParameterInfo)para, ctx));
    else
      throw new NotSupportedException();

    const int maxNumberOfRepeat = 20;
    var participantCount = Environment.ProcessorCount * 2;
    var expectedExceptionThrown = false;

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

      try {
        await Parallel.ForEachAsync(Enumerable.Range(0, participantCount), parallelOptions, (_, cancellationToken) => {
          barrier.SignalAndWait(cancellationToken);

          foreach (var target in targets) {
            formatTypeName(target, ctx);
          }

          return default; // ValueTask
        });
      }
      catch (ArgumentException ex) when (
        (ex.Message ?? string.Empty).Contains(declaringTypeOfTargets.FullName!, StringComparison.Ordinal)
      ) {
        // excepted exception: "An item with the same key has already been added."
        expectedExceptionThrown = true;
      }
      catch (IndexOutOfRangeException) {
        // excepted exception: "Index was outside the bounds of the array."
        expectedExceptionThrown = true;
      }

      if (expectedExceptionThrown)
        break;
    }

    if (!expectedExceptionThrown)
      Assert.Warn($"The operation succeeded unexpectedly. ({formatTypeName.Method.Name}");
  }
}
#endif // SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
