using System.Collections;
using System.Collections.Generic;

using NHibernate.Engine.Query.Sql;

namespace NHibernate.Engine
{
	public class ResultSetMappingDefinition
	{
		private readonly string name;
		private readonly IList<INativeSQLQueryReturn> queryReturns = new List<INativeSQLQueryReturn>();

		public ResultSetMappingDefinition(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		public void AddQueryReturn(INativeSQLQueryReturn queryReturn)
		{
			queryReturns.Add(queryReturn);
		}

		public INativeSQLQueryReturn[] GetQueryReturns()
		{
			INativeSQLQueryReturn[] result = new INativeSQLQueryReturn[queryReturns.Count];
			queryReturns.CopyTo(result, 0);
			return result;
		}
	}
}