using Smdn.Reflection.ReverseGenerating;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class ApiListWriterOptions : GeneratorOptions {
  public bool WriterOrderStaticMembersFirst { get; set; } = false;
}