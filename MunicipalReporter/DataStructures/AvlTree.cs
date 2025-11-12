using System;

namespace MunicipalReporter.DataStructures
{
    public class AvlTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private class Node
        {
            public TKey Key;
            public TValue Value;
            public Node Left, Right;
            public int Height;
            public Node(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Height = 1;
            }
        }

        private Node root;

        public void Insert(TKey key, TValue value)
        {
            root = Insert(root, key, value);
        }

        private Node Insert(Node node, TKey key, TValue value)
        {
            if (node == null) return new Node(key, value);

            int cmp = key.CompareTo(node.Key);
            if (cmp < 0) node.Left = Insert(node.Left, key, value);
            else if (cmp > 0) node.Right = Insert(node.Right, key, value);
            else node.Value = value; // overwrite existing

            UpdateHeight(node);
            return Balance(node);
        }

        public bool TryGet(TKey key, out TValue value)
        {
            Node current = root;
            while (current != null)
            {
                int cmp = key.CompareTo(current.Key);
                if (cmp < 0) current = current.Left;
                else if (cmp > 0) current = current.Right;
                else
                {
                    value = current.Value;
                    return true;
                }
            }
            value = default;
            return false;
        }

        public void InOrder(Action<TKey, TValue> action)
        {
            InOrder(root, action);
        }

        private void InOrder(Node node, Action<TKey, TValue> action)
        {
            if (node == null) return;
            InOrder(node.Left, action);
            action(node.Key, node.Value);
            InOrder(node.Right, action);
        }

        // --- AVL helpers ---
        private int Height(Node node) => node?.Height ?? 0;
        private void UpdateHeight(Node node) => node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));
        private int BalanceFactor(Node node) => Height(node.Left) - Height(node.Right);

        private Node Balance(Node node)
        {
            int bf = BalanceFactor(node);

            if (bf > 1)
            {
                if (BalanceFactor(node.Left) < 0)
                    node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }
            if (bf < -1)
            {
                if (BalanceFactor(node.Right) > 0)
                    node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }
            return node;
        }

        private Node RotateRight(Node y)
        {
            Node x = y.Left;
            Node T2 = x.Right;
            x.Right = y;
            y.Left = T2;
            UpdateHeight(y);
            UpdateHeight(x);
            return x;
        }

        private Node RotateLeft(Node x)
        {
            Node y = x.Right;
            Node T2 = y.Left;
            y.Left = x;
            x.Right = T2;
            UpdateHeight(x);
            UpdateHeight(y);
            return y;
        }
    }
}
//Reference
//Geeks for Geeks, 2025. Insertion in an AVL Tree. [online] Available at: https://www.geeksforgeeks.org/dsa/insertion-in-an-avl-tree/ [Accessed 8 November 2025]