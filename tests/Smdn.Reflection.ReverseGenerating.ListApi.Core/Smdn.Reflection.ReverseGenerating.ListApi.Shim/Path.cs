// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

namespace Smdn.Reflection.ReverseGenerating.ListApi.Shim;

#if !SYSTEM_IO_PATH_JOIN
internal class Path {
  public static string Join(params string[] paths)
    => System.IO.Path.Combine(paths);

  public static string Join(string path1, string path2)
    => System.IO.Path.Combine(path1, path2);

  public static string Join(string path1, string path2, string path3)
    => System.IO.Path.Combine(path1, path2, path3);

  public static string Join(string path1, string path2, string path3, string path4)
    => System.IO.Path.Combine(path1, path2, path3, path4);
}
#endif
