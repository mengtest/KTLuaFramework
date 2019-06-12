using Kernel.Engine;
namespace Kernel.core
{
    public interface IManager
    {
        void Reset();
        void Init();
        void OnDrawGizmos();
    }
    
    public abstract class Manager<T> : Singleton<T>, IManager,ITick,IFixedTick, ILateTick where T : Manager<T>, new()
    {
        public void Reset()
        {
	        OnReset();
        }

        public void Init()
        {
            OnInit();
        }
       
        public void Tick(float deltaTime)
        {
            OnTick(deltaTime);
        }

        public void FixedTick(float fixedDeltaTime)
        {
	        OnFixedTick(fixedDeltaTime);
		}

        public void LateTick(float deltaTime)
        {
            OnLateTick();
        }

		public virtual void OnDrawGizmos()
		{
		}

	    protected virtual void OnReset()
	    {
	    }

		protected virtual void OnInit()
        {
        }
        protected virtual void OnTick(float deltaTime)
        {
        }
        protected virtual void OnFixedTick(float deltaTime)
        {
        }
        protected virtual void OnLateTick()
        {
        }
    }
}
