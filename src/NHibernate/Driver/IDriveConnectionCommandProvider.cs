using System.Data;

namespace NHibernate.Driver
{
	public interface IDriveConnectionCommandProvider
	{
		IDbConnection CreateConnection();
		IDbCommand CreateCommand();
	}
}