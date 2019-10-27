using System;
using System.Threading;
using System.Collections.Generic;

namespace Task03
{
    class Program
    {
        public const int numOfProducers = 10;
        public const int numOfConsumers = 10;

        static void Main(string[] args)
        {
            var producers = new List<Producer<int>>();
            var consumers = new List<Consumer<int>>();

            for (int i = 0; i < numOfProducers; i++)
            {
                var producer = new Producer<int>();
                producers.Add(producer);
                Thread threadProd = new Thread(Producer<int>.produceData);
                threadProd.Start();
            }

            for (int i = 0; i < numOfConsumers; i++)
            {
                var consumer = new Consumer<int>();
                consumers.Add(consumer);
                Thread threadCons = new Thread(Consumer<int>.consumeData);
                threadCons.Start();
            }

            Console.ReadLine();

            foreach (var producer in producers) { producer.StopRunning(); }
            foreach (var consumer in consumers) { consumer.StopRunning(); }

            Console.ReadLine();
        }
    }

    static class Data<T>
    {
        public static List<T> buff = new List<T>();
        public static Mutex mutex = new Mutex();
        public static int prodCounter = 0;
        public static int consCounter = 0;
    }

    class Producer<T> where T : new()
    {
        private static bool _isRunning = true;

        public void StopRunning()
        {
            _isRunning = false;
        }

        public static void produceData()
        {
            while (_isRunning)
            {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} " +
                    $"is waiting for mutex to produce some data.");
                Data<T>.mutex.WaitOne();

                Data<T>.buff.Add(new T());

                Data<T>.prodCounter++;
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} " +
                    $"has produced data {Data<T>.prodCounter} times.");

                if (Data<T>.prodCounter % 2 == 0)
                {
                    Console.WriteLine("Producers are sleeping for 1 second..");
                    Thread.Sleep(1000);
                }

                Data<T>.mutex.ReleaseMutex();
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

        public static void consumeData()
        {
            while (_isRunning)
            {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} " +
                   $"is waiting for mutex to consume some data.");

                Data<T>.mutex.WaitOne();

                if (Data<T>.buff.Count > 0)
                    Data<T>.buff.RemoveAt(Data<T>.buff.Count - 1);

                Data<T>.consCounter++;
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} " +
                    $"has consumed data {Data<T>.consCounter} times.");

                if (Data<T>.consCounter % 2 == 0)
                {
                    Console.WriteLine("Consumers are sleeping for 1 second..");
                    Thread.Sleep(1000);
                }

                Data<T>.mutex.ReleaseMutex();
            }

            Console.WriteLine($"Consuming thread {Thread.CurrentThread.ManagedThreadId} " +
                $"is shutting down..");
        }
    }
}
