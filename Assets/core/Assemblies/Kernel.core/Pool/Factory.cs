using System;
using System.Collections.Generic;

namespace Kernel.core
{
	public class Factory<TP, TD> where TP : class
	{
		private readonly Dictionary<Type, Type> descProduct = new Dictionary<Type, Type>();

		public delegate TP CreateMethod();

		public delegate void InitializeMethod(TP product, TD desc);

		protected readonly Dictionary<Type, CreateMethod> factories = new Dictionary<Type, CreateMethod>();
		protected readonly Dictionary<Type, InitializeMethod> methods = new Dictionary<Type, InitializeMethod>();

		public void Add<TPs, TDs>(InitializeMethod method = null) where TPs : TP, new() where TDs : TD
		{
			Add<TPs, TDs>(() => new TPs(), method);
		}

		public void Add<TPs, TDs>(CreateMethod create, InitializeMethod method = null) where TPs : TP where TDs : TD
		{
			var p = typeof(TPs);
			var d = typeof(TDs);

			descProduct.Add(d, p);

			factories.Add(p, create);
			methods.Add(p, method);
		}

		public TP Create(TD desc)
		{
			var d = desc.GetType();
			var p = descProduct[d];
			var ret = factories[p]();
			var method = methods[p];
			if(method != null)
			{
				method(ret, desc);
			}
			return ret;
		}
	}
}