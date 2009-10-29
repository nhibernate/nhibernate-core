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

			string name;

			if (queryIndexToNumberOfPreceedingParameters.Count == 0)
			{
				// there's only one query... no need to worry about indexes of parameters of previous queries
				name = formatter.GetParameterName(parameter.OriginalPositionInQuery ?? parameterIndex);
			}
			else
			{
				// multiple queries... in case the parameters were switched around (for SQL paging for instance) we need
				// to keep the number of preceeding parameters (in previous queries of the batch) into account
				if (parameter.OriginalPositionInQuery != null)
				{
					name = formatter.GetParameterName(GetNumberOfPreceedingParameters() + parameter.OriginalPositionInQuery.Value);
				}
				else
				{
					name = formatter.GetParameterName(parameterIndex);
				}
			}

			parameterIndex++;
			result.Append(name);
		}

		private int GetNumberOfPreceedingParameters() 
		{
			int queryIndex = parameterIndexToQueryIndex[parameterIndex];
			return queryIndexToNumberOfPreceedingParameters[queryIndex];
		}

		private void DetermineNumberOfPreceedingParametersForEachQuery(SqlString text)
		{
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
