namespace com.zhusmelb.Util.Logging.Listener
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Diagnostics;
    using System.IO;
    using IO = System.IO;

    using com.zhusmelb.Util.Path;

    internal class DefaultPathConvension : PathConvensionAbstract
    {
        private const string LogFolder = "LOG";
        private static readonly Dictionary<string, string> _maps = new Dictionary<string, string>() {
            { LogFolder, "Logging" },
        };

        protected override Dictionary<string, string> GetPathMapper() {
            return _maps;
        }

        public override string Log {
            get {
                return IO.Path.Combine(AppDataBase, this[LogFolder]);
            }
        }
    }

    public class CircularFileTraceListener : TextWriterTraceListener
    {
        private const string FileQuotaAttribute = "maxFileSizeMB";
        private const string BackupLogFileAttribute = "backupLogFile";

        private readonly FileInfo _logFileInfo;
        private readonly Lazy<DefaultPathConvension> _defaultLog =
            new Lazy<DefaultPathConvension>();

        private IPathConvension _pathConvension;
        public IPathConvension DefaultPath {
            get { return _pathConvension ?? _defaultLog.Value; }
            set { _pathConvension = value; }
        }

        public CircularFileTraceListener(string file, string name) : base(file, name) {
            _logFileInfo = getFileInfoFromFilename(file);
            setWriterFromFileInfo(true);
        }

        public CircularFileTraceListener(string file)
            : this(file, string.Empty) 
        { }

        public double MaxFileSizeMb {
            get {
                var r = Double.MaxValue;
                var attr = Attributes[FileQuotaAttribute];
                if (string.IsNullOrEmpty(attr))
                    return r;
                try {
                    r = Double.Parse(attr);
                }
                catch (FormatException) {
                    // silently ignored
                }
                catch (OverflowException) {
                    // silently ignored
                }
                return r;
            }
        }

        public bool BackupLogFile {
            get {
                var r = true;
                var attr = Attributes[BackupLogFileAttribute];
                if (attr == null)
                    return r;
                try {
                    r = Boolean.Parse(attr);
                }
                catch (FormatException) {
                    // silently ignored
                }
                return r;
            }
        }

        #region Override members from TraceListener

        private static readonly string[] _supportedAttributes = {
            FileQuotaAttribute,
            BackupLogFileAttribute
        };

        protected override string[] GetSupportedAttributes()
        {
            return _supportedAttributes;
        }

        private void ensureLogfile()
        {
            Debug.Assert(_logFileInfo != null, "_logFileInfo not initialised");

            // if the log file is still valid, return.
            if (validateLogFile())
                return;

            // otherwise, close existing log file, backup it if needed, then
            // create a new one.
            closeLogFile();
            if (BackupLogFile)
                backupLogFile();
            setWriterFromFileInfo(false);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            ensureLogfile();
            base.TraceData(eventCache, source, eventType, id, data);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            ensureLogfile();
            base.TraceEvent(eventCache, source, eventType, id);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            ensureLogfile();
            base.TraceData(eventCache, source, eventType, id, data);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            ensureLogfile();
            base.TraceEvent(eventCache, source, eventType, id, format, args);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            ensureLogfile();
            base.TraceEvent(eventCache, source, eventType, id, message);
        }

        public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
        {
            ensureLogfile();
            base.TraceTransfer(eventCache, source, id, message, relatedActivityId);
        }

        #endregion

        #region Private helper members

        private FileInfo getFileInfoFromFilename(string file)
        {
            var fullname = IO.Path.IsPathRooted(file)
                ? file
                : IO.Path.Combine(DefaultPath.Log, file);
            return new FileInfo(fullname);
        }

        /// <summary>
        /// Create a <see cref="System.IO.TextWriter"/> and set as its writer.
        /// 
        /// The file name used to create this trace listener is created and set
        /// as its text writer.
        /// </summary>
        /// <param name="appended">
        /// If the log file exists, true will open it for append, otherwise it will be
        /// overwritten. If no pre-existed file, create a new one.
        /// </param>
        private void setWriterFromFileInfo(bool appended) {
            Debug.Assert(_logFileInfo != null, "_logFileInfo not initialised");
            Writer = new StreamWriter(_logFileInfo.FullName, appended, Encoding.UTF8);
        }

        private void closeLogFile() {
            if (Writer == null) return;
            Writer.Close();
        }

        /// <summary>
        /// Validate a log file.
        /// 
        /// A log file is valid when it satisfies the following conditions:
        /// <list type="bullet">
        /// <item>The log file must exist.</item>
        /// <item>The size of the file must be less than <see cref="MaxFileSizeMb" />.</item>
        /// </list>
        /// </summary>
        /// <returns>true if the log file is valid.</returns>
        private bool validateLogFile() {
            Debug.Assert(_logFileInfo != null, "_logFileInfo not initialised");

            if (_logFileInfo == null) 
                return false;

            _logFileInfo.Refresh();
            if (!_logFileInfo.Exists)
                return false;

            const int bytesInMb = 1024 * 1024;
            var sizeInMb = _logFileInfo.Length / bytesInMb;
            return sizeInMb < MaxFileSizeMb;
        }

        private void backupLogFile() {
            Debug.Assert(_logFileInfo != null, "_logFileInfo not initialised");
            try {
                _logFileInfo.Backup();
            }
            // not to handle any exception but just let it passes.
            catch (DirectoryNotFoundException) {}
            catch (IOException) {}
        }
        #endregion
    }
}
