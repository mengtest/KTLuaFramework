using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Kernel.Engine
{
	public struct FixedQuaternion
	{
		public Fixed x;
		public Fixed y;
		public Fixed z;
		public Fixed w;

		public static FixedQuaternion Identity = new FixedQuaternion(Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.One);

		public FixedVector3 eulerAngles
		{
			get
			{
				var sp = new Fixed(-2) * (y * z - w * x);

				if (FixedMathf.Abs(sp) > 0.999999)
				{
					return new FixedVector3(
						90 * sp,
						FixedMathf.Atan2(-x * z + w * y, Fixed.Half - y * y - z * z) * Fixed.Rad2Deg,
						0
					);
				}
				return new FixedVector3(
					FixedMathf.Asin(sp) * Fixed.Rad2Deg,
					FixedMathf.Atan2(x * z + w * y, Fixed.Half - x * x - y * y) * Fixed.Rad2Deg,
					FixedMathf.Atan2(x * y + w * z, Fixed.Half - x * x - z * z) * Fixed.Rad2Deg
				);
			}
		}

		public FixedQuaternion(Fixed x, Fixed y, Fixed z, Fixed w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public FixedQuaternion(FixedVector3 vector3, Fixed w)
		{
			x = vector3.x;
			y = vector3.y;
			z = vector3.z;
			this.w = w;
		}

		public Fixed this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return x;
					case 1:
						return y;
					case 2:
						return z;
					case 3:
						return w;
					default:
						throw  new Exception("index over load");
				}
			}

			set
			{
				switch (index)
				{
					case 0:
						x=value;
						break;
					case 1:
						y=value;
						break;
					case 2:
						z = value;
						break;
					case 3:
						w = value;
						break;
					default:
						throw new Exception("index over load");
				}
			}
		}

		public Fixed magnitude
		{
			get
			{
				return FixedMathf.Sqrt(sqrMagnitude);
			}
		}

		public Fixed sqrMagnitude
		{
			get
			{
				return x * x + y * y + z * z + w * w;
			}
		}

		public Quaternion AsQuaternion()
		{
			return new Quaternion(x.AsFloat(), y.AsFloat(), z.AsFloat(), w.AsFloat());
		}
		public UnityEngine.Quaternion AsUnityQuaternion()
		{
			return new UnityEngine.Quaternion(x.AsFloat(), y.AsFloat(), z.AsFloat(), w.AsFloat());
		}
		public static FixedQuaternion FromUnityQuaternion(UnityEngine.Quaternion quaternion)
		{
			return new FixedQuaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
		}
		public static FixedQuaternion Normalize(FixedQuaternion c)
		{
			Fixed scale = Fixed.One / c.magnitude;
			return new FixedQuaternion(c.x * scale, c.y * scale, c.z * scale, c.w * scale);
		}

		public static implicit operator FixedQuaternion(Quaternion c)
		{
			return new FixedQuaternion(c.x, c.y, c.z, c.w);
		}

		public static Fixed Dot(FixedQuaternion a, FixedQuaternion b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		public static FixedQuaternion LookRotation(FixedVector3 forward)
		{
			FixedVector3 up = FixedVector3.Up;

			Fixed f = FixedVector3.Dot(up, forward);
			if(f == -1)
			{
				up = FixedVector3.Forward;
			}
			else if(f == 1)
			{
				up = FixedVector3.Back;
			}

			return LookRotation(forward, up);
		}

		public static FixedQuaternion LookRotation(FixedVector3 forward, FixedVector3 upwards)
		{
			forward = forward.normalized;
			FixedVector3 right = FixedVector3.Cross(upwards, forward).normalized;
			upwards = FixedVector3.Cross(forward, right);
			var rightX = right.x;
			var rightY = right.y;
			var rightZ = right.z;
			var upwardsX = upwards.x;
			var upwardsY = upwards.y;
			var upwardsZ = upwards.z;
			var forwardX = forward.x;
			var forwardY = forward.y;
			var forwardZ = forward.z;


			Fixed temp1 = rightX + upwardsY + forwardZ;
			var quaternion = new FixedQuaternion();
			if (temp1 > Fixed.Zero)
			{
				var num = FixedMathf.Sqrt(temp1 + Fixed.One);
				quaternion.w = num * Fixed.Half;
				num = Fixed.Half / num;
				quaternion.x = (upwardsZ - forwardY) * num;
				quaternion.y = (forwardX - rightZ) * num;
				quaternion.z = (rightY - upwardsX) * num;
				return quaternion;
			}
			if (rightX >= upwardsY && rightX >= forwardZ)
			{
				var temp2 = FixedMathf.Sqrt(Fixed.One + rightX - upwardsY - forwardZ);
				var temp3 = Fixed.Half / temp2;
				quaternion.x = Fixed.Half * temp2;
				quaternion.y = (rightY + upwardsX) * temp3;
				quaternion.z = (rightZ + forwardX) * temp3;
				quaternion.w = (upwardsZ - forwardY) * temp3;
				return quaternion;
			}
			if (upwardsY > forwardZ)
			{
				var temp4 = FixedMathf.Sqrt(Fixed.One + upwardsY - rightX - forwardZ);
				var temp5 = Fixed.Half / temp4;
				quaternion.x = (upwardsX + rightY) * temp5;
				quaternion.y = Fixed.Half * temp4;
				quaternion.z = (forwardY + upwardsZ) * temp5;
				quaternion.w = (forwardX - rightZ) * temp5;
				return quaternion;
			}
			var temp6 = FixedMathf.Sqrt(Fixed.One + forwardZ - rightX - upwardsY);
			var temp7 = Fixed.Half / temp6;
			quaternion.x = (forwardX + rightZ) * temp7;
			quaternion.y = (forwardY + upwardsZ) * temp7;
			quaternion.z = Fixed.Half * temp6;
			quaternion.w = (rightY - upwardsX) * temp7;
			return quaternion;
		}

		public static FixedQuaternion Lerp(FixedQuaternion a, FixedQuaternion b, Fixed f)
		{
			Fixed tempF = f;
			Fixed temp1 = Fixed.One - tempF;
			FixedQuaternion result = new FixedQuaternion();
			Fixed temp2 = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
			if(temp2 >= Fixed.Zero)
			{
				result.x = temp1 * a.x + tempF * b.x;
				result.y = temp1 * a.y + tempF * b.y;
				result.z = temp1 * a.z + tempF * b.z;
				result.w = temp1 * a.w + tempF * b.w;
			}
			else
			{
				result.x = temp1 * a.x - tempF * b.x;
				result.y = temp1 * a.y - tempF * b.y;
				result.z = temp1 * a.z - tempF * b.z;
				result.w = temp1 * a.w - tempF * b.w;
			}
			Fixed temp3 = Fixed.One / result.magnitude;
			result.x *= temp3;
			result.y *= temp3;
			result.z *= temp3;
			result.w *= temp3;
			return result;
		}

		public static FixedQuaternion Slerp(FixedQuaternion a, FixedQuaternion b, Fixed f)
		{
			if(a.sqrMagnitude == Fixed.Zero)
			{
				if(b.sqrMagnitude == Fixed.Zero)
				{
					return Identity;
				}
				return b;
			}
			if(b.sqrMagnitude == Fixed.Zero)
			{
				return a;
			}

			if (f > Fixed.One)
			{
				f = Fixed.One;
			}
			if(f < Fixed.Zero)
			{
				f = Fixed.Zero;
			}

			Fixed cosHalfAngle = a.w * b.w + FixedVector3.Dot(new FixedVector3(a.x, a.y, a.z), new FixedVector3(b.x, b.y, b.z));
			if(cosHalfAngle >= Fixed.One || cosHalfAngle <= -Fixed.One)
			{
				return a;
			}
			if(cosHalfAngle < Fixed.Zero)
			{
				b.x = -b.x;
				b.y = -b.y;
				b.z = -b.z;
				b.w = -b.w;
				cosHalfAngle = -cosHalfAngle;
			}

			Fixed blendA;
			Fixed blendB;
			if(cosHalfAngle < 0.999999)
			{
				Fixed halfAngle = FixedMathf.Acos(cosHalfAngle);
				Fixed sinHalfAngle = FixedMathf.Sin(halfAngle);
				Fixed oneOverSinHalfAngle = Fixed.One / sinHalfAngle;
				blendA = FixedMathf.Sin(halfAngle * (Fixed.One - f)) * oneOverSinHalfAngle;
				blendB = FixedMathf.Sin(halfAngle * f) * oneOverSinHalfAngle;
			}
			else
			{
				blendA = Fixed.One - f;
				blendB = f;
			}
			FixedQuaternion result =
				new FixedQuaternion(blendA * new FixedVector3(a.x, a.y, a.z) + blendB * new FixedVector3(b.x, b.y, b.z),
					blendA * a.w + blendB * b.w);
			if(result.sqrMagnitude > Fixed.Zero)
			{
				return Normalize(result);
			}
			return Identity;
		}

		public static FixedQuaternion AngleAxis(Fixed degree, FixedVector3 axis)
		{
			if(axis.sqrMagnitude < Fixed.EN6)
			{
				return Identity;
			}
			
			axis = axis.normalized;
			var cos = FixedMathf.Cos(degree * Fixed.Deg2Rad / new Fixed(2));
			var sin = FixedMathf.Sin(degree * Fixed.Deg2Rad / new Fixed(2));

			return new FixedQuaternion(sin * axis.x, sin * axis.y, sin * axis.z, cos);
		}

		public static Fixed Angle(FixedQuaternion a, FixedQuaternion b)
		{
			Fixed f = Dot(a, b);
			return FixedMathf.Acos(FixedMathf.Min(FixedMathf.Abs(f), Fixed.One).AsFloat()) * new Fixed(2) * Fixed.Rad2Deg;
		}

		public static FixedQuaternion Euler(FixedVector3 v)
		{
			var sinz = FixedMathf.Sin(v.z / 2 * Fixed.Deg2Rad);
			var cosz = FixedMathf.Cos(v.z / 2 * Fixed.Deg2Rad);
			var sinx = FixedMathf.Sin(v.x / 2 * Fixed.Deg2Rad);
			var cosx = FixedMathf.Cos(v.x / 2 * Fixed.Deg2Rad);
			var siny = FixedMathf.Sin(v.y / 2 * Fixed.Deg2Rad);
			var cosy = FixedMathf.Cos(v.y / 2 * Fixed.Deg2Rad);

			return new FixedQuaternion(
				cosy * sinx * cosz + siny * cosx * sinz,
				siny * cosx * cosz - cosy * sinx * sinz,
				cosy * cosx * sinz - siny * sinx * cosz,
				cosy * cosx * cosz + siny * sinx * sinz
			);
		}

		//////////////////////////////////////////////////////////
		// 下面的代码计算量大，尽量不要用
		public static FixedVector3 operator *(FixedQuaternion rotation, FixedVector3 point)
		{
			var num1 = rotation.x * 2;
			var num2 = rotation.y * 2;
			var num3 = rotation.z * 2;
			var num4 = rotation.x * num1;
			var num5 = rotation.y * num2;
			var num6 = rotation.z * num3;
			var num7 = rotation.x * num2;
			var num8 = rotation.x * num3;
			var num9 = rotation.y * num3;
			var num10 = rotation.w * num1;
			var num11 = rotation.w * num2;
			var num12 = rotation.w * num3;
			FixedVector3 vector3;
			vector3.x = (1 - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
			vector3.y = (num7 + num12) * point.x + (1 - (num4 + num6)) * point.y + (num9 - num10) * point.z;
			vector3.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1 - (num4 + num5)) * point.z;
			return vector3;
		}

		public static bool operator ==(FixedQuaternion c1, FixedQuaternion c2)
		{
			return c1.Equals(c2);
		}

		public static bool operator !=(FixedQuaternion c1, FixedQuaternion c2)
		{
			return !(c1 == c2);
		}

		public override bool Equals(object c)
		{
			if(ReferenceEquals(null, c))
			{
				return false;
			}
			return c is FixedQuaternion && Equals((FixedQuaternion)c);
		}

		public bool Equals(FixedQuaternion other)
		{
			return x == other.x && y == other.y && z == other.z && w == other.w;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = w.GetHashCode();
				hashCode = (hashCode * 397) ^ x.GetHashCode();
				hashCode = (hashCode * 397) ^ y.GetHashCode();
				hashCode = (hashCode * 397) ^ z.GetHashCode();
				return hashCode;
			}
		}

		public override string ToString()
		{
			return string.Format("({0:F3}, {1:F3}, {2:F3}, {3:F3})", x, y, z, w);
		}
	}
}
