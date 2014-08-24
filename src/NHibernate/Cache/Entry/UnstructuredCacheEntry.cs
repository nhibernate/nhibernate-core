using NHibernate.Engine;

namespace NHibernate.Cache.Entry
{
	public class UnstructuredCacheEntry : ICacheEntryStructure
	{
		public object Structure(object item)
		{
			return item;
		}

		public object Destructure(object map, ISessionFactoryImplementor factory)
		{
			return map;
		}
	}
}
