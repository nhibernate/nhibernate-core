using NHibernate.Engine;

namespace NHibernate.Id
{
	/// <summary>
	/// An <see cref="IIdentifierGenerator" /> that indicates to the <see cref="ISession"/> that identity
	/// (ie. identity/autoincrement column) key generation should be used.
	/// </summary>
	/// <remarks>
	/// <p>
	///	This id generation strategy is specified in the mapping file as 
	///	<code>&lt;generator class="identity" /&gt;</code> 
	///	or if the database natively supports identity columns 
	///	<code>&lt;generator class="native" /&gt;</code>
	/// </p>
	/// <p>
	/// This indicates to NHibernate that the database generates the id when
	/// the entity is inserted.
	/// </p>
	/// </remarks>
	public class IdentityGenerator : IIdentifierGenerator
	{
		#region IIdentifierGenerator Members

		/// <summary>
		/// The IdentityGenerator for autoincrement/identity key generation.
		/// 
		/// </summary>
		/// <param name="s">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
		/// <param name="obj">The entity the id is being generated for.</param>
		/// <returns>
		/// <c>IdentityColumnIndicator</c> Indicates to the Session that identity (i.e. identity/autoincrement column)
		/// key generation should be used.
		/// </returns>
		public object Generate(ISessionImplementor s, object obj)
		{
			return IdentifierGeneratorFactory.IdentityColumnIndicator;
		}

		#endregion
	}
}