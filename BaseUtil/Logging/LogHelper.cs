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
        public static ILogger GetLogger(string name)
        {
            return CreateGenLogger(name);
        }

        #endregion

        #region ILogger extensions

        public static void Info(this ILogger log, string message, params object[] args) {
            log.Log(LogLevel.Info, message, args);
        }

        public static void Debug(this ILogger log, string message, params object[] args) {
            log.Log(LogLevel.Debug, message, args);
        }

        public static void Debug<TObj>(this ILogger log, IEnumerable<TObj> colls, Predicate<IEnumerable<TObj>> pred, 
            Func<TObj, string> msgFunc)
            where TObj : class
        {
            log.CollectionIf(LogLevel.Debug, colls, pred, msgFunc);    
        }

        public static void Error(this ILogger log, string message, params object[] args) {
            log.Log(LogLevel.Error, message, args);
            log.Flush();
        }

        public static void Error(this ILogger log, Exception ex, string message, params object[] args) {
            log.LogError(LogLevel.Error, ex, string.Format(message, args));
            log.Flush();            
        }

        #pragma warning disable CS0618
        public static void StackTrace(this ILogger log, LogLevel level, string prompt) {
            log.AddStackTrace(level, prompt);
        }

        public static void CollectionIf<TObj>(this ILogger log, LogLevel level,  
                IEnumerable<TObj> colls, Predicate<IEnumerable<TObj>> pred, Func<TObj, string> msgFunc)
                where TObj : class
        {
            log.AddCollectionIf(level, pred, colls, msgFunc);
        }
        #pragma warning restore CS0618

        public static void Add(this ILogger log, string sLogMessage, params object[] args)
        {
            log.Add(LogLevel.Verbose, sLogMessage, args);
        }

        public static void Add(this ILogger log, LogLevel level, string sLogMessage, params object[] args) {
            log.Log(level, sLogMessage, args);
        }

        [Obsolete("AddCollectionIf is deprecated, please use CollectionIf and its wrappers of different level instead.")]
        public static void AddCollectionIf<TObjs, TObj>
            (this ILogger log, LogLevel level, Predicate<TObjs> pred, TObjs colls, Func<TObj, string> msgFunc)
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

        [Obsolete("AddStackTrace is deprecated, please use StackTrace instead.")]
        public static void AddStackTrace(this ILogger log, LogLevel level, string prompt)
        {
            log.Add(level, FormatLogMessage("{0} stack trace: \n{1}", () => prompt, () => new StackTrace()));
        }

        public static void Add(this ILogger log, LogLevel level, Func<string> msgFunc)
        {
            if (log.Loggable(level))
                log.Log(level, msgFunc());
        }
        
        public static void LogError(this ILogger log, Exception ex) {
            log.LogError(LogLevel.Error, ex, ex.Message);
            log.Flush();
        }

        public static void LogAndThrow(this ILogger log, Exception ex) {
            log.LogError(ex);
            throw ex;
        }

        #endregion

        #region private members
        
        private static ILogger CreateGenLogger(string name)
        {
            //TODO: This line couple with LogCastleCore too much
            var ts = new LogCastleCore(name);
            return ts;
        }
        #endregion
    }
}