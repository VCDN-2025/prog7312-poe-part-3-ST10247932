using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalReporter.DataStructures
{
    public class Graph<T>
    {
        public Dictionary<T, List<(T to, double weight)>> Adj { get; } = new();

        public void AddNode(T n) { if (!Adj.ContainsKey(n)) Adj[n] = new List<(T, double)>(); }
        public void AddEdge(T a, T b, double weight = 1.0)
        {
            AddNode(a); AddNode(b);
            Adj[a].Add((b, weight));
            Adj[b].Add((a, weight));
        }

        public IEnumerable<T> BFS(T start)
        {
            if (!Adj.ContainsKey(start)) yield break;
            var q = new Queue<T>(); var seen = new HashSet<T>();
            q.Enqueue(start); seen.Add(start);
            while (q.Count > 0)
            {
                var u = q.Dequeue(); yield return u;
                foreach (var (v, w) in Adj[u]) if (!seen.Contains(v)) { seen.Add(v); q.Enqueue(v); }
            }
        }

        public IEnumerable<T> DFS(T start)
        {
            if (!Adj.ContainsKey(start)) yield break;
            var stack = new Stack<T>(); var seen = new HashSet<T>();
            stack.Push(start);
            while (stack.Count > 0)
            {
                var u = stack.Pop();
                if (seen.Contains(u)) continue;
                seen.Add(u); yield return u;
                foreach (var (v, w) in Adj[u]) if (!seen.Contains(v)) stack.Push(v);
            }
        }

        public List<(T u, T v, double w)> KruskalMST()
        {
            var edges = new List<(T u, T v, double w)>();
            var nodes = Adj.Keys.ToList();
            foreach (var u in nodes)
            {
                foreach (var (v, w) in Adj[u])
                {
                    if (Comparer<T>.Default.Compare(u, v) < 0) edges.Add((u, v, w));
                }
            }
            edges.Sort((a, b) => a.w.CompareTo(b.w));
            var uf = new UnionFind<T>(nodes);
            var mst = new List<(T, T, double)>();
            foreach (var e in edges)
            {
                if (!uf.Connected(e.u, e.v)) { uf.Union(e.u, e.v); mst.Add(e); }
            }
            return mst;
        }
    }

    public class UnionFind<T>
    {
        private readonly Dictionary<T, T> parent = new();
        private readonly Dictionary<T, int> rank = new();

        public UnionFind(IEnumerable<T> items)
        {
            foreach (var it in items) { parent[it] = it; rank[it] = 0; }
        }

        private T Find(T x)
        {
            if (!parent.ContainsKey(x)) { parent[x] = x; rank[x] = 0; }
            if (!parent[x].Equals(x)) parent[x] = Find(parent[x]);
            return parent[x];
        }

        public void Union(T a, T b)
        {
            var ra = Find(a); var rb = Find(b);
            if (ra.Equals(rb)) return;
            if (rank[ra] < rank[rb]) parent[ra] = rb;
            else if (rank[ra] > rank[rb]) parent[rb] = ra;
            else { parent[rb] = ra; rank[ra]++; }
        }

        public bool Connected(T a, T b) => Find(a).Equals(Find(b));
    }
}

//Reference
//Geeks for Geeks, 2025. Representation of Graph. [online] Available at: https://www.geeksforgeeks.org/dsa/graph-and-its-representations/ [Accessed 8 November 2025]