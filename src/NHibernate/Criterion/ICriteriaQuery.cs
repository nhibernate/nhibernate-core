using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	public interface ICriteriaQuery
	{
		ISessionFactoryImplementor Factory { get; }

		string GetColumn(ICriteria criteria, string propertyPath);

		IType GetType(ICriteria criteria, string propertyPath);

		string[] GetColumnAliasesUsingProjection(ICriteria criteria, string propertyPath);

		string[] GetColumnsUsingProjection(ICriteria criteria, string propertyPath);

		IType GetTypeUsingProjection(ICriteria criteria, string propertyPath);

		TypedValue GetTypedValue(ICriteria criteria, string propertyPath, object value);

		string GetEntityName(ICriteria criteria);

		string GetEntityName(ICriteria criteria, string propertyPath);

		string GetSQLAlias(ICriteria subcriteria);

		string GetSQLAlias(ICriteria criteria, string propertyPath);

		string GetPropertyName(string propertyName);

		string[] GetIdentifierColumns(ICriteria subcriteria);

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
