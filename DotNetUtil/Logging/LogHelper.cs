namespace com.zhusmelb.Util.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    public static class LogHelper
    {
        static LogHelper()
        {
            Trace.AutoFlush = true;
        }

        #region public Helper functions
        public static Func<string> FormatLogMessage(string fmt, params Func<object>[] args)
        {
            return () =>
            {
                var msgs = new List<string>();
                Array.ForEach(args, el => msgs.Add(el().ToString()));
                return string.Format(fmt, msgs.ToArray());
            };
        }

        public static Func<string> FormatLogMessage(Func<string> fmtFunc , params Func<object>[] args) {
            return () =>
            {
                var msgs = new List<string>();
                var fmtstr = fmtFunc();
                Array.ForEach(args, el => msgs.Add(el().ToString()));
                return string.Format(fmtstr, msgs.ToArray());
            };
        }
        public static IGenLogger GetLogger(string name)
        {
            return CreateLogTraceSource(name);
        }

        #endregion

        #region IGenLogger extensions

        public static void Info(this IGenLogger log, string message, params object[] args) {
            log.Log(LogLevel.Info, message, args);
        }

        public static void Debug(this IGenLogger log, string message, params object[] args) {
            log.Log(LogLevel.Debug, message, args);
        }

        public static void Debug<TObj>(this IGenLogger log, Predicate<IEnumerable<TObj>> pred, IEnumerable<TObj> colls,
            Func<TObj, string> msgFunc)
            where TObj : class
        {
            log.AddCollectionIf(LogLevel.Debug, pred, colls, msgFunc);    
        }

        public static void Error(this IGenLogger log, string message, params object[] args) {
            log.Log(LogLevel.Error, message, args);
            log.Flush();
        }

        public static void Error(this IGenLogger log, Exception ex, string message, params object[] args) {
            log.LogError(LogLevel.Error, ex, string.Format(message, args));
            log.Flush();            
        }

        public static void Add(this IGenLogger log, string sLogMessage, params object[] args)
        {
            log.Add(LogLevel.Verbose, sLogMessage, args);
        }

        public static void Add(this IGenLogger log, LogLevel level, string sLogMessage, params object[] args) {
            log.Log(level, sLogMessage, args);
        }

        public static void AddCollectionIf<TObjs, TObj>
            (this IGenLogger log, LogLevel level, Predicate<TObjs> pred, TObjs colls, Func<TObj, string> msgFunc)
            where TObjs : IEnumerable<TObj>
        {
            if (log.Loggable(level) && pred(colls)) {
                var msgbuilder = new StringBuilder();
                msgbuilder.Append("{");
                foreach (var obj in colls)
                {
                    msgbuilder.Append(msgFunc(obj));
                    msgbuilder.Append(",\n");
                }
                msgbuilder.Append("}");
                log.Log(level, msgbuilder.ToString());
            }
        }

        public static void AddStackTrace(this IGenLogger log, LogLevel level, string prompt)
        {
            log.Add(level, FormatLogMessage("{0} stack trace: \n{1}", () => prompt, () => new StackTrace()));
        }

        public static void Add(this IGenLogger log, LogLevel level, Func<string> msgFunc)
        {
            if (log.Loggable(level))
                log.Log(level, msgFunc());
        }
        
        public static void LogError(this IGenLogger log, Exception ex) {
            log.LogError(LogLevel.Error, ex, ex.Message);
            log.Flush();
        }

        public static void LogAndThrow(this IGenLogger log, Exception ex) {
            log.LogError(ex);
            throw ex;
        }

        #endregion

        #region private members
        
        private static IGenLogger CreateLogTraceSource(string name)
        {
            //TODO: This line couple with LogCastleCore too much
            var ts = new LogCastleCore(name);
            return ts;
        }
        #endregion
    }
}