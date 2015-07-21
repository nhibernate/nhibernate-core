using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class ClassDiscriminatorBinder: Binder
	{
		private readonly PersistentClass rootClass;

		public ClassDiscriminatorBinder(PersistentClass rootClass, Mappings mappings) : base(mappings)
		{
			this.rootClass = rootClass;
		}

		public void BindDiscriminator(HbmDiscriminator discriminatorSchema, Table table)
		{
			if (discriminatorSchema == null)
				return;

			//DISCRIMINATOR
			var discriminator = new SimpleValue(table);
			rootClass.Discriminator = discriminator;
			BindSimpleValue(discriminatorSchema, discriminator);

			if (discriminator.Type == null)
			{
				discriminator.TypeName = NHibernateUtil.String.Name;
			}

			rootClass.IsPolymorphic = true;
			rootClass.IsForceDiscriminator = discriminatorSchema.force;
			rootClass.IsDiscriminatorInsertable = discriminatorSchema.insert;
		}

		private void BindSimpleValue(HbmDiscriminator discriminatorSchema, SimpleValue discriminator)
		{
			if (discriminatorSchema.type != null)
				discriminator.TypeName = discriminatorSchema.type;

			if (discriminatorSchema.formula != null)
			{
				var f = new Formula {FormulaString = discriminatorSchema.formula};
				discriminator.AddFormula(f);
			}
			else
			{
				new ColumnsBinder(discriminator, Mappings).Bind(discriminatorSchema.Columns, false,
				                                                () =>
				                                                new HbmColumn
				                                                	{
				                                                		name =
				                                                			mappings.NamingStrategy.PropertyToColumnName(
				                                                			RootClass.DefaultDiscriminatorColumnName),
				                                                		length = discriminatorSchema.length,
																														notnull = discriminatorSchema.notnull,
																														notnullSpecified = true
				                                                	});
			}
		}
	}
}