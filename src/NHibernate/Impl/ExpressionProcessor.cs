
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NHibernate.Criterion;

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

		private readonly static IDictionary<ExpressionType, Func<string, object, ICriterion>> _simpleExpressionCreators = null;
		private readonly static IDictionary<ExpressionType, Func<string, string, ICriterion>> _propertyExpressionCreators = null;
		private readonly static IDictionary<LambdaSubqueryType, IDictionary<ExpressionType, Func<string, DetachedCriteria, AbstractCriterion>>> _subqueryExpressionCreatorTypes = null;

		static ExpressionProcessor()
		{
			_simpleExpressionCreators = new Dictionary<ExpressionType, Func<string, object, ICriterion>>();
			_simpleExpressionCreators[ExpressionType.Equal] = Eq;
			_simpleExpressionCreators[ExpressionType.NotEqual] = Ne;
			_simpleExpressionCreators[ExpressionType.GreaterThan] = Gt;
			_simpleExpressionCreators[ExpressionType.GreaterThanOrEqual] = Ge;
			_simpleExpressionCreators[ExpressionType.LessThan] = Lt;
			_simpleExpressionCreators[ExpressionType.LessThanOrEqual] = Le;

			_propertyExpressionCreators = new Dictionary<ExpressionType, Func<string, string, ICriterion>>();
			_propertyExpressionCreators[ExpressionType.Equal] = Restrictions.EqProperty;
			_propertyExpressionCreators[ExpressionType.NotEqual] = Restrictions.NotEqProperty;
			_propertyExpressionCreators[ExpressionType.GreaterThan] = Restrictions.GtProperty;
			_propertyExpressionCreators[ExpressionType.GreaterThanOrEqual] = Restrictions.GeProperty;
			_propertyExpressionCreators[ExpressionType.LessThan] = Restrictions.LtProperty;
			_propertyExpressionCreators[ExpressionType.LessThanOrEqual] = Restrictions.LeProperty;

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
		}

		private static ICriterion Eq(string propertyName, object value)
		{
			return Restrictions.Eq(propertyName, value);
		}

		private static ICriterion Ne(string propertyName, object value)
		{
			return
				NHibernate.Criterion.Restrictions.Not(
					NHibernate.Criterion.Restrictions.Eq(propertyName, value));
		}

		private static ICriterion Gt(string propertyName, object value)
		{
			return NHibernate.Criterion.Restrictions.Gt(propertyName, value);
		}

		private static ICriterion Ge(string propertyName, object value)
		{
			return NHibernate.Criterion.Restrictions.Ge(propertyName, value);
		}

		private static ICriterion Lt(string propertyName, object value)
		{
			return NHibernate.Criterion.Restrictions.Lt(propertyName, value);
		}

		private static ICriterion Le(string propertyName, object value)
		{
			return NHibernate.Criterion.Restrictions.Le(propertyName, value);
		}

		/// <summary>
		/// Retrieves the name of the property from a member expression
		/// </summary>
		/// <param name="expression">An expression tree that can contain either a member, or a conversion from a member.
		/// If the member is referenced from a null valued object, then the container is treated as an alias.</param>
		/// <returns>The name of the member property</returns>
		public static string FindMemberExpression(Expression expression)
		{
			if (expression is MemberExpression)
			{
				MemberExpression memberExpression = (MemberExpression)expression;

				if (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
					return FindMemberExpression(memberExpression.Expression) + "." + memberExpression.Member.Name;
				else
					return memberExpression.Member.Name;
			}

			if (expression is UnaryExpression)
			{
				UnaryExpression unaryExpression = (UnaryExpression)expression;

				if (unaryExpression.NodeType != ExpressionType.Convert)
					throw new Exception("Cannot interpret member from " + expression.ToString());

				return FindMemberExpression(unaryExpression.Operand);
			}

			if (expression is MethodCallExpression)
			{
				MethodCallExpression methodCallExpression = (MethodCallExpression)expression;

				if (methodCallExpression.Method.Name == "GetType")
				{
					if (methodCallExpression.Object.NodeType == ExpressionType.MemberAccess)
						return FindMemberExpression(methodCallExpression.Object) + ".class";
					else
						return "class";
				}

				throw new Exception("Unrecognised method call in epression " + expression.ToString());
			}

			throw new Exception("Could not determine member from " + expression.ToString());
		}

		/// <summary>
		/// Retrieves a detached criteria from an appropriate lambda expression
		/// </summary>
		/// <param name="expression">Expresson for detached criteria using .As&lt;>() extension"/></param>
		/// <returns>Evaluated detached criteria</returns>
		public static DetachedCriteria FindDetachedCriteria(Expression expression)
		{
			MethodCallExpression methodCallExpression = expression as MethodCallExpression;

			if (methodCallExpression == null)
				throw new Exception("right operand should be detachedCriteriaInstance.As<T>() - " + expression.ToString());

			var criteriaExpression = Expression.Lambda(methodCallExpression.Arguments[0]).Compile();
			object detachedCriteria = criteriaExpression.DynamicInvoke();
			return (DetachedCriteria)detachedCriteria;
		}

		private static bool EvaluatesToNull(Expression expression)
		{
			var valueExpression = Expression.Lambda(expression).Compile();
			object value = valueExpression.DynamicInvoke();
			return (value == null);
		}

		private static System.Type FindMemberType(Expression expression)
		{
			if (expression is MemberExpression)
			{
				MemberExpression memberExpression = (MemberExpression)expression;

				return memberExpression.Type;
			}

			if (expression is UnaryExpression)
			{
				UnaryExpression unaryExpression = (UnaryExpression)expression;

				if (unaryExpression.NodeType != ExpressionType.Convert)
					throw new Exception("Cannot interpret member from " + expression.ToString());

				return FindMemberType(unaryExpression.Operand);
			}

			if (expression is MethodCallExpression)
			{
				return typeof(System.Type);
			}

			throw new Exception("Could not determine member type from " + expression.ToString());
		}

		private static bool IsMemberExpression(Expression expression)
		{
			if (expression is MemberExpression)
			{
				MemberExpression memberExpression = (MemberExpression)expression;

				if (memberExpression.Expression == null)
					return false;  // it's a member of a static class

				if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
					return true;

				if (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
				{
					// if the member has a null value, it was an alias
					if (EvaluatesToNull(memberExpression.Expression))
						return true;
				}
			}

			if (expression is UnaryExpression)
			{
				UnaryExpression unaryExpression = (UnaryExpression)expression;

				if (unaryExpression.NodeType != ExpressionType.Convert)
					throw new Exception("Cannot interpret member from " + expression.ToString());

				return IsMemberExpression(unaryExpression.Operand);
			}

			return false;
		}

		private static object ConvertType(object value, System.Type type)
		{
			if (value == null)
				return null;

			if (type.IsAssignableFrom(value.GetType()))
				return value;

			if (type.IsEnum)
				return Enum.ToObject(type, value);

			throw new Exception("Cannot convert '" + value.ToString() + "' to " + type.ToString());
		}

		private static ICriterion ProcessSimpleExpression(BinaryExpression be)
		{
			string property = FindMemberExpression(be.Left);
			System.Type propertyType = FindMemberType(be.Left);

			var valueExpression = Expression.Lambda(be.Right).Compile();
			object value = valueExpression.DynamicInvoke();
			value = ConvertType(value, propertyType);

			if (!_simpleExpressionCreators.ContainsKey(be.NodeType))
				throw new Exception("Unhandled simple expression type: " + be.NodeType);

			Func<string, object, ICriterion> simpleExpressionCreator = _simpleExpressionCreators[be.NodeType];
			ICriterion criterion = simpleExpressionCreator(property, value);
			return criterion;
		}

		private static ICriterion ProcessMemberExpression(BinaryExpression be)
		{
			string leftProperty = FindMemberExpression(be.Left);
			string rightProperty = FindMemberExpression(be.Right);

			if (!_propertyExpressionCreators.ContainsKey(be.NodeType))
				throw new Exception("Unhandled property expression type: " + be.NodeType);

			Func<string, string, ICriterion> propertyExpressionCreator = _propertyExpressionCreators[be.NodeType];
			ICriterion criterion = propertyExpressionCreator(leftProperty, rightProperty);
			return criterion;
		}

		private static ICriterion ProcessAndExpression(BinaryExpression expression)
		{
			return
				NHibernate.Criterion.Restrictions.And(
					ProcessExpression(expression.Left),
					ProcessExpression(expression.Right));
		}

		private static ICriterion ProcessOrExpression(BinaryExpression expression)
		{
			return
				NHibernate.Criterion.Restrictions.Or(
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
					throw new Exception("Unhandled binary expression: " + expression.NodeType + ", " + expression.ToString());
			}
		}

		private static ICriterion ProcessBooleanExpression(Expression expression)
		{
			if (expression is MemberExpression)
			{
				return Restrictions.Eq(FindMemberExpression(expression), true);
			}

			if (expression is UnaryExpression)
			{
				UnaryExpression unaryExpression = (UnaryExpression)expression;

				if (unaryExpression.NodeType != ExpressionType.Not)
					throw new Exception("Cannot interpret member from " + expression.ToString());

				return Restrictions.Eq(FindMemberExpression(unaryExpression.Operand), false);
			}

			throw new Exception("Could not determine member type from " + expression.ToString());
		}

		private static ICriterion ProcessExpression(Expression expression)
		{
			if (expression is BinaryExpression)
				return ProcessBinaryExpression((BinaryExpression)expression);
			else
				return ProcessBooleanExpression((Expression)expression);
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

		private static AbstractCriterion ProcessSubqueryExpression(LambdaSubqueryType subqueryType,
																	BinaryExpression be)
		{
			string property = FindMemberExpression(be.Left);
			DetachedCriteria detachedCriteria = FindDetachedCriteria(be.Right);

			var subqueryExpressionCreators = _subqueryExpressionCreatorTypes[subqueryType];

			if (!subqueryExpressionCreators.ContainsKey(be.NodeType))
				throw new Exception("Unhandled subquery expression type: " + subqueryType + "," + be.NodeType);

			Func<string, DetachedCriteria, AbstractCriterion> subqueryExpressionCreator = subqueryExpressionCreators[be.NodeType];
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

	}

}

