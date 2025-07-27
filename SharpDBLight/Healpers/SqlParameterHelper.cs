using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDBLight.Healpers
{
    public static class SqlParameterHelper
    {
        /// <summary>
        /// Adds parameters to a SqlCommand from the provided list.
        /// Automatically handles null values using DBNull.Value.
        /// </summary>
        /// <param name="cmd">The SqlCommand to add parameters to</param>
        /// <param name="parameters">A list of SqlParameter objects.</param>
        public static void AddParameters(System.Data.SQLite.SQLiteCommand cmd, List<System.Data.SQLite.SQLiteParameter>? parameters)
        {
            if (cmd == null)
                throw new ArgumentNullException(nameof(cmd));
            if (parameters == null || parameters.Count == 0)
                return;
            foreach (var param in parameters)
            {
                // Ensure parameter name starts with '@'
                if (!param.ParameterName.StartsWith("@"))
                    param.ParameterName = "@" + param.ParameterName;
                if (!cmd.Parameters.Contains(param.ParameterName))
                {
                    cmd.Parameters.Add(param);
                }
            }
        }
    }
}
