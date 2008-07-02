using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;

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
			if (discriminatorSchema.column != null)
			{
				Column col = new Column();
				col.Value = discriminator;
				BindColumn(discriminatorSchema, col);
				col.Name = mappings.NamingStrategy.ColumnName(discriminatorSchema.column);

				if (table != null)
					table.AddColumn(col);

				discriminator.AddColumn(col);
			}
			else if (discriminatorSchema.Item != null && discriminatorSchema.Item is HbmColumn)
			{
				HbmColumn theCol = (HbmColumn)discriminatorSchema.Item;
				Column col = new Column();
				col.Value = discriminator;
				BindColumn(theCol, col, false);

				col.Name = mappings.NamingStrategy.ColumnName(theCol.name);

				if (table != null)
					table.AddColumn(col);
				//table=null -> an association, fill it in later

				discriminator.AddColumn(col);

				BindIndex(theCol.index, table, col);
				BindUniqueKey(theCol.uniquekey, table, col);
			}

			if (discriminator.ColumnSpan == 0)
			{
				Column col = new Column();
				col.Value = discriminator;
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
	}
}