using System;

namespace Kernel.core
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DataEditorAttribute : System.Attribute
	{
		public Type DataType;

		public DataEditorAttribute(Type t)
		{
			DataType = t;
		}
	}
}