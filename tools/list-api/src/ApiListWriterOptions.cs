using Smdn.Reflection.ReverseGenerating;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

class ApiListWriterOptions : GeneratorOptions {
  public bool WriterOrderStaticMembersFirst { get; set; } = false;
}