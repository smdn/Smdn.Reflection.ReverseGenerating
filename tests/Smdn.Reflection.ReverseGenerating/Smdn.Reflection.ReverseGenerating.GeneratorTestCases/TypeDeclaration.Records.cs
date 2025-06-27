// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if NET7_0_OR_GREATER
#define SYSTEM_RUNTIME_COMPILERSERVICES_REQUIREDMEMBERATTRIBUTE
#endif

using System;
using System.Runtime.CompilerServices;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.TypeDeclaration.Records {
  namespace RecordClasses {
    [TypeDeclarationTestCase("public record class R0", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public class R0", TypeEnableRecordTypes = false)]
    public record R0(int X, string Y);

    [TypeDeclarationTestCase("public record class R1", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public class R1", TypeEnableRecordTypes = false)]
    public record class R1(int X, string Y);

    [TypeDeclarationTestCase("public record class R2", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public class R2", TypeEnableRecordTypes = false)]
    public record class R2 {
#if SYSTEM_RUNTIME_COMPILERSERVICES_REQUIREDMEMBERATTRIBUTE
      public required int X { get; set; }
      public required string Y { get; set; }
#else
      public int X { get; set; }
      public string Y { get; set; }
#endif
    }

    [TypeDeclarationTestCase("public record class REmpty", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public class REmpty", TypeEnableRecordTypes = false)]
    public record class REmpty { }

    namespace Mimicries {
      [TypeDeclarationTestCase("public record class MR0", TypeEnableRecordTypes = true)]
      [TypeDeclarationTestCase("public class MR0", TypeEnableRecordTypes = false)]
      public class MR0 {
        [CompilerGenerated] public override bool Equals(object obj) => throw new NotImplementedException();
        [CompilerGenerated] public static bool operator ==(MR0 left, MR0 right) => throw new NotImplementedException();
        [CompilerGenerated] public static bool operator !=(MR0 left, MR0 right) => throw new NotImplementedException();

        public override int GetHashCode() => throw new NotImplementedException();
      }
    }

    namespace ExtendedRecordClasses {
      [TypeDeclarationTestCase("public record class RX0", TypeEnableRecordTypes = true)]
      [TypeDeclarationTestCase("public class RX0", TypeEnableRecordTypes = false)]
      public record RX0(int X, string Y, double Z) : R0(X, Y);

      namespace Mimicries {
        [TypeDeclarationTestCase("public record class MRX0", TypeEnableRecordTypes = true)]
        [TypeDeclarationTestCase("public class MRX0", TypeEnableRecordTypes = false)]
        public class MRX0 : RecordClasses.Mimicries.MR0 {
          [CompilerGenerated] public override bool Equals(object obj) => throw new NotImplementedException();
          [CompilerGenerated] public bool Equals(R0 obj) => throw new NotImplementedException();
          [CompilerGenerated] public static bool operator ==(MRX0 left, MRX0 right) => throw new NotImplementedException();
          [CompilerGenerated] public static bool operator !=(MRX0 left, MRX0 right) => throw new NotImplementedException();

          public override int GetHashCode() => throw new NotImplementedException();
        }
      }
    }
  }

  namespace NonRecordClasses {
    [TypeDeclarationTestCase("public class NR0", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public class NR0", TypeEnableRecordTypes = false)]
    public class NR0 {
#if SYSTEM_RUNTIME_COMPILERSERVICES_REQUIREDMEMBERATTRIBUTE
      public required int X { get; init; }
      public required string Y { get; init; }
#else
      public int X { get; init; }
      public string Y { get; init; }
#endif

      public NR0(int x, string y)
      {
        X = x;
        Y = y;
      }
    }

#pragma warning disable CS0659,CS0661
    [TypeDeclarationTestCase("public class NR1", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public class NR1", TypeEnableRecordTypes = false)]
    public class NR1 {
#if SYSTEM_RUNTIME_COMPILERSERVICES_REQUIREDMEMBERATTRIBUTE
      public required int X { get; init; }
      public required string Y { get; init; }
#else
      public int X { get; init; }
      public string Y { get; init; }
#endif

      public NR1(int x, string y)
      {
        X = x;
        Y = y;
      }

      public override bool Equals(object obj) => throw new NotImplementedException();
    }

#pragma warning restore CS0659,CS0661

#pragma warning disable CS0660,CS0661
    [TypeDeclarationTestCase("public class NR2", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public class NR2", TypeEnableRecordTypes = false)]
    public class NR2 {
#if SYSTEM_RUNTIME_COMPILERSERVICES_REQUIREDMEMBERATTRIBUTE
      public required int X { get; init; }
      public required string Y { get; init; }
#else
      public int X { get; init; }
      public string Y { get; init; }
#endif

      public NR2(int x, string y)
      {
        X = x;
        Y = y;
      }

      public static bool operator ==(NR2 left, NR2 right) => throw new NotImplementedException();
      public static bool operator !=(NR2 left, NR2 right) => throw new NotImplementedException();
    }
#pragma warning restore CS0660,CS0661
  }

  namespace RecordStructs {
    [TypeDeclarationTestCase("public record struct RS0", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public struct RS0", TypeEnableRecordTypes = false)]
    public record struct RS0(int X, string Y);

    [TypeDeclarationTestCase("public record struct RS1", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public struct RS1", TypeEnableRecordTypes = false)]
    public record struct RS1 {
#if SYSTEM_RUNTIME_COMPILERSERVICES_REQUIREDMEMBERATTRIBUTE
      public required int X { get; set; }
      public required string Y { get; set; }
#else
      public int X { get; set; }
      public string Y { get; set; }
#endif
    }

    [TypeDeclarationTestCase("public record struct RSEmpty", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public struct RSEmpty", TypeEnableRecordTypes = false)]
    public record struct RSEmpty { }

    namespace Mimicries {
      [TypeDeclarationTestCase("public record struct MRS0", TypeEnableRecordTypes = true)]
      [TypeDeclarationTestCase("public struct MRS0", TypeEnableRecordTypes = false)]
      public struct MRS0 {
        [CompilerGenerated] public override bool Equals(object obj) => throw new NotImplementedException();
        [CompilerGenerated] public static bool operator ==(MRS0 left, MRS0 right) => throw new NotImplementedException();
        [CompilerGenerated] public static bool operator !=(MRS0 left, MRS0 right) => throw new NotImplementedException();

        public override int GetHashCode() => throw new NotImplementedException();
      }
    }
  }

  namespace ReadOnlyRecordStructs {
    [TypeDeclarationTestCase("public readonly record struct RRS0", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public readonly struct RRS0", TypeEnableRecordTypes = false)]
    public readonly record struct RRS0(int X, string Y);

    namespace Mimicries {
      [TypeDeclarationTestCase("public readonly record struct MRRS0", TypeEnableRecordTypes = true)]
      [TypeDeclarationTestCase("public readonly struct MRRS0", TypeEnableRecordTypes = false)]
      public readonly struct MRRS0 {
        [CompilerGenerated] public override bool Equals(object obj) => throw new NotImplementedException();
        [CompilerGenerated] public static bool operator ==(MRRS0 left, MRRS0 right) => throw new NotImplementedException();
        [CompilerGenerated] public static bool operator !=(MRRS0 left, MRRS0 right) => throw new NotImplementedException();

        public override int GetHashCode() => throw new NotImplementedException();
      }
    }
  }

  namespace NonRecordStructs {
    [TypeDeclarationTestCase("public struct NRS0", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public struct NRS0", TypeEnableRecordTypes = false)]
    public struct NRS0 {
#if SYSTEM_RUNTIME_COMPILERSERVICES_REQUIREDMEMBERATTRIBUTE
      public required int X { get; init; }
      public required string Y { get; init; }
#else
      public int X { get; init; }
      public string Y { get; init; }
#endif

      public NRS0(int x, string y)
      {
        X = x;
        Y = y;
      }
    }

#pragma warning disable IDE0251,CS0659,CS0661
    [TypeDeclarationTestCase("public struct NRS1", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public struct NRS1", TypeEnableRecordTypes = false)]
    public struct NRS1 {
#if SYSTEM_RUNTIME_COMPILERSERVICES_REQUIREDMEMBERATTRIBUTE
      public required int X { get; init; }
      public required string Y { get; init; }
#else
      public int X { get; init; }
      public string Y { get; init; }
#endif

      public NRS1(int x, string y)
      {
        X = x;
        Y = y;
      }

      public override bool Equals(object obj) => throw new NotImplementedException();
    }
#pragma warning restore IDE0251,CS0659,CS0661

#pragma warning disable CS0660,CS0661
    [TypeDeclarationTestCase("public struct NRS2", TypeEnableRecordTypes = true)]
    [TypeDeclarationTestCase("public struct NRS2", TypeEnableRecordTypes = false)]
    public struct NRS2 {
#if SYSTEM_RUNTIME_COMPILERSERVICES_REQUIREDMEMBERATTRIBUTE
      public required int X { get; init; }
      public required string Y { get; init; }
#else
      public int X { get; init; }
      public string Y { get; init; }
#endif

      public NRS2(int x, string y)
      {
        X = x;
        Y = y;
      }

      public static bool operator ==(NRS2 left, NRS2 right) => throw new NotImplementedException();
      public static bool operator !=(NRS2 left, NRS2 right) => throw new NotImplementedException();
    }
#pragma warning restore CS0660,CS0661
  }
}
