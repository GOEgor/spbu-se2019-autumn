using System;
using System.Threading;

namespace Task02
{
    class PrimAlgorithm
    {
        public static int[] SolveSeq(int[,] matrix, int vertices)
        {
            int[] parents = new int[vertices];
            int[] keys = new int[vertices];
            bool[] mstSet = new bool[vertices];

            for (int i = 0; i < vertices; i++)
            {
                keys[i] = Int32.MaxValue;
                mstSet[i] = false;
            }

            keys[0] = 0;
            parents[0] = -1;

            for (int count = 0; count < vertices - 1; count++)
            {
                int minVertex = minKey(keys, mstSet, vertices);
                mstSet[minVertex] = true;

                for (int vertex = 0; vertex < vertices; vertex++)
                    if (matrix[minVertex, vertex] != Int32.MaxValue && mstSet[vertex] == false
                        && matrix[minVertex, vertex] < keys[vertex])    
                    {
                        parents[vertex] = minVertex;
                        keys[vertex] = matrix[minVertex, vertex];
                    }
            }

            var weights = new int[vertices - 1];
            for (int i = 1; i < vertices; i++)
                weights[i - 1] = matrix[i, parents[i]];

            return weights; 
        }

        public static int[] SolvePar(int[,] matrix, int vertices)
        {
            int[] parents = new int[vertices];
            int[] keys = new int[vertices];
            bool[] mstSet = new bool[vertices];

            for (int i = 0; i < vertices; i++)
            {
                keys[i] = int.MaxValue;
                mstSet[i] = false;
            }

            keys[0] = 0;
            parents[0] = -1;

            int chunkNum = Environment.ProcessorCount;

            AutoResetEvent allDone = new AutoResetEvent(false);

            for (int count = 0; count < vertices - 1; count++)
            {
                int minVertex = minKey(keys, mstSet, vertices);
                mstSet[minVertex] = true;

                int completed = 0;
                for (int chunk = 0; chunk < chunkNum; chunk++)
                {
                    int chunkSize = vertices / chunkNum;
                    int chunkStart = chunkSize * chunk;

                    if (chunk == chunkNum - 1)
                        chunkSize += vertices % chunkNum;

                    int chunkEnd = chunkStart + chunkSize;

                    ThreadPool.QueueUserWorkItem((dumpObj) =>
                    {
                        for (int vertex = chunkStart; vertex < chunkEnd; vertex++)
                            if ((matrix[minVertex, vertex] != Int32.MaxValue) && mstSet[vertex] == false
                                && matrix[minVertex, vertex] < keys[vertex])
                            {
                                parents[vertex] = minVertex;
                                keys[vertex] = matrix[minVertex, vertex];
                            }

                        if (Interlocked.Increment(ref completed) == chunkNum)
                            allDone.Set();
                    });
                }

                allDone.WaitOne();
            }

            var weights = new int[vertices - 1];
            for (int i = 1; i < vertices; i++)
                weights[i - 1] = matrix[i, parents[i]];

            return weights;
        }

        private static int minKey(int[] keys, bool[] mstSet, int vertices)
        {
            int min = Int32.MaxValue, min_index = -1;

            for (int vertex = 0; vertex < vertices; vertex++)
                if (mstSet[vertex] == false && keys[vertex] < min)
                {
                    min = keys[vertex];
                    min_index = vertex;
                }

            return min_index;
        }
    }
}
