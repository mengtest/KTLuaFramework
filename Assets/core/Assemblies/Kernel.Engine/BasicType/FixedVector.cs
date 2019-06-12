using UnityEngine;

namespace Kernel.Engine
{
	public struct FixedVector3
	{
		public Fixed x;
		public Fixed y;
		public Fixed z;

		public static readonly FixedVector3 Zero = new FixedVector3(0, 0, 0);
		public static readonly FixedVector3 One = new FixedVector3(1, 1, 1);
		public static readonly FixedVector3 Forward = new FixedVector3(0, 0, 1);
		public static readonly FixedVector3 Back = new FixedVector3(0, 0, -1);
		public static readonly FixedVector3 Right = new FixedVector3(1, 0, 0);
		public static readonly FixedVector3 Left = new FixedVector3(-1, 0, 0);
		public static readonly FixedVector3 Up = new FixedVector3(0, 1, 0);
		public static readonly FixedVector3 Down = new FixedVector3(0, -1, 0);

		public FixedVector3 normalized
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

		public Fixed magnitude
		{
			get
			{
				return Fixed.Sqrt(sqrMagnitude);
			}
		}

		public Fixed sqrMagnitude
		{
			get
			{
				return x * x + y * y + z * z;
			}
		}

		public FixedVector3(Fixed x, Fixed y, Fixed z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static bool operator ==(FixedVector3 c1, FixedVector3 c2)
		{
			return c1.Equals(c2);
		}

		public static bool operator !=(FixedVector3 c1, FixedVector3 c2)
		{
			return !(c1 == c2);
		}

		public static FixedVector3 operator -(FixedVector3 c)
		{
			return new FixedVector3(-c.x, -c.y, -c.z);
		}

		public static FixedVector3 operator +(FixedVector3 c1, FixedVector3 c2)
		{
			return new FixedVector3(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
		}

		public static FixedVector3 operator -(FixedVector3 c1, FixedVector3 c2)
		{
			return new FixedVector3(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
		}

		public static FixedVector3 operator *(FixedVector3 c1, Fixed f)
		{
			return new FixedVector3(c1.x * f, c1.y * f, c1.z * f);
		}

		public static FixedVector3 operator *(Fixed f, FixedVector3 c1)
		{
			return new FixedVector3(c1.x * f, c1.y * f, c1.z * f);
		}
		
		public static FixedVector3 operator /(FixedVector3 c1, Fixed f)
		{
			return new FixedVector3(c1.x / f, c1.y / f, c1.z / f);
		}

		public static Fixed Dot(FixedVector3 lhs, FixedVector3 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		public static FixedVector3 Cross(FixedVector3 lhs, FixedVector3 rhs)
		{
			return new FixedVector3(
				lhs.y * rhs.z - lhs.z * rhs.y,
				lhs.z * rhs.x - lhs.x * rhs.z,
				lhs.x * rhs.y - lhs.y * rhs.x);
		}

		public Vector3 AsVector3()
		{
			return new Vector3(x.AsFloat(), y.AsFloat(), z.AsFloat());
		}
		public UnityEngine.Vector3 AsUnityVector3()
		{
			return new UnityEngine.Vector3(x.AsFloat(), y.AsFloat(), z.AsFloat());
		}
		public FixedVector2 ToVector2()
		{
			return new FixedVector2(x, z);
		}

		public static implicit operator FixedVector3(Vector3 c)
		{
			return new FixedVector3(c.x, c.y, c.z);
		}
		public static FixedVector3 FromVector3(Vector3 c) {
			return new FixedVector3(c.x, c.y, c.z);
		}
		public static FixedVector3 FromUnityVector3(UnityEngine.Vector3 c)
		{
			return new FixedVector3(c.x, c.y, c.z);
		}
		public override bool Equals(object c)
		{
			if(ReferenceEquals(null, c))
			{
				return false;
			}
			return c is FixedVector3 && Equals((FixedVector3)c);
		}

		public bool Equals(FixedVector3 other)
		{
			return x == other.x && y == other.y && z == other.z;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = x.GetHashCode();
				hashCode = (hashCode * 397) ^ y.GetHashCode();
				hashCode = (hashCode * 397) ^ z.GetHashCode();
				return hashCode;
			}
		}

		public override string ToString()
		{
			return string.Format("({0:F3}, {1:F3}, {2:F3})", x, y, z);
		}

		public void Normalize()
		{
			this = normalized;
		}

		public static FixedVector3 Scale(FixedVector3 a, FixedVector3 b)
		{
			return new FixedVector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static Fixed Distance(FixedVector3 pos, FixedVector3 center)
		{
			return (pos - center).magnitude;
		}
		public static Fixed SqrDistance(FixedVector3 pos, FixedVector3 center)
		{
			return (pos - center).sqrMagnitude;
		}
		public static Fixed SqrDistanceWithoutY(FixedVector3 pos, FixedVector3 center)
		{
			return (pos - center).SqrMagnitudeWithoutY();
		}
		public static Fixed DistanceIgnoreY(FixedVector3 pos, FixedVector3 center)
		{
			pos.y = 0;
			center.y = 0;
			return (pos - center).magnitude;
		}

		public FixedVector3 NormalizedWithoutY()
		{
			var ret = this;
			ret.y = 0;
			return ret.normalized;
		}

		public Fixed SqrMagnitudeWithoutY()
		{
			return x * x + z * z;
		}

		public static FixedVector3 Lerp(FixedVector3 a, FixedVector3 b, Fixed t)
		{
			t = FixedMathf.Clamp01(t);
			return new FixedVector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
		}

		public static Fixed Angle(FixedVector3 a, FixedVector3 b)
		{
			return Vector3.Angle(a.AsVector3(), b.AsVector3());
		}
	}
}
