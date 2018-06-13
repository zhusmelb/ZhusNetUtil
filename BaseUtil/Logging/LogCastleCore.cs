namespace com.zhusmelb.Util.Logging
{
    using System;
    using System.Text;
    using CastleLog = Castle.Core.Logging;

    /// <summary>
    /// Leverage Castle.Log
    /// </summary>
    internal sealed class LogCastleCore : ILogger
    {
        private readonly Lazy<CastleLog.ILogger> _logger;
        private readonly string _name;

        public LogCastleCore(string name) {
            _name = name;
            _logger = new Lazy<CastleLog.ILogger>(initTraceLogger, true);    
        }

        // Lazy construction a ILogger
        // TOOD: Implement with IoC container 
        // In this method, we should ask IoC container for a 
        // CastleLog.ILoggerFactory instance to create CastleLog.ILogger
        // object.
        private CastleLog.ILogger initTraceLogger()
        {
            return new CastleLog.TraceLoggerFactory().Create(_name);
        }

        #region ILogger implementation

        public bool Loggable(LogLevel eLogLevel) {
            var newLevel = eLogLevel.AsLoggerLevel();
            var logger = _logger.Value;
            var b = false;
            switch (newLevel)
            {
                case CastleLog.LoggerLevel.Fatal:
                    b = logger.IsFatalEnabled;
                    break;
                case CastleLog.LoggerLevel.Error:
                    b = logger.IsErrorEnabled;
                    break;
                case CastleLog.LoggerLevel.Warn:
                    b = logger.IsWarnEnabled;
                    break;
                case CastleLog.LoggerLevel.Info:
                    b = logger.IsInfoEnabled;
                    break;
                case CastleLog.LoggerLevel.Debug:
                    b = logger.IsDebugEnabled;
                    break;
            }
            return b;
        }

        #region Core implementation of Log method overloads
        public void Log(LogLevel level, string message) {
            var newLevel = level.AsLoggerLevel();
            var logger = _logger.Value;
            switch (newLevel)
            {
                case CastleLog.LoggerLevel.Fatal:
                    logger.Fatal(message);
                    break;
                case CastleLog.LoggerLevel.Error:
                    logger.Error(message);
                    break;
                case CastleLog.LoggerLevel.Warn:
                    logger.Warn(message);
                    break;
                case CastleLog.LoggerLevel.Info:
                    logger.Info(message);
                    break;
                case CastleLog.LoggerLevel.Debug:
                    logger.Debug(message);
                    break;
            }
            
        }
        public void Log(LogLevel level, string sLogMessage, params object[] args) {
            var newLevel = level.AsLoggerLevel();
            var logger = _logger.Value;
            switch (newLevel)
            {
                case CastleLog.LoggerLevel.Fatal:
                    logger.FatalFormat(sLogMessage, args);
                    break;
                case CastleLog.LoggerLevel.Error:
                    logger.ErrorFormat(sLogMessage, args);
                    break;
                case CastleLog.LoggerLevel.Warn:
                    logger.WarnFormat(sLogMessage, args);
                    break;
                case CastleLog.LoggerLevel.Info:
                    logger.InfoFormat(sLogMessage, args);
                    break;
                case CastleLog.LoggerLevel.Debug:
                    logger.DebugFormat(sLogMessage, args);
                    break;
            }
        }
        #endregion

        public void Log(LogLevel level, Func<String> messageFunc) {
            if (!Loggable(level))
                return;

            try {
                Log(level, messageFunc());
            }
            catch (Exception e) {
                Log(LogLevel.Warning, "messageFunc generates exception", e);
            }
        }
        public void LogData(LogLevel level, byte[] data) {
            if (!Loggable(level))
                return;

            var buf = new StringBuilder();
            var buf1 = new StringBuilder();
            var buf2 = new StringBuilder();

            const int lnLen = 8; // 16 bytes each line
            var restLen = data.Length;

            for (var i = 0; i < restLen; ++i) {

                buf1.AppendFormat("{0,-3:X2}", data[i]);
                var c = Convert.ToChar(data[i]);
                buf2.Append(Char.IsControl(c) ? c : '.');

                if (i+1==restLen || i%lnLen == 0) {
                    buf.AppendFormat("{0} - {1}\n", buf1, buf2);
                    buf1.Clear();
                    buf2.Clear();
                }
            }
            Log(level, buf.ToString());
        }
        public void LogError(LogLevel level, Exception e, string message) {
            if (!Loggable(level))
                return;
            Log(level, message);
            Log(level, e.ToString());
        }
        public void Flush() {
           // do nothing
        }

        #endregion

        #region IDisposable interface
        public void Dispose()
        {
        }
        #endregion
    }

    internal static class CastleLogHelper
    {
        public static LogLevel AsLogLevel(this CastleLog.LoggerLevel level)
        {
            var newLevel = LogLevel.Off;

            switch (level)
            {
                case CastleLog.LoggerLevel.Off:
                    newLevel = LogLevel.Off;
                    break;
                case CastleLog.LoggerLevel.Fatal:
                    newLevel = LogLevel.Critical;
                    break;
                case CastleLog.LoggerLevel.Error:
                    newLevel = LogLevel.Error;
                    break;
                case CastleLog.LoggerLevel.Warn:
                    newLevel = LogLevel.Warning;
                    break;
                case CastleLog.LoggerLevel.Info:
                    newLevel = LogLevel.Info;
                    break;
                case CastleLog.LoggerLevel.Debug:
                    newLevel = LogLevel.Debug;
                    break;

            }
            return newLevel;
        }

        public static CastleLog.LoggerLevel AsLoggerLevel(this LogLevel level)
        {
            var newLevel = CastleLog.LoggerLevel.Off;
            switch (level)
            {
                case LogLevel.Off:
                    newLevel = CastleLog.LoggerLevel.Off;
                    break;
                case LogLevel.Critical:
                    newLevel = CastleLog.LoggerLevel.Fatal;
                    break;
                case LogLevel.Error:
                    newLevel = CastleLog.LoggerLevel.Error;
                    break;
                case LogLevel.Warning:
                    newLevel = CastleLog.LoggerLevel.Warn;
                    break;
                case LogLevel.Info:
                    newLevel = CastleLog.LoggerLevel.Info;
                    break;
                case LogLevel.Debug:
                case LogLevel.Verbose:
                    newLevel = CastleLog.LoggerLevel.Debug;
                    break;
            }
            return newLevel;
        }
    }

}
