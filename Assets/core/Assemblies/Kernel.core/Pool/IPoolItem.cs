using System;

namespace Kernel.core.Pool
{
	public interface IPoolItem : IDisposable
	{
		void OnPreRecycle();
	}
}