// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;

namespace Smdn.Reflection;

// ref: https://github.com/dotnet/roslyn/blob/main/docs/features/nullable-metadata.md
internal enum NullableMetadataValue : byte {
  Oblivious = 0,
  NotAnnotated = 1,
  Annotated = 2,
}
