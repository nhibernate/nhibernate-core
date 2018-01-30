#if !NETSTANDARD2_0 || DRIVER_PACKAGE
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace NHibernate.Driver
{
#if DRIVER_PACKAGE
	public class SqlServer2008Driver : SqlServer2000Driver
#else
	[Obsolete("Use NHibernate.Driver.SqlServer NuGet package and SqlServer2008Driver."
		+ "  There are also Loquacious configuration points: .Connection.BySqlServer2008Driver() and .DataBaseIntegration(x => x.SqlServer2008Driver()).")]
	public class Sql2008ClientDriver : SqlClientDriver
#endif
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
#endif
