namespace HealthCare.Claims.ModularMonolith.Api.IntegrationEvents;

public static class BrokerPathResolver
{
    public static string ResolveQueueDirectory(string configuredPath)
    {
        if (Path.IsPathRooted(configuredPath))
        {
            return Path.GetFullPath(configuredPath);
        }

        var repositoryRoot = FindRepositoryRoot(Directory.GetCurrentDirectory())
            ?? FindRepositoryRoot(AppContext.BaseDirectory);

        return Path.GetFullPath(Path.Combine(repositoryRoot ?? Directory.GetCurrentDirectory(), configuredPath));
    }

    private static string? FindRepositoryRoot(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
