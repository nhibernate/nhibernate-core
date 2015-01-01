using System;
using System.Linq.Expressions;

namespace NHibernate.Linq
{
	public class InsertSyntax<TInput>
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
		/// <typeparam name="TOutput">The type of the output.</typeparam>
		/// <param name="assignmentActions">The assignments.</param>
		/// <returns></returns>
		public int Into<TOutput>(Action<Assignments<TInput, TOutput>> assignmentActions)
		{
			if (assignmentActions == null)
				throw new ArgumentNullException(nameof(assignmentActions));
			var assignments = new Assignments<TInput, TOutput>();
			assignmentActions.Invoke(assignments);
			return InsertInto(assignments);
		}

		/// <summary>
		/// Executes the insert, inserting new entities as specified by the expression
		/// </summary>
		/// <typeparam name="TOutput">The type of the output.</typeparam>
		/// <param name="expression">The expression.</param>
		/// <returns></returns>
		public int As<TOutput>(Expression<Func<TInput, TOutput>> expression)
		{
			var assignments = Assignments<TInput, TOutput>.FromExpression(expression);
			return InsertInto(assignments);
		}

		private int InsertInto<TOutput>(Assignments<TInput, TOutput> assignments)
		{
			return _provider.ExecuteInsert(_sourceExpression, assignments);
		}
	}
}