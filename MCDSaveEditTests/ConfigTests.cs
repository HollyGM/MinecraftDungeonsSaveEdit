using MCDSaveEdit.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
#nullable enable

namespace MCDSaveEditTests.ServicesTests
{
    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        public void TestParseVersion()
        {
            Assert.AreEqual(new Version(1, 5, 8, 0), Config.parseVersion("1.5.8.0"));
            Assert.AreEqual(new Version(1, 5, 8, 0), Config.parseVersion("v1.5.8.0"));
            Assert.AreEqual(new Version(1, 6), Config.parseVersion(" V1.6 "));
        }

        [TestMethod]
        public void TestParseInvalidVersion()
        {
            Assert.IsNull(Config.parseVersion(null));
            Assert.IsNull(Config.parseVersion(""));
            Assert.IsNull(Config.parseVersion("latest"));
            Assert.IsNull(Config.parseVersion("1.5-beta"));
        }
    }
}
