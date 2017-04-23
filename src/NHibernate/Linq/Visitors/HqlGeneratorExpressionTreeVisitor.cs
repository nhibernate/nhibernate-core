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
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors
{
	public class HqlGeneratorExpressionTreeVisitor : IHqlExpressionVisitor
	{
		private readonly HqlTreeBuilder _hqlTreeBuilder = new HqlTreeBuilder();
		private readonly VisitorParameters _parameters;
		private readonly ILinqToHqlGeneratorsRegistry _functionRegistry;

		public static HqlTreeNode Visit(Expression expression, VisitorParameters parameters)
		{
			return new HqlGeneratorExpressionTreeVisitor(parameters).VisitExpression(expression);
		}

		public HqlGeneratorExpressionTreeVisitor(VisitorParameters parameters)
		{
			_functionRegistry = parameters.SessionFactory.Settings.LinqToHqlGeneratorsRegistry;
			_parameters = parameters;
		}


		public ISessionFactory SessionFactory { get { return _parameters.SessionFactory; } }


		public HqlTreeNode Visit(Expression expression)
		{
			return VisitExpression(expression);
		}

		protected HqlTreeNode VisitExpression(Expression expression)
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
					return VisitUnaryExpression((UnaryExpression) expression);
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
					return VisitBinaryExpression((BinaryExpression) expression);
				case ExpressionType.Conditional:
					return VisitConditionalExpression((ConditionalExpression) expression);
				case ExpressionType.Constant:
					return VisitConstantExpression((ConstantExpression) expression);
				case ExpressionType.Invoke:
					return VisitInvocationExpression((InvocationExpression) expression);
				case ExpressionType.Lambda:
					return VisitLambdaExpression((LambdaExpression) expression);
				case ExpressionType.MemberAccess:
					return VisitMemberExpression((MemberExpression) expression);
				case ExpressionType.Call:
					return VisitMethodCallExpression((MethodCallExpression) expression);
					//case ExpressionType.New:
					//    return VisitNewExpression((NewExpression)expression);
					//case ExpressionType.NewArrayBounds:
				case ExpressionType.NewArrayInit:
					return VisitNewArrayExpression((NewArrayExpression) expression);
					//case ExpressionType.MemberInit:
					//    return VisitMemberInitExpression((MemberInitExpression)expression);
					//case ExpressionType.ListInit:
					//    return VisitListInitExpression((ListInitExpression)expression);
				case ExpressionType.Parameter:
					return VisitParameterExpression((ParameterExpression) expression);
				case ExpressionType.TypeIs:
					return VisitTypeBinaryExpression((TypeBinaryExpression) expression);

				default:
					var subQueryExpression = expression as SubQueryExpression;
					if (subQueryExpression != null)
						return VisitSubQueryExpression(subQueryExpression);

					var querySourceReferenceExpression = expression as QuerySourceReferenceExpression;
					if (querySourceReferenceExpression != null)
						return VisitQuerySourceReferenceExpression(querySourceReferenceExpression);

					var vbStringComparisonExpression = expression as VBStringComparisonExpression;
					if (vbStringComparisonExpression != null)
						return VisitVBStringComparisonExpression(vbStringComparisonExpression);

					switch ((NhExpressionType) expression.NodeType)
					{
						case NhExpressionType.Average:
							return VisitNhAverage((NhAverageExpression) expression);
						case NhExpressionType.Min:
							return VisitNhMin((NhMinExpression) expression);
						case NhExpressionType.Max:
							return VisitNhMax((NhMaxExpression) expression);
						case NhExpressionType.Sum:
							return VisitNhSum((NhSumExpression) expression);
						case NhExpressionType.Count:
							return VisitNhCount((NhCountExpression) expression);
						case NhExpressionType.Distinct:
							return VisitNhDistinct((NhDistinctExpression) expression);
						case NhExpressionType.Star:
							return VisitNhStar((NhStarExpression) expression);
						case NhExpressionType.Nominator:
							return VisitExpression(((NhNominatedExpression) expression).Expression);
							//case NhExpressionType.New:
							//    return VisitNhNew((NhNewExpression)expression);
					}

					throw new NotSupportedException(expression.ToString());
			}
		}

		private HqlTreeNode VisitTypeBinaryExpression(TypeBinaryExpression expression)
		{
			return BuildOfType(expression.Expression, expression.TypeOperand);
		}

		internal HqlBooleanExpression BuildOfType(Expression expression, System.Type type)
		{
			var sessionFactory = _parameters.SessionFactory;
			var meta = sessionFactory.GetClassMetadata(type) as Persister.Entity.AbstractEntityPersister;
			if (meta != null && !meta.IsExplicitPolymorphism)
			{
				//Adapted the logic found in SingleTableEntityPersister.DiscriminatorFilterFragment
				var nodes = meta
					.SubclassClosure
					.Select(typeName => (NHibernate.Persister.Entity.IQueryable) sessionFactory.GetEntityPersister(typeName))
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
@"The class {0} can't be instatiated and does not have mapped subclasses; 
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

		protected HqlTreeNode VisitNhStar(NhStarExpression expression)
		{
			return _hqlTreeBuilder.Star();
		}

		private HqlTreeNode VisitInvocationExpression(InvocationExpression expression)
		{
			return VisitExpression(expression.Expression);
		}

		protected HqlTreeNode VisitNhAverage(NhAverageExpression expression)
		{
			var hqlExpression = VisitExpression(expression.Expression).AsExpression();
			if (expression.Type != expression.Expression.Type)
				hqlExpression = _hqlTreeBuilder.Cast(hqlExpression, expression.Type);

			return _hqlTreeBuilder.Cast(_hqlTreeBuilder.Average(hqlExpression), expression.Type);
		}

		protected HqlTreeNode VisitNhCount(NhCountExpression expression)
		{
			return _hqlTreeBuilder.Cast(_hqlTreeBuilder.Count(VisitExpression(expression.Expression).AsExpression()), expression.Type);
		}

		protected HqlTreeNode VisitNhMin(NhMinExpression expression)
		{
			return _hqlTreeBuilder.Min(VisitExpression(expression.Expression).AsExpression());
		}

		protected HqlTreeNode VisitNhMax(NhMaxExpression expression)
		{
			return _hqlTreeBuilder.Max(VisitExpression(expression.Expression).AsExpression());
		}

		protected HqlTreeNode VisitNhSum(NhSumExpression expression)
		{
			return _hqlTreeBuilder.Cast(_hqlTreeBuilder.Sum(VisitExpression(expression.Expression).AsExpression()), expression.Type);
		}

		protected HqlTreeNode VisitNhDistinct(NhDistinctExpression expression)
		{
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters);
			return _hqlTreeBuilder.ExpressionSubTreeHolder(_hqlTreeBuilder.Distinct(), visitor.VisitExpression(expression.Expression));
		}

		protected HqlTreeNode VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			return _hqlTreeBuilder.Ident(_parameters.QuerySourceNamer.GetName(expression.ReferencedQuerySource));
		}

		private HqlTreeNode VisitVBStringComparisonExpression(VBStringComparisonExpression expression)
		{
			// We ignore the case sensitivity flag in the same way that == does.
			return VisitExpression(expression.Comparison);
		}

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

			var lhs = VisitExpression(expression.Left).AsExpression();
			var rhs = VisitExpression(expression.Right).AsExpression();

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
					if (expression.Left.Type == typeof (string) && expression.Right.Type == typeof(string))
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
			var lhs = VisitExpression(expression.Left).ToArithmeticExpression();
			var rhs = VisitExpression(expression.Right).ToArithmeticExpression();

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

			var lhs2 = VisitExpression(expression.Left).ToArithmeticExpression();
			var rhs2 = VisitExpression(expression.Right).ToArithmeticExpression();

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
			var lhs = VisitExpression(expression.Left).ToArithmeticExpression();
			var rhs = VisitExpression(expression.Right).ToArithmeticExpression();

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

			var lhs2 = VisitExpression(expression.Left).ToArithmeticExpression();
			var rhs2 = VisitExpression(expression.Right).ToArithmeticExpression();

			return _hqlTreeBuilder.BooleanOr(
				equality,
				_hqlTreeBuilder.BooleanAnd(
					_hqlTreeBuilder.IsNull(lhs2),
					_hqlTreeBuilder.IsNull(rhs2)));
		}

		static bool IsNullable(HqlExpression original)
		{
			var hqlDot = original as HqlDot;
			return hqlDot != null && hqlDot.Children.Last() is HqlIdent;
		}

		protected HqlTreeNode VisitUnaryExpression(UnaryExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Negate:
					return _hqlTreeBuilder.Negate(VisitExpression(expression.Operand).AsExpression());
				case ExpressionType.UnaryPlus:
					return VisitExpression(expression.Operand).AsExpression();
				case ExpressionType.Not:
					return _hqlTreeBuilder.BooleanNot(VisitExpression(expression.Operand).ToBooleanExpression());
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.TypeAs:
					if ((expression.Operand.Type.IsPrimitive || expression.Operand.Type == typeof(Decimal)) &&
						(expression.Type.IsPrimitive || expression.Type == typeof(Decimal)))
					{
						return _hqlTreeBuilder.Cast(VisitExpression(expression.Operand).AsExpression(), expression.Type);
					}

					return VisitExpression(expression.Operand);
			}

			throw new NotSupportedException(expression.ToString());
		}

		protected HqlTreeNode VisitMemberExpression(MemberExpression expression)
		{
			// Strip out the .Value property of a nullable type, HQL doesn't need that
			if (expression.Member.Name == "Value" && expression.Expression.Type.IsNullable())
			{
				return VisitExpression(expression.Expression);
			}

			// Look for "special" properties (DateTime.Month etc)
			IHqlGeneratorForProperty generator;

			if (_functionRegistry.TryGetGenerator(expression.Member, out generator))
			{
				return generator.BuildHql(expression.Member, expression.Expression, _hqlTreeBuilder, this);
			}

			// Else just emit standard HQL for a property reference
			return _hqlTreeBuilder.Dot(VisitExpression(expression.Expression).AsExpression(), _hqlTreeBuilder.Ident(expression.Member.Name));
		}

		protected HqlTreeNode VisitConstantExpression(ConstantExpression expression)
		{
			if (expression.Value != null)
			{
				IEntityNameProvider entityName = expression.Value as IEntityNameProvider;
				if (entityName != null)
				{
					return _hqlTreeBuilder.Ident(entityName.EntityName);
				}
			}

			NamedParameter namedParameter;

			if (_parameters.ConstantToParameterMap.TryGetValue(expression, out namedParameter))
			{
				_parameters.RequiredHqlParameters.Add(new NamedParameterDescriptor(namedParameter.Name, null, false));

				return _hqlTreeBuilder.Parameter(namedParameter.Name).AsExpression();
			}

			return _hqlTreeBuilder.Constant(expression.Value);
		}

		protected HqlTreeNode VisitMethodCallExpression(MethodCallExpression expression)
		{
			IHqlGeneratorForMethod generator;

			var method = expression.Method;
			if (!_functionRegistry.TryGetGenerator(method, out generator))
			{
				throw new NotSupportedException(method.ToString());
			}

			return generator.BuildHql(method, expression.Object, expression.Arguments, _hqlTreeBuilder, this);
		}

		protected HqlTreeNode VisitLambdaExpression(LambdaExpression expression)
		{
			return VisitExpression(expression.Body);
		}

		protected HqlTreeNode VisitParameterExpression(ParameterExpression expression)
		{
			return _hqlTreeBuilder.Ident(expression.Name);
		}

		protected HqlTreeNode VisitConditionalExpression(ConditionalExpression expression)
		{
			var test = VisitExpression(expression.Test).ToBooleanExpression();
			var ifTrue = VisitExpression(expression.IfTrue).ToArithmeticExpression();
			var ifFalse = (expression.IfFalse != null
							   ? VisitExpression(expression.IfFalse).ToArithmeticExpression()
							   : null);

			HqlExpression @case = _hqlTreeBuilder.Case(new[] {_hqlTreeBuilder.When(test, ifTrue)}, ifFalse);

			return (expression.Type == typeof (bool) || expression.Type == (typeof (bool?)))
					   ? @case
					   : _hqlTreeBuilder.Cast(@case, expression.Type);
		}

		protected HqlTreeNode VisitSubQueryExpression(SubQueryExpression expression)
		{
			ExpressionToHqlTranslationResults query = QueryModelVisitor.GenerateHqlQuery(expression.QueryModel, _parameters, false, null);
			return query.Statement;
		}

		protected HqlTreeNode VisitNewArrayExpression(NewArrayExpression expression)
		{
			var expressionSubTree = expression.Expressions.Select(exp => VisitExpression(exp)).ToArray();
			return _hqlTreeBuilder.ExpressionSubTreeHolder(expressionSubTree);
		}
	}
}
