// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Reflection;

namespace Smdn.Reflection.ReverseGenerating;

public delegate bool AttributeTypeFilter(Type type, ICustomAttributeProvider attributeProvider);
