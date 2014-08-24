using NHibernate.Type;
using NHibernate.Engine;
using System.Data;

namespace NHibernate.Persister.Entity
{
	public struct Loadable
	{
		public readonly static string RowIdAlias = "rowid_";
	}

	/// <summary>
	/// Implemented by <c>ClassPersister</c> that uses <c>Loader</c>. There are several optional
	/// operations used only by loaders that inherit <c>OuterJoinLoader</c>
	/// </summary>
	public interface ILoadable : IEntityPersister
	{
		/// <summary>
		/// The discriminator type
		/// </summary>
		IType DiscriminatorType { get; }

		/// <summary>
		/// Get the names of columns used to persist the identifier
		/// </summary>
		string[] IdentifierColumnNames { get; }

		/// <summary>
		/// Get the name of the column used as a discriminator
		/// </summary>
		string DiscriminatorColumnName { get; }

		bool IsAbstract { get; }

		/// <summary>
		/// Does the persistent class have subclasses?
		/// </summary>
		bool HasSubclasses { get; }

		/// <summary>
		/// Get the concrete subclass corresponding to the given discriminator value
		/// </summary>
		string GetSubclassForDiscriminatorValue(object value);

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

		/// <summary> Does the result set contain rowids?</summary>
		bool HasRowId { get;}

		/// <summary>
		/// Retrieve property values from one row of a result set
		/// </summary>
		object[] Hydrate(IDataReader rs, object id, object obj, ILoadable rootLoadable, string[][] suffixedPropertyColumns,
						 bool allProperties, ISessionImplementor session);
	}
}
