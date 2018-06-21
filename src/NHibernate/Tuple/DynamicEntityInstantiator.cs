using System;
using System.Collections;
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
			if (_entityName != null)
			{
				map[Key] = _entityName;
			}

			return map;
		}

		public bool IsInstance(object obj)
		{
			if (!(obj is IDictionary<string, object> that))
				return false;
			if (_entityName == null)
				return true;

			var type = (string) that[Key];
			return type == null || _isInstanceEntityNames.Contains(type);

		}

		#endregion
	}
}
