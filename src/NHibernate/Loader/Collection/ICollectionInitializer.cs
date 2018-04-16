using NHibernate.Engine;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// An interface for collection loaders
	/// </summary>
	/// <seealso cref="BasicCollectionLoader"/>
	/// <seealso cref="OneToManyLoader"/>
	public partial interface ICollectionInitializer
	{
		/// <summary>
		/// Initialize the given collection
		/// </summary>
		void Initialize(object id, ISessionImplementor session);
	}
}