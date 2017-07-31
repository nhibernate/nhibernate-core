using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using NHibernate.AdoNet;
using NHibernate.Engine.Query;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate driver base for using ODP.Net.
	/// </summary>
	/// <remarks>
	/// Original code was contributed by <a href="http://sourceforge.net/users/jemcalgary/">James Mills</a>
	/// on the NHibernate forums in this 
	/// <a href="http://sourceforge.net/forum/message.php?msg_id=2952662">post</a>.
	/// </remarks>
	public abstract class OracleDataClientDriverBase : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider
	{
		private const string _commandClassName = "OracleCommand";

		private static readonly SqlType _guidSqlType = new SqlType(DbType.Binary, 16);
		private readonly PropertyInfo _oracleCommandBindByName;
		private readonly PropertyInfo _oracleDbType;
		private readonly object _oracleDbTypeRefCursor;
		private readonly object _oracleDbTypeXmlType;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="assemblyName">The assembly name of the managed or unmanage driver. Namespaces will be derived from it.</param>
		/// <exception cref="HibernateException">
		/// Thrown when the requested assembly can not be loaded.
		/// </exception>
		protected OracleDataClientDriverBase(string assemblyName)
			: this(assemblyName, assemblyName + ".Client")
		{
		}

		private OracleDataClientDriverBase(string driverAssemblyName, string clientNamespace)
			: base(clientNamespace, driverAssemblyName, clientNamespace + ".OracleConnection", clientNamespace + "." + _commandClassName)
		{
			var oracleCommandType = ReflectHelper.TypeFromAssembly(clientNamespace + "." + _commandClassName, driverAssemblyName, true);
			_oracleCommandBindByName = oracleCommandType.GetProperty("BindByName");

			var parameterType = ReflectHelper.TypeFromAssembly(clientNamespace + ".OracleParameter", driverAssemblyName, true);
			_oracleDbType = parameterType.GetProperty("OracleDbType");

			var oracleDbTypeEnum = ReflectHelper.TypeFromAssembly(clientNamespace + ".OracleDbType", driverAssemblyName, true);
			_oracleDbTypeRefCursor = Enum.Parse(oracleDbTypeEnum, "RefCursor");
			_oracleDbTypeXmlType = Enum.Parse(oracleDbTypeEnum, "XmlType");
		}

		/// <inheritdoc/>
		public override bool UseNamedPrefixInSql => true;

		/// <inheritdoc/>
		public override bool UseNamedPrefixInParameter => true;

		/// <inheritdoc/>
		public override string NamedPrefix => ":";

		/// <remarks>
		/// Add logic to ensure that a <see cref="DbType.Boolean"/> parameter is not created since
		/// ODP.NET doesn't support it. Handle <see cref="DbType.Guid"/> and <see cref="DbType.Xml"/> cases too.
		/// Adjust <see cref="DbType.String"/> resulting type if needed.
		/// </remarks>
		protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
		{
			switch (sqlType.DbType)
			{
				case DbType.Boolean:
					// if the parameter coming in contains a boolean then we need to convert it 
					// to another type since ODP.NET doesn't support DbType.Boolean
					base.InitializeParameter(dbParam, name, SqlTypeFactory.Int16);
					break;
				case DbType.Guid:
					base.InitializeParameter(dbParam, name, _guidSqlType);
					break;
				case DbType.Xml:
					InitializeParameter(dbParam, name, _oracleDbTypeXmlType);
					break;
				default:
					base.InitializeParameter(dbParam, name, sqlType);
					break;
			}
		}

		private void InitializeParameter(DbParameter dbParam, string name, object sqlType)
		{
			dbParam.ParameterName = FormatNameForParameter(name);
			_oracleDbType.SetValue(dbParam, sqlType, null);
		}

		protected override void OnBeforePrepare(DbCommand command)
		{
			base.OnBeforePrepare(command);

			// need to explicitly turn on named parameter binding
			// http://tgaw.wordpress.com/2006/03/03/ora-01722-with-odp-and-command-parameters/
			_oracleCommandBindByName.SetValue(command, true, null);

			var detail = CallableParser.Parse(command.CommandText);

			if (!detail.IsCallable)
				return;

			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = detail.FunctionName;
			_oracleCommandBindByName.SetValue(command, false, null);

			var outCursor = command.CreateParameter();
			_oracleDbType.SetValue(outCursor, _oracleDbTypeRefCursor, null);

			outCursor.Direction = detail.HasReturn ? ParameterDirection.ReturnValue : ParameterDirection.Output;

			command.Parameters.Insert(0, outCursor);
		}

		System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass => typeof(OracleDataClientBatchingBatcherFactory);
	}
}