
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class QueryOverRestrictionBuilder<T> : QueryOverRestrictionBuilderBase<QueryOver<T>, T>
	{

		public QueryOverRestrictionBuilder(QueryOver<T> root, string propertyName)
			: base(root, propertyName) { }

	}

	public class IQueryOverRestrictionBuilder<T> : QueryOverRestrictionBuilderBase<IQueryOver<T>, T>
	{

		public IQueryOverRestrictionBuilder(IQueryOver<T> root, string propertyName)
			: base(root, propertyName) { }

	}

	public class QueryOverRestrictionBuilderBase<R, T>
		where R : IQueryOver<T>
	{
		public class LambdaBetweenBuilder
		{
			private R root;
			private string propertyName;
			private object lo;

			public LambdaBetweenBuilder(R root, string propertyName, object lo)
			{
				this.root = root;
				this.propertyName = propertyName;
				this.lo = lo;
			}

			public R And(object hi)
			{
				return (R)root.And(Restrictions.Between(propertyName, lo, hi));
			}
		}

		private R root;
		private string propertyName;

		/// <summary>
		/// Constructed with property name
		/// </summary>
		public QueryOverRestrictionBuilderBase(R root, string propertyName)
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
		public R IsIn(ICollection values)
		{
			return (R)root.And(Restrictions.In(propertyName, values));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public R IsIn(object[] values)
		{
			return (R)root.And(Restrictions.In(propertyName, values));
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		public R IsInG<T>(ICollection<T> values)
		{
			return (R)root.And(Restrictions.InG(propertyName, values));
		}

		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public R IsInsensitiveLike(object value)
		{
			return (R)root.And(Restrictions.InsensitiveLike(propertyName, value));
		}
		
		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		public R IsInsensitiveLike(string value, MatchMode matchMode)
		{
			return (R)root.And(Restrictions.InsensitiveLike(propertyName, value, matchMode));
		}

		/// <summary>
		/// Apply an "is empty" constraint to the named property
		/// </summary>
		public R IsEmpty
		{
			get { return (R)root.And(Restrictions.IsEmpty(propertyName)); }
		}

		/// <summary>
		/// Apply a "not is empty" constraint to the named property
		/// </summary>
		public R IsNotEmpty
		{
			get { return (R)root.And(Restrictions.IsNotEmpty(propertyName)); }
		}

		/// <summary>
		/// Apply an "is null" constraint to the named property
		/// </summary>
		public R IsNull
		{
			get { return (R)root.And(Restrictions.IsNull(propertyName)); }
		}

		/// <summary>
		/// Apply an "not is null" constraint to the named property
		/// </summary>
		public R IsNotNull
		{
			get { return (R)root.And(Restrictions.IsNotNull(propertyName)); }
		}

		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public R IsLike(object value)
		{
			return (R)root.And(Restrictions.Like(propertyName, value));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public R IsLike(string value, MatchMode matchMode)
		{
			return (R)root.And(Restrictions.Like(propertyName, value, matchMode));
		}
		
		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		public R IsLike(string value, MatchMode matchMode, char? escapeChar)
		{
			return (R)root.And(Restrictions.Like(propertyName, value, matchMode, escapeChar));
		}
		
	}

}
