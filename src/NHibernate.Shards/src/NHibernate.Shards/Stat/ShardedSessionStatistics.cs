using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Stat;

namespace NHibernate.Shards.Stat
{
	internal class ShardedSessionStatistics : ISessionStatistics
	{
		#region ISessionStatistics Members

		///<summary>
		/// Get the number of entity instances associated with the session
		///</summary>
		///
		public int EntityCount
		{
			get { throw new NotImplementedException(); }
		}

		///<summary>
		/// Get the number of collection instances associated with the session
		///</summary>
		///
		public int CollectionCount
		{
			get { throw new NotImplementedException(); }
		}

		///<summary>
		/// Get the set of all <see cref="T:NHibernate.Engine.EntityKey">EntityKeys</see>.
		///</summary>
		///
		public IList<EntityKey> EntityKeys
		{
			get { throw new NotImplementedException(); }
		}

		///<summary>
		/// Get the set of all <see cref="T:NHibernate.Engine.CollectionKey">CollectionKeys</see>.
		///</summary>
		///
		public IList<CollectionKey> CollectionKeys
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}