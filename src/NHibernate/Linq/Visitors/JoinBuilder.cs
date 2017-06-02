using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public interface IJoiner
	{
		Expression AddJoin(Expression expression, string key);
		bool CanAddJoin(Expression expression);
		void MakeInnerIfJoined(string key);
	}

	public class Joiner : IJoiner
	{
		private readonly Dictionary<string, AdditionalFromClause> _joins = new Dictionary<string, AdditionalFromClause>();
		private readonly NameGenerator _nameGenerator;
		private readonly VisitorParameters _parameters;
		private readonly QueryModel _queryModel;

		internal Joiner(QueryModel queryModel, VisitorParameters parameters)
		{
			_nameGenerator = new NameGenerator(queryModel);
			_parameters = parameters;
			_queryModel = queryModel;
		}

		public Expression AddJoin(Expression expression, string key)
		{
			if (!_joins.TryGetValue(key, out AdditionalFromClause join))
			{
				join = new AdditionalFromClause(_nameGenerator.GetNewName(), expression.Type, expression);
				_parameters.AddLeftJoin(join, null);
				_queryModel.BodyClauses.Add(join);
				_joins.Add(key, join);
			}

			return new QuerySourceReferenceExpression(join);
		}

		public void MakeInnerIfJoined(string key)
		{
			// key is not joined if it occurs only at tails of expressions, e.g.
			// a.B == null, a.B != null, a.B == c.D etc.
			if (_joins.TryGetValue(key, out AdditionalFromClause join))
			{
				_parameters.MakeInnerJoin(join);
			}
		}

		public bool CanAddJoin(Expression expression)
		{
			var source = QuerySourceExtractor.GetQuerySource(expression);

			if (_queryModel.MainFromClause == source)
				return true;

			if (source is IBodyClause bodyClause && _queryModel.BodyClauses.Contains(bodyClause))
				return true;

			var resultOperatorBase = source as ResultOperatorBase;
			return resultOperatorBase != null && _queryModel.ResultOperators.Contains(resultOperatorBase);
		}

		private class QuerySourceExtractor : RelinqExpressionVisitor
		{
			private IQuerySource _querySource;

			public static IQuerySource GetQuerySource(Expression expression)
			{
				var sourceExtractor = new QuerySourceExtractor();
				sourceExtractor.Visit(expression);
				return sourceExtractor._querySource;
			}

			protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
			{
				_querySource = expression.ReferencedQuerySource;
				return base.VisitQuerySourceReference(expression);
			}
		}
	}
}