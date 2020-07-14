using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Linq.Clauses;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.ReWriters
{
	internal interface IIsEntityDecider
	{
		bool IsEntity(MemberExpression expression, out bool isIdentifier);
	}

	public class AddJoinsReWriter : NhQueryModelVisitorBase, IIsEntityDecider, INhQueryModelVisitorExtended
	{
		private readonly ISessionFactoryImplementor _sessionFactory;
		private readonly MemberExpressionJoinDetector _memberExpressionJoinDetector;
		private readonly WhereJoinDetector _whereJoinDetector;
		private JoinClause _currentJoin;
		private bool? _innerJoin;

		private AddJoinsReWriter(ISessionFactoryImplementor sessionFactory, QueryModel queryModel)
		{
			_sessionFactory = sessionFactory;
			var joiner = new Joiner(queryModel, AddJoin);
			_memberExpressionJoinDetector = new MemberExpressionJoinDetector(this, joiner, _sessionFactory);
			_whereJoinDetector = new WhereJoinDetector(this, joiner, _sessionFactory);
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

		public void VisitNhOuterJoinClause(NhOuterJoinClause nhOuterJoinClause, QueryModel queryModel, int index)
		{
			VisitJoinClause(nhOuterJoinClause.JoinClause, false);
		}

		public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
		{
			VisitJoinClause(joinClause, true);
		}

		private void VisitJoinClause(JoinClause joinClause, bool innerJoin)
		{
			joinClause.InnerSequence = _whereJoinDetector.Transform(joinClause.InnerSequence);

			// When associations are located in the outer key (e.g. from a in A join b in B b on a.C.D.Id equals b.Id),
			// do nothing and leave them to HQL for adding the missing joins.

			// When associations are located in the inner key (e.g. from a in A join b in B b on a.Id equals b.C.D.Id),
			// we have to move the condition to the where statement, otherwise the query will be invalid (HQL does not
			// support them).
			// Link newly created joins with the current join clause in order to later detect which join type to use.
			_currentJoin = joinClause;
			_innerJoin = innerJoin;
			joinClause.InnerKeySelector = _whereJoinDetector.Transform(joinClause.InnerKeySelector);
			_currentJoin = null;
			_innerJoin = null;
		}

		// Since v5.3
		[Obsolete("This method has no usages and will be removed in a future version")]
		public bool IsEntity(System.Type type)
		{
			return _sessionFactory.GetImplementors(type.FullName).Any();
		}

		// Since v5.3
		[Obsolete("This method has no usages and will be removed in a future version")]
		public bool IsIdentifier(System.Type type, string propertyName)
		{
			var metadata = _sessionFactory.GetClassMetadata(type);
			return metadata != null && propertyName.Equals(metadata.IdentifierPropertyName);
		}

		private void AddJoin(QueryModel queryModel, NhJoinClause joinClause)
		{
			joinClause.ParentJoinClause = _currentJoin;
			if (_innerJoin == true)
			{
				// Match the parent join type
				joinClause.MakeInner();
			}

			queryModel.BodyClauses.Add(joinClause);
		}

		bool IIsEntityDecider.IsEntity(MemberExpression expression, out bool isIdentifier)
		{
			isIdentifier =
				ExpressionsHelper.TryGetMappedType(_sessionFactory, expression, out var mappedType, out var entityPersister, out _, out var memberPath)
				&& entityPersister?.IdentifierPropertyName == memberPath;

			return mappedType?.IsEntityType == true;
		}
	}
}
