using System;
using System.Data;
using System.Text;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Driver
{
	/// <summary>
	/// Base class for the implementation of IDriver
	/// </summary>
	public abstract class DriverBase : IDriver
	{
		#region IDriver Members

		/// <summary></summary>
		public abstract System.Type CommandType { get; }
		/// <summary></summary>
		public abstract System.Type ConnectionType { get; }

		/// <summary></summary>
		public virtual IDbConnection CreateConnection()
		{
			return ( IDbConnection ) Activator.CreateInstance( ConnectionType );
		}

		/// <summary></summary>
		public virtual IDbCommand CreateCommand()
		{
			return ( IDbCommand ) Activator.CreateInstance( CommandType );
		}

		/// <summary></summary>
		public abstract bool UseNamedPrefixInSql { get; }

		/// <summary></summary>
		public abstract bool UseNamedPrefixInParameter { get; }

		/// <summary></summary>
		public abstract string NamedPrefix { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public string FormatNameForSql( string parameterName )
		{
			return UseNamedPrefixInSql ? ( NamedPrefix + parameterName ) : StringHelper.SqlParameter;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableAlias"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public string FormatNameForSql( string tableAlias, string parameterName )
		{
			if( !UseNamedPrefixInSql )
			{
				return StringHelper.SqlParameter;
			}


			if( tableAlias != null && tableAlias.Length > 0 )
			{
				return NamedPrefix + tableAlias + parameterName;
			}
			else
			{
				return NamedPrefix + parameterName;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public string FormatNameForParameter( string parameterName )
		{
			return UseNamedPrefixInParameter ? ( NamedPrefix + parameterName ) : parameterName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableAlias"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public string FormatNameForParameter( string tableAlias, string parameterName )
		{
			if( !UseNamedPrefixInParameter )
			{
				return parameterName;
			}


			if( tableAlias != null && tableAlias.Length > 0 )
			{
				return NamedPrefix + tableAlias + parameterName;
			}
			else
			{
				return NamedPrefix + parameterName;
			}
		}

		/// <summary></summary>
		public virtual bool SupportsMultipleOpenReaders
		{
			get { return true; }
		}

		/// <summary></summary>
		public virtual bool SupportsPreparingCommands
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="sqlString"></param>
		/// <returns></returns>
		public virtual IDbCommand GenerateCommand( Dialect.Dialect dialect, SqlString sqlString )
		{
			int paramIndex = 0;
			IDbCommand cmd = this.CreateCommand();

			StringBuilder builder = new StringBuilder( sqlString.SqlParts.Length*15 );
			for( int i = 0; i < sqlString.SqlParts.Length; i++ )
			{
				object part = sqlString.SqlParts[ i ];
				Parameter parameter = part as Parameter;

				if( parameter != null )
				{
					string paramName = "p" + paramIndex;
					builder.Append( this.FormatNameForSql( paramName ) );

					IDbDataParameter dbParam = GenerateParameter( cmd, paramName, parameter, dialect );

					cmd.Parameters.Add( dbParam );

					paramIndex++;
				}
				else
				{
					builder.Append( ( string ) part );
				}
			}

			cmd.CommandText = builder.ToString();

			return cmd;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="sqlString"></param>
		/// <returns></returns>
		public virtual IDbCommand GenerateCommand( Dialect.Dialect dialect, string sqlString )
		{
			IDbCommand cmd = this.CreateCommand();
			cmd.CommandText = sqlString;

			return cmd;
		}

		/// <summary>
		/// Generates an IDbDataParameter for the IDbCommand.  It does not add the IDbDataParameter to the IDbCommand's
		/// Parameter collection.
		/// </summary>
		/// <param name="command">The IDbCommand to use to create the IDbDataParameter.</param>
		/// <param name="name">The name to set for IDbDataParameter.Name</param>
		/// <param name="parameter">The Parameter to convert to an IDbDataParameter.</param>
		/// <param name="dialect">The Dialect to use for Default lengths if needed.</param>
		/// <returns>An IDbDataParameter ready to be added to an IDbCommand.</returns>
		/// <remarks>
		/// Drivers that require Size or Precision/Scale to be set before the IDbCommand is prepared should 
		/// override this method and use the info contained in the Parameter to set those value.  By default
		/// those values are not set, only the DbType and ParameterName are set.
		/// </remarks>
		protected virtual IDbDataParameter GenerateParameter( IDbCommand command, string name, Parameter parameter, Dialect.Dialect dialect )
		{
			IDbDataParameter dbParam = command.CreateParameter();
			dbParam.DbType = parameter.SqlType.DbType;

			dbParam.ParameterName = this.FormatNameForParameter( name );

			return dbParam;
		}

		#endregion
	}
}