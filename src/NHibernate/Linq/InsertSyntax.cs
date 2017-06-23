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
			return InsertInto(assignments);
		}

		internal int InsertInto<TTarget>(Assignments<TSource, TTarget> assignments)
		{
			return _provider.ExecuteInsert(_sourceExpression, assignments);
		}
	}
}
