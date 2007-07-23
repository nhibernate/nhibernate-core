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
			this.userFunctions = new Dictionary<string, ISQLFunction>(userFunctions,
				StringComparer.InvariantCultureIgnoreCase);
		}

		public ISQLFunction FindSQLFunction(string functionName)
		{
			if (userFunctions.ContainsKey(functionName))
				return userFunctions[functionName];
			else
				return (ISQLFunction) dialect.Functions[functionName];
		}

		public bool HasFunction(string functionName)
		{
			return userFunctions.ContainsKey(functionName) || dialect.Functions.Contains(functionName);
		}
	}
}
