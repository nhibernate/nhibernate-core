using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Expressions;
using NHibernate.Param;
using NHibernate.Util;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Analyze the select clause to determine what parts can be translated
	/// fully to HQL, and some other properties of the clause.
	/// </summary>
	class SelectClauseHqlNominator : RelinqExpressionVisitor
	{
		private readonly ILinqToHqlGeneratorsRegistry _functionRegistry;
		private readonly ISessionFactoryImplementor _sessionFactory;
		private readonly VisitorParameters _parameters;

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
			_sessionFactory = parameters.SessionFactory;
			_parameters = parameters;
		}

		internal Expression Nominate(Expression expression)
		{
			HqlCandidates = new HashSet<Expression>();
			ContainsUntranslatedMethodCalls = false;
			_canBeCandidate = true;
			_stateStack = new Stack<bool>();
			_stateStack.Push(false);

			return Visit(expression);
		}

		public override Expression Visit(Expression expression)
		{
			if (expression == null)
				return null;

			if (expression is NhNominatedExpression nominatedExpression)
			{
				// Add the nominated clause and strip the nominator wrapper from the select expression
				var innerExpression = nominatedExpression.Expression;
				HqlCandidates.Add(innerExpression);
				return innerExpression;
			}

			var isRegisteredFunction = IsRegisteredFunction(expression);
			var projectConstantsInHql = _stateStack.Peek() || IsConstantExpression(expression) || IsRegisteredFunction(expression);

			// Set some flags, unless we already have proper values for them:
			//    projectConstantsInHql if they are inside a method call executed server side.
			//    ContainsUntranslatedMethodCalls if a method call must be executed locally.
			if (expression.NodeType == ExpressionType.Call && (!projectConstantsInHql || !ContainsUntranslatedMethodCalls))
			{
				ContainsUntranslatedMethodCalls = ContainsUntranslatedMethodCalls || !isRegisteredFunction;
			}

			_stateStack.Push(projectConstantsInHql);
			bool saveCanBeCandidate = _canBeCandidate;
			_canBeCandidate = true;

			try
			{
				if (CanBeEvaluatedInHqlStatementShortcut(expression))
				{
					HqlCandidates.Add(expression);
					return expression;
				}

				expression = base.Visit(expression);

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
			}
			finally
			{
				_stateStack.Pop();
				_canBeCandidate = _canBeCandidate && saveCanBeCandidate;
			}

			return expression;
		}

		private static bool IsAllowedToProjectInHql(System.Type type)
		{
			return (type.IsValueType || type == typeof(string)) && typeof(DateTime) != type && typeof(DateTime?) != type && typeof(TimeSpan) != type && typeof(TimeSpan?) != type;
		}

		private static bool IsValueType(System.Type type)
		{
			return type.IsValueType || type == typeof(string);
		}

		private static bool IsConstantExpression(Expression expression)
		{
			//if (expression.NodeType != ExpressionType.Equal) return false;

			if(expression == null) return true;

			switch (expression.NodeType)
			{
				case ExpressionType.Extension:
				case ExpressionType.Convert:
					return true;
				case ExpressionType.Constant:
					return IsValueType(expression.Type);
				case ExpressionType.MemberAccess:
					var member = (MemberExpression) expression;
					return IsConstantExpression(member.Expression);
				case ExpressionType.Equal:
				case ExpressionType.NotEqual:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.Not:
					return true;
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
				case ExpressionType.Add:
				case ExpressionType.Subtract:
				case ExpressionType.Multiply:
				case ExpressionType.Divide:
				case ExpressionType.Modulo:
					var binary = (BinaryExpression) expression;
					return IsAllowedToProjectInHql(binary.Left.Type) && IsConstantExpression(binary.Left) && 
						   IsAllowedToProjectInHql(binary.Right.Type) && IsConstantExpression(binary.Right);
				case ExpressionType.Coalesce:
					var coalesce = (BinaryExpression) expression;
					return IsConstantExpression(coalesce.Left) && IsConstantExpression(coalesce.Right);
				case ExpressionType.Conditional:
					var conditional = (ConditionalExpression) expression;
					return IsConstantExpression(conditional.Test) && IsConstantExpression(conditional.IfTrue) && IsConstantExpression(conditional.IfFalse);
				default:
					return false;
			}
		}

		private bool IsRegisteredFunction(Expression expression)
		{
			if (expression.NodeType == ExpressionType.Call)
			{
				var methodCallExpression = (MethodCallExpression) expression;
				if (_functionRegistry.TryGetGenerator(methodCallExpression.Method, out var methodGenerator))
				{
					// is static or extension method
					return methodCallExpression.Object == null ||
						// does not belong to parameter
						methodCallExpression.Object.NodeType != ExpressionType.Constant ||
						// does not ignore the parameter it belongs to
						methodGenerator.IgnoreInstance(methodCallExpression.Method);
				}
			}
			else if (expression is NhSumExpression ||
			         expression is NhCountExpression ||
			         expression is NhAverageExpression ||
			         expression is NhMaxExpression ||
			         expression is NhMinExpression)
			{
				return true;
			}
			return false;
		}

		private bool CanBeEvaluatedInHqlSelectStatement(Expression expression, bool projectConstantsInHql)
		{
			// HQL can't do New or Member Init
			if (expression.NodeType == ExpressionType.MemberInit || 
				expression.NodeType == ExpressionType.New || 
				expression.NodeType == ExpressionType.NewArrayInit ||
				expression.NodeType == ExpressionType.NewArrayBounds)
			{
				return false;
			}

			// Constants will only be evaluated in HQL if they're inside a method call
			if (expression.NodeType == ExpressionType.Constant)
			{
				if (!projectConstantsInHql && _parameters.ConstantToParameterMap.ContainsKey((ConstantExpression)expression))
				{
					_parameters.CanCachePlan = false;
				}

				return projectConstantsInHql;
			}

			if (expression.NodeType == ExpressionType.Call)
			{
				// Depends if it's in the function registry
				return IsRegisteredFunction(expression);
			}

			if (expression.NodeType == ExpressionType.Conditional)
			{
				// Theoretically, any conditional that returns a CAST-able primitive should be constructable in HQL.
				// The type needs to be CAST-able because HQL wraps the CASE clause in a CAST and only supports
				// certain types (as defined by the HqlIdent constructor that takes a System.Type as the second argument).
				// However, this may still not cover all cases, so to limit the nomination of conditional expressions,
				// we will only consider those which are already getting constants projected into them.
				return projectConstantsInHql;
			}

			return !(expression is MemberExpression memberExpression) || // Assume all is good
			       // Nominate only expressions that represent a mapped property or a translatable method call
			       ExpressionsHelper.TryGetMappedType(_sessionFactory, expression, out _, out _, out _, out _) ||
			       _functionRegistry.TryGetGenerator(memberExpression.Member, out _);
		}

		private static bool CanBeEvaluatedInHqlStatementShortcut(Expression expression)
		{
			return expression is NhCountExpression;
		}
	}
}
