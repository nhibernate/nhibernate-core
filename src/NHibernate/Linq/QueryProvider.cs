using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Linq
{
	/// <summary>
	/// A basic abstract LINQ query provider
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
			System.Type elementType = TypeHelper.GetElementType(expression.Type);
			try
			{
				return
					(IQueryable)
					Activator.CreateInstance(typeof (Query<>).MakeGenericType(elementType), new object[] {this, expression});
			}
			catch (TargetInvocationException tie)
			{
				throw tie.InnerException;
			}
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