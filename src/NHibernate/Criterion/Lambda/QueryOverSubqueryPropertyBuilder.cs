
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class QueryOverSubqueryPropertyBuilder<TRoot,TSubType> : QueryOverSubqueryPropertyBuilderBase<QueryOver<TRoot,TSubType>, TRoot, TSubType>
	{

		public QueryOverSubqueryPropertyBuilder()
			: base() { }

	}

	public class IQueryOverSubqueryPropertyBuilder<TRoot,TSubType> : QueryOverSubqueryPropertyBuilderBase<IQueryOver<TRoot,TSubType>, TRoot, TSubType>
	{

		public IQueryOverSubqueryPropertyBuilder()
			: base() { }

	}

	public abstract class QueryOverSubqueryPropertyBuilderBase
	{
		protected QueryOverSubqueryPropertyBuilderBase() { }

		internal abstract QueryOverSubqueryPropertyBuilderBase Set(object root, string path, object value);
	}

	public class QueryOverSubqueryPropertyBuilderBase<TReturn, TRoot, TSubType> : QueryOverSubqueryPropertyBuilderBase
		where TReturn : IQueryOver<TRoot,TSubType>
	{

		protected TReturn root;
		protected string path;
		protected object value;

		protected QueryOverSubqueryPropertyBuilderBase()
		{
		}

		internal override QueryOverSubqueryPropertyBuilderBase Set(object root, string path, object value)
		{
			this.root = (TReturn)root;
			this.path = path;
			this.value = value;
			return this;
		}

		private void AddSubquery<U>(
			Func<string, DetachedCriteria, AbstractCriterion> propertyMethod,
			Func<object, DetachedCriteria, AbstractCriterion> valueMethod,
			QueryOver<U> detachedCriteria)
		{
			if (path != null)
			{
				root.And(propertyMethod(path, detachedCriteria.DetachedCriteria));
			}
			else
			{
				root.And(valueMethod(value, detachedCriteria.DetachedCriteria));
			}
		}

		/// <summary>
		/// Add a property equal subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn Eq<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyEq, Subqueries.Eq, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Add a property equal all subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn EqAll<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyEqAll, Subqueries.EqAll, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property greater than or equal subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn Ge<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyGe, Subqueries.Ge, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property greater than or equal all subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn GeAll<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyGeAll, Subqueries.GeAll, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property greater than or equal some subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn GeSome<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyGeSome, Subqueries.GeSome, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property greater than subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn Gt<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyGt, Subqueries.Gt, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property greater than all subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn GtAll<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyGtAll, Subqueries.GtAll, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property greater than some subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn GtSome<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyGtSome, Subqueries.GtSome, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property in subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn In<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyIn, Subqueries.In, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property less than or equal subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn Le<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyLe, Subqueries.Le, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property less than or equal all subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn LeAll<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyLeAll, Subqueries.LeAll, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property less than or equal some subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn LeSome<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyLeSome, Subqueries.LeSome, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property less than subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn Lt<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyLt, Subqueries.Lt, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property less than all subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn LtAll<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyLtAll, Subqueries.LtAll, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property less than some subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn LtSome<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyLtSome, Subqueries.LtSome, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property not equal subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn Ne<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyNe, Subqueries.Ne, detachedCriteria);
			return root;
		}

		/// <summary>
		/// Create a property not in subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public TReturn NotIn<U>(QueryOver<U> detachedCriteria)
		{
			AddSubquery(Subqueries.PropertyNotIn, Subqueries.NotIn, detachedCriteria);
			return root;
		}

	}

}
