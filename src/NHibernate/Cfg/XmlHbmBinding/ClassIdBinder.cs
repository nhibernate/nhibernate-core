using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Type;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class ClassIdBinder : ClassBinder
	{
		public ClassIdBinder(ClassBinder parent) : base(parent) { }

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
				VerifiyIdTypeIsValid(id.Type, rootClass.EntityName);

				new IdGeneratorBinder(Mappings).BindGenerator(id, GetIdGenerator(idSchema));

				id.Table.SetIdentifierValue(id);

				BindUnsavedValue(idSchema, id);
			}
		}

		private void CreateIdentifierProperty(HbmId idSchema, PersistentClass rootClass, SimpleValue id)
		{
			if (idSchema.name != null)
			{
				string access = idSchema.access ?? mappings.DefaultAccess;
				id.SetTypeUsingReflection(rootClass.MappedClass == null ? null : rootClass.MappedClass.AssemblyQualifiedName, idSchema.name, access);

				var property = new Property(id) { Name = idSchema.name };

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

		private HbmGenerator GetIdGenerator(HbmId idSchema)
		{
			return String.IsNullOrEmpty(idSchema.generator1) ? idSchema.generator : new HbmGenerator() { @class = idSchema.generator1 };
		}

		private static void VerifiyIdTypeIsValid(IType idType, string className)
		{
			if (idType == null)
				throw new MappingException(string.Format("Must specify an identifier type: {0}.", className));

			if (idType.ReturnedClass.IsArray)
				throw new MappingException("Illegal use of an array as an identifier (arrays don't reimplement equals).");
		}

		private static void BindUnsavedValue(HbmId idSchema, SimpleValue id)
		{
			id.NullValue = idSchema.unsavedvalue ?? (id.IdentifierGeneratorStrategy == "assigned" ? "undefined" : null);
		}
	}
}