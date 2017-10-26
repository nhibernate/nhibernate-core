using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;


namespace NHibernate.Linq.Visitors
{
	public class QueryOptionsExtractor
	{
		public static IReadOnlyCollection<Action<IQueryableOptions>> ExtractOptions(QueryModel queryModel)
		{
			var operators = new List<OptionsResultOperator>();
			var rootFromClause = GetRootFromClause(queryModel);
			ExtractOptionsAndAddToCollection(rootFromClause, queryModel, operators);
			return operators.Select(x => x.SetOptions?.Value as Action<IQueryableOptions>).Where(x => x != null).ToList();
		}

		private static void ExtractOptionsAndAddToCollection(MainFromClause rootFromClause, QueryModel queryModel, ICollection<OptionsResultOperator> operators)
		{
			queryModel.TransformExpressions(x => new OptionExtractionVisitor(rootFromClause, operators).Visit(x));

			var optionsResultOperators = queryModel.ResultOperators.OfType<OptionsResultOperator>().ToList();

			if (!optionsResultOperators.Any())
			{
				return;
			}

			var subQueryFrom = GetRootFromClause(queryModel);
			var sameAsRoot = rootFromClause == subQueryFrom;

			foreach (var optionsResultOperator in optionsResultOperators)
			{
				if (sameAsRoot)
				{
					operators.Add(optionsResultOperator);
				}
				queryModel.ResultOperators.Remove(optionsResultOperator);
			}
		}

		private static MainFromClause GetRootFromClause(QueryModel root)
		{
			var current = root;
			while (current.MainFromClause.FromExpression is SubQueryExpression)
			{
				current = ((SubQueryExpression)current.MainFromClause.FromExpression).QueryModel;
			}
			return current.MainFromClause;
		}





		private class OptionExtractionVisitor : RelinqExpressionVisitor
		{
			private readonly MainFromClause _root;
			private readonly ICollection<OptionsResultOperator> _operators;

			internal OptionExtractionVisitor(MainFromClause rootFromClause, ICollection<OptionsResultOperator> operators)
			{
				_root = rootFromClause;
				_operators = operators;
			}


			protected override Expression VisitSubQuery(SubQueryExpression expression)
			{
				ExtractOptionsAndAddToCollection(_root, expression.QueryModel, _operators);
				return expression;
			}
		}
	}
}
