using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	/// <summary> 
	/// Represents an operation we performed against the database. 
	/// </summary>
	public interface IPostDatabaseOperationEventArgs : IDatabaseEventArgs
	{
		/// <summary> The entity involved in the database operation. </summary>
		object Entity { get; }

		/// <summary> The id to be used in the database operation. </summary>
		object Id { get; }

		/// <summary> 
		/// The persister for the <see cref="Entity"/>. 
		/// </summary>
		IEntityPersister Persister { get; }
	}
}
