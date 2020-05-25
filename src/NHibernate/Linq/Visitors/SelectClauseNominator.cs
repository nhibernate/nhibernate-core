using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Dialect.Function;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Expressions;
using NHibernate.Util;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Analyze the select clause to determine what parts can be translated
	/// fully to HQL, and some other properties of the clause.
	/// </summary>
	class SelectClauseHqlNominator
	{
		private static readonly HashSet<System.Type> DateTypes = new HashSet<System.Type>
		{
			typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan)
		};

		private static readonly HashSet<ExpressionType> ArithmeticOperations = new HashSet<ExpressionType>
		{
			ExpressionType.Add, ExpressionType.AddChecked,
			ExpressionType.Subtract, ExpressionType.SubtractChecked,
			ExpressionType.Multiply, ExpressionType.MultiplyChecked,
			ExpressionType.Divide
		};

		private static readonly HashSet<ExpressionType> BitwiseOperations = new HashSet<ExpressionType>
		{
			ExpressionType.And,
			ExpressionType.Or,
			ExpressionType.ExclusiveOr,
			ExpressionType.OnesComplement,
			ExpressionType.LeftShift,
			ExpressionType.RightShift
		};

		private readonly ILinqToHqlGeneratorsRegistry _functionRegistry;
		private readonly VisitorParameters _parameters;
		private bool _forceClientSide;

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

		public SelectClauseHqlNominator(VisitorParameters parameters)
		{
			_parameters = parameters;
			_functionRegistry = parameters.SessionFactory.Settings.LinqToHqlGeneratorsRegistry;
		}

		internal void Nominate(Expression expression)
		{
			HqlCandidates = new HashSet<Expression>();
			ContainsUntranslatedMethodCalls = false;
			CanBeEvaluatedInHql(expression);
		}

		private bool CanBeEvaluatedInHql(Expression expression)
		{
			// Do client side evaluation for constants
			if (expression == null || expression.NodeType == ExpressionType.Constant)
			{
				return true;
			}

			bool canBeEvaluated;
			switch (expression.NodeType)
			{
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				case ExpressionType.Divide:
				case ExpressionType.Modulo:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
				case ExpressionType.Power:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
				case ExpressionType.And:
				case ExpressionType.Or:
				case ExpressionType.ExclusiveOr:
				case ExpressionType.LeftShift:
				case ExpressionType.RightShift:
				case ExpressionType.AndAlso:
				case ExpressionType.OrElse:
				case ExpressionType.Equal:
				case ExpressionType.NotEqual:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.GreaterThan:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.Coalesce:
				case ExpressionType.ArrayIndex:
					canBeEvaluated = CanBeEvaluatedInHql((BinaryExpression) expression);
					break;
				case ExpressionType.Conditional:
					canBeEvaluated = CanBeEvaluatedInHql((ConditionalExpression) expression);
					break;
				case ExpressionType.Call:
					canBeEvaluated = CanBeEvaluatedInHql((MethodCallExpression) expression);
					break;
				case ExpressionType.ArrayLength:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
				case ExpressionType.Not:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
				case ExpressionType.UnaryPlus:
					canBeEvaluated = CanBeEvaluatedInHql(((UnaryExpression) expression));
					break;
				case ExpressionType.MemberAccess:
					canBeEvaluated = CanBeEvaluatedInHql((MemberExpression) expression);
					break;
				case ExpressionType.Extension:
					if (expression is NhNominatedExpression nominatedExpression)
					{
						expression = nominatedExpression.Expression;
					}

					canBeEvaluated = true; // Sub queries cannot be executed client side
					break;
				case ExpressionType.MemberInit:
					canBeEvaluated = CanBeEvaluatedInHql((MemberInitExpression) expression);
					break;
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
					canBeEvaluated = CanBeEvaluatedInHql((NewArrayExpression) expression);
					break;
				case ExpressionType.ListInit:
					canBeEvaluated = CanBeEvaluatedInHql((ListInitExpression) expression);
					break;
				case ExpressionType.New:
					canBeEvaluated = CanBeEvaluatedInHql((NewExpression) expression);
					break;
				case ExpressionType.Dynamic:
					canBeEvaluated = CanBeEvaluatedInHql((DynamicExpression) expression);
					break;
				case ExpressionType.Invoke:
					canBeEvaluated = CanBeEvaluatedInHql((InvocationExpression) expression);
					break;
				case ExpressionType.TypeIs:
					canBeEvaluated = CanBeEvaluatedInHql(((TypeBinaryExpression) expression).Expression);
					break;
				default:
					canBeEvaluated = true;
					break;
			}

			if (canBeEvaluated)
			{
				HqlCandidates.Add(expression);
			}

			return canBeEvaluated;
		}

		private bool CanBeEvaluatedInHql(MethodCallExpression methodExpression)
		{
			if (VisitorUtil.TryGetEvalExpression(methodExpression, out var expression))
			{
				if (methodExpression.Method.Name == nameof(ExpressionEvaluation.DatabaseEval))
				{
					HqlCandidates.Add(expression);
					return false;
				}

				if (_forceClientSide)
				{
					throw new InvalidOperationException(
						$"{nameof(ExpressionEvaluation.ClientEval)} cannot be used inside another {nameof(ExpressionEvaluation.ClientEval)}.");
				}

				_forceClientSide = true;
				CanBeEvaluatedInHql(expression);
				_forceClientSide = false;
				return false;
			}

			var canBeEvaluated = _functionRegistry.TryGetGenerator(methodExpression.Method, out var methodGenerator) && !_forceClientSide;
			canBeEvaluated &= methodExpression.Object == null || // Is static or extension method
			                  // Does not ignore the parameter it belongs to
			                  methodGenerator?.IgnoreInstance(methodExpression.Method) == true ||
			                  (
			                      // Does not belong to a parameter
			                      methodExpression.Object.NodeType != ExpressionType.Constant &&
			                      CanBeEvaluatedInHql(methodExpression.Object)
			                  );
			foreach (var argumentExpression in methodExpression.Arguments)
			{
				// If one of the arguments cannot be converted to hql we have to execute the method on the client side
				canBeEvaluated &= CanBeEvaluatedInHql(argumentExpression);
			}

			ContainsUntranslatedMethodCalls |= !canBeEvaluated;
			return canBeEvaluated;
		}

		private bool CanBeEvaluatedInHql(MemberExpression memberExpression)
		{
			if (!CanBeEvaluatedInHql(memberExpression.Expression))
			{
				return false;
			}

			// Check for a mapped property e.g. Count
			if (_functionRegistry.TryGetGenerator(memberExpression.Member, out _))
			{
				return !_forceClientSide;
			}

			// Check whether the member is mapped. TryGetEntityName will return not return the entity name when the
			// member is part of a composite element of a collection, so check if the type was found.
			return ExpressionsHelper.TryGetMappedType(_parameters.SessionFactory, memberExpression, out _, out _, out _, out _);
		}

		private bool CanBeEvaluatedInHql(ConditionalExpression conditionalExpression)
		{
			var canBeEvaluated = CanBeEvaluatedInHql(conditionalExpression.Test);
			// In Oracle, when a query that selects a parameter is executed multiple times with different parameter types,
			// will fail to get the value from the data reader. e.g. select case when <condition> then @p0 else @p1 end.
			// In order to prevent that, we have to execute only the condition on the server side and do the rest on the client side.
			if (canBeEvaluated &&
			    conditionalExpression.IfTrue.NodeType == ExpressionType.Constant &&
			    conditionalExpression.IfFalse.NodeType == ExpressionType.Constant)
			{
				return false;
			}

			canBeEvaluated &= (CanBeEvaluatedInHql(conditionalExpression.IfTrue) && HqlIdent.SupportsType(conditionalExpression.IfTrue.Type)) &
			                  (CanBeEvaluatedInHql(conditionalExpression.IfFalse) && HqlIdent.SupportsType(conditionalExpression.IfFalse.Type));

			return !_forceClientSide && canBeEvaluated;
		}

		private bool CanBeEvaluatedInHql(UnaryExpression unaryExpression)
		{
			return CanBeEvaluatedInHql(unaryExpression.Operand) &&
				CanBitwiseOperationBeEvaluatedInHql(unaryExpression.NodeType, unaryExpression.Operand);
		}

		private bool CanBeEvaluatedInHql(BinaryExpression binaryExpression)
		{
			var canBeEvaluated = CanBeEvaluatedInHql(binaryExpression.Left) &
								 CanBeEvaluatedInHql(binaryExpression.Right);
			if (!canBeEvaluated || _forceClientSide)
			{
				return false;
			}

			// Subtract dates on the client side as the result varies when executed on the server side.
			// In Sql Server when using datetime2 subtract is not possible.
			// In Oracle a number is returned that represents the difference between the two in days.
			if ((binaryExpression.NodeType == ExpressionType.Subtract || binaryExpression.NodeType == ExpressionType.SubtractChecked) &&
			    ContainsAnyOfTypes(DateTypes, binaryExpression.Left, binaryExpression.Right))
			{
				return false;
			}

			if (!CanBitwiseOperationBeEvaluatedInHql(
				binaryExpression.NodeType,
				binaryExpression.Left,
				binaryExpression.Right))
			{
				return false;
			}

			if (!CanArithmeticOperationBeEvaluatedInHql(binaryExpression))
			{
				return false;
			}

			// Concatenation of strings can be only done on the server side when the left and right side types match.
			if (binaryExpression.NodeType == ExpressionType.Add &&
			    (binaryExpression.Left.Type == typeof(string) || binaryExpression.Right.Type == typeof(string)))
			{
				return binaryExpression.Left.Type == binaryExpression.Right.Type;
			}

			if (binaryExpression.NodeType == ExpressionType.Modulo)
			{
				var sqlFunction = _parameters.SessionFactory.SQLFunctionRegistry.FindSQLFunction("mod");
				if (sqlFunction == null || !(sqlFunction is ISQLFunctionExtended extendedSqlFunction))
				{
					return false; // Fallback to old behavior
				}

				var arguments = ExpressionsHelper.GetTypes(_parameters, binaryExpression.Left, binaryExpression.Right);
				return extendedSqlFunction.GetEffectiveReturnType(arguments, _parameters.SessionFactory, false) != null;
			}

			return true;
		}

		private bool CanBitwiseOperationBeEvaluatedInHql(ExpressionType expressionType, params Expression[] expressions)
		{
			if (!BitwiseOperations.Contains(expressionType) || !ContainsType(typeof(bool), expressions))
			{
				return true;
			}

			return _parameters.SessionFactory.Dialect.SupportsBitwiseOperatorsOnBoolean;
		}

		private bool CanArithmeticOperationBeEvaluatedInHql(BinaryExpression expression)
		{
			if (!ContainsType(typeof(decimal), expression.Left, expression.Right))
			{
				return true;
			}

			// Some databases (e.g. SQLite) stores decimals as a floating point number, which may cause incorrect results when using
			// any arithmetic operation.
			if (_parameters.SessionFactory.Dialect.IsDecimalStoredAsFloatingPointNumber && ArithmeticOperations.Contains(expression.NodeType))
			{
				return false;
			}

			// Divide and Multiply operator on decimals produce different results when executed on server side, due to different precisions.
			// In order to achieve the best precision possible, do the calculation on the client.
			if (expression.NodeType == ExpressionType.Divide ||
				expression.NodeType == ExpressionType.Multiply ||
				expression.NodeType == ExpressionType.MultiplyChecked)
			{
				return false;
			}

			return true;
		}

		private bool CanBeEvaluatedInHql(MemberInitExpression memberInitExpression)
		{
			CanBeEvaluatedInHql(memberInitExpression.NewExpression);
			VisitMemberBindings(memberInitExpression.Bindings);
			return false;
		}

		private bool CanBeEvaluatedInHql(DynamicExpression dynamicExpression)
		{
			foreach (var argument in dynamicExpression.Arguments)
			{
				CanBeEvaluatedInHql(argument);
			}

			return false;
		}

		private bool CanBeEvaluatedInHql(ListInitExpression listInitExpression)
		{
			CanBeEvaluatedInHql(listInitExpression.NewExpression);
			foreach (var initializer in listInitExpression.Initializers)
			{
				foreach (var listInitArgument in initializer.Arguments)
				{
					CanBeEvaluatedInHql(listInitArgument);
				}
			}

			return false;
		}
		private bool CanBeEvaluatedInHql(NewArrayExpression newArrayExpression)
		{
			foreach (var arrayExpression in newArrayExpression.Expressions)
			{
				CanBeEvaluatedInHql(arrayExpression);
			}

			return false;
		}

		private bool CanBeEvaluatedInHql(InvocationExpression invocationExpression)
		{
			foreach (var argument in invocationExpression.Arguments)
			{
				CanBeEvaluatedInHql(argument);
			}

			return false;
		}

		private bool CanBeEvaluatedInHql(NewExpression newExpression)
		{
			foreach (var argument in newExpression.Arguments)
			{
				CanBeEvaluatedInHql(argument);
			}

			return false;
		}

		private void VisitMemberBindings(IEnumerable<MemberBinding> bindings)
		{
			foreach (var binding in bindings)
			{
				switch (binding)
				{
					case MemberAssignment assignment:
						CanBeEvaluatedInHql(assignment.Expression);
						break;
					case MemberListBinding listBinding:
						foreach (var argument in listBinding.Initializers.SelectMany(o => o.Arguments))
						{
							CanBeEvaluatedInHql(argument);
						}

						break;
					case MemberMemberBinding memberBinding:
						VisitMemberBindings(memberBinding.Bindings);
						break;
				}
			}
		}

		private static bool ContainsAnyOfTypes(HashSet<System.Type> types, params Expression[] expressions)
		{
			return expressions.Any(o => types.Contains(o.Type.UnwrapIfNullable()));
		}

		private static bool ContainsType(System.Type type, params Expression[] expressions)
		{
			return expressions.Any(o => type == o.Type.UnwrapIfNullable());
		}
	}
}
