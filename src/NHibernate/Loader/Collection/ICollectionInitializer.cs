using NHibernate.Engine;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// An interface for collection loaders
	/// </summary>
	public interface ICollectionInitializer
	{
		/// <summary>
		/// Initialize the given collection
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		void Initialize(object id, ISessionImplementor session);
	}
}