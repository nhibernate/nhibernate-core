using System;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

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

		public CollectionLoader( IQueryableCollection persister, ISessionFactoryImplementor factory, IDictionary enabledFilters )
			: base( factory, enabledFilters )
		{
			this.collectionPersister = persister;
		}

		protected override bool IsSubselectLoadingEnabled
		{
			get { return HasSubselectLoadableCollections; }
		}

		public void Initialize( object id, ISessionImplementor session )
		{
			LoadCollection( session, id, KeyType );
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