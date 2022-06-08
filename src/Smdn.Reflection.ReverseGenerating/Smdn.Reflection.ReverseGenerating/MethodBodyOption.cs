// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
namespace Smdn.Reflection.ReverseGenerating;

public enum MethodBodyOption {
  None,
  EmptyImplementation,
  ThrowNotImplementedException,
  // TODO: throw null
}
