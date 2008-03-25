using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Cache.Entry
{
	public class StructuredCollectionCacheEntry : ICacheEntryStructure
	{
		public virtual object Structure(object item)
		{
			CollectionCacheEntry entry = (CollectionCacheEntry)item;
			return new ArrayList(entry.State);
		}

		public virtual object Destructure(object item, ISessionFactoryImplementor factory)
		{
			ArrayList list = new ArrayList((IList)item);
			return new CollectionCacheEntry(list.ToArray());
		}
	}
}
