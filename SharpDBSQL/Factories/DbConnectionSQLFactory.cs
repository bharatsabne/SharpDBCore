using Microsoft.Data.SqlClient;

namespace SharpDBCore.Factories
{
    public static class DbConnectionSQLFactory
    {
        private static string? _connectionString;
        public static void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
        public static SqlConnection CreateConnection()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException("Default connection string not set. Call SetConnectionString first.");

            return new SqlConnection(_connectionString);
        }
        /// <summary>
        /// Any other database connection string can be created using this method. which allows you to pass a connection string directly.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static SqlConnection CreateConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not set or missing from configuration.");
            }

            return new SqlConnection(connectionString);
        }
    }
}
