using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task05
{
    class FineGrainedBSTree
    {
        public class Node
        {
            public int Value;
            public Node Left;
            public Node Right;
            public Node Parent;

            public readonly Mutex mutex = new Mutex();

            public Node(int newValue, Node newParent)
            {
                Value = newValue;
                Parent = newParent;
                Left = null;
                Right = null;
            }
        }

        private Node _root;

        public bool CheckMutex()
        {
            return traverseAndCheckMutex(_root);
        }

        private bool traverseAndCheckMutex(Node node)
        {
            if (node != null)
            {
                var isLocked = node.mutex.WaitOne(0);
                if (!isLocked)
                    return false;

                node.mutex.ReleaseMutex();

                return traverseAndCheckMutex(node.Left) && traverseAndCheckMutex(node.Right);
            }

            return true;
        }

        public void DisposeMutex()
        {
            traverseAndDisposeMutex(_root);
        }

        private void traverseAndDisposeMutex(Node node)
        {
            if (node != null)
            {
                node.mutex.Close();
                traverseAndDisposeMutex(node.Left);
                traverseAndDisposeMutex(node.Right);
            }
        }

        public bool Find(int value)
        {
            Node curr = _root;

            if (curr == null)
                return false;

            curr.mutex.WaitOne();

            try
            {
                while (true)
                {
                    if (curr.Value == value)
                    {
                        return true;
                    }

                    Node next = curr.Value > value ? curr.Left : curr.Right;

                    if (next == null)
                        break;

                    if (curr.Parent != null)
                        curr.Parent.mutex.ReleaseMutex();

                    next.mutex.WaitOne();

                    curr = next;
                }

                return false;
            }
            finally
            {
                if (curr.Parent != null)
                    curr.Parent.mutex.ReleaseMutex();

                curr.mutex.ReleaseMutex();
            }
        }

        public void Insert(int value)
        {
            Node curr = _root;

            if (curr == null)
            {
                _root = new Node(value, null);
                return;
            }

            curr.mutex.WaitOne();

            try
            {
                while (true)
                {
                    if (curr.Value == value)
                    {
                        return;
                    }
                    else if (curr.Value > value)
                    {
                        if (curr.Left == null)
                        {
                            curr.Left = new Node(value, curr);
                            return;
                        }

                        if (curr.Parent != null)
                            curr.Parent.mutex.ReleaseMutex();

                        curr.Left.mutex.WaitOne();
                        curr = curr.Left;
                    }
                    else
                    {
                        if (curr.Right == null)
                        {
                            curr.Right = new Node(value, curr);
                            return;
                        }

                        if (curr.Parent != null)
                            curr.Parent.mutex.ReleaseMutex();

                        curr.Right.mutex.WaitOne();
                        curr = curr.Right;
                    }
                }
            }
            finally
            {
                if (curr.Parent != null)
                    curr.Parent.mutex.ReleaseMutex();

                curr.mutex.ReleaseMutex();
            }
        }
    }
}

