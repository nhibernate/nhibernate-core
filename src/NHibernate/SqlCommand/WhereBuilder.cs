using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Allows us to construct SQL WHERE fragments
	/// </summary>
	public class WhereBuilder : SqlBaseBuilder
	{
		public WhereBuilder(Dialect.Dialect dialect, ISessionFactoryImplementor factory)
			: base(dialect, factory) {}

		public SqlString WhereClause(string alias, string[] columnNames, IType whereType)
		{
			return ToWhereString(alias, columnNames);
		}
	}
}