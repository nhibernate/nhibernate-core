using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Util;

namespace NHibernate.Linq
{
	///<summary>
	/// Generic IQueryable base class.
	/// </summary>
	public class Query<T> : IOrderedQueryable<T>
	{
		private readonly Expression expression;
		private readonly QueryProvider provider;

		public Query(QueryProvider provider)
		{
			Guard.AgainstNull(provider, "provider");

			this.provider = provider;
			expression = Expression.Constant(this);
		}

		public Query(QueryProvider provider, Expression expression)
		{
			Guard.AgainstNull(provider, "provider");
			Guard.AgainstNull(expression, "expression");


			if (!typeof (IQueryable<T>).IsAssignableFrom(expression.Type))
				throw new ArgumentOutOfRangeException("expression");

			this.provider = provider;
			this.expression = expression;
		}

		#region IOrderedQueryable<T> Members

		Expression IQueryable.Expression
		{
			get { return expression; }
		}

		System.Type IQueryable.ElementType
		{
			get { return typeof (T); }
		}

		IQueryProvider IQueryable.Provider
		{
			get { return provider; }
		}

		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>) provider.Execute(expression)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) provider.Execute(expression)).GetEnumerator();
		}

		#endregion
	}
}