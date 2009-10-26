using System.Data;
using NHibernate.Engine.Query;
using NHibernate.SqlTypes;

namespace NHibernate.SqlCommand
{
	public class SqlCommandInfo
	{
		private readonly CommandType commandType;
		private readonly SqlString text;
		private readonly SqlType[] parameterTypes;

		public SqlCommandInfo(SqlString text, SqlType[] parameterTypes)
		{
			this.text = text;
			this.parameterTypes = parameterTypes;
			this.commandType = CommandType.Text;
		}

		public SqlCommandInfo(SqlString text, bool isStoredProcedure, SqlType[] parameterTypes)
			: this(text, parameterTypes)
		{
			if (isStoredProcedure)
			{
				this.commandType = CommandType.StoredProcedure;
				this.text = CallableParser.Parse(text.ToString());
			}
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

		public override string ToString()
		{
			return Text != null ? Text.ToString().Trim(): GetType().FullName;
		}
	}
}