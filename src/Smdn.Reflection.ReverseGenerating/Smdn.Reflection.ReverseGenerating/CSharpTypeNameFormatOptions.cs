// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;
using System.Text;

namespace Smdn.Reflection.ReverseGenerating;

internal readonly record struct CSharpTypeNameFormatOptions(
  ICustomAttributeProvider AttributeProvider,
  bool WithNamespace,
  bool WithDeclaringTypeName,
  bool TranslateLanguagePrimitiveType,
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
  NullabilityInfoContext? NullabilityInfoContext = null,
#endif
  Action<Type, StringBuilder>? PrependGenericParameterAttributes = null,
  bool OmitAttributeSuffix = false,
  bool AsUnboundTypeName = false,
  bool WithGenericParameterVariance = false
);
