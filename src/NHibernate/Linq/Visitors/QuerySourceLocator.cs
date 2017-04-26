using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Collections;

namespace NHibernate.Linq.Visitors
{
    public class QuerySourceLocator : QueryModelVisitorBase
    {
        private readonly System.Type _type;
        private IQuerySource _querySource;

        private QuerySourceLocator(System.Type type)
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

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            if (_type.IsAssignableFrom(fromClause.ItemType))
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