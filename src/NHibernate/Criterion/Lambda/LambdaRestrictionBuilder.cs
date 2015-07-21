
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
			private ExpressionProcessor.ProjectionInfo projection;
			private object lo;
			private bool isNot;

			public LambdaBetweenBuilder(ExpressionProcessor.ProjectionInfo projection, object lo, bool isNot)
			{
				this.projection = projection;
				this.lo = lo;
				this.isNot = isNot;
			}

			public AbstractCriterion And(object hi)
			{
				AbstractCriterion criterion = projection.Create<AbstractCriterion>(s => Restrictions.Between(s, lo, hi), p => Restrictions.Between(p, lo, hi));

				if (isNot)
					return Restrictions.Not(criterion);

				return criterion;
			}
		}

		private ExpressionProcessor.ProjectionInfo projection;
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
		public LambdaRestrictionBuilder(ExpressionProcessor.ProjectionInfo projection)
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
			return Process(projection.Create<AbstractCriterion>(s => Restrictions.In(s, values), p => Restrictions.In(p, values)));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public AbstractCriterion IsIn(object[] values)
		{
			return Process(projection.Create<AbstractCriterion>(s => Restrictions.In(s, values), p => Restrictions.In(p, values)));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public AbstractCriterion IsInG<T>(IEnumerable<T> values)
		{
			return Process(projection.Create<AbstractCriterion>(s => Restrictions.InG(s, values), p => Restrictions.InG(p, values)));
		}

		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public AbstractCriterion IsInsensitiveLike(object value)
		{
			return Process(projection.Create<AbstractCriterion>(s => Restrictions.InsensitiveLike(s, value), p => Restrictions.InsensitiveLike(p, value)));
		}
		
		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public AbstractCriterion IsInsensitiveLike(string value, MatchMode matchMode)
		{
			return Process(projection.Create<AbstractCriterion>(s => Restrictions.InsensitiveLike(s, value, matchMode), p => Restrictions.InsensitiveLike(p, value, matchMode)));
		}

		/// <summary>
		/// Apply an "is empty" constraint to the named property
		/// </summary>
		public AbstractCriterion IsEmpty
		{
			get { return Process(Restrictions.IsEmpty(projection.AsProperty())); }
		}

		/// <summary>
		/// Apply a "not is empty" constraint to the named property
		/// </summary>
		public AbstractCriterion IsNotEmpty
		{
			get { return Process(Restrictions.IsNotEmpty(projection.AsProperty())); }
		}

		/// <summary>
		/// Apply an "is null" constraint to the named property
		/// </summary>
		public AbstractCriterion IsNull
		{
			get { return Process(projection.Create<AbstractCriterion>(s => Restrictions.IsNull(s), p => Restrictions.IsNull(p))); }
		}

		/// <summary>
		/// Apply an "not is null" constraint to the named property
		/// </summary>
		public AbstractCriterion IsNotNull
		{
			get { return Process(projection.Create<AbstractCriterion>(s => Restrictions.IsNotNull(s), p => Restrictions.IsNotNull(p))); }
		}

		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public AbstractCriterion IsLike(object value)
		{
			return Process(projection.Create<AbstractCriterion>(s => Restrictions.Like(s, value), p => Restrictions.Like(p, value)));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public AbstractCriterion IsLike(string value, MatchMode matchMode)
		{
			return Process(projection.Create<AbstractCriterion>(s => Restrictions.Like(s, value, matchMode), p => Restrictions.Like(p, value, matchMode)));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public AbstractCriterion IsLike(string value, MatchMode matchMode, char? escapeChar)
		{
			return Process(Restrictions.Like(projection.AsProperty(), value, matchMode, escapeChar));
		}
		
	}

}
