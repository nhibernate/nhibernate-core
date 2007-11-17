using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Type;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class ClassDiscriminatorBinder : ClassBinder
	{
		public ClassDiscriminatorBinder(ClassBinder parent)
			: base(parent)
		{
		}

		public void BindDiscriminator(HbmDiscriminator discriminatorSchema, PersistentClass rootClass,
			Table table)
		{
			if (discriminatorSchema == null)
				return;

			//DISCRIMINATOR
			SimpleValue discriminator = new SimpleValue(table);
			rootClass.Discriminator = discriminator;
			BindSimpleValue(discriminatorSchema, discriminator);

			if (discriminator.Type == null)
			{
				discriminator.Type = NHibernateUtil.String;
				foreach (Column col in discriminator.ColumnIterator)
				{
					col.Type = NHibernateUtil.String;
					break;
				}
			}

			rootClass.IsPolymorphic = true;

			if (discriminatorSchema.force)
				rootClass.IsForceDiscriminator = true;

			if (discriminatorSchema.insertSpecified && !discriminatorSchema.insert)
				rootClass.IsDiscriminatorInsertable = false;
		}

		private void BindSimpleValue(HbmDiscriminator discriminatorSchema, SimpleValue discriminator)
		{
			discriminator.Type = GetType(discriminatorSchema);

			if (discriminatorSchema.formula != null)
			{
				Formula f = new Formula();
				f.FormulaString = discriminatorSchema.formula;
				discriminator.AddFormula(f);
			}
			else
				BindColumns(discriminatorSchema, discriminator);
		}

		private void BindColumns(HbmDiscriminator discriminatorSchema, SimpleValue discriminator)
		{
			Table table = discriminator.Table;

			//COLUMN(S)
			if (discriminatorSchema.column1 != null)
			{
				Column col = new Column(discriminator.Type, 0);
				BindColumn(discriminatorSchema, col);
				col.Name = mappings.NamingStrategy.ColumnName(discriminatorSchema.column1);

				if (table != null)
					table.AddColumn(col);

				discriminator.AddColumn(col);
			}
			else if (discriminatorSchema.column != null)
			{
				Column col = new Column(discriminator.Type, 0);
				BindColumn(discriminatorSchema.column, col, false);

				col.Name = mappings.NamingStrategy.ColumnName(discriminatorSchema.column.name);

				if (table != null)
					table.AddColumn(col);
				//table=null -> an association, fill it in later

				discriminator.AddColumn(col);

				BindIndex(discriminatorSchema.column.index, table, col);
				BindUniqueKey(discriminatorSchema.column.uniquekey, table, col);
			}

			if (discriminator.ColumnSpan == 0)
			{
				Column col = new Column(discriminator.Type, 0);
				BindColumn(discriminatorSchema, col);

				col.Name = mappings.NamingStrategy.PropertyToColumnName(
					RootClass.DefaultDiscriminatorColumnName);

				discriminator.Table.AddColumn(col);
				discriminator.AddColumn(col);
			}
		}

		private static void BindColumn(HbmDiscriminator discriminatorSchema, Column column)
		{
			if (discriminatorSchema.length != null)
				column.Length = int.Parse(discriminatorSchema.length);

			column.IsNullable = !discriminatorSchema.notnull;
			column.IsUnique = false;
			column.CheckConstraint = string.Empty;
			column.SqlType = null;
		}

		private static IType GetType(HbmDiscriminator discriminatorSchema)
		{
			if (discriminatorSchema.type == null)
				return null;

			IType type = TypeFactory.HeuristicType(discriminatorSchema.type, null);

			if (type == null)
				throw new MappingException("could not interpret type: " + discriminatorSchema.type);

			return type;
		}
	}
}