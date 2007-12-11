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
			Table primaryTable = new Table();
			primaryTable.Name = "pktable";
			primaryTable.PrimaryKey = new PrimaryKey();
			Column pkColumn = new Column(NHibernateUtil.Int16, 0);
			pkColumn.Name = "pk_column";

			primaryTable.PrimaryKey.AddColumn(pkColumn);

			Table fkTable = new Table();
			fkTable.Name = "fktable";

			ForeignKey fk = new ForeignKey();
			Column fkColumn1 = new Column(NHibernateUtil.Int16, 0);
			fkColumn1.Name = "col1";

			Column fkColumn2 = new Column(NHibernateUtil.Int16, 0);
			fkColumn2.Name = "col2";

			fk.AddColumn(fkColumn1);
			fk.AddColumn(fkColumn2);

			fk.Table = fkTable;

			fk.ReferencedTable = primaryTable;
			fk.AlignColumns();
		}
	}
}