using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDBCore.Enums
{
    /// <summary>
    /// Identifies the category of a database command. for use in logging, categorization, or command management.
    /// </summary>
    public enum CommandCategory
    {
        Unknown,
        Select,
        Insert,
        Update,
        Delete,
        StoredProcedure,
        Maintenance,
        Utility,
        Security
    }
}
