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
            TestContext.Out.Write("Within test {0}, ", testContext.Test.FullName);

            var value = properties.Get("NoNLogConfig") as string;
            var isValueValid = value?.Equals("true", StringComparison.CurrentCultureIgnoreCase);
            if (isValueValid.HasValue && isValueValid.Value) {
                TestContext.WriteLine("NoNLogConfig property found \"{0}\"", value);
                var nlogConfigFile = new FileInfo(@"./Assets/nlog.config");
                Assert.That(nlogConfigFile.Exists, Is.True);
                Assert.That(()=>nlogConfigFile.CopyTo("nlog.config", true), Throws.Nothing);
                return;
            }
            TestContext.WriteLine("No NoNLogConfig property found");
            return;
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

        private void prepareNlogConfig(bool toCopy = true) {
            var nlogConfigFile = new FileInfo("nlog.config");
            if (nlogConfigFile.Exists) {
                Assert.That(()=>nlogConfigFile.Delete(), Throws.Nothing);
                nlogConfigFile.Refresh();
            }
            Assert.That(nlogConfigFile.Exists, Is.False);

            if (!toCopy) return;
            nlogConfigFile = new FileInfo(@"./Assets/nlog.config");
            Assert.That(nlogConfigFile.Exists, Is.True);
            Assert.That(()=>nlogConfigFile.CopyTo("nlog.config", true), Throws.Nothing);
        }
    }

}