using System;
using System.Collections;

namespace NHibernate.Expression
{
	/// <summary>
	/// A factory for property-specific  AbstractCriterion  and projection instances
	/// </summary>
	public class Property : PropertyProjection
	{
		internal Property(String propertyName) : base(propertyName)
		{
		}

		public AbstractCriterion Between(Object min, Object max)
		{
			return Expression.Between(PropertyName, min, max);
		}

		public AbstractCriterion In(ICollection values)
		{
			return Expression.In(PropertyName, values);
		}

		public AbstractCriterion In(Object[] values)
		{
			return Expression.In(PropertyName, values);
		}

		public AbstractCriterion Like(Object value)
		{
			return Expression.Like(PropertyName, value);
		}

		public AbstractCriterion Like(String value, MatchMode matchMode)
		{
			return Expression.Like(PropertyName, value, matchMode);
		}

		public AbstractCriterion Eq(Object value)
		{
			return Expression.Eq(PropertyName, value);
		}

		public AbstractCriterion Gt(Object value)
		{
			return Expression.Gt(PropertyName, value);
		}

		public AbstractCriterion Lt(Object value)
		{
			return Expression.Lt(PropertyName, value);
		}

		public AbstractCriterion Le(Object value)
		{
			return Expression.Le(PropertyName, value);
		}

		public AbstractCriterion Ge(Object value)
		{
			return Expression.Ge(PropertyName, value);
		}

		public AbstractCriterion EqProperty(Property other)
		{
			return Expression.EqProperty(PropertyName, other.PropertyName);
		}

		public AbstractCriterion NotEqProperty(Property other)
		{
			return Expression.NotEqProperty(PropertyName, other.PropertyName);
		}

		public AbstractCriterion LeProperty(Property other)
		{
			return Expression.LeProperty(PropertyName, other.PropertyName);
		}

		public AbstractCriterion GeProperty(Property other)
		{
			return Expression.GeProperty(PropertyName, other.PropertyName);
		}

		public AbstractCriterion LtProperty(Property other)
		{
			return Expression.LtProperty(PropertyName, other.PropertyName);
		}

		public AbstractCriterion GtProperty(Property other)
		{
			return Expression.GtProperty(PropertyName, other.PropertyName);
		}

		public AbstractCriterion EqProperty(String other)
		{
			return Expression.EqProperty(PropertyName, other);
		}

		public AbstractCriterion NotEqProperty(String other)
		{
			return Expression.NotEqProperty(PropertyName, other);
		}

		public AbstractCriterion LeProperty(String other)
		{
			return Expression.LeProperty(PropertyName, other);
		}

		public AbstractCriterion GeProperty(String other)
		{
			return Expression.GeProperty(PropertyName, other);
		}

		public AbstractCriterion LtProperty(String other)
		{
			return Expression.LtProperty(PropertyName, other);
		}

		public AbstractCriterion gtProperty(String other)
		{
			return Expression.GtProperty(PropertyName, other);
		}

		public AbstractCriterion IsNull()
		{
			return Expression.IsNull(PropertyName);
		}

		public AbstractCriterion IsNotNull()
		{
			return Expression.IsNotNull(PropertyName);
		}

		public AbstractEmptinessExpression IsEmpty()
		{
			return Expression.IsEmpty(PropertyName);
		}

		public AbstractEmptinessExpression IsNotEmpty()
		{
			return Expression.IsNotEmpty(PropertyName);
		}

		public CountProjection Count()
		{
			return Projections.Count(PropertyName);
		}

		public AggregateProjection Max()
		{
			return Projections.Max(PropertyName);
		}

		public AggregateProjection Min()
		{
			return Projections.Min(PropertyName);
		}

		public AggregateProjection Avg()
		{
			return Projections.Avg(PropertyName);
		}

		public PropertyProjection Group()
		{
			return Projections.GroupProperty(PropertyName);
		}

		public Order Asc()
		{
			return Order.Asc(PropertyName);
		}

		public Order Desc()
		{
			return Order.Desc(PropertyName);
		}

		public static Property ForName(String propertyName)
		{
			return new Property(propertyName);
		}

		/// <summary>
		/// Get a component attribute of this property
		/// </summary>
		public Property GetProperty(String propertyName)
		{
			return ForName(PropertyName + '.' + propertyName);
		}


		public AbstractCriterion Eq(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyEq(PropertyName, subseLect);
		}

		public AbstractCriterion Ne(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyNe(PropertyName, subseLect);
		}

		public AbstractCriterion Lt(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyLt(PropertyName, subseLect);
		}

		public AbstractCriterion Le(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyLe(PropertyName, subseLect);
		}

		public AbstractCriterion gt(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyGt(PropertyName, subseLect);
		}

		public AbstractCriterion Ge(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyGe(PropertyName, subseLect);
		}

		public AbstractCriterion notIn(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyNotIn(PropertyName, subseLect);
		}

		public AbstractCriterion In(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyIn(PropertyName, subseLect);
		}

		public AbstractCriterion EqAll(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyEqAll(PropertyName, subseLect);
		}

		public AbstractCriterion gtAll(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyGtAll(PropertyName, subseLect);
		}

		public AbstractCriterion LtAll(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyLtAll(PropertyName, subseLect);
		}

		public AbstractCriterion LeAll(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyLeAll(PropertyName, subseLect);
		}

		public AbstractCriterion GeAll(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyGeAll(PropertyName, subseLect);
		}

		public AbstractCriterion gtSome(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyGtSome(PropertyName, subseLect);
		}

		public AbstractCriterion LtSome(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyLtSome(PropertyName, subseLect);
		}

		public AbstractCriterion LeSome(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyLeSome(PropertyName, subseLect);
		}

		public AbstractCriterion GeSome(DetachedCriteria subseLect)
		{
			return Subqueries.PropertyGeSome(PropertyName, subseLect);
		}
	}
}