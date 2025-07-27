using SharpDBLight.Factories;
using SharpDBLight.Healpers;
using SharpDBLight.Interfaces;
using SharpDBLight.Loggers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDBLight.Core
{
    public sealed class DatabaseLightManager : IDatabaseManager
    {
        private SQLiteConnection? _connection;
        private SQLiteTransaction? _transaction;
        private IDbLogger _logger;
        private static readonly Lazy<DatabaseLightManager> _instance = new(() => new DatabaseLightManager());
        public static DatabaseLightManager Instance => _instance.Value;
        private DatabaseLightManager()
        {
            _connection = DbConnectionLightFactory.CreateConnection();
            _logger = new NullLogger();
        }
        public void SetLogger(IDbLogger logger)
        {
            _logger = logger ?? new NullLogger();
        }
        /// <summary>
        /// Gets the currently configured database logger used internally by the <see cref="DatabaseManager"/>.
        /// This logger is intended solely for logging database-related operations, such as executing commands,
        /// managing transactions, and handling connection events.
        ///
        /// If no custom logger is explicitly set via <see cref="SetLogger(IDbLogger)"/>, a default <see cref="NullLogger"/> is used.
        /// The <see cref="NullLogger"/> implementation writes minimal diagnostic information to the Output window (Debug),
        /// but does not persist logs or perform structured logging.
        ///
        /// This property is not intended for general-purpose application logging. For that use case,
        /// a separate logging mechanism should be implemented outside of the database library.
        /// </summary>
        public IDbLogger Logger => _logger;
        private void EnsureConnectionOpen()
        {
            if (_connection?.State != ConnectionState.Open)
                _connection?.Open();
        }
        private async Task EnsureConnectionOpenAsync(CancellationToken cancellationToken = default)
        {
            if (_connection?.State != ConnectionState.Open)
                await _connection!.OpenAsync(cancellationToken);
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
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureConnectionOpenAsync(cancellationToken);
                _transaction = (SQLiteTransaction?)await _connection!.BeginTransactionAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error starting async transaction", ex);
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
        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_transaction == null)
                    throw new InvalidOperationException("No active transaction to commit.");

                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
                if (_connection?.State == ConnectionState.Open)
                    await _connection.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error committing async transaction", ex);
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
        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                    if (_connection?.State == ConnectionState.Open)
                        await _connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error rolling back async transaction", ex);
                throw;
            }
        }
        public int ExecuteNonQuery(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                EnsureConnectionOpen();

                using var cmd = new SQLiteCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };
                if (parameters != null)
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
        public async Task<int> ExecuteNonQueryAsync(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureConnectionOpenAsync(cancellationToken);
                await using var cmd = new SQLiteCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };
                if (parameters != null)
                    SqlParameterHelper.AddParameters(cmd, parameters);

                _logger.LogInfo($"Executing Async NonQuery: {commandText}");

                var result = await cmd.ExecuteNonQueryAsync(cancellationToken);

                if (_transaction == null)
                    await _connection!.CloseAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing Async NonQuery: {commandText}", ex);
                throw;
            }
        }
        public object ExecuteScalar(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                EnsureConnectionOpen();

                using var cmd = new SQLiteCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };
                if (parameters != null)
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
        public async Task<object?> ExecuteScalarAsync(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureConnectionOpenAsync(cancellationToken);
                await using var cmd = new SQLiteCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };
                if (parameters != null)
                    SqlParameterHelper.AddParameters(cmd, parameters);

                _logger.LogInfo($"Executing Async Scalar: {commandText}");

                var result = await cmd.ExecuteScalarAsync(cancellationToken);

                if (_transaction == null)
                    await _connection!.CloseAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing Async Scalar: {commandText}", ex);
                throw;
            }
        }
        public async Task<SQLiteDataReader> ExecuteReaderAsync(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureConnectionOpenAsync(cancellationToken);
                var cmd = new SQLiteCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };
                if (parameters != null)
                    SqlParameterHelper.AddParameters(cmd, parameters);

                _logger.LogInfo($"Executing Async Reader: {commandText}");

                return await Task.Run(() =>
                {
                    return cmd.ExecuteReader(
                        _transaction == null ? CommandBehavior.CloseConnection : CommandBehavior.Default);
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing Async Reader: {commandText}", ex);
                throw;
            }
        }

        public SQLiteDataReader ExecuteReader(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                EnsureConnectionOpen();

                var cmd = new SQLiteCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };
                if (parameters != null)
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
        public DataTable ExecuteDataTable(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                EnsureConnectionOpen();

                using var cmd = new SQLiteCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };

                if (parameters != null)
                    SqlParameterHelper.AddParameters(cmd, parameters);

                _logger.LogInfo($"Executing DataTable query: {commandText}");

                using var adapter = new SQLiteDataAdapter(cmd);
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
        public async Task<DataTable> ExecuteDataTableAsync(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureConnectionOpenAsync(cancellationToken);
                await using var cmd = new SQLiteCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };
                if (parameters != null)
                    SqlParameterHelper.AddParameters(cmd, parameters);

                _logger.LogInfo($"Executing Async DataTable query: {commandText}");

                var dt = new DataTable();

                // SqlDataAdapter doesn't support async natively, so wrap in Task.Run
                await Task.Run(() =>
                {
                    using var adapter = new SQLiteDataAdapter(cmd);
                    adapter.Fill(dt);
                }, cancellationToken);

                if (_transaction == null)
                    await _connection!.CloseAsync();

                return dt;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing Async DataTable: {commandText}", ex);
                throw;
            }
        }
        public DataSet ExecuteDataSet(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                EnsureConnectionOpen();

                using var cmd = new SQLiteCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };

                if (parameters != null)
                    SqlParameterHelper.AddParameters(cmd, parameters);

                _logger.LogInfo($"Executing DataSet query: {commandText}");

                using var adapter = new SQLiteDataAdapter(cmd);
                var dataSet = new DataSet();
                adapter.Fill(dataSet);

                if (_transaction == null)
                    _connection?.Close();

                return dataSet;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing DataSet query: {commandText}", ex);
                throw;
            }
        }
        public async Task<DataSet> ExecuteDataSetAsync(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureConnectionOpenAsync(cancellationToken);

                await using var cmd = new SQLiteCommand(commandText, _connection, _transaction)
                {
                    CommandType = commandType
                };

                if (parameters != null)
                    SqlParameterHelper.AddParameters(cmd, parameters);

                _logger.LogInfo($"Executing Async DataSet query: {commandText}");

                var dataSet = new DataSet();

                // SqlDataAdapter doesn't support async natively, so wrap in Task.Run
                await Task.Run(() =>
                {
                    using var adapter = new SQLiteDataAdapter(cmd);
                    adapter.Fill(dataSet);
                }, cancellationToken);

                if (_transaction == null)
                    await _connection!.CloseAsync();

                return dataSet;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing Async DataSet: {commandText}", ex);
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
        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();

            if (_connection != null)
            {
                if (_connection.State != ConnectionState.Closed)
                    await _connection.CloseAsync();

                await _connection.DisposeAsync();
                _connection = null;
            }
        }
    }
}
