using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task05
{
    class CoarseGrainedBSTree
    {
        public class Node
        {
            public int Value;
            public Node Left;
            public Node Right;
            public Node Parent;

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
            var isUnlocked = _mutex.WaitOne(0);
            if (!isUnlocked)
                return false;

            _mutex.ReleaseMutex();

            return true;
        }

        public void DisposeMutex()
        {
            _mutex.Close();
        }

        public bool Find(int value)
        {
            _mutex.WaitOne();
            Node curr = _root;

            try
            {
                while (curr != null)
                {
                    if (curr.Value == value)
                    {
                        return true;
                    }

                    curr = curr.Value > value ? curr.Left : curr.Right;
                }

                return false;
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void Insert(int value)
        {
            _mutex.WaitOne();
            Node curr = _root;

            try
            {
                while (curr != null)
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

                        curr = curr.Left;
                    }
                    else
                    {
                        if (curr.Right == null)
                        {
                            curr.Right = new Node(value, curr);
                            return;
                        }

                        curr = curr.Right;
                    }
                }

                _root = new Node(value, null);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }
    }
}

