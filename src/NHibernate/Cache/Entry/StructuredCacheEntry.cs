using System;
using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Cache.Entry
{
	public class StructuredCacheEntry : ICacheEntryStructure
	{
		private readonly IEntityPersister persister;

		public StructuredCacheEntry(IEntityPersister persister)
		{
			this.persister = persister;
		}

		public object Destructure(object item, ISessionFactoryImplementor factory)
		{
			IDictionary map = (IDictionary) item;
			string subclass = (string) map["_subclass"];
			object version = map["_version"];
			IEntityPersister subclassPersister = factory.GetEntityPersister(subclass);
			string[] names = subclassPersister.PropertyNames;
			object[] state = new object[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				state[i] = map[names[i]];
			}
			return new CacheEntry
			{
				Subclass = subclass,
				DisassembledState = state,
				Version = version
			};
		}

		public object Structure(object item)
		{
			CacheEntry entry = (CacheEntry) item;
			string[] names = persister.PropertyNames;
			IDictionary map = new Hashtable(names.Length + 2);
			map["_subclass"] = entry.Subclass;
			map["_version"] = entry.Version;
			for (int i = 0; i < names.Length; i++)
			{
				map[names[i]] = entry.DisassembledState[i];
			}
			return map;
		}
	}
}
