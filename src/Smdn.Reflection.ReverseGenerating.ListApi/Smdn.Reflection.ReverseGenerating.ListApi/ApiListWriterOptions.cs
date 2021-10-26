using Smdn.Reflection.ReverseGenerating;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class ApiListWriterOptions : GeneratorOptions {
  public WriterOptions Writer { get; } = new();

  public class WriterOptions {
    public bool OrderStaticMembersFirst { get; set; }= false;
  }
}