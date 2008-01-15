using System.Collections.Generic;
using NHibernate.Shards.Session;

namespace NHibernate.Shards.Engine
{
	/// <summary>
	///  Defines the internal contract between the ShardedSession and other
	/// parts of Hibernate Shards.
	/// <seealso cref="IShardedSession"/> the interface to the application.
	/// <seealso cref="ShardedSessionImpl"/> the actual implementation
	/// </summary>
	public interface IShardedSessionImplementor
	{
		/// <summary>
		/// Gets all the shards the ShardedSession is spanning.
		/// Return list of all shards the ShardedSession is associated with
		/// </summary>
		IList<IShard> Shards { get; }
	}
}