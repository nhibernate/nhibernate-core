
using System;
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

			public LambdaBetweenBuilder(string propertyName, object lo)
			{
				this.propertyName = propertyName;
				this.lo = lo;
			}

			public AbstractCriterion And(object hi)
			{
				return Restrictions.Between(propertyName, lo, hi);
			}
		}

		private string propertyName;

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
			return new LambdaBetweenBuilder(propertyName, lo);
		}

		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public AbstractCriterion IsInsensitiveLike(object value)
		{
			return Restrictions.InsensitiveLike(propertyName, value);
		}
		
		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public AbstractCriterion IsInsensitiveLike(string value, MatchMode matchMode)
		{
			return Restrictions.InsensitiveLike(propertyName, value, matchMode);
		}

		/// <summary>
		/// Apply an "is empty" constraint to the named property
		/// </summary>
		public AbstractEmptinessExpression IsEmpty
		{
			get { return Restrictions.IsEmpty(propertyName); }
		}

		/// <summary>
		/// Apply a "not is empty" constraint to the named property
		/// </summary>
		public AbstractEmptinessExpression IsNotEmpty
		{
			get { return Restrictions.IsNotEmpty(propertyName); }
		}

		/// <summary>
		/// Apply an "is null" constraint to the named property
		/// </summary>
		public AbstractCriterion IsNull
		{
			get { return Restrictions.IsNull(propertyName); }
		}

		/// <summary>
		/// Apply an "not is null" constraint to the named property
		/// </summary>
		public AbstractCriterion IsNotNull
		{
			get { return Restrictions.IsNotNull(propertyName); }
		}

		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public SimpleExpression IsLike(object value)
		{
			return Restrictions.Like(propertyName, value);
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public SimpleExpression IsLike(string value, MatchMode matchMode)
		{
			return Restrictions.Like(propertyName, value, matchMode);
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public AbstractCriterion IsLike(string value, MatchMode matchMode, char? escapeChar)
		{
			return Restrictions.Like(propertyName, value, matchMode, escapeChar);
		}
		
	}

}
