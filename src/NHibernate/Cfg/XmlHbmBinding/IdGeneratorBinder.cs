using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class IdGeneratorBinder: Binder
	{
		public IdGeneratorBinder(Mappings mappings) : base(mappings) {}

		public void BindGenerator(SimpleValue id, HbmGenerator generatorMapping)
		{
			if (generatorMapping != null)
			{
				if (generatorMapping.@class == null)
					throw new MappingException("no class given for generator");

				// NH Differen behavior : specific feature NH-1817
				TypeDef typeDef = mappings.GetTypeDef(generatorMapping.@class);
				if (typeDef != null)
				{
					id.IdentifierGeneratorStrategy = typeDef.TypeClass;
					// parameters on the property mapping should override parameters in the typedef
					var allParameters = new Dictionary<string, string>(typeDef.Parameters);
					ArrayHelper.AddAll(allParameters, GetGeneratorProperties(generatorMapping, id.Table.Schema));

					id.IdentifierGeneratorProperties = allParameters;
				}
				else
				{
					id.IdentifierGeneratorStrategy = generatorMapping.@class;
					id.IdentifierGeneratorProperties = GetGeneratorProperties(generatorMapping, id.Table.Schema);
				}
			}
		}

		private IDictionary<string, string> GetGeneratorProperties(HbmGenerator generatorMapping, string schema)
		{
			var results = new Dictionary<string, string>();

			// By default, any tables for the id generator will be in the same schema as
			// the owning entity table. If this isn't specified, grab it from the root
			// mappings element instead. It can also be overriden with a parameter in the
			// <generator> clause itself.

			if (schema != null)
				results[Id.PersistentIdGeneratorParmsNames.Schema] = schema;
			else if (mappings.SchemaName != null)
				results[Id.PersistentIdGeneratorParmsNames.Schema] = mappings.SchemaName;

			if (mappings.PreferPooledValuesLo != null)
				results[Environment.PreferPooledValuesLo] = mappings.PreferPooledValuesLo;

			foreach (HbmParam paramSchema in generatorMapping.param ?? new HbmParam[0])
				results[paramSchema.name] = paramSchema.GetText();

			return results;
		}
	}
}