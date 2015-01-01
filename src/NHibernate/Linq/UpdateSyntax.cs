using System;
using System.Linq.Expressions;

namespace NHibernate.Linq
{
	public class UpdateSyntax<T>
	{
		private readonly Expression _sourceExpression;
		private readonly INhQueryProvider _provider;

		internal UpdateSyntax(Expression sourceExpression, INhQueryProvider provider)
		{
			_sourceExpression = sourceExpression;
			_provider = provider;
		}

		/// <summary>
		/// Specify the assignments and execute the update.
		/// </summary>
		/// <param name="assignments">The assignments.</param>
		/// <param name="versioned">if set to <c>true</c> [versioned].</param>
		/// <returns></returns>
		public int Assign(Action<Assignments<T, T>> assignments, bool versioned = false)
		{
			var u = new Assignments<T, T>();
			assignments.Invoke(u);

			return ExecuteUpdate(versioned, u);
		}

		/// <summary>
		/// Specify the assignments and execute the update.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query">The query.</param>
		/// <param name="expression">The assignments expressed as a member initialization, e.g. x => new Dog{Name = x.Name,Age = x.Age + 5}.</param>
		/// <param name="versioned">if set to <c>true</c> [versioned].</param>
		/// <returns></returns>
		public int As(Expression<Func<T, T>> expression, bool versioned = false)
		{

			var assignments = Assignments<T, T>.FromExpression(expression);
			return ExecuteUpdate(versioned, assignments);
		}

		private int ExecuteUpdate<T>(bool versioned, Assignments<T, T> assignments)
		{
			return _provider.ExecuteUpdate(_sourceExpression, assignments, versioned);
		}
	}
}