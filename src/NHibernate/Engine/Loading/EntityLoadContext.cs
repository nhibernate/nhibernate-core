using System.Collections;
using System.Data;
using log4net;

namespace NHibernate.Engine.Loading
{
	public class EntityLoadContext
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(EntityLoadContext));
		private LoadContexts loadContexts;
		private readonly IDataReader resultSet;
		private readonly IList hydratingEntities = new ArrayList(20); // todo : need map? the prob is a proper key, right?

		public EntityLoadContext(LoadContexts loadContexts, IDataReader resultSet)
		{
			this.loadContexts = loadContexts;
			this.resultSet = resultSet;
		}

		internal void Cleanup()
		{
			if (!(hydratingEntities.Count == 0))
			{
				log.Warn("On CollectionLoadContext#clear, hydratingEntities contained [" + hydratingEntities.Count + "] entries");
			}
			hydratingEntities.Clear();
		}

		public override string ToString()
		{
			return base.ToString() + "<rs=" + resultSet + ">";
		}
	}
}
