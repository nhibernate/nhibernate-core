using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.SqlCommand;
using NHibernate.Engine.Query;

namespace NHibernate.Driver
{
	public class SqlStringFormatter : ISqlStringVisitor
	{
		private readonly StringBuilder result = new StringBuilder();
		private int parameterIndex = 0;
		private readonly ISqlParameterFormatter formatter;
		private readonly string multipleQueriesSeparator;
		private bool hasReturnParameter;
		private bool foundReturnParameter = false;
		private IList<string> assignedParameterNames = new List<string>();

		public SqlStringFormatter(ISqlParameterFormatter formatter, string multipleQueriesSeparator)
		{
			this.formatter = formatter;
			this.multipleQueriesSeparator = multipleQueriesSeparator;
		}

		public void Format(SqlString text)
		{
			hasReturnParameter = DetermineIfSqlStringHasReturnParameter(text);
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

		void ISqlStringVisitor.String(SqlString sqlString)
		{
			result.Append(sqlString.ToString());
		}

		void ISqlStringVisitor.Parameter(Parameter parameter)
		{
			if (hasReturnParameter && !foundReturnParameter)
			{
				result.Append(parameter);
				assignedParameterNames.Add(String.Empty);
				foundReturnParameter = true;
				return;
			}

			// NH: even if using SqlType[] the final commad may have X parameters, with this line we will use Y parameters in the IDbCommand
			// for example the ParameterCollection may contains two parameters called @p0 and @p1 but the command contains just @p0.
			// In this way the same parameter can be used in different places in the query without create a problem to the dear SQL-server (see NH1981)
			// TODO: find a way to have exactly the same amount of parameters between the final IDbCommand and its IDataParameterCollection
			// A candidateplace is making DriverBase.SetCommandParameters a little bit more intelligent... perhaps SqlString aware (see also DriverBase.SetCommandText, DriverBase.GenerateCommand)
			string name = formatter.GetParameterName(parameter.ParameterPosition ?? parameterIndex);

			assignedParameterNames.Add(name);
			parameterIndex++;
			result.Append(name);
		}

		private bool DetermineIfSqlStringHasReturnParameter(SqlString text)
		{
			CallableParser.Detail callableDetail = CallableParser.Parse(text.ToString());
			return (callableDetail.IsCallable && callableDetail.HasReturn);
		}

		public bool HasReturnParameter
		{
			get { return foundReturnParameter; }
		}

		public string[] AssignedParameterNames
		{
			get { return assignedParameterNames.ToArray(); }
		}
	}
}
