using UnityEngine;

namespace Kernel.Engine
{
	public struct FixedVector2
	{
		public Fixed x;
		public Fixed y;

		public static readonly FixedVector2 Zero = new FixedVector2(0, 0);

		public FixedVector2(Fixed x, Fixed y)
		{
			this.x = x;
			this.y = y;
		}

		public Fixed sqrMagnitude
		{
			get
			{
				return x * x + y * y;
			}
		}

		public FixedVector2 normalized
		{
			get
			{
				var num = magnitude;
				if(num > 0)
				{
					return this / num;
				}
				return Zero;
			}
		}

		public FixedVector2 orthogonal
		{
			get
			{
				return new FixedVector2(y, -x);
			}
		}

		public Fixed magnitude
		{
			get
			{
				return Fixed.Sqrt(sqrMagnitude);
			}
		}

		public static bool operator ==(FixedVector2 c1, FixedVector2 c2)
		{
			return c1.Equals(c2);
		}

		public static bool operator !=(FixedVector2 c1, FixedVector2 c2)
		{
			return !(c1 == c2);
		}

		public static FixedVector2 operator -(FixedVector2 c)
		{
			return new FixedVector2(-c.x, -c.y);
		}

		public static FixedVector2 operator +(FixedVector2 c1, FixedVector2 c2)
		{
			return new FixedVector2(c1.x + c2.x, c1.y + c2.y);
		}

		public static FixedVector2 operator -(FixedVector2 c1, FixedVector2 c2)
		{
			return new FixedVector2(c1.x - c2.x, c1.y - c2.y);
		}

		public static FixedVector2 operator *(FixedVector2 c1, Fixed f)
		{
			return new FixedVector2(c1.x * f, c1.y * f);
		}

		public static FixedVector2 operator *(Fixed f, FixedVector2 c1)
		{
			return new FixedVector2(c1.x * f, c1.y * f);
		}

		public static FixedVector2 operator /(FixedVector2 c1, Fixed f)
		{
			return new FixedVector2(c1.x / f, c1.y / f);
		}

		public static implicit operator FixedVector3(FixedVector2 v)
		{
			return new FixedVector3(v.x, v.y, 0.0f);
		}

		public static implicit operator FixedVector2(FixedVector3 v)
		{
			return new FixedVector2(v.x, v.y);
		}

		public static Fixed Dot(FixedVector2 lhs, FixedVector2 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y;
		}

		/// <summary>
		///   <para>Linearly interpolates between vectors a and b by t.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		public static FixedVector2 Lerp(FixedVector2 a, FixedVector2 b, Fixed t)
		{
			t = FixedMathf.Clamp01(t);
			return new FixedVector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
		}

		/// <summary>
		///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="maxLength"></param>
		public static FixedVector2 ClampMagnitude(FixedVector2 vector, Fixed maxLength)
		{
			if (vector.sqrMagnitude > maxLength * maxLength)
				return vector.normalized * maxLength;
			return vector;
		}

		public override bool Equals(object c)
		{
			if(ReferenceEquals(null, c))
			{
				return false;
			}
			return c is FixedVector2 && Equals((FixedVector2)c);
		}

		public bool Equals(FixedVector2 other)
		{
			return x == other.x && y == other.y;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = x.GetHashCode();
				hashCode = (hashCode * 397) ^ y.GetHashCode();
				return hashCode;
			}
		}

		public override string ToString()
		{
			return string.Format("({0:F3}, {1:F3})", x, y);
		}

		public void Normalize()
		{
			this = normalized;
		}

		public Vector2 AsVector2()
		{
			return new Vector2(x.AsFloat(), y.AsFloat());
		}
	}
}
