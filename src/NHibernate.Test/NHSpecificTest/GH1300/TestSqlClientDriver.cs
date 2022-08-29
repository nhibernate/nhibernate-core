using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using NHibernate.Driver;

namespace NHibernate.Test.NHSpecificTest.GH1300
{
	public class TestSqlClientDriver : SqlClientDriver
	{
		public List<SqlParameter> LastCommandParameters { get; private set; } = new List<SqlParameter>();

		public override void AdjustCommand(DbCommand command)
		{
			base.AdjustCommand(command);
			LastCommandParameters = command.Parameters.OfType<SqlParameter>().ToList();
		}
		public void ClearCommands()
		{
			LastCommandParameters.Clear();
		}
	}
}
