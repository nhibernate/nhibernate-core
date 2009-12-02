using System;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class ClassIdBinder : ClassBinder
	{
		public ClassIdBinder(ClassBinder parent)
			: base(parent)
		{
		}

		public void BindId(HbmId idSchema, PersistentClass rootClass, Table table)
		{
			if (idSchema != null)
			{
				var id = new SimpleValue(table);
				new TypeBinder(id, Mappings).Bind(idSchema.Type);

				rootClass.Identifier = id;

				Func<HbmColumn> defaultColumn = () => new HbmColumn
				                                      	{
				                                      		name = idSchema.name ?? RootClass.DefaultIdentifierColumnName,
																									length = idSchema.length
				                                      	};
				new ColumnsBinder(id, Mappings).Bind(idSchema.Columns, false, defaultColumn);

				CreateIdentifierProperty(idSchema, rootClass, id);
				VerifiyIdTypeIsValid(id, rootClass.EntityName);
				BindGenerator(idSchema, id);
				id.Table.SetIdentifierValue(id);
				BindUnsavedValue(idSchema, id);
			}
		}

		private void CreateIdentifierProperty(HbmId idSchema, PersistentClass rootClass, SimpleValue id)
		{
			if (idSchema.name != null)
			{
				string access = idSchema.access ?? mappings.DefaultAccess;
				id.SetTypeUsingReflection(rootClass.MappedClass == null ? null : rootClass.MappedClass.AssemblyQualifiedName,
				                          idSchema.name, access);

				var property = new Property(id) {Name = idSchema.name};

				if (property.Value.Type == null)
					throw new MappingException("could not determine a property type for: " + property.Name);

				property.PropertyAccessorName = idSchema.access ?? mappings.DefaultAccess;
				property.Cascade = mappings.DefaultCascade;
				property.IsUpdateable = true;
				property.IsInsertable = true;
				property.IsOptimisticLocked = true;
				property.Generation = PropertyGeneration.Never;
				property.MetaAttributes = GetMetas(idSchema, EmptyMeta);

				rootClass.IdentifierProperty = property;

				property.LogMapped(log);
			}
		}

		private static void VerifiyIdTypeIsValid(IValue id, string className)
		{
			if (id.Type == null)
				throw new MappingException(string.Format("Must specify an identifier type: {0}.", className));

			if (id.Type.ReturnedClass.IsArray)
				throw new MappingException(
					"Illegal use of an array as an identifier (arrays don't reimplement equals).");
		}

		private void BindGenerator(HbmId idSchema, SimpleValue id)
		{
			if (idSchema.generator != null)
			{
				if (idSchema.generator.@class == null)
					throw new MappingException("no class given for generator");

				// NH Differen behavior : specific feature NH-1817
				TypeDef typeDef = mappings.GetTypeDef(idSchema.generator.@class);
				if (typeDef != null)
				{
					id.IdentifierGeneratorStrategy = typeDef.TypeClass;
					// parameters on the property mapping should override parameters in the typedef
					var allParameters = new Dictionary<string, string>(typeDef.Parameters);
					ArrayHelper.AddAll(allParameters, GetGeneratorProperties(idSchema, id));

					id.IdentifierGeneratorProperties = allParameters;
				}
				else
				{
					id.IdentifierGeneratorStrategy = idSchema.generator.@class;
					id.IdentifierGeneratorProperties = GetGeneratorProperties(idSchema, id);
				}
			}
		}

		private IDictionary<string,string> GetGeneratorProperties(HbmId idSchema, IValue id)
		{
			var results = new Dictionary<string, string>();

			if (id.Table.Schema != null)
				results.Add(Id.PersistentIdGeneratorParmsNames.Schema, id.Table.Schema);
			else if (mappings.SchemaName != null)
				results.Add(Id.PersistentIdGeneratorParmsNames.Schema, dialect.QuoteForSchemaName(mappings.SchemaName));

			foreach (HbmParam paramSchema in idSchema.generator.param ?? new HbmParam[0])
				results.Add(paramSchema.name, paramSchema.GetText());

			return results;
		}

		private static void BindUnsavedValue(HbmId idSchema, SimpleValue id)
		{
			id.NullValue = idSchema.unsavedvalue ?? (id.IdentifierGeneratorStrategy == "assigned" ? "undefined" : null);
		}
	}
}
