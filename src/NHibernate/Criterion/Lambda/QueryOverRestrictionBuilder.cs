
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

		public QueryOverRestrictionBuilder(QueryOver<TRoot,TSubType> root, string propertyName)
			: base(root, propertyName) { }

	}

	public class IQueryOverRestrictionBuilder<TRoot,TSubType> : QueryOverRestrictionBuilderBase<IQueryOver<TRoot,TSubType>, TRoot, TSubType>
	{

		public IQueryOverRestrictionBuilder(IQueryOver<TRoot,TSubType> root, string propertyName)
			: base(root, propertyName) { }

	}

	public class QueryOverRestrictionBuilderBase<TReturn,TRoot,TSubType>
		where TReturn : IQueryOver<TRoot,TSubType>
	{
		public class LambdaBetweenBuilder
		{
			private TReturn root;
			private string propertyName;
			private object lo;

			public LambdaBetweenBuilder(TReturn root, string propertyName, object lo)
			{
				this.root = root;
				this.propertyName = propertyName;
				this.lo = lo;
			}

			public TReturn And(object hi)
			{
				return (TReturn)root.And(Restrictions.Between(propertyName, lo, hi));
			}
		}

		private TReturn root;
		private string propertyName;

		/// <summary>
		/// Constructed with property name
		/// </summary>
		public QueryOverRestrictionBuilderBase(TReturn root, string propertyName)
		{
			this.root = root;
			this.propertyName = propertyName;
		}

		/// <summary>
		/// Apply a "between" constraint to the named property
		/// </summary>
		public LambdaBetweenBuilder IsBetween(object lo)
		{
			return new LambdaBetweenBuilder(root, propertyName, lo);
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public TReturn IsIn(ICollection values)
		{
			return (TReturn)root.And(Restrictions.In(propertyName, values));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public TReturn IsIn(object[] values)
		{
			return (TReturn)root.And(Restrictions.In(propertyName, values));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public TReturn IsInG<T>(ICollection<T> values)
		{
			return (TReturn)root.And(Restrictions.InG(propertyName, values));
		}

		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public TReturn IsInsensitiveLike(object value)
		{
			return (TReturn)root.And(Restrictions.InsensitiveLike(propertyName, value));
		}
		
		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public TReturn IsInsensitiveLike(string value, MatchMode matchMode)
		{
			return (TReturn)root.And(Restrictions.InsensitiveLike(propertyName, value, matchMode));
		}

		/// <summary>
		/// Apply an "is empty" constraint to the named property
		/// </summary>
		public TReturn IsEmpty
		{
			get { return (TReturn)root.And(Restrictions.IsEmpty(propertyName)); }
		}

		/// <summary>
		/// Apply a "not is empty" constraint to the named property
		/// </summary>
		public TReturn IsNotEmpty
		{
			get { return (TReturn)root.And(Restrictions.IsNotEmpty(propertyName)); }
		}

		/// <summary>
		/// Apply an "is null" constraint to the named property
		/// </summary>
		public TReturn IsNull
		{
			get { return (TReturn)root.And(Restrictions.IsNull(propertyName)); }
		}

		/// <summary>
		/// Apply an "not is null" constraint to the named property
		/// </summary>
		public TReturn IsNotNull
		{
			get { return (TReturn)root.And(Restrictions.IsNotNull(propertyName)); }
		}

		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public TReturn IsLike(object value)
		{
			return (TReturn)root.And(Restrictions.Like(propertyName, value));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public TReturn IsLike(string value, MatchMode matchMode)
		{
			return (TReturn)root.And(Restrictions.Like(propertyName, value, matchMode));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public TReturn IsLike(string value, MatchMode matchMode, char? escapeChar)
		{
			return (TReturn)root.And(Restrictions.Like(propertyName, value, matchMode, escapeChar));
		}
		
	}

}
