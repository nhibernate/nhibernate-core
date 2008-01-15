namespace NHibernate.Shards.Session
{
	/// <summary>
	/// Interface for events that can be laziliy applied to a <see cref="ISession"/>.
	/// Useful because we don't allocate a  <see cref="ISession"/> until we actually need it,
	/// and programmers might be calling a variety of methods against the
	/// {@link ShardedSession} which
	/// need to be applied to the actual <see cref="ISession"/>
	/// once the actual <see cref="ISession"/> is allocated.
	/// </summary>
	public interface IOpenSessionEvent
	{
		/// <summary>
		/// Invokes any actions that have to occur when a session is opened.
		/// </summary>
		/// <param name="session">Session which is being opened</param>
		void OnOpenSession(ISession session);
	}
}