namespace com.zhusmelb.Util.Logging
{
    public enum LogLevel
    {
        Off,
        Critical,   // Panic! A seriously bad unrecoverable event has occurred.
        Error,      // Serious Errors, usually recoverable.
        Warning,    // Things that should not happen under normal circumstances, but are recoverable.
        API,        // External api calls, external call params, SQL etc.
        Info,       // Significant events, partcularly time intensive tasks
        Debug,      // Internal events, private members call etc.
        Verbose,    // Most detailed logging, including binary data dumps, entry/exit of functions
    }
}