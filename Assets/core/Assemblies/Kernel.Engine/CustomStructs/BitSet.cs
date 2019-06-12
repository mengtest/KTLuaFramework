using System;
using UnityEngine;

namespace Kernel.core
{
	/// <summary>
	/// by lijunfeng 2019/6/3
	/// </summary>
	public struct BitSet
	{
		private ulong[] segments;
		private int segmentsLength;

		private BitSet(ulong[] segments, int segmentsLength)
		{
			this.segments = segments;
			this.segmentsLength = segmentsLength;
		}

		public BitSet(bool[] values):this()
		{
			var length = values.Length;
			CreateSegements(length);
			for (int i = length-1;i>-1;--i)
			{
				this[length-1-i] = values[i];
			}
		}

		public BitSet(int[] values) : this()
		{
			var length = values.Length;
			CreateSegements(length);
			for (int i = length - 1; i > -1; --i)
			{
				Set(length - 1 - i, values[i]);
			}
		}

		public BitSet(BitSet bits)
		{
			segments = bits.Segments;
			segmentsLength = bits.SegmentsLength;
		}

		public BitSet(ulong[] segments)
		{
			this.segments = segments;
			segmentsLength = segments.Length;
		}

		public BitSet(int length) : this()
		{
			CreateSegements(length);
		}

		public BitSet(int length, bool defaultValue) : this()
		{
			CreateSegements(length);
			SetAll(defaultValue);
		}

		public bool this[int index]
		{
			get
			{
				var segmentIndex = Mathf.CeilToInt((index + 1) / 64f) - 1;
				var bitIndex = index - (segmentIndex << 6);
				return (segments[segmentIndex] &((ulong)1<< bitIndex))!=0;
			}
			set
			{
				var segmentIndex = Mathf.CeilToInt((index + 1) / 64f) - 1;
				var bitIndex = index - (segmentIndex << 6);
				segments[segmentIndex] = value
					? segments[segmentIndex] | ((ulong) 1 << bitIndex)
					: segments[segmentIndex] & (~((ulong) 1 << bitIndex));
			}
		}

		public bool IsZero
		{
			get
			{
				for (int i = 0; i < segmentsLength; ++i)
				{
					if (segments[i] != 0)
						return false;
				}

				return true;
			}
		}

		public int Count
		{
			get { return segmentsLength<<6; }
		}

		public ulong[] Segments
		{
			get { return segments; }
		}

		public int SegmentsLength
		{
			get { return segmentsLength; }
		}

		public void SetAll(bool value)
		{
			for (int i = 0; i < segmentsLength; ++i)
			{
				segments[i] = (ulong)(value ? 1:0);
			}
		}

		private void CreateSegements(int length)
		{
			segmentsLength = Mathf.CeilToInt(length/64f);
			segments = new ulong[segmentsLength];
		}

		private void Set(int index, int value)
		{
			var segmentIndex = Mathf.CeilToInt((index + 1) / 64f)-1;
			var bitIndex = index - (segmentIndex << 6);
			if (value == 1)
			{
				segments[segmentIndex] |= ((ulong)1 << bitIndex);
			}
			else if (value == 0)
			{
				segments[segmentIndex] &= (~((ulong)1 << bitIndex));
			}
			else
			{
				throw new Exception("invalid argument value,it must be 0 or 1");
			}
		}

		public static BitSet operator &(BitSet L, BitSet R)
		{
			if (L.segmentsLength != R.segmentsLength)
			{
				throw new Exception("SegmenstLength is not equals");
			}

			var segmenst = new ulong[L.segmentsLength];
			for (int i = 0; i < L.segmentsLength; ++i)
			{
				segmenst[i] = L.segments[i] & R.segments[i];
			}

			return new BitSet(segmenst);
		}

		//左值改变,避免创建新对象
		public BitSet AndEquals(BitSet R)
		{
			if (segmentsLength != R.segmentsLength)
			{
				throw new Exception("SegmenstLength is not equals");
			}

			for (int i = 0; i < segmentsLength; ++i)
			{
				segments[i] &= R.segments[i];
			}

			return this;
		}

		public static BitSet operator |(BitSet L, BitSet R)
		{
			if (L.segmentsLength != R.segmentsLength)
			{
				throw new Exception("SegmenstLength is not equals");
			}

			var segmenst = new ulong[L.segmentsLength];
			for (int i = 0; i < L.segmentsLength; ++i)
			{
				segmenst[i] = L.segments[i] | R.segments[i];
			}

			return new BitSet(segmenst);
		}

		//左值改变,避免创建新对象
		public BitSet OrEquals(BitSet R)
		{
			if (segmentsLength != R.segmentsLength)
			{
				throw new Exception("SegmenstLength is not equals");
			}

			for (int i = 0; i < segmentsLength; ++i)
			{
				segments[i] |= R.segments[i];
			}

			return this;
		}

		public static BitSet operator ^(BitSet L, BitSet R)
		{
			if (L.segmentsLength != R.segmentsLength)
			{
				throw new Exception("SegmenstLength is not equals");
			}

			var segmenst = new ulong[L.segmentsLength];
			for (int i = 0; i < L.segmentsLength; ++i)
			{
				segmenst[i] = L.segments[i] ^ R.segments[i];
			}

			return new BitSet(segmenst);
		}

		//左值改变,避免创建新对象
		public BitSet XorEquals(BitSet R)
		{
			if (segmentsLength != R.segmentsLength)
			{
				throw new Exception("SegmenstLength is not equals");
			}

			for (int i = 0; i < segmentsLength; ++i)
			{
				segments[i] ^= R.segments[i];
			}

			return this;
		}

		public static BitSet operator ~(BitSet L)
		{
			var segmenst = new ulong[L.segmentsLength];
			for (int i = 0; i < L.segmentsLength; ++i)
			{
				segmenst[i] = ~L.segments[i];
			}

			return new BitSet(segmenst);
		}

		//左值改变,避免创建新对象
		public BitSet NotEquals()
		{
			for (int i = 0; i < segmentsLength; ++i)
			{
				segments[i] =~segments[i];
			}

			return this;
		}

		//慎用，效率低
		public static BitSet operator <<(BitSet L, int n)
		{
			var length = L.segmentsLength << 6;
			if (n > length)
			{
				throw new Exception("n is over bit length");
			}

			var bitSet = new BitSet(length);
			for (int i =length-1; i >=n; --i)
			{
				bitSet[i] = L[i-n];
			}

			return bitSet;
		}

		//慎用，效率低
		public static BitSet operator >>(BitSet L, int n)
		{
			var length = L.segmentsLength << 6;
			if (n > length)
			{
				throw new Exception("n is over bit length");
			}

			var bitSet = new BitSet(length);
			for (int i = 0,j=length-n; i <j; ++i)
			{
				bitSet[i] = L[i+n];
			}

			return bitSet;
		}

		public static bool operator ==(BitSet L, BitSet R)
		{
			if (L.segmentsLength != R.segmentsLength)
			{
				throw new Exception("SegmenstLength is not equals");
			}

			for (int i = 0; i < L.segmentsLength; ++i)
			{
				if (L.segments[i] != R.segments[i])
					return false;
			}

			return true;
		}

		public static bool operator !=(BitSet L, BitSet R)
		{
			if (L.segmentsLength != R.segmentsLength)
			{
				throw new Exception("SegmenstLength is not equals");
			}

			for (int i = 0; i < L.segmentsLength; ++i)
			{
				if (L.segments[i] == R.segments[i])
					return true;
			}

			return false;
		}
	}

}

