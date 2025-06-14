namespace SharpDBCore.Interfaces
{
    public interface IDbLogger
    {
        void LogInfo(string message);
        void LogError(string message, Exception? ex = null);
    }
}
