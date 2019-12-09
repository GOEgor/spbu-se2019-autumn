using NUnit.Framework;
using System;
using System.Collections.Generic;
using Task05;

namespace Tests
{
    public class CoarseTestsSeq
    {
        [Test]
        public void Find_EmptyTree_False()
        {
            var tree = new CoarseGrainedBSTree();

            Assert.False(tree.Find(1));         
        }

        [Test]
        public void Find_TreeWithOnlyRoot_True()
        {
            var tree = new CoarseGrainedBSTree();

            tree.Insert(1);

            Assert.True(tree.Find(1));
        }

        [Test]
        public void Find_TreeWithValues_True()
        {
            var tree = new CoarseGrainedBSTree();
            var random = new Random();
            var values = new List<int>();

            for (int i = 0; i < 100; i++)
                values.Add(random.Next(0, 100));

            foreach (int value in values)
                tree.Insert(value);

            foreach (int value in values)
                Assert.True(tree.Find(value));
        }

        [Test]
        public void Find_TreeWithWrongValues_False()
        {
            var tree = new CoarseGrainedBSTree();
            var random = new Random();
            var values = new List<int>();

            for (int i = 0; i < 100; i++)
                values.Add(random.Next(0, 100));

            for (int value = 100; value < 200; value++)
                tree.Insert(value);

            foreach (int value in values)
                Assert.False(tree.Find(value));
        }
    }
}