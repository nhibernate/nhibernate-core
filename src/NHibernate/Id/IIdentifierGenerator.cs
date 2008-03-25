using NHibernate.Engine;

namespace NHibernate.Id
{
	public struct IdGeneratorParmsNames
	{
		/// <summary> The configuration parameter holding the entity name</summary>
		public readonly static string EntityName = "entity_name";
	}


	/// <summary>
	/// The general contract between a class that generates unique
	/// identifiers and the <see cref="ISession"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// It is not intended that this interface ever be exposed to the 
	/// application.  It <b>is</b> intended that users implement this interface
	/// to provide custom identifier generation strategies.
	/// </para>
	/// <para>
	/// Implementors should provide a public default constructor.
	/// </para>
	/// <para>
	/// Implementations that accept configuration parameters should also
	/// implement <see cref="IConfigurable"/>.
	/// </para>
	/// <para>
	/// Implementors <b>must</b> be threadsafe.
	/// </para>
	/// </remarks>
	public interface IIdentifierGenerator
	{
		/// <summary>
		/// Generate a new identifier
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
		/// <param name="obj">The entity for which the id is being generated.</param>
		/// <returns>The new identifier</returns>
		object Generate(ISessionImplementor session, object obj);
	}
}