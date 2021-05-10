using NHibernate.Mapping;

namespace NHibernate.Persister.Collection
{
	//TODO 6.0 move into ICollectionPersister
	public interface ICacheableCollectionPersister : ICollectionPersister
	{
		/// <summary>
		/// When <see cref="PersistentClass.CacheConcurrencyStrategy"/> is "never" then UpdateTimestampsCache is not tracked
		/// So QueryCache is not used for this strategy
		/// </summary>
		bool SupportsQueryCache { get; }
	}
}
