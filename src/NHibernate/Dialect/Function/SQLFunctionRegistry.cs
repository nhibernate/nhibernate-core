using System;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.Dialect.Function
{
	public class SQLFunctionRegistry
	{
		private readonly Dialect dialect;
		private readonly IDictionary userFunctions;

		public SQLFunctionRegistry(Dialect dialect, IDictionary userFunctions)
		{
			this.dialect = dialect;
			this.userFunctions = CollectionHelper.CreateCaseInsensitiveHashtable(userFunctions);
		}

		public ISQLFunction FindSQLFunction(string functionName)
		{
			ISQLFunction userFunction = (ISQLFunction) userFunctions[functionName];
			return userFunction != null ? userFunction : (ISQLFunction) dialect.Functions[functionName];
		}

		public bool HasFunction(string functionName)
		{
			bool hasUserFunction = userFunctions.Contains(functionName);
			return hasUserFunction || dialect.Functions.Contains(functionName);
		}
	}
}