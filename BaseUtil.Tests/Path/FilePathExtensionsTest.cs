namespace com.zhusmelb.Util.Path.Test
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using com.zhusmelb.Util.Path;

    [TestFixture]
    public class TestFilePathExtensions
    {
        [TestCase(@"c:\bin\a.bat")]
        [TestCase(@"c:\bin\b")]
        [TestCase(@"c.bat")]
        [TestCase(@"c")]
        public void TestTimestampFile(string fn) {
            var dt = new DateTime(2018, 01, 01, 12,12, 12);
            var newName = FilePathExtensions.GetTimestampedName(fn, dt);

            var p = Path.GetDirectoryName(fn);
            var n = Path.GetFileNameWithoutExtension(fn);            
            var ext = Path.GetExtension(fn);
            ext = ext.Equals(string.Empty) ? null : ext;
            
            var expected = Path.ChangeExtension(Path.Combine(p, $"{n}-20180101T121212"), ext);
            Assert.That(newName, Is.EqualTo(expected));
        }
    }
}