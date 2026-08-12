using System;
using System.Collections.Generic;

namespace RailHexLib
{
    /// <summary>
    /// Минимальная реализация min-heap очереди с приоритетами, совместимая
    /// по API с System.Collections.Generic.PriorityQueue<TElement, TPriority>
    /// (.NET 6+). Нужна для сборки под netstandard2.1 (Unity Mono), где
    /// стандартного типа нет.
    ///
    /// Меньший приоритет извлекается первым.
    /// </summary>
    public class PriorityQueue<TElement, TPriority>
    {
        private readonly List<(TElement Element, TPriority Priority)> heap = new();
        private readonly IComparer<TPriority> comparer;

        public PriorityQueue()
        {
            comparer = Comparer<TPriority>.Default;
        }

        public PriorityQueue(IComparer<TPriority> comparer)
        {
            this.comparer = comparer ?? Comparer<TPriority>.Default;
        }

        public int Count => heap.Count;

        public void Enqueue(TElement element, TPriority priority)
        {
            heap.Add((element, priority));
            SiftUp(heap.Count - 1);
        }

        public TElement Dequeue()
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("PriorityQueue is empty");

            var root = heap[0];
            int last = heap.Count - 1;
            heap[0] = heap[last];
            heap.RemoveAt(last);
            if (heap.Count > 0)
                SiftDown(0);
            return root.Element;
        }

        public TElement Peek()
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("PriorityQueue is empty");
            return heap[0].Element;
        }

        public bool TryDequeue(out TElement element, out TPriority priority)
        {
            if (heap.Count == 0)
            {
                element = default;
                priority = default;
                return false;
            }
            priority = heap[0].Priority;
            element = Dequeue();
            return true;
        }

        public void Clear() => heap.Clear();

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (comparer.Compare(heap[index].Priority, heap[parent].Priority) >= 0)
                    break;
                (heap[index], heap[parent]) = (heap[parent], heap[index]);
                index = parent;
            }
        }

        private void SiftDown(int index)
        {
            int count = heap.Count;
            while (true)
            {
                int left = 2 * index + 1;
                int right = 2 * index + 2;
                int smallest = index;

                if (left < count &&
                    comparer.Compare(heap[left].Priority, heap[smallest].Priority) < 0)
                    smallest = left;
                if (right < count &&
                    comparer.Compare(heap[right].Priority, heap[smallest].Priority) < 0)
                    smallest = right;

                if (smallest == index)
                    break;

                (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
                index = smallest;
            }
        }
    }
}
