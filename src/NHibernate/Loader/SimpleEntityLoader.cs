using System;
using System.Collections;
using System.Data;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader
{
	/// <summary>
	/// Loads entity instances one instance per select (ie without outerjoin fetching)
	/// </summary>
	public class SimpleEntityLoader : Loader, IUniqueEntityLoader
	{
		private readonly ILoadable[ ] persister;
		private readonly IType idType;
		private SqlString sqlString;
		private readonly LockMode[ ] lockMode;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="sqlString"></param>
		/// <param name="lockMode"></param>
		/// <param name="d"></param>
		public SimpleEntityLoader( ILoadable persister, SqlString sqlString, LockMode lockMode, Dialect.Dialect d ) : base( d )
		{
			this.persister = new ILoadable[ ] {persister};
			this.idType = persister.IdentifierType;
			this.sqlString = sqlString;
			this.lockMode = new LockMode[ ] {lockMode};
			PostInstantiate();
		}

		/// <summary></summary>
		protected internal override SqlString SqlString
		{
			get { return sqlString; }
			set { sqlString = value; }
		}

		/// <summary></summary>
		protected override int[] Owners
		{
			get { return null; }
		}

		/// <summary></summary>
		protected override bool IsSingleRowLoader
		{
			get { return true; }
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="id"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lockModes"></param>
		/// <returns></returns>
		protected override LockMode[ ] GetLockModes( IDictionary lockModes )
		{
			return lockMode;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="row"></param>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected override Object GetResultColumnOrRow(
			Object[ ] row,
			IDataReader rs,
			ISessionImplementor session )
		{
			return row[ 0 ];
		}
	}
}