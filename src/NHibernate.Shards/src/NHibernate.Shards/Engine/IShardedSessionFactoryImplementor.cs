using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Shards.Session;

namespace NHibernate.Shards.Engine
{
	/// <summary>
	/// Internal interface for implementors of ShardedSessionFactory
	/// </summary>
	internal interface IShardedSessionFactoryImplementor : IShardedSessionFactory, ISessionFactoryImplementor
	{
		IDictionary<ISessionFactoryImplementor, Set<ShardId>> GetSessionFactoryShardIdMap();

		bool ContainsFactory(ISessionFactoryImplementor factory);
	}
}