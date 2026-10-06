using MCDSaveEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
#nullable enable

namespace MCDSaveEditTests.CoreTests
{
    [TestClass]
    public class IEnumerableExtensionsTests
    {
        [TestMethod]
        public void TestRemovingKeepsDuplicatesOfOtherElements()
        {
            var result = new[] { 1, 2, 2, 3 }.removing(3).ToArray();
            CollectionAssert.AreEqual(new[] { 1, 2, 2 }, result);
        }

        [TestMethod]
        public void TestReplacing()
        {
            var result = new[] { "a", "b", "c" }.replacing("b", "x").ToArray();
            CollectionAssert.AreEqual(new[] { "a", "x", "c" }, result);
        }

        [TestMethod]
        public void TestReplacingMissingElementLeavesCollectionUnchanged()
        {
            var result = new[] { "a", "b" }.replacing("missing", "x").ToArray();
            CollectionAssert.AreEqual(new[] { "a", "b" }, result);
        }
    }
}
