using System;
using System.Threading;

namespace Task02
{
    class FloydAlgorithm
    {
        public static int[,] SolveSeq(int[,] matrix, int vertices)
        {
            for (int k = 0; k < vertices; ++k)
                for (int i = 0; i < vertices; ++i)
                    for (int j = 0; j < vertices; ++j)
                        if (matrix[i, k] != Int32.MaxValue && matrix[k, j] != Int32.MaxValue)
                            matrix[i, j] = Math.Min(matrix[i, j], matrix[i, k] + matrix[k, j]);

            return matrix;
        }

        public static int[,] SolvePar(int[,] matrix, int vertices)
        {
            int chunkNum = Environment.ProcessorCount;

            Thread[] threads = new Thread[chunkNum];

            for (int chunk = 0; chunk < chunkNum; ++chunk)
            {
                int chunkSize = vertices / chunkNum;
                int chunkStart = chunkSize * chunk;

                if (chunk == chunkNum - 1)
                    chunkSize += vertices % chunkNum;

                int chunkEnd = chunkStart + chunkSize;

                threads[chunk] = new Thread(() =>
                {
                    for (int k = chunkStart; k < chunkEnd; ++k)
                        for (int i = chunkStart; i < chunkEnd; i++)
                            for (int j = i + 1; j < chunkEnd; j++)
                                if (matrix[i, k] != Int32.MaxValue && matrix[k, j] != Int32.MaxValue)
                                    matrix[i, j] = Math.Min(matrix[i, j], matrix[i, k] + matrix[k, j]);
                });

                threads[chunk].Start();
            }

            foreach (var thread in threads)
                thread.Join();

            return matrix;
        }
    }
}
