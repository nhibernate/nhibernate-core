using System;
using System.Collections;
using System.Xml;
using NHibernate.Engine;

namespace NHibernate.Cfg
{
	public class ResultSetMappingSecondPass : ResultSetMappingBinder, IQuerySecondPass
	{
		private readonly XmlNode element;
		private readonly string path;
		private readonly Mappings mappings;

		public ResultSetMappingSecondPass(XmlNode element, String path, Mappings mappings, XmlNamespaceManager nsmgr)
			: base(nsmgr)
		{
			this.element = element;
			this.path = path;
			this.mappings = mappings;
		}

		public void DoSecondPass(IDictionary persistentClasses)
		{
			ResultSetMappingDefinition definition = BuildResultSetMappingDefinition(element, path, mappings);
			mappings.AddResultSetMapping(definition);
		}
	}
}