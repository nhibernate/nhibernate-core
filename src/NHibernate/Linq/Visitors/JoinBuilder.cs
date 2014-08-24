using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.Clauses;
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
		private readonly Dictionary<string, NhJoinClause> _joins = new Dictionary<string, NhJoinClause>();
		private readonly NameGenerator _nameGenerator;
		private readonly QueryModel _queryModel;

		internal Joiner(QueryModel queryModel)
		{
			_nameGenerator = new NameGenerator(queryModel);
			_queryModel = queryModel;
		}

		public IEnumerable<NhJoinClause> Joins
		{
			get { return _joins.Values; }
		}

		public Expression AddJoin(Expression expression, string key)
		{
			NhJoinClause join;

			if (!_joins.TryGetValue(key, out join))
			{
				join = new NhJoinClause(_nameGenerator.GetNewName(), expression.Type, expression);
				_queryModel.BodyClauses.Add(join);
				_joins.Add(key, join);
			}

			return new QuerySourceReferenceExpression(@join);
		}

		public void MakeInnerIfJoined(string key)
		{
			// key is not joined if it occurs only at tails of expressions, e.g.
			// a.B == null, a.B != null, a.B == c.D etc.
			NhJoinClause nhJoinClause;
			if (_joins.TryGetValue(key, out nhJoinClause))
			{
				nhJoinClause.MakeInner();
			}
		}

		public bool CanAddJoin(Expression expression)
		{
			var source = QuerySourceExtractor.GetQuerySource(expression);
			
			if (_queryModel.MainFromClause == source) 
				return true;
			
			var bodyClause = source as IBodyClause;
			if (bodyClause != null && _queryModel.BodyClauses.Contains(bodyClause)) 
				return true;
			
			var resultOperatorBase = source as ResultOperatorBase;
			return resultOperatorBase != null && _queryModel.ResultOperators.Contains(resultOperatorBase);
		}

		private class QuerySourceExtractor : ExpressionTreeVisitor
		{
			private IQuerySource _querySource;

			public static IQuerySource GetQuerySource(Expression expression)
			{
				var sourceExtractor = new QuerySourceExtractor();
				sourceExtractor.VisitExpression(expression);
				return sourceExtractor._querySource;
			}

			protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
			{
				_querySource = expression.ReferencedQuerySource;
				return base.VisitQuerySourceReferenceExpression(expression);
			}
		}
	}
}