namespace MSBuild.Sdk.Pack.Tests;

public class PackProjectReferenceTests
{
    [Test]
    public async Task PacksUsingSingleTargetFramework()
    {
        using var fixture = new MSBuildFixture();

        var libPath = fixture.WriteReferenceProject("Lib", "<TargetFramework>net10.0</TargetFramework>");
        var appPath = fixture.WriteConsumingProject("App", "<TargetFramework>net10.0</TargetFramework>", "../Lib/Lib.csproj");

        var result = await fixture.Pack(appPath);

        await Assert.That(result.PackageContains("App", "lib/net10.0/Lib.dll")).IsTrue();
    }

    [Test]
    public async Task MismatchedTargetFrameworkFails()
    {
        using var fixture = new MSBuildFixture();

        var libPath = fixture.WriteReferenceProject("Lib", "<TargetFramework>net8.0</TargetFramework>");
        var appPath = fixture.WriteConsumingProject("App", "<TargetFramework>net10.0</TargetFramework>", "../Lib/Lib.csproj");

        var result = await fixture.Pack(appPath);

        await Assert.That(result.Success).IsFalse().Because(result.CombinedOutput);
        await Assert.That(result.CombinedOutput).Contains("targets 'net8.0'");
        await Assert.That(result.CombinedOutput).Contains("this project's target framework 'net10.0'");
        await Assert.That(result.CombinedOutput).Contains("SetTargetFramework");
    }

    [Test]
    public async Task MismatchedMultipleTargetFrameworksFails()
    {
        using var fixture = new MSBuildFixture();

        var libPath = fixture.WriteReferenceProject("Lib", "<TargetFrameworks>net8.0;netstandard2.0</TargetFrameworks>");
        var appPath = fixture.WriteConsumingProject("App", "<TargetFramework>net10.0</TargetFramework>", "../Lib/Lib.csproj");

        var result = await fixture.Pack(appPath);

        await Assert.That(result.Success).IsFalse().Because(result.CombinedOutput);
        await Assert.That(result.CombinedOutput).Contains("targets 'net8.0;netstandard2.0'");
        await Assert.That(result.CombinedOutput).Contains("this project's target framework 'net10.0'");
    }

    [Test]
    public async Task PacksUsingOneMatchingTargetFramework()
    {
        using var fixture = new MSBuildFixture();

        var libPath = fixture.WriteReferenceProject("Lib", "<TargetFrameworks>net8.0;net10.0</TargetFrameworks>");
        var appPath = fixture.WriteConsumingProject("App", "<TargetFramework>net10.0</TargetFramework>", "../Lib/Lib.csproj");

        var result = await fixture.Pack(appPath);

        await Assert.That(result.PackageContains("App", "lib/net10.0/Lib.dll")).IsTrue();
    }

    [Test]
    public async Task PacksAllMatchingTargetFramework()
    {
        using var fixture = new MSBuildFixture();

        var libPath = fixture.WriteReferenceProject("Lib", "<TargetFrameworks>net8.0;net10.0</TargetFrameworks>");
        var appPath = fixture.WriteConsumingProject("App", "<TargetFrameworks>net10.0;net8.0</TargetFrameworks>", "../Lib/Lib.csproj");

        var result = await fixture.Pack(appPath);

        await Assert.That(result.PackageContains("App", "lib/net8.0/Lib.dll")).IsTrue();
        await Assert.That(result.PackageContains("App", "lib/net10.0/Lib.dll")).IsTrue();
    }

    [Test]
    public async Task CanPackProjectRefernceWithOverride()
    {
        using var fixture = new MSBuildFixture();

        var libPath = fixture.WriteReferenceProject("Lib", "<TargetFramework>net8.0</TargetFramework>");
        var appPath = fixture.WriteConsumingProject(
            "App",
            "<TargetFramework>net10.0</TargetFramework>",
            "../Lib/Lib.csproj",
            """SetTargetFramework="TargetFramework=net8.0" """);

        var result = await fixture.Pack(appPath);

        await Assert.That(result.PackageContains("App", "lib/net10.0/Lib.dll")).IsTrue();
    }
}
