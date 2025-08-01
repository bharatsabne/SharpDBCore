﻿using Microsoft.Data.SqlClient;
using SharpDBCore.Interface;
using System.Data;

namespace SharpDBCore.Interfaces
{
    public interface IDatabaseSQLManager: IDisposable, IAsyncDisposable
    {
        void SetLogger(IDbLogger logger);
        // Synchronous transaction management
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        // Synchronous query execution
        int ExecuteNonQuery(string commandText, List<SqlParameter>? parameters = null, CommandType commandType = CommandType.Text);
        object ExecuteScalar(string commandText, List<SqlParameter>? parameters = null, CommandType commandType = CommandType.Text);
        SqlDataReader ExecuteReader(string commandText, List<SqlParameter>? parameters = null, CommandType commandType = CommandType.Text);
        DataTable ExecuteDataTable(string commandText, List<SqlParameter>? parameters = null, CommandType commandType = CommandType.Text);
        DataSet ExecuteDataSet(string commandText, List<SqlParameter>? parameters = null, CommandType commandType = CommandType.Text);

        // Asynchronous transaction management
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        // Asynchronous query execution
        Task<int> ExecuteNonQueryAsync(string commandText, List<SqlParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default);
        Task<object?> ExecuteScalarAsync(string commandText, List<SqlParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default);
        Task<SqlDataReader> ExecuteReaderAsync(string commandText, List<SqlParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default);
        Task<DataTable> ExecuteDataTableAsync(string commandText, List<SqlParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default);
        Task<DataSet> ExecuteDataSetAsync(string commandText, List<SqlParameter>? parameters = null, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default);
    }
}
