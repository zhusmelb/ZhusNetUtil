namespace com.zhusmelb.Util.Path
{
    using System;
    using System.IO;
    using IO = System.IO;
    
    public static class FilePathExtensions
    {
        /// <summary>
        /// Backup a file represented by a FileInfo object to <code>destName</code>
        /// </summary>
        /// <param name="destName">destinate file name</param>
        /// <remarks>
        /// If <see cref="destName" /> is null, the destinate file name is time-stamped.
        /// with format "{origin-name}-YYYYMMDDTHHmmss.{origin-ext}"
        /// </remarks>
        public static void Backup(this FileInfo src, string destName = null) {
            src.Refresh();
            var n = string.IsNullOrEmpty(destName) 
                ? GetTimestampedName(src.FullName, src.LastWriteTime) 
                : destName;
            src.CopyTo(n, true);
        }

        /// <summary>
        /// Timestamp the name of a given file <c>src</c>
        /// </summary>
        /// <param name="src">origin file name</param>
        /// <param name="dt">Datetime to stamp with the <c>src</c></param>
        /// <exceptions>
        /// </exceptions>
        /// <remarks>
        /// The default timestamp format is "{origin-name}-YYYYMMDDTHHmmss.{origin-ext}"
        /// </remarks>
        public static string GetTimestampedName(string src, DateTime dt) {
            if (src==null) 
                throw new ArgumentNullException(nameof(src));
                
            var path = IO.Path.GetDirectoryName(src); // can be null or string.Empty
            var ext = IO.Path.GetExtension(src); // can be string.Empty if no extension
            ext = ext.Equals(string.Empty) ? (string)null : ext;

            var n = IO.Path.GetFileNameWithoutExtension(src);
            n = $"{n}-{dt:yyyyMMddTHHmmss}";
            n = IO.Path.ChangeExtension(IO.Path.Combine(new[] {path, n}), ext);
            return n;
        }
    }
}
