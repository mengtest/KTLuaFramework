using System.Collections.Generic;

namespace Kernel.core
{
	public class RecycledLinkedList<T> : LinkedList<T>
	{
		private static Stack<LinkedListNode<T>> pool = new Stack<LinkedListNode<T>>();

		public new LinkedListNode<T> AddLast(T value)
		{
			var n = Spawn(value);
			AddLast(n);
			return n;
		}

		public new LinkedListNode<T> AddFirst(T value)
		{
			var n = Spawn(value);
			AddFirst(n);
			return n;
		}

		public new void RemoveFirst()
		{
			var n = First;
			base.RemoveFirst();

			Recycle(n);
		}

		public new void RemoveLast()
		{
			var n = Last;
			base.RemoveLast();

			Recycle(n);
		}

		private static void Recycle(LinkedListNode<T> n)
		{
			if(n != null)
			{
				n.Value = default(T);
				pool.Push(n);
			}
		}

		private static LinkedListNode<T> Spawn(T value)
		{
			if(pool.Count > 0)
			{
				var n = pool.Pop();
				n.Value = value;
				return n;
			}
			return new LinkedListNode<T>(value);
		}
	}
}
