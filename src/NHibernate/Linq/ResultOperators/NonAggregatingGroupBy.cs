using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;

namespace NHibernate.Linq.ResultOperators
{
	public class NonAggregatingGroupBy : ClientSideTransformOperator
	{
		public NonAggregatingGroupBy(GroupResultOperator groupBy)
		{
			GroupBy = groupBy;
		}

		public GroupResultOperator GroupBy { get; }

		public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo) =>
			GroupBy.GetOutputDataInfo(inputInfo);
	}
}
