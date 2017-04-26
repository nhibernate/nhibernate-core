using System.Data.Common;
using NHibernate.SqlCommand;

namespace NHibernate.Driver
{
	public interface IResultSetsCommand
	{
		void Append(ISqlCommand command);
		bool HasQueries { get; }
		SqlString Sql { get; }
		DbDataReader GetReader(int? commandTimeout);
	}
}