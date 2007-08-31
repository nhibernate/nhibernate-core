using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Expression
{
	public interface ICriteriaQuery
	{
		ISessionFactoryImplementor Factory { get; }

		string GetColumn(ICriteria criteria, string propertyPath);

		IType GetType(ICriteria criteria, string propertyPath);

		IType GetPropertyType(ICriteria criteria, string propertyPath);

		TypedValue GetTypedValue(ICriteria criteria, string propertyPath, object value);

		System.Type GetEntityName(ICriteria criteria);

		System.Type GetEntityName(ICriteria criteria, string propertyPath);

		string GetSQLAlias(ICriteria subcriteria);

		string GetSQLAlias(ICriteria criteria, string propertyPath);

		string GetPropertyName(string propertyName);

		string[] GetIdentifierColumns(ICriteria subcriteria);

		IType GetIdentifierType(ICriteria subcriteria);

		TypedValue GetTypedIdentifierValue(ICriteria subcriteria, object value);

		string GenerateSQLAlias();

		// Column names are used in expressions (it's assumed that all expressions
		// end up in the WHERE clause, not in HAVING). Column aliases are used in ORDER BY.
		string[] GetPropertyColumnNames(ICriteria criteria, string propertyPath);
		string[] GetPropertyColumnAliases(ICriteria criteria, string propertyPath);
    }
}