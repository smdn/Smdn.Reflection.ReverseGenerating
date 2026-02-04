// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.AttributeList.AttributeTypes;

[AttributeListTestCase("[System.Flags]")]
[Flags]
enum Flags1 : int { }

[AttributeListTestCase("[System.Flags]", AttributeWithNamespace = true)]
[AttributeListTestCase("[Flags]", AttributeWithNamespace = false)]
[Flags]
enum Flags2 : int { }

[AttributeListTestCase("[System.Flags], [System.Obsolete]")]
[Flags]
[Obsolete]
enum Flags3 : int { }

[AttributeListTestCase("[System.Flags], [System.Obsolete]")]
[Obsolete]
[Flags]
enum Flags4 : int { }

[AttributeListTestCase("[System.Obsolete]")]
[Obsolete]
class Obsolete1 { }

[AttributeListTestCase("[System.Obsolete(\"obsolete\")]")]
[Obsolete("obsolete")]
class Obsolete2 { }

[AttributeListTestCase("[System.Obsolete(\"deprecated\", true)]", AttributeWithNamedArguments = false)]
[AttributeListTestCase("[System.Obsolete(message: \"deprecated\", error: true)]", AttributeWithNamedArguments = true)]
[Obsolete("deprecated", true)]
class Obsolete3 { }

[AttributeListTestCase("[System.Obsolete(\"deprecated\", false)]", AttributeWithNamedArguments = false)]
[AttributeListTestCase("[System.Obsolete(message: \"deprecated\", error: false)]", AttributeWithNamedArguments = true)]
[Obsolete("deprecated", false)]
class Obsolete4 { }

[AttributeListTestCase("[System.Serializable]")]
[Serializable]
class Serializable1 { }

class Conditionals {
  [AttributeListTestCase("[System.Diagnostics.Conditional(\"DEBUG\")]")]
  [AttributeListTestCase("[Conditional(\"DEBUG\")]", AttributeWithNamespace = false)]
  [System.Diagnostics.Conditional("DEBUG")]
  public void M() { }
}

[AttributeListTestCase("[System.Runtime.CompilerServices.Extension]")]
static class Extension {
  [AttributeListTestCase("[System.Runtime.CompilerServices.Extension]")]
  public static void M1(this int x) { }
}

[AttributeListTestCase("[System.Runtime.CompilerServices.Extension]")]
static class ExtensionMembers {
  extension(int x) {
    // emitted attribute data may differ depending on the compiler version (?)
    // [AttributeListTestCase("[System.Runtime.CompilerServices.ExtensionMarker(...)]")]
    public void M() => throw new NotImplementedException();

    // emitted attribute data may differ depending on the compiler version (?)
    // [AttributeListTestCase("[System.Runtime.CompilerServices.ExtensionMarker(...)]")]
    public bool P => throw new NotImplementedException();

    // emitted attribute data may differ depending on the compiler version (?)
    // [AttributeListTestCase("[System.Runtime.CompilerServices.ExtensionMarker(...)]")]
    public static int operator +(int other) => throw new NotImplementedException();
  }
}

[AttributeListTestCase("[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 1)]", AttributeWithNamespace = true, ValueWithNamespace = true)]
[AttributeListTestCase("[System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack = 1)]", AttributeWithNamespace = true, ValueWithNamespace = false)]
[AttributeListTestCase("[StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 1)]", AttributeWithNamespace = false, ValueWithNamespace = true)]
[AttributeListTestCase("[StructLayout(LayoutKind.Explicit, Pack = 1)]", AttributeWithNamespace = false, ValueWithNamespace = false)]
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 1)]
struct StructLayout1 {
  [AttributeListTestCase("[System.Runtime.InteropServices.FieldOffset(0)]", AttributeWithNamespace = true)]
  [AttributeListTestCase("[FieldOffset(0)]", AttributeWithNamespace = false)]
  [System.Runtime.InteropServices.FieldOffset(0)]
  public byte F0;
}

[AttributeListTestCase("[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 1)]", AttributeWithNamespace = true, ValueWithNamespace = true)]
[AttributeListTestCase("[System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Size = 1)]", AttributeWithNamespace = true, ValueWithNamespace = false)]
[AttributeListTestCase("[StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 1)]", AttributeWithNamespace = false, ValueWithNamespace = true)]
[AttributeListTestCase("[StructLayout(LayoutKind.Explicit, Size = 1)]", AttributeWithNamespace = false, ValueWithNamespace = false)]
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 1)]
struct StructLayout2 {
  [AttributeListTestCase("[System.Runtime.InteropServices.FieldOffset(0)]", AttributeWithNamespace = true)]
  [AttributeListTestCase("[FieldOffset(0)]", AttributeWithNamespace = false)]
  [System.Runtime.InteropServices.FieldOffset(0)] public byte F0;
}

[AttributeListTestCase("")]
struct NoStructLayout { }
