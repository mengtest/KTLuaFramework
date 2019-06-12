namespace Kernel.core
{
	public interface IDataEditor
	{
		void SetData(object data);
		void OnGUI(ref bool changed);
		void OnDrawGizmos();
	}

	///直接从这个类继承就行，data就自带了，不用每次强制转换
	public class DataEditorBase<T> : IDataEditor where T : class
	{
		protected T Data
		{
			get;
			set;
		}

		public virtual void SetData(object data)
		{
			Data = data as T;
		}
		
		///重载这个方法
		public virtual void OnGUI(ref bool changed)
		{
		}

		///重载这个方法
		public virtual void OnDrawGizmos()
		{
		}
	}
}