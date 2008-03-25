using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// Superclass for loaders that initialize collections
	/// <seealso cref="OneToManyLoader" />
	/// <seealso cref="BasicCollectionLoader" />
	/// </summary>
	public class CollectionLoader : OuterJoinLoader, ICollectionInitializer
	{
		private readonly IQueryableCollection collectionPersister;

		public CollectionLoader(IQueryableCollection persister, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: base(factory, enabledFilters)
		{
			collectionPersister = persister;
		}

		protected internal override bool IsSubselectLoadingEnabled
		{
			get { return HasSubselectLoadableCollections(); }
		}

		public virtual void Initialize(object id, ISessionImplementor session)
		{
			LoadCollection(session, id, KeyType);
		}

		protected IType KeyType
		{
			get { return collectionPersister.KeyType; }
		}

		public override string ToString()
		{
			return GetType().FullName + '(' + collectionPersister.Role + ')';
		}
	}
}