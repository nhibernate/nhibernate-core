using System.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Stat
{
	/// <summary> 
	/// Information about the first-level (session) cache for a particular session instance
	/// </summary>
	public interface ISessionStatistics
	{
		/// <summary> Get the number of entity instances associated with the session</summary>
		int EntityCount { get;}

		/// <summary> Get the number of collection instances associated with the session</summary>
		int CollectionCount { get;}

		/// <summary> Get the set of all <see cref="EntityKey">EntityKeys</see>.</summary>
		IList<EntityKey> EntityKeys { get;}

		/// <summary> Get the set of all <see cref="CollectionKey">CollectionKeys</see>.</summary>
		IList<CollectionKey> CollectionKeys { get;}
	}
}