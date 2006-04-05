using System;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// A walker for loaders that fetch entities
	/// </summary>
	public class EntityJoinWalker : AbstractEntityJoinWalker
	{
		private readonly LockMode lockMode;

		public EntityJoinWalker(
			IOuterJoinLoadable persister,
			string[ ] uniqueKey,
			IType uniqueKeyType,
			int batchSize,
			LockMode lockMode,
			ISessionFactoryImplementor factory,
			IDictionary enabledFilters )
			: base( persister, factory, enabledFilters )
		{
			this.lockMode = lockMode;

			SqlStringBuilder whereCondition = WhereString( Alias, uniqueKey, uniqueKeyType, batchSize )
				//include the discriminator and class-level where, but not filters
				.Add( persister.FilterFragment( Alias, CollectionHelper.EmptyMap ) );

			InitAll( whereCondition.ToSqlString(), "", lockMode );
		}

		/// <summary>
		/// Disable outer join fetching if this loader obtains an
		/// upgrade lock mode
		/// </summary>
		protected override bool IsJoinedFetchEnabled( IAssociationType type, FetchMode config, Cascades.CascadeStyle cascadeStyle )
		{
			return lockMode.GreaterThan( LockMode.Read ) ?
			       false :
			       base.IsJoinedFetchEnabled( type, config, cascadeStyle );
		}

		public override string Comment
		{
			get { return "load " + Persister.ClassName; }
		}
	}
}