namespace com.zhusmelb.Util.Path
{
    using System;
    using System.IO;
    using IO = System.IO;
    
    static class FilePathExtensions
    {
        /// <summary>
        /// Copy a file represented by <code>src</code> to <code>destName</code>
        /// </summary>
        /// <param name="src">source file</param>
        /// <param name="destName">destinate file name</param>
        /// <remarks>
        /// If <see cref="destName" /> is null, the destinate file name is time stamped.
        /// with format "{origin-name}-YYYYMMDDTHHmmss.{origin-ext}"
        /// </remarks>
        public static void Backup(this FileInfo src, string destName = null) {
            src.Refresh();
            var n = destName;
            if (string.IsNullOrEmpty(n)) {
                var path = src.DirectoryName;
                var tm = src.LastWriteTime;
                n = IO.Path.ChangeExtension(src.Name, null);
                n = string.Format("{0}-{1:yyyyMMddTHHmmss}", n, tm);
                n = IO.Path.Combine(new[] {path, n, src.Extension});
            }
            src.CopyTo(n, true);
        }
    }
}
