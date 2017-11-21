using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Stat
{
	public class SessionStatisticsImpl : ISessionStatistics
	{
		private readonly IPersistenceContext _persistenceContext;

		public SessionStatisticsImpl(IPersistenceContext persistenceContext)
		{
			_persistenceContext = persistenceContext;
		}

		//Since v5.1
		[Obsolete("Please use SessionStatisticsImpl(IPersistenceContext persistenceContext) constructor.")]
		public SessionStatisticsImpl(ISessionImplementor session)
			: this(session.PersistenceContext)
		{
		}

		public int EntityCount => _persistenceContext.EntityEntries.Count;

		public int CollectionCount => _persistenceContext.CollectionEntries.Count;

		public IList<EntityKey> EntityKeys => new List<EntityKey>(_persistenceContext.EntitiesByKey.Keys).AsReadOnly();

		public IList<CollectionKey> CollectionKeys => new List<CollectionKey>(_persistenceContext.CollectionsByKey.Keys).AsReadOnly();

		public override string ToString()
		{
			return new StringBuilder()
				.Append("SessionStatistics[")
				.Append("entity count=").Append(EntityCount)
				.Append("collection count=").Append(CollectionCount)
				.Append(']').ToString();
		}
	}
}
