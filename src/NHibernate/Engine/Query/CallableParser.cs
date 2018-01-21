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

			int indexOfCall = -1;
			callableDetail.IsCallable = sqlString.Length > 5 && // to be able to check sqlString[0] we at least need to make sure that string has at least 1 character. The simplest case all other conditions are true is "{call}" which is 6 characters, so check it.
										sqlString[0] == '{' &&
										sqlString[sqlString.Length - 1] == '}' &&
										(indexOfCall = sqlString.IndexOf("call", StringComparison.Ordinal)) > 0;

			if (!callableDetail.IsCallable)
				return callableDetail;

			Match functionMatch = functionNameFinder.Match(sqlString);

			if ((!functionMatch.Success) || (functionMatch.Groups.Count < 2))
				throw new HibernateException("Could not determine function name for callable SQL: " + sqlString);

			callableDetail.FunctionName = functionMatch.Groups[1].Value;

			callableDetail.HasReturn = HasReturnParameter(sqlString, indexOfCall);

			return callableDetail;
		}

		internal static bool HasReturnParameter(string sqlString, int indexOfCall)
		{
			int indexOfQuestionMark;
			int indexOfEqual;
			return indexOfCall > 0 &&
					(indexOfQuestionMark = sqlString.IndexOf('?')) > 0 &&
					(indexOfEqual = sqlString.IndexOf('=')) > 0 &&
					indexOfQuestionMark < indexOfCall &&
					indexOfEqual < indexOfCall;
		}
	}
}
