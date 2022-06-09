// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Smdn.Reflection;

internal class StructLayoutCustomAttributeData : CustomAttributeData {
  private struct DefaultLayoutStruct { }
  private static readonly StructLayoutAttribute DefaultStructLayoutAttribute = typeof(DefaultLayoutStruct).StructLayoutAttribute!;

  public StructLayoutCustomAttributeData(StructLayoutAttribute attr)
  {
    if (attr is null)
      throw new ArgumentNullException(nameof(attr));

    ConstructorArguments = Array.AsReadOnly(
      new CustomAttributeTypedArgument[] {
        new(attr.Value.GetType(), attr.Value),
      }
    );

    const BindingFlags attributeFieldBindingFlags = BindingFlags.Public | BindingFlags.Instance;
    var namedArgs = new List<CustomAttributeNamedArgument>(capacity: 3);

    if (attr.CharSet is not (CharSet.Auto or CharSet.Ansi)) {
      namedArgs.Add(
        new(
          GetStructLayoutAttributeField(nameof(StructLayoutAttribute.CharSet)),
          new(attr.CharSet.GetType(), attr.CharSet)
        )
      );
    }

    if (!(attr.Pack == 0 || attr.Pack == DefaultStructLayoutAttribute.Pack)) {
      namedArgs.Add(
        new(
          GetStructLayoutAttributeField(nameof(StructLayoutAttribute.Pack)),
          new(attr.Pack.GetType(), attr.Pack)
        )
      );
    }

    if (attr.Size != 0) {
      namedArgs.Add(
        new(
          GetStructLayoutAttributeField(nameof(StructLayoutAttribute.Size)),
          new(attr.Size.GetType(), attr.Size)
        )
      );
    }

    NamedArguments = namedArgs.AsReadOnly();

    static FieldInfo GetStructLayoutAttributeField(string name)
      => typeof(StructLayoutAttribute).GetField(name, attributeFieldBindingFlags)
        ?? throw new InvalidOperationException($"can not get field '{name}' of {nameof(StructLayoutAttribute)}");
  }

#if CAN_OVERRIDE_CUSTOMATTRIBUTEDATA_ATTRIBUTETYPE
  public override Type AttributeType => Constructor.DeclaringType!;
#endif
  public override ConstructorInfo Constructor { get; } = typeof(StructLayoutAttribute).GetConstructor(
    bindingAttr: BindingFlags.Public | BindingFlags.Instance,
    binder: null,
    types: new[] { typeof(LayoutKind) },
    modifiers: null
  )!;
  public override IList<CustomAttributeTypedArgument> ConstructorArguments { get; }
  public override IList<CustomAttributeNamedArgument> NamedArguments { get; }
}
