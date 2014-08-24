using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
namespace NHibernate.Test.NHSpecificTest.NH1405
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Bug()
		{
			string[] populate = new string[]
        	{
						"insert into PPDM_COLUMN ( SYSTEM_ID, TABLE_NAME, COLUMN_NAME, CONTROL_COLUMN ) values ( 'SYSTEM', 'TABLE', 'COLUMN1', null )",
						"insert into PPDM_COLUMN ( SYSTEM_ID, TABLE_NAME, COLUMN_NAME, CONTROL_COLUMN ) values ( 'SYSTEM', 'TABLE', 'COLUMN2', 'COLUMN1' )",
						"insert into PPDM_COLUMN ( SYSTEM_ID, TABLE_NAME, COLUMN_NAME, CONTROL_COLUMN ) values ( 'SYSTEM', 'TABLE', 'COLUMN3', 'COLUMN2' )"
        	};

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				foreach (string sql in populate)
				{
					IDbCommand cmd = session.Connection.CreateCommand();
					cmd.CommandText = sql;
					tx.Enlist(cmd);
					cmd.ExecuteNonQuery();
				}
				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				IQuery query = session.CreateQuery("from Column");
				IList<Column> columns = query.List<Column>();
				Assert.AreEqual(3, columns.Count);
				foreach (Column column in columns)
				{
					Assert.IsNotNull(column.ColumnName, "Column.ColumnName should not be null.");
					Assert.IsFalse((null != column.ControlColumn) && (null == column.ControlColumn.ColumnName),
					               "Column's control column's ColumnName should not be null.");
				}
				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				IDbCommand cmd = session.Connection.CreateCommand();
				cmd.CommandText = "DELETE FROM PPDM_COLUMN";
				tx.Enlist(cmd);
				cmd.ExecuteNonQuery();
				tx.Commit();
			}
		}
	}
}