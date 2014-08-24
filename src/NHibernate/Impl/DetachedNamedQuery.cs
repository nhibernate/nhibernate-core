using System;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// Named query in "detached mode" where the NHibernate session is not available.
	/// </summary>
	/// <seealso cref="AbstractDetachedQuery"/>
	/// <seealso cref="IDetachedQuery"/>
	/// <seealso cref="IQuery"/>
	/// <seealso cref="ISession.GetNamedQuery(string)"/>
	[Serializable]
	public class DetachedNamedQuery : AbstractDetachedQuery
	{
		private readonly string queryName;
		private bool cacheableWasSet;
		private bool cacheModeWasSet;
		private bool cacheRegionWasSet;
		private bool readOnlyWasSet;
		private bool timeoutWasSet;
		private bool fetchSizeWasSet;
		private bool commentWasSet;
		private bool flushModeWasSet;

		/// <summary>
		/// Create a new instance of <see cref="DetachedNamedQuery"/> for a named query string defined in the mapping file.
		/// </summary>
		/// <param name="queryName">The name of a query defined externally.</param>
		/// <remarks>
		/// The query can be either in HQL or SQL format.
		/// </remarks>
		public DetachedNamedQuery(string queryName)
		{
			this.queryName = queryName;
		}

		/// <summary>
		/// Get the query name.
		/// </summary>
		public string QueryName
		{
			get { return queryName; }
		}

		/// <summary>
		/// Get an executable instance of <see cref="IQuery"/>, to actually run the query.
		/// </summary>
		public override IQuery GetExecutableQuery(ISession session)
		{
			IQuery result = session.GetNamedQuery(queryName);
			SetDefaultProperties((ISessionFactoryImplementor)session.SessionFactory);
			SetQueryProperties(result);
			return result;
		}

		private void SetDefaultProperties(ISessionFactoryImplementor factory)
		{
			NamedQueryDefinition nqd = factory.GetNamedQuery(queryName) ?? factory.GetNamedSQLQuery(queryName);

			if (!cacheableWasSet)
			{
				cacheable = nqd.IsCacheable;
			}

			if (!cacheRegionWasSet)
			{
				cacheRegion = nqd.CacheRegion;
			}

			if(!timeoutWasSet && nqd.Timeout != -1)
			{
				selection.Timeout= nqd.Timeout;
			}

			if (!fetchSizeWasSet && nqd.FetchSize != -1)
			{
				selection.FetchSize = nqd.FetchSize;
			}

			if (!cacheModeWasSet && nqd.CacheMode.HasValue)
			{
				cacheMode = nqd.CacheMode.Value;
			}

			if (!readOnlyWasSet)
			{
				readOnly = nqd.IsReadOnly;
			}

			if (!commentWasSet && nqd.Comment != null)
			{
				comment = nqd.Comment;
			}

			if(!flushModeWasSet)
			{
				flushMode = nqd.FlushMode;
			}
		}

		/// <summary>
		/// Creates a new DetachedNamedQuery that is a deep copy of the current instance.
		/// </summary>
		/// <returns>The clone.</returns>
		public DetachedNamedQuery Clone()
		{
			var result = new DetachedNamedQuery(queryName);
			CopyTo(result);
			return result;
		}

		public override IDetachedQuery SetCacheable(bool cacheable)
		{
			cacheableWasSet = true;
			return base.SetCacheable(cacheable);
		}

		public override IDetachedQuery SetCacheMode(CacheMode cacheMode)
		{
			cacheModeWasSet = true;
			return base.SetCacheMode(cacheMode);
		}
		public override IDetachedQuery SetCacheRegion(string cacheRegion)
		{
			cacheRegionWasSet = true;
			return base.SetCacheRegion(cacheRegion);
		}

		public override IDetachedQuery SetReadOnly(bool readOnly)
		{
			readOnlyWasSet = true;
			return base.SetReadOnly(readOnly);
		}

		public override IDetachedQuery SetTimeout(int timeout)
		{
			timeoutWasSet = true;
			return base.SetTimeout(timeout);
		}

		public override IDetachedQuery SetFetchSize(int fetchSize)
		{
			fetchSizeWasSet = true;
			return base.SetFetchSize(fetchSize);
		}

		public override IDetachedQuery SetComment(string comment)
		{
			commentWasSet = true;
			return base.SetComment(comment);
		}

		public override IDetachedQuery SetFlushMode(FlushMode flushMode)
		{
			flushModeWasSet = true;
			return base.SetFlushMode(flushMode);
		}
	}
}