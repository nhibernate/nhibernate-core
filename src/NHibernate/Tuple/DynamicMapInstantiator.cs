using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Mapping;

namespace NHibernate.Tuple
{
	[Serializable]
	public class DynamicMapInstantiator : IInstantiator
	{
		public const string KEY = "$type$";

		private readonly string entityName;
		private readonly HashSet<string> isInstanceEntityNames = new HashSet<string>();
		private readonly Mapping.Component component;

		public DynamicMapInstantiator(Mapping.Component component)
		{
			entityName = null;
			this.component = component;
		}

		public DynamicMapInstantiator(PersistentClass mappingInfo)
		{
			entityName = mappingInfo.EntityName;
			isInstanceEntityNames.Add(entityName);
			if (mappingInfo.HasSubclasses)
			{
				foreach (PersistentClass subclassInfo in mappingInfo.SubclassClosureIterator)
					isInstanceEntityNames.Add(subclassInfo.EntityName);
			}
		}

		#region IInstantiator Members

		public object Instantiate(object id)
		{
			return Instantiate();
		}

		public object Instantiate()
		{
			IDictionary map = GenerateMap();
			if (entityName != null)
			{
				map[KEY] = entityName;
			}
			return map;
		}

		protected virtual IDictionary GenerateMap()
		{
			if ((this.component != null) && (typeof(IDictionary<string, object>).IsAssignableFrom(this.component.Type.ReturnedClass) == true))
			{
				return new Dictionary<string, object>();
			}
			else
			{
				return new Hashtable();
			}
		}

		public bool IsInstance(object obj)
		{
			IDictionary that = obj as IDictionary;
			if (that != null)
			{
				if (entityName == null)
				{
					return true;
				}
				string type = (string)that[KEY];
				return type == null || isInstanceEntityNames.Contains(type);
			}
			else
			{
				return obj is IDictionary<string, object>;
			}
		}

		#endregion
	}
}
