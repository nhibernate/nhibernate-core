using System;
using NHibernate.Linq.Clauses;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.Visitors
{
	// Since v5.5
	[Obsolete("This class is not used and will be removed in a future version")]
	public class QuerySourceLocator : NhQueryModelVisitorBase
	{
		readonly System.Type _type;
		IQuerySource _querySource;

		QuerySourceLocator(System.Type type)
		{
			_type = type;
		}

		public static IQuerySource FindQuerySource(QueryModel queryModel, System.Type type)
		{
			var finder = new QuerySourceLocator(type);

			finder.VisitQueryModel(queryModel);

			return finder._querySource;
		}

		public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
		{
			if (_type.IsAssignableFrom(fromClause.ItemType))
			{
				if (_querySource == null)
				{
					_querySource = fromClause;
					return;
				}
			}

			base.VisitAdditionalFromClause(fromClause, queryModel, index);
		}

		public override void VisitNhJoinClause(NhJoinClause joinClause, QueryModel queryModel, int index)
		{
			if (_type.IsAssignableFrom(joinClause.ItemType))
			{
				if (_querySource == null)
				{
					_querySource = joinClause;
					return;
				}
			}

			base.VisitNhJoinClause(joinClause, queryModel, index);
		}

		public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
		{
			if (_type.IsAssignableFrom(fromClause.ItemType) || fromClause.ItemType.IsAssignableFrom(_type))
			{
				_querySource = fromClause;
			}
			else
			{
				base.VisitMainFromClause(fromClause, queryModel);
			}
		}
	}
}
