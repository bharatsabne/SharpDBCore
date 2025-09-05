using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDBCore.Interface;

namespace SharpDBLight.Interfaces
{
    public interface IDatabaseLightManager : IDisposable, IAsyncDisposable
    {
        void SetLogger(IDbLogger logger);

        // Synchronous transaction management
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        // Synchronous query execution
        int ExecuteNonQuery(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text);
        object? ExecuteScalar(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text);
        SQLiteDataReader ExecuteReader(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text);
        DataTable ExecuteDataTable(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text);
        DataSet ExecuteDataSet(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text);

        // Asynchronous transaction management
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        // Asynchronous query execution
        Task<int> ExecuteNonQueryAsync(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default);
        Task<object?> ExecuteScalarAsync(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default);
        Task<SQLiteDataReader> ExecuteReaderAsync(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default);
        Task<DataTable> ExecuteDataTableAsync(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default);
        Task<DataSet> ExecuteDataSetAsync(string commandText, List<SQLiteParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default);
    }
}
