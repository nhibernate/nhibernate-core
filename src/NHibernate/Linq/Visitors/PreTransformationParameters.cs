using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Contains the information needed by <see cref="NhRelinqQueryParser.PreTransform"/> to perform an early transformation.
	/// </summary>
	public class PreTransformationParameters
	{
		/// <summary>
		/// The default constructor.
		/// </summary>
		/// <param name="queryMode">The query mode of the expression to pre-transform.</param>
		/// <param name="sessionFactory">The session factory used in the pre-transform process.</param>
		public PreTransformationParameters(QueryMode queryMode, ISessionFactoryImplementor sessionFactory)
		{
			QueryMode = queryMode;
			SessionFactory = sessionFactory;
			// Skip detecting variables for DML queries as HQL does not support reusing parameters for them.
			MinimizeParameters = QueryMode == QueryMode.Select;
		}

		/// <summary>
		/// The query mode of the expression to pre-transform.
		/// </summary>
		public QueryMode QueryMode { get; }

		/// <summary>
		/// The session factory used in the pre-transform process.
		/// </summary>
		public ISessionFactoryImplementor SessionFactory { get; }

		/// <summary>
		/// Whether to minimize the number of parameters for variables.
		/// </summary>
		public bool MinimizeParameters { get; set; }

		/// <summary>
		/// The filter which decides whether a part of the expression will be pre-evalauted or not.
		/// </summary>
		internal IEvaluatableExpressionFilter EvaluatableExpressionFilter { get; set; }

		/// <summary>
		/// A dictionary of <see cref="ConstantExpression"/> that were evaluated from variables.
		/// </summary>
		internal IDictionary<ConstantExpression, QueryVariable> QueryVariables { get; set; }
	}
}
