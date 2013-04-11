namespace IndexedCollections.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BalancedTreeTests
    {
        #region Public Methods

        [TestMethod]
        public void BalancedTree_AddTest()
        {
            var dictionary = CreateRandomDictionary(2000);
            var tree = new BalancedTree<int, string>(17);
            var dictionary2 = new Dictionary<int, string>();

            Assert.IsFalse(tree.Any());
            foreach (var kvp in dictionary)
            {
                dictionary2.Add(kvp.Key, kvp.Value);
                tree.Add(kvp.Key, kvp.Value);
                Assert.IsTrue(tree.SequenceEqual(dictionary2.OrderBy(kvp1 => kvp1.Key)));
            }
        }

        [TestMethod]
        public void BalancedTree_RemoveTest()
        {
            var dictionary = CreateRandomDictionary(2000);
            var tree = new BalancedTree<int, string>(17, dictionary);
            var keys = dictionary.Keys.ToList();

            foreach (var key in keys)
            {
                dictionary.Remove(key);
                tree.Remove(key);
                Assert.IsTrue(tree.SequenceEqual(dictionary.OrderBy(kvp => kvp.Key)));
            }

            Assert.IsFalse(tree.Any());
        }

        [TestMethod]
        public void BalancedTree_ReversedTreeTest()
        {
            var dictionary = CreateRandomDictionary(20000);
            var reversedTree = new BalancedTree<int, string>(24, dictionary, true);
            Assert.IsTrue(reversedTree.SequenceEqual(dictionary.OrderByDescending(kvp => kvp.Key)));
        }

        #endregion

        #region Methods

        private static Dictionary<int, string> CreateRandomDictionary(int size)
        {
            var random = new Random();
            return Enumerable
                .Range(0, size)
                .Select(i => random.Next(Int32.MaxValue))
                .Distinct()
                .ToDictionary(i => i, i => "xxxyyyzzzvvvmmm" + random.Next(Int32.MaxValue).ToString());
        }

        #endregion
    }
}