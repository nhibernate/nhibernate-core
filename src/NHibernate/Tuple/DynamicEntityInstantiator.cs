using System;
using System.Collections.Generic;
using NHibernate.Mapping;

namespace NHibernate.Tuple
{
	[Serializable]
	public class DynamicEntityInstantiator : IInstantiator
	{
		public const string Key = "$type$";

		private readonly string _entityName;
		private readonly HashSet<string> _isInstanceEntityNames = new HashSet<string>();

		public DynamicEntityInstantiator(PersistentClass mappingInfo)
		{
			_entityName = mappingInfo.EntityName;
			_isInstanceEntityNames.Add(_entityName);
			if (mappingInfo.HasSubclasses)
			{
				foreach (var subclassInfo in mappingInfo.SubclassClosureIterator)
					_isInstanceEntityNames.Add(subclassInfo.EntityName);
			}
		}

		protected virtual IDictionary<string, object> GenerateMap()
		{
			return new Dictionary<string, object>();
		}

		#region IInstantiator Members

		public object Instantiate(object id)
		{
			return Instantiate();
		}

		public object Instantiate()
		{
			var map = GenerateMap();
			map[Key] = _entityName;

			return map;
		}

		public bool IsInstance(object obj)
		{
			if (!(obj is IDictionary<string, object> that))
				return false;

			return that.TryGetValue(Key, out var type) && _isInstanceEntityNames.Contains(type as string);
		}

		#endregion
	}
}
