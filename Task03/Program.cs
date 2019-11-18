using System;
using System.Threading;
using System.Collections.Generic;

namespace Task03
{
    class Program
    {
        public const int NumOfProducers = 10;
        public const int NumOfConsumers = 10;
        public const int SemCapacity = Int32.MaxValue;

        static void Main(string[] args)
        {
            var producers = new List<Producer<int>>();
            var consumers = new List<Consumer<int>>();

            for (int i = 0; i < NumOfProducers; i++)
            {
                var producer = new Producer<int>();
                producers.Add(producer);
                Thread threadProd = new Thread(Producer<int>.ProduceData);
                threadProd.Start();
            }

            for (int i = 0; i < NumOfConsumers; i++)
            {
                var consumer = new Consumer<int>();
                consumers.Add(consumer);
                Thread threadCons = new Thread(Consumer<int>.ConsumeData);
                threadCons.Start();
            }

            Console.ReadLine();

            foreach (var producer in producers) { producer.StopRunning(); }
            foreach (var consumer in consumers) { consumer.StopRunning(); }

            for (int i = 0; i < NumOfConsumers; i++)
                Data<int>.Sem.Release();
        }
    }

    static class Data<T>
    {
        public static List<T> Buff = new List<T>();
        public static Mutex Mtx = new Mutex();
        public static SemaphoreSlim Sem = new SemaphoreSlim(0, Program.SemCapacity);
    }

    class Producer<T> where T : new()
    {
        private static bool _isRunning = true;

        public void StopRunning()
        {
            _isRunning = false;
        }

        public static void ProduceData()
        {
            while (_isRunning)
            {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} " +
                    $"is waiting for mutex to produce some data.");
                Data<T>.Mtx.WaitOne();

                if (!_isRunning)
                {
                    Data<T>.Mtx.ReleaseMutex();
                    break;
                }

                Data<T>.Buff.Add(new T());

                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} " +
                    $"has produced data.");

                Thread.Sleep(500);

                Data<T>.Mtx.ReleaseMutex();
                Data<T>.Sem.Release();
            }

            Console.WriteLine($"Producing thread {Thread.CurrentThread.ManagedThreadId} " +
                $"is shutting down..");
        }
    }

    class Consumer<T>
    {
        private static bool _isRunning = true;

        public void StopRunning()
        {
            _isRunning = false;
        }

        public static void ConsumeData()
        {
            while (_isRunning)
            {
                if (Data<T>.Sem.CurrentCount == 0)
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} " +
                        $"is waiting for semaphore because buffer is empty.");

                Data<T>.Sem.Wait();

                if (!_isRunning) break;

                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} " +
                   $"is waiting for mutex to consume some data.");

                Data<T>.Mtx.WaitOne();
                Data<T>.Buff.RemoveAt(Data<T>.Buff.Count - 1);

                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} " +
                    $"has consumed data.");

                Thread.Sleep(500);

                Data<T>.Mtx.ReleaseMutex();
            }

            Console.WriteLine($"Consuming thread {Thread.CurrentThread.ManagedThreadId} " +
                $"is shutting down..");
        }
    }
}
