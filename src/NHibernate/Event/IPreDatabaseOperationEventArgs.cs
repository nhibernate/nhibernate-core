using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	public interface IPreDatabaseOperationEventArgs : IDatabaseEventArgs
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
