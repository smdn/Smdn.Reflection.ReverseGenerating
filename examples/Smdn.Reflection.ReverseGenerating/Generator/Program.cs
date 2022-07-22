using System.Reflection;
using Smdn.Reflection.ReverseGenerating;

var options = new GeneratorOptions();

// Generates the type declaration of int(Int32)
Console.WriteLine(Generator.GenerateTypeDeclaration(typeof(int), null, options));
Console.WriteLine();

// Generates the type declaration of Func<T1, T2, T3, TResult>
Console.WriteLine(Generator.GenerateTypeDeclaration(typeof(Func<,,,>), null, options));
Console.WriteLine();

// Generates the type declaration of Dictionary<TKey, TValue>
Console.WriteLine(Generator.GenerateTypeDeclaration(typeof(Dictionary<,>), null, options));
Console.WriteLine();

// Generates the base interfaces of Dictionary<TKey, TValue>
options.TypeDeclaration.WithNamespace = true;

foreach (var decl in Generator.GenerateExplicitBaseTypeAndInterfaces(typeof(Dictionary<,>), null, options)) {
  Console.WriteLine(decl);
}
Console.WriteLine();

// Generates the type declaration of Dictionary<,> with base type, interfaces and generic parameter constraints
foreach (var decl in Generator.GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(typeof(Stream), null, options)) {
  Console.WriteLine(decl);
}
Console.WriteLine();

// Generates the member declaration of int.TryParse
var intTryParse = typeof(int).GetMethods().First(m => m.Name == "TryParse");

Console.WriteLine(Generator.GenerateMemberDeclaration(intTryParse, null, options));

options.MemberDeclaration.MethodBody = MethodBodyOption.ThrowNull;
options.MemberDeclaration.WithDeclaringTypeName = true;
options.AttributeDeclaration.TypeFilter = (Type attributeType, ICustomAttributeProvider attributeProvider) => false;

Console.WriteLine(Generator.GenerateMemberDeclaration(intTryParse, null, options));

options.MemberDeclaration.MethodBody = MethodBodyOption.None;
options.MemberDeclaration.NullabilityInfoContext = null;

Console.WriteLine(Generator.GenerateMemberDeclaration(intTryParse, null, options));
Console.WriteLine();

// Generates the attribute declarations of List<>
options.AttributeDeclaration.TypeFilter = (Type attributeType, ICustomAttributeProvider attributeProvider) => true;

Console.WriteLine(string.Join(", ", Generator.GenerateAttributeList(typeof(List<>), null, options)));

options.AttributeDeclaration.WithNamespace = true;
options.AttributeDeclaration.OmitAttributeSuffix = false;

Console.WriteLine(string.Join(", ", Generator.GenerateAttributeList(typeof(List<>), null, options)));

Console.WriteLine();
