using System.Collections;

using log4net;

using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class FilterDefinitionFactory
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (FilterDefinitionFactory));

		public static FilterDefinition CreateFilterDefinition(HbmFilterDef filterDefSchema)
		{
			log.DebugFormat("Parsing filter-def [{0}]", filterDefSchema.name);

			string defaultCondition = filterDefSchema.GetDefaultCondition();
			Hashtable parameterTypes = GetFilterParameterTypes(filterDefSchema);

			log.DebugFormat("Parsed filter-def [{0}]", filterDefSchema.name);

			return new FilterDefinition(filterDefSchema.name, defaultCondition, parameterTypes);
		}

		private static Hashtable GetFilterParameterTypes(HbmFilterDef filterDefSchema)
		{
			Hashtable parameterTypes = new Hashtable();

			foreach (HbmFilterParam paramSchema in filterDefSchema.ListParameters())
			{
				log.DebugFormat("Adding filter parameter : {0} -> {1}", paramSchema.name, paramSchema.type);

				IType heuristicType = TypeFactory.HeuristicType(paramSchema.type);

				log.DebugFormat("Parameter heuristic type : {0}", heuristicType);

				parameterTypes.Add(paramSchema.name, heuristicType);
			}

			return parameterTypes;
		}
	}
}