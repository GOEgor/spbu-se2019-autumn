using System;
using System.Threading.Tasks;

namespace Task02
{
    class Sort
    {
        private static readonly Random Random = new Random();

        private const int parallelMinDiff = 300;

        public static void StartSortSeq<T>(T[] items) where T : IComparable<T>
        {
            QuickSortSeq(items, 0, items.Length);
        }

        private static void QuickSortSeq<T>(T[] items, int left, int right) where T : IComparable<T>
        {
            if (right - left < 2)
                return;

            int pivot = Partition(items, left, right);
            QuickSortSeq(items, left, pivot);
            QuickSortSeq(items, pivot + 1, right);
        }

        public static void StartSortPar<T>(T[] items) where T : IComparable<T>
        {
            QuickSortPar(items, 0, items.Length);
        }

        private static void QuickSortPar<T>(T[] items, int left, int right) where T : IComparable<T>
        {
            if (right - left < 2)
                return;

            int pivot = Partition(items, left, right);
            if (right - left > parallelMinDiff)
            {
                Task leftTask = Task.Run(() => QuickSortPar(items, left, pivot));
                Task rightTask = Task.Run(() => QuickSortPar(items, pivot + 1, right));

                Task.WaitAll(leftTask, rightTask);
            }
            else
            {
                QuickSortSeq(items, left, pivot);
                QuickSortSeq(items, pivot + 1, right);
            }
        }

        private static int Partition<T>(T[] items, int left, int right) where T : IComparable<T>
        {
            int pivotPos = Random.Next(left, right);
            T pivotValue = items[pivotPos];

            Swap(ref items[right - 1], ref items[pivotPos]);
            int store = left;

            for (int index = left; index < right - 1; ++index)
            {
                if (items[index].CompareTo(pivotValue) < 0)
                {
                    Swap(ref items[index], ref items[store]);
                    ++store;
                }
            }

            Swap(ref items[right - 1], ref items[store]);
            return store;
        }

        private static void Swap<T>(ref T firstElem, ref T secondElem)
        {
            T temp = firstElem;
            firstElem = secondElem;
            secondElem = temp;
        }
    }
}
