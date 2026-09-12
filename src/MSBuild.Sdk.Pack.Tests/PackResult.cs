using System.IO.Compression;

namespace MSBuild.Sdk.Pack.Tests;

public record PackResult(bool Success, string Output, string Error, string OutputDirectory)
{
    public string CombinedOutput { get; } =
        $"""
         {Output}
         {Error}
         """;

    public bool PackageContains(string packageId, string path)
    {
        var packagePath = Path.Combine(OutputDirectory, $"{packageId}.1.0.0.nupkg");

        using var archive = ZipFile.OpenRead(packagePath);

        return archive.Entries.Any(x => x.FullName == path);
    }
}
