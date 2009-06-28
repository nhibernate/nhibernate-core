
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NHibernate.Impl
{

	/// <summary>
	/// Implementation of the <see cref="ICriteria&lt;T&gt;"/> interface
	/// </summary>
	[Serializable]
	public class CriteriaImpl<T> : ICriteria<T>
	{

		private ICriteria _criteria;

		public CriteriaImpl(ICriteria criteria)
		{
			_criteria = criteria;
		}

		public ICriteria UnderlyingCriteria
		{
			get { return _criteria; }
		}

		ICriteria<T> ICriteria<T>.And(Expression<Func<T, bool>> expression)
		{
			return Add(expression);
		}

		ICriteria<T> ICriteria<T>.And(Expression<Func<bool>> expression)
		{
			return Add(expression);
		}

		ICriteria<T> ICriteria<T>.Where(Expression<Func<T, bool>> expression)
		{
			return Add(expression);
		}

		ICriteria<T> ICriteria<T>.Where(Expression<Func<bool>> expression)
		{
			return Add(expression);
		}

		ICriteria<U> ICriteria<T>.Join<U>(Expression<Func<T, U>> expression)
		{
			return new CriteriaImpl<U>(
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(expression.Body)));
		}

		ICriteria<U> ICriteria<T>.Join<U>(Expression<Func<T, IEnumerable<U>>> expression)
		{
			return new CriteriaImpl<U>(
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(expression.Body)));
		}

		IList<T> ICriteria<T>.List()
		{
			return _criteria.List<T>();
		}

		private CriteriaImpl<T> Add(Expression<Func<T, bool>> expression)
		{
			_criteria.Add(ExpressionProcessor.ProcessExpression<T>(expression));
			return this;
		}

		private CriteriaImpl<T> Add(Expression<Func<bool>> expression)
		{
			_criteria.Add(ExpressionProcessor.ProcessExpression(expression));
			return this;
		}

	}

}
