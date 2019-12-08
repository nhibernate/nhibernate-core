using NHibernate.Mapping;
using NHibernate.Persister.Collection;

namespace NHibernate.Engine
{
	public interface IMetadata : IMapping
	{
		PersistentClass GetPersistentClass(string entityName);
		NHibernate.Mapping.Collection GetCollection(string role);
	}
}
