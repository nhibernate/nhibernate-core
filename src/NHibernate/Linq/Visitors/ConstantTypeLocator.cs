using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Locates <see cref="ConstantExpression"/> actual type based on its usage.
	/// </summary>
	public static class ConstantTypeLocator
	{
		/// <summary>
		/// List of <see cref="ExpressionType"/> for which the <see cref="MemberExpression"/> should be related to the other side
		/// of a <see cref="BinaryExpression"/> (e.g. o.MyEnum == MyEnum.Option -> MyEnum.Option should have o.MyEnum as a related
		/// <see cref="MemberExpression"/>).
		/// </summary>
		private static readonly HashSet<ExpressionType> ValidBinaryExpressionTypes = new HashSet<ExpressionType>
		{
			ExpressionType.Equal,
			ExpressionType.NotEqual,
			ExpressionType.GreaterThanOrEqual,
			ExpressionType.GreaterThan,
			ExpressionType.LessThan,
			ExpressionType.LessThanOrEqual,
			ExpressionType.Coalesce,
			ExpressionType.Assign
		};

		/// <summary>
		/// List of <see cref="ExpressionType"/> for which the <see cref="MemberExpression"/> should be copied across
		/// as related (e.g. (o.MyEnum ?? MyEnum.Option) == MyEnum.Option2 -> MyEnum.Option2 should have o.MyEnum as a related
		/// <see cref="MemberExpression"/>).
		/// </summary>
		private static readonly HashSet<ExpressionType> NonVoidOperators = new HashSet<ExpressionType>
		{
			ExpressionType.Coalesce,
			ExpressionType.Conditional
		};

		/// <summary>
		/// Calculate constant expressions types inside the query model.
		/// </summary>
		/// <param name="queryModel">The query model.</param>
		/// <param name="targetType">The target entity type.</param>
		/// <param name="sessionFactory">The session factory.</param>
		/// <returns></returns>
		public static Dictionary<ConstantExpression, IType> GetTypes(
			QueryModel queryModel,
			System.Type targetType,
			ISessionFactoryImplementor sessionFactory)
		{
			return GetTypes(queryModel, targetType, sessionFactory, false);
		}

		internal static Dictionary<ConstantExpression, IType> GetTypes(
			QueryModel queryModel,
			System.Type targetType,
			ISessionFactoryImplementor sessionFactory,
			bool removeMappedAsCalls)
		{
			var types = new Dictionary<ConstantExpression, IType>();
			var visitor = new ConstantTypeLocatorVisitor(removeMappedAsCalls, targetType, sessionFactory);
			queryModel.TransformExpressions(visitor.Visit);

			foreach (var pair in visitor.ConstantExpressions)
			{
				var type = pair.Value;
				var constantExpression = pair.Key;
				if (type != null)
				{
					// MappedAs was used
					types.Add(constantExpression, type);
					continue;
				}

				// In order to get the actual type we have to check first the related member expressions, as
				// an enum is translated in a numeric type when used in a BinaryExpression and also it can be mapped as string.
				// By getting the type from a related member expression we also get the correct length in case of StringType
				// or precision when having a DecimalType.
				if (visitor.RelatedExpressions.TryGetValue(constantExpression, out var memberExpressions))
				{
					foreach (var memberExpression in memberExpressions)
					{
						if (ExpressionsHelper.TryGetMappedType(
							sessionFactory,
							memberExpression,
							out type,
							out _,
							out _,
							out _))
						{
							break;
						}
					}
				}

				// No related MemberExpressions was found, guess the type by value or its type when null.
				if (type == null)
				{
					type = constantExpression.Value != null
						? ParameterHelper.TryGuessType(constantExpression.Value, sessionFactory, out _)
						: ParameterHelper.TryGuessType(constantExpression.Type, sessionFactory, out _);
				}

				types.Add(constantExpression, type);
			}

			return types;
		}

		private class ConstantTypeLocatorVisitor : RelinqExpressionVisitor
		{
			private readonly bool _removeMappedAsCalls;
			private readonly System.Type _targetType;
			private readonly ISessionFactoryImplementor _sessionFactory;
			public readonly Dictionary<ConstantExpression, IType> ConstantExpressions =
				new Dictionary<ConstantExpression, IType>();
			public readonly Dictionary<Expression, HashSet<MemberExpression>> RelatedExpressions =
				new Dictionary<Expression, HashSet<MemberExpression>>();

			public ConstantTypeLocatorVisitor(
				bool removeMappedAsCalls,
				System.Type targetType,
				ISessionFactoryImplementor sessionFactory)
			{
				_removeMappedAsCalls = removeMappedAsCalls;
				_targetType = targetType;
				_sessionFactory = sessionFactory;
			}

			protected override Expression VisitBinary(BinaryExpression node)
			{
				node = (BinaryExpression) base.VisitBinary(node);
				if (!ValidBinaryExpressionTypes.Contains(node.NodeType))
				{
					return node;
				}

				var left = Unwrap(node.Left);
				var right = Unwrap(node.Right);
				if (node.NodeType == ExpressionType.Assign)
				{
					VisitAssign(left, right);
				}
				else
				{
					AddRelatedMemberExpression(node, left, right);
					AddRelatedMemberExpression(node, right, left);
				}

				return node;
			}

			protected override Expression VisitConditional(ConditionalExpression node)
			{
				node = (ConditionalExpression) base.VisitConditional(node);
				var ifTrue = Unwrap(node.IfTrue);
				var ifFalse = Unwrap(node.IfFalse);
				AddRelatedMemberExpression(node, ifTrue, ifFalse);
				AddRelatedMemberExpression(node, ifFalse, ifTrue);

				return node;
			}

			protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
			{
				node = base.VisitMemberAssignment(node);
				return node;
			}

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (VisitorUtil.IsMappedAs(node.Method))
				{
					var rawParameter = Visit(node.Arguments[0]);
					var parameter = rawParameter as ConstantExpression;
					var type = node.Arguments[1] as ConstantExpression;
					if (parameter == null)
						throw new HibernateException(
							$"{nameof(LinqExtensionMethods.MappedAs)} must be called on an expression which can be evaluated as " +
							$"{nameof(ConstantExpression)}. It was call on {rawParameter?.GetType().Name ?? "null"} instead.");
					if (type == null)
						throw new HibernateException(
							$"{nameof(LinqExtensionMethods.MappedAs)} type must be supplied as {nameof(ConstantExpression)}. " +
							$"It was {node.Arguments[1]?.GetType().Name ?? "null"} instead.");

					ConstantExpressions[parameter] = (IType) type.Value;

					return _removeMappedAsCalls
						? rawParameter
						: node;
				}

				return base.VisitMethodCall(node);
			}

			protected override Expression VisitConstant(ConstantExpression node)
			{
				if (node.Value is IEntityNameProvider || RelatedExpressions.ContainsKey(node))
				{
					return node;
				}

				RelatedExpressions.Add(node, new HashSet<MemberExpression>());
				ConstantExpressions.Add(node, null);
				return node;
			}

			public override Expression Visit(Expression node)
			{
				if (node is SubQueryExpression subQueryExpression)
				{
					subQueryExpression.QueryModel.TransformExpressions(Visit);
				}

				return base.Visit(node);
			}

			private void VisitAssign(Expression leftNode, Expression rightNode)
			{
				// Insert and Update statements have assign expressions, where the left side is a parameter and its name
				// represents the property path to be assigned
				if (!(leftNode is ParameterExpression parameterExpression) ||
				    !(rightNode is ConstantExpression constantExpression))
				{
					return;
				}

				var entityName = _sessionFactory.TryGetGuessEntityName(_targetType);
				if (entityName == null)
				{
					return;
				}

				var persister = _sessionFactory.GetEntityPersister(entityName);
				ConstantExpressions[constantExpression] = persister.EntityMetamodel.GetPropertyType(parameterExpression.Name);
			}

			private void AddRelatedMemberExpression(Expression node, Expression left, Expression right)
			{
				HashSet<MemberExpression> set;
				if (left is MemberExpression leftMemberExpression)
				{
					AddMemberExpression(right, leftMemberExpression);
					if (NonVoidOperators.Contains(node.NodeType))
					{
						AddMemberExpression(node, leftMemberExpression);
					}
				}

				// Copy all found MemberExpressions to the other side
				// (e.g. (o.Prop ?? constant1) == constant2 -> copy o.Prop to constant2)
				if (RelatedExpressions.TryGetValue(left, out set))
				{
					foreach (var nestedMemberExpression in set)
					{
						AddMemberExpression(right, nestedMemberExpression);
						if (NonVoidOperators.Contains(node.NodeType))
						{
							AddMemberExpression(node, nestedMemberExpression);
						}
					}
				}
			}

			private void AddMemberExpression(Expression expression, MemberExpression memberExpression)
			{
				if (!RelatedExpressions.TryGetValue(expression, out var set))
				{
					set = new HashSet<MemberExpression>();
					RelatedExpressions.Add(expression, set);
				}

				set.Add(memberExpression);
			}

			private static Expression Unwrap(Expression expression)
			{
				if (expression is UnaryExpression unaryExpression)
				{
					return unaryExpression.Operand;
				}

				return expression;
			}
		}
	}
}
