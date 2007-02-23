using System;
using System.Collections;
using NHibernate.Loader.Custom;

namespace NHibernate.Engine
{
	public class ResultSetMappingDefinition
	{
		private string name;
		private IList queryReturns = new ArrayList();

		public ResultSetMappingDefinition(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		public void AddQueryReturn(ISQLQueryReturn queryReturn)
		{
			queryReturns.Add(queryReturn);
		}

		public ISQLQueryReturn[] GetQueryReturns()
		{
			ISQLQueryReturn[] result = new ISQLQueryReturn[queryReturns.Count];
			queryReturns.CopyTo(result, 0);
			return result;
		}
	}
}