
using System;
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

		ICriteria<T> ICriteria<T>.Add(Expression<Func<T, bool>> expression)
		{
			_criteria.Add(expression);
			return this;
		}

	}

}
