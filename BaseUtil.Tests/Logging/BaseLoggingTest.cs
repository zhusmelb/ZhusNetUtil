namespace com.zhusmelb.Util.Logging.Test
{
    using NUnit.Framework;
    using com.zhusmelb.Util.Logging;

    [TestFixture]
    internal class TestLoggingBasic 
    {
        [Test]
        public void TestLog() {
            var logger = LogHelper.GetLogger("TestLogger");

            logger.Debug("Hellow tester!");
            logger.Info("Hellow Tester!!!");
        }
    }

}