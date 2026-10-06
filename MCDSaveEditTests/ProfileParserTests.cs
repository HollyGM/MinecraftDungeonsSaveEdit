using MCDSaveEdit.Save.Models.Profiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
#nullable enable

namespace MCDSaveEditTests.DataTests
{
    [TestClass]
    public class ProfileParserTests
    {
        private static async Task<ProfileSaveFile?> read(string json)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return await ProfileParser.Read(stream);
        }

        private static async Task<JsonDocument> write(ProfileSaveFile profile)
        {
            using var stream = await ProfileParser.Write(profile);
            stream.Seek(0, SeekOrigin.Begin);
            return await JsonDocument.ParseAsync(stream);
        }

        [TestMethod]
        public async Task TestUnknownFieldsArePreservedOnWrite()
        {
            const string json = @"{
                ""items"": [{
                    ""type"": ""Sword"", ""power"": 1, ""rarity"": ""Common"",
                    ""futureItemField"": { ""nested"": [1, 2, 3] },
                    ""enchantments"": [{ ""id"": ""Unset"", ""level"": 0, ""futureEnchantmentField"": true }]
                }],
                ""xp"": 0,
                ""futureTopLevelField"": ""keep me""
            }";

            var profile = await read(json);
            Assert.IsNotNull(profile);
            using var output = await write(profile!);

            var root = output.RootElement;
            Assert.AreEqual("keep me", root.GetProperty("futureTopLevelField").GetString());
            var item = root.GetProperty("items")[0];
            Assert.AreEqual(3, item.GetProperty("futureItemField").GetProperty("nested").GetArrayLength());
            Assert.IsTrue(item.GetProperty("enchantments")[0].GetProperty("futureEnchantmentField").GetBoolean());
        }

        [TestMethod]
        public async Task TestNonAsciiTextIsPreserved()
        {
            //Several of these characters are encoded with UTF-8 bytes between 0x80 and 0x9F
            const string name = "Łukasz 中文 Ünïcödé ✓";
            var profile = await read("{\"items\": [], \"name\": \"" + name + "\"}");
            Assert.IsNotNull(profile);
            Assert.AreEqual(name, profile!.Name);
        }

        [TestMethod]
        public async Task TestBracesInsideStringsDoNotEndTheObject()
        {
            var profile = await read("{\"items\": [], \"name\": \"}{ tricky \\\" }\", \"xp\": 500}");
            Assert.IsNotNull(profile);
            Assert.AreEqual("}{ tricky \" }", profile!.Name);
            Assert.AreEqual(500, profile.Xp);
        }

        [TestMethod]
        public async Task TestTrailingDataAndControlCharactersAreIgnored()
        {
            //PS4 and Switch files carry duplicate data and padding after the first object
            var profile = await read("{\"items\": [],\r\n\t\"xp\": 1600}\0\0{\"items\": [], \"xp\": 3}\0");
            Assert.IsNotNull(profile);
            Assert.AreEqual(1600, profile!.Xp);
            Assert.AreEqual(0, profile.Items.Count());
        }
    }
}
