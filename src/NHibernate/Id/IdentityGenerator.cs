using NHibernate.Engine;

namespace NHibernate.Id
{
	/// <summary>
	/// Indicates to the <see cref="ISession"/> that identity
	/// (ie. identity/autoincrement column) key generation should be used.
	/// </summary>
	/// <remarks>
	/// <para>
	///	This id generation strategy is specified in the mapping file as 
	///	<c>&lt;generator class="identity"&gt;</c>
	/// </para>
	/// <para>
	/// This indicates to NHibernate that the database generates the id when
	/// the entity is inserted.
	/// </para>
	/// </remarks>
	public class IdentityGenerator : IIdentifierGenerator
	{
		/// <summary>
		/// This class can not generate the <c>id</c>.  It has to get the 
		/// value from the database.
		/// </summary>
		/// <param name="s">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
		/// <param name="obj">The entity the id is being generated for.</param>
		/// <returns>
		/// <c>null</c> because this <see cref="IIdentifierGenerator"/> can not generate
		/// an id.  The entity must be inserted into the database to get the database 
		/// generated id.
		/// </returns>
		public object Generate( ISessionImplementor s, object obj )
		{
			return null;
		}
	}
}