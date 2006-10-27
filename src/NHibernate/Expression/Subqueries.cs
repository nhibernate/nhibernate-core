using System;

namespace NHibernate.Expression
{
	/// <summary>
	/// Factory class for AbstractCriterion instances that represent 
	/// involving subqueries.
	/// <c>Expression</c>
	/// <c>Projection</c>
	/// <c>AbstractCriterion</c>
	/// </summary>
	public class Subqueries
	{
		public static AbstractCriterion Exists(DetachedCriteria dc)
		{
			return new ExistsSubqueryExpression("exists", dc);
		}

		public static AbstractCriterion NotExists(DetachedCriteria dc)
		{
			return new ExistsSubqueryExpression("not exists", dc);
		}

		public static AbstractCriterion PropertyEqAll(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, "=", "all", dc);
		}

		public static AbstractCriterion PropertyIn(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, "in", null, dc);
		}

		public static AbstractCriterion PropertyNotIn(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, "not in", null, dc);
		}

		public static AbstractCriterion PropertyEq(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, "=", null, dc);
		}

		public static AbstractCriterion PropertyNe(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, "<>", null, dc);
		}

		public static AbstractCriterion PropertyGt(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, ">", null, dc);
		}

		public static AbstractCriterion PropertyLt(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, "<", null, dc);
		}

		public static AbstractCriterion PropertyGe(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, ">=", null, dc);
		}

		public static AbstractCriterion PropertyLe(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, "<=", null, dc);
		}

		public static AbstractCriterion PropertyGtAll(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, ">", "all", dc);
		}

		public static AbstractCriterion PropertyLtAll(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, "<", "all", dc);
		}

		public static AbstractCriterion PropertyGeAll(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, ">=", "all", dc);
		}

		public static AbstractCriterion PropertyLeAll(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, "<=", "all", dc);
		}

		public static AbstractCriterion PropertyGtSome(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, ">", "some", dc);
		}

		public static AbstractCriterion PropertyLtSome(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, "<", "some", dc);
		}

		public static AbstractCriterion PropertyGeSome(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, ">=", "some", dc);
		}

		public static AbstractCriterion PropertyLeSome(String propertyName, DetachedCriteria dc)
		{
			return new PropertySubqueryExpression(propertyName, "<=", "some", dc);
		}

		public static AbstractCriterion EqAll(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, "=", "all", dc);
		}

		public static AbstractCriterion In(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, "in", null, dc);
		}

		public static AbstractCriterion NotIn(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, "not in", null, dc);
		}

		public static AbstractCriterion Eq(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, "=", null, dc);
		}

		public static AbstractCriterion Gt(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, ">", null, dc);
		}

		public static AbstractCriterion Lt(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, "<", null, dc);
		}

		public static AbstractCriterion Ge(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, ">=", null, dc);
		}

		public static AbstractCriterion Le(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, "<=", null, dc);
		}

		public static AbstractCriterion Ne(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, "<>", null, dc);
		}

		public static AbstractCriterion GtAll(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, ">", "all", dc);
		}

		public static AbstractCriterion LtAll(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, "<", "all", dc);
		}

		public static AbstractCriterion GeAll(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, ">=", "all", dc);
		}

		public static AbstractCriterion LeAll(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, "<=", "all", dc);
		}

		public static AbstractCriterion GtSome(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, ">", "some", dc);
		}

		public static AbstractCriterion LtSome(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, "<", "some", dc);
		}

		public static AbstractCriterion GeSome(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, ">=", "some", dc);
		}

		public static AbstractCriterion LeSome(Object value, DetachedCriteria dc)
		{
			return new SimpleSubqueryExpression(value, "<=", "some", dc);
		}
	}
}