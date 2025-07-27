using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDBCore.Interface
{
    public interface IDbLogger
    {
        void LogInfo(string message);
        void LogError(string message, Exception? ex = null);
    }
}
