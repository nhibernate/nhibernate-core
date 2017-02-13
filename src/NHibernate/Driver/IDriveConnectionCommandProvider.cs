using System.Data.Common;

namespace NHibernate.Driver
{
	public interface IDriveConnectionCommandProvider
	{
		DbConnection CreateConnection();
		DbCommand CreateCommand();
	}
}