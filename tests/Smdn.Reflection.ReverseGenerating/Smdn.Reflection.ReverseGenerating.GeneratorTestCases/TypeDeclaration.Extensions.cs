// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.Extensions {
  namespace ExtensionGroupingTypeDeclarations {
    [ExtensionGroupingTypeDeclarationTestCase("extension(int x)")]
    [ExtensionGroupingTypeDeclarationTestCase("extension(int x)", MemberWithAccessibility = true)]
    [ExtensionGroupingTypeDeclarationTestCase("extension(int x)", TypeWithAccessibility = true)]
    [ExtensionGroupingTypeReferencingNamespacesTestCase("")]
    public static class PrimitiveType {
      extension(int x) {
        public bool P => throw new NotImplementedException();
      }
    }

#nullable enable
    [ExtensionGroupingTypeDeclarationTestCase("extension(string? x)")]
    [ExtensionGroupingTypeReferencingNamespacesTestCase("")]
    public static class NullableReferenceType {
      extension(string? x) {
        public bool P => throw new NotImplementedException();
      }
    }
#nullable restore

    [ExtensionGroupingTypeDeclarationTestCase("extension(Guid? x)", ParameterWithNamespace = false)]
    [ExtensionGroupingTypeDeclarationTestCase("extension(System.Guid? x)", TypeWithNamespace = false)]
    [ExtensionGroupingTypeDeclarationTestCase("extension(System.Guid? x)", MemberWithNamespace = false)]
    [ExtensionGroupingTypeReferencingNamespacesTestCase("System")]
    public static class NullableValueType {
      extension(Guid? x) {
        public bool P => throw new NotImplementedException();
      }
    }

    [ExtensionGroupingTypeDeclarationTestCase("extension(IEnumerable<string> x)", ParameterWithNamespace = false)]
    [ExtensionGroupingTypeDeclarationTestCase("extension(System.Collections.Generic.IEnumerable<string> x)", ParameterWithNamespace = true)]
    [ExtensionGroupingTypeReferencingNamespacesTestCase("System.Collections.Generic")]
    public static class CloseGenericType {
      extension(IEnumerable<string> x) {
        public bool P => throw new NotImplementedException();
      }
    }

    [ExtensionGroupingTypeDeclarationTestCase("extension<T>(IEnumerable<T> x) where T : class, IDisposable", ParameterWithNamespace = false, TypeWithNamespace = false)]
    [ExtensionGroupingTypeDeclarationTestCase("extension<T>(IEnumerable<T> x) where T : class, System.IDisposable", ParameterWithNamespace = false, TypeWithNamespace = true)]
    [ExtensionGroupingTypeDeclarationTestCase("extension<T>(System.Collections.Generic.IEnumerable<T> x) where T : class, IDisposable", ParameterWithNamespace = true, TypeWithNamespace = false)]
    [ExtensionGroupingTypeReferencingNamespacesTestCase("System, System.Collections.Generic")]
    public static class OpenGenericType {
      extension<T>(IEnumerable<T> x) where T : class, IDisposable {
        public bool P => throw new NotImplementedException();
      }
    }

    namespace ParameterModifiers {
      [ExtensionGroupingTypeDeclarationTestCase("extension(ref int x)")]
      [ExtensionGroupingTypeReferencingNamespacesTestCase("")]
      public static class Ref {
        extension(ref int x) {
          public bool P => throw new NotImplementedException();
        }
      }

      [ExtensionGroupingTypeDeclarationTestCase("extension(ref readonly System.ReadOnlySpan<int> x)")]
      [ExtensionGroupingTypeReferencingNamespacesTestCase(
        // System.ReadOnlySpan
        // System.Runtime.CompilerServices.RequiresLocationAttribute
        // System.Runtime.InteropServices.InAttribute
        "System, System.Runtime.CompilerServices, System.Runtime.InteropServices"
      )]
      public static class RefReadOnly {
        extension(ref readonly ReadOnlySpan<int> x) {
          public bool P => throw new NotImplementedException();
        }
      }
    }
  }
}
