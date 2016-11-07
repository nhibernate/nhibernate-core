using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// A <c>ClassPersister</c> that may be loaded by outer join using
	/// the <c>OuterJoinLoader</c> hierarchy and may be an element
	/// of a one-to-many association.
	/// </summary>
	public interface IOuterJoinLoadable : ILoadable, IJoinable
	{
		// <summary>
		// Get the name of the column used as a discriminator
		// </summary>
		//string DiscriminatorColumnName { get; }

		EntityType EntityType { get; }

		/// <summary>
		/// Generate a list of collection index and element columns
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		string SelectFragment(string alias, string suffix);

		/// <summary>
		/// How many properties are there, for this class and all subclasses? (optional operation)
		/// </summary>
		/// <returns></returns>
		int CountSubclassProperties();

		/// <summary>
		/// May this property be fetched using an SQL outerjoin?
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		FetchMode GetFetchMode(int i);

		/// <summary>
		/// Get the cascade style of this (subclass closure) property
		/// </summary>
		CascadeStyle GetCascadeStyle(int i);

		/// <summary>
		/// Is this property defined on a subclass of the mapped class?
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		bool IsDefinedOnSubclass(int i);

		/// <summary>
		/// Get an array of the types of all properties of all subclasses (optional operation)
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		IType GetSubclassPropertyType(int i);

		/// <summary>
		/// Get the name of the numbered property of the class or a subclass
		/// (optional operation)
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		string GetSubclassPropertyName(int i);

		/// <summary>
		/// Is the numbered property of the class of subclass nullable?
		/// </summary>
		bool IsSubclassPropertyNullable(int i);

		/// <summary>
		/// Return the column names used to persist all properties of all sublasses of the persistent class
		/// (optional operation)
		/// </summary>
		string[] GetSubclassPropertyColumnNames(int i);

		/// <summary>
		/// Return the table name used to persist the numbered property of 
		/// the class or a subclass
		/// (optional operation)
		/// </summary>
		string GetSubclassPropertyTableName(int i);

		/// <summary>
		/// Given the number of a property of a subclass, and a table alias, return the aliased column names
		/// (optional operation)
		/// </summary>
		/// <param name="name"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		string[] ToColumns(string name, int i);

		/// <summary>
		/// Get the main from table fragment, given a query alias (optional operation)
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		string FromTableFragment(string alias);

		/// <summary>
		/// Get the column names for the given property path
		/// </summary>
		string[] GetPropertyColumnNames(string propertyPath);

		/// <summary>
		/// Get the table name for the given property path
		/// </summary>
		string GetPropertyTableName(string propertyName);

		/// <summary>
		/// Return the aliased identifier column names
		/// </summary>
		string[] ToIdentifierColumns(string alias);

		/// <summary>
		/// Get the table alias used for the supplied column
		/// </summary>
		string GenerateTableAliasForColumn(string rootAlias, string column);
	}
}