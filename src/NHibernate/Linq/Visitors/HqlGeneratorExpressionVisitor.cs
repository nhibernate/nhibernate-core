using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Functions;
using NHibernate.Param;
using NHibernate.Util;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.Visitors
{
	public class HqlGeneratorExpressionVisitor : IHqlExpressionVisitor
	{
		private readonly HqlTreeBuilder _hqlTreeBuilder = new HqlTreeBuilder();
		private readonly VisitorParameters _parameters;
		private readonly ILinqToHqlGeneratorsRegistry _functionRegistry;

		public static HqlTreeNode Visit(Expression expression, VisitorParameters parameters)
			=> new HqlGeneratorExpressionVisitor(parameters).Visit(expression);

		public HqlGeneratorExpressionVisitor(VisitorParameters parameters)
		{
			_functionRegistry = parameters.SessionFactory.Settings.LinqToHqlGeneratorsRegistry;
			_parameters = parameters;
		}

		public ISessionFactory SessionFactory => _parameters.SessionFactory;
		
		public HqlTreeNode Visit(Expression expression)
		{
			if (expression == null)
				return null;

			switch (expression.NodeType)
			{
				case ExpressionType.ArrayLength:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
				case ExpressionType.Not:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
				case ExpressionType.UnaryPlus:
					return VisitUnaryExpression((UnaryExpression)expression);
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
					return VisitBinaryExpression((BinaryExpression)expression);
				case ExpressionType.Conditional:
					return VisitConditionalExpression((ConditionalExpression)expression);
				case ExpressionType.Constant:
					return VisitConstantExpression((ConstantExpression)expression);
				case ExpressionType.Invoke:
					return VisitInvocationExpression((InvocationExpression)expression);
				case ExpressionType.Lambda:
					return VisitLambdaExpression((LambdaExpression)expression);
				case ExpressionType.MemberAccess:
					return VisitMemberExpression((MemberExpression)expression);
				case ExpressionType.Call:
					return VisitMethodCallExpression((MethodCallExpression)expression);
				//case ExpressionType.New:
				//    return VisitNewExpression((NewExpression)expression);
				//case ExpressionType.NewArrayBounds:
				case ExpressionType.NewArrayInit:
					return VisitNewArrayExpression((NewArrayExpression)expression);
				//case ExpressionType.MemberInit:
				//    return VisitMemberInitExpression((MemberInitExpression)expression);
				//case ExpressionType.ListInit:
				//    return VisitListInitExpression((ListInitExpression)expression);
				case ExpressionType.Parameter:
					return VisitParameterExpression((ParameterExpression)expression);
				case ExpressionType.TypeIs:
					return VisitTypeBinaryExpression((TypeBinaryExpression)expression);

				default:
					switch (expression)
					{
						case SubQueryExpression subQueryExpression:
							return VisitSubQueryExpression(subQueryExpression);
						case QuerySourceReferenceExpression querySourceReferenceExpression:
							return VisitQuerySourceReferenceExpression(querySourceReferenceExpression);
						case VBStringComparisonExpression vbStringComparisonExpression:
							return VisitVBStringComparisonExpression(vbStringComparisonExpression);
						case NhSimpleExpression nhExpression:
							switch (nhExpression.NhNodeType)
							{
								case NhExpressionType.Average:
									return VisitNhAverage(nhExpression);
								case NhExpressionType.Min:
									return VisitNhMin(nhExpression);
								case NhExpressionType.Max:
									return VisitNhMax(nhExpression);
								case NhExpressionType.Sum:
									return VisitNhSum(nhExpression);
								case NhExpressionType.Count:
									return VisitNhCount(nhExpression);
								case NhExpressionType.Distinct:
									return VisitNhDistinct(nhExpression);
								case NhExpressionType.Star:
									return VisitNhStar(nhExpression);
								case NhExpressionType.Nominator:
									return Visit(nhExpression.Expression);
							}
							break;
					}

					throw new NotSupportedException(expression.ToString());
			}
		}

		private HqlTreeNode VisitTypeBinaryExpression(TypeBinaryExpression expression)
			=> BuildOfType(expression.Expression, expression.TypeOperand);

		internal HqlBooleanExpression BuildOfType(Expression expression, System.Type type)
		{
			var sessionFactory = _parameters.SessionFactory;
			if (sessionFactory.GetClassMetadata(type) is Persister.Entity.AbstractEntityPersister meta && !meta.IsExplicitPolymorphism)
			{
				//Adapted the logic found in SingleTableEntityPersister.DiscriminatorFilterFragment
				var nodes = meta
					.SubclassClosure
					.Select(typeName => (NHibernate.Persister.Entity.IQueryable)sessionFactory.GetEntityPersister(typeName))
					.Where(persister => !persister.IsAbstract)
					.Select(persister => _hqlTreeBuilder.Ident(persister.EntityName))
					.ToList();

				if (nodes.Count == 1)
				{
					return _hqlTreeBuilder.Equality(
						_hqlTreeBuilder.Dot(Visit(expression).AsExpression(), _hqlTreeBuilder.Class()),
						nodes[0]);
				}

				if (nodes.Count > 1)
				{
					return _hqlTreeBuilder.In(
						_hqlTreeBuilder.Dot(
							Visit(expression).AsExpression(),
							_hqlTreeBuilder.Class()),
						_hqlTreeBuilder.ExpressionSubTreeHolder(nodes));
				}

				if (nodes.Count == 0)
				{
					const string abstractClassWithNoSubclassExceptionMessageTemplate =
@"The class {0} can't be instantiated and does not have mapped subclasses;
possible solutions:
- don't map the abstract class
- map its subclasses.";

					throw new NotSupportedException(string.Format(abstractClassWithNoSubclassExceptionMessageTemplate, meta.EntityName));
				}
			}

			return _hqlTreeBuilder.Equality(
				_hqlTreeBuilder.Dot(Visit(expression).AsExpression(), _hqlTreeBuilder.Class()),
				_hqlTreeBuilder.Ident(type.FullName));
		}

		protected HqlTreeNode VisitNhStar(NhSimpleExpression expression)
			=> _hqlTreeBuilder.Star();

		private HqlTreeNode VisitInvocationExpression(InvocationExpression expression)
			=> Visit(expression.Expression);

		protected HqlTreeNode VisitNhAverage(NhSimpleExpression expression)
		{
			var hqlExpression = Visit(expression.Expression).AsExpression();
			if (expression.Type != expression.Expression.Type)
				hqlExpression = _hqlTreeBuilder.Cast(hqlExpression, expression.Type);

			return _hqlTreeBuilder.Cast(_hqlTreeBuilder.Average(hqlExpression), expression.Type);
		}

		protected HqlTreeNode VisitNhCount(NhSimpleExpression expression)
			=> _hqlTreeBuilder.Cast(_hqlTreeBuilder.Count(Visit(expression.Expression).AsExpression()), expression.Type);

		protected HqlTreeNode VisitNhMin(NhSimpleExpression expression)
			=>_hqlTreeBuilder.Min(Visit(expression.Expression).AsExpression());

		protected HqlTreeNode VisitNhMax(NhSimpleExpression expression)
			=> _hqlTreeBuilder.Max(Visit(expression.Expression).AsExpression());

		protected HqlTreeNode VisitNhSum(NhSimpleExpression expression)
			=> _hqlTreeBuilder.Cast(_hqlTreeBuilder.Sum(Visit(expression.Expression).AsExpression()), expression.Type);

		protected HqlTreeNode VisitNhDistinct(NhSimpleExpression expression)
		{
			var visitor = new HqlGeneratorExpressionVisitor(_parameters);
			return _hqlTreeBuilder.ExpressionSubTreeHolder(_hqlTreeBuilder.Distinct(), visitor.Visit(expression.Expression));
		}

		protected HqlTreeNode VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
			=> _hqlTreeBuilder.Ident(_parameters.QuerySourceNamer.GetName(expression.ReferencedQuerySource));

		private HqlTreeNode VisitVBStringComparisonExpression(VBStringComparisonExpression expression)
			// We ignore the case sensitivity flag in the same way that == does.
			=> Visit(expression.Comparison);

		protected HqlTreeNode VisitBinaryExpression(BinaryExpression expression)
		{
			if (expression.NodeType == ExpressionType.Equal)
			{
				return TranslateEqualityComparison(expression);
			}
			if (expression.NodeType == ExpressionType.NotEqual)
			{
				return TranslateInequalityComparison(expression);
			}

			var lhs = Visit(expression.Left).AsExpression();
			var rhs = Visit(expression.Right).AsExpression();

			switch (expression.NodeType)
			{
				case ExpressionType.And:
					return _hqlTreeBuilder.BitwiseAnd(lhs, rhs);

				case ExpressionType.AndAlso:
					return _hqlTreeBuilder.BooleanAnd(lhs.ToBooleanExpression(), rhs.ToBooleanExpression());

				case ExpressionType.Or:
					return _hqlTreeBuilder.BitwiseOr(lhs, rhs);

				case ExpressionType.OrElse:
					return _hqlTreeBuilder.BooleanOr(lhs.ToBooleanExpression(), rhs.ToBooleanExpression());

				case ExpressionType.Add:
					if (expression.Left.Type == typeof(string) && expression.Right.Type == typeof(string))
					{
						return _hqlTreeBuilder.MethodCall("concat", lhs, rhs);
					}
					return _hqlTreeBuilder.Add(lhs, rhs);

				case ExpressionType.Subtract:
					return _hqlTreeBuilder.Subtract(lhs, rhs);

				case ExpressionType.Multiply:
					return _hqlTreeBuilder.Multiply(lhs, rhs);

				case ExpressionType.Divide:
					return _hqlTreeBuilder.Divide(lhs, rhs);

				case ExpressionType.Modulo:
					return _hqlTreeBuilder.MethodCall("mod", lhs, rhs);

				case ExpressionType.LessThan:
					return _hqlTreeBuilder.LessThan(lhs, rhs);

				case ExpressionType.LessThanOrEqual:
					return _hqlTreeBuilder.LessThanOrEqual(lhs, rhs);

				case ExpressionType.GreaterThan:
					return _hqlTreeBuilder.GreaterThan(lhs, rhs);

				case ExpressionType.GreaterThanOrEqual:
					return _hqlTreeBuilder.GreaterThanOrEqual(lhs, rhs);

				case ExpressionType.Coalesce:
					return _hqlTreeBuilder.Coalesce(lhs, rhs);
			}

			throw new InvalidOperationException();
		}

		private HqlTreeNode TranslateInequalityComparison(BinaryExpression expression)
		{
			var lhs = Visit(expression.Left).ToArithmeticExpression();
			var rhs = Visit(expression.Right).ToArithmeticExpression();

			// Check for nulls on left or right.
			if (VisitorUtil.IsNullConstant(expression.Right))
				rhs = null;
			if (VisitorUtil.IsNullConstant(expression.Left))
				lhs = null;

			if (lhs == null && rhs == null)
			{
				return _hqlTreeBuilder.False();
			}

			if (lhs == null)
			{
				return _hqlTreeBuilder.IsNotNull(rhs);
			}

			if (rhs == null)
			{
				return _hqlTreeBuilder.IsNotNull(lhs);
			}

			var lhsNullable = IsNullable(lhs);
			var rhsNullable = IsNullable(rhs);

			var inequality = _hqlTreeBuilder.Inequality(lhs, rhs);

			if (!lhsNullable && !rhsNullable)
			{
				return inequality;
			}

			var lhs2 = Visit(expression.Left).ToArithmeticExpression();
			var rhs2 = Visit(expression.Right).ToArithmeticExpression();

			HqlBooleanExpression booleanExpression;
			if (lhsNullable && rhsNullable)
			{
				booleanExpression = _hqlTreeBuilder.Inequality(
					_hqlTreeBuilder.IsNull(lhs2).ToArithmeticExpression(),
					_hqlTreeBuilder.IsNull(rhs2).ToArithmeticExpression());
			}
			else if (lhsNullable)
			{
				booleanExpression = _hqlTreeBuilder.IsNull(lhs2);
			}
			else
			{
				booleanExpression = _hqlTreeBuilder.IsNull(rhs2);
			}

			return _hqlTreeBuilder.BooleanOr(inequality, booleanExpression);
		}

		private HqlTreeNode TranslateEqualityComparison(BinaryExpression expression)
		{
			var lhs = Visit(expression.Left).ToArithmeticExpression();
			var rhs = Visit(expression.Right).ToArithmeticExpression();

			// Check for nulls on left or right.
			if (VisitorUtil.IsNullConstant(expression.Right))
			{
				rhs = null;
			}

			if (VisitorUtil.IsNullConstant(expression.Left))
			{
				lhs = null;
			}

			if (lhs == null && rhs == null)
			{
				return _hqlTreeBuilder.True();
			}

			if (lhs == null)
			{
				return _hqlTreeBuilder.IsNull(rhs);
			}

			if (rhs == null)
			{
				return _hqlTreeBuilder.IsNull((lhs));
			}

			var lhsNullable = IsNullable(lhs);
			var rhsNullable = IsNullable(rhs);

			var equality = _hqlTreeBuilder.Equality(lhs, rhs);

			if (!lhsNullable || !rhsNullable)
			{
				return equality;
			}

			var lhs2 = Visit(expression.Left).ToArithmeticExpression();
			var rhs2 = Visit(expression.Right).ToArithmeticExpression();

			return _hqlTreeBuilder.BooleanOr(
				equality,
				_hqlTreeBuilder.BooleanAnd(
					_hqlTreeBuilder.IsNull(lhs2),
					_hqlTreeBuilder.IsNull(rhs2)));
		}

		static bool IsNullable(HqlExpression original)
			=> original is HqlDot hqlDot && hqlDot.Children.Last() is HqlIdent;

		protected HqlTreeNode VisitUnaryExpression(UnaryExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Negate:
					return _hqlTreeBuilder.Negate(Visit(expression.Operand).AsExpression());
				case ExpressionType.UnaryPlus:
					return Visit(expression.Operand).AsExpression();
				case ExpressionType.Not:
					return _hqlTreeBuilder.BooleanNot(Visit(expression.Operand).ToBooleanExpression());
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.TypeAs:
					if ((expression.Operand.Type.IsPrimitive || expression.Operand.Type == typeof(Decimal)) &&
						(expression.Type.IsPrimitive || expression.Type == typeof(Decimal)))
					{
						return _hqlTreeBuilder.Cast(Visit(expression.Operand).AsExpression(), expression.Type);
					}

					return Visit(expression.Operand);
			}

			throw new NotSupportedException(expression.ToString());
		}

		protected HqlTreeNode VisitMemberExpression(MemberExpression expression)
		{
			// Strip out the .Value property of a nullable type, HQL doesn't need that
			if (expression.Member.Name == nameof(Nullable<int>.Value) && expression.Expression.Type.IsNullable())
			{
				return Visit(expression.Expression);
			}

			// Look for "special" properties (DateTime.Month etc)
			if (_functionRegistry.TryGetGenerator(expression.Member, out IHqlGeneratorForProperty generator))
			{
				return generator.BuildHql(expression.Member, expression.Expression, _hqlTreeBuilder, this);
			}

			// Else just emit standard HQL for a property reference
			return _hqlTreeBuilder.Dot(Visit(expression.Expression).AsExpression(), _hqlTreeBuilder.Ident(expression.Member.Name));
		}

		protected HqlTreeNode VisitConstantExpression(ConstantExpression expression)
		{
			if (expression.Value != null)
			{
				if (expression.Value is IEntityNameProvider entityName)
				{
					return _hqlTreeBuilder.Ident(entityName.EntityName);
				}
			}

			if (_parameters.ConstantToParameterMap.TryGetValue(expression, out NamedParameter namedParameter))
			{
				_parameters.RequiredHqlParameters.Add(new NamedParameterDescriptor(namedParameter.Name, null, false));

				return _hqlTreeBuilder.Parameter(namedParameter.Name).AsExpression();
			}

			return _hqlTreeBuilder.Constant(expression.Value);
		}

		protected HqlTreeNode VisitMethodCallExpression(MethodCallExpression expression)
		{
			var method = expression.Method;
			if (!_functionRegistry.TryGetGenerator(method, out IHqlGeneratorForMethod generator))
			{
				throw new NotSupportedException(method.ToString());
			}

			return generator.BuildHql(method, expression.Object, expression.Arguments, _hqlTreeBuilder, this);
		}

		protected HqlTreeNode VisitLambdaExpression(LambdaExpression expression)
			=> Visit(expression.Body);

		protected HqlTreeNode VisitParameterExpression(ParameterExpression expression)
			=> _hqlTreeBuilder.Ident(expression.Name);

		protected HqlTreeNode VisitConditionalExpression(ConditionalExpression expression)
		{
			var test = Visit(expression.Test).ToBooleanExpression();
			var ifTrue = Visit(expression.IfTrue).ToArithmeticExpression();
			var ifFalse = (expression.IfFalse != null
				? Visit(expression.IfFalse).ToArithmeticExpression()
				: null);

			HqlExpression @case = _hqlTreeBuilder.Case(new[] { _hqlTreeBuilder.When(test, ifTrue) }, ifFalse);

			return (expression.Type == typeof(bool) || expression.Type == (typeof(bool?)))
				? @case
				: _hqlTreeBuilder.Cast(@case, expression.Type);
		}

		protected HqlTreeNode VisitSubQueryExpression(SubQueryExpression expression)
		{
			var query = QueryModelVisitor.GenerateHqlQuery(expression.QueryModel, _parameters, false);
			return query.Statement;
		}

		protected HqlTreeNode VisitNewArrayExpression(NewArrayExpression expression)
		{
			var expressionSubTree = expression.Expressions.Select(exp => Visit(exp)).ToArray();
			return _hqlTreeBuilder.ExpressionSubTreeHolder(expressionSubTree);
		}
	}
}
