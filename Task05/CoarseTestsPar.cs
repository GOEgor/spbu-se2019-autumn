using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task05;

namespace Tests
{
    public class CoarseTestsPar
    {
        [Test]
        public void Test()
        {
            List<int> insertList = new List<int>();
            List<int> findList = new List<int>();
            var tree = new CoarseGrainedBSTree();
            var tasks = new List<Task>();

            for (var i = 0; i < 100; i++)
            {
                var random = new Random();
                insertList.Add(random.Next(0, 50));
                findList.Add(random.Next(0, 100));
            }

            foreach (var value in insertList)
                tasks.Add(Task.Run(() => tree.Insert(value)));

            foreach (var value in findList)
                tasks.Add(Task.Run(() => tree.Find(value)));
            
            Task.WaitAll(tasks.ToArray());
            Assert.True(tree.CheckMutex());
        }
            
    }
}