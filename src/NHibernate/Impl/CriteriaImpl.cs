using System;
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

		
		public ICriteria SetMaxResults(int maxResults) 
		{
			selection.MaxRows = maxResults;
			return this;
		}
		
		public ICriteria SetFirstResult(int firstResult) 
		{
			selection.FirstRow = firstResult;
			return this;
		}
		
		public ICriteria SetTimeout(int timeout) 
		{
			selection.Timeout = timeout;
			return this;
		}
	
		public ICriteria Add(NExpression.Expression expression) 
		{
			expressions.Add(expression);
			conjunction.Add(expression);
			return this;
		}
	
		//ADDED this
		public NExpression.Expression Expression 
		{
			get {return conjunction;}
		}
		
		public RowSelection Selection 
		{
			get { return selection; }
		}
	
		public CriteriaImpl(System.Type persistentClass, SessionImpl session) 
		{
			this.persistentClass = persistentClass;
			this.session = session;
		}
	
		public IList List() 
		{
			return session.Find(this);
		}
	
		public IEnumerator IterateExpressions() 
		{
			return expressions.GetEnumerator();
		}
	
		public IEnumerator IterateOrderings() 
		{ 
			return orderings.GetEnumerator(); 
		} 
    
		public System.Type PersistentClass 
		{
			get { return persistentClass; }
		}
	
		public override string ToString() {
			return expressions.ToString();
		}

		public ICriteria AddOrder(NExpression.Order ordering) 
		{ 
			orderings.Add(ordering); 
			return this; 
		}          

		public FetchMode GetFetchMode(string path) 
		{
			if (fetchModes.Contains(path))
			{
				return (FetchMode)fetchModes[path];
			}
			else
			{
			    return FetchMode.Default;
			}
		}

		public ICriteria SetFetchMode(string associationPath, FetchMode mode)
		{
			fetchModes[associationPath] = mode;
			return this;
		}
	}
}
