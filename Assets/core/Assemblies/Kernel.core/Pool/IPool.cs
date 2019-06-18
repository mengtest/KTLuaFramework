namespace Kernel.core
{
	public interface IPool
	{
		void RecycleItem(IPoolItem item);
		IPoolItem SpawnItem();
	}
}