using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;

namespace NHibernate.Linq
{
	internal class FetchLazyPropertiesResultOperator : ResultOperatorBase
	{
		public override IStreamedData ExecuteInMemory(IStreamedData input)
		{
			throw new NotImplementedException();
		}

		public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
		{
			return inputInfo;
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
