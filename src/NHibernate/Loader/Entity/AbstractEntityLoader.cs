using System;
using System.Collections;

using log4net;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// Abstract superclass for entity loaders that use outer joins
	/// </summary>
	public abstract class AbstractEntityLoader : OuterJoinLoader, IUniqueEntityLoader
	{
		protected static readonly ILog log = LogManager.GetLogger( typeof( AbstractEntityLoader ) );
		protected readonly IOuterJoinLoadable persister;
		protected readonly IType uniqueKeyType;
		protected readonly System.Type entityName;

		public AbstractEntityLoader(
			IOuterJoinLoadable persister,
			IType uniqueKeyType,
			ISessionFactoryImplementor factory,
			IDictionary enabledFilters )
			: base( factory, enabledFilters )
		{
			this.uniqueKeyType = uniqueKeyType;
			this.entityName = persister.MappedClass;
			this.persister = persister;
		}

		public object Load( object id, object optionalObject, ISessionImplementor session )
		{
			return Load( session, id, optionalObject, id );
		}

		protected virtual object Load( ISessionImplementor session, object id, object optionalObject, object optionalId )
		{
			IList list = LoadEntity(
				session,
				id,
				uniqueKeyType,
				optionalObject,
				entityName,
				optionalId,
				persister );

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
				if( CollectionOwners != null )
				{
					return list[ 0 ];
				}
				else
				{
					throw new HibernateException(
						"More than one row with the given identifier was found: " +
						id +
						", for class: " +
						persister.MappedClass
						);
				}
			}
		}

		protected override object GetResultColumnOrRow(object[] row, System.Data.IDataReader rs, ISessionImplementor session)
		{
			return row[ row.Length - 1 ];
		}

		protected override bool IsSingleRowLoader
		{
			get { return true; }
		}
	}
}