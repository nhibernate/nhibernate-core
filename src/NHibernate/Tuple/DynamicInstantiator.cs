using NHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace NHibernate.Tuple
{
    [Serializable()]
    public class DynamicInstantiator : IInstantiator
    {
     	public const string KEY = "$type$";

		private readonly string entityName;
		private readonly HashSet<string> isInstanceEntityNames = new HashSet<string>();

		public DynamicInstantiator()
		{
			entityName = null;
		}

        public DynamicInstantiator(PersistentClass mappingInfo)
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
            dynamic dyn = GenerateDynamic();
			if (entityName != null)
			{
				dyn.EntityName = entityName;
			}
			return dyn;
		}

		protected virtual IDynamicMetaObjectProvider GenerateDynamic()
		{
			return new ExpandoObject();
		}

		public bool IsInstance(object obj)
		{
            dynamic that = obj as IDynamicMetaObjectProvider;
			if (that != null)
			{
				if (entityName == null)
				{
					return true;
				}
				string type = (string)that.EntityName;
				return type == null || isInstanceEntityNames.Contains(type);
			}
			else
			{
				return false;
			}
		}

		#endregion
    }
}
