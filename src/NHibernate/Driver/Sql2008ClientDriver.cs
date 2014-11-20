using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Driver
{
	public class Sql2008ClientDriver : SqlClientDriver
	{
		const byte MaxTime = 5;

		#if NETFX
		private static readonly Action<object, SqlDbType> SetSqlDbType = (p, t) => ((System.Data.SqlClient.SqlParameter) p).SqlDbType = t;
		private static readonly Action<object, string> SetTypeName = (p, t) => ((System.Data.SqlClient.SqlParameter) p).TypeName = t;
		#else
		private static readonly Action<object, SqlDbType> SetSqlDbType = DelegateHelper.BuildPropertySetter<SqlDbType>(System.Type.GetType("System.Data.SqlClient.SqlParameter, System.Data.SqlClient", true), "SqlDbType");
		private static readonly Action<object, string> SetTypeName = DelegateHelper.BuildPropertySetter<SqlDbType>(System.Type.GetType("System.Data.SqlClient.SqlParameter, System.Data.SqlClient", true), "TypeName");
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
			
			if (sqlType is StructuredSqlType type)
			{
				//NH-3736
				SetSqlDbType(dbParam, SqlDbType.Structured);
				SetTypeName(dbParam, type.TypeName);
			}
		}

		public override bool RequiresTimeSpanForTime => true;

		/// <inheritdoc />
		public override DateTime MinDate => DateTime.MinValue;
	}
}
