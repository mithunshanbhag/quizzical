using CliWrap;
using CliWrap.Buffered;

namespace Quizzical.E2ETests;

internal static class QuizzicalCliRunner
{
    public static async Task<BufferedCommandResult> RunAsync(string configurationJson, CancellationToken cancellationToken)
    {
        var repositoryRoot = GetRepositoryRoot();
        var configurationPath = await WriteConfigurationFileAsync(configurationJson, cancellationToken);

        try
        {
            var appAssemblyPath = Path.Combine(repositoryRoot, "src", "bin", "Debug", "net10.0", "Quizzical.dll");

            if (!File.Exists(appAssemblyPath))
                throw new FileNotFoundException("The Quizzical app assembly was not found. Ensure the app project has been built before running E2E tests.", appAssemblyPath);

            return await Cli.Wrap("dotnet")
                .WithWorkingDirectory(repositoryRoot)
                .WithArguments([appAssemblyPath])
                .WithEnvironmentVariables(new Dictionary<string, string?>
                {
                    ["Quizzical__TestConfigurationPath"] = configurationPath
                })
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync(cancellationToken);
        }
        finally
        {
            if (File.Exists(configurationPath))
                File.Delete(configurationPath);
        }
    }

    private static string GetRepositoryRoot()
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
    }

    private static async Task<string> WriteConfigurationFileAsync(string configurationJson, CancellationToken cancellationToken)
    {
        var configurationPath = Path.Combine(
            Path.GetTempPath(),
            $"quizzical-e2e-{Guid.NewGuid():N}.json");

        await File.WriteAllTextAsync(configurationPath, configurationJson, cancellationToken);
        return configurationPath;
    }
}