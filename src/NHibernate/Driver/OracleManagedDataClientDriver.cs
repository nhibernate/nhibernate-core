using System;
using System.Data;
using System.Reflection;
using NHibernate.AdoNet;
using NHibernate.Engine.Query;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle.ManagedDataAccess DataProvider
	/// </summary>
	public class OracleManagedDataClientDriver : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider
	{
		private const string driverAssemblyName = "Oracle.ManagedDataAccess";
		private const string connectionTypeName = "Oracle.ManagedDataAccess.Client.OracleConnection";
		private const string commandTypeName = "Oracle.ManagedDataAccess.Client.OracleCommand";
		private static readonly SqlType GuidSqlType = new SqlType(DbType.Binary, 16);
		private readonly PropertyInfo oracleCommandBindByName;
		private readonly PropertyInfo oracleDbType;
		private readonly object oracleDbTypeRefCursor;
		private readonly object oracleDbTypeXmlType;
        private readonly object oracleDbTypeBlob;

		/// <summary>
		/// Initializes a new instance of <see cref="OracleDataClientDriver"/>.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>Oracle.ManagedDataAccess</c> assembly can not be loaded.
		/// </exception>
		public OracleManagedDataClientDriver()
			: base(
				"Oracle.ManagedDataAccess.Client",
				driverAssemblyName,
				connectionTypeName,
				commandTypeName)
		{
			var oracleCommandType = ReflectHelper.TypeFromAssembly("Oracle.ManagedDataAccess.Client.OracleCommand", driverAssemblyName, true);
			oracleCommandBindByName = oracleCommandType.GetProperty("BindByName");

			var parameterType = ReflectHelper.TypeFromAssembly("Oracle.ManagedDataAccess.Client.OracleParameter", driverAssemblyName, true);
			oracleDbType = parameterType.GetProperty("OracleDbType");

			var oracleDbTypeEnum = ReflectHelper.TypeFromAssembly("Oracle.ManagedDataAccess.Client.OracleDbType", driverAssemblyName, true);
			oracleDbTypeRefCursor = Enum.Parse(oracleDbTypeEnum, "RefCursor");
			oracleDbTypeXmlType = Enum.Parse(oracleDbTypeEnum, "XmlType");
			oracleDbTypeBlob = Enum.Parse(oracleDbTypeEnum, "Blob");
		}

		/// <summary></summary>
		public override string NamedPrefix
		{
			get { return ":"; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		/// <remarks>
		/// This adds logic to ensure that a DbType.Boolean parameter is not created since
		/// ODP.NET doesn't support it.
		/// </remarks>
		protected override void InitializeParameter(IDbDataParameter dbParam, string name, SqlType sqlType)
		{
			// if the parameter coming in contains a boolean then we need to convert it 
			// to another type since ODP.NET doesn't support DbType.Boolean
			switch (sqlType.DbType)
			{
				case DbType.Boolean:
					base.InitializeParameter(dbParam, name, SqlTypeFactory.Int16);
					break;
				case DbType.Guid:
					base.InitializeParameter(dbParam, name, GuidSqlType);
					break;
				case DbType.Xml:
					this.InitializeParameter(dbParam, name, oracleDbTypeXmlType);
					break;
                case DbType.Binary:
                    this.InitializeParameter(dbParam, name, oracleDbTypeBlob);
                    break;
				default:
					base.InitializeParameter(dbParam, name, sqlType);
					break;
			}
		}

		private void InitializeParameter(IDbDataParameter dbParam, string name, object sqlType)
		{
			dbParam.ParameterName = FormatNameForParameter(name);
			oracleDbType.SetValue(dbParam, sqlType, null);
		}

		protected override void OnBeforePrepare(IDbCommand command)
		{
			base.OnBeforePrepare(command);

			// need to explicitly turn on named parameter binding
			// http://tgaw.wordpress.com/2006/03/03/ora-01722-with-odp-and-command-parameters/
			oracleCommandBindByName.SetValue(command, true, null);

			var detail = CallableParser.Parse(command.CommandText);

			if (!detail.IsCallable)
				return;

			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = detail.FunctionName;
			oracleCommandBindByName.SetValue(command, false, null);

			var outCursor = command.CreateParameter();
			oracleDbType.SetValue(outCursor, oracleDbTypeRefCursor, null);

			outCursor.Direction = detail.HasReturn ? ParameterDirection.ReturnValue : ParameterDirection.Output;

			command.Parameters.Insert(0, outCursor);
		}

		System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass
		{
			get { return typeof (OracleDataClientBatchingBatcherFactory); }
		}
	}
}
