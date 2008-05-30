using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	/// <summary> 
	/// An instance of <see cref="ICriteriaQuery"/> is passed to criterion, 
	/// order and projection instances when actually compiling and
	/// executing the query. This interface is not used by application
	/// code. 
	/// </summary>
	public interface ICriteriaQuery
	{
		ISessionFactoryImplementor Factory { get; }

		/// <summary> Get the names of the columns mapped by a property path, ignoring projection aliases </summary>
		string GetColumn(ICriteria criteria, string propertyPath);

		/// <summary> Get the type of a property path, ignoring projection aliases</summary>
		IType GetType(ICriteria criteria, string propertyPath);

		string[] GetColumnAliasesUsingProjection(ICriteria criteria, string propertyPath);

		/// <summary> Get the names of the columns mapped by a property path</summary>
		string[] GetColumnsUsingProjection(ICriteria criteria, string propertyPath);

		/// <summary> Get the type of a property path</summary>
		IType GetTypeUsingProjection(ICriteria criteria, string propertyPath);

		/// <summary> Get the a typed value for the given property value.</summary>
		TypedValue GetTypedValue(ICriteria criteria, string propertyPath, object value);

		/// <summary> Get the entity name of an entity</summary>
		string GetEntityName(ICriteria criteria);

		/// <summary> 
		/// Get the entity name of an entity, taking into account
		/// the qualifier of the property path
		/// </summary>
		string GetEntityName(ICriteria criteria, string propertyPath);

		/// <summary> Get the root table alias of an entity</summary>
		string GetSQLAlias(ICriteria subcriteria);

		/// <summary> 
		/// Get the root table alias of an entity, taking into account
		/// the qualifier of the property path
		/// </summary>
		string GetSQLAlias(ICriteria criteria, string propertyPath);

		/// <summary> Get the property name, given a possibly qualified property name</summary>
		string GetPropertyName(string propertyName);

		/// <summary> Get the identifier column names of this entity</summary>
		string[] GetIdentifierColumns(ICriteria subcriteria);

		/// <summary> Get the identifier type of this entity</summary>
		IType GetIdentifierType(ICriteria subcriteria);

		TypedValue GetTypedIdentifierValue(ICriteria subcriteria, object value);

		string GenerateSQLAlias();

		int GetIndexForAlias();

		/// <summary>
		/// When adding values to the query string it is imparative that they are reported via this function back to the query builder. 
		/// Do not report the same item multiple times as it will be assumed to be a separate parameter.
		/// </summary>
		void AddUsedTypedValues(TypedValue [] values);
	}
}
