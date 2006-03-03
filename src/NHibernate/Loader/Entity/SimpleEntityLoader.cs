using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.Loader.Entity;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// Loads entity instances one instance per select (ie without outerjoin fetching)
	/// </summary>
	public class SimpleEntityLoader : Loader, IUniqueEntityLoader
	{
		private readonly ILoadable[ ] persister;
		private readonly IType idType;
		private SqlString sql;
		private readonly LockMode[ ] lockMode;

		public SimpleEntityLoader( ILoadable persister, SqlString sql, LockMode lockMode )
		{
			this.persister = new ILoadable[ ] {persister};
			this.idType = persister.IdentifierType;
			this.sql = sql;
			this.lockMode = new LockMode[ ] {lockMode};
			PostInstantiate();
		}

		protected internal override SqlString SqlString
		{
			get { return sql; }
			set { sql = value; }
		}

		/// <summary></summary>
		protected override ILoadable[ ] Persisters
		{
			get { return persister; }
			set { throw new NotSupportedException( "cannot assign to the Persisters property" ); }
		}

		/// <summary></summary>
		protected override ICollectionPersister CollectionPersister
		{
			get { return null; }
		}

		/// <summary></summary>
		protected override string[ ] Suffixes
		{
			get { return NoSuffix; }
			set { throw new NotSupportedException( "A SimpleEntityLoader has no Suffixes" ); }
		}

		public object Load( ISessionImplementor session, object id, object obj )
		{
			IList list = LoadEntity( session, id, idType, obj, id );
			if( list.Count == 1 )
			{
				return list[ 0 ];
			}
			else if( list.Count == 0 )
			{
				return null;
			}
			else
			{
				throw new HibernateException( "More than one row with the given identifier was found: " + id + ", for class: " + persister[ 0 ].ClassName );
			}
		}

		protected override LockMode[ ] GetLockModes( IDictionary lockModes )
		{
			return lockMode;
		}

		protected override Object GetResultColumnOrRow(
			Object[ ] row,
			IDataReader rs,
			ISessionImplementor session )
		{
			return row[ 0 ];
		}

		protected override bool IsSingleRowLoader
		{
			get { return true; }
		}

		protected override int[] Owners
		{
			get { return null; }
			set { throw new NotSupportedException("SimpleEntityLoader.set_Owners"); }
		}
	}
}