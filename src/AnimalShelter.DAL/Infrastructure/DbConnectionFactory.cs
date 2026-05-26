using AnimalShelter.DAL.Infrastructure.Interfaces;
using dotenv.net;
using Npgsql;

namespace AnimalShelter.DAL.Infrastructure;

public class DbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public DbConnectionFactory(IEnumMapper enumMapper)
    {
        // Recherche récursive du fichier .env pour être multi-plateforme et flexible
        var currentDir = Directory.GetCurrentDirectory();
        string envPath = "";

        // On remonte jusqu'à 5 niveaux pour trouver le dossier docker/.env
        for (int i = 0; i < 5; i++)
        {
            var potentialPath = Path.Combine(currentDir, "docker", ".env");
            if (File.Exists(potentialPath))
            {
                envPath = potentialPath;
                break;
            }
            currentDir = Directory.GetParent(currentDir)?.FullName ?? currentDir;
        }

        if (string.IsNullOrEmpty(envPath))
        {
            // Si non trouvé dans docker/, on cherche à la racine du projet
            envPath = ".env";
        }

        // Chargement du fichier .env pour les variables de la DB
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: [envPath]));

        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "Localhost";
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER");
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        var db = Environment.GetEnvironmentVariable("POSTGRES_DB");

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(db))
        {
            throw new InvalidOperationException("Database environment variables are not properly set in .env file.");
        }

        string connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={db};";

        var builder = new NpgsqlDataSourceBuilder(connectionString);

        // Mapping des enums PostgreSQL via l'EnumMapper
        enumMapper.MapEnums(builder);

        _dataSource = builder.Build();
    }

    public NpgsqlConnection CreateConnection()
    {
        return _dataSource.CreateConnection();
    }
}
