using System;

using NHibernate.Sql;
using NUnit.Framework;

namespace NHibernate.UnitTesting.Sql
{
	/// <summary>
	/// Summary description for DeleteTesting.
	/// </summary>
	[TestFixture]
	public class DeleteTesting
	{
		Delete sql;

		[Test]
		public void SimpleTest() {
			sql = new Delete();
			sql.SetTableName( "table1" ).SetPrimaryKeyColumnNames( "pk1" );
			Assertion.AssertEquals("delete from table1 where pk1=?", sql.ToStatementString() );
		}

		[Test]
		public void MultiPrimaryKeyTest() {
			sql = new Delete();
			sql.SetTableName( "table1" ).SetPrimaryKeyColumnNames( "pk1", "pk2", "pk3", "pk4" );
			Assertion.AssertEquals("delete from table1 where pk1=? and pk2=? and pk3=? and pk4=?", sql.ToStatementString() );
		}

		[Test]
		public void PrimaryKeyAndVersionTest() {
			sql = new Delete();
			sql.SetTableName( "table1" ).SetPrimaryKeyColumnNames( "pk1" ).SetVersionColumnName("version");
			Assertion.AssertEquals("delete from table1 where pk1=? and version=?", sql.ToStatementString() );

			sql = new Delete();
			sql.SetTableName( "table1" ).SetPrimaryKeyColumnNames( "pk1", "pk2" ).SetVersionColumnName("version");
			Assertion.AssertEquals("delete from table1 where pk1=? and pk2=? and version=?", sql.ToStatementString() );
		}

		[Test]
		public void WhereTest() {
			sql = new Delete();
			sql.SetTableName( "table1" ).SetPrimaryKeyColumnNames( "pk1" ).SetWhere("otherCol=\"Some value\"");
			Assertion.AssertEquals("delete from table1 where pk1=? and otherCol=\"Some value\"", sql.ToStatementString() );
		}
	}
}
