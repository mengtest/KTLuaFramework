using System.Collections.Generic;
using Kernel.Engine;

namespace Kernel.core
{
	public class AttributeProvider
	{
		private readonly Dictionary<int, AttributeValue> attributes = new Dictionary<int, AttributeValue>();
		private Dictionary<int, AttributeValue> dirtyAttributes=new Dictionary<int, AttributeValue>();
		private bool dirty;

		public void Add(int key, AttributeValue value)
		{
			AttributeValue variable;
			if(GetAttributeVariable(key, out variable))
				variable.Add(value);
		}

		public void Add(int key, double value)
		{
			AttributeValue variable;
			if (GetAttributeVariable(key, out variable))
				variable.Add(new AttributeValue(value));
		}

		public void Sub(int key, AttributeValue value)
		{
			AttributeValue variable;
			if (GetAttributeVariable(key, out variable))
				variable.Sub(value);
		}

		public void Sub(int key, double value)
		{
			AttributeValue variable;
			if (GetAttributeVariable(key, out variable))
				variable.Sub(new AttributeValue(value));
		}

		public AttributeProvider SetVariableForce(int key, AttributeValue value)
		{
			if (!attributes.ContainsKey(key))
				attributes.Add(key,value);
			else
				attributes[key].Set(value);

			return this;
		}

		public AttributeProvider SetVariableForce(int key, double value)
		{
			if (!attributes.ContainsKey(key))
				attributes.Add(key, new AttributeValue(value));
			else
				attributes[key].Set(value);

			return this;
		}

		public AttributeProvider SetChangedVariableForce(int key, AttributeValue value)
		{
			if (!dirtyAttributes.ContainsKey(key))
				dirtyAttributes.Add(key, value);
			else
				dirtyAttributes[key].Set(value);

			dirty = true;
			return this;
		}

		public AttributeProvider SetChangedVariableForce(int key, double value)
		{
			if (!dirtyAttributes.ContainsKey(key))
				dirtyAttributes.Add(key, new AttributeValue(value));
			else
				dirtyAttributes[key].Set(value);

			dirty = true;
			return this;
		}

		public AttributeValue GetAttributeVariable(int key)
		{
			AttributeValue value;
			if (dirty && dirtyAttributes.TryGetValue(key, out value))
			{
				return value;
			}
			else if (dirtyAttributes.TryGetValue(key, out value))
			{
				return value;
			}

			return default(AttributeValue);
		}

		private bool GetAttributeVariable(int key,out AttributeValue value)
		{
			if (dirty && dirtyAttributes.TryGetValue(key,out value))
			{
				return true;
			}
			else if (dirtyAttributes.TryGetValue(key, out value))
			{
				return true;
			}

			return false;
		}

		public void ClearAttributeVariable()
		{
			attributes.Clear();
			dirtyAttributes?.Clear();
			dirty = false;
		}

		public Dictionary<int, double> ToAttributeDictionary(Dictionary<int, double> cache = null)
		{
			if (cache == null) cache = new Dictionary<int, double>();
			cache.Clear();

			foreach (var attributeVariable in attributes)
			{
				cache.Add(attributeVariable.Key,attributeVariable.Value.Value);
			}
			return cache;
		}

		public List<AttributeValue> GetAllAttributeVariables()
		{
			List<AttributeValue> list = new List<AttributeValue>();
			foreach (var a in attributes.Values)
			{
				list.Add(a);
			}
			return list;
		}

		/// <summary>
		/// Ìí¼Ó»ù´¡ÊôÐÔ
		/// </summary>
		/// <param name="values"></param>
		public void Reset(Dictionary<int, double> values)
		{
			ClearAttributeVariable();
			foreach (var v in values)
			{
				SetVariableForce(v.Key,v.Value);
			}
		}
	}
}