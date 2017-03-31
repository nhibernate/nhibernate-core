using System;
using System.Linq.Expressions;

namespace NHibernate.Linq
{
	/// <summary>
	/// An update object on which values to update can be specified.
	/// </summary>
	/// <typeparam name="T">The type of the entities to update.</typeparam>
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
		/// <param name="versioned">If set to <c>true</c> [versioned].</param>
		/// <returns>The number of updated entities.</returns>
		public int Assign(Action<Assignments<T, T>> assignments, bool versioned = false)
		{
			var u = new Assignments<T, T>();
			assignments.Invoke(u);

			return ExecuteUpdate(versioned, u);
		}

		/// <summary>
		/// Specify the assignments and execute the update.
		/// </summary>
		/// <param name="expression">The assignments expressed as a member initialization, e.g. <c>x => new Dog { Name = x.Name, Age = x.Age + 5 }</c>. Unset members are ignored and left untouched.</param>
		/// <param name="versioned">If set to <c>true</c> [versioned].</param>
		/// <returns>The number of updated entities.</returns>
		public int As(Expression<Func<T, T>> expression, bool versioned = false)
		{
			var assignments = Assignments<T, T>.FromExpression(expression);
			return ExecuteUpdate(versioned, assignments);
		}

		private int ExecuteUpdate(bool versioned, Assignments<T, T> assignments)
		{
			return _provider.ExecuteUpdate(_sourceExpression, assignments, versioned);
		}
	}
}