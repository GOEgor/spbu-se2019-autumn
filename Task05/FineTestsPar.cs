using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task05;
using System.Threading;

namespace Tests
{
    public class FineTestsPar
    {
        [Test]
        public void TestTaskParallelism()
        {
            List<int> insertList = new List<int>();
            List<int> findList = new List<int>();
            var tree = new FineGrainedBSTree();
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

        [Test]
        public void Test20ThreadsParallelism()
        {
            List<int> insertList = new List<int>();
            List<int> findList = new List<int>();
            var tree = new FineGrainedBSTree();
            var random = new Random();

            for (var i = 0; i < 10; i++)
            {
                insertList.Add(random.Next(0, 100000));
                findList.Add(random.Next(0, 1000));
            }

            for (int i = 0; i < 100000; i++)
                tree.Insert(random.Next(0, 100000));

            Thread[] threads = new Thread[20];
            int threadCnt = 0;

            foreach (var value in insertList)
                threads[threadCnt++] = new Thread(() => {
                    tree.Insert(value);
                });

            foreach (var value in findList)
                threads[threadCnt++] = new Thread(() => {
                    tree.Find(value);
                });

            foreach (var thread in threads)
                thread.Start();

            foreach (var thread in threads)
                thread.Join();

            Assert.True(tree.CheckMutex());
        }
    }
}