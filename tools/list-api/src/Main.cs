using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime.Versioning;

using Smdn.Reflection.ReverseGenerating;

class ListApiMain {
  static void Main(string[] args)
  {
    var libs = new List<string>();
    var options = new ApiListWriterOptions();
    var showUsage = false;
    var stdout = false;

    foreach (var arg in args) {
      switch (arg) {
        case "--generate-impl":
          options.MemberDeclarationMethodBody = MethodBodyOption.ThrowNotImplementedException;
          break;

        case "--generate-fullname":
          options.TypeDeclarationWithNamespace = true;
          options.MemberDeclarationWithNamespace = true;
          break;

        case "--stdout":
          stdout = true;
          break;

        case "/?":
        case "-h":
        case "--help":
          showUsage = true;
          break;

        default: {
          libs.Add(arg);
          break;
        }
      }
    }

    if (showUsage) {
      Console.Error.WriteLine("--stdout: output to stdout");
      Console.Error.WriteLine("--generate-fullname: generate type and member declaration with full type name");
      Console.Error.WriteLine("--generate-impl: generate with empty implementation");
      Console.Error.WriteLine();
      return;
    }

    foreach (var lib in libs) {
      Assembly assm = null;

      try {
        Console.Error.WriteLine($"loading {lib}");

        //assm = Assembly.ReflectionOnlyLoadFrom(lib);
        assm = Assembly.LoadFrom(lib);

        Console.Error.WriteLine($"loaded '{assm.FullName}'");
      }
      catch (Exception ex) {
        Console.Error.WriteLine($"error: cannot load: {lib}");
        Console.Error.WriteLine(ex);
      }

      if (assm == null)
        continue;

      var frameworkName = assm.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
      var assemblyIdentifier = (frameworkName == null)
        ? assm.GetCustomAttribute<AssemblyProductAttribute>()?.Product
        : $"{assm.GetName().Name}-v{assm.GetName().Version}-{frameworkName}";

      var defaultOutputFileName = $"{assemblyIdentifier}.apilist.cs";

      using (Stream outputStream = stdout ? Console.OpenStandardOutput() : File.Open(defaultOutputFileName, FileMode.Create)) {
        using (var outputWriter = new StreamWriter(outputStream, stdout ? Console.OutputEncoding : new UTF8Encoding(false))) {
          var writer = new ApiListWriter(outputWriter, assm, options);

          writer.WriteAssemblyInfoHeader();

          writer.WriteExportedTypes();
        }
      }

      Console.Error.WriteLine("done");
    }
  }
}

