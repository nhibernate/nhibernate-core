using System;
using System.Text.RegularExpressions;
using NHibernate.Util;
using NHibernate.SqlCommand;

namespace NHibernate.Engine.Query
{
	public static class CallableParser
	{

		public class Detail
		{
			public bool IsCallable;
			public bool HasReturn;
			public string FunctionName;
		}

		private static readonly Regex functionNameFinder = new Regex(@"\{[\S\s]*call[\s]+([\w\.]+)[^\w]");

		public static Detail Parse(string sqlString)
		{
			Detail callableDetail = new Detail();

			callableDetail.IsCallable = sqlString.IndexOf("{") == 0 &&
										sqlString.IndexOf("}") == (sqlString.Length - 1) &&
										sqlString.IndexOf("call") > 0;

			if (!callableDetail.IsCallable)
				return callableDetail;

			Match functionMatch = functionNameFinder.Match(sqlString);

			if ((!functionMatch.Success) || (functionMatch.Groups.Count < 2))
				throw new HibernateException("Could not determine function name for callable SQL: " + sqlString);

			callableDetail.FunctionName = functionMatch.Groups[1].Value;

			callableDetail.HasReturn = sqlString.IndexOf("call") > 0 &&
										sqlString.IndexOf("?") > 0 &&
										sqlString.IndexOf("=") > 0 &&
										sqlString.IndexOf("?") < sqlString.IndexOf("call") &&
										sqlString.IndexOf("=") < sqlString.IndexOf("call");

			return callableDetail;
		}
	}
}