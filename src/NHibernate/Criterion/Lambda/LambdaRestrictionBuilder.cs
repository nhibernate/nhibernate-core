
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
			private IProjection projection;
			private object lo;
			private bool isNot;

			public LambdaBetweenBuilder(IProjection projection, object lo, bool isNot)
			{
				this.projection = projection;
				this.lo = lo;
				this.isNot = isNot;
			}

			public AbstractCriterion And(object hi)
			{
				if (isNot)
					return Restrictions.Not(Restrictions.Between(projection, lo, hi));

				return Restrictions.Between(projection, lo, hi);
			}
		}

		private IProjection projection;
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
		public LambdaRestrictionBuilder(IProjection projection)
		{
			this.projection = projection;
		}

		/// <summary>
		/// Apply a "between" constraint to the named property
		/// </summary>
		public LambdaBetweenBuilder IsBetween(object lo)
		{
			return new LambdaBetweenBuilder(projection, lo, isNot);
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
			return Process(Restrictions.In(projection, values));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public AbstractCriterion IsIn(object[] values)
		{
			return Process(Restrictions.In(projection, values));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public AbstractCriterion IsInG<T>(ICollection<T> values)
		{
			return Process(Restrictions.InG(projection, values));
		}

		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public AbstractCriterion IsInsensitiveLike(object value)
		{
			return Process(Restrictions.InsensitiveLike(projection, value));
		}
		
		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public AbstractCriterion IsInsensitiveLike(string value, MatchMode matchMode)
		{
			return Process(Restrictions.InsensitiveLike(projection, value, matchMode));
		}

		/// <summary>
		/// Apply an "is empty" constraint to the named property
		/// </summary>
		public AbstractCriterion IsEmpty
		{
			get { return Process(Restrictions.IsEmpty(ExpressionProcessor.FindProperty(projection))); }
		}

		/// <summary>
		/// Apply a "not is empty" constraint to the named property
		/// </summary>
		public AbstractCriterion IsNotEmpty
		{
			get { return Process(Restrictions.IsNotEmpty(ExpressionProcessor.FindProperty(projection))); }
		}

		/// <summary>
		/// Apply an "is null" constraint to the named property
		/// </summary>
		public AbstractCriterion IsNull
		{
			get { return Process(Restrictions.IsNull(projection)); }
		}

		/// <summary>
		/// Apply an "not is null" constraint to the named property
		/// </summary>
		public AbstractCriterion IsNotNull
		{
			get { return Process(Restrictions.IsNotNull(projection)); }
		}

		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public AbstractCriterion IsLike(object value)
		{
			return Process(Restrictions.Like(projection, value));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public AbstractCriterion IsLike(string value, MatchMode matchMode)
		{
			return Process(Restrictions.Like(projection, value, matchMode));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public AbstractCriterion IsLike(string value, MatchMode matchMode, char? escapeChar)
		{
			return Process(Restrictions.Like(ExpressionProcessor.FindProperty(projection), value, matchMode, escapeChar));
		}
		
	}

}
