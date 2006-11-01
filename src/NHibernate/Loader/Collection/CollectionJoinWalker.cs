using System;
using System.Collections;

using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// Superclass of walkers for collection initializers
	/// <seealso cref="CollectionLoader" />
	/// <seealso cref="OneToManyJoinWalker" />
	/// <seealso cref="BasicCollectionJoinWalker" />
	/// </summary>
	public abstract class CollectionJoinWalker : JoinWalker
	{
		public CollectionJoinWalker( ISessionFactoryImplementor factory, IDictionary enabledFilters )
			: base( factory, enabledFilters )
		{
		}

		protected SqlStringBuilder WhereString( string alias, string[ ] columnNames, IType type, SqlString subselect, int batchSize )
		{
			if( subselect == null )
			{
				return base.WhereString( alias, columnNames, type, batchSize );
			}
			else
			{
				SqlStringBuilder buf = new SqlStringBuilder();
				if( columnNames.Length > 1 )
				{
					buf.Add( "(" );
				}
				buf.Add( StringHelper.Join( ", ", StringHelper.Qualify( alias, columnNames ) ) );
				if( columnNames.Length > 1 )
				{
					buf.Add( ")" );
				}
				buf.Add( " in " )
					.Add( "(" )
					.Add( subselect )
					.Add( ")" );
				return buf;
			}
		}
	}
}