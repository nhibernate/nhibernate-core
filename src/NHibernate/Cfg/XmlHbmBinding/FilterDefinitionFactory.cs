using System.Collections.Generic;

using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class FilterDefinitionFactory
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(FilterDefinitionFactory));

		public static FilterDefinition CreateFilterDefinition(HbmFilterDef filterDefSchema)
		{
			log.Debug("Parsing filter-def [{0}]", filterDefSchema.name);

			string defaultCondition = filterDefSchema.GetDefaultCondition();
			IDictionary<string, IType> parameterTypes = GetFilterParameterTypes(filterDefSchema);

			log.Debug("Parsed filter-def [{0}]", filterDefSchema.name);

			return new FilterDefinition(filterDefSchema.name, defaultCondition, parameterTypes, filterDefSchema.usemanytoone);
		}

		private static IDictionary<string, IType> GetFilterParameterTypes(HbmFilterDef filterDefSchema)
		{
			Dictionary<string, IType> parameterTypes = new Dictionary<string, IType>();

			foreach (HbmFilterParam paramSchema in filterDefSchema.ListParameters())
			{
				log.Debug("Adding filter parameter : {0} -> {1}", paramSchema.name, paramSchema.type);

				IType heuristicType = TypeFactory.HeuristicType(paramSchema.type);

				log.Debug("Parameter heuristic type : {0}", heuristicType);

				parameterTypes.Add(paramSchema.name, heuristicType);
			}

			return parameterTypes;
		}
	}
}
