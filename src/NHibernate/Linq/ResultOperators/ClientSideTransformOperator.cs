using System;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.StreamedData;

namespace NHibernate.Linq.ResultOperators
{
	public class ClientSideTransformOperator : ResultOperatorBase
	{
		public override IStreamedData ExecuteInMemory(IStreamedData input)
		{
			throw new NotImplementedException();
		}

		public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
		{
		    return null;
		}

		public override ResultOperatorBase Clone(CloneContext cloneContext)
		{
			throw new NotImplementedException();
		}
	}
}