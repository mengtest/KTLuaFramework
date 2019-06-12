using System;

#if !DISABLE_UNITY
using UnityEngine;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kernel.core
{
	public partial class GUIUtil
	{
		public static void CopyToSystemClipboard(string content)
		{
#if !DISABLE_UNITY
			GUIUtility.systemCopyBuffer = content;
#endif
		}

#if !DISABLE_UNITY
		private class GUIEnabled : IDisposable
		{
			private readonly bool old;

			public GUIEnabled(bool value)
			{
				old = GUI.enabled;
				GUI.enabled = value;
			}

			public void Dispose()
			{
				GUI.enabled = old;
			}
		}

		private class GUIContentColor : IDisposable
		{
			private readonly Color old;

			public GUIContentColor(Color value)
			{
				old = GUI.contentColor;
				GUI.contentColor = value;
			}

			public void Dispose()
			{
				GUI.contentColor = old;
			}
		}

		private class GUIColor : IDisposable
		{
			private readonly Color old;

			public GUIColor(Color value)
			{
				old = GUI.color;
				GUI.color = value;
			}

			public void Dispose()
			{
				GUI.color = old;
			}
		}

		private class GUIBackgroundColor : IDisposable
		{
			private readonly Color old;

			public GUIBackgroundColor(Color value)
			{
				old = GUI.backgroundColor;
				GUI.backgroundColor = value;
			}

			public void Dispose()
			{
				GUI.backgroundColor = old;
			}
		}

		private class GUISettings : IDisposable
		{
			private readonly IDisposable[] disposables;

			public GUISettings(params IDisposable[] args)
			{
				disposables = args;
			}

			public void Dispose()
			{
				for(int i = 0; i < disposables.Length; i++)
				{
					disposables[i].Dispose();
				}
			}
		}
#endif

#if UNITY_EDITOR
		private class GUILayoutVertical : IDisposable
		{
			public GUILayoutVertical(GUILayoutOption[] options)
			{
				EditorGUILayout.BeginVertical(options);
			}

			public GUILayoutVertical(GUIStyle style, params GUILayoutOption[] options)
			{
				EditorGUILayout.BeginVertical(style, options);
			}

			public GUILayoutVertical(GUIStyle style)
			{
				EditorGUILayout.BeginVertical(style);
			}

			public void Dispose()
			{
				EditorGUILayout.EndVertical();
			}
		}

		private class GUILayoutHorizontal : IDisposable
		{
			public GUILayoutHorizontal()
			{
				EditorGUILayout.BeginHorizontal();
			}

			public GUILayoutHorizontal(GUIStyle style)
			{
				EditorGUILayout.BeginHorizontal(style);
			}

			public GUILayoutHorizontal(params GUILayoutOption[] options)
			{
				EditorGUILayout.BeginHorizontal(options);
			}

			public GUILayoutHorizontal(GUIStyle style, params GUILayoutOption[] options)
			{
				EditorGUILayout.BeginHorizontal(style, options);
			}

			public void Dispose()
			{
				EditorGUILayout.EndHorizontal();
			}
		}

		private class GUIEditorIndent : IDisposable
		{
			public GUIEditorIndent()
			{
				++EditorGUI.indentLevel;
			}

			public void Dispose()
			{
				--EditorGUI.indentLevel;
			}
		}

		private class GUIScroll : IDisposable
		{
			public GUIScroll(ref Vector2 scroll)
			{
				scroll = EditorGUILayout.BeginScrollView(scroll);
			}

			public GUIScroll(ref Vector2 scroll, params GUILayoutOption[] options)
			{
				scroll = GUILayout.BeginScrollView(scroll, options);
			}

			public GUIScroll(ref Vector2 scroll, GUIStyle style)
			{
				scroll = GUILayout.BeginScrollView(scroll, style);
			}


			public void Dispose()
			{
				EditorGUILayout.EndScrollView();
			}
		}


		private class GUIHandleColor : IDisposable
		{
			private readonly Color old;

			public GUIHandleColor(Color color)
			{
				old = Handles.color;
				Handles.color = color;
			}

			public void Dispose()
			{
				Handles.color = old;
			}
		}

		private class GUILabelWidth : IDisposable
		{
			private readonly float old;

			public GUILabelWidth(float width)
			{
				old = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = width;
			}

			public void Dispose()
			{
				EditorGUIUtility.labelWidth = old;
			}
		}

		private class GUITextAlign : IDisposable
		{
			private readonly TextAnchor old;
			private readonly GUIStyle guiStyle;

			public GUITextAlign(GUIStyle style, TextAnchor anchor)
			{
				guiStyle = style;
				old = style.alignment;
				style.alignment = anchor;
			}

			public void Dispose()
			{
				guiStyle.alignment = old;
			}
		}
#endif
	}
}