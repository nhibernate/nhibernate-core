using System;
using System.Collections;

namespace NHibernate.Criterion
{
	/// <summary>
	/// A factory for property-specific  AbstractCriterion  and projection instances
	/// </summary>
	[Serializable]
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

		public AbstractCriterion GtProperty(String other)
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


		public AbstractCriterion Eq(DetachedCriteria subselect)
		{
			return Subqueries.PropertyEq(PropertyName, subselect);
		}

		public AbstractCriterion Ne(DetachedCriteria subselect)
		{
			return Subqueries.PropertyNe(PropertyName, subselect);
		}

		public AbstractCriterion Lt(DetachedCriteria subselect)
		{
			return Subqueries.PropertyLt(PropertyName, subselect);
		}

		public AbstractCriterion Le(DetachedCriteria subselect)
		{
			return Subqueries.PropertyLe(PropertyName, subselect);
		}

		public AbstractCriterion Bt(DetachedCriteria subselect)
		{
			return Subqueries.PropertyGt(PropertyName, subselect);
		}

		public AbstractCriterion Ge(DetachedCriteria subselect)
		{
			return Subqueries.PropertyGe(PropertyName, subselect);
		}

		public AbstractCriterion NotIn(DetachedCriteria subselect)
		{
			return Subqueries.PropertyNotIn(PropertyName, subselect);
		}

		public AbstractCriterion In(DetachedCriteria subselect)
		{
			return Subqueries.PropertyIn(PropertyName, subselect);
		}

		public AbstractCriterion EqAll(DetachedCriteria subselect)
		{
			return Subqueries.PropertyEqAll(PropertyName, subselect);
		}

		public AbstractCriterion GtAll(DetachedCriteria subselect)
		{
			return Subqueries.PropertyGtAll(PropertyName, subselect);
		}

		public AbstractCriterion LtAll(DetachedCriteria subselect)
		{
			return Subqueries.PropertyLtAll(PropertyName, subselect);
		}

		public AbstractCriterion LeAll(DetachedCriteria subselect)
		{
			return Subqueries.PropertyLeAll(PropertyName, subselect);
		}

		public AbstractCriterion GeAll(DetachedCriteria subselect)
		{
			return Subqueries.PropertyGeAll(PropertyName, subselect);
		}

		public AbstractCriterion GtSome(DetachedCriteria subselect)
		{
			return Subqueries.PropertyGtSome(PropertyName, subselect);
		}

		public AbstractCriterion LtSome(DetachedCriteria subselect)
		{
			return Subqueries.PropertyLtSome(PropertyName, subselect);
		}

		public AbstractCriterion LeSome(DetachedCriteria subselect)
		{
			return Subqueries.PropertyLeSome(PropertyName, subselect);
		}

		public AbstractCriterion GeSome(DetachedCriteria subselect)
		{
			return Subqueries.PropertyGeSome(PropertyName, subselect);
		}
	}
}