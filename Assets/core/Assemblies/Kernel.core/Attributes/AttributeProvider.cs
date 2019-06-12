using System.Collections.Generic;
using Kernel.Engine;

namespace Kernel.core
{
	public class AttributeProvider : IAttributeProvider
	{
		private readonly Dictionary<int, AttributeVariable> attributes = new Dictionary<int, AttributeVariable>(88);
		private Dictionary<int, AttributeVariable> dirtyAttributes;
		private bool dirty;

		public void Add(int key, AttributeValue value)
		{
			Dictionary<int, AttributeVariable> dictionary;
			var variable = GetAttributeVariable(key, value, out dictionary);
			if(dictionary == null)
			{
				attributes[key] = variable;
			}
			else
			{
				variable.value.Add(value);
                dictionary[key] = variable;
			}
		}

		public void Add(AttributeVariable v)
		{
			Dictionary<int, AttributeVariable> dictionary;
			var variable = GetAttributeVariable(v.Key, v.Value, out dictionary);
			if (dictionary == null)
			{
				attributes[v.Key] = v;
			}
			else
			{
				variable.Add(v);
				dictionary[v.Key] = variable;
			}
		}

		public void Add(IList<AttributeVariable> attributeVariables)
		{
			if (attributeVariables != null && attributeVariables.Count > 0)
			{
				for (var i = 0; i < attributeVariables.Count; i++)
				{
					Add(attributeVariables[i]);
				}
			}
		}

		public void Sub(AttributeVariable v)
		{
			Dictionary<int, AttributeVariable> dictionary;
			var variable = GetAttributeVariable(v.Key, v.Value, out dictionary);
			if (dictionary != null)
			{
				variable.Sub(v);
				dictionary[v.Key] = variable;
			}
		}

		public void Sub(IList<AttributeVariable> attributeVariables)
		{
			if (attributeVariables != null && attributeVariables.Count > 0)
			{
				for (var i = 0; i < attributeVariables.Count; i++)
				{
					Sub(attributeVariables[i]);
				}
			}
		}

		public AttributeVariable GetAttributeVariable(int key)
		{
			AttributeVariable res;
			if (dirty && dirtyAttributes != null)
			{
				if (dirtyAttributes.TryGetValue(key, out res))
				{
					return res;
				}
			}

			if (attributes.TryGetValue(key, out res))
			{
				return res;
			}
			return new AttributeVariable(key, new AttributeValue(AttributeModifyType.BASIC, 0));
		}

		public AttributeVariable GetAttributeVariable(int key, AttributeValue value, out Dictionary<int, AttributeVariable> dictionary)
		{
			AttributeVariable res;
			dictionary = null;
			if (dirty && dirtyAttributes != null)
			{
				if (dirtyAttributes.TryGetValue(key, out res))
				{
					dictionary = dirtyAttributes;
					return res;
				}
			}

			if (attributes.TryGetValue(key, out res))
			{
				dictionary = attributes;
				return res;
			}
			return new AttributeVariable(key, value);
		}

		public void SetAttributeVariable(AttributeVariable v)
		{
			AttributeVariable variable;
			if (attributes.TryGetValue(v.Key, out variable))
			{
				variable.Set(v);
				attributes[v.Key] = variable;
			}
			else
			{
				variable = new AttributeVariable(v.Key, new AttributeValue(0));
				variable.Set(v);
				attributes.Add(v.Key, v);
			}
		}

		public void SetAttributeVariableForce(int key, double value)
		{
			var newVariable = new AttributeVariable(key, new AttributeValue(value));
			AttributeVariable variable;
			if (dirtyAttributes == null) dirtyAttributes = new Dictionary<int, AttributeVariable>(88);
			if (dirtyAttributes.TryGetValue(key, out variable))
			{
				variable.Set(newVariable);
				dirtyAttributes[key] = variable;
			}
			else
			{
				dirtyAttributes.Add(key, newVariable);
			}
			dirty = true;
		}

		public void ClearAttributeVariable()
		{
			attributes.Clear();
			dirtyAttributes?.Clear();
			dirty = false;
		}

		public Dictionary<int, Fixed> ToAttributeDictionary(Dictionary<int, Fixed> cache = null)
		{
			if (cache == null) cache = new Dictionary<int, Fixed>();
			cache.Clear();

			foreach (var attributeVariable in attributes)
			{
				cache[attributeVariable.Key] = attributeVariable.Value.Value.Value;
			}
			return cache;
		}

		public List<AttributeVariable> GetAllAttributeVariables()
		{
			List<AttributeVariable> list = new List<AttributeVariable>();
			foreach (var a in attributes.Values)
			{
				list.Add(a);
			}
			return list;
		}

		public void Reset(Dictionary<int, Fixed> values)
		{
			ClearAttributeVariable();
			foreach (var v in values)
			{
				Add(new AttributeVariable(v.Key, new AttributeValue(v.Value.AsDouble())));
			}
		}
		public void Debug()
		{
			foreach (var kv in attributes)
			{
				if (kv.Value.value.Value > 0)
				{
					UnityEngine.Debug.LogError("key: " + ((AttributeType)kv.Key).ToString() + "  " + kv.Value.value.Value);
				}
				
			}
		}
	}
}