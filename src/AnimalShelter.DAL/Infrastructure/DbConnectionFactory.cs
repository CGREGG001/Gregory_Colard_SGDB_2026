using System.Data;
using dotenv.net;
using Npgsql;

namespace AnimalShelter.DAL.Infrastructure;

public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory()
    {
        // Chargement du fichier .env pour les variables de la DB (se trouve dans ./docker/.env).
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { "../../docker/.env" }));

        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "Localhost";
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER");
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        var db = Environment.GetEnvironmentVariable("POSTGRES_DB");

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(db))
        {
            throw new InvalidOperationException("Database environment variables are not properly set in .env file.");
        }

        _connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={db};";
    }

    // Crée et retourne une nouvelle connexion vers PostgreSQL pour instancier une NpgsqlConnection.
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
