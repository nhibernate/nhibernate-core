using System.Collections;
using System.Data;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1383
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void ComponentWithNullable()
		{
			using(ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				IDbCommand cmd = session.Connection.CreateCommand();
				cmd.CommandText =
					"INSERT INTO MyBO(Name, Comp_I1, Comp_I2, NullableComp_I1, NullableComp_I2) VALUES('ABC', null, null, null, null)";
				tx.Enlist(cmd);
				cmd.ExecuteNonQuery();

				tx.Commit();
			}

			using (ISession session = OpenSession())
			{
				IQuery query = session.CreateQuery("FROM MyBO");
				IList li = query.List();
				MyBO bo = (MyBO) li[0];
				Assert.IsNull(bo.Comp, "Comp is NOT null :-(");
				Assert.IsNull(bo.NullableComp, "NullableComp is NOT null :-(");
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("FROM MyBO");
				tx.Commit();
			}
		}
	}
}