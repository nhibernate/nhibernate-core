
namespace NHibernate.Event
{
	/// <summary> 
	/// Contract for listeners which require notification of SessionFactory closing,
    /// presumably to destroy internal state.
	/// </summary>
    public interface IDestructible
	{
        /// <summary>
        /// Notification of <see cref="ISessionFactory"/> shutdown.
        /// </summary>
        void Cleanup();
	}
}