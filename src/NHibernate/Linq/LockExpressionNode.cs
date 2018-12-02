using System;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace NHibernate.Linq
{
	internal class LockExpressionNode : ResultOperatorExpressionNodeBase
	{
		private static readonly ParameterExpression Parameter = Expression.Parameter(typeof(object));
		
		private readonly ConstantExpression _lockMode;
		private readonly ResolvedExpressionCache<Expression> _cache;

		public LockExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression lockMode)
			: base(parseInfo, null, null)
		{
			_lockMode = lockMode;
			_cache = new ResolvedExpressionCache<Expression>(this);
		}

		public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
		{
			return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
		}

		protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
		{
			//Resolve identity expression (_=>_). Normally this would be resolved into QuerySourceReferenceExpression.
			
			var expression = _cache.GetOrCreate(
				r => r.GetResolvedExpression(Parameter, Parameter, clauseGenerationContext));

			if (!(expression is QuerySourceReferenceExpression qsrExpression))
				throw new NotSupportedException($"WithLock is not supported on {expression}");

			return new LockResultOperator(qsrExpression, _lockMode);
		}
	}
}
