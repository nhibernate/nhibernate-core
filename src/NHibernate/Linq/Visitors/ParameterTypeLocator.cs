using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Linq.Functions;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.Type;
using NHibernate.Util;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing;
using TypeExtensions = NHibernate.Util.TypeExtensions;

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

			foreach (var pair in visitor.ParameterConstants)
			{
				var namedParameter = pair.Key;
				var constantExpressions = pair.Value;
				// In case any of the constants has the type set, use it (e.g. MappedAs)
				namedParameter.Type = constantExpressions.Select(o => visitor.ConstantExpressions[o]).FirstOrDefault(o => o != null);
				if (namedParameter.Type != null)
				{
					continue;
				}

				namedParameter.Type = GetParameterType(sessionFactory, constantExpressions, visitor, namedParameter, out var tryProcessInHql);
				namedParameter.IsGuessedType = tryProcessInHql;
			}
		}

		private static IType GetCandidateType(
			ISessionFactoryImplementor sessionFactory,
			IEnumerable<ConstantExpression> constantExpressions,
			ConstantTypeLocatorVisitor visitor,
			System.Type constantType)
		{
			IType candidateType = null;
			foreach (var expression in constantExpressions)
			{
				// In order to get the actual type we have to check first the related member expressions, as
				// an enum is translated in a numeric type when used in a BinaryExpression and also it can be mapped as string.
				// By getting the type from a related member expression we also get the correct length in case of StringType
				// or precision when having a DecimalType.
				if (!visitor.RelatedExpressions.TryGetValue(expression, out var relatedExpressions))
					continue;
				foreach (var relatedExpression in relatedExpressions)
				{
					if (!ExpressionsHelper.TryGetMappedType(sessionFactory, relatedExpression, out var mappedType, out _, out _, out _))
						continue;

					if (mappedType.IsCollectionType)
					{
						var collection = (IQueryableCollection) ((IAssociationType) mappedType).GetAssociatedJoinable(sessionFactory);
						mappedType = collection.ElementType;
					}

					if (candidateType == null)
						candidateType = mappedType;
					else if (!candidateType.Equals(mappedType))
						return null;
				}
			}

			if (candidateType == null)
				return null;

			// When comparing an integral column with a real parameter, the parameter type must remain real type
			// and the column needs to be casted in order to prevent invalid results (e.g. Where(o => o.Integer >= 2.2d)).
			if (constantType.IsRealNumberType() && candidateType.ReturnedClass.IsIntegralNumberType())
				return null;

			return candidateType;
		}

		private static IType GetParameterType(
			ISessionFactoryImplementor sessionFactory,
			HashSet<ConstantExpression> constantExpressions,
			ConstantTypeLocatorVisitor visitor,
			NamedParameter namedParameter,
			out bool tryProcessInHql)
		{
			tryProcessInHql = false;
			// All constant expressions have the same type/value
			var constantExpression = constantExpressions.First();
			var constantType = constantExpression.Type.UnwrapIfNullable();
			var candidateType = GetCandidateType(sessionFactory, constantExpressions, visitor, constantType);
			if (candidateType != null)
			{
				return candidateType;
			}

			// Leave hql logic to determine the type except when the value is a char. Hql logic detects a char as a string, which causes an exception
			// when trying to set a string db parameter with a char value.
			tryProcessInHql = !(constantExpression.Value is char);
			// No related MemberExpressions was found, guess the type by value or its type when null.
			// When a numeric parameter is compared to different columns with different types (e.g. Where(o => o.Single >= singleParam || o.Double <= singleParam))
			// do not change the parameter type, but instead cast the parameter when comparing with different column types.
			return constantExpression.Value != null
				? ParameterHelper.TryGuessType(constantExpression.Value, sessionFactory, namedParameter.IsCollection)
				: ParameterHelper.TryGuessType(constantType, sessionFactory, namedParameter.IsCollection);
		}

		private class ConstantTypeLocatorVisitor : RelinqExpressionVisitor
		{
			private readonly bool _removeMappedAsCalls;
			private readonly System.Type _targetType;
			private readonly IDictionary<ConstantExpression, NamedParameter> _parameters;
			private readonly ISessionFactoryImplementor _sessionFactory;
			public readonly Dictionary<ConstantExpression, IType> ConstantExpressions =
				new Dictionary<ConstantExpression, IType>();
			public readonly Dictionary<NamedParameter, HashSet<ConstantExpression>> ParameterConstants =
				new Dictionary<NamedParameter, HashSet<ConstantExpression>>();
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

				var left = UnwrapUnary(node.Left);
				var right = UnwrapUnary(node.Right);
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
				var ifTrue = UnwrapUnary(node.IfTrue);
				var ifFalse = UnwrapUnary(node.IfFalse);
				AddRelatedExpression(node, ifTrue, ifFalse);
				AddRelatedExpression(node, ifFalse, ifTrue);

				return node;
			}

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (VisitorUtil.IsMappedAs(node.Method))
				{
					var rawParameter = Visit(node.Arguments[0]);
					var parameter = UnwrapUnary(rawParameter) as ConstantExpression;
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

				if (EqualsGenerator.Methods.Contains(node.Method) || CompareGenerator.IsCompareMethod(node.Method))
				{
					node = (MethodCallExpression) base.VisitMethodCall(node);
					var left = UnwrapUnary(node.Method.IsStatic ? node.Arguments[0] : node.Object);
					var right = UnwrapUnary(node.Method.IsStatic ? node.Arguments[1] : node.Arguments[0]);
					AddRelatedExpression(node, left, right);
					AddRelatedExpression(node, right, left);

					return node;
				}

				return base.VisitMethodCall(node);
			}

			protected override Expression VisitConstant(ConstantExpression node)
			{
				if (node.Value is IEntityNameProvider || RelatedExpressions.ContainsKey(node) || !_parameters.TryGetValue(node, out var param))
				{
					return node;
				}

				RelatedExpressions.Add(node, new HashSet<Expression>());
				ConstantExpressions.Add(node, null);
				if (!ParameterConstants.TryGetValue(param, out var set))
				{
					set = new HashSet<ConstantExpression>();
					ParameterConstants.Add(param, set);
				}

				set.Add(node);

				return node;
			}

			protected override Expression VisitSubQuery(SubQueryExpression node)
			{
				if (!TryLinkContainsMethod(node.QueryModel))
				{
					node.QueryModel.TransformExpressions(Visit);
				}

				return node;
			}

			private bool TryLinkContainsMethod(QueryModel queryModel)
			{
				// ReLinq wraps all ResultOperatorExpressionNodeBase into a SubQueryExpression. In case of
				// ContainsResultOperator where the constant expression is dislocated from the related expression,
				// we have to manually link the related expressions.
				if (queryModel.ResultOperators.Count != 1 ||
					!(queryModel.ResultOperators[0] is ContainsResultOperator containsOperator) ||
					!(queryModel.SelectClause.Selector is QuerySourceReferenceExpression querySourceReference) ||
					!(querySourceReference.ReferencedQuerySource is MainFromClause mainFromClause))
				{
					return false;
				}

				var left = UnwrapUnary(Visit(mainFromClause.FromExpression));
				var right = UnwrapUnary(Visit(containsOperator.Item));
				// The constant is on the left side (e.g. db.Users.Where(o => users.Contains(o)))
				// The constant is on the right side (e.g. db.Customers.Where(o => o.Orders.Contains(item)))
				if (left.NodeType != ExpressionType.Constant && right.NodeType != ExpressionType.Constant)
				{
					return false;
				}

				// Copy all found MemberExpressions to the constant expression
				// (e.g. values.Contains(o.Name != o.Name2 ? o.Enum1 : o.Enum2) -> copy o.Enum1 and o.Enum2)
				AddRelatedExpression(null, left, right);
				AddRelatedExpression(null, right, left);

				return true;
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
					left.NodeType == ExpressionType.ArrayIndex || // e.g. group.Key[0] == variable
					IsDynamicMember(left) ||
					left is QuerySourceReferenceExpression)
				{
					AddRelatedExpression(right, left);
					if (node != null && NonVoidOperators.Contains(node.NodeType))
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
						if (node != null && NonVoidOperators.Contains(node.NodeType))
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
#if NETCOREAPP2_0_OR_GREATER
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
		}

		/// <summary>
		/// Unwraps <see cref="System.Linq.Expressions.UnaryExpression"/>.
		/// </summary>
		/// <param name="expression">The expression to unwrap.</param>
		/// <returns>The unwrapped expression.</returns>
		internal static Expression UnwrapUnary(Expression expression)
		{
			while (expression is UnaryExpression unaryExpression)
			{
				expression = unaryExpression.Operand;
			}

			return expression;
		}
	}
}
