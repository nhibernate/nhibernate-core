using NHibernate.Engine;

namespace NHibernate.Cache.Entry
{
	public interface ICacheEntryStructure
	{
		object Structure(object item);
		object Destructure(object map, ISessionFactoryImplementor factory);
	}
}