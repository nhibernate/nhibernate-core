using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace NHibernate.Linq
{
	internal sealed class FetchLazyPropertiesExpressionNode : ResultOperatorExpressionNodeBase
	{
		public FetchLazyPropertiesExpressionNode(MethodCallExpressionParseInfo parseInfo)
			: base(parseInfo, null, null)
		{
		}

		public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
		{
			return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
		}

		protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
		{
			return new FetchLazyPropertiesResultOperator();
		}
	}
}
