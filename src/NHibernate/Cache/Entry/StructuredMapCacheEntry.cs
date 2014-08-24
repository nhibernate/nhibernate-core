using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Cache.Entry
{
	public class StructuredMapCacheEntry : ICacheEntryStructure
	{
		public object Structure(object item)
		{
			CollectionCacheEntry entry = (CollectionCacheEntry)item;
			object[] state = entry.State;
			IDictionary map = new Hashtable(state.Length);
			for (int i = 0; i < state.Length; )
			{
				map[state[i++]] = state[i++];
			}
			return map;
		}

		public object Destructure(object item, ISessionFactoryImplementor factory)
		{
			IDictionary map = (IDictionary)item;
			object[] state = new object[map.Count * 2];
			int i = 0;
			foreach (DictionaryEntry me in map)
			{
				state[i++] = me.Key;
				state[i++] = me.Value;				
			}
			return new CollectionCacheEntry(state);
		}
	}
}
