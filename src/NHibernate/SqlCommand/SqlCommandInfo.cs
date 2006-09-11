using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.SqlCommand
{
	public class SqlCommandInfo
	{
		private CommandType commandType;
		private SqlString text;
		private SqlType[] parameterTypes;
		
		public SqlCommandInfo(CommandType commandType, SqlString text, SqlType[] parameterTypes)
		{
			this.commandType = commandType;
			this.text = text;
			this.parameterTypes = parameterTypes;
		}
		
		public CommandType CommandType
		{
			get { return commandType; }
		}
		
		public SqlString Text
		{
			get { return text; }
		}
		
		public SqlType[] ParameterTypes
		{
			get { return parameterTypes; }
		}
	}
}
