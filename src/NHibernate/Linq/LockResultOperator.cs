using System;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace NHibernate.Linq
{
	internal class LockResultOperator : ResultOperatorBase
	{
		private QuerySourceReferenceExpression _qsrExpression;

		public IQuerySource QuerySource => _qsrExpression.ReferencedQuerySource;

		public ConstantExpression LockMode { get; }

		public LockResultOperator(QuerySourceReferenceExpression qsrExpression, ConstantExpression lockMode)
		{
			_qsrExpression = qsrExpression;
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
			_qsrExpression = (QuerySourceReferenceExpression) transformation(_qsrExpression);
		}
	}
}
