
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class QueryOverRestrictionBuilder<TRoot,TSubType> : QueryOverRestrictionBuilderBase<QueryOver<TRoot,TSubType>, TRoot, TSubType>
	{

		public QueryOverRestrictionBuilder(QueryOver<TRoot,TSubType> root, ExpressionProcessor.ProjectionInfo projection)
			: base(root, projection) { }

		public QueryOverRestrictionBuilder<TRoot,TSubType> Not
		{
			get
			{
				isNot = !isNot;
				return this;
			}
		}

	}

	public class IQueryOverRestrictionBuilder<TRoot,TSubType> : QueryOverRestrictionBuilderBase<IQueryOver<TRoot,TSubType>, TRoot, TSubType>
	{

		public IQueryOverRestrictionBuilder(IQueryOver<TRoot,TSubType> root, ExpressionProcessor.ProjectionInfo projection)
			: base(root, projection) { }

		public IQueryOverRestrictionBuilder<TRoot,TSubType> Not
		{
			get
			{
				isNot = !isNot;
				return this;
			}
		}

	}

	public class QueryOverRestrictionBuilderBase<TReturn,TRoot,TSubType>
		where TReturn : IQueryOver<TRoot,TSubType>
	{
		public class LambdaBetweenBuilder
		{
			private TReturn root;
			private ExpressionProcessor.ProjectionInfo projection;
			private bool isNot;
			private object lo;

			public LambdaBetweenBuilder(TReturn root, ExpressionProcessor.ProjectionInfo projection, bool isNot, object lo)
			{
				this.root = root;
				this.projection = projection;
				this.isNot = isNot;
				this.lo = lo;
			}

			private TReturn Add(ICriterion criterion)
			{
				if (isNot)
					criterion = Restrictions.Not(criterion);

				return (TReturn)root.And(criterion);
			}

			public TReturn And(object hi)
			{
				return Add(projection.Create<ICriterion>(s => Restrictions.Between(s, lo, hi), p => Restrictions.Between(p, lo, hi)));
			}
		}

		private TReturn root;
		private ExpressionProcessor.ProjectionInfo projection;
		protected bool isNot;

		/// <summary>
		/// Constructed with property name
		/// </summary>
		public QueryOverRestrictionBuilderBase(TReturn root, ExpressionProcessor.ProjectionInfo projection)
		{
			this.root = root;
			this.projection = projection;
		}

		private TReturn Add(ICriterion criterion)
		{
			if (isNot)
				criterion = Restrictions.Not(criterion);

			return (TReturn)root.And(criterion);
		}

		/// <summary>
		/// Apply a "between" constraint to the named property
		/// </summary>
		public LambdaBetweenBuilder IsBetween(object lo)
		{
			return new LambdaBetweenBuilder(root, projection, isNot, lo);
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public TReturn IsIn(ICollection values)
		{
			return Add(projection.Create<ICriterion>(s => Restrictions.In(s, values), p => Restrictions.In(p, values)));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public TReturn IsIn(object[] values)
		{
			return Add(projection.Create<ICriterion>(s => Restrictions.In(s, values), p => Restrictions.In(p, values)));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public TReturn IsInG<T>(IEnumerable<T> values)
		{
			return Add(projection.Create<ICriterion>(s => Restrictions.InG(s, values), p => Restrictions.InG(p, values)));
		}

		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public TReturn IsInsensitiveLike(object value)
		{
			return Add(projection.CreateCriterion(Restrictions.InsensitiveLike, Restrictions.InsensitiveLike, value));
		}
		
		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public TReturn IsInsensitiveLike(string value, MatchMode matchMode)
		{
			return Add(projection.Create<ICriterion>(s => Restrictions.InsensitiveLike(s, value, matchMode), p => Restrictions.InsensitiveLike(p, value, matchMode)));
		}

		/// <summary>
		/// Apply an "is empty" constraint to the named property
		/// </summary>
		public TReturn IsEmpty
		{
			get { return Add(Restrictions.IsEmpty(projection.AsProperty())); }
		}

		/// <summary>
		/// Apply a "not is empty" constraint to the named property
		/// </summary>
		public TReturn IsNotEmpty
		{
			get { return Add(Restrictions.IsNotEmpty(projection.AsProperty())); }
		}

		/// <summary>
		/// Apply an "is null" constraint to the named property
		/// </summary>
		public TReturn IsNull
		{
			get { return Add(projection.CreateCriterion(Restrictions.IsNull, Restrictions.IsNull)); }
		}

		/// <summary>
		/// Apply an "not is null" constraint to the named property
		/// </summary>
		public TReturn IsNotNull
		{
			get { return Add(projection.CreateCriterion(Restrictions.IsNotNull, Restrictions.IsNotNull)); }
		}

		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public TReturn IsLike(object value)
		{
			return Add(projection.CreateCriterion(Restrictions.Like, Restrictions.Like, value));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public TReturn IsLike(string value, MatchMode matchMode)
		{
			return Add(projection.Create<ICriterion>(s => Restrictions.Like(s, value, matchMode), p => Restrictions.Like(p, value, matchMode)));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public TReturn IsLike(string value, MatchMode matchMode, char? escapeChar)
		{
			return Add(Restrictions.Like(projection.AsProperty(), value, matchMode, escapeChar));
		}
		
	}

}
