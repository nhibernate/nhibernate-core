using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// The result of <see cref="NhRelinqQueryParser.PreTransform"/> method.
	/// </summary>
	public class PreTransformationResult
	{
		internal PreTransformationResult(
			Expression expression,
			ISessionFactoryImplementor sessionFactory,
			IDictionary<ConstantExpression, QueryVariable> queryVariables)
		{
			Expression = expression;
			SessionFactory = sessionFactory;
			QueryVariables = queryVariables;
		}

		/// <summary>
		/// The transformed expression.
		/// </summary>
		public Expression Expression { get; }

		/// <summary>
		/// The session factory used in the pre-transform process.
		/// </summary>
		public ISessionFactoryImplementor SessionFactory { get; }

		/// <summary>
		/// A dictionary of <see cref="ConstantExpression"/> that were evaluated from variables.
		/// </summary>
		internal IDictionary<ConstantExpression, QueryVariable> QueryVariables { get; }
	}
}
