using System;

namespace MunicipalReporter.DataStructures
{
    public class MinHeap<T> where T : IComparable<T>
    {
        private readonly List<T> data = new();
        public int Count => data.Count;
        public void Insert(T item) { data.Add(item); SiftUp(data.Count - 1); }
        public T Peek() { if (data.Count == 0) throw new InvalidOperationException(); return data[0]; }
        public T Pop()
        {
            if (data.Count == 0) throw new InvalidOperationException();
            var val = data[0];
            data[0] = data[^1];
            data.RemoveAt(data.Count - 1);
            if (data.Count > 0) SiftDown(0);
            return val;
        }

        private void SiftUp(int i)
        {
            while (i > 0)
            {
                int p = (i - 1) / 2;
                if (data[i].CompareTo(data[p]) >= 0) break;
                (data[i], data[p]) = (data[p], data[i]);
                i = p;
            }
        }

        private void SiftDown(int i)
        {
            int n = data.Count;
            while (true)
            {
                int l = 2 * i + 1, r = 2 * i + 2, smallest = i;
                if (l < n && data[l].CompareTo(data[smallest]) < 0) smallest = l;
                if (r < n && data[r].CompareTo(data[smallest]) < 0) smallest = r;
                if (smallest == i) break;
                (data[i], data[smallest]) = (data[smallest], data[i]);
                i = smallest;
            }
        }
    }
}
//Reference
//Geeks for Geeks, 2025. Introduction to Min-Heap. [online] Available at:https://www.geeksforgeeks.org/dsa/introduction-to-min-heap-data-structure/ [Accessed 8 November 2025]