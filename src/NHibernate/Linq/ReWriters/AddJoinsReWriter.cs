using System;
using System.Collections.Specialized;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Linq.Clauses;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.ReWriters
{
	internal interface IIsEntityDecider
	{
		bool IsEntity(System.Type type);
		bool IsIdentifier(System.Type type, string propertyName);
	}

	public class AddJoinsReWriter : NhQueryModelVisitorBase, IIsEntityDecider
	{
		private readonly ISessionFactoryImplementor _sessionFactory;
		private readonly MemberExpressionJoinDetector _memberExpressionJoinDetector;
		private readonly WhereJoinDetector _whereJoinDetector;
		private int? _joinInsertIndex;
		private JoinClause _currentJoin;

		private AddJoinsReWriter(ISessionFactoryImplementor sessionFactory, QueryModel queryModel)
		{
			_sessionFactory = sessionFactory;
			var joiner = new Joiner(queryModel, AddJoin);
			_memberExpressionJoinDetector = new MemberExpressionJoinDetector(this, joiner);
			_whereJoinDetector = new WhereJoinDetector(this, joiner);
		}

		public static void ReWrite(QueryModel queryModel, VisitorParameters parameters)
		{
			var visitor = new AddJoinsReWriter(parameters.SessionFactory, queryModel);
			visitor.VisitQueryModel(queryModel);
		}

		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
		{
			_memberExpressionJoinDetector.Transform(selectClause);
		}

		public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
		{
			_memberExpressionJoinDetector.Transform(ordering);
		}

		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
			_memberExpressionJoinDetector.Transform(resultOperator);
		}

		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			_whereJoinDetector.Transform(whereClause);
		}

		public override void VisitNhHavingClause(NhHavingClause havingClause, QueryModel queryModel, int index)
		{
			_whereJoinDetector.Transform(havingClause);
		}

		public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
		{
			VisitJoinClause(joinClause, queryModel, joinClause);
		}

		private void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, IBodyClause bodyClause)
		{
			joinClause.InnerSequence = _whereJoinDetector.Transform(joinClause.InnerSequence);

			// When associations are located in the outer key (e.g. from a in A join b in B b on a.C.D.Id equals b.Id),
			// we have to insert the association join before the current join in order to produce a valid query.
			_joinInsertIndex = queryModel.BodyClauses.IndexOf(bodyClause);
			joinClause.OuterKeySelector = _whereJoinDetector.Transform(joinClause.OuterKeySelector);
			_joinInsertIndex = null;

			// When associations are located in the inner key (e.g. from a in A join b in B b on a.Id equals b.C.D.Id),
			// we have to move the condition to the where statement, otherwise the query will be invalid.
			// Link newly created joins with the current join clause in order to later detect which join type to use.
			_currentJoin = joinClause;
			joinClause.InnerKeySelector = _whereJoinDetector.Transform(joinClause.InnerKeySelector);
			_currentJoin = null;
		}

		public bool IsEntity(System.Type type)
		{
			return _sessionFactory.GetImplementors(type.FullName).Any();
		}

		public bool IsIdentifier(System.Type type, string propertyName)
		{
			var metadata = _sessionFactory.GetClassMetadata(type);
			return metadata != null && propertyName.Equals(metadata.IdentifierPropertyName);
		}

		private void AddJoin(QueryModel queryModel, NhJoinClause joinClause)
		{
			joinClause.ParentJoinClause = _currentJoin;
			if (_joinInsertIndex.HasValue)
			{
				queryModel.BodyClauses.Insert(_joinInsertIndex.Value, joinClause);
				_joinInsertIndex++;
			}
			else
			{
				queryModel.BodyClauses.Add(joinClause);
			}
		}
	}
}
