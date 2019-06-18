namespace Kernel.core
{
	public class Event
	{
		public readonly int Type;

		public Event(int type)
		{
			Type = type;
		}

		public virtual string GetName()
		{
			return string.Format("{0}", Type);
		}

		public override string ToString()
		{
			return string.Format("Type:{0}, eventType:{1}", GetType().Name, GetName());
		}
	}
}