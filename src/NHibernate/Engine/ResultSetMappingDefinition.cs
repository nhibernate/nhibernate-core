using System;
using System.Collections.ObjectModel;
using NHibernate.Engine.Query.Sql;


namespace NHibernate.Engine
{
	[Serializable]
	public class ResultSetMappingDefinition
	{
		private readonly string name;
		private readonly ReadOnlyCollection<INativeSQLQueryReturn> queryReturns;

		public ResultSetMappingDefinition(string name, INativeSQLQueryReturn[] queryReturns)
		{
			if (queryReturns == null)
			{
				throw new ArgumentNullException(nameof(queryReturns));
			}

			this.name = name;
			this.queryReturns = new ReadOnlyCollection<INativeSQLQueryReturn>(queryReturns);
		}

		public string Name
		{
			get { return name; }
		}

		public ReadOnlyCollection<INativeSQLQueryReturn> QueryReturns
		{
			get { return queryReturns; }
		}
	}
}