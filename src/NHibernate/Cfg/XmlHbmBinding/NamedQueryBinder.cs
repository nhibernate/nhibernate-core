using System.Collections;

using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class NamedQueryBinder : Binder
	{
		public NamedQueryBinder(Mappings mappings)
			: base(mappings)
		{
		}

		public NamedQueryBinder(Binder parent)
			: base(parent)
		{
		}

		public void AddQuery(HbmQuery querySchema)
		{
			string queryName = querySchema.name;
			string queryText = querySchema.GetText();

			log.DebugFormat("Named query: {0} -> {1}", queryName, queryText);

			bool cacheable = false;
			string region = null;
			int timeout = -1;
			int fetchSize = -1;
			bool readOnly = true;
			string comment = null;
			FlushMode flushMode = FlushModeConverter.GetFlushMode(querySchema);

			IDictionary parameterTypes = new SequencedHashMap();

			NamedQueryDefinition namedQuery = new NamedQueryDefinition(queryText, cacheable, region, timeout,
				fetchSize, flushMode, readOnly, comment, parameterTypes);

			mappings.AddQuery(queryName, namedQuery);
		}
	}
}