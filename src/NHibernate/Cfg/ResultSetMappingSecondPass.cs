using System;
using System.Collections.Generic;
using System.Xml;

using NHibernate.Engine;
using NHibernate.Mapping;

namespace NHibernate.Cfg
{
	public class ResultSetMappingSecondPass : ResultSetMappingBinder, IQuerySecondPass
	{
		private XmlNode element;
		private string path;
		private Mappings mappings;

		public ResultSetMappingSecondPass(XmlNode element, String path, Mappings mappings)
		{
			this.element = element;
			this.path = path;
			this.mappings = mappings;
		}

		public void DoSecondPass(IDictionary<System.Type, PersistentClass> persistentClasses)
		{
			ResultSetMappingDefinition definition = BuildResultSetMappingDefinition(element, path, mappings);
			mappings.AddResultSetMapping(definition);
		}
	}
}