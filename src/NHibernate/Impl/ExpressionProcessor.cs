using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.Type;
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
			/// Note:  throws if the supplied IProjection is not a IPropertyProjection
			/// </summary>
			public string AsProperty()
			{
				if (_property != null) return _property;

				var propertyProjection = _projection as IPropertyProjection;
				if (propertyProjection == null) throw new InvalidOperationException("Cannot determine property for " + _projection);
				return propertyProjection.PropertyName;
			}

			internal bool IsConstant(out ConstantProjection value) => (value = _projection as ConstantProjection) != null;
		}

		private static readonly Dictionary<ExpressionType, Func<ProjectionInfo, object, ICriterion>> _simpleExpressionCreators;
		private static readonly Dictionary<ExpressionType, Func<ProjectionInfo, ProjectionInfo, ICriterion>> _propertyExpressionCreators;
		private static readonly Dictionary<LambdaSubqueryType, IDictionary<ExpressionType, Func<string, DetachedCriteria, AbstractCriterion>>> _subqueryExpressionCreatorTypes;
		private static readonly Dictionary<string, Func<MethodCallExpression, ICriterion>> _customMethodCallProcessors;
		private static readonly Dictionary<string, Func<Expression, IProjection>> _customProjectionProcessors;
		private static readonly Dictionary<ExpressionType, ISQLFunction> _binaryArithmethicTemplates = new Dictionary<ExpressionType, ISQLFunction>();
		private static readonly ISQLFunction _unaryNegateTemplate;

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
			RegisterCustomMethodCall(() => RestrictionExtensions.IsIn(null, Array.Empty<object>()), RestrictionExtensions.ProcessIsInArray);
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
			RegisterCustomProjection(() => ProjectionsExtensions.AsEntity(default(object)), ProjectionsExtensions.ProcessAsEntity);

			RegisterBinaryArithmeticExpression(ExpressionType.Add, "+");
			RegisterBinaryArithmeticExpression(ExpressionType.Subtract, "-");
			RegisterBinaryArithmeticExpression(ExpressionType.Multiply, "*");
			RegisterBinaryArithmeticExpression(ExpressionType.Divide, "/");
			_unaryNegateTemplate = new VarArgsSQLFunction("(-", string.Empty, ")");
		}

		private static void RegisterBinaryArithmeticExpression(ExpressionType type, string sqlOperand)
		{
			_binaryArithmethicTemplates[type] = new VarArgsSQLFunction("(", sqlOperand, ")");
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
		/// Walk or Invoke expression to extract its runtime value
		/// </summary>
		public static object FindValue(Expression expression)
		{
			object value;
			switch (expression.NodeType)
			{
				case ExpressionType.Constant:
					var constantExpression = (ConstantExpression) expression;
					return constantExpression.Value;
				case ExpressionType.MemberAccess:
					var memberExpression = (MemberExpression) expression;
					value = memberExpression.Expression != null ? FindValue(memberExpression.Expression) : null;

					switch (memberExpression.Member.MemberType)
					{
						case MemberTypes.Field:
							return ((FieldInfo) memberExpression.Member).GetValue(value);
						case MemberTypes.Property:
							return ((PropertyInfo) memberExpression.Member).GetValue(value);
					}
					break;
				case ExpressionType.Call:
					var methodCallExpression = (MethodCallExpression) expression;
					var args = new object[methodCallExpression.Arguments.Count];
					for (int i = 0; i < args.Length; i++)
						args[i] = FindValue(methodCallExpression.Arguments[i]);

					if (methodCallExpression.Object == null) //extension or static method
					{
						return methodCallExpression.Method.Invoke(null, args);
					}
					else
					{
						var callingObject = FindValue(methodCallExpression.Object);

						return methodCallExpression.Method.Invoke(callingObject, args);
					}
				case ExpressionType.Convert:
					var unaryExpression = (UnaryExpression) expression;
					if ((Nullable.GetUnderlyingType(unaryExpression.Type) ?? unaryExpression.Type) == unaryExpression.Operand.Type
						|| unaryExpression.Type == typeof(object))
					{
						return FindValue(unaryExpression.Operand);
					}
					else if (unaryExpression.Method != null && unaryExpression.Type != unaryExpression.Operand.Type)
					{
						return unaryExpression.Method.Invoke(null, new[] { FindValue(unaryExpression.Operand) });
					}
					else if (unaryExpression.Type == (Nullable.GetUnderlyingType(unaryExpression.Operand.Type) ?? unaryExpression.Operand.Type))
					{
						value = FindValue(unaryExpression.Operand);
						if (value != null || Nullable.GetUnderlyingType(unaryExpression.Type) != null)
						{
							return value;
						}
					}
					break;
			}

			var lambdaExpression = Expression.Lambda(expression).Compile(true);
			value = lambdaExpression.DynamicInvoke();
			return value;
		}

		/// <summary>
		/// Retrieves the projection for the expression
		/// </summary>
		public static ProjectionInfo FindMemberProjection(Expression expression)
		{
			if (!IsMemberExpression(expression))
				return AsArithmeticProjection(expression)
					?? ProjectionInfo.ForProjection(Projections.Constant(FindValue(expression), NHibernateUtil.GuessType(expression.Type)));

			var unwrapExpression = UnwrapConvertExpression(expression);
			if (unwrapExpression != null)
			{
				return FindMemberProjection(unwrapExpression);
			}

			if (expression.NodeType == ExpressionType.Call)
			{
				var methodCallExpression = (MethodCallExpression) expression;
				var signature = Signature(methodCallExpression.Method);
				if (_customProjectionProcessors.TryGetValue(signature, out var processor))
				{
					return ProjectionInfo.ForProjection(processor(methodCallExpression));
				}
			}
			if (expression.NodeType == ExpressionType.MemberAccess)
			{
				var memberExpression = (MemberExpression) expression;
				var signature = Signature(memberExpression.Member);
				if (_customProjectionProcessors.TryGetValue(signature, out var processor))
				{
					return ProjectionInfo.ForProjection(processor(memberExpression));
				}
			}

			return ProjectionInfo.ForProperty(FindMemberExpression(expression));
		}

		private static Expression UnwrapConvertExpression(Expression expression)
		{
			return IsConversion(expression.NodeType)
				? ((UnaryExpression) expression).Operand
				: null;
		}

		private static ProjectionInfo AsArithmeticProjection(Expression expression)
		{
			if (expression.NodeType == ExpressionType.Negate)
			{
				var unary = (UnaryExpression) expression;
				return ProjectionInfo.ForProjection(
					new SqlFunctionProjection(_unaryNegateTemplate, TypeFactory.HeuristicType(unary.Type), FindMemberProjection(unary.Operand).AsProjection()));
			}

			if (!_binaryArithmethicTemplates.TryGetValue(expression.NodeType, out var template))
			{
				var unwrapExpression = UnwrapConvertExpression(expression);
				return unwrapExpression != null ? AsArithmeticProjection(unwrapExpression) : null;
			}

			var be = (BinaryExpression) expression;
			return ProjectionInfo.ForProjection(
				new SqlFunctionProjection(
					template,
					TypeFactory.HeuristicType(be.Type),
					FindMemberProjection(be.Left).AsProjection(),
					FindMemberProjection(be.Right).AsProjection()));
		}

		//http://stackoverflow.com/a/2509524/259946
		private static readonly Regex GeneratedMemberNameRegex = new Regex(@"^(CS\$)?<\w*>[1-9a-s]__[a-zA-Z]+[0-9]*$", RegexOptions.Compiled | RegexOptions.Singleline);

		private static bool IsCompilerGeneratedMemberExpressionOfCompilerGeneratedClass(Expression expression)
		{
			if (expression.NodeType != ExpressionType.MemberAccess)
				return false;

			var memberExpression = (MemberExpression) expression;
			if (memberExpression.Member.DeclaringType != null)
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
			switch (expression.NodeType)
			{
				case ExpressionType.MemberAccess:
				{
					var memberExpression = (MemberExpression) expression;
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

				case ExpressionType.Call:
				{
					var methodCallExpression = (MethodCallExpression) expression;

					switch (methodCallExpression.Method.Name)
					{
						case "GetType":
							return ClassMember(methodCallExpression.Object);
						case "get_Item":
							return FindMemberExpression(methodCallExpression.Object);
						case "First":
							return FindMemberExpression(methodCallExpression.Arguments[0]);
					}

					throw new ArgumentException("Unrecognised method call in expression " + methodCallExpression, nameof(expression));
				}

				case ExpressionType.Parameter:
					return string.Empty;
			}

			var unwrapExpression = UnwrapConvertExpression(expression);
			if (unwrapExpression != null)
				return FindMemberExpression(unwrapExpression);

			throw new ArgumentException("Could not determine member from " + expression, nameof(expression));
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
			if (expression.NodeType != ExpressionType.Call)
				throw new ArgumentException("right operand should be detachedQueryInstance.As<T>() - " + expression, nameof(expression));

			var methodCallExpression = (MethodCallExpression) expression;
			return ((QueryOver) FindValue(methodCallExpression.Object)).DetachedCriteria;
		}

		private static bool EvaluatesToNull(Expression expression)
		{
			return FindValue(expression) == null;
		}

		private static System.Type FindMemberType(Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.MemberAccess:
					return expression.Type;
				case ExpressionType.Call:
					return ((MethodCallExpression) expression).Method.ReturnType;
			}

			var unwrapExpression = UnwrapConvertExpression(expression);
			if (unwrapExpression != null)
			{
				return FindMemberType(unwrapExpression);
			}

			if (expression is UnaryExpression || expression is BinaryExpression)
				return expression.Type;

			throw new ArgumentException("Could not determine member type from " + expression, nameof(expression));
		}

		private static bool IsMemberExpression(Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Parameter:
					return true;

				case ExpressionType.MemberAccess:
					var expr = ((MemberExpression) expression).Expression;
					return expr != null && // it's not a member of a static class
							IsMemberExpressionOrAlias(expr);

				case ExpressionType.Call:
				{
					var methodCallExpression = (MethodCallExpression) expression;

					string signature = Signature(methodCallExpression.Method);
					if (_customProjectionProcessors.ContainsKey(signature))
						return true;

					switch (methodCallExpression.Method.Name)
					{
						case "First":
							return IsMemberExpressionOrAlias(methodCallExpression.Arguments[0]);
						case "GetType":
						case "get_Item":
							return IsMemberExpressionOrAlias(methodCallExpression.Object);
					}

					return false;
				}
			}

			var unwrapExpression = UnwrapConvertExpression(expression);
			return unwrapExpression != null && IsMemberExpression(unwrapExpression);
		}

		private static bool IsMemberExpressionOrAlias(Expression expr)
		{
			return IsMemberExpression(expr) ||
					// if the member has a null value, it was an alias
					EvaluatesToNull(expr);
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

			throw new ArgumentException(string.Format("Cannot convert '{0}' to {1}", value, type));
		}

		private static ICriterion ProcessSimpleExpression(Expression left, TypedValue rightValue, ExpressionType nodeType)
		{
			ProjectionInfo property = FindMemberProjection(left);
			System.Type propertyType = FindMemberType(left);

			var value = ConvertType(rightValue.Value, propertyType);

			if (value == null)
				return ProcessSimpleNullExpression(property, nodeType);

			Func<ProjectionInfo, object, ICriterion> simpleExpressionCreator;
			if (!_simpleExpressionCreators.TryGetValue(nodeType, out simpleExpressionCreator))
				throw new InvalidOperationException("Unhandled simple expression type: " + nodeType);

			return simpleExpressionCreator(property, value);
		}

		private static ICriterion ProcessAsVisualBasicStringComparison(Expression left, ExpressionType nodeType)
		{
			if (left.NodeType != ExpressionType.Call)
			{
				return null;
			}

			var methodCall = (MethodCallExpression) left;
			return methodCall.Method.Name == "CompareString"
				? ProcessMemberExpression(methodCall.Arguments[0], methodCall.Arguments[1], nodeType)
				: null;
		}

		private static ICriterion ProcessSimpleNullExpression(ProjectionInfo property, ExpressionType expressionType)
		{
			if (expressionType == ExpressionType.Equal)
				return property.CreateCriterion(Restrictions.IsNull, Restrictions.IsNull);

			if (expressionType == ExpressionType.NotEqual)
				return Restrictions.Not(
					property.CreateCriterion(Restrictions.IsNull, Restrictions.IsNull));

			throw new ArgumentException("Cannot supply null value to operator " + expressionType, nameof(expressionType));
		}

		private static ICriterion ProcessMemberExpression(Expression left, Expression right, ExpressionType nodeType)
		{
			ProjectionInfo rightProperty = FindMemberProjection(right);
			if (rightProperty.IsConstant(out var constProjection))
			{
				return ProcessAsVisualBasicStringComparison(left, nodeType)
						?? ProcessSimpleExpression(left, constProjection.TypedValue, nodeType);
			}

			ProjectionInfo leftProperty = FindMemberProjection(left);
			Func<ProjectionInfo, ProjectionInfo, ICriterion> propertyExpressionCreator;
			if (!_propertyExpressionCreators.TryGetValue(nodeType, out propertyExpressionCreator))
				throw new InvalidOperationException("Unhandled property expression type: " + nodeType);

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
					return ProcessMemberExpression(expression.Left, expression.Right, expression.NodeType);
				default:
					throw new NotImplementedException("Unhandled binary expression: " + expression.NodeType + ", " + expression);
			}
		}

		private static ICriterion ProcessBooleanExpression(Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.MemberAccess:
					return Restrictions.Eq(FindMemberExpression(expression), true);

				case ExpressionType.Not:
				{
					var unaryExpression = (UnaryExpression) expression;
					return IsMemberExpression(unaryExpression.Operand)
						? Restrictions.Eq(FindMemberExpression(unaryExpression.Operand), false)
						: Restrictions.Not(ProcessExpression(unaryExpression.Operand));
				}

				case ExpressionType.Call:
					return ProcessCustomMethodCall((MethodCallExpression) expression);

				case ExpressionType.TypeIs:
				{
					var tbe = (TypeBinaryExpression) expression;
					return Restrictions.Eq(ClassMember(tbe.Expression), tbe.TypeOperand.FullName);
				}
			}

			throw new ArgumentException(
				"Could not determine member type from " + expression.NodeType + ", " + expression + ", " + expression.GetType(),
				nameof(expression));
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
				throw new InvalidOperationException("Unrecognised method call: " + signature);

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
				throw new InvalidOperationException("Unhandled subquery expression type: " + subqueryType + "," + be.NodeType);

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
