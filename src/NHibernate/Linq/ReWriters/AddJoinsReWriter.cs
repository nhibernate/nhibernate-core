using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.ReWriters
{
	internal interface IIsEntityDecider
	{
		bool IsEntity(System.Type type);
	}

	public class AddJoinsReWriter : QueryModelVisitorBase, IIsEntityDecider
	{
		private readonly Dictionary<string, NhJoinClause> _joins = new Dictionary<string, NhJoinClause>();
		private readonly Dictionary<MemberExpression, QuerySourceReferenceExpression> _expressionMap = new Dictionary<MemberExpression, QuerySourceReferenceExpression>();
		private readonly NameGenerator _nameGenerator;
		private readonly ISessionFactory _sessionFactory;

		private AddJoinsReWriter(NameGenerator nameGenerator, ISessionFactory sessionFactory)
		{
			_nameGenerator = nameGenerator;
			_sessionFactory = sessionFactory;
		}

		public static void ReWrite(QueryModel queryModel, ISessionFactory sessionFactory)
		{
			new AddJoinsReWriter(new NameGenerator(queryModel), sessionFactory).ReWrite(queryModel);
		}

		private void ReWrite(QueryModel queryModel)
		{
			VisitQueryModel(queryModel);

			if (_joins.Count > 0)
			{
				MemberExpressionSwapper swap = new MemberExpressionSwapper(_expressionMap);
				queryModel.TransformExpressions(swap.VisitExpression);

				foreach (var join in _joins.Values)
				{
					queryModel.BodyClauses.Add(join);
				}
			}
		}

		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
		{
			new SelectAndOrderByJoinDetector(_nameGenerator, this, _joins, _expressionMap).VisitExpression(selectClause.Selector);
		}

		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			WhereJoinDetector.Find(whereClause.Predicate, _nameGenerator,
											  this,
											  _joins,
											  _expressionMap);
		}

		public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
		{
			var joinDetector = new SelectAndOrderByJoinDetector(_nameGenerator, this, _joins, _expressionMap);
			foreach (Ordering ordering in orderByClause.Orderings)
			{
				joinDetector.VisitExpression(ordering.Expression);
			}
		}

		public bool IsEntity(System.Type type)
		{
			return _sessionFactory.GetClassMetadata(type) != null;
		}
	}

	public class MemberExpressionSwapper : NhExpressionTreeVisitor
	{
		private readonly Dictionary<MemberExpression, QuerySourceReferenceExpression> _expressionMap;

		public MemberExpressionSwapper(Dictionary<MemberExpression, QuerySourceReferenceExpression> expressionMap)
		{
			_expressionMap = expressionMap;
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			if (expression == null)
			{
				return null;
			}

			QuerySourceReferenceExpression replacement;

			if (_expressionMap.TryGetValue(expression, out replacement))
			{
				return replacement;
			}
			else
			{
				return base.VisitMemberExpression(expression);
			}
		}
	}
}
