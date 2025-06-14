using SharpDBCore.Interfaces;

namespace SharpDBCore.Loggers
{
    public class NullLogger : IDbLogger
    {
        public virtual void LogInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"NullLogger.LogInfo called with message: {message}");
        }
        public virtual void LogError(string message, Exception? ex = null)
        {
            System.Diagnostics.Debug.WriteLine($"NullLogger.LogError called with message: {message}, Exception: {ex}");
        }
    }
}
