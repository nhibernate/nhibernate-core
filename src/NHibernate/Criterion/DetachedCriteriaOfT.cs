
using System;
using System.Linq.Expressions;

using NHibernate.Impl;

namespace NHibernate.Criterion
{

	/// <summary>
	/// Some applications need to create criteria queries in "detached
	/// mode", where the Hibernate session is not available. This class
	/// may be instantiated anywhere, and then a <c>ICriteria</c>
	/// may be obtained by passing a session to 
	/// <c>GetExecutableCriteria()</c>. All methods have the
	/// same semantics and behavior as the corresponding methods of the
	/// <c>ICriteria&lt;T&gt;</c> interface.
	/// </summary>
	[Serializable]
	public class DetachedCriteria<T>
	{

		private DetachedCriteria _criteria;

		protected internal DetachedCriteria(DetachedCriteria detachedCriteria)
		{
			_criteria = detachedCriteria;
		}

		/// <summary>
		/// Get an executable instance of <c>Criteria&lt;T&gt;</c>,
		/// to actually run the query.</summary>
		public ICriteria<T> GetExecutableCriteria(ISession session)
		{
			return new CriteriaImpl<T>(_criteria.GetExecutableCriteria(session));
		}

		public DetachedCriteria<T> And(Expression<Func<T, bool>> expression)
		{
			return Add(expression);
		}

		public DetachedCriteria<T> Where(Expression<Func<T, bool>> expression)
		{
			return Add(expression);
		}

		private DetachedCriteria<T> Add(Expression<Func<T, bool>> expression)
		{
			_criteria.Add(ExpressionProcessor.ProcessExpression<T>(expression));
			return this;
		}

		public static implicit operator DetachedCriteria(DetachedCriteria<T> detachedCriteria)
		{
			return detachedCriteria._criteria;
		}

	}

}