using System;
using System.Collections;

namespace NHibernate.Dialect.Function
{
	public class SQLFunctionRegistry
	{
		private readonly Dialect dialect;
		private readonly IDictionary userFunctions;

		public SQLFunctionRegistry(Dialect dialect, IDictionary userFunctions)
		{
			this.dialect = dialect;
			this.userFunctions = new Hashtable(userFunctions);
		}

		public ISQLFunction FindSQLFunction(string functionName)
		{
#if NET_2_0
			string name = functionName.ToLowerInvariant();
#else
			string name = functionName.ToLower(System.Globalization.CultureInfo.InvariantCulture);
#endif
			ISQLFunction userFunction = (ISQLFunction)userFunctions[name];
			// TODO: (H3.2 comment)lowercasing done here. Was done "at random" before; maybe not needed at all ?
			return userFunction != null ? userFunction : (ISQLFunction)dialect.Functions[name];
		}

		public bool HasFunction(string functionName)
		{
#if NET_2_0
			string name = functionName.ToLowerInvariant();
#else
			string name = functionName.ToLower(System.Globalization.CultureInfo.InvariantCulture);
#endif
			bool hasUserFunction = userFunctions.Contains(name);
			// TODO: (H3.2 comment)toLowerCase was not done before. Only used in Template.
			return hasUserFunction || dialect.Functions.Contains(name);
		}
	}
}
