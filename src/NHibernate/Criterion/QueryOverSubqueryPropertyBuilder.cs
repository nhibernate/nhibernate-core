
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{

	public class QueryOverSubqueryPropertyBuilder<T> : QueryOverSubqueryPropertyBuilderBase<QueryOver<T>, T>
	{

		public QueryOverSubqueryPropertyBuilder()
			: base() { }

	}

	public class IQueryOverSubqueryPropertyBuilder<T> : QueryOverSubqueryPropertyBuilderBase<IQueryOver<T>, T>
	{

		public IQueryOverSubqueryPropertyBuilder()
			: base() { }

	}

	public abstract class QueryOverSubqueryPropertyBuilderBase
	{
		protected QueryOverSubqueryPropertyBuilderBase() { }

		public abstract QueryOverSubqueryPropertyBuilderBase Set(object root, string path);
	}

	public class QueryOverSubqueryPropertyBuilderBase<R, T> : QueryOverSubqueryPropertyBuilderBase
		where R : IQueryOver<T>
	{

		protected R root;
		protected string path;

		protected QueryOverSubqueryPropertyBuilderBase()
		{
		}

		public override QueryOverSubqueryPropertyBuilderBase Set(object root, string path)
		{
			this.root = (R)root;
			this.path = path;
			return this;
		}

		/// <summary>
		/// Add a property equal subquery criterion
		/// </summary>
		/// <param name="detachedCriteria">detached subquery</param>
		public R Eq(QueryOver<T> detachedCriteria)
		{
			root.Where(Subqueries.PropertyEq(path, detachedCriteria.DetachedCriteria));
			return root;
		}

	}

}
