namespace com.zhusmelb.Util.Logging.Test
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using NUnit.Framework;
    using com.zhusmelb.Util.Logging;

    [TestFixture]
    internal class TestLoggingBasic 
    {
        [SetUp]
        public void Init() {
            var testContext = TestContext.CurrentContext;
            var properties = testContext.Test.Properties;
            TestContext.Progress.Write("Within test {0}, ", testContext.Test.FullName);

            var value = properties.Get("NoNLogConfig") as string;
            var noNLogConfig = value?.Equals("true", StringComparison.CurrentCultureIgnoreCase);
            TestContext.Progress.Write("noNLogConfig {0}, ", noNLogConfig);
            if (noNLogConfig.HasValue && noNLogConfig.Value) {
                TestContext.Progress.WriteLine("Do not copy nlog.config");
                return;
            }
            TestContext.Progress.WriteLine("Copying nlog.config...");
            var nlogConfigFile = new FileInfo(@"./Assets/nlog.config");
            Assert.That(nlogConfigFile.Exists, Is.True);
            Assert.That(()=>nlogConfigFile.CopyTo("nlog.config", true), Throws.Nothing);
            return;
        }
        
        [TearDown]
        public void CleanUp() {
            var nlogConfigFile = new FileInfo("nlog.config");
            if (nlogConfigFile.Exists) {
                Assert.That(()=>nlogConfigFile.Delete(), Throws.Nothing);
                nlogConfigFile.Refresh();
            }
            Assert.That(nlogConfigFile.Exists, Is.False);
        }

        [Test]
        public void TestLog() {
            var logger = LogHelper.GetLogger("TestLogger");
            logger.Debug("Hellow tester!");
            logger.Info("Hellow Tester!!!");
        }

        [Test, Property("NoNLogConfig", "true")]
        public void TestNoConfig() {
            Assert.That(() => {
                var logger = LogHelper.GetLogger("TestLogger1");
                logger.Debug("Hellow tester!");
                logger.Info("Hellow Tester!!!");
            }, Throws.Nothing);
        }
    }
}