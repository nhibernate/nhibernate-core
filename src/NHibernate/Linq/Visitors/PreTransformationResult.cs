using System.Collections.Generic;
using System.Linq.Expressions;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// The result of <see cref="NhRelinqQueryParser.PreTransform"/> method.
	/// </summary>
	public class PreTransformationResult
	{
		internal PreTransformationResult(
			Expression expression,
			IDictionary<ConstantExpression, QueryVariable> queryVariables)
		{
			Expression = expression;
			QueryVariables = queryVariables;
		}

		/// <summary>
		/// The transformed expression.
		/// </summary>
		public Expression Expression { get; }

		/// <summary>
		/// A dictionary of <see cref="ConstantExpression"/> that were evaluated from variables.
		/// </summary>
		internal IDictionary<ConstantExpression, QueryVariable> QueryVariables { get; }
	}
}
