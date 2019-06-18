using System;

namespace Kernel.core
{
	public interface IPoolItem : IDisposable
	{
		void OnPreRecycle();
	}
}