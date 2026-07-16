using Microsoft.Data.Sqlite;

namespace HealthCare.Claims.DocumentsService.Infrastructure.Persistence;

public static class DatabasePathResolver
{
    public static string ResolveSqliteConnectionString(string? connectionString)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString ?? "Data Source=.local-data/documents-service/documents.db");

        if (!string.IsNullOrWhiteSpace(builder.DataSource) &&
            !builder.DataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase) &&
            !Path.IsPathRooted(builder.DataSource))
        {
            var repositoryRoot = FindRepositoryRoot(Directory.GetCurrentDirectory())
                ?? FindRepositoryRoot(AppContext.BaseDirectory)
                ?? Directory.GetCurrentDirectory();

            builder.DataSource = Path.GetFullPath(Path.Combine(repositoryRoot, builder.DataSource));
        }

        return builder.ToString();
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
