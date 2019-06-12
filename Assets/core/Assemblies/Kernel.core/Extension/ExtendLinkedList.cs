using System;
using System.Collections.Generic;

namespace Kernel.core
{
	public static class ExtendLinkedList
	{
		public static LinkedListNode<T> FindEx<T>(this LinkedList<T> list, Predicate<T> predicate)
		{
			if(null != list && null != predicate)
			{
				var first = list.First;
				if(null != first)
				{
					var next = first;
					while(!predicate(next.Value))
					{
						next = next.Next;
						if(null == next || next == first)
						{
							return null;
						}
					}

					return next;
				}
			}

			return null;
		}
	}
}