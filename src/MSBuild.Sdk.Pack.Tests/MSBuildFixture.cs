using static SimpleExec.Command;

namespace MSBuild.Sdk.Pack.Tests;

public class MSBuildFixture : IDisposable
{
    private static readonly string SdkTargetsFile = FindTargetsFile();

    private readonly string rootDirectory = Path.Combine(Path.GetTempPath(), "msbuild-tests", Guid.NewGuid().ToString("N"));

    public string WriteReferenceProject(string name, string targetFramework)
    {
        return WriteProject(
            name,
            $"""
             <Project Sdk="Microsoft.NET.Sdk">
               <PropertyGroup>
                 {targetFramework}
               </PropertyGroup>
             </Project>
             """);
    }

    public string WriteConsumingProject(string name, string targetFramework, string referenceProjectPath, string referenceAttributes = "")
    {
        return WriteProject(
            name,
            $"""
             <Project Sdk="Microsoft.NET.Sdk">
               <PropertyGroup>
                 {targetFramework}
                 <PackageId>{name}</PackageId>
                 <Version>1.0.0</Version>
               </PropertyGroup>
               <ItemGroup>
                 <ProjectReference Include="{referenceProjectPath}" Pack="true" {referenceAttributes} />
               </ItemGroup>
               <Import Project="{SdkTargetsFile}" />
             </Project>
             """);
    }

    public async Task<PackResult> Pack(string projectPath)
    {
        var outputDirectory = Path.Combine(rootDirectory, "out", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDirectory);

        var success = true;

        bool HandleExitCode(int exitCode)
        {
            success = exitCode == 0;

            return true;
        }

        var (output, error) = await ReadAsync("dotnet", $"pack {projectPath} -o {outputDirectory} -c Release", handleExitCode: HandleExitCode);

        return new PackResult(success, output, error, outputDirectory);
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(rootDirectory, true);
        }
        catch (IOException)
        {
        }
    }

    private string WriteProject(string name, string content)
    {
        var directory = Path.Combine(rootDirectory, name);
        Directory.CreateDirectory(directory);

        var projectPath = Path.Combine(directory, $"{name}.csproj");
        File.WriteAllText(projectPath, content);

        return projectPath;
    }

    private static string FindTargetsFile()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var file = Path.Combine(directory.FullName, "src", "MSBuild.Sdk.Pack", "build", "MSBuild.Sdk.Pack.targets");

            if (File.Exists(file))
            {
                return file;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException("Targets file not found");
    }
}
