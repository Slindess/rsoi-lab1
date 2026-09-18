using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonApp.Core.Interfaces.Repositories;
using PersonApp.DataAccess.Database;
using PersonApp.DataAccess.Repositories;

namespace PersonApp.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = GetPostgresConnectionString(configuration);

        services.AddDbContext<PersonsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPersonRepository, PersonRepository>();

        return services;
    }

    private static string GetPostgresConnectionString(IConfiguration configuration)
    {
        var configured = configuration.GetConnectionString("Postgres");
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured;
        }

        var databaseUrl = configuration["DATABASE_URL"];
        if (!string.IsNullOrWhiteSpace(databaseUrl))
        {
            return ConvertDatabaseUrl(databaseUrl);
        }

        var host = configuration["PGHOST"];
        var port = configuration["PGPORT"];
        var database = configuration["PGDATABASE"];
        var user = configuration["PGUSER"];
        var password = configuration["PGPASSWORD"];

        if (!string.IsNullOrWhiteSpace(host) &&
            !string.IsNullOrWhiteSpace(port) &&
            !string.IsNullOrWhiteSpace(database) &&
            !string.IsNullOrWhiteSpace(user) &&
            !string.IsNullOrWhiteSpace(password))
        {
            return $"Host={host};Port={port};Database={database};Username={user};Password={password}";
        }

        throw new InvalidOperationException("Postgres connection settings are not configured.");
    }

    private static string ConvertDatabaseUrl(string databaseUrl)
    {
        var uri = new Uri(databaseUrl);
        var credentials = uri.UserInfo.Split(':', 2);

        if (credentials.Length != 2)
        {
            throw new InvalidOperationException("DATABASE_URL must contain username and password.");
        }

        return string.Join(';', new[]
        {
            $"Host={uri.Host}",
            $"Port={uri.Port}",
            $"Database={uri.AbsolutePath.TrimStart('/')}",
            $"Username={Uri.UnescapeDataString(credentials[0])}",
            $"Password={Uri.UnescapeDataString(credentials[1])}",
            "SSL Mode=Require",
            "Trust Server Certificate=true"
        });
    }
}
