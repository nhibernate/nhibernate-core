using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace NHibernate.Linq
{
	internal class LockExpressionNode : ResultOperatorExpressionNodeBase
	{
		private readonly MethodCallExpressionParseInfo _parseInfo;
		private readonly ConstantExpression _lockMode;

		public LockExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression lockMode)
			: base(parseInfo, null, null)
		{
			_parseInfo = parseInfo;
			_lockMode = lockMode;
		}

		public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
		{
			return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
		}

		protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
		{
			return new LockResultOperator(_parseInfo, _lockMode);
		}
	}
}
