
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class LambdaRestrictionBuilder
	{
		public class LambdaBetweenBuilder
		{
			private string propertyName;
			private object lo;
			private bool isNot;

			public LambdaBetweenBuilder(string propertyName, object lo, bool isNot)
			{
				this.propertyName = propertyName;
				this.lo = lo;
				this.isNot = isNot;
			}

			public AbstractCriterion And(object hi)
			{
				if (isNot)
					return Restrictions.Not(Restrictions.Between(propertyName, lo, hi));

				return Restrictions.Between(propertyName, lo, hi);
			}
		}

		private string propertyName;
		private bool isNot;

		private AbstractCriterion Process(AbstractCriterion criterion)
		{
			if (isNot)
				return Restrictions.Not(criterion);

			return criterion;
		}

		/// <summary>
		/// Constructed with property name
		/// </summary>
		public LambdaRestrictionBuilder(string propertyName)
		{
			this.propertyName = propertyName;
		}

		/// <summary>
		/// Apply a "between" constraint to the named property
		/// </summary>
		public LambdaBetweenBuilder IsBetween(object lo)
		{
			return new LambdaBetweenBuilder(propertyName, lo, isNot);
		}

		public LambdaRestrictionBuilder Not
		{
			get
			{
				isNot = !isNot;
				return this;
			}
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public AbstractCriterion IsIn(ICollection values)
		{
			return Process(Restrictions.In(propertyName, values));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public AbstractCriterion IsIn(object[] values)
		{
			return Process(Restrictions.In(propertyName, values));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public AbstractCriterion IsInG<T>(ICollection<T> values)
		{
			return Process(Restrictions.InG(propertyName, values));
		}

		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public AbstractCriterion IsInsensitiveLike(object value)
		{
			return Process(Restrictions.InsensitiveLike(propertyName, value));
		}
		
		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public AbstractCriterion IsInsensitiveLike(string value, MatchMode matchMode)
		{
			return Process(Restrictions.InsensitiveLike(propertyName, value, matchMode));
		}

		/// <summary>
		/// Apply an "is empty" constraint to the named property
		/// </summary>
		public AbstractCriterion IsEmpty
		{
			get { return Process(Restrictions.IsEmpty(propertyName)); }
		}

		/// <summary>
		/// Apply a "not is empty" constraint to the named property
		/// </summary>
		public AbstractCriterion IsNotEmpty
		{
			get { return Process(Restrictions.IsNotEmpty(propertyName)); }
		}

		/// <summary>
		/// Apply an "is null" constraint to the named property
		/// </summary>
		public AbstractCriterion IsNull
		{
			get { return Process(Restrictions.IsNull(propertyName)); }
		}

		/// <summary>
		/// Apply an "not is null" constraint to the named property
		/// </summary>
		public AbstractCriterion IsNotNull
		{
			get { return Process(Restrictions.IsNotNull(propertyName)); }
		}

		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public AbstractCriterion IsLike(object value)
		{
			return Process(Restrictions.Like(propertyName, value));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public AbstractCriterion IsLike(string value, MatchMode matchMode)
		{
			return Process(Restrictions.Like(propertyName, value, matchMode));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public AbstractCriterion IsLike(string value, MatchMode matchMode, char? escapeChar)
		{
			return Process(Restrictions.Like(propertyName, value, matchMode, escapeChar));
		}
		
	}

}
