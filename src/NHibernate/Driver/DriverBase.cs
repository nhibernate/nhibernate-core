using System;
using System.Data;

using NHibernate.Util;

namespace NHibernate.Driver
{
	/// <summary>
	/// Base class for the implementation of IDriver
	/// </summary>
	public abstract class DriverBase : IDriver
	{
		public DriverBase()
		{
		}

		#region IDriver Members

		public abstract System.Type CommandType {get; }
		public abstract System.Type ConnectionType {get; }

		public virtual IDbConnection CreateConnection() 
		{
			return (IDbConnection) Activator.CreateInstance(ConnectionType);
		}

		public virtual IDbCommand CreateCommand()
		{
			return (IDbCommand) Activator.CreateInstance(CommandType);
		}

		public abstract bool UseNamedPrefixInSql {get;}

		public abstract bool UseNamedPrefixInParameter {get;}

		public abstract string NamedPrefix {get;}

		public string FormatNameForSql(string parameterName)
		{
			return UseNamedPrefixInSql ? (NamedPrefix + parameterName): StringHelper.SqlParameter;
		}

		public string FormatNameForSql(string tableAlias, string parameterName)
		{
			if(!UseNamedPrefixInSql) return StringHelper.SqlParameter;

			
			if(tableAlias!=null && tableAlias!=String.Empty) 
			{
				return NamedPrefix + tableAlias + parameterName;
			}
			else 
			{
				return NamedPrefix + parameterName;
			}
		}

		public string FormatNameForParameter(string parameterName)
		{
			return UseNamedPrefixInParameter ? (NamedPrefix + parameterName) : parameterName;
		}

		public string FormatNameForParameter(string tableAlias, string parameterName)
		{
			if(!UseNamedPrefixInParameter) return parameterName;

			
			if(tableAlias!=null && tableAlias!=String.Empty) 
			{
				return NamedPrefix + tableAlias + parameterName;
			}
			else 
			{
				return NamedPrefix + parameterName;
			}
		}

		public virtual bool SupportsMultipleOpenReaders 
		{
			get { return true;}
		}

		public virtual bool SupportsPreparingCommands 
		{ 
			get { return true; }
		}

		#endregion
	}
}
