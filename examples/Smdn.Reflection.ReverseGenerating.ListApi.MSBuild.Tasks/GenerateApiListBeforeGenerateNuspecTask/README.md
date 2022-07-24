This directory demonstrates generating an API list before generating `.nuspec` file of a single targeting project.

# Steps to run this example
1. Run `dotnet restore`
2. Run `dotnet pack`, and the API list will be generated to the directory `api-list`.

You can configure the directory where the API listings will be generated by specifying `ApiListOutputBaseDirectory` project property.