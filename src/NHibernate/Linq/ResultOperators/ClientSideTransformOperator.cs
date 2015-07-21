using System;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;

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

	    public override void TransformExpressions(Func<Expression, Expression> transformation)
	    {
	    }
	}
}