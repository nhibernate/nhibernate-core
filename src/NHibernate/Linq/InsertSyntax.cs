using System;
using System.Linq.Expressions;

namespace NHibernate.Linq
{
	/// <summary>
	/// An insert object on which entities to insert can be specified.
	/// </summary>
	/// <typeparam name="TSource">The type of the entities selected as source of the insert.</typeparam>
	public class InsertSyntax<TSource>
	{
		private readonly Expression _sourceExpression;
		private readonly INhQueryProvider _provider;

		internal InsertSyntax(Expression sourceExpression, INhQueryProvider provider)
		{
			_sourceExpression = sourceExpression;
			_provider = provider;
		}

		/// <summary>
		/// Executes the insert, using the specified assignments.
		/// </summary>
		/// <typeparam name="TTarget">The type of the entities to insert.</typeparam>
		/// <param name="assignmentActions">The assignments.</param>
		/// <returns>The number of inserted entities.</returns>
		public int Into<TTarget>(Action<Assignments<TSource, TTarget>> assignmentActions)
		{
			if (assignmentActions == null)
				throw new ArgumentNullException(nameof(assignmentActions));
			var assignments = new Assignments<TSource, TTarget>();
			assignmentActions.Invoke(assignments);

			return ExecuteInsert<TTarget>(DmlExpressionRewriter.PrepareExpression<TSource>(_sourceExpression, assignments.List));
		}

		/// <summary>
		/// Executes the insert, inserting new entities as specified by the expression.
		/// </summary>
		/// <typeparam name="TTarget">The type of the entities to insert.</typeparam>
		/// <param name="expression">The expression projecting a source entity to the entity to insert.</param>
		/// <returns>The number of inserted entities.</returns>
		public int As<TTarget>(Expression<Func<TSource, TTarget>> expression)
		{
			return ExecuteInsert<TTarget>(DmlExpressionRewriter.PrepareExpression(_sourceExpression, expression));
		}

		private int ExecuteInsert<TTarget>(Expression insertExpression)
		{
			return _provider.ExecuteDml<TTarget>(QueryMode.Insert, insertExpression);
		}
	}
}