using System.Data;

using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the SqlClient DataProvider
	/// </summary>
	public class SqlServerCeDriver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlClientDriver"/> class.
		/// </summary>
		public SqlServerCeDriver() : base(
			"System.Data.SqlServerCe",
			"System.Data.SqlServerCe.SqlCeConnection",
			"System.Data.SqlServerCe.SqlCeCommand" )
		{
		}

		/// <summary>
		/// MsSql requires the use of a Named Prefix in the SQL statement.  
		/// </summary>
		/// <remarks>
		/// <c>true</c> because MsSql uses "<c>@</c>".
		/// </remarks>
		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		/// <summary>
		/// MsSql requires the use of a Named Prefix in the Parameter.  
		/// </summary>
		/// <remarks>
		/// <c>true</c> because MsSql uses "<c>@</c>".
		/// </remarks>
		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		/// <summary>
		/// The Named Prefix for parameters.  
		/// </summary>
		/// <value>
		/// Sql Server uses <c>"@"</c>.
		/// </value>
		public override string NamedPrefix
		{
			get { return "@"; }
		}

		/// <summary>
		/// The SqlClient driver does NOT support more than 1 open IDataReader
		/// with only 1 IDbConnection.
		/// </summary>
		/// <value><c>false</c> - it is not supported.</value>
		/// <remarks>
		/// Ms Sql 2000 (and 7) throws an Exception when multiple DataReaders are 
		/// attempted to be Opened.  When Yukon comes out a new Driver will be 
		/// created for Yukon because it is supposed to support it.
		/// </remarks>
		public override bool SupportsMultipleOpenReaders
		{
			get { return false; }
		}

		/// <summary>
		/// Generates an IDbDataParameter that has values for the Size or Precision/Scale Properties set.
		/// </summary>
		/// <param name="command">The IDbCommand to use to create the IDbDataParameter.</param>
		/// <param name="name">The name to set for IDbDataParameter.Name</param>
		/// <param name="parameter">The Parameter to convert to an IDbDataParameter.</param>
		/// <param name="dialect">The Dialect to use for Default lengths if needed.</param>
		/// <returns>An IDbDataParameter ready to be added to an IDbCommand.</returns>
		/// <remarks>
		/// In order to prepare an IDbCommand against an MsSql database all variable length values need
		/// to be set.
		/// </remarks>
		protected override IDbDataParameter GenerateParameter( IDbCommand command, string name, Parameter parameter, Dialect.Dialect dialect )
		{
			IDbDataParameter dbParam = base.GenerateParameter( command, name, parameter, dialect );

			// take a look at the SqlType and determine if this is one that MsSql needs to set Size or
			// Precision/Scale for

			// if this parameter needs to have a value set for the Size or Precision/Scale besides the default
			// value of the Dialect then one of these will not be null
			ParameterLength pl = null;
			ParameterPrecisionScale pps = null;

			switch( parameter.SqlType.DbType )
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
					pl = parameter as ParameterLength;
					dbParam.Size = dialect.MaxAnsiStringSize;
					break;

				case DbType.Binary:
					pl = parameter as ParameterLength;
					if( parameter.SqlType is BinaryBlobSqlType )
					{
						dbParam.Size = dialect.MaxBinaryBlobSize;
					}
					else
					{
						dbParam.Size = dialect.MaxBinarySize;
					}
					break;

				case DbType.String:
				case DbType.StringFixedLength:
					pl = parameter as ParameterLength;
					if( parameter.SqlType is StringClobSqlType )
					{
						dbParam.Size = dialect.MaxStringClobSize;
					}
					else
					{
						dbParam.Size = dialect.MaxStringSize;
					}
					break;
				case DbType.Decimal:
					pps = parameter as ParameterPrecisionScale;
					//TODO: remove this hardcoding...
					dbParam.Precision = 19;
					dbParam.Scale = 5;
					break;
			}

			// if one of these is not null then override the default values
			// set in the earlier switch statement.
			if( pl != null )
			{
				dbParam.Size = pl.Length;
			}
			else if( pps != null )
			{
				dbParam.Precision = pps.Precision;
				dbParam.Scale = pps.Scale;
			}

			return dbParam;

		}

	}
}