using System.Collections.Generic;

using NHibernate.Engine.Query.Sql;
using System;

namespace NHibernate.Engine
{
	[Serializable]
	public class ResultSetMappingDefinition
	{
		private readonly string name;
		private readonly List<INativeSQLQueryReturn> queryReturns = new List<INativeSQLQueryReturn>();

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
			if (queryReturn != null)
			{
				queryReturns.Add(queryReturn);
			}
		}

		public INativeSQLQueryReturn[] GetQueryReturns()
		{
			return queryReturns.ToArray();
		}
	}
}