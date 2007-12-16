using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Test.MappingTest
{
	[TestFixture]
	public class ForeignKeyFixture
	{
		[Test]
		[ExpectedException(typeof(FKUnmatchingColumnsException))]
		public void UnmatchingColumns()
		{
			Table primaryTable = new Table("pktable");
			primaryTable.PrimaryKey = new PrimaryKey();
			SimpleValue sv = new SimpleValue();
			sv.TypeName = NHibernateUtil.Int16.Name;
			Column pkColumn = new Column("pk_column");
			pkColumn.Value = sv;

			primaryTable.PrimaryKey.AddColumn(pkColumn);

			Table fkTable = new Table("fktable");

			ForeignKey fk = new ForeignKey();
			sv = new SimpleValue();
			sv.TypeName = NHibernateUtil.Int16.Name;
			Column fkColumn1 = new Column("col1");
			fkColumn1.Value = sv;

			sv = new SimpleValue();
			sv.TypeName = NHibernateUtil.Int16.Name;
			Column fkColumn2 = new Column("col2");
			fkColumn2.Value = sv;

			fk.AddColumn(fkColumn1);
			fk.AddColumn(fkColumn2);

			fk.Table = fkTable;

			fk.ReferencedTable = primaryTable;
			fk.AlignColumns();
		}
	}
}