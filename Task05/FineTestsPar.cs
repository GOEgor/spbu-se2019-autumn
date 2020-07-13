using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task05;
using System.Threading;
using System.Diagnostics;

namespace Tests
{
    public class FineTestsPar
    {
        private List<int> insertList = new List<int>();
        private List<int> findList = new List<int>();
        private FineGrainedBSTree tree = new FineGrainedBSTree();
        private Random random = new Random();

        [SetUp]
        public void SetUp()
        {
            for (int i = 0; i < 1000; i++)
                tree.Insert(random.Next(0, 100));

            for (var i = 0; i < 100; i++)
            {
                insertList.Add(random.Next(0, 100));
                findList.Add(random.Next(0, 100));
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            tree.DisposeMutex();
        }

        [Test]
        public void CheckMutexAfterInsertAndFind_RandomTreeWithTaskParallelism_True()
        {
            var tasks = new List<Task>();

            foreach (var value in insertList)
                tasks.Add(Task.Run(() => tree.Insert(value)));

            foreach (var value in findList)
                tasks.Add(Task.Run(() => tree.Find(value)));

            Task.WaitAll(tasks.ToArray());
            Assert.True(tree.CheckMutex());
        }

        [DatapointSource]
        public int[] values = new int[] { 1, 5, 10, 20, 50, 100, 200 };

        [Theory]
        public void CheckMutexAfterInsert_RandomTreeWithThreadParallelism_True(int threadsAmount)
        {
            Thread[] threads = new Thread[threadsAmount];
            int threadCnt = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < threadsAmount; i++)
                threads[threadCnt++] = new Thread(() => {
                    tree.Insert(insertList[i]);
                });

            foreach (var thread in threads)
                thread.Start();

            foreach (var thread in threads)
                thread.Join();

            sw.Stop();
            Console.WriteLine("Elapsed={0}", sw.Elapsed);

            Assert.True(tree.CheckMutex());
        }

        [Theory]
        public void CheckMutexAfterFind_RandomTreeWithThreadParallelism_True(int threadsAmount)
        {
            Thread[] threads = new Thread[threadsAmount];
            int threadCnt = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < threadsAmount; i++)
                threads[threadCnt++] = new Thread(() => {
                    tree.Find(findList[i]);
                });

            foreach (var thread in threads)
                thread.Start();

            foreach (var thread in threads)
                thread.Join();

            sw.Stop();
            Console.WriteLine("Elapsed={0}", sw.Elapsed);

            Assert.True(tree.CheckMutex());
        }
    }
}