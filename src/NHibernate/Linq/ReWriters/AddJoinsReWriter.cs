using System.Linq;
using NHibernate.Engine;
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

	public class AddJoinsReWriter : QueryModelVisitorBase, IIsEntityDecider
	{
		private readonly ISessionFactoryImplementor _sessionFactory;
		private readonly SelectJoinDetector _selectJoinDetector;
		private readonly ResultOperatorAndOrderByJoinDetector _resultOperatorAndOrderByJoinDetector;
		private readonly WhereJoinDetector _whereJoinDetector;

		private AddJoinsReWriter(ISessionFactoryImplementor sessionFactory, QueryModel queryModel)
		{
			_sessionFactory = sessionFactory;
			var joiner = new Joiner(queryModel);
			_selectJoinDetector = new SelectJoinDetector(this, joiner);
			_resultOperatorAndOrderByJoinDetector = new ResultOperatorAndOrderByJoinDetector(this, joiner);
			_whereJoinDetector = new WhereJoinDetector(this, joiner);
		}

		public static void ReWrite(QueryModel queryModel, ISessionFactoryImplementor sessionFactory)
		{
			new AddJoinsReWriter(sessionFactory, queryModel).VisitQueryModel(queryModel);
		}

		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
		{
			_selectJoinDetector.Transform(selectClause);
		}

		public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
		{
			_resultOperatorAndOrderByJoinDetector.Transform(ordering);
		}

		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
			_resultOperatorAndOrderByJoinDetector.Transform(resultOperator);
		}

		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			_whereJoinDetector.Transform(whereClause);
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
	}
}
