
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

		public QueryOverRestrictionBuilder(QueryOver<TRoot,TSubType> root, IProjection projection)
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

		public IQueryOverRestrictionBuilder(IQueryOver<TRoot,TSubType> root, IProjection projection)
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
			private IProjection projection;
			private bool isNot;
			private object lo;

			public LambdaBetweenBuilder(TReturn root, IProjection projection, bool isNot, object lo)
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
				return Add(Restrictions.Between(projection, lo, hi));
			}
		}

		private TReturn root;
		private IProjection projection;
		protected bool isNot;

		/// <summary>
		/// Constructed with property name
		/// </summary>
		public QueryOverRestrictionBuilderBase(TReturn root, IProjection projection)
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
			return Add(Restrictions.In(projection, values));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public TReturn IsIn(object[] values)
		{
			return Add(Restrictions.In(projection, values));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public TReturn IsInG<T>(ICollection<T> values)
		{
			return Add(Restrictions.InG(projection, values));
		}

		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public TReturn IsInsensitiveLike(object value)
		{
			return Add(Restrictions.InsensitiveLike(projection, value));
		}
		
		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public TReturn IsInsensitiveLike(string value, MatchMode matchMode)
		{
			return Add(Restrictions.InsensitiveLike(projection, value, matchMode));
		}

		/// <summary>
		/// Apply an "is empty" constraint to the named property
		/// </summary>
		public TReturn IsEmpty
		{
			get { return Add(Restrictions.IsEmpty(ExpressionProcessor.FindProperty(projection))); }
		}

		/// <summary>
		/// Apply a "not is empty" constraint to the named property
		/// </summary>
		public TReturn IsNotEmpty
		{
			get { return Add(Restrictions.IsNotEmpty(ExpressionProcessor.FindProperty(projection))); }
		}

		/// <summary>
		/// Apply an "is null" constraint to the named property
		/// </summary>
		public TReturn IsNull
		{
			get { return Add(Restrictions.IsNull(projection)); }
		}

		/// <summary>
		/// Apply an "not is null" constraint to the named property
		/// </summary>
		public TReturn IsNotNull
		{
			get { return Add(Restrictions.IsNotNull(projection)); }
		}

		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public TReturn IsLike(object value)
		{
			return Add(Restrictions.Like(projection, value));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public TReturn IsLike(string value, MatchMode matchMode)
		{
			return Add(Restrictions.Like(projection, value, matchMode));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public TReturn IsLike(string value, MatchMode matchMode, char? escapeChar)
		{
			return Add(Restrictions.Like(ExpressionProcessor.FindProperty(projection), value, matchMode, escapeChar));
		}
		
	}

}
