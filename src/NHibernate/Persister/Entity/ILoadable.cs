using NHibernate.Type;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// Implemented by <c>ClassPersister</c> that uses <c>Loader</c>. There are several optional
	/// operations used only by loaders that inherit <c>OuterJoinLoader</c>
	/// </summary>
	public interface ILoadable : IEntityPersister
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
		System.Type GetSubclassForDiscriminatorValue(object value);

		/// <summary>
		/// Get the names of columns used to persist the identifier
		/// </summary>
		string[] IdentifierColumnNames { get; }

		/// <summary>
		/// Get the result set aliases used for the identifier columns, given a suffix
		/// </summary>
		string[] GetIdentifierAliases(string suffix);

		/// <summary>
		/// Get the result set aliases used for the property columns, given a suffix (properties of this class, only).
		/// </summary>
		string[] GetPropertyAliases(string suffix, int i);

		/// <summary>
		/// Get the result set column names mapped for this property (properties of this class, only).
		/// </summary>
		string[] GetPropertyColumnNames(int i);

		/// <summary>
		/// Get the alias used for the discriminator column, given a suffix
		/// </summary>
		string GetDiscriminatorAlias(string suffix);

		/// <summary>
		/// Get the name of the column used as a discriminator
		/// </summary>
		string DiscriminatorColumnName { get; }

		/// <summary>
		/// Does this entity own any collections which are fetchable by subselect?
		/// </summary>
		bool HasSubselectLoadableCollections { get; }
	}
}