// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using NUnit.Framework;

#if SYSTEM_IO_PATH_JOIN
using PathJoiner = System.IO.Path;
#else
using PathJoiner = Smdn.Reflection.ReverseGenerating.ListApi.Shim.Path;
#endif

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
class ProjectFinderTests {
  [TestCase("Lib", "Lib.csproj")]
  [TestCase("Exe", "Exe.csproj")]
  [TestCase("ExeVB", "ExeVB.vbproj")]
  [TestCase("ExeFSharp", "ExeFSharp.fsproj")]
  [TestCase("Solution", "Solution.sln")]
  public void FindSingleProjectOrSolution(string directoryName, string expectedProjectFileName)
  {
    var project = ProjectFinder.FindSingleProjectOrSolution(
      new(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, directoryName))
    );

    Assert.IsNotNull(project, nameof(project));
    Assert.AreEqual(project.Name, expectedProjectFileName, nameof(project.Name));
  }

  [Test]
  public void FindSingleProjectOrSolution_SearchTopDirectoryOnly()
  {
    Assert.Throws<FileNotFoundException>(() => {
      ProjectFinder.FindSingleProjectOrSolution(
        new(TestAssemblyInfo.RootDirectory.FullName)
      );
    });
  }

  [Test]
  public void FindSingleProjectOrSolution_MultipleFileFound()
  {
    Assert.Throws<InvalidOperationException>(() => {
      ProjectFinder.FindSingleProjectOrSolution(
        new(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "MultipleProj"))
      );
    });
  }
}
