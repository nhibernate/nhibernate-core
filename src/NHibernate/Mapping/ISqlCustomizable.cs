using NHibernate.Engine;

namespace NHibernate.Mapping
{
	public interface ISqlCustomizable
	{
		void SetCustomSQLInsert(string sql, bool callable, ExecuteUpdateResultCheckStyle checkStyle);
		void SetCustomSQLUpdate(string sql, bool callable, ExecuteUpdateResultCheckStyle checkStyle);
		void SetCustomSQLDelete(string sql, bool callable, ExecuteUpdateResultCheckStyle checkStyle);
	}
}