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
		private readonly bool _versioned;

		internal UpdateSyntax(Expression sourceExpression, INhQueryProvider provider, bool versioned)
		{
			_sourceExpression = sourceExpression;
			_provider = provider;
			_versioned = versioned;
		}

		/// <summary>
		/// Specify the assignments and execute the update.
		/// </summary>
		/// <param name="assignmentActions">The assignments.</param>
		/// <returns>The number of updated entities.</returns>
		public int Assign(Action<Assignments<T, T>> assignmentActions)
		{
			if (assignmentActions == null)
				throw new ArgumentNullException(nameof(assignmentActions));
			var assignments = new Assignments<T, T>();
			assignmentActions.Invoke(assignments);

			return ExecuteUpdate(DmlExpressionRewriter.PrepareExpression<T>(_sourceExpression, assignments.List));
		}

		/// <summary>
		/// Specify the assignments and execute the update.
		/// </summary>
		/// <param name="expression">The assignments expressed as a member initialization, e.g.
		/// <c>x => new Dog { Name = x.Name, Age = x.Age + 5 }</c>. Unset members are ignored and left untouched.</param>
		/// <returns>The number of updated entities.</returns>
		public int As(Expression<Func<T, T>> expression)
		{
			return ExecuteUpdate(DmlExpressionRewriter.PrepareExpression(_sourceExpression, expression));
		}

		private int ExecuteUpdate(Expression updateExpression)
		{
			return _provider.ExecuteDml<T>(_versioned ? QueryMode.UpdateVersioned : QueryMode.Update, updateExpression);
		}
	}
}