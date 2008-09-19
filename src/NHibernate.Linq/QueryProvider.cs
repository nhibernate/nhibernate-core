using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Util;

namespace NHibernate.Linq
{
	/// <summary>
	/// Generic IQueryProvider base class.
	/// </summary>
	public abstract class QueryProvider : IQueryProvider
	{
		#region IQueryProvider Members

		IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
		{
			return new Query<T>(this, expression);
		}

		IQueryable IQueryProvider.CreateQuery(Expression expression)
		{
			System.Type elementType = TypeSystem.GetElementType(expression.Type);
			return
				(IQueryable)
				Activator.CreateInstance(typeof (Query<>).MakeGenericType(elementType), new object[] {this, expression});
		}

		T IQueryProvider.Execute<T>(Expression expression)
		{
			return (T) Execute(expression);
		}

		object IQueryProvider.Execute(Expression expression)
		{
			return Execute(expression);
		}

		#endregion

		public abstract object Execute(Expression expression);
	}
}