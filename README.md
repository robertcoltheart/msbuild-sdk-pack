# MSBuild.Sdk.Pack

[![NuGet](https://img.shields.io/nuget/v/MSBuild.Sdk.Pack?style=for-the-badge)](https://www.nuget.org/packages/MSBuild.Sdk.Pack) [![License](https://img.shields.io/github/license/robertcoltheart/msbuild-sdk-pack?style=for-the-badge)](https://github.com/robertcoltheart/msbuild-sdk-pack/blob/master/LICENSE)

This package provides MSBuild tasks to package multiple projects into a single NuGet package.

## Usage
Install the package from NuGet with `dotnet add package MSBuild.Sdk.Pack`.

In your project reference, you can add `Pack="true"` to include the referenced project in your NuGet package, as below. You should
also add `PrivateAssets="true"` to ensure NuGet does not add your project as a NuGet dependency.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <!-- Pack the referenced project output to NuGet and add PrivateAssets=true to remove the dependency -->
    <ProjectReference Include="..\ClassLibrary\ClassLibrary.csproj"
                      PrivateAssets="true"
                      Pack="true" />
  </ItemGroup>

</Project>
```

By default, the referenced project and any XML documentation files will be added to the same path in your NuGet packed as the source package.

You can alternatively specify `ProjectPath="path/in/package"` to customize the output of the referenced library, which is useful for source generators or analyzers.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <!-- Add a source generator library to this NuGet package -->
    <ProjectReference Include="..\SourceGenerator\SourceGenerator.csproj"
                      PrivateAssets="true"
                      Pack="true"
                      PackagePath="analyzers/dotnet/cs" />
  </ItemGroup>

</Project>
```

## Get in touch
Raise an [issue here](https://github.com/robertcoltheart/msbuild-sdk-pack/issues).

## Contributing
Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on how to contribute to this project.

## License
Burpless is released under the [MIT License](LICENSE)
