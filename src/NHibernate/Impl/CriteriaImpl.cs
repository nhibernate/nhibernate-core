using System;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Expression;
using NHibernate.Persister;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;

using NExpression = NHibernate.Expression;

namespace NHibernate.Impl
{
	/// <summary>
	/// Implementation of the <see cref="ICriteria"/> interface
	/// </summary>
	internal class CriteriaImpl : ICriteria
	{
		private IList criteria = new ArrayList();
		private IList orderings = new ArrayList();
		private IDictionary fetchModes = new Hashtable();
		private IDictionary associationPathByAlias = new Hashtable();
		private IDictionary aliasByAssociationPath = new Hashtable();
		private IDictionary classByAlias = new Hashtable();
		private IDictionary lockModes = new Hashtable();
		private int maxResults = RowSelection.NoValue;
		private int firstResult;
		private int timeout = RowSelection.NoValue;
		private int fetchSize = RowSelection.NoValue;
		private System.Type persistentClass;
		private SessionImpl session;
		private IResultTransformer resultTransformer = new RootEntityResultTransformer();
		private bool cacheable;
		private string cacheRegion;

		private int counter = 0;

		private string GenerateAlias()
		{
			return "x" + counter++ + StringHelper.Underscore;
		}

		public sealed class Subcriteria : ICriteria
		{
			// Added to simulate Java-style inner class
			private CriteriaImpl parent;
			private string rootAlias;
			private string rootPath;

			internal Subcriteria( CriteriaImpl parent, string rootAlias, string rootPath )
			{
				this.parent = parent;
				this.rootAlias = rootAlias;
				this.rootPath = rootPath;
			}

			public ICriteria Add( ICriterion expression )
			{
				parent.Add( rootAlias, expression );
				return this;
			}

			public ICriteria CreateAlias( string associationPath, string alias )

			{
				parent.CreateAlias( rootAlias, associationPath, alias );
				return this;
			}

			public ICriteria AddOrder( Order order )
			{
				throw new NotSupportedException( "subcriteria cannot be ordered" );
			}

			public ICriteria CreateCriteria( string associationPath )

			{
				return parent.CreateCriteriaAt( rootAlias, associationPath );
			}

			public IList List()
			{
				return parent.List();
			}

			public object UniqueResult()
			{
				return parent.UniqueResult();
			}

			public ICriteria SetFetchMode( string associationPath, FetchMode mode )

			{
				parent.SetFetchMode( StringHelper.Qualify( rootPath, associationPath ), mode );
				return this;
			}

			public ICriteria SetFirstResult( int firstResult )
			{
				parent.SetFirstResult( firstResult );
				return this;
			}

			public ICriteria SetMaxResults( int maxResults )
			{
				parent.SetMaxResults( maxResults );
				return this;
			}

			public ICriteria SetTimeout( int timeout )
			{
				parent.SetTimeout( timeout );
				return this;
			}

			public ICriteria SetFetchSize( int fetchSize )
			{
				parent.SetFetchSize( fetchSize );
				return this;
			}

			public System.Type CriteriaClass
			{
				get { return parent.GetCriteriaClass( rootAlias ); }
			}

			public System.Type GetCriteriaClass( string alias )
			{
				return parent.GetCriteriaClass( alias );
			}

			public ICriteria CreateCriteria( string associationPath, string alias )

			{
				return parent.CreateCriteriaAt( rootAlias, associationPath, alias );
			}

			// Deprecated methods not ported: ReturnMaps, ReturnRootEntities

			public ICriteria SetLockMode( LockMode lockMode )
			{
				parent.SetLockMode( rootAlias, lockMode );
				return this;
			}

			public ICriteria SetLockMode( string alias, LockMode lockMode )
			{
				parent.SetLockMode( alias, lockMode );
				return this;
			}

			public ICriteria SetResultTransformer( IResultTransformer resultProcessor )
			{
				parent.SetResultTransformer( resultProcessor );
				return this;
			}

			public ICriteria SetCacheable( bool cacheable )
			{
				parent.SetCacheable( cacheable );
				return this;
			}

			public ICriteria SetCacheRegion( string cacheRegion )
			{
				parent.SetCacheRegion( cacheRegion );
				return this;
			}
		}

		public ICriteria SetMaxResults( int maxResults )
		{
			this.maxResults = maxResults;
			return this;
		}

		public ICriteria SetFirstResult( int firstResult )
		{
			this.firstResult = firstResult;
			return this;
		}

		public ICriteria SetTimeout( int timeout )
		{
			this.timeout = timeout;
			return this;
		}

		public ICriteria SetFetchSize( int fetchSize )
		{
			this.fetchSize = fetchSize;
			return this;
		}

		public ICriteria Add( ICriterion expression )
		{
			Add( CriteriaUtil.RootAlias, expression );
			return this;
		}

		public int MaxResults
		{
			get { return maxResults; }
		}

		public int FirstResult
		{
			get { return firstResult; }
		}

		public int Timeout
		{
			get { return timeout; }
		}

		public int FetchSize
		{
			get { return fetchSize; }
		}

		public CriteriaImpl( System.Type persistentClass, SessionImpl session )
		{
			this.persistentClass = persistentClass;
			this.session = session;
			this.classByAlias[ CriteriaUtil.RootAlias ] = persistentClass;
			this.cacheable = false;
		}

		/// <summary>
		/// Copy all the internal attributes of the given CriteriaImpl
		/// except alter the root persistent class type to be the given one.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="original"></param>
		public CriteriaImpl( System.Type persistentClass, CriteriaImpl original )
		{
			this.persistentClass = persistentClass;

			this.classByAlias = original.classByAlias;
			this.classByAlias[ CriteriaUtil.RootAlias ] = persistentClass;

			this.criteria = original.criteria;
			this.orderings = original.orderings;
			this.fetchModes = original.fetchModes;
			this.associationPathByAlias = original.associationPathByAlias;
			this.aliasByAssociationPath = original.aliasByAssociationPath;
			this.lockModes = original.lockModes;
			this.maxResults = original.maxResults;
			this.firstResult = original.firstResult;
			this.timeout = original.timeout;
			this.fetchSize = original.fetchSize;
			this.session = original.session;
			this.resultTransformer = original.resultTransformer;
			this.counter = original.counter;
			this.cacheable = original.cacheable;
			this.cacheRegion = original.cacheRegion;
		}

		public IList List()
		{
			return session.Find( this );
		}

		public IEnumerable IterateExpressionEntries()
		{
			return criteria;
		}

		public IEnumerable IterateOrderings()
		{
			return orderings;
		}

		public System.Type GetPersistentClass( string alias )
		{
			return ( System.Type ) classByAlias[ alias ];
		}

		public IDictionary AliasClasses
		{
			get { return classByAlias; }
		}

		public override string ToString()
		{
			return criteria.ToString();
		}

		public ICriteria AddOrder( Order ordering )
		{
			orderings.Add( ordering );
			return this;
		}

		public FetchMode GetFetchMode( string path )
		{
			object result = fetchModes[ path ];
			return result == null ? FetchMode.Default : ( FetchMode ) result;
		}

		public ICriteria SetFetchMode( string associationPath, FetchMode mode )
		{
			fetchModes[ associationPath ] = mode;
			return this;
		}

		public ICriteria CreateAlias( string associationPath, string alias )
		{
			CreateAlias( CriteriaUtil.RootAlias, associationPath, alias );
			return this;
		}

		private void CreateAlias( string rootAlias, string associationPath, string alias )
		{
			string testAlias = StringHelper.Root( associationPath );
			if( classByAlias.Contains( testAlias ) )
			{
				rootAlias = testAlias;
				associationPath = associationPath.Substring( rootAlias.Length + 1 );
			}

			string rootPath = ( string ) associationPathByAlias[ rootAlias ];
			string wholeAssociationPath;
			if( rootPath == null )
			{
				if( !CriteriaUtil.RootAlias.Equals( rootAlias ) )
				{
					throw new HibernateException( "unknown alias: " + rootAlias );
				}
				wholeAssociationPath = associationPath;
			}
			else
			{
				wholeAssociationPath = StringHelper.Qualify( rootPath, associationPath );
			}

			object oldPath = associationPathByAlias[ alias ];
			associationPathByAlias[ alias ] = wholeAssociationPath;
			if( oldPath != null )
			{
				throw new HibernateException( "alias already defined: " + alias );
			}
			
			object oldAlias = aliasByAssociationPath[ wholeAssociationPath ];
			aliasByAssociationPath[ wholeAssociationPath ] = alias;
			if( oldAlias != null )
			{
				throw new HibernateException( "association already joined: " + wholeAssociationPath );
			}
			classByAlias[ alias ] = GetClassForPath( rootAlias, associationPath );
		}

		public bool IsJoin( string path )
		{
			return aliasByAssociationPath.Contains( path );
		}

		public string GetAlias( string associationPath )
		{
			return ( string ) aliasByAssociationPath[ associationPath ];
		}

		public ICriteria Add( string alias, ICriterion expression )
		{
			criteria.Add( new CriterionEntry( expression, alias ) );
			return this;
		}

		public System.Type GetClassForPath( string rootAlias, string associationPath )
		{
			ISessionFactoryImplementor factory = session.Factory;
			System.Type clazz = ( System.Type ) classByAlias[ rootAlias ];
			IType type = ( ( IPropertyMapping ) factory.GetPersister( clazz ) ).ToType( associationPath );
			if( !type.IsAssociationType )
			{
				throw new QueryException( string.Format( "not an association path: {0}", associationPath ) );
			}

			return ( ( IAssociationType ) type ).GetAssociatedClass( factory );
		}

		public sealed class CriterionEntry
		{
			private readonly ICriterion criterion;
			private readonly string alias;

			internal CriterionEntry( ICriterion criterion, string alias )
			{
				this.alias = alias;
				this.criterion = criterion;
			}

			public ICriterion Criterion
			{
				get { return criterion; }
			}

			public string Alias
			{
				get { return alias; }
			}
		}

		public ICriteria CreateCriteria( string associationPath )
		{
			return CreateCriteriaAt( CriteriaUtil.RootAlias, associationPath );
		}

		private ICriteria CreateCriteriaAt( String rootAlias, String associationPath )
		{
			return CreateCriteriaAt( rootAlias, associationPath, GenerateAlias() );
		}

		private ICriteria CreateCriteriaAt( String rootAlias, String associationPath, String alias )
		{
			String testAlias = StringHelper.Root( associationPath );
			if( classByAlias.Contains( testAlias ) )
			{
				rootAlias = testAlias;
				associationPath = associationPath.Substring( rootAlias.Length + 1 );
			}

			CreateAlias( rootAlias, associationPath, alias );
			return new Subcriteria( this, alias, associationPath );
		}

		public object UniqueResult()
		{
			return AbstractQueryImpl.UniqueElement( List() );
		}

		public System.Type CriteriaClass
		{
			get { return persistentClass; }
		}

		public System.Type GetCriteriaClass( string alias )
		{
			return ( System.Type ) classByAlias[ alias ];
		}

		public ICriteria CreateCriteria( string associationPath, string alias )
		{
			return CreateCriteriaAt( CriteriaUtil.RootAlias, associationPath, alias );
		}

		// Deprecated methods not ported: ReturnMaps, ReturnRootEntities

		public ICriteria SetLockMode( LockMode lockMode )
		{
			return SetLockMode( CriteriaUtil.RootAlias, lockMode );
		}

		public ICriteria SetLockMode( string alias, LockMode lockMode )
		{
			lockModes[ alias ] = lockMode;
			return this;
		}

		public IDictionary LockModes
		{
			get { return lockModes; }
		}

		public IResultTransformer ResultTransformer
		{
			get { return resultTransformer; }
		}

		public ICriteria SetResultTransformer( IResultTransformer tupleMapper )
		{
			this.resultTransformer = tupleMapper;
			return this;
		}

		public bool Cacheable
		{
			get { return cacheable; }
		}

		public string CacheRegion
		{
			get { return cacheRegion; }
		}

		public ICriteria SetCacheable( bool cacheable )
		{
			this.cacheable = cacheable;
			return this;
		}

		public ICriteria SetCacheRegion( string cacheRegion )
		{
			this.cacheRegion = cacheRegion.Trim();
			return this;
		}
	}
}