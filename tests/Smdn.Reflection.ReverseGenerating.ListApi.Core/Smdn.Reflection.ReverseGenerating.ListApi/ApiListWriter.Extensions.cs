// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using NUnit.Framework;

using Smdn.IO;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

#pragma warning disable IDE0040
partial class ApiListWriterTests {
#pragma warning restore IDE0040
  private static System.Collections.IEnumerable YieldTestCases_WriteExportedTypes_Extensions_Reconstruct()
  {
    foreach (var loadIntoReflectionOnlyContext in new[] { true, false }) {
      yield return new object[] {
        loadIntoReflectionOnlyContext,
        """
        using System;
        using System.Collections.Generic;

        public class C {
          public void M() => throw new NotImplementedException();
        }

        public static class CExtensionMembers {
          extension(int n) {
            // empty extension group
          }

          public static class CNestedPre {
            public static void MStaticNestedPre() => throw new NotImplementedException();
          }

          extension<T>(IEnumerable<T> t) {
            public bool P => throw new NotImplementedException();
            public static bool PStatic => throw new NotImplementedException();

            public void M0() => throw new NotImplementedException();
            public static void MStatic0() => throw new NotImplementedException();

            public int M1(int x) => throw new NotImplementedException();
            public static int MStatic1(int x) => throw new NotImplementedException();
          }

          extension(Uri) {
            public static Uri operator +(Uri @this, Uri other) => @this;
            public static Uri operator +(Uri @this) => @this;
          }

          extension(Uri u) {
            public void operator +=(Uri other) => throw new NotImplementedException();
          }

          public static void MStaticNonExtension0() => throw new NotImplementedException();
          public static int MStaticNonExtension1(int x) => throw new NotImplementedException();

          public static class CNestedPost {
            public static void MStaticNestedPost() => throw new NotImplementedException();
          }
        }

        public class CX {
          public void M() => throw new NotImplementedException();
        }
        """,
        new[] {
          typeof(Uri).Assembly.GetName().Name + ".dll",
          typeof(IEnumerable<>).Assembly.GetName().Name + ".dll",
          typeof(NotImplementedException).Assembly.GetName().Name + ".dll",
          typeof(CompilerFeatureRequiredAttribute).Assembly.GetName().Name + ".dll",
        },
        """
        using System;
        using System.Collections.Generic;

        public class C {
          public C() {}

          public void M() {}
        }

        public static class CExtensionMembers {
          extension<T>(IEnumerable<T> t) {
            public static bool PStatic { get; }

            public static void MStatic0() {}
            public static int MStatic1(int x) {}

            public bool P { get; }

            public void M0() {}
            public int M1(int x) {}
          }

          extension(Uri) {
            public static Uri operator + (Uri @this, Uri other) {}
            public static Uri operator + (Uri @this) {}

            public void operator += (Uri other) {}
          }

          extension(Uri u) {
            public static Uri operator + (Uri @this, Uri other) {}
            public static Uri operator + (Uri @this) {}

            public void operator += (Uri other) {}
          }

          public static class CNestedPost {
            public static void MStaticNestedPost() {}
          }

          public static class CNestedPre {
            public static void MStaticNestedPre() {}
          }

          public static void MStaticNonExtension0() {}
          public static int MStaticNonExtension1(int x) {}
        }

        public class CX {
          public CX() {}

          public void M() {}
        }
        """
      };

      yield return new object[] {
        loadIntoReflectionOnlyContext,
        """
        using System;
        using System.Collections.Generic;

        public static class C {
          extension(IEnumerable<Guid> x) {
            public bool MConstructedGeneric() => throw new NotImplementedException();
            public static bool MConstructedGenericStatic() => throw new NotImplementedException();

            public bool PConstructedGeneric {
              get => throw new NotImplementedException();
              set => throw new NotImplementedException();
            }
            public static bool PConstructedGenericStatic {
              get => throw new NotImplementedException();
              set => throw new NotImplementedException();
            }

            public static IEnumerable<Guid> operator +(IEnumerable<Guid> @this, IEnumerable<Guid> other) => throw new NotImplementedException(); // op_Addition
            public static IEnumerable<Guid> operator +(IEnumerable<Guid> @this) => throw new NotImplementedException(); // op_UnaryPlus
            public void operator +=(IEnumerable<Guid> other) => throw new NotImplementedException(); // op_AdditionAssignment
          }

          extension(Uri x) {
            public bool MNonGeneric() => throw new NotImplementedException();
            public static bool MNonGenericStatic() => throw new NotImplementedException();

            public bool PNonGeneric {
              get => throw new NotImplementedException();
              set => throw new NotImplementedException();
            }
            public static bool PNonGenericStatic {
              get => throw new NotImplementedException();
              set => throw new NotImplementedException();
            }

            public static Uri operator +(Uri @this, Uri other) => throw new NotImplementedException(); // op_Addition
            public static Uri operator +(Uri @this) => throw new NotImplementedException(); // op_UnaryPlus
            public void operator +=(Uri other) => throw new NotImplementedException(); // op_AdditionAssignment
          }

          extension<T>(IEnumerable<T> x) {
            public bool MGenericDef() => throw new NotImplementedException();
            public static bool MGenericDefStatic() => throw new NotImplementedException();

            public bool PGenericDef {
              get => throw new NotImplementedException();
              set => throw new NotImplementedException();
            }
            public static bool PGenericDefStatic {
              get => throw new NotImplementedException();
              set => throw new NotImplementedException();
            }

            public static IEnumerable<T> operator +(IEnumerable<T> @this, IEnumerable<T> other) => throw new NotImplementedException(); // op_Addition
            public static IEnumerable<T> operator +(IEnumerable<T> @this) => throw new NotImplementedException(); // op_UnaryPlus
            public void operator +=(IEnumerable<T> other) => throw new NotImplementedException(); // op_AdditionAssignment
          }
        }
        """,
        new[] {
          typeof(Uri).Assembly.GetName().Name + ".dll",
          typeof(Guid).Assembly.GetName().Name + ".dll",
          typeof(IEnumerable<>).Assembly.GetName().Name + ".dll",
          typeof(NotImplementedException).Assembly.GetName().Name + ".dll",
          typeof(CompilerFeatureRequiredAttribute).Assembly.GetName().Name + ".dll",
        },
        """
        using System;
        using System.Collections.Generic;

        public static class C {
          extension(IEnumerable<Guid> x) {
            public static bool PConstructedGenericStatic { get; set; }

            public static bool MConstructedGenericStatic() {}
            public static IEnumerable<Guid> operator + (IEnumerable<Guid> @this, IEnumerable<Guid> other) {}
            public static IEnumerable<Guid> operator + (IEnumerable<Guid> @this) {}

            public bool PConstructedGeneric { get; set; }

            public bool MConstructedGeneric() {}
            public void operator += (IEnumerable<Guid> other) {}
          }

          extension<T>(IEnumerable<T> x) {
            public static bool PGenericDefStatic { get; set; }

            public static bool MGenericDefStatic() {}
            public static IEnumerable<$T0> operator + (IEnumerable<$T0> @this, IEnumerable<$T0> other) {}
            public static IEnumerable<$T0> operator + (IEnumerable<$T0> @this) {}

            public bool PGenericDef { get; set; }

            public bool MGenericDef() {}
            public void operator += (IEnumerable<$T0> other) {}
          }

          extension(Uri x) {
            public static bool PNonGenericStatic { get; set; }

            public static bool MNonGenericStatic() {}
            public static Uri operator + (Uri @this, Uri other) {}
            public static Uri operator + (Uri @this) {}

            public bool PNonGeneric { get; set; }

            public bool MNonGeneric() {}
            public void operator += (Uri other) {}
          }
        }
        """
      };
    } // foreach
  }

  [TestCaseSource(nameof(YieldTestCases_WriteExportedTypes_Extensions_Reconstruct))]
  public void WriteExportedTypes_Extensions_Reconstruct(
    bool loadIntoReflectionOnlyContext,
    string sourceCode,
    string[] referenceAssemblyFileNames,
    string expectedOutput
  )
  {
    var options = new ApiListWriterOptions();

    options.Writer.ReconstructExtensionDeclarations = true;
    options.Writer.OrderExtensionDeclarationsFirst = true;
    options.Writer.WriteNullableAnnotationDirective = false;
    options.Writer.OrderStaticMembersFirst = true;
    options.Writer.WriteHeader = false;
    options.Writer.WriteFooter = false;
    options.Indent = "  ";
    options.MemberDeclaration.MethodBody = MethodBodyOption.EmptyImplementation;
    options.MemberDeclaration.AccessorBody = MethodBodyOption.EmptyImplementation;
    options.AttributeDeclaration.TypeFilter = AttributeFilter.Default;

    var output = new StringReader(
      WriteApiListFromSourceCode(
        sourceCode,
        options,
        referenceAssemblyFileNames: referenceAssemblyFileNames,
        loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext
      )
    ).ReadAllLines();

    var joinedOutput = string.Join("\n", output);

    // Console.WriteLine(joinedOutput);

    Assert.That(
      joinedOutput.Trim(),
      Is.EqualTo(expectedOutput.Replace("\r", "").Trim())
    );
  }
}
