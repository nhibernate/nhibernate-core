using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.SqlCommand
{
	public class SqlCommandInfo
	{
		private readonly SqlString text;
		private readonly SqlType[] parameterTypes;

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

		public override string ToString()
		{
			return Text != null ? Text.ToString().Trim(): GetType().FullName;
		}
	}
}