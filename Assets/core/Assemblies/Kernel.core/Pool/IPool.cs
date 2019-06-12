namespace Kernel.core.Pool
{
	public interface IPool
	{
		void RecycleItem(IPoolItem item);
		IPoolItem SpawnItem();
	}
}