using MCDSaveEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
#nullable enable

namespace MCDSaveEditTests.CoreTests
{
    [TestClass]
    public class SafeFileWriterTests
    {
        private string _directory = string.Empty;

        [TestInitialize]
        public void Setup()
        {
            _directory = Path.Combine(Path.GetTempPath(), "MCDSaveEditTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(_directory, recursive: true);
        }

        private static MemoryStream streamOf(string text)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(text));
        }

        [TestMethod]
        public async Task TestWritesNewFile()
        {
            var path = Path.Combine(_directory, "new.dat");
            using var stream = streamOf("new contents");
            await SafeFileWriter.writeAsync(stream, path, path + ".bak");

            Assert.AreEqual("new contents", File.ReadAllText(path));
            Assert.IsFalse(File.Exists(path + ".bak"));
            Assert.IsFalse(File.Exists(path + ".tmp"));
        }

        [TestMethod]
        public async Task TestReplacesExistingFileAndKeepsBackup()
        {
            var path = Path.Combine(_directory, "existing.dat");
            File.WriteAllText(path, "a much longer original content");
            using var stream = streamOf("short");
            stream.Seek(0, SeekOrigin.End); //the writer must rewind the stream itself
            await SafeFileWriter.writeAsync(stream, path, path + ".bak");

            Assert.AreEqual("short", File.ReadAllText(path));
            Assert.AreEqual("a much longer original content", File.ReadAllText(path + ".bak"));
            Assert.IsFalse(File.Exists(path + ".tmp"));
        }

        [TestMethod]
        public async Task TestReplacesExistingFileWithoutBackup()
        {
            var path = Path.Combine(_directory, "existing.json");
            File.WriteAllText(path, "old");
            using var stream = streamOf("new");
            await SafeFileWriter.writeAsync(stream, path);

            Assert.AreEqual("new", File.ReadAllText(path));
            Assert.AreEqual(1, Directory.GetFiles(_directory).Length);
        }
    }
}
