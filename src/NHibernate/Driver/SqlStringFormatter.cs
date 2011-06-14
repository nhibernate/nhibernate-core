using System;
using System.Collections.Generic;
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

		private readonly Dictionary<int, int> queryIndexToNumberOfPreceedingParameters = new Dictionary<int, int>();
		private readonly Dictionary<int, int> parameterIndexToQueryIndex = new Dictionary<int, int>();

		private bool hasReturnParameter = false;
		private bool foundReturnParameter = false;

		public SqlStringFormatter(ISqlParameterFormatter formatter, string multipleQueriesSeparator)
		{
			this.formatter = formatter;
			this.multipleQueriesSeparator = multipleQueriesSeparator;
		}

		public void Format(SqlString text)
		{
			DetermineNumberOfPreceedingParametersForEachQuery(text);
			foundReturnParameter = false;
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
				foundReturnParameter = true;
				return;
			}

			// NH: even if using SqlType[] the final commad may have X parameters, with this line we will use Y parameters in the IDbCommand
			// for example the ParameterCollection may contains two parameters called @p0 and @p1 but the command contains just @p0.
			// In this way the same parameter can be used in different places in the query without create a problem to the dear SQL-server (see NH1981)
			// TODO: find a way to have exactly the same amount of parameters between the final IDbCommand and its IDataParameterCollection
			// A candidateplace is making DriverBase.SetCommandParameters a little bit more intelligent... perhaps SqlString aware (see also DriverBase.SetCommandText, DriverBase.GenerateCommand)
			string name = formatter.GetParameterName(parameter.ParameterPosition ?? parameterIndex);

			parameterIndex++;
			result.Append(name);
		}

		private void DetermineNumberOfPreceedingParametersForEachQuery(SqlString text)
		{
			// NH: this code smell very bad. It look like specific for ORACLE and probably unused even for ORACLE
			int currentParameterIndex = 0;
			int currentQueryParameterCount = 0;
			int currentQueryIndex = 0;
			hasReturnParameter = false;
			foundReturnParameter = false;

			CallableParser.Detail callableDetail = CallableParser.Parse(text.ToString());

			if (callableDetail.IsCallable && callableDetail.HasReturn)
				hasReturnParameter = true;

			foreach (object part in text.Parts)
			{
				if (part.ToString().Equals(multipleQueriesSeparator))
				{
					queryIndexToNumberOfPreceedingParameters[currentQueryIndex] = currentParameterIndex - currentQueryParameterCount;
					currentQueryParameterCount = 0;
					currentQueryIndex++;
					continue;
				}

				Parameter parameter = part as Parameter;

				if (parameter != null)
				{
					if (hasReturnParameter && !foundReturnParameter)
					{
						foundReturnParameter = true;
					}
					else
					{
						parameterIndexToQueryIndex[currentParameterIndex] = currentQueryIndex;
					}
					currentQueryParameterCount++;
					currentParameterIndex++;
				}
			}
		}
	}
}
