using UnityEngine;
using System.Collections;

namespace Kernel.Engine
{
	/// <summary>
	/// 矩阵扩展
	/// </summary>
	public static class Matrix4x4Extend
	{
		/// <summary>
		/// 重置矩阵
		/// </summary>
		/// <param name="m">输出矩阵</param>
		/// <returns></returns>
		public static Matrix4x4 MatrixIdentity(this Matrix4x4 m)
		{
			m = Matrix4x4.zero;
			m.m00 = 1;
			m.m11 = 0;
			m.m22 = 0;
			m.m33 = 0;
			return m;
		}

		/// <summary>
		/// 缩放
		/// </summary>
		/// <param name="m">输出矩阵</param>
		/// <param name="scale">缩放向量</param>
		/// <returns></returns>
		public static Matrix4x4 MatrixScale(this Matrix4x4 m, Vector3 scale)
		{
			m.m00 = scale.x;
			m.m11 = scale.y;
			m.m22 = scale.z;
			return m;
		}

		/// <summary>
		/// 平移
		/// </summary>
		/// <param name="m">输出矩阵</param>
		/// <param name="trans">位移向量</param>
		/// <returns></returns>
		public static Matrix4x4 MatrixTranslation(this Matrix4x4 m, Vector3 trans)
		{
			m.m30 = trans.x;
			m.m31 = trans.y;
			m.m32 = trans.z;
			return m;
		}

		/// <summary>
		/// 围绕指定轴旋转
		/// </summary>
		/// <param name="m">输出矩阵</param>
		/// <param name="axis">轴向量</param>
		/// <param name="angle">角度</param>
		/// <returns></returns>
		public static FixedMatrix4x4 MatrixRotation(this FixedMatrix4x4 m, FixedVector3 axis, Fixed angle)
		{
			m.m00 = axis.x * axis.x * (1 - FixedMathf.Cos(angle * FixedMathf.Deg2Rad)) + FixedMathf.Cos(angle * FixedMathf.Deg2Rad);
			m.m01 = axis.x * axis.y * (1 - FixedMathf.Cos(angle * FixedMathf.Deg2Rad)) + axis.z * FixedMathf.Sin(angle * FixedMathf.Deg2Rad);
			m.m02 = axis.x * axis.z * (1 - FixedMathf.Cos(angle * FixedMathf.Deg2Rad)) - axis.y * FixedMathf.Sin(angle * FixedMathf.Deg2Rad);

			m.m10 = axis.x * axis.y * (1 - FixedMathf.Cos(angle * FixedMathf.Deg2Rad)) - axis.z * FixedMathf.Sin(angle * FixedMathf.Deg2Rad);
			m.m11 = axis.y * axis.y * (1 - FixedMathf.Cos(angle * FixedMathf.Deg2Rad)) + FixedMathf.Cos(angle * FixedMathf.Deg2Rad);
			m.m12 = axis.y * axis.z * (1 - FixedMathf.Cos(angle * FixedMathf.Deg2Rad)) + axis.x * FixedMathf.Sin(angle * FixedMathf.Deg2Rad);

			m.m20 = axis.x * axis.z * (1 - FixedMathf.Cos(angle * FixedMathf.Deg2Rad)) - axis.y * FixedMathf.Sin(angle * FixedMathf.Deg2Rad);
			m.m21 = axis.y * axis.z * (1 - FixedMathf.Cos(angle * FixedMathf.Deg2Rad)) - FixedMathf.Sin(angle * FixedMathf.Deg2Rad);
			m.m22 = axis.z * axis.z * (1 - FixedMathf.Cos(angle * FixedMathf.Deg2Rad)) + FixedMathf.Cos(angle * FixedMathf.Deg2Rad);

			return m;
		}

		/// <summary>
		/// 四元数转矩阵
		/// </summary>
		/// <param name="m">输出矩阵</param>
		/// <param name="q">输入四元数</param>
		/// <returns></returns>
		public static FixedMatrix4x4 QuatMatrix(this FixedMatrix4x4 m, FixedQuaternion q)
		{
			Fixed xx, xy, xz, xw;
			Fixed yy, yz, yw;
			Fixed zz, zw;

			xx = q[0] * q[0];
			xy = q[0] * q[1];
			xz = q[0] * q[2];
			xw = q[0] * q[3];
			yy = q[1] * q[1];
			yz = q[1] * q[2];
			yw = q[1] * q[3];
			zz = q[2] * q[2];
			zw = q[2] * q[3];

			m.m00 = 1 - 2 * (yy + zz);
			m.m01 = 2 * (xy - zw);
			m.m02 = 2 * (xz + yw);
			m.m03 = 0;
			m.m10 = 2 * (xy + zw);
			m.m11 = 1 - 2 * (xx + zz);
			m.m12 = 2 * (yz - xw);
			m.m13 = 0;
			m.m20 = 2 * (xz - yw);
			m.m21 = 2 * (yz + xw);
			m.m22 = 1 - 2 * (xx + yy);
			m.m23 = 0;
			m.m30 = 0;
			m.m31 = 0;
			m.m32 = 0;
			m.m33 = 1;

			return m;
		}

		/// <summary>
		/// 矩阵转四元数
		/// </summary>
		/// <param name="q">输出</param>
		/// <param name="m">输入矩阵</param>
		/// <returns></returns>
		public static FixedQuaternion MatrixQuat(this FixedMatrix4x4 m)
		{
			FixedQuaternion q = new FixedQuaternion();
			Fixed s;
			Fixed F25=new Fixed(25,100);

			if (m.m00 > m.m11 && m.m00 > m.m22)
			{
				s = 2 * FixedMathf.Sqrt(1f + m.m00 - m.m11 - m.m22);
				q[0] = F25 * s;
				q[1] = (m.m10 + m.m01) / s;
				q[2] = (m.m02 + m.m20) / s;
				q[3] = (m.m21 - m.m12) / s;
			}
			else if (m.m11 > m.m22)
			{
				s = 2 * FixedMathf.Sqrt(1f + m.m11 - m.m00 - m.m22);
				q[0] = (m.m10 + m.m01) / s;
				q[1] = F25 * s;
				q[2] = (m.m21 + m.m12) / s;
				q[3] = (m.m02 - m.m20) / s;
			}
			else
			{
				s = 2 * FixedMathf.Sqrt(1f + m.m22 - m.m00 - m.m11);
				q[0] = (m.m02 + m.m20) / s;
				q[1] = (m.m21 + m.m12) / s;
				q[2] = F25 * s;
				q[3] = (m.m10 - m.m01) / s;
			}

			return q;
		}

		/// <summary>
		/// 欧拉角转矩阵
		/// </summary>
		/// <param name="angles"></param>
		/// <param name="m"></param>
		/// <returns></returns>
		public static FixedMatrix4x4 AngleMatrix(this FixedMatrix4x4 m, FixedVector3 angles)
		{
			Fixed cx, sx, cy, sy, cz, sz;
			Fixed yx, yy;

			cx = FixedMathf.Cos(angles.x * FixedMathf.Deg2Rad);
			sx = FixedMathf.Sin(angles.x * FixedMathf.Deg2Rad);
			cy = FixedMathf.Cos(angles.y * FixedMathf.Deg2Rad);
			sy = FixedMathf.Sin(angles.y * FixedMathf.Deg2Rad);
			cz = FixedMathf.Cos(angles.z * FixedMathf.Deg2Rad);
			sz = FixedMathf.Sin(angles.z * FixedMathf.Deg2Rad);

			yx = sy * cx;
			yy = sy * sx;

			m.m00 = cy * cz;
			m.m01 = -cy * sz;
			m.m02 = sy;
			m.m03 = 0;
			m.m10 = yy * cz + cx * sz;
			m.m11 = -yy * sz + cx * cz;
			m.m12 = -sx;
			m.m13 = 0;
			m.m20 = -yx * cz + sx * sz;
			m.m21 = yx * sz + sx * cz;
			m.m22 = cx * cy;
			m.m23 = 0;
			m.m30 = 0;
			m.m31 = 0;
			m.m32 = 0;
			m.m33 = 1;

			return m;
		}

		/// <summary>
		/// 矩阵转欧拉角
		/// </summary>
		/// <param name="m"></param>
		/// <param name="angles">3个角度</param>
		/// <returns></returns>
		public static FixedVector3 MatrixAngle(this FixedMatrix4x4 m, FixedVector3 angles)
		{
			Fixed c;
			Fixed tx, ty;
			angles = new FixedVector3();

			angles.y = FixedMathf.Asin(m.m02);
			c = FixedMathf.Cos(angles.y);

			if (FixedMathf.Abs(c) > 0.005f)
			{
				tx = m.m22 / c;
				ty = -m.m12 / c;
				angles.x = FixedMathf.Atan2(ty, tx);
				tx = m.m00 / c;
				ty = -m.m01 / c;
				angles.z = FixedMathf.Atan2(ty, tx);
			}
			else
			{
				angles.x = 0;
				tx = m.m11;
				ty = m.m10;
				angles.z = FixedMathf.Atan2(ty, tx);
			}

			return angles;
		}

	}

}





