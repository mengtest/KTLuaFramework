using System;
using System.Collections.Generic;

namespace kernel.core
{
    public interface IPriorityItem<K>
    {
        K Key { get; }
        float Value { get; }
    }

    public class PriorityQueue<K,T> where T: IPriorityItem<K>
    {
        private struct ItemWrap
        {
            public int index;
            public T Item;
        }

		private struct PriorityItemComparer : IComparer<ItemWrap>
        {
            int sign;

            public PriorityItemComparer(bool isASC=true)
            {
                this.sign = isASC?1:-1;
            }

            public int Compare(ItemWrap x, ItemWrap y)
            {
                if (x.Item.Value < y.Item.Value)
                    return 1* sign;
                else if (x.Item.Value > y.Item.Value)
                    return -1* sign;
                else
                    return 0;
            }
        }

        Dictionary<K, ItemWrap> map = new Dictionary<K, ItemWrap>(16);
        ItemWrap[] heap;
        IComparer<ItemWrap> comparer;

        public int Count { get; private set; }

        public PriorityQueue(bool isASC=true) : this(16, isASC) { }

        public PriorityQueue(int capacity, bool isASC) : this(capacity, new PriorityItemComparer(isASC)){}

        private PriorityQueue(int capacity,IComparer<ItemWrap> comparer)
        {
            this.comparer = (comparer == null) ? Comparer<ItemWrap>.Default : comparer;
            this.heap = new ItemWrap[capacity];
        }

        public PriorityQueue<K,T> Push(T v)
        {
            if (Count >= heap.Length) Array.Resize(ref heap, Count * 2);
            var val = new ItemWrap() { index = Count, Item = v };
            map.Add(v.Key,val);
            heap[Count] = val;
            SiftUp(Count++);
            return this;
        }

        public void Remove(K key)
        {
            if (map.ContainsKey(key))
            {
                RemoveAt(map[key].index);
                map.Remove(key);
            }
        }

        public T Pop()
        {
            var v = Top();
            heap[0] = heap[--Count];
            if (Count > 0) SiftDown(0);
            return v;
        }

        public T Top()
        {
            if (Count > 0) return heap[0].Item;
            throw new InvalidOperationException("优先队列为空");
        }

        public void Clear()
        {
            map.Clear();
            Array.Clear(heap, 0, Count);
            Count = 0;
        }

        void RemoveAt(int n)
        {
            var v = Item(n);
            heap[n] = heap[--Count];
            heap[Count].index = n;
            if (Count > 0) SiftDown(n);
        }

        T Item(int n)
        {
            if (Count ==0)
                throw new InvalidOperationException("优先队列为空");

            if (n < 0 || n > Count)
                throw new OverflowException("索引非法");

            return heap[n].Item;
        }

        void SiftUp(int n)
        {
            var v = heap[n];
            for (var n2 = n / 2; n > 0 && comparer.Compare(v, heap[n2]) > 0; n = n2, n2 /= 2)
            {
                heap[n] = heap[n2];
                heap[n].index = n;
            }

            heap[n] = v;
            heap[n].index = n;
        }

        void SiftDown(int n)
        {
            var v = heap[n];
            for (var n2 = n * 2; n2 < Count; n = n2, n2 *= 2)
            {
                if (n2 + 1 < Count && comparer.Compare(heap[n2 + 1], heap[n2]) > 0) n2++;
                if (comparer.Compare(v, heap[n2]) >= 0) break;
                heap[n] = heap[n2];
                heap[n].index = n;
            }
            heap[n] = v;
            heap[n].index = n;
        }
    }
}


//namespace ConsoleApp1
//{
//    class Program
//    {
//        public struct Node<K> : IPriorityItem<K>
//        {
//            public K Key{
//                get;
//                set;
//            }

//            public float Value { get; set; }
//        }

//        static void Main(string[] args)
//        {
//            PriorityQueue<string, Node<string>> q = new PriorityQueue<string, Node<string>>();

//            q.Push(new Node<string>() { Key = "a", Value = 0.5f })
//            .Push(new Node<string>() { Key = "b", Value = 0.3f })
//            .Push(new Node<string>() { Key = "c", Value = 0.4f })
//            .Push(new Node<string>() { Key = "d", Value = 0.9f })
//            .Push(new Node<string>() { Key = "e", Value = 1f })
//            .Push(new Node<string>() { Key = "f", Value = 5f })
//            .Push(new Node<string>() { Key = "g", Value = 0.1f })
//            .Push(new Node<string>() { Key = "h", Value = 0.2f }).Remove("f");

//            while (q.Count>0)
//            {
//                Console.WriteLine(q.Pop().Value);
//            }

//            Console.Read();
//        }
//    }
//}
