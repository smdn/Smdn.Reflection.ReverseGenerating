// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if WORKAROUND_NULLABILITYINFO_BYREFTYPE
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Smdn.Reflection;

// Workaround: The pseudo ParameterInfo type which unwraps 'ByRef' type to its element type
// See https://github.com/dotnet/runtime/issues/72320
internal sealed class UnwrapByRefParameterInfo : ParameterInfo {
  public ParameterInfo BaseParameter { get; }

  public UnwrapByRefParameterInfo(ParameterInfo baseParameter)
  {
#if DEBUG
    if (!baseParameter.ParameterType.IsByRef)
      throw new ArgumentException($"{baseParameter.ParameterType} must be by-ref");
#endif

    BaseParameter = baseParameter;
  }

  public override MemberInfo Member => BaseParameter.Member;
  public override Type ParameterType => BaseParameter.ParameterType.GetElementType()!;
  public override IList<CustomAttributeData> GetCustomAttributesData()
    => BaseParameter.GetCustomAttributesData();
}
#endif
