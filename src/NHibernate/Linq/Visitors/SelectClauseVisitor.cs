using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Expressions;
using NHibernate.Util;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public class SelectClauseVisitor : RelinqExpressionVisitor
	{
		private static readonly MethodInfo UnwrapNullableDefinition = ReflectHelper.GetMethodDefinition(() => UnwrapNullable<int>(default, null));
		private static readonly HashSet<ExpressionType> ComparisonOperators = new HashSet<ExpressionType>
		{
			ExpressionType.Equal,
			ExpressionType.NotEqual,
			ExpressionType.GreaterThanOrEqual,
			ExpressionType.GreaterThan,
			ExpressionType.LessThan,
			ExpressionType.LessThanOrEqual
		};

		private readonly HqlTreeBuilder _hqlTreeBuilder = new HqlTreeBuilder();
		private HashSet<Expression> _hqlNodes;
		private readonly Dictionary<string, int>  _hqlNodeKeyIndexes = new Dictionary<string, int>();
		private readonly ParameterExpression _inputParameter;
		private readonly VisitorParameters _parameters;
		private int _iColumn;
		private List<HqlExpression> _hqlTreeNodes = new List<HqlExpression>();
		private readonly HqlGeneratorExpressionVisitor _hqlVisitor;

		/// <summary>
		/// Expressions for which we cannot alter their types to nullable:
		/// - Root expression
		/// - Arguments
		/// - Member assignments
		/// - Array elements
		/// - List elements
		/// - Test conditions
		/// </summary>
		private readonly HashSet<Expression> _requireTypeMatching = new HashSet<Expression>();

		public SelectClauseVisitor(System.Type inputType, VisitorParameters parameters)
		{
			_inputParameter = Expression.Parameter(inputType, "input");
			_parameters = parameters;
			_hqlVisitor = new HqlGeneratorExpressionVisitor(_parameters);
		}

		public LambdaExpression ProjectionExpression { get; private set; }

		public IEnumerable<HqlExpression> GetHqlNodes()
		{
			return _hqlTreeNodes;
		}

		public void VisitSelector(Expression expression)
		{
			var distinct = expression as NhDistinctExpression;
			if (distinct != null)
			{
				expression = distinct.Expression;
			}

			// Find the sub trees that can be expressed purely in HQL
			var nominator = new SelectClauseHqlNominator(_parameters);
			nominator.Nominate(expression);
			_hqlNodes = nominator.HqlCandidates;

			// Strip the nominator wrapper from the select expression
			expression = UnwrapExpression(expression);

			// Linq2SQL ignores calls to local methods. Linq2EF seems to not support
			// calls to local methods at all. For NHibernate we support local methods,
			// but prevent their use together with server-side distinct, since it may
			// end up being wrong.
			if (distinct != null && nominator.ContainsUntranslatedMethodCalls)
				throw new NotSupportedException("Cannot use distinct on result that depends on methods for which no SQL equivalent exist.");

			// Now visit the tree
			AddRequiredTypeMatchingExpression(expression);
			var projection = Visit(expression);
			if ((projection != expression && !_hqlNodes.Contains(expression)) || _hqlTreeNodes.Count == 0)
			{
				ProjectionExpression = Expression.Lambda(projection, _inputParameter);
			}

			// When having only constants in the select clause we need to add one column in order to have a valid sql statement
			if (_hqlTreeNodes.Count == 0)
			{
				_hqlTreeNodes.Add(_hqlVisitor.Visit(Expression.Constant(1)).AsExpression());
			}

			// Handle any boolean results in the output nodes
			_hqlTreeNodes = _hqlTreeNodes.ConvertAll(node => node.ToArithmeticExpression());

			if (distinct != null)
			{
				var treeNodes = new List<HqlTreeNode>(_hqlTreeNodes.Count + 1) {_hqlTreeBuilder.Distinct()};
				treeNodes.AddRange(_hqlTreeNodes);
				_hqlTreeNodes = new List<HqlExpression>(1) {_hqlTreeBuilder.ExpressionSubTreeHolder(treeNodes)};
			}
		}

		#region Overrides

		public override Expression Visit(Expression expression)
		{
			if (expression == null)
			{
				return null;
			}

			expression = UnwrapExpression(expression);
			if (_hqlNodes.Contains(expression))
			{
				// In order to avoid selecting the same expressions multiple times, calculate the expression key
				// and use the same column index for them.
				var key = ExpressionKeyVisitor.VisitChild(expression, _parameters.ConstantToParameterMap, _parameters.SessionFactory);
				if (!_hqlNodeKeyIndexes.TryGetValue(key, out var index))
				{
					index = _iColumn++;
					_hqlNodeKeyIndexes.Add(key, index);
					// Pure HQL evaluation
					_hqlTreeNodes.Add(_hqlVisitor.Visit(expression).AsExpression());
				}

				var input = Expression.ArrayIndex(_inputParameter, Expression.Constant(index));

				// When the value of the _inputParameter is a value type we need to make an additional null check
				// in order to prevent a NRE when trying to cast null to a value type.
				// (e.g. Select(o => (int?) o.ManyToOne.Id), where Id is a value type but can be null when ManyToOne is null)
				if (!expression.Type.IsNullableOrReference())
				{
					return UnwrapIfTypeMatchingRequired(
						CreateNullConditional(expression.Type, input, input.Type, arg => Convert(arg, expression.Type)),
						expression);
				}

				return Convert(input, expression.Type);
			}

			// Can't handle this node with HQL.  Just recurse down, and emit the expression
			return base.Visit(expression);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			var leftNode = Visit(node.Left);
			var rightNode = Visit(node.Right);

			if (leftNode.Type == node.Left.Type && rightNode.Type == node.Right.Type)
			{
				return node.Update(leftNode, node.Conversion, rightNode);
			}

			// Rules for ?.:
			// - All arithmetic operations return null when one or both sides are null
			// - Sides of a boolean logical operation return false in case they are null
			Expression test;
			Expression defaultValue;
			var leftVariable = Expression.Variable(leftNode.Type, "left");
			var rightVariable = Expression.Variable(rightNode.Type, "right");

			if (leftNode.Type.IsNullableOrReference() && rightNode.Type.IsNullableOrReference())
			{
				test = Expression.MakeBinary(ExpressionType.OrElse,
							Expression.Equal(leftVariable, Expression.Default(leftNode.Type)),
							Expression.Equal(rightVariable, Expression.Default(rightNode.Type))
						);
			}
			else if (leftNode.Type.IsNullableOrReference())
			{
				test = Expression.Equal(leftVariable, Expression.Default(leftNode.Type));
			}
			else if (rightNode.Type.IsNullableOrReference())
			{
				test = Expression.Equal(rightVariable, Expression.Default(rightNode.Type));
			}
			else
			{
				test = Expression.Constant(false);
			}

			// OrElse and AndAlso logical operators never return null even when one of the sides is null, so we have to use false for sides that are null.
			// As AndAlso require both side to be true we don't need to apply the operator when one of the sides is null.
			if (node.NodeType == ExpressionType.OrElse)
			{
				defaultValue = Expression.MakeBinary(
					node.NodeType,
					leftVariable.Type.IsNullable() 
						? Expression.Coalesce(leftVariable, Expression.Constant(false))
						: (Expression) leftVariable,
					rightVariable.Type.IsNullable()
						? Expression.Coalesce(rightVariable, Expression.Constant(false))
						: (Expression) rightVariable,
					node.IsLiftedToNull,
					node.Method
				);
			}
			// Comparison operators never return null when one of the sides is null
			else if (node.NodeType == ExpressionType.AndAlso ||  ComparisonOperators.Contains(node.NodeType))
			{
				defaultValue = Expression.Default(typeof(bool));
			}
			else
			{
				defaultValue = Expression.Default(node.Type.GetNullableType());
			}

			// var left = <leftNode>;
			// var right = <rightNode>;
			// return left == default || right == default ? default(<returnType>) : left <op> right;
			return UnwrapIfTypeMatchingRequired(
				Expression.Block(
					new[] { leftVariable, rightVariable },
					Expression.Assign(leftVariable, leftNode),
					Expression.Assign(rightVariable, rightNode),
					Expression.Condition(
						test,
						defaultValue,
						ConvertIfNeeded(
							Expression.MakeBinary(
								node.NodeType,
								ConvertIfNeeded(leftVariable, node.Left.Type),
								ConvertIfNeeded(rightVariable, node.Right.Type),
								node.IsLiftedToNull,
								node.Method
							),
							defaultValue.Type
						)
					)
				),
				node);
		}

		protected override Expression VisitConditional(ConditionalExpression node)
		{
			// Rules for ?.:
			// - Test expression return false in case the result is null
			var testNode = Visit(node.Test);
			var ifTrueNode = Visit(node.IfTrue);
			var ifFalseNode = Visit(node.IfFalse);

			// Test expressions in sql never return null as case with zero and one is used. In order
			// to simulate it on the client we have to add a coalesce expression when the test
			// expression is null and return false instead. (e.g. Select(o => o.ManyToOne.Bool ? o.Prop1 : o.Prop2),
			// where ManyToOne can be null)
			if (testNode.Type.IsNullable())
			{
				testNode = Expression.Coalesce(testNode, Expression.Constant(false));
			}

			if (ifTrueNode.Type == node.Type && ifFalseNode.Type == node.Type)
			{
				return node.Update(testNode, ifTrueNode, ifFalseNode);
			}

			if (node.Type == ifFalseNode.Type)
			{
				ifFalseNode = Expression.Convert(ifFalseNode, ifTrueNode.Type);
			}
			else if (node.Type == ifTrueNode.Type)
			{
				ifTrueNode = Expression.Convert(ifTrueNode, ifFalseNode.Type);
			}

			return UnwrapIfTypeMatchingRequired(Expression.Condition(testNode, ifTrueNode, ifFalseNode), node);
		}

		protected override Expression VisitDynamic(DynamicExpression node)
		{
			return node.Update(VisitArguments(node) ?? (IEnumerable<Expression>) node.Arguments);
		}

		protected override Expression VisitInvocation(InvocationExpression node)
		{
			var args = VisitArguments(node) ?? (IEnumerable<Expression>) node.Arguments;
			return node.Update(node.Expression, args);
		}

		// Override the original implementation to visit arguments first
		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			var args = VisitArguments(node) ?? (IEnumerable<Expression>) node.Arguments;
			if (node.Object == null) // Static method
			{
				return node.Update(node.Object, args);
			}

			var obj = Visit(node.Object);
			if (!obj.Type.IsNullableOrReference())
			{
				return node.Update(obj, args);
			}

			return UnwrapIfTypeMatchingRequired(
				CreateNullConditional(node.Type, obj, node.Object.Type, arg => Expression.Call(arg, node.Method, args)),
				node);
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			var expression = Visit(node.Expression);
			if (expression == null)
			{
				return node;
			}

			if (!expression.Type.IsNullableOrReference())
			{
				return node.Update(expression);
			}

			return UnwrapIfTypeMatchingRequired(
				CreateNullConditional(node.Type, expression, node.Expression.Type, arg => Expression.MakeMemberAccess(arg, node.Member)),
				node);
		}

		protected override Expression VisitNewArray(NewArrayExpression node)
		{
			return node.Update(VisitExpressions(node.Expressions) ?? (IEnumerable<Expression>) node.Expressions);
		}

		protected override Expression VisitNew(NewExpression node)
		{
			return node.Update(VisitExpressions(node.Arguments) ?? (IEnumerable<Expression>) node.Arguments);
		}

		protected override Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			return node.Update(Visit(node.Expression));
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			// Rules for ?.: 
			// - When the operand is null then the result of the unary operator is also null (except for Not)
			// - When Not operand is null then the result is false
			var operand = Visit(node.Operand);
			if (node.NodeType == ExpressionType.Convert || node.NodeType == ExpressionType.ConvertChecked)
			{
				if (operand.Type == node.Type)
				{
					return operand; // Cast was already done
				}

				if (operand.Type.IsNullableOrReference())
				{
					return node.Update(operand); // A NRE will never occur for casting to reference types
				}
			}
			// Not is transformed into case with zero and one, so when the expression is null we have to return false
			// to match server evaluation. In order to return false on null, use true for the Not operator.
			else if (node.NodeType == ExpressionType.Not && operand.Type.IsNullable())
			{
				operand = Expression.Coalesce(operand, Expression.Constant(true));
			}

			if (!operand.Type.IsNullableOrReference() || node.NodeType == ExpressionType.TypeAs)
			{
				return node.Update(operand);
			}

			return UnwrapIfTypeMatchingRequired(
				CreateNullConditional(node.Type, operand, node.Operand.Type, arg => Expression.MakeUnary(node.NodeType, arg, node.Type, node.Method)),
				node);
		}

		protected override Expression VisitMemberInit(MemberInitExpression node)
		{
			return node.Update(VisitAndConvert(node.NewExpression, nameof(VisitMemberInit)), Visit(node.Bindings, VisitMemberBinding));
		}

		protected override Expression VisitListInit(ListInitExpression node)
		{
			return node.Update(VisitAndConvert(node.NewExpression, nameof(VisitListInit)), Visit(node.Initializers, VisitElementInit));
		}

		protected override ElementInit VisitElementInit(ElementInit node)
		{
			return node.Update(VisitExpressions(node.Arguments) ?? (IEnumerable<Expression>) node.Arguments);
		}

		protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
		{
			AddRequiredTypeMatchingExpression(node.Expression);
			return node.Update(Visit(node.Expression));
		}

		#endregion

		private Expression[] VisitExpressions(ReadOnlyCollection<Expression> nodes)
		{
			Expression[] newNodes = null;
			for (int i = 0, n = nodes.Count; i < n; i++)
			{
				var curNode = nodes[i];
				AddRequiredTypeMatchingExpression(curNode);
				var node = Visit(curNode);
				if (newNodes != null)
				{
					newNodes[i] = node;
				}
				else if (!ReferenceEquals(node, curNode))
				{
					newNodes = new Expression[n];
					for (var j = 0; j < i; j++)
					{
						newNodes[j] = nodes[j];
					}

					newNodes[i] = node;
				}
			}

			return newNodes;
		}

		private Expression[] VisitArguments(IArgumentProvider nodes)
		{
			Expression[] newNodes = null;
			for (int i = 0, n = nodes.ArgumentCount; i < n; i++)
			{
				var curNode = nodes.GetArgument(i);
				AddRequiredTypeMatchingExpression(curNode);
				var node = Visit(curNode);
				if (newNodes != null)
				{
					newNodes[i] = node;
				}
				else if (!ReferenceEquals(node, curNode))
				{
					newNodes = new Expression[n];
					for (var j = 0; j < i; j++)
					{
						newNodes[j] = nodes.GetArgument(j);
					}

					newNodes[i] = node;
				}
			}

			return newNodes;
		}

		private void AddRequiredTypeMatchingExpression(Expression expression)
		{
			_requireTypeMatching.Add(UnwrapExpression(expression));
		}

		private Expression UnwrapExpression(Expression expression)
		{
			if (expression is NhNominatedExpression nominatedExpression)
			{
				return nominatedExpression.Expression;
			}

			if (expression is MethodCallExpression methodExpression &&
				VisitorUtil.TryGetEvalExpression(methodExpression, out var evalExpression))
			{
				return evalExpression;
			}

			return expression;
		}

		private static Expression ConvertIfNeeded(Expression node, System.Type type)
		{
			return node.Type == type
				? node
				: Expression.Convert(node, type);
		}

		private Expression UnwrapIfTypeMatchingRequired(Expression expression, Expression node)
		{
			return _requireTypeMatching.Contains(node) && expression.Type.IsNullable()
				? CallUnwrapNullable(expression, node)
				: expression;
		}

		/// <summary>
		/// Adds a null check (simulates ?. operator) for client side evaluations in order to prevent NRE and makes it consistent with the server
		/// side evaluation.
		/// </summary>
		/// <param name="nodeType">The root expression.</param>
		/// <param name="operand">The operand to check for <see langword="null"/> value.</param>
		/// <param name="originalOperandType">The non transformed operand to match the type.</param>
		/// <param name="getOperationFunc">The expression that will be used when the <paramref name="operand"/> value is not null.</param>
		/// <returns>The transformed expression.</returns>
		private static BlockExpression CreateNullConditional(System.Type nodeType, Expression operand, System.Type originalOperandType, Func<Expression, Expression> getOperationFunc)
		{
			// Simulate ?. operator by using an ExpressionBlock
			// var value = <operand>;
			// return value == default(<operand.Type>) ? default(<returnType>) : operation(value)
			var valueVariable = Expression.Variable(operand.Type, "value");
			var returnType = nodeType.GetNullableType();
			return Expression.Block(
				new[] { valueVariable },
				Expression.Assign(valueVariable, operand), // Assign to a variable to avoid evaluating the operand multiple times
				Expression.Condition(
					Expression.Equal(valueVariable, Expression.Default(operand.Type)),
					Expression.Default(returnType),
					ConvertIfNeeded(
						getOperationFunc(ConvertIfNeeded(valueVariable, originalOperandType)), // Cast will be done only for nullable types
						returnType
					)
				)
			);
		}

		private static readonly MethodInfo ConvertChangeType =
			ReflectHelper.FastGetMethod(System.Convert.ChangeType, default(object), default(System.Type));

		private static Expression Convert(Expression expression, System.Type type)
		{
			//#1121
			if (type.IsEnum)
			{
				expression = Expression.Call(
					ConvertChangeType,
					expression,
					Expression.Constant(Enum.GetUnderlyingType(type)));
			}

			return Expression.Convert(expression, type);
		}

		private static MethodCallExpression CallUnwrapNullable(Expression valueExpression, Expression node)
		{
			return Expression.Call(UnwrapNullableDefinition.MakeGenericMethod(node.Type),
						valueExpression,
						Expression.Constant(node));
		}

		private static TValueType UnwrapNullable<TValueType>(TValueType? value, Expression node) where TValueType : struct
		{
			if (!value.HasValue)
			{
				throw new InvalidOperationException(
					$"Null value cannot be assigned to a value type '{typeof(TValueType)}'. Cast expression '{node}' to '{typeof(TValueType?)}'.");
			}

			return value.Value;
		}
	}

	// Since v5
	[Obsolete]
	public static class BooleanToCaseConvertor
	{
		[Obsolete]
		public static IEnumerable<HqlExpression> Convert(IEnumerable<HqlExpression> hqlTreeNodes)
		{
			return hqlTreeNodes.Select(node => node.ToArithmeticExpression());
		}

		[Obsolete]
		public static HqlExpression ConvertBooleanToCase(HqlExpression node)
		{
			return node.ToArithmeticExpression();
		}
	}
}
