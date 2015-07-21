using System.Collections.Generic;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Stat
{
	public class SessionStatisticsImpl : ISessionStatistics
	{
		private readonly ISessionImplementor session;

		public SessionStatisticsImpl(ISessionImplementor session)
		{
			this.session = session;
		}

		#region ISessionStatistics Members

		public int EntityCount
		{
			get { return session.PersistenceContext.EntityEntries.Count; }
		}

		public int CollectionCount
		{
			get { return session.PersistenceContext.CollectionEntries.Count; }
		}

		public IList<EntityKey> EntityKeys
		{
			get
			{
				List<EntityKey> result = new List<EntityKey>();
				result.AddRange(session.PersistenceContext.EntitiesByKey.Keys);
				return result.AsReadOnly();
			}
		}

		public IList<CollectionKey> CollectionKeys
		{
			get 
			{
				List<CollectionKey> result = new List<CollectionKey>();
				result.AddRange(session.PersistenceContext.CollectionsByKey.Keys);
				return result.AsReadOnly();
			}
		}

		#endregion

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