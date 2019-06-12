

namespace Kernel.core
{
	public interface IComponent
	{
	}

	public interface IComponentData<T> where T : struct
	{
		T Data { get; set; }
	}
}

