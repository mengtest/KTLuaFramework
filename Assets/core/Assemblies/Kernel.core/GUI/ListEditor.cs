using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Kernel.core
{
	public class StructListEditor<T> : IDisposable where T : struct
	{
		private readonly List<T> list;

#if UNITY_EDITOR
		private readonly string name;
		private readonly Func<T, T> onShow;
		private readonly bool firstRemovable;
		private readonly bool addable;
		private readonly bool showIndex;
#endif
		private readonly List<int> removeIndexList = new List<int>();
		private T[] array;

		public StructListEditor(ref List<T> list, Func<T, T> onShow, string name = null, bool firstRemovable = true,
			bool addable = true, bool showIndex = true)
		{
			this.list = list;
#if UNITY_EDITOR
			this.name = name ?? typeof(T).Name;
			this.onShow = onShow;
			this.firstRemovable = firstRemovable;
			this.addable = addable;
			this.showIndex = showIndex;
#endif
		}

		public StructListEditor(ref T[] array, Func<T, T> onShow, string name = null, bool firstRemovable = true,
			bool addable = true, bool showIndex = true)
		{
			this.array = array;
			list = array.ToList();
#if UNITY_EDITOR
			this.name = name ?? typeof(T).Name;
			this.onShow = onShow;
			this.firstRemovable = firstRemovable;
			this.addable = addable;
			this.showIndex = showIndex;
#endif
		}

		public void Dispose()
		{
			foreach(var i in removeIndexList)
			{
				list.RemoveAt(i);
			}
			removeIndexList.Clear();
			if(array != null)
			{
				array = list.ToArray();
			}
		}

		public void OnGUI(ref bool changed)
		{
#if UNITY_EDITOR
			using(GUIUtil.LayoutVertical("Box", GUILayout.ExpandWidth(true)))
			{
				using(GUIUtil.LayoutHorizontal())
				{
					GUILayout.Label(name, GUILayout.ExpandWidth(true));
					if(addable && GUILayout.Button("Add", GUILayout.Width(80)))
					{
						list.Add(new T());
						changed = true;
					}
				}

				for(var i = 0; i < list.Count; ++i)
				{
					using(GUIUtil.LayoutHorizontal("Box"))
					{
						//编号
						using(GUIUtil.LayoutVertical(GUILayout.Width(30)))
						{
							if(showIndex)
							{
								GUILayout.Label(i.ToString(), GUILayout.ExpandWidth(false));
							}
						}

						//内容
						using(GUIUtil.LayoutVertical())
						{
							if(onShow != null)
							{
								var tt = list[i];
								list[i] = onShow(list[i]);
								changed = !Equals(tt, list[i]) || changed;
							}
						}

						//删除
						using(GUIUtil.LayoutVertical(GUILayout.Width(56)))
						{
							if(i != 0 || firstRemovable)
							{
								if(GUILayout.Button("Delete", GUILayout.Width(56)))
								{
                                    removeIndexList.Add(i);
									changed = true;
								}
							}
						}
					}
				}
			}
#endif
		}
	}

	public class ListEditor<T> : IDisposable where T : class, new()
	{
		protected readonly Func<T> createFunc;
		protected readonly bool firstRemovable;
		protected readonly List<T> list;
		protected readonly string name;
		protected readonly Func<T, bool> onShow;
		protected readonly List<int> removeIndexList = new List<int>();
#if UNITY_EDITOR
		private readonly bool showIndex;
#endif

		public ListEditor(ref List<T> list, Func<T, bool> onShow, string name = null, bool firstRemovable = true,
			Func<T> createFunc = null, bool showIndex = true)
		{
			this.list = list;
			this.name = name ?? typeof(T).Name;
			this.onShow = onShow;
			this.firstRemovable = firstRemovable;
			this.createFunc = createFunc;
#if UNITY_EDITOR
			this.showIndex = showIndex;
#endif
		}

		public ListEditor(ref List<T> list, string name = null, bool firstRemovable = true,
			Func<T> createFunc = null, bool showIndex = true)
		{
			this.list = list;
			this.name = name ?? typeof(T).Name;
			onShow = OnShowImpl;
			this.firstRemovable = firstRemovable;
			this.createFunc = createFunc;
#if UNITY_EDITOR
			this.showIndex = showIndex;
#endif
		}

		public void Dispose()
		{
			foreach(var i in removeIndexList)
			{
				list.RemoveAt(i);
			}
			removeIndexList.Clear();
		}

		public virtual void OnGUI(ref bool changed)
		{
#if UNITY_EDITOR
			using(GUIUtil.LayoutVertical("Box", GUILayout.ExpandWidth(true)))
			{
				using(GUIUtil.LayoutHorizontal())
				{
					GUILayout.Label(name, GUILayout.ExpandWidth(true));
					if(GUILayout.Button("Add", GUILayout.Width(80)))
					{
                        list.Add(createFunc == null ? new T() : createFunc.Invoke());
						changed = true;
					}
				}

				for(var i = 0; i < list.Count; ++i)
				{
					var t = list[i];

					using(GUIUtil.LayoutHorizontal("Box"))
					{
						//编号
						using(GUIUtil.LayoutVertical(GUILayout.Width(30)))
						{
							if(showIndex)
							{
								GUILayout.Label(i.ToString(), GUILayout.ExpandWidth(false));
							}
						}

						//内容
						using(GUIUtil.LayoutVertical())
						{
							if(onShow != null)
							{
								changed = onShow(t) || changed;
							}
						}

						//删除
						using(GUIUtil.LayoutVertical(GUILayout.Width(56)))
						{
							if(i != 0 || firstRemovable)
							{
								if(GUILayout.Button("Delete", GUILayout.Width(56)))
								{
                                    removeIndexList.Add(i);
									changed = true;
								}
							}
						}
					}
				}
			}
#endif
		}

		private static bool OnShowImpl(T holder)
		{
#if UNITY_EDITOR
			//新版数据editor代码放在数据外
			var changed = false;
			if(DataEditorManager.Instance.IsEditorExisted(holder))
			{
				DataEditorManager.Instance.ShowDataEditor(holder, ref changed);
				return changed;
			}

			//早期数据editor代码放在数据内
			var propertyHolder = holder as IPropertyHolder;
			if(propertyHolder != null)
			{
				var h = propertyHolder;
				h.OnGUI(ref changed);
				return changed;
			}
#endif
			throw new NotImplementedException("Must be IPropertyHolder or implement Func<T, bool>.");
		}
	}

	public class ListEditorUtil
	{
		/// <summary>
		/// 一个列表里的项移上移下
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="list">列表</param>
		/// <param name="item">项</param>
		/// <param name="isMoveUp">true = 上移，false = 下移</param>
		public static void MoveUpDown<T>(List<T> list, T item, bool isMoveUp)
		{
			if(list.Count <= 1)
			{
				return;
			}

			int index = list.IndexOf(item);

			if(index < 0 || index >= list.Count)
			{
				return;
			}

			T t = list[index];
			int targetIndex = 0;
			if(index == 0)
			{
				if(isMoveUp)
				{
					//不能进行交换，直接插入到最后
					list.RemoveAt(index);
					list.Add(t);
					return;
				}
				targetIndex = 1;
			}
			else if(index == list.Count - 1)
			{

				if(!isMoveUp)
				{
					//不能进行交换，直接插入到最前
					list.RemoveAt(index);
					list.Insert(0, t);
					return;
				}
				targetIndex = list.Count - 2;
			}
			else
			{
				targetIndex = index + (isMoveUp ? -1 : 1);
			}

			list[index] = list[targetIndex];
			list[targetIndex] = t;
		}
	}
}