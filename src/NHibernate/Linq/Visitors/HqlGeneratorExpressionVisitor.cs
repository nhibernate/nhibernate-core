using System;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Functions;
using NHibernate.Param;
using NHibernate.Type;
using NHibernate.Util;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.Visitors
{
	public class HqlGeneratorExpressionVisitor : IHqlExpressionVisitor
	{
		private readonly HqlTreeBuilder _hqlTreeBuilder = new HqlTreeBuilder();
		private readonly VisitorParameters _parameters;
		private readonly ILinqToHqlGeneratorsRegistry _functionRegistry;
		private readonly NullableExpressionDetector _nullableExpressionDetector;

		public static HqlTreeNode Visit(Expression expression, VisitorParameters parameters)
		{
			return new HqlGeneratorExpressionVisitor(parameters).Visit(expression);
		}

		public HqlGeneratorExpressionVisitor(VisitorParameters parameters)
		{
			_functionRegistry = parameters.SessionFactory.Settings.LinqToHqlGeneratorsRegistry;
			_parameters = parameters;
			_nullableExpressionDetector = new NullableExpressionDetector(_parameters.SessionFactory, _functionRegistry);
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
					//    return VisitNew((NewExpression)expression);
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
				case ExpressionType.Dynamic:
					return VisitDynamicExpression((DynamicExpression) expression);

				case ExpressionType.Extension:
					switch (expression)
					{
						case SubQueryExpression subQueryExpression:
							return VisitSubQueryExpression(subQueryExpression);
						case QuerySourceReferenceExpression querySourceReferenceExpression:
							return VisitQuerySourceReferenceExpression(querySourceReferenceExpression);
						case VBStringComparisonExpression vbStringComparisonExpression:
							return VisitVBStringComparisonExpression(vbStringComparisonExpression);
						case NhAverageExpression nhAverageExpression:
							return VisitNhAverage(nhAverageExpression);
						case NhMinExpression nhMinExpression:
							return VisitNhMin(nhMinExpression);
						case NhMaxExpression nhMaxExpression:
							return VisitNhMax(nhMaxExpression);
						case NhSumExpression nhSumExpression:
							return VisitNhSum(nhSumExpression);
						case NhCountExpression nhCountExpression:
							return VisitNhCount(nhCountExpression);
						case NhDistinctExpression nhDistinctExpression:
							return VisitNhDistinct(nhDistinctExpression);
						case NhStarExpression nhStarExpression:
							return VisitNhStar(nhStarExpression);
						case NhNominatedExpression nhNominatedExpression:
							return VisitNhNominated(nhNominatedExpression);
						//case NhNewExpression nhNewExpression:
						//	return VisitNhNew(nhNewExpression);
						default:
							throw new NotSupportedException(expression.ToString());
					}

				default:
					throw new NotSupportedException(expression.NodeType.ToString());
			}
		}

		private HqlTreeNode VisitDynamicExpression(DynamicExpression expression)
		{
			switch (expression.Binder)
			{
				case GetMemberBinder binder:
					return _hqlTreeBuilder.Dot(
						VisitExpression(expression.Arguments[0]).AsExpression(),
						_hqlTreeBuilder.Ident(binder.Name));
			}

			throw new NotSupportedException($"Dynamic expression with a binder of {expression.Binder.GetType()} is not supported");
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

		private HqlTreeNode VisitNhNominated(NhNominatedExpression nhNominatedExpression)
		{
			return VisitExpression(nhNominatedExpression.Expression);
		}

		private HqlTreeNode VisitInvocationExpression(InvocationExpression expression)
		{
			//This is an ugly workaround for dynamic expressions.
			//Unfortunately we can not tap into the expression tree earlier to intercept the dynamic expression
			if (expression.Arguments.Count == 2 &&
			    expression.Arguments[0] is ConstantExpression constant &&
			    constant.Value is CallSite site &&
			    site.Binder is GetMemberBinder binder)
			{
				return _hqlTreeBuilder.Dot(
					VisitExpression(expression.Arguments[1]).AsExpression(),
					_hqlTreeBuilder.Ident(binder.Name));
			}

			return VisitExpression(expression.Expression);
		}

		protected HqlTreeNode VisitNhAverage(NhAverageExpression expression)
		{
			// We need to cast the argument when its type is different from Average method return type,
			// otherwise the result may be incorrect. In SQL Server avg always returns int
			// when the argument is int.
			var hqlExpression = VisitExpression(expression.Expression).AsExpression();
			hqlExpression = IsCastRequired(expression.Expression, expression.Type, out _)
				? (HqlExpression) _hqlTreeBuilder.Cast(hqlExpression, expression.Type)
				: _hqlTreeBuilder.TransparentCast(hqlExpression, expression.Type);

			// In Oracle the avg function can return a number with up to 40 digits which cannot be retrieved from the data reader due to the lack of such
			// numeric type in .NET. In order to avoid that we have to add a cast to trim the number so that it can be converted into a .NET numeric type.
			return _hqlTreeBuilder.Cast(_hqlTreeBuilder.Average(hqlExpression), expression.Type);
		}

		protected HqlTreeNode VisitNhCount(NhCountExpression expression)
		{
			if (expression is NhLongCountExpression)
			{
				return IsCastRequired(expression.Type, "count_big", out _)
					? (HqlTreeNode) _hqlTreeBuilder.Cast(_hqlTreeBuilder.CountBig(VisitExpression(expression.Expression).AsExpression()), expression.Type)
					: _hqlTreeBuilder.TransparentCast(_hqlTreeBuilder.CountBig(VisitExpression(expression.Expression).AsExpression()), expression.Type);
			}

			return IsCastRequired(expression.Type, "count", out _)
				? (HqlTreeNode) _hqlTreeBuilder.Cast(_hqlTreeBuilder.Count(VisitExpression(expression.Expression).AsExpression()), expression.Type)
				: _hqlTreeBuilder.TransparentCast(_hqlTreeBuilder.Count(VisitExpression(expression.Expression).AsExpression()), expression.Type);
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
			return IsCastRequired("sum", expression.Expression, expression.Type)
				? (HqlTreeNode) _hqlTreeBuilder.Cast(_hqlTreeBuilder.Sum(VisitExpression(expression.Expression).AsExpression()), expression.Type)
				: _hqlTreeBuilder.TransparentCast(_hqlTreeBuilder.Sum(VisitExpression(expression.Expression).AsExpression()), expression.Type);
		}

		protected HqlTreeNode VisitNhDistinct(NhDistinctExpression expression)
		{
			var visitor = new HqlGeneratorExpressionVisitor(_parameters);
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

			_nullableExpressionDetector.SearchForNotNullMemberChecks(expression);

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
					return _hqlTreeBuilder.Coalesce(lhs.ToArithmeticExpression(), rhs.ToArithmeticExpression());
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

			var lhsNullable = _nullableExpressionDetector.IsNullable(expression.Left, expression);
			var rhsNullable = _nullableExpressionDetector.IsNullable(expression.Right, expression);

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

			var lhsNullable = _nullableExpressionDetector.IsNullable(expression.Left, expression);
			var rhsNullable = _nullableExpressionDetector.IsNullable(expression.Right, expression);

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
					return IsCastRequired(expression.Operand, expression.Type, out var existType)
						? _hqlTreeBuilder.Cast(VisitExpression(expression.Operand).AsExpression(), expression.Type)
						// Make a transparent cast when an IType exists, so that it can be used to retrieve the value from the data reader
						: existType && HqlIdent.SupportsType(expression.Type)
							? _hqlTreeBuilder.TransparentCast(VisitExpression(expression.Operand).AsExpression(), expression.Type)
							: VisitExpression(expression.Operand);
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

			// If both operands are parameters, HQL will not be able to determine the resulting type before
			// parameters binding. But it has to compute result set columns type before parameters are bound,
			// so an artificial cast is introduced to hint HQL at the resulting type.
			return expression.Type == typeof(bool) || expression.Type == typeof(bool?)
				? @case
				: _hqlTreeBuilder.TransparentCast(@case, expression.Type);
		}

		protected HqlTreeNode VisitSubQueryExpression(SubQueryExpression expression)
		{
			ExpressionToHqlTranslationResults query = QueryModelVisitor.GenerateHqlQuery(expression.QueryModel, _parameters, false, null);
			return query.Statement;
		}

		protected HqlTreeNode VisitNewArrayExpression(NewArrayExpression expression)
		{
			var expressionSubTree = expression.Expressions.ToArray(exp => VisitExpression(exp));
			return _hqlTreeBuilder.ExpressionSubTreeHolder(expressionSubTree);
		}

		private bool IsCastRequired(Expression expression, System.Type toType, out bool existType)
		{
			existType = false;
			return toType != typeof(object) &&
					IsCastRequired(GetType(expression), TypeFactory.GetDefaultTypeFor(toType), out existType);
		}

		private bool IsCastRequired(IType type, IType toType, out bool existType)
		{
			// A type can be null when casting an entity into a base class, in that case we should not cast
			if (type == null || toType == null || Equals(type, toType))
			{
				existType = false;
				return false;
			}

			var sqlTypes = type.SqlTypes(_parameters.SessionFactory);
			var toSqlTypes = toType.SqlTypes(_parameters.SessionFactory);
			if (sqlTypes.Length != 1 || toSqlTypes.Length != 1)
			{
				existType = false;
				return false; // Casting a multi-column type is not possible
			}

			existType = true;
			if (sqlTypes[0].DbType == toSqlTypes[0].DbType)
			{
				return false;
			}

			if (type.ReturnedClass.IsEnum && sqlTypes[0].DbType == DbType.String)
			{
				existType = false;
				return false; // Never cast an enum that is mapped as string, the type will provide a string for the parameter value
			}

			// Some dialects can map several sql types into one, cast only if the dialect types are different
			if (!_parameters.SessionFactory.Dialect.TryGetCastTypeName(sqlTypes[0], out var castTypeName) ||
			    !_parameters.SessionFactory.Dialect.TryGetCastTypeName(toSqlTypes[0], out var toCastTypeName))
			{
				return false; // The dialect does not support such cast
			}

			return castTypeName != toCastTypeName;
		}

		private bool IsCastRequired(string sqlFunctionName, Expression argumentExpression, System.Type returnType)
		{
			var argumentType = GetType(argumentExpression);
			if (argumentType == null || returnType == typeof(object))
			{
				return false;
			}

			var returnNhType = TypeFactory.GetDefaultTypeFor(returnType);
			if (returnNhType == null)
			{
				return true; // Fallback to the old behavior
			}

			var sqlFunction = _parameters.SessionFactory.SQLFunctionRegistry.FindSQLFunction(sqlFunctionName);
			if (sqlFunction == null)
			{
				return true; // Fallback to the old behavior
			}

			var fnReturnType = sqlFunction.ReturnType(argumentType, _parameters.SessionFactory);
			return fnReturnType == null || IsCastRequired(fnReturnType, returnNhType, out _);
		}

		private IType GetType(Expression expression)
		{
			// Try to get the mapped type for the member as it may be a non default one
			return expression.Type == typeof(object)
				? null
				: (ExpressionsHelper.TryGetMappedType(_parameters.SessionFactory, expression, out var type, out _, out _, out _)
					? type
					: TypeFactory.GetDefaultTypeFor(expression.Type));
		}
	}
}
