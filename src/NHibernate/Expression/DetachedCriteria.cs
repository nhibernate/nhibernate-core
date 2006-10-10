using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Impl;
using NHibernate.Transform;

namespace NHibernate.Expression
{
	/// <summary>
	/// Some applications need to create criteria queries in "detached
	/// mode", where the Hibernate session is not available. This class
	/// may be instantiated anywhere, and then a <c>ICriteria</c>
	/// may be obtained by passing a session to 
	/// <c>GetExecutableCriteria()</c>. All methods have the
	/// same semantics and behavior as the corresponding methods of the
	/// <c>ICriteria</c> interface.
	/// </summary>
	public class DetachedCriteria
	{
		private CriteriaImpl impl;
		private ICriteria criteria;

		protected DetachedCriteria(System.Type entityType)
		{
			impl = new CriteriaImpl(entityType, null);
			criteria = impl;
		}

		protected DetachedCriteria(System.Type entityType, String alias)
		{
			impl = new CriteriaImpl(entityType, alias, null);
			criteria = impl;
		}

		protected DetachedCriteria(CriteriaImpl impl, ICriteria criteria)
		{
			this.impl = impl;
			this.criteria = criteria;
		}

		/// <summary>
		/// Get an executable instance of <c>Criteria</c>,
		/// to actually run the query.</summary>
		public ICriteria GetExecutableCriteria(ISession session)
		{
			impl.Session = (SessionImpl) session;
			return impl;
		}

		public static DetachedCriteria For(System.Type entityType)
		{
			return new DetachedCriteria(entityType);
		}

#if NET_2_0
		public static DetachedCriteria For<T>()
		{
			return new DetachedCriteria(typeof (T));
		}
#endif

		public static DetachedCriteria For(System.Type entityType, String alias)
		{
			return new DetachedCriteria(entityType, alias);
		}


		public DetachedCriteria Add(ICriterion criterion)
		{
			criteria.Add(criterion);
			return this;
		}

		public DetachedCriteria AddOrder(Order order)
		{
			criteria.AddOrder(order);
			return this;
		}

		public DetachedCriteria CreateAlias(String associationPath, String alias)
		{
			criteria.CreateAlias(associationPath, alias);
			return this;
		}

		public DetachedCriteria createCriteria(String associationPath, String alias)
		{
			return new DetachedCriteria(impl, criteria.CreateCriteria(associationPath));
		}

		public DetachedCriteria createCriteria(String associationPath)
		{
			return new DetachedCriteria(impl, criteria.CreateCriteria(associationPath));
		}

		public String Alias
		{
			get { return criteria.Alias; }
		}

		public DetachedCriteria SetFetchMode(String associationPath, FetchMode mode)
		{
			criteria.SetFetchMode(associationPath, mode);
			return this;
		}

		public DetachedCriteria SetProjection(IProjection projection)
		{
			criteria.SetProjection(projection);
			return this;
		}

		public DetachedCriteria SetResultTransformer(IResultTransformer resultTransformer)
		{
			criteria.SetResultTransformer(resultTransformer);
			return this;
		}

		public override String ToString()
		{
			return "DetachableCriteria(" + criteria.ToString() + ')';
		}
	}
}