using System.Collections.Generic;

using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class FilterDefinitionFactory
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (FilterDefinitionFactory));

		public static FilterDefinition CreateFilterDefinition(HbmFilterDef filterDefSchema)
		{
			log.DebugFormat("Parsing filter-def [{0}]", filterDefSchema.name);

			string defaultCondition = filterDefSchema.GetDefaultCondition();
			IDictionary<string, IType> parameterTypes = GetFilterParameterTypes(filterDefSchema);

			log.DebugFormat("Parsed filter-def [{0}]", filterDefSchema.name);

			return new FilterDefinition(filterDefSchema.name, defaultCondition, parameterTypes, filterDefSchema.usemanytoone);
		}

		private static IDictionary<string, IType> GetFilterParameterTypes(HbmFilterDef filterDefSchema)
		{
			Dictionary<string, IType> parameterTypes = new Dictionary<string, IType>();

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