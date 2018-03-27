namespace com.zhusmelb.Util.Logging
{
    using System;

    /// <summary>
    /// General purpose logger interface 
    /// </summary>
    public interface IGenLogger : IDisposable
    {
        bool Loggable(LogLevel eLogLevel);
        void Flush();

        void Log(LogLevel level, string message);
        void Log(LogLevel level, string message, params object[] args);
        void Log(LogLevel level, Func<String> messageFunc);

        void LogData(LogLevel level, byte[] data);
        void LogError(LogLevel level, Exception e, string message);
    }
}