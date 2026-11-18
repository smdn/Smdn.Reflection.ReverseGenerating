// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1822

using System.Threading.Tasks;

// The following test cases require the assembly to be built with "runtime-async=on".
namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.RuntimeAsync;

public class AsyncVoidMethods {
  [MemberDeclarationTestCase("public async void AsyncVoid() {}")]
  public async void AsyncVoid() { await Task.Delay(0); }
}

public class AsyncTaskMethods {
  [MemberDeclarationTestCase("public async System.Threading.Tasks.Task AsyncTask() {}")]
  public async Task AsyncTask() { await Task.Delay(0); }

  [MemberDeclarationTestCase("public async System.Threading.Tasks.Task<int> AsyncTaskOfInt() {}")]
  public async Task<int> AsyncTaskOfInt() { await Task.Delay(0); return default; }

  [MemberDeclarationTestCase("public System.Threading.Tasks.Task NonAsyncTask() {}")]
  public Task NonAsyncTask() => Task.Delay(0);

  [MemberDeclarationTestCase("public System.Threading.Tasks.Task<int> NonAsyncTaskOfInt() {}")]
  public Task<int> NonAsyncTaskOfInt() => Task.FromResult<int>(default);
}

public class AsyncValueTaskMethods {
  [MemberDeclarationTestCase("public async System.Threading.Tasks.ValueTask AsyncValueTask() {}")]
  public async ValueTask AsyncValueTask() { await Task.Delay(0); }

  [MemberDeclarationTestCase("public async System.Threading.Tasks.ValueTask<int> AsyncValueTaskOfInt() {}")]
  public async ValueTask<int> AsyncValueTaskOfInt() { await Task.Delay(0); return 0; }

  [MemberDeclarationTestCase("public System.Threading.Tasks.ValueTask NonAsyncValueTask() {}")]
  public ValueTask NonAsyncValueTask() => new ValueTask();

  [MemberDeclarationTestCase("public System.Threading.Tasks.ValueTask<int> NonAsyncValueTaskOfInt() {}")]
  public ValueTask<int> NonAsyncValueTaskOfInt() => new ValueTask<int>(default(int));
}
