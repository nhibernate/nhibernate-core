using System;

using NHibernate.Engine;

namespace NHibernate.Id {
	
	/// <summary>
	/// The general contract between a class that generates unique
	/// identifiers and the <c>ISession</c>.
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
		/// <param name="session"></param>
		/// <param name="obj">The entity for which the id is being generate</param>
		/// <returns>The new identifier</returns>
		object Generate(ISessionImplementor session, object obj);
	}
}
