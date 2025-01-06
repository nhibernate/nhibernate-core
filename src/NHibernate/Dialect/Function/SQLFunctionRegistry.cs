using System;
using System.Collections.Generic;

namespace NHibernate.Dialect.Function
{
	public class SQLFunctionRegistry
	{
		private readonly Dialect dialect;
		private readonly IDictionary<string, ISQLFunction> userFunctions;
		//Temporary alias support
		private static Dictionary<string, string> _functionAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "secondtruncated", "second" } };

		public SQLFunctionRegistry(Dialect dialect, IDictionary<string, ISQLFunction> userFunctions)
		{
			this.dialect = dialect;
			this.userFunctions = new Dictionary<string, ISQLFunction>(userFunctions,
				StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Find function by function name ignoring case
		/// </summary>
		public ISQLFunction FindSQLFunction(string functionName)
		{
			if (!userFunctions.TryGetValue(functionName, out ISQLFunction result) && !dialect.Functions.TryGetValue(functionName, out result))
			{
				if (_functionAliases.TryGetValue(functionName, out var sqlFunction) && !_functionAliases.ContainsKey(sqlFunction))
					return FindSQLFunction(sqlFunction);
			}
			return result;
		}

		public bool HasFunction(string functionName)
		{
			if (userFunctions.ContainsKey(functionName) || dialect.Functions.ContainsKey(functionName))
				return true;
			if (_functionAliases.TryGetValue(functionName, out var sqlFunction) && !_functionAliases.ContainsKey(sqlFunction))
				return HasFunction(sqlFunction);
			return false;
		}
	}
}
