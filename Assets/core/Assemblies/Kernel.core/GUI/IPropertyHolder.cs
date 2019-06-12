namespace Kernel.core
{
	/// <summary>
	/// 这个接口是给PropertyEditor使用的，可以修改和显示任意实现了此接口的类
	/// 子类只能有方法（函数），不能有公有的属性或者变量，可以有私有的。
	/// 一般不要使用这个类，请使用PropertyHolder
	/// </summary>
	public interface IPropertyHolder
	{
#if UNITY_EDITOR
		void OnGUI(ref bool changed);
#endif
	}
}