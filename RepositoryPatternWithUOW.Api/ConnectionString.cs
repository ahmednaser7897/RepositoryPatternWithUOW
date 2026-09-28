using Microsoft.Extensions.Configuration;
namespace RepositoryPatternWithUOW.Api;

internal static class ConnectionString
{
    public static string LoadConnectionString()
    {
        var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        return connectionString ?? "";
    }
}

