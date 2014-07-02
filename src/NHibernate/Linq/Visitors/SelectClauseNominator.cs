using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Analyze the select clause to determine what parts can be translated
	/// fully to HQL, and some other properties of the clause.
	/// </summary>
	class SelectClauseHqlNominator : ExpressionTreeVisitor
	{
		private readonly ILinqToHqlGeneratorsRegistry _functionRegistry;

		/// <summary>
		/// The expression parts that can be converted to pure HQL.
		/// </summary>
		public HashSet<Expression> HqlCandidates { get; private set; }

		/// <summary>
		/// If true after an expression have been analyzed, the
		/// expression as a whole contain at least one method call which
		/// cannot be converted to a registered function, i.e. it must
		/// be executed client side.
		/// </summary>
		public bool ContainsUntranslatedMethodCalls { get; private set; }

		private bool _canBeCandidate;
		Stack<bool> _stateStack;

		public SelectClauseHqlNominator(VisitorParameters parameters)
		{
			_functionRegistry = parameters.SessionFactory.Settings.LinqToHqlGeneratorsRegistry;
		}

		internal void Visit(Expression expression)
		{
			HqlCandidates = new HashSet<Expression>();
			ContainsUntranslatedMethodCalls = false;
			_canBeCandidate = true;
			_stateStack = new Stack<bool>();
			_stateStack.Push(false);

			VisitExpression(expression);
		}

		public override Expression VisitExpression(Expression expression)
		{
			try
			{
				var projectConstantsInHql = _stateStack.Peek() ||
											expression != null && IsRegisteredFunction(expression);

				// Set some flags, unless we already have proper values for them:
				//    projectConstantsInHql if they are inside a method call executed server side.
				//    ContainsUntranslatedMethodCalls if a method call must be executed locally.
				var isMethodCall = expression != null && expression.NodeType == ExpressionType.Call;
				if (isMethodCall && (!projectConstantsInHql || !ContainsUntranslatedMethodCalls))
				{
					var isRegisteredFunction = IsRegisteredFunction(expression);
					projectConstantsInHql = projectConstantsInHql || isRegisteredFunction;
					ContainsUntranslatedMethodCalls = ContainsUntranslatedMethodCalls || !isRegisteredFunction;
				}

				_stateStack.Push(projectConstantsInHql);

				if (expression == null)
					return null;

				bool saveCanBeCandidate = _canBeCandidate;

				_canBeCandidate = true;

				if (CanBeEvaluatedInHqlStatementShortcut(expression))
				{
					HqlCandidates.Add(expression);
					return expression;
				}

				base.VisitExpression(expression);

				if (_canBeCandidate)
				{
					if (CanBeEvaluatedInHqlSelectStatement(expression, projectConstantsInHql))
					{
						HqlCandidates.Add(expression);
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