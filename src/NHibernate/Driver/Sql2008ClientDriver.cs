using System;
using System.Data;
using System.Data.Common;
using NHibernate.Util;

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

		#if NETFX || DRIVER_PACKAGE
		private static readonly Action<object, SqlDbType> SetSqlDbType = (p, t) => ((System.Data.SqlClient.SqlParameter) p).SqlDbType = t;
		#else
		private static readonly Action<object, SqlDbType> SetSqlDbType = DelegateHelper.BuildPropertySetter<SqlDbType>(System.Type.GetType("System.Data.SqlClient.SqlParameter, System.Data.SqlClient", true), "SqlDbType");
		#endif

		protected override void InitializeParameter(DbParameter dbParam, string name, SqlTypes.SqlType sqlType)
		{
			base.InitializeParameter(dbParam, name, sqlType);
			switch (sqlType.DbType)
			{
				case DbType.Time:
					SetSqlDbType(dbParam, SqlDbType.Time);
					dbParam.Size = MaxTime;
					break;
				case DbType.Date:
					SetSqlDbType(dbParam, SqlDbType.Date);
					break;
			}
		}

		public override bool RequiresTimeSpanForTime => true;

		/// <inheritdoc />
		public override DateTime MinDate => DateTime.MinValue;
	}
}
