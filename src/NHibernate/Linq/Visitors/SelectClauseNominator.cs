using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Expressions;

namespace NHibernate.Linq.Visitors
{
	class SelectClauseHqlNominator : NhExpressionTreeVisitor
	{
		private readonly ILinqToHqlGeneratorsRegistry _functionRegistry;

		private HashSet<Expression> _candidates;
		private bool _canBeCandidate;
		Stack<bool> _stateStack;

		public SelectClauseHqlNominator(VisitorParameters parameters)
		{
			_functionRegistry = parameters.SessionFactory.Settings.LinqToHqlGeneratorsRegistry;
		}

		internal HashSet<Expression> Nominate(Expression expression)
		{
			_candidates = new HashSet<Expression>();
			_canBeCandidate = true;
			_stateStack = new Stack<bool>();
			_stateStack.Push(false);

			VisitExpression(expression);

			return _candidates;
		}

		public override Expression VisitExpression(Expression expression)
		{
			try
			{
				bool projectConstantsInHql = _stateStack.Peek();

				if (!projectConstantsInHql && expression != null && IsRegisteredFunction(expression))
				{
					projectConstantsInHql = true;
				}

				_stateStack.Push(projectConstantsInHql);

				if (expression == null)
					return null;

				bool saveCanBeCandidate = _canBeCandidate;

				_canBeCandidate = true;

				if (CanBeEvaluatedInHqlStatementShortcut(expression))
				{
					_candidates.Add(expression);
					return expression;
				}

				base.VisitExpression(expression);

				if (_canBeCandidate)
				{
					if (CanBeEvaluatedInHqlSelectStatement(expression, projectConstantsInHql))
					{
						_candidates.Add(expression);
					}
					else
					{
						_canBeCandidate = false;
					}
				}

				_canBeCandidate = _canBeCandidate & saveCanBeCandidate;
			}
			finally
			{
				_stateStack.Pop();
			}

			return expression;
		}

		private bool IsRegisteredFunction(Expression expression)
		{
			if (expression.NodeType == ExpressionType.Call)
			{
				var methodCallExpression = (MethodCallExpression) expression;
				IHqlGeneratorForMethod methodGenerator;
				if (_functionRegistry.TryGetGenerator(methodCallExpression.Method, out methodGenerator))
				{
					return methodCallExpression.Object == null || // is static or extension method
						   methodCallExpression.Object.NodeType != ExpressionType.Constant; // does not belong to parameter 
				}
			}
			return false;
		}

		private bool CanBeEvaluatedInHqlSelectStatement(Expression expression, bool projectConstantsInHql)
		{
			// HQL can't do New or Member Init
			if ((expression.NodeType == ExpressionType.MemberInit) || (expression.NodeType == ExpressionType.New))
			{
				return false;
			}

			// Constants will only be evaluated in HQL if they're inside a method call
			if (expression.NodeType == ExpressionType.Constant)
			{
				return projectConstantsInHql;
			}

			if (expression.NodeType == ExpressionType.Call)
			{
				// Depends if it's in the function registry
				if (!IsRegisteredFunction(expression))
					return false;
			}

			// Assume all is good
			return true;
		}

		private static bool CanBeEvaluatedInHqlStatementShortcut(Expression expression)
		{
			return ((NhExpressionType)expression.NodeType) == NhExpressionType.Count;
		}
	}
}