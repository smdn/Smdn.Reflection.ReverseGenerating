// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cSpell:ignore accessibilities,nullabilities

#pragma warning disable CS8597

using System;
using System.Runtime.CompilerServices;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Properties;

class GeneratorOptions {
  public abstract class Abstract {
    [MemberDeclarationTestCase("public abstract int P { get; set; }", AccessorBody = MethodBodyOption.None)]
    [MemberDeclarationTestCase("public abstract int P { get; set; }", AccessorBody = MethodBodyOption.EmptyImplementation)]
    [MemberDeclarationTestCase("public abstract int P { get; set; }", AccessorBody = MethodBodyOption.ThrowNotImplementedException)]
    [MemberDeclarationTestCase("public abstract int P { get; set; }", AccessorBody = MethodBodyOption.ThrowNull)]
    [MemberDeclarationTestCase("public abstract int P { get; set; }", MemberWithDeclaringTypeName = false, MemberWithNamespace = false, AccessorBody = MethodBodyOption.None)]
    [MemberDeclarationTestCase("public abstract int GeneratorOptions.Abstract.P { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.None)]
    [MemberDeclarationTestCase("public abstract int Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Properties.GeneratorOptions.Abstract.P { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, AccessorBody = MethodBodyOption.None)]
    public abstract int P { get; set; }
  }

  public abstract class NonAbstract {
    [MemberDeclarationTestCase("public int P { get; set; }", AccessorBody = MethodBodyOption.None)]
    [MemberDeclarationTestCase("public int P { get; set; }", AccessorBody = MethodBodyOption.EmptyImplementation)]
    [MemberDeclarationTestCase("public int P { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }", AccessorBody = MethodBodyOption.ThrowNotImplementedException)]
    [MemberDeclarationTestCase("public int P { get => throw null; set => throw null; }", AccessorBody = MethodBodyOption.ThrowNull)]
    [MemberDeclarationTestCase("public int P { get; set; }", MemberWithDeclaringTypeName = false, MemberWithNamespace = false, AccessorBody = MethodBodyOption.None)]
    [MemberDeclarationTestCase("public int GeneratorOptions.NonAbstract.P { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false, AccessorBody = MethodBodyOption.None)]
    [MemberDeclarationTestCase("public int Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Properties.GeneratorOptions.NonAbstract.P { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true, AccessorBody = MethodBodyOption.None)]
    public int P { get; set; }
  }
}

public class ValueTupleTypes {
  [MemberDeclarationTestCase("public (int X, int Y) P1 { get; }")] public (int X, int Y) P1 => throw null;
  [MemberDeclarationTestCase("public (int, int) P2 { get; }")] public (int, int) P2 => throw null;
  [MemberDeclarationTestCase("public (int, int) P3 { get; }")] public ValueTuple<int, int> P3 => throw null;
  [MemberDeclarationTestCase("public System.ValueTuple<int> P4 { get; }")] public ValueTuple<int> P4 => throw null;
}

public class RefReturnTypes {
  [MemberDeclarationTestCase($"public ref int {nameof(PRefInt)} {{ get; }}")] public ref int PRefInt => throw null;
  [MemberDeclarationTestCase($"public ref int? {nameof(PRefNullableInt)} {{ get; }}")] public ref int? PRefNullableInt => throw null;
  [MemberDeclarationTestCase($"public ref string {nameof(PRefString)} {{ get; }}")] public ref string PRefString => throw null;
  [MemberDeclarationTestCase($"public ref System.Collections.Generic.KeyValuePair<int, int> {nameof(PRefKeyValuePairOfIntInt)} {{ get; }}")] public ref System.Collections.Generic.KeyValuePair<int, int> PRefKeyValuePairOfIntInt => throw null;
}

public class Accessors {
  [MemberDeclarationTestCase("public int P1 { get; set; }")] public int P1 { get; set; }
  [MemberDeclarationTestCase("public int P2 { get; }")] public int P2 { get; }
  [MemberDeclarationTestCase("public int P3 { set; }")] public int P3 { set { int x = value; } }
  [MemberDeclarationTestCase("public int P4 { get; init; }")] public int P4 { get; init; }

  public struct AccessorsStruct {
    [MemberDeclarationTestCase("public int P1 { get; }")] public int P1 => 0;
  }

  public readonly struct AccessorsReadOnlyStruct {
    [MemberDeclarationTestCase("public int P1 { get; }")] public int P1 => 0;
  }
}


public class Modifiers_ReadOnly {
  public struct Struct {
    [MemberDeclarationTestCase("public readonly int P1 { get; }")] public int P1 { get; }
    [MemberDeclarationTestCase("public readonly int P2 { get; }")] public readonly int P2 { get; }
    [MemberDeclarationTestCase("public int P3 { get; }")] public int P3 { get => 0; }
    [MemberDeclarationTestCase("public readonly int P4 { get; }")] public readonly int P4 { get => 0; }
    [MemberDeclarationTestCase("public int P5 { get; }")] public int P5 => 0;
    [MemberDeclarationTestCase("public readonly int P6 { get; }")] public readonly int P6 => 0;
    [MemberDeclarationTestCase("public readonly int P7 { get; set; }")] public int P7 { readonly get => 0; set => throw new NotImplementedException(); }

    [MemberDeclarationTestCase("public static int SP0 { get; } = 0;")] public static int SP0 { get; }
    [MemberDeclarationTestCase("public static int SP1 { get; }")] public static int SP1 { get => 0; }
  }

  public readonly struct ReadOnlyStruct {
    [MemberDeclarationTestCase("public int P1 { get; }")] public int P1 { get; }
    [MemberDeclarationTestCase("public int P2 { get; }")] public readonly int P2 { get; }
    [MemberDeclarationTestCase("public int P3 { get; }")] public int P3 { get => 0; }
    [MemberDeclarationTestCase("public int P4 { get; }")] public readonly int P4 { get => 0; }
    [MemberDeclarationTestCase("public int P5 { get; }")] public int P5 => 0;
    [MemberDeclarationTestCase("public int P6 { get; }")] public readonly int P6 => 0;

    [MemberDeclarationTestCase("public static int SP0 { get; } = 0;")] public static int SP0 { get; }
    [MemberDeclarationTestCase("public static int SP1 { get; }")] public static int SP1 { get => 0; }
  }
}

public class Modifiers_Static {
  [MemberDeclarationTestCase("public static int P1 { get; set; }")] public static int P1 { get; set; }
  [MemberDeclarationTestCase("public static int P2 { get; }")] public static int P2 { get => 0; }
  [MemberDeclarationTestCase("public static int P3 { set; }")] public static int P3 { set => throw new NotImplementedException(); }
}

public abstract class Modifiers_Abstract {
  [MemberDeclarationTestCase("public abstract int P1 { get; set; }")] public abstract int P1 { get; set; }
  [MemberDeclarationTestCase("public virtual int P2 { get; set; }")] public virtual int P2 { get; set; }

  [MemberDeclarationTestCase("protected virtual int PProtectedVirtual { get; private set; }")] protected virtual int PProtectedVirtual { get; private set; }
}

public abstract class Modifiers_Override : Modifiers_Abstract {
  [MemberDeclarationTestCase("public override int P2 { get; set; }")] public override int P2 { get; set; }

  [MemberDeclarationTestCase("protected override int PProtectedVirtual { get; }")] protected override int PProtectedVirtual { get => 0; }
}

public abstract class Modifiers_New : Modifiers_Abstract {
  [MemberDeclarationTestCase("new public int P2 { get; set; }")] public new int P2 { get; set; }

  [MemberDeclarationTestCase("new public int PProtectedVirtual { private get; set; }")] public new int PProtectedVirtual { private get; set; }
}

public abstract class Modifiers_NewVirtual : Modifiers_Abstract {
  [MemberDeclarationTestCase("new public virtual int P2 { get; set; }")] public new virtual int P2 { get; set; }

  [MemberDeclarationTestCase("new protected virtual int PProtectedVirtual { private get; set; }")] new protected virtual int PProtectedVirtual { private get; set; }
}

public class Modifiers_Virtual_WithAccessorAccessibility {
  [MemberDeclarationTestCase("public virtual int PVirtualWithPrivateGetter { private get; set; }")] public virtual int PVirtualWithPrivateGetter { private get; set; }
  [MemberDeclarationTestCase("public virtual int PVirtualWithPrivateSetter { get; private set; }")] public virtual int PVirtualWithPrivateSetter { get; private set; }

  [MemberDeclarationTestCase("public virtual int PVirtual { get; }")] public virtual int PVirtual { get; }
  [MemberDeclarationTestCase("public virtual int PVirtualWithProtectedGetter { protected get; set; }")] public virtual int PVirtualWithProtectedGetter { protected get; set; }
  [MemberDeclarationTestCase("public virtual int PVirtualWithProtectedSetter { get; protected set; }")] public virtual int PVirtualWithProtectedSetter { get; protected set; }
}

public class Modifiers_Override_WithAccessorAccessibility : Modifiers_Virtual_WithAccessorAccessibility {
  [MemberDeclarationTestCase("public override int PVirtual { get; }")] public override int PVirtual { get => throw null; }
  [MemberDeclarationTestCase("public override int PVirtualWithProtectedGetter { set; }")] public override int PVirtualWithProtectedGetter { set => throw null; }
  [MemberDeclarationTestCase("public override int PVirtualWithProtectedSetter { get; }")] public override int PVirtualWithProtectedSetter { get => throw null; }
}

public class Modifiers_SealedOverride_WithAccessorAccessibility : Modifiers_Virtual_WithAccessorAccessibility {
  [MemberDeclarationTestCase("public sealed override int PVirtual { get; }")] public sealed override int PVirtual { get => throw null; }
  [MemberDeclarationTestCase("public sealed override int PVirtualWithProtectedGetter { protected get; set; }")] public sealed override int PVirtualWithProtectedGetter { protected get => throw null; set => throw null;  }
  [MemberDeclarationTestCase("public sealed override int PVirtualWithProtectedSetter { get; protected set; }")] public sealed override int PVirtualWithProtectedSetter { get => throw null; protected set => throw null; }
}

public class Modifiers_NewVirtual_WithAccessorAccessibility : Modifiers_Virtual_WithAccessorAccessibility {
  [MemberDeclarationTestCase("new public virtual int PVirtual { get; }")] public new virtual int PVirtual { get; }
  [MemberDeclarationTestCase("new public virtual int PVirtualWithProtectedGetter { private get; set; }")] new public virtual int PVirtualWithProtectedGetter { /* changes accessibility */ private get; set; }
  [MemberDeclarationTestCase("new public virtual int PVirtualWithProtectedSetter { get; private set; }")] public new virtual int PVirtualWithProtectedSetter { get; /* changes accessibility */ private set; }
}

public class Indexers1 {
  [IndexerName("Indexer")]
  [MemberDeclarationTestCase("public int this[int x] { get; set; }")]
  [MemberDeclarationTestCase("public int Indexers1.Indexer[int x] { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public int Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Properties.Indexers1.Indexer[int x] { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
  public int this[int x] { get { return 0; } set { int val = value; } }
}

public class Indexers2 {
  [IndexerName("Indexer")]
  [MemberDeclarationTestCase("public int this[int x, int y] { get; set; }")] public int this[int x, int y] { get { return 0; } set { int val = value; } }
}

public class Indexers3 {
  [IndexerName("Indexer")]
  [MemberDeclarationTestCase("public int this[int @in] { get; set; }")] public int this[int @in] { get { return 0; } set { int val = value; } }
}

public class Indexers4 {
  [IndexerName("Indexer")]
  [MemberDeclarationTestCase("public int this[string x] { get; set; }")] public int this[string x] { get { return 0; } set { int val = value; } }
}

public class IndexerWithoutIndexerNameAttribute {
  [MemberDeclarationTestCase("public int this[int x] { get; set; }")]
  [MemberDeclarationTestCase("public int IndexerWithoutIndexerNameAttribute.Item[int x] { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public int Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Properties.IndexerWithoutIndexerNameAttribute.Item[int x] { get; set; }", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
  public int this[int x] { get { return 0; } set { int val = value; } }
}

public class AccessibilitiesOfProperty {
  [MemberDeclarationTestCase("public int P1 { get; set; }")] public int P1 { get; set; }
  [MemberDeclarationTestCase("internal int P2 { get; set; }")] internal int P2 { get; set; }
  [MemberDeclarationTestCase("protected int P3 { get; set; }")] protected int P3 { get; set; }
  [MemberDeclarationTestCase("internal protected int P4 { get; set; }")] protected internal int P4 { get; set; }
  [MemberDeclarationTestCase("internal protected int P5 { get; set; }")] internal protected int P5 { get; set; }
  [MemberDeclarationTestCase("private protected int P6 { get; set; }")] private protected int P6 { get; set; }
  [MemberDeclarationTestCase("private protected int P7 { get; set; }")] protected private int P7 { get; set; }
  [MemberDeclarationTestCase("private int P8 { get; set; }")] private int P8 { get; set; }

  [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] internal int P9 { get; set; }
  [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private protected int P10 { get; set; }
  [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private int P11 { get; set; }
}

public class AccessibilitiesOfAccessors_SetMethod_Public {
  [MemberDeclarationTestCase("public int P1 { get; internal set; }")] public int P1 { get; internal set; }
  [MemberDeclarationTestCase("public int P2 { get; protected set; }")] public int P2 { get; protected set; }
  [MemberDeclarationTestCase("public int P3 { get; internal protected set; }")] public int P3 { get; protected internal set; }
  [MemberDeclarationTestCase("public int P4 { get; internal protected set; }")] public int P4 { get; internal protected set; }
  [MemberDeclarationTestCase("public int P5 { get; private protected set; }")] public int P5 { get; private protected set; }
  [MemberDeclarationTestCase("public int P6 { get; private protected set; }")] public int P6 { get; protected private set; }
  [MemberDeclarationTestCase("public int P7 { get; private set; }")] public int P7 { get; private set; }

  [MemberDeclarationTestCase("public int P8 { get; }", IgnorePrivateOrAssembly = true)] public int P8 { get; internal set; }
  [MemberDeclarationTestCase("public int P9 { get; }", IgnorePrivateOrAssembly = true)] public int P9 { get; private protected set; }
  [MemberDeclarationTestCase("public int P10 { get; }", IgnorePrivateOrAssembly = true)] public int P10 { get; private set; }
}

public class AccessibilitiesOfAccessors_GetMethod_Public {
  [MemberDeclarationTestCase("public int P1 { internal get; set; }")] public int P1 { internal get; set; }
  [MemberDeclarationTestCase("public int P2 { protected get; set; }")] public int P2 { protected get; set; }
  [MemberDeclarationTestCase("public int P3 { internal protected get; set; }")] public int P3 { protected internal get; set; }
  [MemberDeclarationTestCase("public int P4 { internal protected get; set; }")] public int P4 { internal protected get; set; }
  [MemberDeclarationTestCase("public int P5 { private protected get; set; }")] public int P5 { private protected get; set; }
  [MemberDeclarationTestCase("public int P6 { private protected get; set; }")] public int P6 { protected private get; set; }
  [MemberDeclarationTestCase("public int P7 { private get; set; }")] public int P7 { private get; set; }

  [MemberDeclarationTestCase("public int P8 { set; }", IgnorePrivateOrAssembly = true)] public int P8 { internal get; set; }
  [MemberDeclarationTestCase("public int P9 { set; }", IgnorePrivateOrAssembly = true)] public int P9 { private protected get; set; }
  [MemberDeclarationTestCase("public int P10 { set; }", IgnorePrivateOrAssembly = true)] public int P10 { private get; set; }
}

public class AccessibilitiesOfAccessors_FamilyOrAssembly {
  [MemberDeclarationTestCase("internal protected int P0 { get; set; }")] internal protected int P0 { get; set; }
  [MemberDeclarationTestCase("internal protected int P1 { get; internal set; }")] internal protected int P1 { get; internal set; }
  [MemberDeclarationTestCase("internal protected int P2 { get; protected set; }")] internal protected int P2 { get; protected set; }
  [MemberDeclarationTestCase("internal protected int P3 { get; private protected set; }")] internal protected int P3 { get; private protected set; }
  [MemberDeclarationTestCase("internal protected int P4 { get; private protected set; }")] internal protected int P4 { get; protected private set; }
  [MemberDeclarationTestCase("internal protected int P5 { get; private set; }")] internal protected int P5 { get; private set; }

  [MemberDeclarationTestCase("internal protected int P6 { get; }", IgnorePrivateOrAssembly = true)] internal protected int P6 { get; internal set; }
  [MemberDeclarationTestCase("internal protected int P7 { get; }", IgnorePrivateOrAssembly = true)] internal protected int P7 { get; private protected set; }
  [MemberDeclarationTestCase("internal protected int P8 { get; }", IgnorePrivateOrAssembly = true)] internal protected int P8 { get; private set; }
}

#if SYSTEM_RUNTIME_COMPILERSERVICES_COMPILERFEATUREREQUIREDATTRIBUTE
public class ClassRequiredProperties {
  [MemberDeclarationTestCase($"public required int {nameof(Required)} {{ get; set; }}")] public required int Required { get; set; }
  [MemberDeclarationTestCase($"public required int {nameof(RequiredInitOnly)} {{ get; init; }}")] public required int RequiredInitOnly { get; init; }

  public abstract class WithAbstractOrVirtualModifiers {
    [MemberDeclarationTestCase($"public abstract required int {nameof(AbstractRequired)} {{ get; set; }}")] public abstract required int AbstractRequired { get; set; }
    [MemberDeclarationTestCase($"public abstract required int {nameof(RequiredAbstract)} {{ get; set; }}")] public required abstract int RequiredAbstract { get; set; }

    [MemberDeclarationTestCase($"public virtual required int {nameof(VirtualRequired)} {{ get; set; }}")] public virtual required int VirtualRequired { get; set; }
    [MemberDeclarationTestCase($"public virtual required int {nameof(RequiredVirtual)} {{ get; set; }}")] public required virtual int RequiredVirtual { get; set; }

    [MemberDeclarationTestCase($"public virtual int {nameof(NonRequiredVirtual)} {{ get; set; }}")] public virtual int NonRequiredVirtual { get; set; }
    [MemberDeclarationTestCase($"public virtual int {nameof(NonRequiredVirtualInitOnly)} {{ get; init; }}")] public virtual int NonRequiredVirtualInitOnly { get; init; }
  }

  public class WithOverrideModifiers : WithAbstractOrVirtualModifiers{
    [MemberDeclarationTestCase($"public override required int {nameof(AbstractRequired)} {{ get; set; }}")] public override required int AbstractRequired { get; set; }
    [MemberDeclarationTestCase($"public override required int {nameof(RequiredAbstract)} {{ get; set; }}")] public required override int RequiredAbstract { get; set; }

    [MemberDeclarationTestCase($"public override required int {nameof(NonRequiredVirtual)} {{ get; set; }}")] public override required int NonRequiredVirtual { get; set; }
    [MemberDeclarationTestCase($"public override required int {nameof(NonRequiredVirtualInitOnly)} {{ get; init; }}")] public required override int NonRequiredVirtualInitOnly { get; init; }
  }
}

public struct StructRequiredProperties {
  [MemberDeclarationTestCase($"public required int {nameof(Required)} {{ get; set; }}")] public required int Required { get; set; }
  [MemberDeclarationTestCase($"public required int {nameof(RequiredInitOnly)} {{ get; init; }}")] public required int RequiredInitOnly { get; init; }
}

public record class RecordClassRequiredProperties(int X) {
  [MemberDeclarationTestCase($"public required int {nameof(Required)} {{ get; set; }}")] public required int Required { get; set; }
  [MemberDeclarationTestCase($"public required int {nameof(RequiredInitOnly)} {{ get; init; }}")] public required int RequiredInitOnly { get; init; }
}

public record struct RecordStructRequiredProperties(int X) {
  [MemberDeclarationTestCase($"public required int {nameof(Required)} {{ get; set; }}")] public required int Required { get; set; }
  [MemberDeclarationTestCase($"public required int {nameof(RequiredInitOnly)} {{ get; init; }}")] public required int RequiredInitOnly { get; init; }
}
#endif // SYSTEM_RUNTIME_COMPILERSERVICES_COMPILERFEATUREREQUIREDATTRIBUTE
