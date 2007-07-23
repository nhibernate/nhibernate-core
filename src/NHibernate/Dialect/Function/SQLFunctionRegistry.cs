using System;
using System.Collections.Generic;

namespace NHibernate.Dialect.Function
{
	public class SQLFunctionRegistry
	{
		private readonly Dialect dialect;
		private readonly IDictionary<string, ISQLFunction> userFunctions;

		public SQLFunctionRegistry(Dialect dialect, IDictionary<string, ISQLFunction> userFunctions)
		{
			this.dialect = dialect;
			this.userFunctions = new HashtableDictionary<string, ISQLFunction>(userFunctions,
				StringComparer.InvariantCultureIgnoreCase);
		}

		public ISQLFunction FindSQLFunction(string functionName)
		{
			return userFunctions[functionName] ?? (ISQLFunction) dialect.Functions[functionName];
		}

		public bool HasFunction(string functionName)
		{
			return userFunctions.ContainsKey(functionName) || dialect.Functions.Contains(functionName);
		}
	}
}