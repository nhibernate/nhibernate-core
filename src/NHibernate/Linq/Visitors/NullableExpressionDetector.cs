using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Linq.Clauses;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Functions;
using NHibernate.Util;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors
{
	internal class NullableExpressionDetector
	{
		private static readonly HashSet<System.Type> NotNullOperators = new HashSet<System.Type>
		{
			typeof(AllResultOperator),
			typeof(AnyResultOperator),
			typeof(ContainsResultOperator),
			typeof(CountResultOperator),
			typeof(LongCountResultOperator)
		};

		private readonly Dictionary<BinaryExpression, List<MemberExpression>> _equalityNotNullMembers =
			new Dictionary<BinaryExpression, List<MemberExpression>>();
		private readonly ISessionFactoryImplementor _sessionFactory;
		private readonly ILinqToHqlGeneratorsRegistry _functionRegistry;

		public NullableExpressionDetector(ISessionFactoryImplementor sessionFactory, ILinqToHqlGeneratorsRegistry functionRegistry)
		{
			_sessionFactory = sessionFactory;
			_functionRegistry = functionRegistry;
		}

		public void SearchForNotNullMemberChecks(BinaryExpression expression)
		{
			// Check for a member not null check that has a not equals expression
			// Example: o.Status != null && o.Status != "New"
			// Example: (o.Status != null && o.OldStatus != null) && (o.Status != o.OldStatus)
			// Example: (o.Status != null && o.OldStatus != null) && (o.Status == o.OldStatus)
			if (
				expression.NodeType != ExpressionType.AndAlso ||
				(
					expression.Right.NodeType != ExpressionType.NotEqual &&
					expression.Right.NodeType != ExpressionType.Equal
				) ||
				(
					expression.Left.NodeType != ExpressionType.AndAlso &&
					expression.Left.NodeType != ExpressionType.NotEqual
				))
			{
				return;
			}

			// Skip if there are no member access expressions on the right side
			var notEqualExpression = (BinaryExpression) expression.Right;
			if (!IsMemberAccess(notEqualExpression.Left) && !IsMemberAccess(notEqualExpression.Right))
			{
				return;
			}

			var notNullMembers = new List<MemberExpression>();
			// We may have multiple conditions
			// Example: o.Status != null && o.OldStatus != null
			if (expression.Left.NodeType == ExpressionType.AndAlso)
			{
				FindAllNotNullMembers((BinaryExpression) expression.Left, notNullMembers);
			}
			else
			{
				FindNotNullMember((BinaryExpression) expression.Left, notNullMembers);
			}

			if (notNullMembers.Count > 0)
			{
				_equalityNotNullMembers[notEqualExpression] = notNullMembers;
			}
		}

		public bool IsNullable(Expression expression, BinaryExpression equalityExpression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.TypeAs:
					// a cast will not return null if the operand is not null (as long as TypeAs is not translated to
					// try_convert in SQL).
					return IsNullable(((UnaryExpression) expression).Operand, equalityExpression);
				case ExpressionType.Not:
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
					return false;
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				case ExpressionType.Divide:
				case ExpressionType.Modulo:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
				case ExpressionType.Power:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
					var binaryExpression = (BinaryExpression) expression;
					return IsNullable(binaryExpression.Left, equalityExpression) || IsNullable(binaryExpression.Right, equalityExpression);
				case ExpressionType.ArrayIndex:
					return true; // for indexed lists we cannot determine whether the item will be null or not
				case ExpressionType.Coalesce:
					return IsNullable(((BinaryExpression) expression).Right, equalityExpression);
				case ExpressionType.Conditional:
					var conditionalExpression = (ConditionalExpression) expression;
					return IsNullable(conditionalExpression.IfTrue, equalityExpression) ||
						   IsNullable(conditionalExpression.IfFalse, equalityExpression);
				case ExpressionType.Call:
					var methodInfo = ((MethodCallExpression) expression).Method;
					return !_functionRegistry.TryGetGenerator(methodInfo, out var method) || method.AllowsNullableReturnType(methodInfo);
				case ExpressionType.MemberAccess:
					return IsNullable((MemberExpression) expression, equalityExpression);
				case ExpressionType.Extension:
					return IsNullableExtension(expression, equalityExpression);
				case ExpressionType.TypeIs: // an equal or in operator will be generated and those cannot return null
				case ExpressionType.NewArrayInit:
					return false;
				case ExpressionType.Constant:
					return VisitorUtil.IsNullConstant(expression);
				case ExpressionType.Parameter:
					return !expression.Type.IsValueType;
				default:
					return true;
			}
		}

		private bool IsNullable(MemberExpression memberExpression, BinaryExpression equalityExpression)
		{
			if (_functionRegistry.TryGetGenerator(memberExpression.Member, out _))
			{
				// We have to skip the property as it will be converted to a function that can return null
				// if the argument is null
				return IsNullable(memberExpression.Expression, equalityExpression);
			}

			var memberType = memberExpression.Member.GetPropertyOrFieldType();
			if (memberType?.IsValueType == true && !memberType.IsNullable())
			{
				return IsNullable(memberExpression.Expression, equalityExpression);
			}

			// Check if there was a not null check prior the equality expression
			if ((
				    equalityExpression.NodeType == ExpressionType.NotEqual ||
				    equalityExpression.NodeType == ExpressionType.Equal
			    ) &&
			    _equalityNotNullMembers.TryGetValue(equalityExpression, out var notNullMembers) &&
			    notNullMembers.Any(o => AreEqual(o, memberExpression)))
			{
				return false;
			}

			if (!ExpressionsHelper.TryGetMappedNullability(_sessionFactory, memberExpression, out var nullable) || nullable)
			{
				return true; // The expression contains one or many unsupported nodes or the member is nullable
			}

			return IsNullable(memberExpression.Expression, equalityExpression);
		}

		private bool IsNullableExtension(Expression extensionExpression, BinaryExpression equalityExpression)
		{
			switch (extensionExpression)
			{
				case QuerySourceReferenceExpression querySourceReferenceExpression:
					switch (querySourceReferenceExpression.ReferencedQuerySource)
					{
						case MainFromClause _:
							return false; // we reached to the root expression, there were no nullable expressions
						case NhJoinClause joinClause:
							return IsNullable(joinClause.FromExpression, equalityExpression);
						default:
							return true; // unknown query source
					}
				case SubQueryExpression subQueryExpression:
					if (subQueryExpression.QueryModel.SelectClause.Selector is NhAggregatedExpression subQueryAggregatedExpression)
					{
						return subQueryAggregatedExpression.AllowsNullableReturnType;
					}
					else if (subQueryExpression.QueryModel.ResultOperators.Any(o => NotNullOperators.Contains(o.GetType())))
					{
						return false;
					}

					return true;
				case NhAggregatedExpression aggregatedExpression:
					return aggregatedExpression.AllowsNullableReturnType;
				default:
					return true; // a query can return null and we cannot calculate it as it is not yet executed
			}
		}

		private static bool IsMemberAccess(Expression expression)
		{
			if (expression.NodeType == ExpressionType.MemberAccess)
			{
				return true;
			}

			// Nullable members can be wrapped in a convert expression
			return expression is UnaryExpression unaryExpression && unaryExpression.Operand.NodeType == ExpressionType.MemberAccess;
		}

		private static void FindAllNotNullMembers(BinaryExpression andAlsoExpression, List<MemberExpression> notNullMembers)
		{
			if (andAlsoExpression.Right.NodeType == ExpressionType.NotEqual)
			{
				FindNotNullMember((BinaryExpression) andAlsoExpression.Right, notNullMembers);
			}
			else if (andAlsoExpression.Right.NodeType == ExpressionType.AndAlso)
			{
				FindAllNotNullMembers((BinaryExpression) andAlsoExpression.Right, notNullMembers);
			}
			else
			{
				return;
			}

			if (andAlsoExpression.Left.NodeType == ExpressionType.NotEqual)
			{
				FindNotNullMember((BinaryExpression) andAlsoExpression.Left, notNullMembers);
			}
			else if (andAlsoExpression.Left.NodeType == ExpressionType.AndAlso)
			{
				FindAllNotNullMembers((BinaryExpression) andAlsoExpression.Left, notNullMembers);
			}
		}

		private static void FindNotNullMember(BinaryExpression notEqualExpression, List<MemberExpression> notNullMembers)
		{
			if (notEqualExpression.Left.NodeType == ExpressionType.MemberAccess && VisitorUtil.IsNullConstant(notEqualExpression.Right))
			{
				notNullMembers.Add((MemberExpression) notEqualExpression.Left);
			}
			else if (VisitorUtil.IsNullConstant(notEqualExpression.Left) && notEqualExpression.Right.NodeType == ExpressionType.MemberAccess)
			{
				notNullMembers.Add((MemberExpression) notEqualExpression.Right);
			}
		}

		private static bool AreEqual(MemberExpression memberExpression, MemberExpression otherMemberExpression)
		{
			if (memberExpression.Member != otherMemberExpression.Member ||
			    memberExpression.Expression.NodeType != otherMemberExpression.Expression.NodeType)
			{
				return false;
			}

			switch (memberExpression.Expression)
			{
				case QuerySourceReferenceExpression querySourceReferenceExpression:
					if (otherMemberExpression.Expression is QuerySourceReferenceExpression otherQuerySourceReferenceExpression)
					{
						return querySourceReferenceExpression.ReferencedQuerySource ==
						       otherQuerySourceReferenceExpression.ReferencedQuerySource;
					}

					return false;
				// Components have a nested member expression
				case MemberExpression nestedMemberExpression:
					if (otherMemberExpression.Expression is MemberExpression otherNestedMemberExpression)
					{
						return AreEqual(nestedMemberExpression, otherNestedMemberExpression);
					}

					return false;
				default:
					return memberExpression.Expression == otherMemberExpression.Expression;
			}
		}
	}
}
