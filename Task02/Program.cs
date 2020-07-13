using System;
using System.IO;

namespace Task02
{
    class Program
    {
        public const int vertices = 5000;
        public const int edges = 5000000;
        public const int maxEdgeValue = 1000;

        static void Main(string[] args)
        {
            var graph = new Graph();

            string file = graph.GenerateFile(vertices, edges, "D:\\test.in");
            var matrix = graph.GenerateMatrixArr(file);
            var edgesArr = graph.GenerateEdgesArr(file);

            var floydSeq = FloydAlgorithm.SolveSeq(matrix, vertices);
            var primSeq = PrimAlgorithm.SolveSeq(matrix, vertices);
            var kruskalSeq = KruskalAlgorithm.SolveSeq(edgesArr, vertices);

            var floydPar = FloydAlgorithm.SolvePar(matrix, vertices);
            var primPar = PrimAlgorithm.SolvePar(matrix, vertices);
            var kruskalPar = KruskalAlgorithm.SolvePar(edgesArr, vertices);

            //Console.WriteLine(graph.CompareMatrices(floydSeq, floydPar, vertices));
            //Console.WriteLine(graph.GetWeightsSum(primSeq) == graph.GetWeightsSum(primPar));
            //Console.WriteLine(graph.GetEdgesSum(kruskalSeq) == graph.GetEdgesSum(kruskalPar));

            using (StreamWriter sw = File.CreateText("D:\\resFloyd.out"))
            {
                graph.PrintMatrixToFile(floydPar, vertices, sw);
            }

            using (StreamWriter sw = File.CreateText("D:\\resPrim.out"))
            {
                sw.WriteLine(graph.GetWeightsSum(primPar));
            }

            using (StreamWriter sw = File.CreateText("D:\\resKruskal.out"))
            {
                sw.WriteLine(graph.GetEdgesSum(kruskalPar));
            }
        }
    }
}
