using System;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace NHibernate.Linq
{
	internal class LockResultOperator : ResultOperatorBase
	{
		public MethodCallExpressionParseInfo ParseInfo { get; }
		public ConstantExpression LockMode { get; }

		public LockResultOperator(MethodCallExpressionParseInfo parseInfo, ConstantExpression lockMode)
		{
			ParseInfo = parseInfo;
			LockMode = lockMode;
		}

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
