
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NHibernate.Criterion;
using NHibernate.Util;
using Expression = System.Linq.Expressions.Expression;

namespace NHibernate.Impl
{

	/// <summary>
	/// Subquery type enumeration
	/// </summary>
	public enum LambdaSubqueryType
	{
		/// <summary>exact</summary>
		Exact = 1,
		/// <summary>all</summary>
		All = 2,
		/// <summary>some</summary>
		Some = 3,
	}

	/// <summary>
	/// Converts lambda expressions to NHibernate criterion/order
	/// </summary>
	public static class ExpressionProcessor
	{
		public class ProjectionInfo
		{
			private string _property;
			private IProjection _projection;

			protected ProjectionInfo() { }
			public static ProjectionInfo ForProperty(string property) { return new ProjectionInfo() { _property = property }; }
			public static ProjectionInfo ForProjection(IProjection projection) { return new ProjectionInfo() { _projection = projection }; }

			public IProjection AsProjection() { return _projection ?? Projections.Property(_property); }

			public ICriterion CreateCriterion(Func<string, ICriterion> stringFunc, Func<IProjection, ICriterion> projectionFunc)
			{
				return (_property != null)
					? stringFunc(_property)
					: projectionFunc(_projection);
			}

			public ICriterion CreateCriterion(Func<string, object, ICriterion> stringFunc, Func<IProjection, object, ICriterion> projectionFunc, object value)
			{
				return (_property != null)
					? stringFunc(_property, value)
					: projectionFunc(_projection, value);
			}

			public ICriterion CreateCriterion(ProjectionInfo rhs,
												Func<string, string, ICriterion> ssFunc,
												Func<string, IProjection, ICriterion> spFunc,
												Func<IProjection, string, ICriterion> psFunc,
												Func<IProjection, IProjection, ICriterion> ppFunc)
			{
				if (_property != null && rhs._property != null)
					return ssFunc(_property, rhs._property);
				if (_property != null)
					return spFunc(_property, rhs._projection);
				if (rhs._property != null)
					return psFunc(_projection, rhs._property);
				return ppFunc(_projection, rhs._projection);
			}

			public T Create<T>(Func<string, T> stringFunc, Func<IProjection, T> projectionFunc)
			{
				return (_property != null)
					? stringFunc(_property)
					: projectionFunc(_projection);
			}

			public Order CreateOrder(Func<string, Order> orderStringDelegate, Func<IProjection, Order> orderProjectionDelegate)
			{
				return (_property != null)
					? orderStringDelegate(_property)
					: orderProjectionDelegate(_projection);
			}


			/// <summary>
			/// Retrieve the property name from a supplied PropertyProjection
			/// Note:  throws is the supplied IProjection is not a PropertyProjection
			/// </summary>
			public string AsProperty()
			{
				if (_property != null) return _property;

				var propertyProjection = _projection as PropertyProjection;
				if (propertyProjection == null) throw new Exception("Cannot determine property for " + _projection);
				return propertyProjection.PropertyName;
			}
		}

		private readonly static IDictionary<ExpressionType, Func<ProjectionInfo, object, ICriterion>> _simpleExpressionCreators;
		private readonly static IDictionary<ExpressionType, Func<ProjectionInfo, ProjectionInfo, ICriterion>> _propertyExpressionCreators;
		private readonly static IDictionary<LambdaSubqueryType, IDictionary<ExpressionType, Func<string, DetachedCriteria, AbstractCriterion>>> _subqueryExpressionCreatorTypes;
		private readonly static IDictionary<string, Func<MethodCallExpression, ICriterion>> _customMethodCallProcessors;
		private readonly static IDictionary<string, Func<Expression, IProjection>> _customProjectionProcessors;

		static ExpressionProcessor()
		{
			_simpleExpressionCreators = new Dictionary<ExpressionType, Func<ProjectionInfo, object, ICriterion>>();
			_simpleExpressionCreators[ExpressionType.Equal] = Eq;
			_simpleExpressionCreators[ExpressionType.NotEqual] = Ne;
			_simpleExpressionCreators[ExpressionType.GreaterThan] = Gt;
			_simpleExpressionCreators[ExpressionType.GreaterThanOrEqual] = Ge;
			_simpleExpressionCreators[ExpressionType.LessThan] = Lt;
			_simpleExpressionCreators[ExpressionType.LessThanOrEqual] = Le;

			_propertyExpressionCreators = new Dictionary<ExpressionType, Func<ProjectionInfo, ProjectionInfo, ICriterion>>();
			_propertyExpressionCreators[ExpressionType.Equal]				= (lhs, rhs) => lhs.CreateCriterion(rhs, Restrictions.EqProperty, Restrictions.EqProperty, Restrictions.EqProperty, Restrictions.EqProperty);
			_propertyExpressionCreators[ExpressionType.NotEqual]			= (lhs, rhs) => lhs.CreateCriterion(rhs, Restrictions.NotEqProperty, Restrictions.NotEqProperty, Restrictions.NotEqProperty, Restrictions.NotEqProperty);
			_propertyExpressionCreators[ExpressionType.GreaterThan]			= (lhs, rhs) => lhs.CreateCriterion(rhs, Restrictions.GtProperty, Restrictions.GtProperty, Restrictions.GtProperty, Restrictions.GtProperty);
			_propertyExpressionCreators[ExpressionType.GreaterThanOrEqual]	= (lhs, rhs) => lhs.CreateCriterion(rhs, Restrictions.GeProperty, Restrictions.GeProperty, Restrictions.GeProperty, Restrictions.GeProperty);
			_propertyExpressionCreators[ExpressionType.LessThan]			= (lhs, rhs) => lhs.CreateCriterion(rhs, Restrictions.LtProperty, Restrictions.LtProperty, Restrictions.LtProperty, Restrictions.LtProperty);
			_propertyExpressionCreators[ExpressionType.LessThanOrEqual]		= (lhs, rhs) => lhs.CreateCriterion(rhs, Restrictions.LeProperty, Restrictions.LeProperty, Restrictions.LeProperty, Restrictions.LeProperty);

			_subqueryExpressionCreatorTypes = new Dictionary<LambdaSubqueryType, IDictionary<ExpressionType, Func<string, DetachedCriteria, AbstractCriterion>>>();
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.Exact] = new Dictionary<ExpressionType, Func<string, DetachedCriteria, AbstractCriterion>>();
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.All] = new Dictionary<ExpressionType, Func<string, DetachedCriteria, AbstractCriterion>>();
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.Some] = new Dictionary<ExpressionType, Func<string, DetachedCriteria, AbstractCriterion>>();

			_subqueryExpressionCreatorTypes[LambdaSubqueryType.Exact][ExpressionType.Equal] = Subqueries.PropertyEq;
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.Exact][ExpressionType.NotEqual] = Subqueries.PropertyNe;
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.Exact][ExpressionType.GreaterThan] = Subqueries.PropertyGt;
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.Exact][ExpressionType.GreaterThanOrEqual] = Subqueries.PropertyGe;
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.Exact][ExpressionType.LessThan] = Subqueries.PropertyLt;
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.Exact][ExpressionType.LessThanOrEqual] = Subqueries.PropertyLe;

			_subqueryExpressionCreatorTypes[LambdaSubqueryType.All][ExpressionType.Equal] = Subqueries.PropertyEqAll;
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.All][ExpressionType.GreaterThan] = Subqueries.PropertyGtAll;
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.All][ExpressionType.GreaterThanOrEqual] = Subqueries.PropertyGeAll;
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.All][ExpressionType.LessThan] = Subqueries.PropertyLtAll;
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.All][ExpressionType.LessThanOrEqual] = Subqueries.PropertyLeAll;

			_subqueryExpressionCreatorTypes[LambdaSubqueryType.Some][ExpressionType.GreaterThan] = Subqueries.PropertyGtSome;
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.Some][ExpressionType.GreaterThanOrEqual] = Subqueries.PropertyGeSome;
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.Some][ExpressionType.LessThan] = Subqueries.PropertyLtSome;
			_subqueryExpressionCreatorTypes[LambdaSubqueryType.Some][ExpressionType.LessThanOrEqual] = Subqueries.PropertyLeSome;

			_customMethodCallProcessors = new Dictionary<string, Func<MethodCallExpression, ICriterion>>();
			RegisterCustomMethodCall(() => RestrictionExtensions.IsLike("", ""), RestrictionExtensions.ProcessIsLike);
			RegisterCustomMethodCall(() => RestrictionExtensions.IsLike("", "", null), RestrictionExtensions.ProcessIsLikeMatchMode);
			RegisterCustomMethodCall(() => RestrictionExtensions.IsLike("", "", null, null), RestrictionExtensions.ProcessIsLikeMatchModeEscapeChar);
			RegisterCustomMethodCall(() => RestrictionExtensions.IsInsensitiveLike("", ""), RestrictionExtensions.ProcessIsInsensitiveLike);
			RegisterCustomMethodCall(() => RestrictionExtensions.IsInsensitiveLike("", "", null), RestrictionExtensions.ProcessIsInsensitiveLikeMatchMode);
			RegisterCustomMethodCall(() => RestrictionExtensions.IsIn(null, new object[0]), RestrictionExtensions.ProcessIsInArray);
			RegisterCustomMethodCall(() => RestrictionExtensions.IsIn(null, new List<object>()), RestrictionExtensions.ProcessIsInCollection);
			RegisterCustomMethodCall(() => RestrictionExtensions.IsBetween(null, null).And(null), RestrictionExtensions.ProcessIsBetween);

			_customProjectionProcessors = new Dictionary<string, Func<Expression, IProjection>>();
			RegisterCustomProjection(() => default(DateTime).Year, e => ProjectionsExtensions.ProcessYear(e.Expression));
			RegisterCustomProjection(() => default(DateTime).Day, e => ProjectionsExtensions.ProcessDay(e.Expression));
			RegisterCustomProjection(() => default(DateTime).Month, e => ProjectionsExtensions.ProcessMonth(e.Expression));
		    RegisterCustomProjection(() => default(DateTime).Hour, e => ProjectionsExtensions.ProcessHour(e.Expression));
		    RegisterCustomProjection(() => default(DateTime).Minute, e => ProjectionsExtensions.ProcessMinute(e.Expression));
		    RegisterCustomProjection(() => default(DateTime).Second, e => ProjectionsExtensions.ProcessSecond(e.Expression));
		    RegisterCustomProjection(() => default(DateTime).Date, e => ProjectionsExtensions.ProcessDate(e.Expression));

			RegisterCustomProjection(() => default(DateTimeOffset).Year, e => ProjectionsExtensions.ProcessYear(e.Expression));
			RegisterCustomProjection(() => default(DateTimeOffset).Day, e => ProjectionsExtensions.ProcessDay(e.Expression));
			RegisterCustomProjection(() => default(DateTimeOffset).Month, e => ProjectionsExtensions.ProcessMonth(e.Expression));
		    RegisterCustomProjection(() => default(DateTimeOffset).Hour, e => ProjectionsExtensions.ProcessHour(e.Expression));
		    RegisterCustomProjection(() => default(DateTimeOffset).Minute, e => ProjectionsExtensions.ProcessMinute(e.Expression));
		    RegisterCustomProjection(() => default(DateTimeOffset).Second, e => ProjectionsExtensions.ProcessSecond(e.Expression));
		    RegisterCustomProjection(() => default(DateTimeOffset).Date, e => ProjectionsExtensions.ProcessDate(e.Expression));

#pragma warning disable 618
			RegisterCustomProjection(() => ProjectionsExtensions.YearPart(default(DateTime)), e => ProjectionsExtensions.ProcessYear(e.Arguments[0]));
			RegisterCustomProjection(() => ProjectionsExtensions.DayPart(default(DateTime)), e => ProjectionsExtensions.ProcessDay(e.Arguments[0]));
			RegisterCustomProjection(() => ProjectionsExtensions.MonthPart(default(DateTime)), e => ProjectionsExtensions.ProcessMonth(e.Arguments[0]));
			RegisterCustomProjection(() => ProjectionsExtensions.HourPart(default(DateTime)), e => ProjectionsExtensions.ProcessHour(e.Arguments[0]));
			RegisterCustomProjection(() => ProjectionsExtensions.MinutePart(default(DateTime)), e => ProjectionsExtensions.ProcessMinute(e.Arguments[0]));
			RegisterCustomProjection(() => ProjectionsExtensions.SecondPart(default(DateTime)), e => ProjectionsExtensions.ProcessSecond(e.Arguments[0]));
			RegisterCustomProjection(() => ProjectionsExtensions.DatePart(default(DateTime)), e => ProjectionsExtensions.ProcessDate(e.Arguments[0]));
#pragma warning restore 618

            RegisterCustomProjection(() => ProjectionsExtensions.Sqrt(default(int)), ProjectionsExtensions.ProcessSqrt);
			RegisterCustomProjection(() => ProjectionsExtensions.Sqrt(default(double)), ProjectionsExtensions.ProcessSqrt);
			RegisterCustomProjection(() => ProjectionsExtensions.Sqrt(default(decimal)), ProjectionsExtensions.ProcessSqrt);
			RegisterCustomProjection(() => ProjectionsExtensions.Sqrt(default(byte)), ProjectionsExtensions.ProcessSqrt);
			RegisterCustomProjection(() => ProjectionsExtensions.Sqrt(default(long)), ProjectionsExtensions.ProcessSqrt);
			RegisterCustomProjection(() => ProjectionsExtensions.Lower(string.Empty), ProjectionsExtensions.ProcessLower);
			RegisterCustomProjection(() => ProjectionsExtensions.Upper(string.Empty), ProjectionsExtensions.ProcessUpper);
			RegisterCustomProjection(() => ProjectionsExtensions.TrimStr(string.Empty), ProjectionsExtensions.ProcessTrimStr);
			RegisterCustomProjection(() => ProjectionsExtensions.StrLength(string.Empty), ProjectionsExtensions.ProcessStrLength);
			RegisterCustomProjection(() => ProjectionsExtensions.BitLength(string.Empty), ProjectionsExtensions.ProcessBitLength);
			RegisterCustomProjection(() => ProjectionsExtensions.Substr(string.Empty, 0, 0), ProjectionsExtensions.ProcessSubstr);
			RegisterCustomProjection(() => ProjectionsExtensions.CharIndex(string.Empty, string.Empty, 0), ProjectionsExtensions.ProcessCharIndex);
			RegisterCustomProjection(() => ProjectionsExtensions.Coalesce<DBNull>(null, null), ProjectionsExtensions.ProcessCoalesce);
			RegisterCustomProjection(() => ProjectionsExtensions.Coalesce<int>(null, 0), ProjectionsExtensions.ProcessCoalesce);
			RegisterCustomProjection(() => Projections.Concat(null), Projections.ProcessConcat);
			RegisterCustomProjection(() => ProjectionsExtensions.Mod(0, 0), ProjectionsExtensions.ProcessMod);
			RegisterCustomProjection(() => ProjectionsExtensions.Abs(default(int)), ProjectionsExtensions.ProcessIntAbs);
			RegisterCustomProjection(() => ProjectionsExtensions.Abs(default(double)), ProjectionsExtensions.ProcessDoubleAbs);
			RegisterCustomProjection(() => ProjectionsExtensions.Abs(default(Int64)), ProjectionsExtensions.ProcessInt64Abs);

			RegisterCustomProjection(() => Math.Round(default(double)), ProjectionsExtensions.ProcessRound);
			RegisterCustomProjection(() => Math.Round(default(decimal)), ProjectionsExtensions.ProcessRound);
			RegisterCustomProjection(() => Math.Round(default(double), default(int)), ProjectionsExtensions.ProcessRound);
			RegisterCustomProjection(() => Math.Round(default(decimal), default(int)), ProjectionsExtensions.ProcessRound);
		}

		private static ICriterion Eq(ProjectionInfo property, object value)
		{
			return property.CreateCriterion(Restrictions.Eq, Restrictions.Eq, value);
		}

		private static ICriterion Ne(ProjectionInfo property, object value)
		{
			return
				Restrictions.Not(
					property.CreateCriterion(Restrictions.Eq, Restrictions.Eq, value));
		}

		private static ICriterion Gt(ProjectionInfo property, object value)
		{
			return property.CreateCriterion(Restrictions.Gt, Restrictions.Gt, value);
		}

		private static ICriterion Ge(ProjectionInfo property, object value)
		{
			return property.CreateCriterion(Restrictions.Ge, Restrictions.Ge, value);
		}

		private static ICriterion Lt(ProjectionInfo property, object value)
		{
			return property.CreateCriterion(Restrictions.Lt, Restrictions.Lt, value);
		}

		private static ICriterion Le(ProjectionInfo property, object value)
		{
			return property.CreateCriterion(Restrictions.Le, Restrictions.Le, value);
		}

		/// <summary>
		/// Invoke the expression to extract its runtime value
		/// </summary>
		public static object FindValue(Expression expression)
		{
			var valueExpression = Expression.Lambda(expression).Compile();
			object value = valueExpression.DynamicInvoke();
			return value;
		}

		/// <summary>
		/// Retrieves the projection for the expression
		/// </summary>
		public static ProjectionInfo FindMemberProjection(Expression expression)
		{
			if (!IsMemberExpression(expression))
				return ProjectionInfo.ForProjection(Projections.Constant(FindValue(expression)));

			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null)
			{
				if (!IsConversion(unaryExpression.NodeType))
					throw new Exception("Cannot interpret member from " + expression);

				return FindMemberProjection(unaryExpression.Operand);
			}

			var methodCallExpression = expression as MethodCallExpression;
			if (methodCallExpression != null)
			{
				var signature = Signature(methodCallExpression.Method);
				Func<Expression, IProjection> processor;
				if (_customProjectionProcessors.TryGetValue(signature, out processor))
				{
					return ProjectionInfo.ForProjection(processor(methodCallExpression));
				}
			}
		    var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
			{
                var signature = Signature(memberExpression.Member);
				Func<Expression, IProjection> processor;
				if (_customProjectionProcessors.TryGetValue(signature, out processor))
				{
                    return ProjectionInfo.ForProjection(processor(memberExpression));
				}
			}

			return ProjectionInfo.ForProperty(FindMemberExpression(expression));
		}

		//http://stackoverflow.com/a/2509524/259946
		private static readonly Regex GeneratedMemberNameRegex = new Regex(@"^(CS\$)?<\w*>[1-9a-s]__[a-zA-Z]+[0-9]*$", RegexOptions.Compiled | RegexOptions.Singleline);

		private static bool IsCompilerGeneratedMemberExpressionOfCompilerGeneratedClass(Expression expression)
		{
			var memberExpression = expression as MemberExpression;
			if (memberExpression != null && memberExpression.Member.DeclaringType != null)
			{
				return Attribute.GetCustomAttribute(memberExpression.Member.DeclaringType, typeof(CompilerGeneratedAttribute)) != null 
					&& GeneratedMemberNameRegex.IsMatch(memberExpression.Member.Name);
			}

			return false;
		}

		/// <summary>
		/// Retrieves the name of the property from a member expression
		/// </summary>
		/// <param name="expression">An expression tree that can contain either a member, or a conversion from a member.
		/// If the member is referenced from a null valued object, then the container is treated as an alias.</param>
		/// <returns>The name of the member property</returns>
		public static string FindMemberExpression(Expression expression)
		{
			var memberExpression = expression as MemberExpression;
			if (memberExpression != null)
			{
				var parentExpression = memberExpression.Expression;
				if (parentExpression != null)
				{
					if (parentExpression.NodeType == ExpressionType.MemberAccess
						|| parentExpression.NodeType == ExpressionType.Call)
					{
						if (memberExpression.Member.DeclaringType.IsNullable())
						{
							// it's a Nullable<T>, so ignore any .Value
							if (memberExpression.Member.Name == "Value")
								return FindMemberExpression(parentExpression);
						}

						if (IsCompilerGeneratedMemberExpressionOfCompilerGeneratedClass(parentExpression))
						{
							return memberExpression.Member.Name;
						}

						return FindMemberExpression(parentExpression) + "." + memberExpression.Member.Name;
					}
					if (IsConversion(parentExpression.NodeType))
					{
						return (FindMemberExpression(parentExpression) + "." + memberExpression.Member.Name).TrimStart('.');
					}
				}

				return memberExpression.Member.Name;
			}

			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null)
			{
				if (!IsConversion(unaryExpression.NodeType))
					throw new Exception("Cannot interpret member from " + expression);

				return FindMemberExpression(unaryExpression.Operand);
			}

			var methodCallExpression = expression as MethodCallExpression;
			if (methodCallExpression != null)
			{
				if (methodCallExpression.Method.Name == "GetType")
					return ClassMember(methodCallExpression.Object);

				if (methodCallExpression.Method.Name == "get_Item")
					return FindMemberExpression(methodCallExpression.Object);

				if (methodCallExpression.Method.Name == "First")
					return FindMemberExpression(methodCallExpression.Arguments[0]);

				throw new Exception("Unrecognised method call in expression " + methodCallExpression);
			}

			if (expression is ParameterExpression)
				return "";

			throw new Exception("Could not determine member from " + expression);
		}

		/// <summary>
		/// Retrieves the name of the property from a member expression (without leading member access)
		/// </summary>
		public static string FindPropertyExpression(Expression expression)
		{
			string memberExpression = FindMemberExpression(expression);
			int periodPosition = memberExpression.LastIndexOf('.') + 1;
			string property = (periodPosition <= 0) ? memberExpression : memberExpression.Substring(periodPosition);
			return property;
		}

		/// <summary>
		/// Retrieves a detached criteria from an appropriate lambda expression
		/// </summary>
		/// <param name="expression">Expression for detached criteria using .As&lt;>() extension"/></param>
		/// <returns>Evaluated detached criteria</returns>
		public static DetachedCriteria FindDetachedCriteria(Expression expression)
		{
			var methodCallExpression = expression as MethodCallExpression;
			if (methodCallExpression == null)
				throw new Exception("right operand should be detachedQueryInstance.As<T>() - " + expression);

			var criteriaExpression = Expression.Lambda(methodCallExpression.Object).Compile();
			QueryOver detachedQuery = (QueryOver)criteriaExpression.DynamicInvoke();
			return detachedQuery.DetachedCriteria;
		}

		private static bool EvaluatesToNull(Expression expression)
		{
			var valueExpression = Expression.Lambda(expression).Compile();
			object value = valueExpression.DynamicInvoke();
			return (value == null);
		}

		private static System.Type FindMemberType(Expression expression)
		{
			var memberExpression = expression as MemberExpression;
			if (memberExpression != null)
			{
				return memberExpression.Type;
			}

			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null)
			{
				if (!IsConversion(unaryExpression.NodeType))
					throw new Exception("Cannot interpret member from " + expression);

				return FindMemberType(unaryExpression.Operand);
			}

			var methodCallExpression = expression as MethodCallExpression;
			if (methodCallExpression != null)
			{
				return methodCallExpression.Method.ReturnType;
			}

			throw new Exception("Could not determine member type from " + expression);
		}

		private static bool IsMemberExpression(Expression expression)
		{
			if (expression is ParameterExpression)
				return true;

			var memberExpression = expression as MemberExpression;
			if (memberExpression != null)
			{
				if (memberExpression.Expression == null)
					return false;  // it's a member of a static class

				if (IsMemberExpression(memberExpression.Expression))
					return true;

				// if the member has a null value, it was an alias
				return EvaluatesToNull(memberExpression.Expression);
			}

			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null)
			{
				if (!IsConversion(unaryExpression.NodeType))
					throw new Exception("Cannot interpret member from " + expression);

				return IsMemberExpression(unaryExpression.Operand);
			}

			var methodCallExpression = expression as MethodCallExpression;
			if (methodCallExpression != null)
			{
				string signature = Signature(methodCallExpression.Method);
				if (_customProjectionProcessors.ContainsKey(signature))
					return true;

				if (methodCallExpression.Method.Name == "First")
				{
					if (IsMemberExpression(methodCallExpression.Arguments[0]))
						return true;

					return EvaluatesToNull(methodCallExpression.Arguments[0]);
				}

				if (methodCallExpression.Method.Name == "GetType"
					|| methodCallExpression.Method.Name == "get_Item")
				{
					if (IsMemberExpression(methodCallExpression.Object))
						return true;

					return EvaluatesToNull(methodCallExpression.Object);
				}
			}

			return false;
		}

		private static bool IsConversion(ExpressionType expressionType)
		{
			return (expressionType == ExpressionType.Convert || expressionType == ExpressionType.ConvertChecked);
		}

		private static object ConvertType(object value, System.Type type)
		{
			if (value == null)
				return null;

			if (type.IsInstanceOfType(value))
				return value;

			type = type.UnwrapIfNullable();

			if (type.IsEnum)
				return Enum.ToObject(type, value);

			if (type.IsPrimitive)
				return Convert.ChangeType(value, type);

			throw new Exception(string.Format("Cannot convert '{0}' to {1}", value, type));
		}

		private static ICriterion ProcessSimpleExpression(BinaryExpression be)
		{
			if (be.Left.NodeType == ExpressionType.Call && ((MethodCallExpression)be.Left).Method.Name == "CompareString")
				return ProcessVisualBasicStringComparison(be);

			return ProcessSimpleExpression(be.Left, be.Right, be.NodeType);
		}

		private static ICriterion ProcessSimpleExpression(Expression left, Expression right, ExpressionType nodeType)
		{
			ProjectionInfo property = FindMemberProjection(left);
			System.Type propertyType = FindMemberType(left);

			object value = FindValue(right);
			value = ConvertType(value, propertyType);

			if (value == null)
				return ProcessSimpleNullExpression(property, nodeType);

			Func<ProjectionInfo, object, ICriterion> simpleExpressionCreator;
			if (!_simpleExpressionCreators.TryGetValue(nodeType, out simpleExpressionCreator))
				throw new Exception("Unhandled simple expression type: " + nodeType);

			return simpleExpressionCreator(property, value);
		}

		private static ICriterion ProcessVisualBasicStringComparison(BinaryExpression be)
		{
			var methodCall = (MethodCallExpression)be.Left;

			if (IsMemberExpression(methodCall.Arguments[1]))
				return ProcessMemberExpression(methodCall.Arguments[0], methodCall.Arguments[1], be.NodeType);
			else
				return ProcessSimpleExpression(methodCall.Arguments[0], methodCall.Arguments[1], be.NodeType);
		}

		private static ICriterion ProcessSimpleNullExpression(ProjectionInfo property, ExpressionType expressionType)
		{
			if (expressionType == ExpressionType.Equal)
				return property.CreateCriterion(Restrictions.IsNull, Restrictions.IsNull);

			if (expressionType == ExpressionType.NotEqual)
				return Restrictions.Not(
					property.CreateCriterion(Restrictions.IsNull, Restrictions.IsNull));

			throw new Exception("Cannot supply null value to operator " + expressionType);
		}

		private static ICriterion ProcessMemberExpression(BinaryExpression be)
		{
			return ProcessMemberExpression(be.Left, be.Right, be.NodeType);
		}

		private static ICriterion ProcessMemberExpression(Expression left, Expression right, ExpressionType nodeType)
		{
			ProjectionInfo leftProperty = FindMemberProjection(left);
			ProjectionInfo rightProperty = FindMemberProjection(right);

			Func<ProjectionInfo, ProjectionInfo, ICriterion> propertyExpressionCreator;
			if (!_propertyExpressionCreators.TryGetValue(nodeType, out propertyExpressionCreator))
				throw new Exception("Unhandled property expression type: " + nodeType);

			return propertyExpressionCreator(leftProperty, rightProperty);
		}

		private static ICriterion ProcessAndExpression(BinaryExpression expression)
		{
			return Restrictions.And(
				ProcessExpression(expression.Left),
				ProcessExpression(expression.Right));
		}

		private static ICriterion ProcessOrExpression(BinaryExpression expression)
		{
			return Restrictions.Or(
				ProcessExpression(expression.Left),
				ProcessExpression(expression.Right));
		}

		private static ICriterion ProcessBinaryExpression(BinaryExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.AndAlso:
					return ProcessAndExpression(expression);

				case ExpressionType.OrElse:
					return ProcessOrExpression(expression);

				case ExpressionType.Equal:
				case ExpressionType.NotEqual:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
					if (IsMemberExpression(expression.Right))
						return ProcessMemberExpression(expression);
					else
						return ProcessSimpleExpression(expression);

				default:
					throw new Exception("Unhandled binary expression: " + expression.NodeType + ", " + expression);
			}
		}

		private static ICriterion ProcessBooleanExpression(Expression expression)
		{
			if (expression is MemberExpression)
			{
				return Restrictions.Eq(FindMemberExpression(expression), true);
			}

			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null)
			{
				if (unaryExpression.NodeType != ExpressionType.Not)
					throw new Exception("Cannot interpret member from " + expression);

				if (IsMemberExpression(unaryExpression.Operand))
					return Restrictions.Eq(FindMemberExpression(unaryExpression.Operand), false);
				else
					return Restrictions.Not(ProcessExpression(unaryExpression.Operand));
			}

			var methodCallExpression = expression as MethodCallExpression;
			if (methodCallExpression != null)
			{
				return ProcessCustomMethodCall(methodCallExpression);
			}

			var typeBinaryExpression = expression as TypeBinaryExpression;
			if (typeBinaryExpression != null)
			{
				return Restrictions.Eq(ClassMember(typeBinaryExpression.Expression), typeBinaryExpression.TypeOperand.FullName);
			}

			throw new Exception("Could not determine member type from " + expression.NodeType + ", " + expression + ", " + expression.GetType());
		}

		private static string ClassMember(Expression expression)
		{
			if (expression.NodeType == ExpressionType.MemberAccess)
				return FindMemberExpression(expression) + ".class";
			
			return "class";
		}

		public static string Signature(MethodInfo methodInfo)
		{
			while (methodInfo.IsGenericMethod && !methodInfo.IsGenericMethodDefinition)
				methodInfo = methodInfo.GetGenericMethodDefinition();

			return methodInfo.DeclaringType.FullName
				+ ":" + methodInfo;
		}

		public static string Signature(MemberInfo memberInfo)
		{
		    return memberInfo.DeclaringType.FullName + ":" + memberInfo;
		}

		private static ICriterion ProcessCustomMethodCall(MethodCallExpression methodCallExpression)
		{
			string signature = Signature(methodCallExpression.Method);

			Func<MethodCallExpression, ICriterion> customMethodCallProcessor;
			if (!_customMethodCallProcessors.TryGetValue(signature, out customMethodCallProcessor))
				throw new Exception("Unrecognised method call: " + signature);

			return customMethodCallProcessor(methodCallExpression);
		}

		private static ICriterion ProcessExpression(Expression expression)
		{
			var binaryExpression = expression as BinaryExpression;
			if (binaryExpression != null)
				return ProcessBinaryExpression(binaryExpression);
			
			return ProcessBooleanExpression(expression);
		}

		private static ICriterion ProcessLambdaExpression(LambdaExpression expression)
		{
			return ProcessExpression(expression.Body);
		}

		/// <summary>
		/// Convert a lambda expression to NHibernate ICriterion
		/// </summary>
		/// <typeparam name="T">The type of the lambda expression</typeparam>
		/// <param name="expression">The lambda expression to convert</param>
		/// <returns>NHibernate ICriterion</returns>
		public static ICriterion ProcessExpression<T>(Expression<Func<T, bool>> expression)
		{
			return ProcessLambdaExpression(expression);
		}

		/// <summary>
		/// Convert a lambda expression to NHibernate ICriterion
		/// </summary>
		/// <param name="expression">The lambda expression to convert</param>
		/// <returns>NHibernate ICriterion</returns>
		public static ICriterion ProcessExpression(Expression<Func<bool>> expression)
		{
			return ProcessLambdaExpression(expression);
		}

		/// <summary>
		/// Convert a lambda expression to NHibernate Order
		/// </summary>
		/// <typeparam name="T">The type of the lambda expression</typeparam>
		/// <param name="expression">The lambda expression to convert</param>
		/// <param name="orderDelegate">The appropriate order delegate (order direction)</param>
		/// <returns>NHibernate Order</returns>
		public static Order ProcessOrder<T>(Expression<Func<T, object>> expression,
											Func<string, Order> orderDelegate)
		{
			string property = FindMemberExpression(expression.Body);
			Order order = orderDelegate(property);
			return order;
		}

		/// <summary>
		/// Convert a lambda expression to NHibernate Order
		/// </summary>
		/// <param name="expression">The lambda expression to convert</param>
		/// <param name="orderDelegate">The appropriate order delegate (order direction)</param>
		/// <returns>NHibernate Order</returns>
		public static Order ProcessOrder(Expression<Func<object>> expression,
											Func<string, Order> orderDelegate)
		{
			string property = FindMemberExpression(expression.Body);
			Order order = orderDelegate(property);
			return order;
		}

		/// <summary>
		/// Convert a lambda expression to NHibernate Order
		/// </summary>
		/// <param name="expression">The lambda expression to convert</param>
		/// <param name="orderDelegate">The appropriate order delegate (order direction)</param>
		/// <returns>NHibernate Order</returns>
		public static Order ProcessOrder(	LambdaExpression expression,
											Func<string, Order> orderDelegate)
		{
			string property = FindPropertyExpression(expression.Body);
			Order order = orderDelegate(property);
			return order;
		}

		/// <summary>
		/// Convert a lambda expression to NHibernate Order
		/// </summary>
		/// <param name="expression">The lambda expression to convert</param>
		/// <param name="orderStringDelegate">The appropriate order delegate (order direction)</param>
		/// <param name="orderProjectionDelegate">The appropriate order delegate (order direction)</param>
		/// <returns>NHibernate Order</returns>
		public static Order ProcessOrder(	LambdaExpression expression,
											Func<string, Order> orderStringDelegate,
											Func<IProjection, Order> orderProjectionDelegate)
		{
			ProjectionInfo projection = FindMemberProjection(expression.Body);
			Order order = projection.CreateOrder(orderStringDelegate, orderProjectionDelegate);
			return order;
		}

		private static AbstractCriterion ProcessSubqueryExpression(LambdaSubqueryType subqueryType,
																	BinaryExpression be)
		{
			string property = FindMemberExpression(be.Left);
			DetachedCriteria detachedCriteria = FindDetachedCriteria(be.Right);

			var subqueryExpressionCreators = _subqueryExpressionCreatorTypes[subqueryType];

			Func<string, DetachedCriteria, AbstractCriterion> subqueryExpressionCreator;
			if (!subqueryExpressionCreators.TryGetValue(be.NodeType, out subqueryExpressionCreator))
				throw new Exception("Unhandled subquery expression type: " + subqueryType + "," + be.NodeType);

			return subqueryExpressionCreator(property, detachedCriteria);
		}

		/// <summary>
		/// Convert a lambda expression to NHibernate subquery AbstractCriterion
		/// </summary>
		/// <typeparam name="T">type of member expression</typeparam>
		/// <param name="subqueryType">type of subquery</param>
		/// <param name="expression">lambda expression to convert</param>
		/// <returns>NHibernate.ICriterion.AbstractCriterion</returns>
		public static AbstractCriterion ProcessSubquery<T>(LambdaSubqueryType subqueryType,
															Expression<Func<T, bool>> expression)
		{
			BinaryExpression be = (BinaryExpression)expression.Body;
			AbstractCriterion criterion = ProcessSubqueryExpression(subqueryType, be);
			return criterion;
		}

		/// <summary>
		/// Convert a lambda expression to NHibernate subquery AbstractCriterion
		/// </summary>
		/// <param name="subqueryType">type of subquery</param>
		/// <param name="expression">lambda expression to convert</param>
		/// <returns>NHibernate.ICriterion.AbstractCriterion</returns>
		public static AbstractCriterion ProcessSubquery(LambdaSubqueryType subqueryType,
														Expression<Func<bool>> expression)
		{
			BinaryExpression be = (BinaryExpression)expression.Body;
			AbstractCriterion criterion = ProcessSubqueryExpression(subqueryType, be);
			return criterion;
		}

		/// <summary>
		/// Register a custom method for use in a QueryOver expression
		/// </summary>
		/// <param name="function">Lambda expression demonstrating call of custom method</param>
		/// <param name="functionProcessor">function to convert MethodCallExpression to ICriterion</param>
		public static void RegisterCustomMethodCall(Expression<Func<bool>> function, Func<MethodCallExpression, ICriterion> functionProcessor)
		{
			MethodCallExpression functionExpression = (MethodCallExpression)function.Body;
			string signature = Signature(functionExpression.Method);
			_customMethodCallProcessors.Add(signature, functionProcessor);
		}

		/// <summary>
		/// Register a custom projection for use in a QueryOver expression
		/// </summary>
		/// <param name="function">Lambda expression demonstrating call of custom method</param>
		/// <param name="functionProcessor">function to convert MethodCallExpression to IProjection</param>
		public static void RegisterCustomProjection<T>(Expression<Func<T>> function, Func<MethodCallExpression, IProjection> functionProcessor)
		{
			MethodCallExpression functionExpression = (MethodCallExpression)function.Body;
			string signature = Signature(functionExpression.Method);
		    _customProjectionProcessors.Add(signature, e => functionProcessor((MethodCallExpression) e));
		}

        /// <summary>
		/// Register a custom projection for use in a QueryOver expression
		/// </summary>
		/// <param name="function">Lambda expression demonstrating call of custom method</param>
		/// <param name="functionProcessor">function to convert MethodCallExpression to IProjection</param>
		public static void RegisterCustomProjection<T>(Expression<Func<T>> function, Func<MemberExpression, IProjection> functionProcessor)
        {
            MemberExpression functionExpression = (MemberExpression) function.Body;
            string signature = Signature(functionExpression.Member);
            _customProjectionProcessors.Add(signature, e => functionProcessor((MemberExpression) e));
		}
	}
}

