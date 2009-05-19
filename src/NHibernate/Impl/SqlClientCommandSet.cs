using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace NHibernate.Impl
{
	internal class SqlClientCommandSet : DbCommandSet<SqlConnection, System.Data.SqlClient.SqlCommand>
	{
		private static readonly System.Type sqlCmdSetType;

		static SqlClientCommandSet()
		{
			Assembly sysData = typeof (IDbConnection).Assembly;
			sqlCmdSetType = sysData.GetType("System.Data.SqlClient.SqlCommandSet");
		}

		protected override object CreateInternalCommandSet()
		{
			if (sqlCmdSetType == null)
			{
				throw new HibernateException("Could not find SqlCommandSet" + Environment.NewLine
				                             + "If you are running on Mono, batching support isn't implemented on Mono"
				                             + Environment.NewLine
				                             + "If you are running on Microsoft .NET, this probably means that internal details"
				                             + Environment.NewLine
				                             +
				                             "of the BCL that we rely on to allow this have changed, this is a bug. Please inform the developers");
			}
			return Activator.CreateInstance(sqlCmdSetType, true);
		}
	}
}