namespace Kernel.core
{
	public class YieldWork
#if !DISABLE_UNITY
		: UnityEngine.CustomYieldInstruction
#endif
	{
		public interface IYieldable
		{
			bool IsFinished
			{
				get;
			}
		}

#if !DISABLE_UNITY
		public override bool keepWaiting
		{
			get
			{
				return yieldable != null && !yieldable.IsFinished;
			}
		}
#endif

		private readonly IYieldable yieldable;

		public YieldWork(IYieldable yieldable)
		{
			this.yieldable = yieldable;
		}
	}
}
