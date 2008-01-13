using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Expressions
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
	}
}