using System.Collections;
using NHibernate.Engine;
using NExpression = NHibernate.Expression;

namespace NHibernate.Impl
{
	/// <summary>
	/// Implementation of the <see cref="ICriteria"/> interface
	/// </summary>
	internal class CriteriaImpl : ICriteria
	{
		private IList expressions = new ArrayList();
		private IList orderings = new ArrayList();
		private IDictionary fetchModes = new Hashtable();

		private RowSelection selection = new RowSelection();
		private System.Type persistentClass;
		private SessionImpl session;

		private NExpression.Junction conjunction = NExpression.Expression.Conjunction();

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
		/// <param name="expression"></param>
		/// <returns></returns>
		public ICriteria Add( NExpression.Expression expression )
		{
			expressions.Add( expression );
			conjunction.Add( expression );
			return this;
		}

		//ADDED this
		/// <summary></summary>
		public NExpression.Expression Expression
		{
			get { return conjunction; }
		}

		/// <summary></summary>
		public RowSelection Selection
		{
			get { return selection; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="session"></param>
		public CriteriaImpl( System.Type persistentClass, SessionImpl session )
		{
			this.persistentClass = persistentClass;
			this.session = session;
		}

		/// <summary></summary>
		public IList List()
		{
			return session.Find( this );
		}

		/// <summary></summary>
		public IEnumerator IterateExpressions()
		{
			return expressions.GetEnumerator();
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
			return expressions.ToString();
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
	}
}