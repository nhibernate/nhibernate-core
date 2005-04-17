using System.Collections;
using NHibernate.Engine;
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
		private int maxResults;
		private int firstResult;
		private int timeout;
		private int fetchSize;
		private System.Type persistentClass;
		private SessionImpl session;
		private IResultTransformer resultTransformer; //== new RootEntityResultTransformer();
		private bool cacheable;
		private string cacheRegion;
		private RowSelection selection = new RowSelection();

		private int counter = 0;

		private NExpression.Junction conjunction = NExpression.Expression.Conjunction();

		/// <summary>
		/// 
		/// </summary>
		public const string RootAlias = "this";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="session"></param>
		public CriteriaImpl( System.Type persistentClass, SessionImpl session )
		{
			this.persistentClass = persistentClass;
			this.session = session;
			this.classByAlias[ CriteriaImpl.RootAlias ] = persistentClass ;
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

			//this.classByAlias = original.ClassByAlias;
			//this.classByAlias.put( CriteriaImpl.RootAlias, persistentClass);
		
			this.criteria = original.Criteria;
			this.orderings = original.Orderings;
			this.fetchModes = original.FetchModes;
			this.associationPathByAlias = original.AssociationPathByAlias;
			this.aliasByAssociationPath = original.AliasByAssociationPath;
			this.lockModes = original.LockModes;
			this.maxResults = original.MaxResults;
			this.firstResult = original.FirstResult;
			this.timeout = original.Timeout;
			this.fetchSize = original.FetchSize;
			this.session = original.Session;
			this.resultTransformer = original.ResultTransformer;
			this.counter = original.Counter;
			this.cacheable = original.Cacheable;
			this.cacheRegion = original.CacheRegion;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxResults"></param>
		/// <returns></returns>
		public ICriteria SetMaxResults( int maxResults )
		{
			selection.MaxRows = maxResults;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="firstResult"></param>
		/// <returns></returns>
		public ICriteria SetFirstResult( int firstResult )
		{
			selection.FirstRow = firstResult;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public ICriteria SetTimeout( int timeout )
		{
			selection.Timeout = timeout;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fetchSize"></param>
		/// <returns></returns>
		public ICriteria SetFetchSize( int fetchSize )
		{
			this.fetchSize = fetchSize;
			return this;
		}

		//ADDED this
		/// <summary></summary>
		public NExpression.ICriterion Expression
		{
			get { return conjunction; }
		}

		/// <summary></summary>
		public RowSelection Selection
		{
			get { return selection; }
		}

		internal IList Criteria
		{
			get { return criteria; }
		}

		internal IList Orderings
		{
			get { return orderings; }
		}

		internal IDictionary LockModes
		{
			get { return lockModes; }
		}

		internal IDictionary FetchModes
		{
			get { return fetchModes; }
		}

		internal IDictionary AssociationPathByAlias
		{
			get { return associationPathByAlias; }
		}

		internal IDictionary AliasByAssociationPath
		{
			get { return aliasByAssociationPath; }
		}

		internal SessionImpl Session
		{
			get { return session; }
		}

		internal IResultTransformer ResultTransformer
		{
			get { return resultTransformer; }
		}

		internal int Counter
		{
			get { return counter; }
		}

		internal bool Cacheable 
		{
			get { return cacheable; }
		}

		internal string CacheRegion
		{
			get { return cacheRegion; }
		}

		/// <summary></summary>
		public int MaxResults
		{
			get { return maxResults; }
		}

		/// <summary></summary>
		public int FirstResult
		{
			get { return firstResult; }
		}

		/// <summary></summary>
		public int Timeout
		{
			get { return timeout; }
		}

		/// <summary></summary>
		public int FetchSize
		{
			get { return fetchSize; }
		}

		/// <summary></summary>
		public bool IsJoin( string path )
		{
			return aliasByAssociationPath.Contains( path );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public ICriteria Add( NExpression.ICriterion expression )
		{
			criteria.Add( expression );
			conjunction.Add( expression );
			return this;
		}

		/// <summary></summary>
		public IList List()
		{
			return session.Find( this );
		}

		/// <summary></summary>
		public IEnumerator IterateExpressions()
		{
			return criteria.GetEnumerator();
		}

		/// <summary></summary>
		public IEnumerator IterateOrderings()
		{
			return orderings.GetEnumerator();
		}

		/// <summary></summary>
		public System.Type PersistentClass
		{
			get { return persistentClass; }
		}

		/// <summary></summary>
		public override string ToString()
		{
			return criteria.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ordering"></param>
		/// <returns></returns>
		public ICriteria AddOrder( NExpression.Order ordering )
		{
			orderings.Add( ordering );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public FetchMode GetFetchMode( string path )
		{
			if( fetchModes.Contains( path ) )
			{
				return ( FetchMode ) fetchModes[ path ];
			}
			else
			{
				return FetchMode.Default;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="associationPath"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public ICriteria SetFetchMode( string associationPath, FetchMode mode )
		{
			fetchModes[ associationPath ] = mode;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lockMode"></param>
		/// <returns></returns>
		public ICriteria SetLockMode( LockMode lockMode )
		{
			return SetLockMode( CriteriaImpl.RootAlias, lockMode );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="lockMode"></param>
		/// <returns></returns>
		public ICriteria SetLockMode( string alias, LockMode lockMode )
		{
			lockModes[ alias ] = lockMode;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="associationPath"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public ICriteria CreateAlias( string associationPath, string alias )
		{
			CreateAlias( CriteriaImpl.RootAlias, associationPath, alias );
			return this;
		}

		private void CreateAlias( string rootAlias, string associationPath, string alias )
		{
			string testAlias = StringHelper.Root( associationPath );
			if ( classByAlias.Contains( testAlias ) )
			{
				rootAlias = testAlias;
				associationPath = associationPath.Substring( rootAlias.Length + 1 );
			}

			string rootPath = (string) associationPathByAlias[ rootAlias ];
			string wholeAssociationPath;
			if ( rootPath == null )
			{
				if ( !CriteriaImpl.RootAlias.Equals( rootAlias ) )
				{
					throw new HibernateException( string.Format( "unknown alias: {0}", rootAlias ) );
				}
				wholeAssociationPath = associationPath;
			}
			else
			{
				wholeAssociationPath = StringHelper.Qualify( rootPath, associationPath );
			}

			object oldPath = associationPathByAlias[ alias ] = wholeAssociationPath;
			if ( oldPath != null )
			{
			}
			object oldAlias = aliasByAssociationPath[ wholeAssociationPath ] = alias;
			if ( oldAlias != null )
			{
			}
			classByAlias[ alias ] = GetClassForPath( rootAlias, associationPath );
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="associationPath"></param>
		/// <returns></returns>
		public string GetAlias( string associationPath )
		{
			return (string) aliasByAssociationPath[ associationPath ];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rootAlias"></param>
		/// <param name="associationPath"></param>
		/// <returns></returns>
		public System.Type GetClassForPath( string rootAlias, string associationPath )
		{
			ISessionFactoryImplementor factory = session.Factory;
			System.Type clazz = (System.Type) classByAlias[ rootAlias ];
			IType type = ( (IPropertyMapping) factory.GetPersister( clazz ) ).ToType( associationPath );
			if ( !type.IsAssociationType )
			{
				throw new QueryException( string.Format( "not an association path: {0}", associationPath ) );
			}

			return ( (IAssociationType) type).GetAssociatedClass( factory );
		}
	}
}