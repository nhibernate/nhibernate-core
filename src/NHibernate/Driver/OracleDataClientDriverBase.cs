using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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

		private readonly Action<object, bool> _commandBindByNameSetter;
		private readonly Action<object, object> _parameterOracleDbTypeSetter;
		private readonly object _oracleDbTypeRefCursor;
		private readonly object _oracleDbTypeXmlType;
		private readonly object _oracleDbTypeBlob;
		private readonly object _oracleDbTypeNVarchar2;
		private readonly object _oracleDbTypeNChar;

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
			_commandBindByNameSetter = DelegateHelper.BuildPropertySetter<bool>(oracleCommandType, "BindByName");

			var parameterType = ReflectHelper.TypeFromAssembly(clientNamespace + ".OracleParameter", driverAssemblyName, true);
			_parameterOracleDbTypeSetter = DelegateHelper.BuildPropertySetter<object>(parameterType, "OracleDbType");

			var oracleDbTypeEnum = ReflectHelper.TypeFromAssembly(clientNamespace + ".OracleDbType", driverAssemblyName, true);
			_oracleDbTypeRefCursor = Enum.Parse(oracleDbTypeEnum, "RefCursor");
			_oracleDbTypeXmlType = Enum.Parse(oracleDbTypeEnum, "XmlType");
			_oracleDbTypeBlob = Enum.Parse(oracleDbTypeEnum, "Blob");
			_oracleDbTypeNVarchar2 = Enum.Parse(oracleDbTypeEnum, "NVarchar2");
			_oracleDbTypeNChar = Enum.Parse(oracleDbTypeEnum, "NChar");
		}

		/// <inheritdoc/>
		public override void Configure(IDictionary<string, string> settings)
		{
			base.Configure(settings);

			// If changing the default value, keep it in sync with Oracle8iDialect.Configure.
			UseNPrefixedTypesForUnicode = PropertiesHelper.GetBoolean(Cfg.Environment.OracleUseNPrefixedTypesForUnicode, settings, false);
		}

		/// <summary>
		/// <para>Oracle has a dual Unicode support model.</para>
		/// <para>Either the whole database use an Unicode encoding, and then all string types
		/// will be Unicode. In such case, Unicode strings should be mapped to non <c>N</c> prefixed
		/// types, such as <c>Varchar2</c>. This is the default.</para>
		/// <para>Or <c>N</c> prefixed types such as <c>NVarchar2</c> are to be used for Unicode strings.</para>
		/// <para>This property is set according to <see cref="Cfg.Environment.OracleUseNPrefixedTypesForUnicode"/>
		/// configuration parameter.</para>
		/// </summary>
		/// <remarks>
		/// See https://docs.oracle.com/cd/B19306_01/server.102/b14225/ch6unicode.htm#CACHCAHF
		/// https://docs.oracle.com/database/121/ODPNT/featOraCommand.htm#i1007557
		/// </remarks>
		public bool UseNPrefixedTypesForUnicode { get; private set; }

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
				case DbType.Binary:
					InitializeParameter(dbParam, name, _oracleDbTypeBlob);
					break;
				case DbType.String:
					if (UseNPrefixedTypesForUnicode)
						InitializeParameter(dbParam, name, _oracleDbTypeNVarchar2);
					else
						base.InitializeParameter(dbParam, name, sqlType);
					break;
				case DbType.StringFixedLength:
					if (UseNPrefixedTypesForUnicode)
						InitializeParameter(dbParam, name, _oracleDbTypeNChar);
					else
						base.InitializeParameter(dbParam, name, sqlType);
					break;
				default:
					base.InitializeParameter(dbParam, name, sqlType);
					break;
			}
		}

		private void InitializeParameter(DbParameter dbParam, string name, object oracleDbType)
		{
			dbParam.ParameterName = FormatNameForParameter(name);
			_parameterOracleDbTypeSetter(dbParam, oracleDbType);
		}

		protected override void OnBeforePrepare(DbCommand command)
		{
			base.OnBeforePrepare(command);

			// need to explicitly turn on named parameter binding
			// http://tgaw.wordpress.com/2006/03/03/ora-01722-with-odp-and-command-parameters/
			_commandBindByNameSetter(command, true);

			var detail = CallableParser.Parse(command.CommandText);

			if (!detail.IsCallable)
				return;

			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = detail.FunctionName;
			_commandBindByNameSetter(command, false);

			var outCursor = command.CreateParameter();
			_parameterOracleDbTypeSetter(outCursor, _oracleDbTypeRefCursor);

			outCursor.Direction = detail.HasReturn ? ParameterDirection.ReturnValue : ParameterDirection.Output;

			command.Parameters.Insert(0, outCursor);
		}

		System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass => typeof(OracleDataClientBatchingBatcherFactory);
	}
}
