using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine;
using Remotion.Linq;

namespace NHibernate.Linq
{
	/// <summary>
	/// Interface to access the entity name of a NhQueryable instance.
	/// </summary>
	interface IEntityNameProvider
	{
		string EntityName { get; }
	}

	/// <summary>
	/// Provides the main entry point to a LINQ query.
	/// </summary>
	public class NhQueryable<T> : QueryableBase<T>, IEntityNameProvider
	{
		// This constructor is called by our users, create a new IQueryExecutor.
		public NhQueryable(ISessionImplementor session)
			: this(session, typeof(T).FullName)
		{
		}

		// This constructor is called by our users, create a new IQueryExecutor.
		public NhQueryable(ISessionImplementor session, string entityName)
			: base(QueryProviderFactory.CreateQueryProvider(session))
		{
			EntityName = entityName;
		}


		// This constructor is called indirectly by LINQ's query methods, just pass to base.
		public NhQueryable(IQueryProvider provider, Expression expression)
			: this(provider, expression, typeof(T).FullName)
		{
		}

		// This constructor is called indirectly by LINQ's query methods, just pass to base.
		public NhQueryable(IQueryProvider provider, Expression expression, string entityName)
			: base(provider, expression)
		{
			EntityName = entityName;
		}

		public string EntityName { get; private set; }

		public override string ToString()
		{
			return "NHibernate.Linq.NhQueryable`1[" + EntityName + "]";
		}
	}
}
