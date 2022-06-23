// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using NUnit.Framework;

using Smdn.Reflection.ReverseGenerating;

namespace Smdn.Reflection.ReverseGenerating.ListApi.AttributeFilterTestCases;

class ClassToDetermineNamespace { }

[TypeAttributeFilterTestCaseAttribute("[Extension]")]
[TypeAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
public static class ExtensionClass {
  [MemberAttributeFilterTestCaseAttribute("")]
  [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  public static int Method(int x) => throw null;

  [MemberAttributeFilterTestCaseAttribute("[Extension]")]
  [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  public static int ExtensionMethod(this int x) => throw null;
}

public class TypeAttributes {
  public class IsReadOnlyAttr {
    [TypeAttributeFilterTestCaseAttribute("")]
    [TypeAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public struct Struct { }

    [TypeAttributeFilterTestCaseAttribute("[IsReadOnly]")]
    [TypeAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public readonly struct ReadOnlyStruct { }
  }

  public class CLSCompliantAttr {
#pragma warning disable CS3021
    [TypeAttributeFilterTestCaseAttribute("[CLSCompliant(false)]")]
    [TypeAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    [CLSCompliant(false)]
    public class CLSIncompliantType {
      [MemberAttributeFilterTestCaseAttribute("")]
      [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
      public int CLSCompliantMethod() => throw null;

      [MemberAttributeFilterTestCaseAttribute("[CLSCompliant(false)]")]
      [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
      [CLSCompliant(false)]
      public uint CLSIncompliantMethod() => throw null;

      [MemberAttributeFilterTestCaseAttribute("[CLSCompliant(false)]")]
      [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
      [CLSCompliant(false)]
      public uint CLSIncompliantField = 0U;
    }
#pragma warning restore CS3021
  }

  public class DefaultMemberAttr {
    [TypeAttributeFilterTestCaseAttribute("[DefaultMember(\"Items\")]")]
    [TypeAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public class DefaultMemberClass {
      [MemberAttributeFilterTestCaseAttribute("")]
      [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
      [IndexerName("Items")]
      public int this[int index] => throw null;
    }
  }
}

public class MethodAttributes {
  public class IteratorStateMachineAttr {
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public IEnumerable<int> Method() => throw null;

    [MemberAttributeFilterTestCaseAttribute("[IteratorStateMachine]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public IEnumerable<int> IteratorStateMachineMethod()
    {
      yield break;
    }
  }

  public class AsyncStateMachineAttr {
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public Task TaskMethod() => throw null;

    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public ValueTask ValueTaskMethod() => throw null;

    [MemberAttributeFilterTestCaseAttribute("[DebuggerStepThrough], [AsyncStateMachine]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public async void VoidAsyncStateMachineMethod() => await Task.Delay(0);

    [MemberAttributeFilterTestCaseAttribute("[DebuggerStepThrough], [AsyncStateMachine]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public async Task TaskAsyncStateMachineMethod() => await Task.Delay(0);

    [MemberAttributeFilterTestCaseAttribute("[DebuggerStepThrough], [AsyncStateMachine]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public async ValueTask ValueTaskAsyncStateMachineMethod() => await Task.Delay(0);
  }
}

public class FieldAttributes {
  public class TupleElementNamesAttr {
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public (int, int) Unnamed;

    [MemberAttributeFilterTestCaseAttribute("[TupleElementNames]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public (int X, int Y) Named;
  }
}

public class PropertyAttributes {
  public class TupleElementNamesAttr {
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public (int, int) Unnamed => throw null;

    [MemberAttributeFilterTestCaseAttribute("[TupleElementNames]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    public (int X, int Y) Named => throw null;
  }
}

public class ParameterAttributes {
  public void OptionalAttr(
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    int nonOptional,
    [MemberAttributeFilterTestCaseAttribute("[Optional]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    int optional = 0
  ) => throw null;

  public void InAttr_IsReadOnlyAttr(
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    int param,
    [MemberAttributeFilterTestCaseAttribute("[IsReadOnly], [In]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    in int paramIn
  ) => throw null;

  public void OutAttr(
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    int param,
    [MemberAttributeFilterTestCaseAttribute("[Out]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    out int paramIn
  ) => throw null;

  public void ParamArrayAttr(
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    int param,
    [MemberAttributeFilterTestCaseAttribute("[ParamArray]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    params int[] paramArray
  ) => throw null;

  public void TupleElementNamesAttr_Parameter(
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    int param,
    [MemberAttributeFilterTestCaseAttribute("")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    (int, int) unnamedTuple,
    [MemberAttributeFilterTestCaseAttribute("[TupleElementNames]")]
    [MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
    (int X, int Y) namedTuple
  ) => throw null;

  [return: MemberAttributeFilterTestCaseAttribute("")]
  [return: MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  public (int, int) TupleElementNamesAttr_ReturnParameter_Unnamed() => throw null;

  [return: MemberAttributeFilterTestCaseAttribute("[return: TupleElementNames]")]
  [return: MemberAttributeFilterTestCaseAttribute("", FilterType = typeof(AttributeFilter), FilterMemberName = nameof(AttributeFilter.Default))]
  public (int X, int Y) TupleElementNamesAttr_ReturnParameter_Named() => throw null;
}
