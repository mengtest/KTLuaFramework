namespace Kernel.core
{
	public interface ITick
	{
		void Tick(float deltaTime);
	}

	public interface IFixedTick
	{
		void FixedTick(float fixedDeltaTime);
	}

	public interface ILateTick
	{
		void LateTick(float deltaTime);
	}
}
