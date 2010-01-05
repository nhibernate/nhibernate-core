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

			if (schema != null)
				results.Add(Id.PersistentIdGeneratorParmsNames.Schema, schema);
			else if (mappings.SchemaName != null)
				results.Add(Id.PersistentIdGeneratorParmsNames.Schema, mappings.Dialect.QuoteForSchemaName(mappings.SchemaName));

			foreach (HbmParam paramSchema in generatorMapping.param ?? new HbmParam[0])
				results.Add(paramSchema.name, paramSchema.GetText());

			return results;
		}
	}
}