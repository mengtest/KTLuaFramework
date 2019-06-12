using System;
using System.Collections.Generic;

namespace Kernel.core
{
	public static class ExtendType
	{
		class TypeItem
		{
			public string name;
			public string assemblyQualifiedName;
		}

		public static bool IsStaticClassEx(this Type type)
		{
			return null != type
			       && type.GetConstructor(new Type[0]) == null
			       && type.IsAbstract
			       && type.IsSealed;
		}

		private static TypeItem _SetDefaultTypeItem(Type type)
		{
			TypeItem item;
			if(!_typeItems.TryGetValue(type, out item))
			{
				item = new TypeItem();
				_typeItems.Add(type, item);
			}

			return item;
		}

		public static string GetNameEx(this Type type)
		{
			if(null != type)
			{
				var item = _SetDefaultTypeItem(type);
				if(null == item.name)
				{
					item.name = type.Name;
				}

				return item.name;
			}

			return string.Empty;
		}

		public static string GetAssemblyQualifiedNameEx(this Type type)
		{
			if(null != type)
			{
				var item = _SetDefaultTypeItem(type);
				if(null == item.assemblyQualifiedName)
				{
					item.assemblyQualifiedName = type.AssemblyQualifiedName;
				}

				return item.assemblyQualifiedName;
			}

			return string.Empty;
		}

		private static readonly Dictionary<Type, TypeItem> _typeItems = new Dictionary<Type, TypeItem>();
	}
}