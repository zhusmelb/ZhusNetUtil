namespace com.zhusmelb.Util.Logging
{
    using System;

    /// <summary>
    /// General purpose logger interface.
    /// </summary>
    /// <remarks>
    /// This interface is used as logging framework in all other .NET development
    /// in zhusmel.com namespace. Even though there are plenty of other logging
    /// frameworks exist, I still decide to make up my own, here are the reasons:
    /// <list>
    /// <item>Logging code scatters around all over in the project. It is one of
    /// those things that you cannot afford to change once put in place. Having 
    /// a set of logging level and APIs in own control will protected any protential
    /// changes from the third party frameworks.</item>
    /// <item>Over the time, logging need arises which the third party logging 
    /// framework choosen at may not satisfy in time. Also bugs in the third party
    /// framework may not get fixed in time. Encapulate third party framework 
    /// will make it easier to change the logging framework.</item>
    /// </list>
    /// </remarks>
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