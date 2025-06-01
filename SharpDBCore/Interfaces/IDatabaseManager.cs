using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDBCore.Interfaces
{
    public interface IDatabaseManager: IDisposable
    {
        void SetLogger(IDbLogger logger);

        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        int ExecuteNonQuery(string commandText, Dictionary<string, object>? parameters = null, CommandType commandType = CommandType.Text);
        object ExecuteScalar(string commandText, Dictionary<string, object>? parameters = null, CommandType commandType = CommandType.Text);
        SqlDataReader ExecuteReader(string commandText, Dictionary<string, object>? parameters = null, CommandType commandType = CommandType.Text);
        DataTable ExecuteDataTable(string commandText, Dictionary<string, object>? parameters = null, CommandType commandType = CommandType.Text);
    }
}
