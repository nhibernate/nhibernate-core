using NHibernate.Mapping;

namespace NHibernate.Persister.Entity
{
	//TODO 6.0 move into IEntityPersister
	public interface ICacheableEntityPersister : IEntityPersister
	{
		/// <summary>
		/// When <see cref="PersistentClass.CacheConcurrencyStrategy"/> is "never" then UpdateTimestampsCache is not tracked
		/// So QueryCache is not used for this strategy
		/// </summary>
		bool SupportsQueryCache { get; }
	}
}
