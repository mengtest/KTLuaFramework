using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace Kernel.core
{
	public static class GizmosUtil
	{
		public static void DrawPlane(Vector3 position, Quaternion rotation, Vector2 size, Color color, Color colorInner)
		{
#if UNITY_EDITOR
			using(GizmosMatrix())
			{
				Gizmos.matrix = Matrix4x4.TRS(position, rotation, UnityEngine.Vector3.one);
				using(GizmosColor())
				{
					// 画四条线
					{
						Gizmos.color = color;
						Gizmos.DrawLine(new UnityEngine.Vector3(size.x / 2, 0, size.y / 2), new UnityEngine.Vector3(size.x / 2, 0, -size.y / 2));
						Gizmos.DrawLine(new UnityEngine.Vector3(size.x / 2, 0, -size.y / 2), new UnityEngine.Vector3(-size.x / 2, 0, -size.y / 2));
						Gizmos.DrawLine(new UnityEngine.Vector3(-size.x / 2, 0, -size.y / 2), new UnityEngine.Vector3(-size.x / 2, 0, size.y / 2));
						Gizmos.DrawLine(new UnityEngine.Vector3(-size.x / 2, 0, size.y / 2), new UnityEngine.Vector3(size.x / 2, 0, size.y / 2));
					}
					// 画里面的
					{
						Gizmos.color = colorInner;
						Gizmos.DrawCube(UnityEngine.Vector3.zero, new UnityEngine.Vector3(size.x, 0, size.y));
					}
				}
			}
#endif
		}

		public static void DrawCircle(Vector3 position, Quaternion rotation, float radius, Color color, Color colorInner)
		{
#if UNITY_EDITOR
			using(GizmosColor())
			{
				Handles.color = color;
				Handles.DrawWireArc(position, rotation * Vector3.up, rotation * Vector3.right, 360, radius);
				Handles.color = colorInner;
				Handles.DrawSolidArc(position, rotation * Vector3.up, rotation * Vector3.right, 360, radius);
			}
#endif
		}

		public static void DrawCapsule(Vector3 position, Quaternion rotation, float height, float radius, Color color, Color colorInner)
		{
#if UNITY_EDITOR
			using(GizmosMatrix())
			{
				height = Mathf.Max(0, height - radius * 2);
				Gizmos.matrix = Matrix4x4.TRS(position, UnityEngine.Quaternion.identity, UnityEngine.Vector3.one);
				using(GizmosColor())
				{
					Gizmos.color = color;
					var r = radius * Mathf.Cos(45);// Mathf.Cos45;
					// 画四条竖线
					{
						Gizmos.DrawLine(new UnityEngine.Vector3(radius, height / 2, 0), new UnityEngine.Vector3(radius, -height / 2, 0));
						Gizmos.DrawLine(new UnityEngine.Vector3(-radius, height / 2, 0), new UnityEngine.Vector3(-radius, -height / 2, 0));
						Gizmos.DrawLine(new UnityEngine.Vector3(0, height / 2, radius), new UnityEngine.Vector3(0, -height / 2, radius));
						Gizmos.DrawLine(new UnityEngine.Vector3(0, height / 2, -radius), new UnityEngine.Vector3(0, -height / 2, -radius));
					}
					// 再画四条竖线
					{
						Gizmos.DrawLine(new UnityEngine.Vector3(r, height / 2, r), new UnityEngine.Vector3(r, -height / 2, r));
						Gizmos.DrawLine(new UnityEngine.Vector3(-r, height / 2, -r), new UnityEngine.Vector3(-r, -height / 2, -r));
						Gizmos.DrawLine(new UnityEngine.Vector3(r, height / 2, -r), new UnityEngine.Vector3(r, -height / 2, -r));
						Gizmos.DrawLine(new UnityEngine.Vector3(-r, height / 2, r), new UnityEngine.Vector3(-r, -height / 2, r));
					}
					// 上下各画一个圆两个半圆
					{
						UnityEngine.Vector3 h = new UnityEngine.Vector3(0, height / 2, 0);
						Handles.color = color;
						Handles.matrix = Gizmos.matrix;
						Handles.DrawWireArc(h, UnityEngine.Vector3.up, UnityEngine.Vector3.right, 360, radius);
						Handles.DrawWireArc(-h, UnityEngine.Vector3.up, UnityEngine.Vector3.right, 360, radius);
						Handles.DrawWireArc(h, UnityEngine.Vector3.left, UnityEngine.Vector3.forward, 180, radius);
						Handles.DrawWireArc(h, UnityEngine.Vector3.forward, UnityEngine.Vector3.right, 180, radius);
						Handles.DrawWireArc(-h, UnityEngine.Vector3.left, UnityEngine.Vector3.back, 180, radius);
						Handles.DrawWireArc(-h, UnityEngine.Vector3.back, UnityEngine.Vector3.right, 180, radius);
					}
				}
			}
#endif
		}

		public static void DrawCylinder(Vector3 position, Quaternion rotation, float height, float radius, Color color, Color colorInner)
		{
#if UNITY_EDITOR
			using(GizmosMatrix())
			{
				Gizmos.matrix = Matrix4x4.TRS(position, UnityEngine.Quaternion.identity, UnityEngine.Vector3.one);
				using(GizmosColor())
				{
					// 以下是画圆柱体的代码
					Gizmos.color = color;
					var r = radius * Mathf.Cos(45);
					// 画四条竖线
					{
						Gizmos.DrawLine(new UnityEngine.Vector3(radius, height / 2, 0), new UnityEngine.Vector3(radius, -height / 2, 0));
						Gizmos.DrawLine(new UnityEngine.Vector3(-radius, height / 2, 0), new UnityEngine.Vector3(-radius, -height / 2, 0));
						Gizmos.DrawLine(new UnityEngine.Vector3(0, height / 2, radius), new UnityEngine.Vector3(0, -height / 2, radius));
						Gizmos.DrawLine(new UnityEngine.Vector3(0, height / 2, -radius), new UnityEngine.Vector3(0, -height / 2, -radius));
					}
					// 再画四条竖线
					{
						Gizmos.DrawLine(new UnityEngine.Vector3(r, height / 2, r), new UnityEngine.Vector3(r, -height / 2, r));
						Gizmos.DrawLine(new UnityEngine.Vector3(-r, height / 2, -r), new UnityEngine.Vector3(-r, -height / 2, -r));
						Gizmos.DrawLine(new UnityEngine.Vector3(r, height / 2, -r), new UnityEngine.Vector3(r, -height / 2, -r));
						Gizmos.DrawLine(new UnityEngine.Vector3(-r, height / 2, r), new UnityEngine.Vector3(-r, -height / 2, r));
					}
					// 画上下两个圆
					{
						UnityEngine.Vector3 h = new UnityEngine.Vector3(0, height / 2, 0);
						Handles.color = color;
						Handles.matrix = Gizmos.matrix;
						Handles.DrawWireArc(h, UnityEngine.Vector3.up, UnityEngine.Vector3.right, 360, radius);
						Handles.DrawWireArc(-h, UnityEngine.Vector3.up, UnityEngine.Vector3.right, 360, radius);
					}
				}
			}
#endif
		}

		public static void DrawCube(Vector3 position, Quaternion rotation, Vector3 size, Color color, Color colorInner)
		{
#if UNITY_EDITOR
			using(GizmosMatrix())
			{
				Gizmos.matrix = Matrix4x4.TRS(position, rotation, UnityEngine.Vector3.one);
				using(GizmosColor())
				{
					Gizmos.color = color;
					Gizmos.DrawWireCube(UnityEngine.Vector3.zero, size);
					Gizmos.color = colorInner;
					Gizmos.DrawCube(UnityEngine.Vector3.zero, size);
				}
			}
#endif
		}

		public static void DrawBall(Vector3 position, Quaternion rotation, float radius, Color color, Color colorInner)
		{
#if UNITY_EDITOR
			using(GizmosMatrix())
			{
				Gizmos.matrix = Matrix4x4.TRS(position, rotation, UnityEngine.Vector3.one);
				using(GizmosColor())
				{
					Gizmos.color = color;
					Gizmos.DrawWireSphere(UnityEngine.Vector3.zero, radius);
					Gizmos.color = colorInner;
					Gizmos.DrawSphere(UnityEngine.Vector3.zero, radius);
				}
			}
#endif
		}

		public static IDisposable GizmosMatrix()
		{
			return new GizmosDrawMatrix();
		}

		public static IDisposable GizmosColor()
		{
			return new GizmosDrawColor();
		}

		private class GizmosDrawMatrix : IDisposable
		{
#if UNITY_EDITOR
			private readonly Matrix4x4 matrixGizmos;
			private readonly Matrix4x4 matrixHandles;
#endif

			public GizmosDrawMatrix()
			{
#if UNITY_EDITOR
				matrixGizmos = Gizmos.matrix;
				matrixHandles = Handles.matrix;
#endif
			}

			public void Dispose()
			{
#if UNITY_EDITOR
				Gizmos.matrix = matrixGizmos;
				Handles.matrix = matrixHandles;
#endif
			}
		}

		private class GizmosDrawColor : IDisposable
		{
			private readonly Color colorGizmos;
			private readonly Color colorHandles;

			public GizmosDrawColor()
			{
#if UNITY_EDITOR
				colorGizmos = Gizmos.color;
				colorHandles = Handles.color;
#endif
			}

			public void Dispose()
			{
#if UNITY_EDITOR
				Gizmos.color = colorGizmos;
				Handles.color = colorHandles;
#endif
			}
		}
	}
}