using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.SqlCommand
{
	public class SqlCommandInfo
	{
		private SqlString text;
		private SqlType[] parameterTypes;

		public SqlCommandInfo(SqlString text, SqlType[] parameterTypes)
		{
			this.text = text;
			this.parameterTypes = parameterTypes;
		}

		public CommandType CommandType
		{
			// Always Text for now
			get { return CommandType.Text; }
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