using System.Data;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Driver
{
	public interface IResultSetsCommand
	{
		void Append(SqlCommandInfo commandInfo);
		int ParametersCount { get; }
		bool HasQueries { get; }
		SqlString Sql { get; }
		IDataReader GetReader(QueryParameters[] queryParameters, int? commandTimeout);
	}
}