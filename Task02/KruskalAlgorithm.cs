namespace Task02
{
    class KruskalAlgorithm
    {
        private struct Subset
        {
            public int Parent;
            public int Rank;
        }

        private static int Find(Subset[] subsets, int i)
        {
            if (subsets[i].Parent != i)
                subsets[i].Parent = Find(subsets, subsets[i].Parent);

            return subsets[i].Parent;
        }

        private static void Union(Subset[] subsets, int x, int y)
        {
            int xroot = Find(subsets, x);
            int yroot = Find(subsets, y);

            if (subsets[xroot].Rank < subsets[yroot].Rank)
                subsets[xroot].Parent = yroot;
            else if (subsets[xroot].Rank > subsets[yroot].Rank)
                subsets[yroot].Parent = xroot;
            else
            {
                subsets[yroot].Parent = xroot;
                ++subsets[xroot].Rank;
            }
        }

        public static Graph.Edge[] SolveSeq(Graph.Edge[] edges, int verticesCount)
        {
            var mst = new Graph.Edge[verticesCount - 1];

            Sort.StartSortSeq(edges);

            int i = 0;
            int e = 0;

            var subsets = new Subset[verticesCount];

            for (int v = 0; v < verticesCount; v++)
            {
                subsets[v].Parent = v;
                subsets[v].Rank = 0;
            }

            while (e < verticesCount - 1)
            {
                Graph.Edge nextEdge = edges[i++];
                int x = Find(subsets, nextEdge.src);
                int y = Find(subsets, nextEdge.dest);

                if (x != y)
                {
                    mst[e++] = nextEdge;
                    Union(subsets, x, y);
                }
            }

            return mst;
        }

        public static Graph.Edge[] SolvePar(Graph.Edge[] edges, int verticesCount)
        {
            var mst = new Graph.Edge[verticesCount - 1];

            Sort.StartSortPar(edges);

            int i = 0;
            int e = 0;

            var subsets = new Subset[verticesCount];

            for (int v = 0; v < verticesCount; v++)
            {
                subsets[v].Parent = v;
                subsets[v].Rank = 0;
            }

            while (e < verticesCount - 1)
            {
                Graph.Edge nextEdge = edges[i++];
                int x = Find(subsets, nextEdge.src);
                int y = Find(subsets, nextEdge.dest);

                if (x != y)
                {
                    mst[e++] = nextEdge;
                    Union(subsets, x, y);
                }
            }

            return mst;
        }
    }
}
