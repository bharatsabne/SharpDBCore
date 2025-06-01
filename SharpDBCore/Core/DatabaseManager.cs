using Microsoft.Data.SqlClient;
using SharpDBCore.Factories;
using SharpDBCore.Helpers;
using SharpDBCore.Interfaces;
using SharpDBCore.Loggers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDBCore.Core
{
    public sealed class DatabaseManager : IDatabaseManager
    {
        private SqlConnection? _connection;
        private SqlTransaction? _transaction;
        private IDbLogger _logger;

        private static readonly Lazy<DatabaseManager> _instance = new(() => new DatabaseManager());

        public static DatabaseManager Instance => _instance.Value;

        private DatabaseManager()
        {
            _connection = DbConnectionFactory.CreateConnection();
            _logger = new NullLogger(); 
        }

        public void SetLogger(IDbLogger logger)
        {
            _logger = logger ?? new NullLogger();
        }

        private void EnsureConnectionOpen()
        {
            if (_connection?.State != ConnectionState.Open)
                _connection?.Open();
        }

        public void BeginTransaction()
        {
            try
            {
                EnsureConnectionOpen();
                _transaction = _connection?.BeginTransaction();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error starting transaction", ex);
                throw;
            }
        }

        public void CommitTransaction()
        {
            try
            {
                if (_transaction == null)
                    throw new InvalidOperationException("No active transaction to commit.");

                _transaction.Commit();

                _transaction.Dispose();
                _transaction = null;
                _connection?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error committing transaction", ex);
                throw;
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();

                    _transaction.Dispose();
                    _transaction = null;
                    _connection?.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error rolling back transaction", ex);
                throw;
            }
        }

        public int ExecuteNonQuery(string commandText, Dictionary<string, object>? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                EnsureConnectionOpen();

                using var cmd = new SqlCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };
                if(parameters!=null)
                    SqlParameterHelper.AddParameters(cmd, parameters);

                _logger.LogInfo($"Executing NonQuery: {commandText}");

                int result = cmd.ExecuteNonQuery();

                if (_transaction == null)
                    _connection?.Close();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing NonQuery: {commandText}", ex);
                throw;
            }
        }

        public object ExecuteScalar(string commandText, Dictionary<string, object>? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                EnsureConnectionOpen();

                using var cmd = new SqlCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };
                if(parameters != null)
                    SqlParameterHelper.AddParameters(cmd, parameters);

                _logger.LogInfo($"Executing Scalar: {commandText}");

                var result = cmd.ExecuteScalar();

                if (_transaction == null)
                    _connection?.Close();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing Scalar: {commandText}", ex);
                throw;
            }
        }

        public SqlDataReader ExecuteReader(string commandText, Dictionary<string, object>? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                EnsureConnectionOpen();

                var cmd = new SqlCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };
                if(parameters != null)
                    SqlParameterHelper.AddParameters(cmd, parameters);

                _logger.LogInfo($"Executing Reader: {commandText}");

                // If no transaction, close connection when reader is closed
                return cmd.ExecuteReader(_transaction == null ? CommandBehavior.CloseConnection : CommandBehavior.Default);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing Reader: {commandText}", ex);
                throw;
            }
        }
        public DataTable ExecuteDataTable(string commandText, Dictionary<string, object>? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                EnsureConnectionOpen();

                using var cmd = new SqlCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };

                if (parameters != null)
                    SqlParameterHelper.AddParameters(cmd, parameters);

                _logger.LogInfo($"Executing DataTable query: {commandText}");

                using var adapter = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                if (_transaction == null)
                    _connection?.Close();

                return dataTable;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing DataTable query: {commandText}", ex);
                throw;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();

            if (_connection != null)
            {
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();

                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
