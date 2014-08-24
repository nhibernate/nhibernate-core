using NHibernate.Engine;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	///  Loads entities for a <see cref="NHibernate.Persister.Entity.IEntityPersister"/>
	/// </summary>
	public interface IUniqueEntityLoader
	{
		/// <summary>
		/// Load an entity instance. If <c>OptionalObject</c> is supplied, load the entity
		/// state into the given (uninitialized) object
		/// </summary>
		object Load(object id, object optionalObject, ISessionImplementor session);
	}
}