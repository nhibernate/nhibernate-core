using System;
using System.Collections;

using NExpression = NHibernate.Expression;

namespace NHibernate.Impl {
	/// <summary>
	/// Summary description for CriteriaImpl.
	/// </summary>
	internal class CriteriaImpl : ICriteria {
		private IList expressions = new ArrayList();
		private NExpression.Junction conjunction = NExpression.Expression.Conjunction();

		private IList orderings = new ArrayList();
		private int maxResults;
		private int firstResult;
		private int timeout;
		private System.Type persistentClass;
		private SessionImpl session;
	
		public ICriteria SetMaxResults(int maxResults) {
			this.maxResults = maxResults;
			return this;
		}
		public ICriteria SetFirstResult(int firstResult) {
			this.firstResult = firstResult;
			return this;
		}
		public ICriteria SetTimeout(int timeout) {
			this.timeout = timeout;
			return this;
		}
	
		public ICriteria Add(NExpression.Expression expression) {
			expressions.Add(expression);
			conjunction.Add(expression);
			return this;
		}
	
		//ADDED this
		public NExpression.Expression Expression {
			get {return conjunction;}
		}

		public int MaxResults {
			get { return maxResults; }
		}
		public int FirstResult {
			get { return firstResult; }
		}
		public int Timeout {
			get { return timeout; }
		}
	
		public CriteriaImpl(System.Type persistentClass, SessionImpl session) {
			this.persistentClass = persistentClass;
			this.session = session;
		}
	
		public IList List() {
			return session.Find(this);
	
		}
	
		public IEnumerator IterateExpressions() {
			return expressions.GetEnumerator();
		}
	
		public IEnumerator IterateOrderings() { 
			return orderings.GetEnumerator(); 
		} 
    
		public System.Type PersistentClass {
			get { return persistentClass; }
		}
	
		public override string ToString() {
			return expressions.ToString();
		}

		public ICriteria AddOrder(NExpression.Order ordering) { 
			orderings.Add(ordering); 
			return this; 
		}          
	}
}
