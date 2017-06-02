using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace NHibernate.Driver
{
	public class Sql2008ClientDriver : SqlClientDriver
	{
		protected override void InitializeParameter(DbParameter dbParam, string name, SqlTypes.SqlType sqlType)
		{
			base.InitializeParameter(dbParam, name, sqlType);
			if (sqlType.DbType == DbType.Time)
			{
				((SqlParameter) dbParam).SqlDbType = SqlDbType.Time;
			}
		}

		public override bool RequiresTimeSpanForTime => true;
	}
}