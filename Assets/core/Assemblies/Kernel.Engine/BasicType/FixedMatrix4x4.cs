
using System;

namespace Kernel.Engine
{
    public struct FixedMatrix4x4
	{
		private static readonly FixedMatrix4x4 identityMatrix = new FixedMatrix4x4(new FixedQuaternion(1f, 0.0f, 0.0f, 0.0f), new FixedQuaternion(0.0f, 1f, 0.0f, 0.0f), new FixedQuaternion(0.0f, 0.0f, 1f, 0.0f), new FixedQuaternion(0.0f, 0.0f, 0.0f, 1f));

		public Fixed m00;
		public Fixed m10;
		public Fixed m20;
		public Fixed m30;
		public Fixed m01;
		public Fixed m11;
		public Fixed m21;
		public Fixed m31;
		public Fixed m02;
		public Fixed m12;
		public Fixed m22;
		public Fixed m32;
		public Fixed m03;
		public Fixed m13;
		public Fixed m23;
		public Fixed m33;

		/// <summary>
		///   <para>Returns the identity matrix (Read Only).</para>
		/// </summary>
		public static FixedMatrix4x4 identity
		{
			get
			{
				return FixedMatrix4x4.identityMatrix;
			}
		}

		public FixedMatrix4x4(FixedQuaternion column0, FixedQuaternion column1, FixedQuaternion column2, FixedQuaternion column3)
		{
			this.m00 = column0.x;
			this.m01 = column1.x;
			this.m02 = column2.x;
			this.m03 = column3.x;
			this.m10 = column0.y;
			this.m11 = column1.y;
			this.m12 = column2.y;
			this.m13 = column3.y;
			this.m20 = column0.z;
			this.m21 = column1.z;
			this.m22 = column2.z;
			this.m23 = column3.z;
			this.m30 = column0.w;
			this.m31 = column1.w;
			this.m32 = column2.w;
			this.m33 = column3.w;
		}

		public static FixedMatrix4x4 operator *(FixedMatrix4x4 lhs, FixedMatrix4x4 rhs)
		{
			FixedMatrix4x4 matrix4x4;
			matrix4x4.m00 = (Fixed)((Fixed)lhs.m00 * (Fixed)rhs.m00 + (Fixed)lhs.m01 * (Fixed)rhs.m10 + (Fixed)lhs.m02 * (Fixed)rhs.m20 + (Fixed)lhs.m03 * (Fixed)rhs.m30);
			matrix4x4.m01 = (Fixed)((Fixed)lhs.m00 * (Fixed)rhs.m01 + (Fixed)lhs.m01 * (Fixed)rhs.m11 + (Fixed)lhs.m02 * (Fixed)rhs.m21 + (Fixed)lhs.m03 * (Fixed)rhs.m31);
			matrix4x4.m02 = (Fixed)((Fixed)lhs.m00 * (Fixed)rhs.m02 + (Fixed)lhs.m01 * (Fixed)rhs.m12 + (Fixed)lhs.m02 * (Fixed)rhs.m22 + (Fixed)lhs.m03 * (Fixed)rhs.m32);
			matrix4x4.m03 = (Fixed)((Fixed)lhs.m00 * (Fixed)rhs.m03 + (Fixed)lhs.m01 * (Fixed)rhs.m13 + (Fixed)lhs.m02 * (Fixed)rhs.m23 + (Fixed)lhs.m03 * (Fixed)rhs.m33);
			matrix4x4.m10 = (Fixed)((Fixed)lhs.m10 * (Fixed)rhs.m00 + (Fixed)lhs.m11 * (Fixed)rhs.m10 + (Fixed)lhs.m12 * (Fixed)rhs.m20 + (Fixed)lhs.m13 * (Fixed)rhs.m30);
			matrix4x4.m11 = (Fixed)((Fixed)lhs.m10 * (Fixed)rhs.m01 + (Fixed)lhs.m11 * (Fixed)rhs.m11 + (Fixed)lhs.m12 * (Fixed)rhs.m21 + (Fixed)lhs.m13 * (Fixed)rhs.m31);
			matrix4x4.m12 = (Fixed)((Fixed)lhs.m10 * (Fixed)rhs.m02 + (Fixed)lhs.m11 * (Fixed)rhs.m12 + (Fixed)lhs.m12 * (Fixed)rhs.m22 + (Fixed)lhs.m13 * (Fixed)rhs.m32);
			matrix4x4.m13 = (Fixed)((Fixed)lhs.m10 * (Fixed)rhs.m03 + (Fixed)lhs.m11 * (Fixed)rhs.m13 + (Fixed)lhs.m12 * (Fixed)rhs.m23 + (Fixed)lhs.m13 * (Fixed)rhs.m33);
			matrix4x4.m20 = (Fixed)((Fixed)lhs.m20 * (Fixed)rhs.m00 + (Fixed)lhs.m21 * (Fixed)rhs.m10 + (Fixed)lhs.m22 * (Fixed)rhs.m20 + (Fixed)lhs.m23 * (Fixed)rhs.m30);
			matrix4x4.m21 = (Fixed)((Fixed)lhs.m20 * (Fixed)rhs.m01 + (Fixed)lhs.m21 * (Fixed)rhs.m11 + (Fixed)lhs.m22 * (Fixed)rhs.m21 + (Fixed)lhs.m23 * (Fixed)rhs.m31);
			matrix4x4.m22 = (Fixed)((Fixed)lhs.m20 * (Fixed)rhs.m02 + (Fixed)lhs.m21 * (Fixed)rhs.m12 + (Fixed)lhs.m22 * (Fixed)rhs.m22 + (Fixed)lhs.m23 * (Fixed)rhs.m32);
			matrix4x4.m23 = (Fixed)((Fixed)lhs.m20 * (Fixed)rhs.m03 + (Fixed)lhs.m21 * (Fixed)rhs.m13 + (Fixed)lhs.m22 * (Fixed)rhs.m23 + (Fixed)lhs.m23 * (Fixed)rhs.m33);
			matrix4x4.m30 = (Fixed)((Fixed)lhs.m30 * (Fixed)rhs.m00 + (Fixed)lhs.m31 * (Fixed)rhs.m10 + (Fixed)lhs.m32 * (Fixed)rhs.m20 + (Fixed)lhs.m33 * (Fixed)rhs.m30);
			matrix4x4.m31 = (Fixed)((Fixed)lhs.m30 * (Fixed)rhs.m01 + (Fixed)lhs.m31 * (Fixed)rhs.m11 + (Fixed)lhs.m32 * (Fixed)rhs.m21 + (Fixed)lhs.m33 * (Fixed)rhs.m31);
			matrix4x4.m32 = (Fixed)((Fixed)lhs.m30 * (Fixed)rhs.m02 + (Fixed)lhs.m31 * (Fixed)rhs.m12 + (Fixed)lhs.m32 * (Fixed)rhs.m22 + (Fixed)lhs.m33 * (Fixed)rhs.m32);
			matrix4x4.m33 = (Fixed)((Fixed)lhs.m30 * (Fixed)rhs.m03 + (Fixed)lhs.m31 * (Fixed)rhs.m13 + (Fixed)lhs.m32 * (Fixed)rhs.m23 + (Fixed)lhs.m33 * (Fixed)rhs.m33);
			return matrix4x4;
		}

		public static FixedQuaternion operator *(FixedMatrix4x4 lhs, FixedQuaternion vector)
		{
			FixedQuaternion vector4;
			vector4.x = (Fixed)((Fixed)lhs.m00 * (Fixed)vector.x + (Fixed)lhs.m01 * (Fixed)vector.y + (Fixed)lhs.m02 * (Fixed)vector.z + (Fixed)lhs.m03 * (Fixed)vector.w);
			vector4.y = (Fixed)((Fixed)lhs.m10 * (Fixed)vector.x + (Fixed)lhs.m11 * (Fixed)vector.y + (Fixed)lhs.m12 * (Fixed)vector.z + (Fixed)lhs.m13 * (Fixed)vector.w);
			vector4.z = (Fixed)((Fixed)lhs.m20 * (Fixed)vector.x + (Fixed)lhs.m21 * (Fixed)vector.y + (Fixed)lhs.m22 * (Fixed)vector.z + (Fixed)lhs.m23 * (Fixed)vector.w);
			vector4.w = (Fixed)((Fixed)lhs.m30 * (Fixed)vector.x + (Fixed)lhs.m31 * (Fixed)vector.y + (Fixed)lhs.m32 * (Fixed)vector.z + (Fixed)lhs.m33 * (Fixed)vector.w);
			return vector4;
		}

		public static bool operator ==(FixedMatrix4x4 lhs, FixedMatrix4x4 rhs)
		{
			return lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2) && lhs.GetColumn(3) == rhs.GetColumn(3);
		}

		public static bool operator !=(FixedMatrix4x4 lhs, FixedMatrix4x4 rhs)
		{
			return !(lhs == rhs);
		}

        public override bool Equals(Object lhs)
        {
            return this.Equals(lhs);
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        /// <summary>
        ///   <para>Get a column of the matrix.</para>
        /// </summary>
        /// <param name="index"></param>
        public FixedQuaternion GetColumn(int index)
		{
			switch (index)
			{
				case 0:
					return new FixedQuaternion(this.m00, this.m10, this.m20, this.m30);
				case 1:
					return new FixedQuaternion(this.m01, this.m11, this.m21, this.m31);
				case 2:
					return new FixedQuaternion(this.m02, this.m12, this.m22, this.m32);
				case 3:
					return new FixedQuaternion(this.m03, this.m13, this.m23, this.m33);
				default:
					throw new IndexOutOfRangeException("Invalid column index!");
			}
		}

		/// <summary>
		///   <para>Transforms a position by this matrix (generic).</para>
		/// </summary>
		/// <param name="point"></param>
		public FixedVector3 MultiplyPoint(FixedVector3 point)
		{
			FixedVector3 vector3;
			vector3.x = (Fixed)((Fixed)this.m00 * (Fixed)point.x + (Fixed)this.m01 * (Fixed)point.y + (Fixed)this.m02 * (Fixed)point.z) + this.m03;
			vector3.y = (Fixed)((Fixed)this.m10 * (Fixed)point.x + (Fixed)this.m11 * (Fixed)point.y + (Fixed)this.m12 * (Fixed)point.z) + this.m13;
			vector3.z = (Fixed)((Fixed)this.m20 * (Fixed)point.x + (Fixed)this.m21 * (Fixed)point.y + (Fixed)this.m22 * (Fixed)point.z) + this.m23;
			Fixed num = 1f / ((Fixed)((Fixed)this.m30 * (Fixed)point.x + (Fixed)this.m31 * (Fixed)point.y + (Fixed)this.m32 * (Fixed)point.z) + this.m33);
			vector3.x *= num;
			vector3.y *= num;
			vector3.z *= num;
			return vector3;
		}

		/// <summary>
		///   <para>Transforms a position by this matrix (fast).</para>
		/// </summary>
		/// <param name="point"></param>
		public FixedVector3 MultiplyPoint3x4(FixedVector3 point)
		{
			FixedVector3 vector3;
			vector3.x = (Fixed)((Fixed)this.m00 * (Fixed)point.x + (Fixed)this.m01 * (Fixed)point.y + (Fixed)this.m02 * (Fixed)point.z) + this.m03;
			vector3.y = (Fixed)((Fixed)this.m10 * (Fixed)point.x + (Fixed)this.m11 * (Fixed)point.y + (Fixed)this.m12 * (Fixed)point.z) + this.m13;
			vector3.z = (Fixed)((Fixed)this.m20 * (Fixed)point.x + (Fixed)this.m21 * (Fixed)point.y + (Fixed)this.m22 * (Fixed)point.z) + this.m23;
			return vector3;
		}

		/// <summary>
		///   <para>Transforms a direction by this matrix.</para>
		/// </summary>
		/// <param name="vector"></param>
		public FixedVector3 MultiplyVector(FixedVector3 vector)
		{
			FixedVector3 vector3;
			vector3.x = (Fixed)((Fixed)this.m00 * (Fixed)vector.x + (Fixed)this.m01 * (Fixed)vector.y + (Fixed)this.m02 * (Fixed)vector.z);
			vector3.y = (Fixed)((Fixed)this.m10 * (Fixed)vector.x + (Fixed)this.m11 * (Fixed)vector.y + (Fixed)this.m12 * (Fixed)vector.z);
			vector3.z = (Fixed)((Fixed)this.m20 * (Fixed)vector.x + (Fixed)this.m21 * (Fixed)vector.y + (Fixed)this.m22 * (Fixed)vector.z);
			return vector3;
		}
	}
}
