using System;
using System.Data;
using System.Data.Common;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate driver for Microsoft SQL Server CE data provider
	/// </summary>
	public class SqlServerCeDriver : ReflectionBasedDriver
	{
		private static readonly Action<object, SqlDbType> SetSqlDbType =
			DelegateHelper.BuildPropertySetter<SqlDbType>(
				System.Type.GetType("System.Data.SqlServerCe.SqlCeParameter, System.Data.SqlServerCe", true),
				"SqlDbType");

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerCeDriver"/> class.
		/// </summary>
		public SqlServerCeDriver()
			: base(
				"System.Data.SqlServerCe",
				"System.Data.SqlServerCe.SqlCeConnection",
				"System.Data.SqlServerCe.SqlCeCommand")
		{
		}

		/// <summary>
		/// MsSql requires the use of a Named Prefix in the SQL statement.  
		/// </summary>
		/// <remarks>
		/// <see langword="true" /> because MsSql uses "<c>@</c>".
		/// </remarks>
		public override bool UseNamedPrefixInSql => true;

		/// <summary>
		/// MsSql requires the use of a Named Prefix in the Parameter.  
		/// </summary>
		/// <remarks>
		/// <see langword="true" /> because MsSql uses "<c>@</c>".
		/// </remarks>
		public override bool UseNamedPrefixInParameter => true;

		/// <summary>
		/// The Named Prefix for parameters.  
		/// </summary>
		/// <value>
		/// Sql Server uses <c>"@"</c>.
		/// </value>
		public override string NamedPrefix => "@";

		/// <summary>
		/// The SqlClient driver does NOT support more than 1 open DbDataReader
		/// with only 1 DbConnection.
		/// </summary>
		/// <value><see langword="false" /> - it is not supported.</value>
		/// <remarks>
		/// Ms Sql 2000 (and 7) throws an Exception when multiple DataReaders are 
		/// attempted to be Opened.  When Yukon comes out a new Driver will be 
		/// created for Yukon because it is supposed to support it.
		/// </remarks>
		public override bool SupportsMultipleOpenReaders => false;

		protected override void SetCommandTimeout(DbCommand cmd)
		{
		}

		public override IResultSetsCommand GetResultSetsCommand(Engine.ISessionImplementor session)
		{
			return new BasicResultSetsCommand(session);
		}

		protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
		{
			base.InitializeParameter(dbParam, name, AdjustSqlType(sqlType));

			AdjustDbParamTypeForLargeObjects(dbParam, sqlType);
		}

		private static SqlType AdjustSqlType(SqlType sqlType)
		{
			switch (sqlType.DbType)
			{
				case DbType.AnsiString:
					return new StringSqlType(sqlType.Length);
				case DbType.AnsiStringFixedLength:
					return new StringFixedLengthSqlType(sqlType.Length);
				case DbType.Date:
					return SqlTypeFactory.DateTime;
				case DbType.Time:
					return SqlTypeFactory.DateTime;
				default:
					return sqlType;
			}
		}

		private void AdjustDbParamTypeForLargeObjects(DbParameter dbParam, SqlType sqlType)
		{
			if (sqlType is BinaryBlobSqlType)
			{
				SetSqlDbType(dbParam, SqlDbType.Image);
			}
			else if (sqlType is StringClobSqlType)
			{
				SetSqlDbType(dbParam, SqlDbType.NText);
			}
		}

		public override bool SupportsNullEnlistment => false;

		/// <summary>
		/// <see langword="false"/>. Enlistment is completely disabled when auto-enlistment is disabled.
		/// <see cref="DbConnection.EnlistTransaction(System.Transactions.Transaction)"/> does nothing in
		/// this case.
		/// </summary>
		public override bool SupportsEnlistmentWhenAutoEnlistmentIsDisabled => false;

		/// <inheritdoc />
		public override DateTime MinDate => new DateTime(1753, 1, 1);
	}
}
