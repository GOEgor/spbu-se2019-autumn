using System;
using System.IO;
using System.Collections.Generic;

namespace Task02
{
    class Graph
    {
        public class Edge : IComparable<Edge>
        {
            public Edge(int src, int dest, int weight)
            {
                this.src = src;
                this.dest = dest;
                this.weight = weight;
            }

            public int src, dest, weight;

            int IComparable<Edge>.CompareTo(Edge other)
            {
                return weight - other.weight;
            }
        }

        public string GenerateFile(int vertices, int edges, string name)
        {
            var rand = new Random();

            using (StreamWriter sw = File.CreateText(name))
            {
                sw.WriteLine(vertices);
                var checkRandom = new int[vertices, vertices];
                for (int i = 0; i < edges; i++)
                {
                    int v1, v2;
                    do
                    {
                        v1 = rand.Next(vertices - 1);
                        v2 = rand.Next(v1 + 1, vertices);
                    }
                    while (checkRandom[v1, v2] != 0);

                    checkRandom[v1, v2] = 1;
                    int weight = rand.Next(1, Program.maxEdgeValue);

                    sw.WriteLine($"{v1} {v2} {weight}");
                }
            }
            return name;
        }

        public int[,] GenerateMatrixArr(string fileName)
        {
            using (StreamReader sr = File.OpenText(fileName))
            {
                int vertices = int.Parse(sr.ReadLine());
                int[,] matrix = new int[vertices, vertices];
                for (int i = 0; i < vertices; i++)
                    for (int j = 0; j < vertices; j++)
                        if (i == j) matrix[i, j] = 0; else matrix[i, j] = Int32.MaxValue;

                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    string[] words = s.Split(' ');
                    int v1 = int.Parse(words[0]);
                    int v2 = int.Parse(words[1]);
                    int w = int.Parse(words[2]);

                    matrix[v1, v2] = w;
                    matrix[v2, v1] = w;
                }

                return matrix;
            }
        }

        public Edge[] GenerateEdgesArr(string fileName)
        {
            using (StreamReader sr = File.OpenText(fileName))
            {
                List<Edge> edges = new List<Edge>();
                int vertices = int.Parse(sr.ReadLine());

                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    string[] words = s.Split(' ');
                    int v1 = int.Parse(words[0]);
                    int v2 = int.Parse(words[1]);
                    int w = int.Parse(words[2]);

                    edges.Add(new Edge(v1, v2, w));
                }

                return edges.ToArray();
            }
        }

        public bool CompareMatrices(int[,] matrix1, int[,] matrix2, int vertices)
        {
            for (int i = 0; i < vertices; i++)
            {
                for (int j = 0; j < vertices; j++)
                {
                    if (matrix1[i, j] != matrix2[i, j])
                        return false;
                }
            }
            return true;
        }

        public void PrintMatrixToFile(int[,] matrix, int vertices, StreamWriter file)
        {
            for (int i = 0; i < vertices; i++)
            {
                for (int j = 0; j < vertices; j++)
                {
                    if (matrix[i, j] == Int16.MaxValue)
                        file.Write(String.Format("{0,4}", "-"));
                    else
                        file.Write(String.Format("{0,4}", matrix[i, j]));
                }
                file.WriteLine();
            }
        }

        public int GetEdgesSum(Graph.Edge[] edgeArr)
        {
            int sum = 0;
            foreach (var edge in edgeArr)
                sum += edge.weight;
            return sum;
        }

        public int GetWeightsSum(int[] arr)
        {
            int sum = 0;
            foreach (var weight in arr)
                sum += weight;
            return sum;
        }

    }
}
