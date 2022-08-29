using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Loader.Collection
{
	public abstract partial class AbstractBatchingCollectionInitializer : ICollectionInitializer
	{
		protected IQueryableCollection CollectionPersister { get; }

		protected AbstractBatchingCollectionInitializer(IQueryableCollection collectionPersister)
		{
			CollectionPersister = collectionPersister;
		}

		public abstract void Initialize(object id, ISessionImplementor session);
	}
}
