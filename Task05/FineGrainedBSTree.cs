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
        private Mutex _mutex = new Mutex();

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

        public bool Find(int value)
        {
            Node curr = _root;

            if (curr == null)
                return false;

            curr.mutex.WaitOne();

            while (true)
            {
                if (curr.Value == value)
                {
                    if (curr.Parent != null)
                        curr.Parent.mutex.ReleaseMutex();

                    curr.mutex.ReleaseMutex();
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

            if (curr.Parent != null)
                curr.Parent.mutex.ReleaseMutex();

            curr.mutex.ReleaseMutex();
            return false;
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

            while (true)
            {
                if (curr.Value == value)
                {
                    if (curr.Parent != null)
                        curr.Parent.mutex.ReleaseMutex();

                    curr.mutex.ReleaseMutex();
                    return;
                }
                else if (curr.Value > value)
                {
                    if (curr.Left == null)
                    {
                        curr.Left = new Node(value, curr);

                        if (curr.Parent != null)
                            curr.Parent.mutex.ReleaseMutex();

                        curr.mutex.ReleaseMutex();
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

                        if (curr.Parent != null)
                            curr.Parent.mutex.ReleaseMutex();

                        curr.mutex.ReleaseMutex();
                        return;
                    }

                    if (curr.Parent != null)
                        curr.Parent.mutex.ReleaseMutex();

                    curr.Right.mutex.WaitOne();
                    curr = curr.Right;
                }
            }
        }
    }
}

