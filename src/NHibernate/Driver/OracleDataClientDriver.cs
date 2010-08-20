using System.Data;
using System.Reflection;
using NHibernate.AdoNet;
using NHibernate.Engine.Query;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle.DataAccess DataProvider
	/// </summary>
	/// <remarks>
	/// Code was contributed by <a href="http://sourceforge.net/users/jemcalgary/">James Mills</a>
	/// on the NHibernate forums in this 
	/// <a href="http://sourceforge.net/forum/message.php?msg_id=2952662">post</a>.
	/// </remarks>
	public class OracleDataClientDriver : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider
	{
		private const string driverAssemblyName = "Oracle.DataAccess";
		private const string connectionTypeName = "Oracle.DataAccess.Client.OracleConnection";
		private const string commandTypeName = "Oracle.DataAccess.Client.OracleCommand";
		private static readonly SqlType GuidSqlType = new SqlType(DbType.Binary, 16);
		private readonly PropertyInfo oracleCommandBindByName;
		private readonly PropertyInfo oracleDbType;
		private readonly object oracleDbTypeRefCursor;

		/// <summary>
		/// Initializes a new instance of <see cref="OracleDataClientDriver"/>.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>Oracle.DataAccess</c> assembly can not be loaded.
		/// </exception>
		public OracleDataClientDriver()
			: base(
			"Oracle.DataAccess.Client",
			driverAssemblyName,
			connectionTypeName,
			commandTypeName)
		{
			System.Type oracleCommandType = ReflectHelper.TypeFromAssembly("Oracle.DataAccess.Client.OracleCommand", driverAssemblyName, false);
			oracleCommandBindByName = oracleCommandType.GetProperty("BindByName");

			System.Type parameterType = ReflectHelper.TypeFromAssembly("Oracle.DataAccess.Client.OracleParameter", driverAssemblyName, false);
			oracleDbType = parameterType.GetProperty("OracleDbType");

			System.Type oracleDbTypeEnum = ReflectHelper.TypeFromAssembly("Oracle.DataAccess.Client.OracleDbType", driverAssemblyName, false);
			oracleDbTypeRefCursor = System.Enum.Parse(oracleDbTypeEnum, "RefCursor");
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string NamedPrefix
		{
			get { return ":"; }
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
				default:
					base.InitializeParameter(dbParam, name, sqlType);
					break;
			}
		}

		protected override void OnBeforePrepare(IDbCommand command)
		{
			base.OnBeforePrepare(command);

			// need to explicitly turn on named parameter binding
			// http://tgaw.wordpress.com/2006/03/03/ora-01722-with-odp-and-command-parameters/
			oracleCommandBindByName.SetValue(command, true, null);

			CallableParser.Detail detail = CallableParser.Parse(command.CommandText);

			if (!detail.IsCallable)
				return;

			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = detail.FunctionName;
			oracleCommandBindByName.SetValue(command, false, null);

			IDbDataParameter outCursor = command.CreateParameter();
			oracleDbType.SetValue(outCursor, oracleDbTypeRefCursor, null);

			outCursor.Direction = detail.HasReturn ? ParameterDirection.ReturnValue : ParameterDirection.Output;

			command.Parameters.Insert(0, outCursor);
		}

		#region IEmbeddedBatcherFactoryProvider Members

		System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass
		{
			get { return typeof (OracleDataClientBatchingBatcherFactory); }
		}

		#endregion
	}
}