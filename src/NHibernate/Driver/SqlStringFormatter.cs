using System;
using System.Text;
using NHibernate.SqlCommand;

namespace NHibernate.Driver
{
	public class SqlStringFormatter : ISqlStringVisitor
	{
		private StringBuilder result = new StringBuilder();
		private int parameterIndex = 0;
		private ISqlParameterFormatter formatter;

		public SqlStringFormatter(ISqlParameterFormatter formatter)
		{
			this.formatter = formatter;
		}

		public void Format(SqlString text)
		{
			text.Visit(this);
		}

		public string GetFormattedText()
		{
			return result.ToString();
		}

		void ISqlStringVisitor.String(string text)
		{
			result.Append(text);
		}

		void ISqlStringVisitor.Parameter()
		{
			string name = formatter.GetParameterName(parameterIndex);
			parameterIndex++;
			result.Append(name);
		}
	}
}