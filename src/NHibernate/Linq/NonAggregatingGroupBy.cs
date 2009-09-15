using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq
{
    public class NonAggregatingGroupBy : ClientSideTransformOperator
    {
        public NonAggregatingGroupBy(GroupResultOperator groupBy)
        {
            GroupBy = groupBy;
        }

        public GroupResultOperator GroupBy { get; private set; }
    }
}