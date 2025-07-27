using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDBLight.Factories
{
    public static class DbConnectionLightFactory
    {
        private static string? _connectionString;
        public static void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
        public static SQLiteConnection CreateConnection()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException("Default connection string not set. Call SetConnectionString first.");

            return new SQLiteConnection(_connectionString);
        }
        /// <summary>
        /// Any other database connection string can be created using this method. which allows you to pass a connection string directly.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static SQLiteConnection CreateConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not set or missing from configuration.");
            }
            _connectionString = connectionString;
            return new SQLiteConnection(connectionString);
        }
    }
}
