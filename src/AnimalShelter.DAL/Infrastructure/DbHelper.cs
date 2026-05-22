using Npgsql;

namespace AnimalShelter.DAL.Infrastructure
{
    public static class DbHelper
    {
        /// <summary>
        /// Exécute une requête SQL et retourne un seul enregistrement typé.
        /// </summary>
        /// <typeparam name="T">Type de l'objet retourné.</typeparam>
        /// <param name="factory">Factory fournissant la connexion PostgreSQL.</param>
        /// <param name="sql">Requête SQL à exécuter.</param>
        /// <param name="bind">Action permettant de binder les paramètres sur la commande.</param>
        /// <param name="map">Fonction de mapping du NpgsqlDataReader vers l'objet T.</param>
        /// <returns>
        /// L'objet T si un enregistrement est trouvé, sinon null.
        /// </returns>
        public static async Task<T?> QuerySingleAsync<T>(DbConnectionFactory factory,
        string sql, Action<NpgsqlCommand> bind, Func<NpgsqlDataReader, T> map)
        {
            await using var connection = factory.CreateConnection();
            await using var cmd = new NpgsqlCommand(sql, connection);

            bind(cmd);

            await using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? map(reader) : default;
        }

        /// <summary>
        /// Exécute une requête SQL et retourne une liste typée d'objets.
        /// </summary>
        /// <typeparam name="T">Type des objets retournés.</typeparam>
        /// <param name="factory">Factory fournissant la connexion PostgreSQL.</param>
        /// <param name="sql">Requête SQL à exécuter.</param>
        /// <param name="bind">Action optionnelle pour binder les paramètres.</param>
        /// <param name="map">Fonction de mapping du NpgsqlDataReader vers T.</param>
        /// <returns>
        /// Une liste d'objets T (peut être vide).
        /// </returns>
        public static async Task<List<T>> QueryListAsync<T>(DbConnectionFactory factory,
        string sql, Action<NpgsqlCommand>? bind, Func<NpgsqlDataReader, T> map)
        {
            var list = new List<T>();

            await using var connection = factory.CreateConnection();
            await using var cmd = new NpgsqlCommand(sql, connection);

            bind?.Invoke(cmd);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(map(reader));

            return list;
        }

        /// <summary>
        /// Exécute une requête SQL et retourne une valeur scalaire (ex: ID généré).
        /// </summary>
        /// <param name="factory">Factory fournissant la connexion PostgreSQL.</param>
        /// <param name="sql">Requête SQL à exécuter.</param>
        /// <param name="bind">Action permettant de binder les paramètres.</param>
        /// <returns>
        /// La valeur retournée par la requête (ex: un ID).
        /// </returns>
        public static async Task<object?> ExecuteScalarAsync(DbConnectionFactory factory,
            string sql, Action<NpgsqlCommand> bind)
        {
            await using var connection = factory.CreateConnection();
            await using var cmd = new NpgsqlCommand(sql, connection);

            bind(cmd);

            return await cmd.ExecuteScalarAsync();
        }

        /// <summary>
        /// Exécute une commande SQL ne retournant pas de résultat (UPDATE, DELETE).
        /// </summary>
        /// <param name="factory">Factory fournissant la connexion PostgreSQL.</param>
        /// <param name="sql">Commande SQL à exécuter.</param>
        /// <param name="bind">Action permettant de binder les paramètres.</param>
        /// <returns>
        /// Le nombre de lignes affectées.
        /// </returns>
        public static async Task<int> ExecuteNonQueryAsync(DbConnectionFactory factory,
            string sql, Action<NpgsqlCommand> bind)
        {
            await using var connection = factory.CreateConnection();
            await using var cmd = new NpgsqlCommand(sql, connection);

            bind(cmd);

            return await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Convertit une valeur C# nullable en valeur compatible DB (DBNull.Value).
        /// </summary>
        /// <param name="value">Valeur potentiellement null.</param>
        /// <returns>
        /// La valeur d'origine ou DBNull.Value.
        /// </returns>
        public static object DbValue(object? value)
        {
            return value ?? DBNull.Value;
        }
    }
}
