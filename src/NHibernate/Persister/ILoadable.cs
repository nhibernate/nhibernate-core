using NHibernate.Loader;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Persister
{
	/// <summary>
	/// Implemented by <c>ClassPersister</c> that uses <c>Loader</c>. There are several optional
	/// operations used only by loaders that inherit <c>OuterJoinLoader</c>
	/// </summary>
	public interface ILoadable : IClassPersister
	{
		/// <summary>
		/// Does the persistent class have subclasses?
		/// </summary>
		bool HasSubclasses { get; }

		/// <summary>
		/// The discriminator type
		/// </summary>
		IType DiscriminatorType { get; }

		/// <summary>
		/// Get the concrete subclass corresponding to the given discriminator value
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		System.Type GetSubclassForDiscriminatorValue( object value );

		/// <summary>
		/// Get the result set aliases used for the identifier columns, given a suffix
		/// </summary>
		/// <param name="suffix"></param>
		/// <returns></returns>
		string[] GetIdentifierAliases( string suffix );

		/// <summary>
		/// Get the result set aliases used for the property columns, given a suffix (properties of this class, only).
		/// </summary>
		/// <param name="suffix"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		string[] GetPropertyAliases( string suffix, int i );

		/// <summary>
		/// Get the alias used for the discriminator column, given a suffix
		/// </summary>
		/// <param name="suffix"></param>
		/// <returns></returns>
		string GetDiscriminatorAlias( string suffix );
	}
}