using NHibernate.Engine;

namespace NHibernate.Shards.Session
{
	/// <summary>
	/// Interface for any entity that can provide the control session.
	/// Control session is used to access control (meta)data which usually lives on
	/// only one shard.
	/// </summary>
	public interface IControlSessionProvider
	{
		/// <summary>
		/// Opens control session.
		/// </summary>
		/// <returns>control session</returns>
		ISessionImplementor OpenControlSession();
	}
}