using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace NHibernate.Driver
{
	public class Sql2008ClientDriver : SqlClientDriver
	{
		const byte MaxTime = 5;

		protected override void InitializeParameter(DbParameter dbParam, string name, SqlTypes.SqlType sqlType)
		{
			base.InitializeParameter(dbParam, name, sqlType);
			switch (sqlType.DbType)
			{
				case DbType.Time:
					((SqlParameter) dbParam).SqlDbType = SqlDbType.Time;
					dbParam.Size = MaxTime;
					break;
				case DbType.Date:
					((SqlParameter) dbParam).SqlDbType = SqlDbType.Date;
					break;
			}
		}

		public override bool RequiresTimeSpanForTime => true;

		/// <inheritdoc />
		public override DateTime MinDate => DateTime.MinValue;
	}
}
