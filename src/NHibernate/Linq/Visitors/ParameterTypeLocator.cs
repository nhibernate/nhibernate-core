using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Type;
using NHibernate.Util;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Locates parameter actual type based on its usage.
	/// </summary>
	public static class ParameterTypeLocator
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
		/// Set query parameter types based on the given query model.
		/// </summary>
		/// <param name="parameters">The query parameters.</param>
		/// <param name="queryModel">The query model.</param>
		/// <param name="targetType">The target entity type.</param>
		/// <param name="sessionFactory">The session factory.</param>
		public static void SetParameterTypes(
			IDictionary<ConstantExpression, NamedParameter> parameters,
			QueryModel queryModel,
			System.Type targetType,
			ISessionFactoryImplementor sessionFactory)
		{
			SetParameterTypes(parameters, queryModel, targetType, sessionFactory, false);
		}

		internal static void SetParameterTypes(
			IDictionary<ConstantExpression, NamedParameter> parameters,
			QueryModel queryModel,
			System.Type targetType,
			ISessionFactoryImplementor sessionFactory,
			bool removeMappedAsCalls)
		{
			if (parameters.Count == 0)
			{
				return;
			}

			var visitor = new ConstantTypeLocatorVisitor(removeMappedAsCalls, targetType, parameters, sessionFactory);
			queryModel.TransformExpressions(visitor.Visit);

			foreach (var pair in visitor.ConstantExpressions)
			{
				var type = pair.Value;
				var constantExpression = pair.Key;
				if (!parameters.TryGetValue(constantExpression, out var namedParameter))
				{
					continue;
				}

				if (type != null)
				{
					// MappedAs was used
					namedParameter.Type = type;
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
						? ParameterHelper.TryGuessType(constantExpression.Value, sessionFactory, namedParameter.IsCollection)
						: ParameterHelper.TryGuessType(constantExpression.Type, sessionFactory, namedParameter.IsCollection);
				}

				namedParameter.Type = type;
			}
		}

		private class ConstantTypeLocatorVisitor : RelinqExpressionVisitor
		{
			private readonly bool _removeMappedAsCalls;
			private readonly System.Type _targetType;
			private readonly IDictionary<ConstantExpression, NamedParameter> _parameters;
			private readonly ISessionFactoryImplementor _sessionFactory;
			public readonly Dictionary<ConstantExpression, IType> ConstantExpressions =
				new Dictionary<ConstantExpression, IType>();
			public readonly Dictionary<Expression, HashSet<Expression>> RelatedExpressions =
				new Dictionary<Expression, HashSet<Expression>>();

			public ConstantTypeLocatorVisitor(
				bool removeMappedAsCalls,
				System.Type targetType,
				IDictionary<ConstantExpression, NamedParameter> parameters,
				ISessionFactoryImplementor sessionFactory)
			{
				_removeMappedAsCalls = removeMappedAsCalls;
				_targetType = targetType;
				_sessionFactory = sessionFactory;
				_parameters = parameters;
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
					AddRelatedExpression(node, left, right);
					AddRelatedExpression(node, right, left);
				}

				return node;
			}

			protected override Expression VisitConditional(ConditionalExpression node)
			{
				node = (ConditionalExpression) base.VisitConditional(node);
				var ifTrue = Unwrap(node.IfTrue);
				var ifFalse = Unwrap(node.IfFalse);
				AddRelatedExpression(node, ifTrue, ifFalse);
				AddRelatedExpression(node, ifFalse, ifTrue);

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
				if (node.Value is IEntityNameProvider || RelatedExpressions.ContainsKey(node) || !_parameters.ContainsKey(node))
				{
					return node;
				}

				RelatedExpressions.Add(node, new HashSet<Expression>());
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

			private void AddRelatedExpression(Expression node, Expression left, Expression right)
			{
				if (left.NodeType == ExpressionType.MemberAccess ||
					IsDynamicMember(left) ||
					left is QuerySourceReferenceExpression)
				{
					AddRelatedExpression(right, left);
					if (NonVoidOperators.Contains(node.NodeType))
					{
						AddRelatedExpression(node, left);
					}
				}

				// Copy all found MemberExpressions to the other side
				// (e.g. (o.Prop ?? constant1) == constant2 -> copy o.Prop to constant2)
				if (RelatedExpressions.TryGetValue(left, out var set))
				{
					foreach (var nestedMemberExpression in set)
					{
						AddRelatedExpression(right, nestedMemberExpression);
						if (NonVoidOperators.Contains(node.NodeType))
						{
							AddRelatedExpression(node, nestedMemberExpression);
						}
					}
				}
			}

			private void AddRelatedExpression(Expression expression, Expression relatedExpression)
			{
				if (!RelatedExpressions.TryGetValue(expression, out var set))
				{
					set = new HashSet<Expression>();
					RelatedExpressions.Add(expression, set);
				}

				set.Add(relatedExpression);
			}

			private bool IsDynamicMember(Expression expression)
			{
				switch (expression)
				{
#if NETCOREAPP2_0
					case InvocationExpression invocationExpression:
						// session.Query<Product>().Where("Properties.Name == @0", "First Product")
						return ExpressionsHelper.TryGetDynamicMemberBinder(invocationExpression, out _);
#endif
					case DynamicExpression dynamicExpression:
						return dynamicExpression.Binder is GetMemberBinder;
					case MethodCallExpression methodCallExpression:
						// session.Query<Product>() where p.Properties["Name"] == "First Product" select p
						return VisitorUtil.TryGetPotentialDynamicComponentDictionaryMember(methodCallExpression, out _);
					default:
						return false;
				}
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
